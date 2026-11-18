import { invoke } from '@tauri-apps/api/core';
import type { GeneratorConfig } from '../stores/generatorStore';
import { useModelsStore, type CustomModel } from '../stores/modelsStore';
import { useStylesStore } from '../stores/stylesStore';
import { artStyles, customStyleToArtStyle } from '../constants/artStyles';
import { isTauri } from './saveImage';

const PROXY_URL = 'https://t2i.manh9011.workers.dev/';

export interface GenerateImageOptions {
    throwOnError?: boolean;
}

interface NativeHttpResponse {
    status: number;
    statusText: string;
    ok: boolean;
    contentType: string;
    bodyBase64: string;
    bodyText: string | null;
}

interface HttpResult {
    status: number;
    statusText: string;
    ok: boolean;
    contentType: string;
    bodyBase64: string;
    bodyText: string | null;
}

function resolveStyle(artStyleName: string) {
    if (artStyles[artStyleName]) {
        return artStyles[artStyleName];
    }
    const stylesStore = useStylesStore();
    const custom = stylesStore.getStyleByName(artStyleName);
    if (custom) {
        return customStyleToArtStyle(custom);
    }
    return artStyles['No Style'];
}

function computeAspectRatio(width: number, height: number): '1:1' | '16:9' | '9:16' {
    if (width === height) return '1:1';
    return width > height ? '16:9' : '9:16';
}

function resolveEffectiveSeed(seed: number): number {
    if (seed < 0) {
        return Math.floor(Math.random() * 2147483647);
    }
    return seed;
}

function buildPlaceholderMap(
    config: GeneratorConfig,
    prompt: string,
    negative: string
): Record<string, string> {
    const ratio = computeAspectRatio(config.width, config.height);
    const seed = resolveEffectiveSeed(config.seed);
    const size = `${config.width}x${config.height}`;

    return {
        prompt,
        positive_prompt: prompt,
        negative,
        negative_prompt: negative,
        cfg: String(config.guidanceScale),
        guidance_scale: String(config.guidanceScale),
        seed: String(seed),
        ratio,
        aspect_ratio: ratio,
        width: String(config.width),
        height: String(config.height),
        size,
    };
}

function replacePlaceholders(
    template: string,
    placeholders: Record<string, string>,
    mode: 'json' | 'url' | 'header' | 'plain'
): string {
    if (!template) return '';
    return template.replace(
        /\{(prompt|positive_prompt|negative|negative_prompt|cfg|guidance_scale|seed|ratio|aspect_ratio|width|height|size)\}/gi,
        (match, rawKey: string) => {
            const key = rawKey.toLowerCase();
            const val = placeholders[key];
            if (val === undefined) return match;

            if (mode === 'json') {
                // Keep numeric strings unquoted-safe and escape text strings for JSON string literals
                return JSON.stringify(val).slice(1, -1);
            }
            if (mode === 'url') {
                if (key === 'prompt' || key === 'positive_prompt' || key === 'negative' || key === 'negative_prompt') {
                    return encodeURIComponent(val);
                }
                return val;
            }
            if (mode === 'header') {
                return val.replace(/[\r\n]+/g, ' ');
            }
            return val;
        }
    );
}

function parseHeaders(
    headersTemplate: string,
    placeholders: Record<string, string>
): Record<string, string> {
    const trimmed = headersTemplate.trim();
    if (!trimmed) return {};

    const result: Record<string, string> = {};

    // Try JSON object first
    if (trimmed.startsWith('{')) {
        const replaced = replacePlaceholders(trimmed, placeholders, 'json');
        try {
            const parsed = JSON.parse(replaced);
            if (parsed && typeof parsed === 'object' && !Array.isArray(parsed)) {
                for (const [k, v] of Object.entries(parsed)) {
                    if (k.trim() && v !== undefined && v !== null) {
                        result[k.trim()] = String(v).replace(/[\r\n]+/g, ' ');
                    }
                }
                return result;
            }
        } catch {
            // Fall through to line-by-line parsing
        }
    }

    // Line-by-line "Key: Value"
    for (const line of trimmed.split(/\r?\n/)) {
        const colonIdx = line.indexOf(':');
        if (colonIdx === -1) continue;
        const key = line.slice(0, colonIdx).trim();
        const rawVal = line.slice(colonIdx + 1).trim();
        if (!key) continue;
        result[key] = replacePlaceholders(rawVal, placeholders, 'header');
    }

    return result;
}

function arrayBufferToBase64(buffer: ArrayBuffer): string {
    const bytes = new Uint8Array(buffer);
    const chunkSize = 0x8000;
    let binary = '';
    for (let i = 0; i < bytes.length; i += chunkSize) {
        const chunk = bytes.subarray(i, i + chunkSize);
        binary += String.fromCharCode(...chunk);
    }
    return btoa(binary);
}

function base64ToBlob(base64: string, mimeType = 'image/png'): Blob {
    const cleaned = base64.replace(/\s+/g, '');
    const binary = atob(cleaned);
    const len = binary.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return new Blob([bytes], { type: mimeType });
}

function inferMimeFromBase64(base64: string): string {
    const s = base64.trimStart();
    if (s.startsWith('iVBOR')) return 'image/png';
    if (s.startsWith('/9j/')) return 'image/jpeg';
    if (s.startsWith('R0lGOD')) return 'image/gif';
    if (s.startsWith('UklGR')) return 'image/webp';
    if (s.startsWith('PHN2Z')) return 'image/svg+xml';
    return 'image/png';
}

async function performHttpRequest(params: {
    url: string;
    method: string;
    headers: Record<string, string>;
    body?: string;
}): Promise<HttpResult> {
    if (isTauri()) {
        const res = await invoke<NativeHttpResponse>('custom_http_request', {
            request: {
                url: params.url,
                method: params.method,
                headers: params.headers,
                body: params.body ?? null,
            },
        });
        return res;
    }

    const fetchInit: RequestInit = {
        method: params.method,
        headers: params.headers,
    };
    if (params.body && params.method !== 'GET' && params.method !== 'HEAD') {
        fetchInit.body = params.body;
    }

    const response = await fetch(params.url, fetchInit);
    const contentType = response.headers.get('content-type') || '';
    const buffer = await response.arrayBuffer();
    const bodyBase64 = arrayBufferToBase64(buffer);

    let bodyText: string | null = null;
    const ctLower = contentType.toLowerCase();
    if (
        ctLower.includes('json') ||
        ctLower.includes('text') ||
        ctLower.includes('xml') ||
        ctLower.includes('html') ||
        buffer.byteLength < 512_000
    ) {
        try {
            bodyText = new TextDecoder().decode(buffer);
        } catch {
            bodyText = null;
        }
    }

    return {
        status: response.status,
        statusText: response.statusText,
        ok: response.ok,
        contentType,
        bodyBase64,
        bodyText,
    };
}

function getValueByPath(obj: unknown, path: string): unknown {
    if (obj === null || obj === undefined || !path.trim()) return undefined;
    const tokens = path
        .trim()
        .replace(/\[(['"]?)([^'"\]]+)\1\]/g, '.$2')
        .replace(/^\./, '')
        .split('.')
        .filter(Boolean);

    let current: any = obj;
    for (const token of tokens) {
        if (current === null || current === undefined) return undefined;
        if (Array.isArray(current)) {
            const idx = Number(token);
            if (!Number.isInteger(idx) || idx < 0 || idx >= current.length) return undefined;
            current = current[idx];
        } else if (typeof current === 'object') {
            current = current[token];
        } else {
            return undefined;
        }
    }
    return current;
}

export function extractFieldByPath(obj: unknown, configuredPath: string, fallbackPaths: string[] = []): unknown {
    const candidates = [
        ...configuredPath.split(',').map(p => p.trim()).filter(Boolean),
        ...fallbackPaths,
    ];

    for (const candidate of candidates) {
        const val = getValueByPath(obj, candidate);
        if (val !== undefined && val !== null && val !== '') {
            return val;
        }
    }
    return undefined;
}

function formatErrorValue(val: unknown): string {
    if (val === undefined || val === null) return '';
    if (typeof val === 'string') return val;
    if (typeof val === 'number' || typeof val === 'boolean') return String(val);
    if (Array.isArray(val)) {
        return val
            .map(item => {
                if (item && typeof item === 'object' && 'msg' in item) {
                    const loc = Array.isArray((item as any).loc) ? (item as any).loc.join('.') : '';
                    return loc ? `${loc}: ${(item as any).msg}` : String((item as any).msg);
                }
                return formatErrorValue(item);
            })
            .filter(Boolean)
            .join('; ');
    }
    if (typeof val === 'object') {
        const obj = val as Record<string, unknown>;
        if (typeof obj.message === 'string') return obj.message;
        if (typeof obj.msg === 'string') return obj.msg;
        if (typeof obj.detail === 'string') return obj.detail;
        try {
            return JSON.stringify(val);
        } catch {
            return String(val);
        }
    }
    return String(val);
}

function escapeXml(text: string): string {
    return text
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function wrapTextLines(message: string, maxCharsPerLine = 58, maxLines = 18): string[] {
    const rawLines = (message || 'Unknown error').replace(/\r\n/g, '\n').split('\n');
    const wrapped: string[] = [];

    for (const rawLine of rawLines) {
        const line = rawLine.trim();
        if (!line) {
            if (wrapped.length > 0 && wrapped[wrapped.length - 1] !== '') {
                wrapped.push('');
            }
            continue;
        }

        const words = line.split(/\s+/);
        let current = '';

        for (const word of words) {
            if (word.length > maxCharsPerLine) {
                if (current) {
                    wrapped.push(current);
                    current = '';
                }
                for (let i = 0; i < word.length; i += maxCharsPerLine) {
                    wrapped.push(word.slice(i, i + maxCharsPerLine));
                }
                continue;
            }

            const candidate = current ? `${current} ${word}` : word;
            if (candidate.length <= maxCharsPerLine) {
                current = candidate;
            } else {
                wrapped.push(current);
                current = word;
            }
        }
        if (current) {
            wrapped.push(current);
        }
    }

    if (wrapped.length === 0) {
        return ['Unknown error occurred'];
    }
    if (wrapped.length > maxLines) {
        const sliced = wrapped.slice(0, maxLines);
        sliced[maxLines - 1] = sliced[maxLines - 1].slice(0, Math.max(0, maxCharsPerLine - 3)) + '...';
        return sliced;
    }
    return wrapped;
}

export function buildErrorSvgBlob(
    errorMessage: string,
    meta?: { modelName?: string; status?: number }
): Blob {
    const lines = wrapTextLines(errorMessage, 58, 18);
    const title = meta?.status ? `Request Failed (HTTP ${meta.status})` : 'Generation Error';
    const modelLabel = meta?.modelName ? `Model: ${meta.modelName}` : 'Custom / Built-in API';
    const lineHeight = 24;
    const textStartY = 238;

    const tspans = lines
        .map((line, idx) => {
            const dy = idx === 0 ? 0 : lineHeight;
            const content = line ? escapeXml(line) : '&#160;';
            return `<tspan x="72" dy="${dy}">${content}</tspan>`;
        })
        .join('');

    const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="800" height="800" viewBox="0 0 800 800">
  <defs>
    <linearGradient id="bgGrad" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0%" stop-color="#141418"/>
      <stop offset="100%" stop-color="#1e1b22"/>
    </linearGradient>
    <linearGradient id="accentGrad" x1="0" y1="0" x2="1" y2="0">
      <stop offset="0%" stop-color="#ef4444"/>
      <stop offset="100%" stop-color="#f97316"/>
    </linearGradient>
  </defs>
  <rect width="800" height="800" fill="url(#bgGrad)"/>
  <rect x="40" y="40" width="720" height="720" rx="20" fill="#1f1f25" stroke="#3f3f46" stroke-width="2"/>
  <rect x="40" y="40" width="720" height="8" rx="4" fill="url(#accentGrad)"/>

  <circle cx="92" cy="108" r="24" fill="#ef4444" fill-opacity="0.18" stroke="#ef4444" stroke-width="2"/>
  <path d="M92 94 L92 111" stroke="#f87171" stroke-width="3.5" stroke-linecap="round"/>
  <circle cx="92" cy="121" r="2.5" fill="#f87171"/>

  <text x="132" y="102" fill="#f8fafc" font-family="system-ui, -apple-system, Segoe UI, Roboto, sans-serif" font-size="24" font-weight="700">${escapeXml(title)}</text>
  <text x="132" y="130" fill="#a1a1aa" font-family="system-ui, -apple-system, Segoe UI, Roboto, sans-serif" font-size="15">${escapeXml(modelLabel)}</text>

  <rect x="60" y="168" width="680" height="552" rx="12" fill="#121216" stroke="#2e2e36" stroke-width="1.5"/>
  <text x="72" y="200" fill="#f87171" font-family="ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace" font-size="14" font-weight="600">ERROR DETAILS</text>
  <line x1="72" y1="212" x2="728" y2="212" stroke="#27272a" stroke-width="1"/>

  <text x="72" y="${textStartY}" fill="#e4e4e7" font-family="ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace" font-size="16">${tspans}</text>
</svg>`;

    return new Blob([svg], { type: 'image/svg+xml;charset=utf-8' });
}

function isLocalFilePath(value: string): boolean {
    const s = value.trim();
    if (/^[a-zA-Z]:[\\/]/.test(s)) return true;
    if (/^\\\\/.test(s)) return true;
    if (/^file:\/\//i.test(s)) return true;
    // Only treat "/" prefixed value as local path when it ends with an image extension
    // so JPEG base64 strings starting with "/9j/" are never misidentified as file paths.
    if (s.startsWith('/') && /\.(png|jpe?g|webp|gif|bmp|svg)$/i.test(s)) return true;
    return false;
}

async function resolveImageValueToBlob(rawValue: string): Promise<Blob> {
    const val = rawValue.trim();
    if (!val) {
        throw new Error('Extracted image field is empty');
    }

    if (/^https?:\/\//i.test(val)) {
        const res = await performHttpRequest({
            url: val,
            method: 'GET',
            headers: {},
        });
        if (!res.ok) {
            throw new Error(`Failed to download image from URL (${res.status} ${res.statusText}): ${val}`);
        }
        const mime = res.contentType && res.contentType.startsWith('image/')
            ? res.contentType.split(';')[0]
            : 'image/png';
        return base64ToBlob(res.bodyBase64, mime);
    }

    if (/^data:/i.test(val)) {
        const commaIdx = val.indexOf(',');
        if (commaIdx === -1) {
            throw new Error('Invalid data URL returned in image field');
        }
        const meta = val.slice(5, commaIdx);
        const dataPart = val.slice(commaIdx + 1);
        const mime = meta.split(';')[0] || 'image/png';
        if (meta.includes(';base64')) {
            return base64ToBlob(dataPart, mime);
        }
        return new Blob([decodeURIComponent(dataPart)], { type: mime });
    }

    if (isLocalFilePath(val)) {
        if (!isTauri()) {
            throw new Error(`API returned local file path (${val}), which cannot be read outside Tauri desktop mode. Set response_format to "b64_json" or "image".`);
        }
        const b64 = await invoke<string>('read_local_file_base64', { path: val });
        const ext = val.split('.').pop()?.toLowerCase();
        const mime = ext === 'jpg' || ext === 'jpeg'
            ? 'image/jpeg'
            : ext === 'webp'
                ? 'image/webp'
                : ext === 'gif'
                    ? 'image/gif'
                    : 'image/png';
        return base64ToBlob(b64, mime);
    }

    // Treat as raw base64 image string
    const cleaned = val.replace(/^["']|["']$/g, '');
    const mime = inferMimeFromBase64(cleaned);
    return base64ToBlob(cleaned, mime);
}

function extractErrorFromBody(
    res: HttpResult,
    errorField: string
): string {
    if (res.bodyText) {
        try {
            const parsed = JSON.parse(res.bodyText);
            const extracted = extractFieldByPath(parsed, errorField, [
                'detail',
                'error.message',
                'error',
                'message',
                'msg',
            ]);
            const formatted = formatErrorValue(extracted);
            if (formatted) return formatted;
            return res.bodyText;
        } catch {
            if (res.bodyText.trim()) {
                return res.bodyText.trim();
            }
        }
    }
    return `HTTP ${res.status}${res.statusText ? ` ${res.statusText}` : ''}`;
}

async function generateWithCustomModel(
    customModel: CustomModel,
    config: GeneratorConfig,
    prompt: string,
    negative: string
): Promise<Blob> {
    const placeholders = buildPlaceholderMap(config, prompt, negative);
    const url = replacePlaceholders(customModel.url, placeholders, 'url');
    const method = (customModel.method || 'POST').toUpperCase();
    const headers = parseHeaders(customModel.headers || '', placeholders);

    let body: string | undefined;
    if (method !== 'GET' && method !== 'HEAD' && customModel.payload.trim()) {
        const rawPayload = customModel.payload.trim();
        const isJsonTemplate = rawPayload.startsWith('{') || rawPayload.startsWith('[');
        body = replacePlaceholders(customModel.payload, placeholders, isJsonTemplate ? 'json' : 'plain');

        const hasContentType = Object.keys(headers).some(k => k.toLowerCase() === 'content-type');
        if (!hasContentType && isJsonTemplate) {
            headers['Content-Type'] = 'application/json';
        }
    }

    const res = await performHttpRequest({ url, method, headers, body });

    if (!res.ok) {
        const msg = extractErrorFromBody(res, customModel.errorField);
        throw Object.assign(new Error(msg), { httpStatus: res.status });
    }

    const ctLower = (res.contentType || '').toLowerCase();
    const treatAsImage =
        customModel.responseType === 'image' && !ctLower.includes('application/json');

    if (treatAsImage || ctLower.startsWith('image/')) {
        if (!res.bodyBase64) {
            throw new Error('Server returned empty image response');
        }
        const mime = ctLower.startsWith('image/') ? res.contentType.split(';')[0] : 'image/png';
        return base64ToBlob(res.bodyBase64, mime);
    }

    // Parse JSON response
    const text = res.bodyText ?? '';
    let parsed: unknown;
    try {
        parsed = JSON.parse(text);
    } catch {
        const preview = text.trim().slice(0, 300) || 'Empty response';
        throw new Error(`Expected JSON response but received: ${preview}`);
    }

    // Check if JSON explicitly carries an error field while HTTP 200
    const explicitError = customModel.errorField
        ? extractFieldByPath(parsed, customModel.errorField)
        : undefined;
    const statusVal = getValueByPath(parsed, 'status');
    const isStatusError = typeof statusVal === 'string' && ['error', 'failed', 'fail'].includes(statusVal.toLowerCase());

    const extractedImage = extractFieldByPath(parsed, customModel.imageField, [
        'b64_json',
        'data[0].b64_json',
        'data[0].url',
        'url',
        'image',
        'local_path',
        'data[0].local_path',
    ]);

    if ((explicitError && !extractedImage) || isStatusError) {
        const errMsg = formatErrorValue(explicitError) || extractErrorFromBody(res, customModel.errorField);
        throw new Error(errMsg);
    }

    if (typeof extractedImage !== 'string' || !extractedImage.trim()) {
        const fallbackErr = formatErrorValue(
            extractFieldByPath(parsed, customModel.errorField, ['detail', 'error.message', 'error', 'message'])
        );
        if (fallbackErr) {
            throw new Error(fallbackErr);
        }
        throw new Error(
            `Image field "${customModel.imageField || '(auto)'}" not found in JSON response`
        );
    }

    return await resolveImageValueToBlob(extractedImage);
}

async function generateWithBuiltInModel(
    config: GeneratorConfig,
    prompt: string,
    negative: string
): Promise<Blob> {
    const params = new URLSearchParams();
    params.append('guidance_scale', config.guidanceScale.toString());
    params.append('negative_prompt', negative);
    params.append('prompt', prompt);
    params.append('quality', 'high');
    params.append('width', config.width.toString());
    params.append('height', config.height.toString());
    params.append('seed', config.seed === -1 ? '-1' : config.seed.toString());
    params.append('model', config.model);

    const url = `${PROXY_URL}?${params.toString()}`;
    console.log(`[API Call] Generating image with model: ${params.get('model')}`);
    const response = await fetch(url);
    if (!response.ok) {
        let detail = `${response.status}: ${response.statusText}`;
        try {
            const text = await response.text();
            if (text) {
                try {
                    const json = JSON.parse(text);
                    const extracted = formatErrorValue(
                        extractFieldByPath(json, 'error.message', ['detail', 'error', 'message'])
                    );
                    if (extracted) detail = extracted;
                } catch {
                    detail = text;
                }
            }
        } catch {
            // ignore read error
        }
        throw Object.assign(new Error(`Proxy responded with ${detail}`), { httpStatus: response.status });
    }
    const contentType = (response.headers.get('content-type') || '').toLowerCase();
    if (contentType.includes('application/json')) {
        const text = await response.text();
        try {
            const json = JSON.parse(text);
            const errMsg = formatErrorValue(
                extractFieldByPath(json, 'error.message', ['detail', 'error', 'message'])
            );
            if (errMsg) {
                throw new Error(errMsg);
            }
        } catch (e) {
            if (e instanceof Error && e.message && !e.message.startsWith('Unexpected token')) {
                throw e;
            }
        }
        throw new Error(text || 'Unexpected JSON response from built-in proxy');
    }
    return await response.blob();
}

export const generateImage = async (
    config: GeneratorConfig,
    options?: GenerateImageOptions
): Promise<Blob> => {
    const style = resolveStyle(config.artStyle);
    const prompt = style.prompt(config);
    const negative = style.negative(config);

    const modelsStore = useModelsStore();
    const customModel = modelsStore.getCustomModelById(config.model);
    const modelName = modelsStore.getModelName(config.model);

    try {
        if (customModel) {
            return await generateWithCustomModel(customModel, config, prompt, negative);
        }
        return await generateWithBuiltInModel(config, prompt, negative);
    } catch (error: any) {
        if (options?.throwOnError) {
            throw error;
        }
        const message = error instanceof Error ? error.message : String(error);
        const status = typeof error?.httpStatus === 'number' ? error.httpStatus : undefined;
        return buildErrorSvgBlob(message, { modelName, status });
    }
};
