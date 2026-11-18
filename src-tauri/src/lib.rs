use base64::Engine;
use serde::{Deserialize, Serialize};
use std::fs;
use std::io::{BufRead, BufReader};
#[cfg(target_os = "windows")]
use std::os::windows::process::CommandExt;
use std::path::{Path, PathBuf};
use std::process::{Command, Stdio};
use std::time::{SystemTime, UNIX_EPOCH};
use tauri::{AppHandle, Emitter, Manager};

#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
struct UpscaleRequest {
    id: String,
    input_base64: String,
    scale: u32,
    model: String,
    tile_size: Option<String>,
    gpu_id: Option<String>,
    threads: Option<String>,
    tta: bool,
}

#[derive(Debug, Serialize)]
struct UpscaleResponse {
    output_base64: String,
}

#[derive(Debug, Serialize, Clone)]
struct ProgressPayload {
    id: String,
    progress: u32,
    message: String,
}

fn extract_progress(line: &str) -> Option<u32> {
    if let Some(percent_pos) = line.find('%') {
        let bytes = line.as_bytes();
        let mut start = percent_pos;
        while start > 0 && (bytes[start - 1].is_ascii_digit() || bytes[start - 1] == b'.') {
            start -= 1;
        }
        if start < percent_pos {
            if let Ok(value) = line[start..percent_pos].parse::<f32>() {
                return Some(value.round().clamp(0.0, 100.0) as u32);
            }
        }
    }
    None
}

fn temp_file_path(prefix: &str, extension: &str) -> PathBuf {
    let stamp = SystemTime::now()
        .duration_since(UNIX_EPOCH)
        .map(|v| v.as_millis())
        .unwrap_or(0);
    let filename = format!("{prefix}-{stamp}.{extension}");
    std::env::temp_dir().join(filename)
}

fn resolve_realesrgan_paths(app: &AppHandle) -> Result<(PathBuf, PathBuf), String> {
    let resource_dir = app
        .path()
        .resource_dir()
        .map_err(|e| format!("Failed to resolve resource dir: {e}"))?;
    let base_dir = resource_dir.join("realesrgan");
    let exe_path = base_dir.join("realesrgan.exe");
    let models_dir = base_dir.join("models");
    if exe_path.exists() {
        return Ok((strip_unc_prefix(&exe_path), strip_unc_prefix(&models_dir)));
    }

    let fallback_base = std::env::current_dir()
        .map_err(|e| format!("Failed to resolve current dir: {e}"))?
        .join("..")
        .join("public")
        .join("realesrgan");
    let fallback_exe = fallback_base.join("realesrgan.exe");
    let fallback_models = fallback_base.join("models");
    if fallback_exe.exists() {
        return Ok((
            strip_unc_prefix(&fallback_exe),
            strip_unc_prefix(&fallback_models),
        ));
    }

    Err(format!(
        "RealESRGAN executable not found at {exe_path:?} or {fallback_exe:?}"
    ))
}

fn strip_unc_prefix(path: &Path) -> PathBuf {
    let value = path.to_string_lossy();
    if let Some(stripped) = value.strip_prefix(r"\\?\") {
        PathBuf::from(stripped)
    } else {
        path.to_path_buf()
    }
}

fn cleanup_temp_file(path: &Path) {
    if let Err(err) = fs::remove_file(path) {
        eprintln!("Failed to remove temp file {path:?}: {err}");
    }
}

#[tauri::command]
async fn run_realesrgan(
    app: AppHandle,
    request: UpscaleRequest,
) -> Result<UpscaleResponse, String> {
    if !cfg!(target_os = "windows") {
        return Err("RealESRGAN upscale is supported on Windows only".to_string());
    }

    let app_handle = app.clone();
    tauri::async_runtime::spawn_blocking(move || {
        let (exe_path, models_dir) = resolve_realesrgan_paths(&app_handle)?;
        let input_path = temp_file_path("realesrgan-input", "png");
        let output_path = temp_file_path("realesrgan-output", "png");

        let input_bytes = base64::engine::general_purpose::STANDARD
            .decode(request.input_base64.as_bytes())
            .map_err(|e| format!("Failed to decode base64 input: {e}"))?;
        fs::write(&input_path, input_bytes)
            .map_err(|e| format!("Failed to write temp input file: {e}"))?;

        let input_path = strip_unc_prefix(&input_path);
        let output_path = strip_unc_prefix(&output_path);
        let models_dir = strip_unc_prefix(&models_dir);

        let mut command = Command::new(&exe_path);
        #[cfg(target_os = "windows")]
        command.creation_flags(0x08000000);
        command
            .arg("-i")
            .arg(&input_path)
            .arg("-o")
            .arg(&output_path)
            .arg("-s")
            .arg(request.scale.to_string())
            .arg("-n")
            .arg(&request.model)
            .arg("-m")
            .arg(models_dir)
            .arg("-f")
            .arg("png");

        if let Some(tile_size) = request.tile_size.as_ref() {
            if !tile_size.trim().is_empty() && tile_size != "0" {
                command.arg("-t").arg(tile_size);
            }
        }
        if let Some(gpu_id) = request.gpu_id {
            if gpu_id != "auto" && !gpu_id.is_empty() {
                command.arg("-g").arg(gpu_id);
            }
        }
        if let Some(threads) = request.threads.as_ref() {
            if !threads.trim().is_empty() {
                command.arg("-j").arg(threads);
            }
        }
        if request.tta {
            command.arg("-x");
        }

        command.stdout(Stdio::piped()).stderr(Stdio::piped());

        println!("[realesrgan] command: {:?}", command);

        let mut child = command
            .spawn()
            .map_err(|e| format!("Failed to launch RealESRGAN: {e}"))?;

        let request_id = request.id.clone();
        let request_id_err = request_id.clone();
        let stdout = child.stdout.take();
        let stderr = child.stderr.take();
        let app_handle_stdout = app_handle.clone();
        let app_handle_stderr = app_handle.clone();

        let stdout_task = std::thread::spawn(move || {
            if let Some(stdout) = stdout {
                let reader = BufReader::new(stdout);
                for line in reader.lines().flatten() {
                    println!("[realesrgan][stdout] {line}");
                    if let Some(progress) = extract_progress(&line) {
                        let _ = app_handle_stdout.emit(
                            "realesrgan-progress",
                            ProgressPayload {
                                id: request_id.clone(),
                                progress,
                                message: line.clone(),
                            },
                        );
                    }
                }
            }
        });

        let stderr_task = std::thread::spawn(move || {
            if let Some(stderr) = stderr {
                let reader = BufReader::new(stderr);
                for line in reader.lines().flatten() {
                    eprintln!("[realesrgan][stderr] {line}");
                    if let Some(progress) = extract_progress(&line) {
                        let _ = app_handle_stderr.emit(
                            "realesrgan-progress",
                            ProgressPayload {
                                id: request_id_err.clone(),
                                progress,
                                message: line.clone(),
                            },
                        );
                    }
                }
            }
        });

        let status = child
            .wait()
            .map_err(|e| format!("Failed to wait for RealESRGAN: {e}"))?;

        let _ = stdout_task.join();
        let _ = stderr_task.join();

        if !status.success() {
            cleanup_temp_file(&input_path);
            cleanup_temp_file(&output_path);
            return Err(format!("RealESRGAN exited with status: {status}"));
        }

        let output_bytes =
            fs::read(&output_path).map_err(|e| format!("Failed to read output file: {e}"))?;
        cleanup_temp_file(&input_path);
        cleanup_temp_file(&output_path);

        let output_base64 = base64::engine::general_purpose::STANDARD.encode(output_bytes);
        Ok(UpscaleResponse { output_base64 })
    })
    .await
    .map_err(|e| format!("RealESRGAN task failed: {e}"))?
}

#[derive(Debug, Serialize)]
struct GpuInfo {
    id: u32,
    name: String,
}

#[tauri::command]
async fn get_realesrgan_gpus(app: AppHandle) -> Result<Vec<GpuInfo>, String> {
    if !cfg!(target_os = "windows") {
        return Ok(vec![]);
    }

    // Try to resolve paths - if this fails, we can't run the exe
    let (exe_path, _) = match resolve_realesrgan_paths(&app) {
        Ok(p) => p,
        Err(_) => return Ok(vec![]), // Return empty if tool not found instead of erroring
    };

    // Run with -i and -o dummy paths and -g -1 (which usually lists GPUs and exits? Or errors?)
    // Using -h is the safest "list and exit"
    let mut cmd = Command::new(&exe_path);
    #[cfg(target_os = "windows")]
    cmd.creation_flags(0x08000000);

    let output = cmd
        .arg("-h")
        .output()
        .map_err(|e| format!("Failed to run RealESRGAN for info: {e}"))?;

    let stdout = String::from_utf8_lossy(&output.stdout);
    let stderr = String::from_utf8_lossy(&output.stderr);

    // Combine both outputs for search
    let full_output = format!("{}\n{}", stdout, stderr);

    let mut gpus = Vec::new();

    for line in full_output.lines() {
        let line = line.trim();
        // Line format typically: "[0] NVIDIA GeForce RTX 3080"
        // Check for start with [
        if let Some(start_bracket) = line.find('[') {
            // ensure it's at start (ignoring some whitespace/control chars?)
            if start_bracket > 2 {
                continue;
            }

            if let Some(end_bracket) = line[start_bracket..].find(']') {
                let absolute_end = start_bracket + end_bracket;
                let id_str = &line[start_bracket + 1..absolute_end];

                if let Ok(id) = id_str.parse::<u32>() {
                    let name = line[absolute_end + 1..].trim();
                    if !name.is_empty() {
                        // Avoid duplicates
                        if !gpus.iter().any(|g: &GpuInfo| g.id == id) {
                            gpus.push(GpuInfo {
                                id,
                                name: name.to_string(),
                            });
                        }
                    }
                }
            }
        }
    }

    Ok(gpus)
}

#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
struct CustomHttpRequest {
    url: String,
    method: String,
    headers: std::collections::HashMap<String, String>,
    body: Option<String>,
}

#[derive(Debug, Serialize)]
#[serde(rename_all = "camelCase")]
struct CustomHttpResponse {
    status: u16,
    status_text: String,
    ok: bool,
    content_type: String,
    body_base64: String,
    body_text: Option<String>,
}

/// Execute an HTTP request from Rust so local/external APIs without CORS headers work in WebView.
#[tauri::command]
async fn custom_http_request(request: CustomHttpRequest) -> Result<CustomHttpResponse, String> {
    let client = tauri_plugin_http::reqwest::Client::builder()
        .timeout(std::time::Duration::from_secs(1800))
        .build()
        .map_err(|e| format!("Failed to create HTTP client: {e}"))?;

    let method_str = request.method.trim().to_uppercase();
    let method = tauri_plugin_http::reqwest::Method::from_bytes(method_str.as_bytes())
        .unwrap_or(tauri_plugin_http::reqwest::Method::POST);

    let mut req_builder = client.request(method.clone(), &request.url);
    for (key, value) in &request.headers {
        if !key.trim().is_empty() {
            req_builder = req_builder.header(key.trim(), value);
        }
    }

    if method != tauri_plugin_http::reqwest::Method::GET
        && method != tauri_plugin_http::reqwest::Method::HEAD
    {
        if let Some(body) = request.body {
            req_builder = req_builder.body(body);
        }
    }

    let response = req_builder
        .send()
        .await
        .map_err(|e| format!("HTTP request failed: {e}"))?;

    let status = response.status();
    let status_code = status.as_u16();
    let status_text = status.canonical_reason().unwrap_or("").to_string();
    let ok = status.is_success();
    let content_type = response
        .headers()
        .get(tauri_plugin_http::reqwest::header::CONTENT_TYPE)
        .and_then(|v| v.to_str().ok())
        .unwrap_or("")
        .to_string();

    let bytes = response
        .bytes()
        .await
        .map_err(|e| format!("Failed to read response body: {e}"))?;

    let body_base64 = base64::engine::general_purpose::STANDARD.encode(&bytes);
    let ct_lower = content_type.to_lowercase();
    let is_text_like = ct_lower.contains("json")
        || ct_lower.contains("text")
        || ct_lower.contains("xml")
        || ct_lower.contains("html");
    let body_text = if is_text_like || bytes.len() < 512_000 {
        String::from_utf8(bytes.to_vec()).ok()
    } else {
        None
    };

    Ok(CustomHttpResponse {
        status: status_code,
        status_text,
        ok,
        content_type,
        body_base64,
        body_text,
    })
}

/// Read a local image file from disk and return its Base64-encoded bytes.
#[tauri::command]
async fn read_local_file_base64(path: String) -> Result<String, String> {
    let trimmed = path.trim();
    let normalized = if let Some(rest) = trimmed.strip_prefix("file:///") {
        if cfg!(target_os = "windows") {
            rest.to_string()
        } else {
            format!("/{rest}")
        }
    } else if let Some(rest) = trimmed.strip_prefix("file://") {
        rest.to_string()
    } else {
        trimmed.to_string()
    };

    let target_path = Path::new(&normalized);
    let bytes = fs::read(target_path)
        .map_err(|e| format!("Failed to read local file '{}': {e}", target_path.display()))?;
    Ok(base64::engine::general_purpose::STANDARD.encode(bytes))
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .plugin(tauri_plugin_fs::init())
        .plugin(tauri_plugin_http::init())
        .plugin(tauri_plugin_dialog::init())
        .invoke_handler(tauri::generate_handler![
            run_realesrgan,
            get_realesrgan_gpus,
            custom_http_request,
            read_local_file_base64
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
