using Microsoft.Web.WebView2.Core;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using static Perchance.ArtStyle;

namespace Perchance
{
    public partial class PerchanceBox : UserControl
    {
        private CancellationTokenSource? cantok;
        private string? token;

        [ComVisible(true)]
        public class SharedObject(PerchanceBox frm)
        {
            public void SetToken(string token)
            {
                frm.token = token;
                frm.cantok!.Cancel();
            }
        }

        public int Thread
        {
            get => thread;
            set
            {
                thread = value;
                lblLabel.Text = "Image " + value;
            }
        }

        private int thread;
        private string? key;
        private string? adAccessCode = "";
        private Configuration? lastCfg;
        private readonly SharedObject shared;
        private readonly HttpClient client = new();

        public async Task Generate(Configuration cfg)
        {
            try
            {
                lastCfg = cfg;

            repeat_1:
                var verifyUrl = $"https://image-generation.perchance.org/api/verifyUser?thread={Thread}&__cacheBust={rand.NextDouble()}";

                var rr = await client.GetAsync(verifyUrl, Global.Cancellation.Token);
                var dt = await rr.Content.ReadFromJsonAsync<Dictionary<string, string>>(Global.Cancellation.Token);
                if (dt == null)
                    return;

                switch (dt["status"])
                {
                    case "already_verified":
                    case "success":
                        key = dt["userKey"];
                        break;
                    default:
                        lblError.BringToFront();
                        lblError.Text = dt["status"] + " " + dt["reason"];

                        cantok = new CancellationTokenSource();

                        wv2Captcha.Source = new Uri("https://image-generation.perchance.org/embed");
                        wv2Captcha.BringToFront();

                        await Task.Run(() => cantok.Token.WaitHandle.WaitOne(), Global.Cancellation.Token);

                        if (token != null)
                        {
                            var tokenVerifyUrl = $"https://image-generation.perchance.org/api/verifyUser?token={token}&__cacheBust={rand.NextDouble()}";

                            var rrr = await client.GetAsync(tokenVerifyUrl, Global.Cancellation.Token);
                            var dtt = await rrr.Content.ReadFromJsonAsync<Dictionary<string, string>>(Global.Cancellation.Token);
                            if (dtt != null && dtt.TryGetValue("userKey", out var value))
                                key = value;
                        }
                        else
                            return;
                        break;
                }

                var style = styles.TryGetValue(cfg.ArtStyle, out var s) ? s : styles["No Style"];

                var repeatCount = 0;
            repeat_2:
                var queryParams = new Dictionary<string, string>
                {
                    { "prompt", style.MakePrompt(cfg) },
                    { "seed", txtSeed.Text },
                    { "resolution", $"{cfg.Width}x{cfg.Height}" },
                    { "guidanceScale", $"{cfg.GuidanceScale}" },
                    { "negativePrompt", style.MakeNegative(cfg) },
                    { "channel", "free-nsfw-ai-generator" },
                    { "subChannel", "public" },
                    { "userKey", key! },
                    { "adAccessCode", adAccessCode! },
                    { "requestId", $"{rand.NextDouble()}" },
                    { "__cacheBust", $"{rand.NextDouble()}" },
                    { "bdf", $"{rand.NextDouble()}" }
                }.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}");

                var apiUrl = $"https://image-generation.perchance.org/api/generate?{string.Join("&", queryParams)}";
                var response = await client.GetAsync(apiUrl, Global.Cancellation.Token);
                var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(Global.Cancellation.Token);

                if (data == null)
                {
                    lblError.BringToFront();
                    lblError.Text = "generate failure !";
                    return;
                }

                var status = data["status"].ToString();
                switch (status)
                {
                    case "success":
                        var imageId = data["imageId"];
                        var downloadUrl = $"https://image-generation.perchance.org/api/downloadTemporaryImage?imageId={imageId}";

                        var imageResponse = await client.GetAsync(downloadUrl, Global.Cancellation.Token);
                        var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync(Global.Cancellation.Token);
                        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history", cfg.Hash, data["seed"] + ".jpg");
                        await File.WriteAllBytesAsync(dir, imageBytes, Global.Cancellation.Token);

                        txtSeed.Text = data["seed"].ToString();
                        ptbImage.BringToFront();
                        ptbImage.ImageLocation = dir;
                        return;
                    case "invalid_ad_access_code":
                        var accessCodeUrl = $"https://perchance.org/api/getAccessCodeForAdPoweredStuff?__cacheBust={rand.NextDouble()}";
                        var resp = await client.GetAsync(accessCodeUrl, Global.Cancellation.Token);
                        adAccessCode = await resp.Content.ReadAsStringAsync(Global.Cancellation.Token);

                        if (++repeatCount < 5)
                        {
                            lblError.BringToFront();
                            lblError.Text = $"{status} (r: {repeatCount}/5)";
                            goto repeat_2;
                        }
                        break;
                    case "gen_failure":
                    case "waiting_for_prev_request_to_finish":
                        await Task.Delay(3000, Global.Cancellation.Token);
                        if (++repeatCount < 5)
                        {
                            lblError.BringToFront();
                            lblError.Text = $"{status} (r: {repeatCount}/5)";
                            goto repeat_2;
                        }
                        break;
                    case "invalid_key":
                        await Task.Delay(3000, Global.Cancellation.Token);
                        if (++repeatCount < 5)
                        {
                            lblError.BringToFront();
                            lblError.Text = $"{status} (r: {repeatCount}/5)";
                            goto repeat_1;
                        }
                        break;
                }

                lblError.BringToFront();
                lblError.Text = $"{status} (r: {repeatCount}/5)";
            }
            catch (Exception e)
            {
                lblError.BringToFront();
                lblError.Text = e.Message;
            }
            finally
            {
                Global.Semaphore.Release();
            }
        }

        public void BeginGenerate()
        {
            Visible = true;

            btnRefresh.Enabled = false;
            btnKeep.Enabled = false;

            lblError.BringToFront();
            lblError.Text = "Generating...";

            if (!btnKeep.Checked)
                txtSeed.Text = "-1";
        }

        public void LoadImage(string imageLocation, Action? onLoaded = null)
        {
            try
            {
                txtSeed.Text = Path.GetFileNameWithoutExtension(imageLocation);
                ptbImage.ImageLocation = imageLocation;
                ptbImage.BringToFront();
                Visible = true;
                onLoaded?.Invoke();
            }
            catch
            {
            }
        }

        public void EndGenerate()
        {
            btnRefresh.Enabled = true;
            btnKeep.Enabled = true;
        }

        public PerchanceBox()
        {
            InitializeComponent();
            shared = new SharedObject(this);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            if (lastCfg != null)
            {
                BeginGenerate();
                await Generate(lastCfg);
                EndGenerate();
            }
        }

        private void wv2Captcha_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            wv2Captcha.CoreWebView2.AddHostObjectToScript("shared", shared);
            wv2Captcha.CoreWebView2.WebResourceRequested += wv2Captcha_WebResourceRequested;
            wv2Captcha.CoreWebView2.AddWebResourceRequestedFilter("https://image-generation.perchance.org/embed", CoreWebView2WebResourceContext.Document);
        }

        private void wv2Captcha_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var def = e.GetDeferral();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("""
            <div id="turnstile-widget">
                <svg xmlns="http://www.w3.org/2000/svg" width="120" height="30" viewBox="0 0 120 30" fill="#fff">
                    <circle cx="15" cy="15" r="15">
                        <animate attributeName="r" from="15" to="15" begin="0s" dur="0.8s" values="15;9;15" calcMode="linear" repeatCount="indefinite"/>
                        <animate attributeName="fill-opacity" from="1" to="1" begin="0s" dur="0.8s" values="1;.5;1" calcMode="linear" repeatCount="indefinite"/>
                    </circle>
                    <circle cx="60" cy="15" r="9" fill-opacity="0.3">
                        <animate attributeName="r" from="9" to="9" begin="0s" dur="0.8s" values="9;15;9" calcMode="linear" repeatCount="indefinite"/>
                        <animate attributeName="fill-opacity" from="0.5" to="0.5" begin="0s" dur="0.8s" values=".5;1;.5" calcMode="linear" repeatCount="indefinite"/>
                    </circle>
                    <circle cx="105" cy="15" r="15">
                        <animate attributeName="r" from="15" to="15" begin="0s" dur="0.8s" values="15;9;15" calcMode="linear" repeatCount="indefinite"/>
                        <animate attributeName="fill-opacity" from="1" to="1" begin="0s" dur="0.8s" values="1;.5;1" calcMode="linear" repeatCount="indefinite"/>
                    </circle>
                </svg>
            </div>
            <script src="https://challenges.cloudflare.com/turnstile/v0/api.js"></script>
            <script type="text/javascript">
                turnstile.render('#turnstile-widget', {
                    sitekey: '0x4AAAAAAAA8g8NphwaSOT59',
                    theme: 'light',
                    callback: function(token) {
                        chrome.webview.hostObjects.shared.SetToken(token);
                    }
                });
                setTimeout(() => location.reload(), 40000);
            </script>
            """);
            writer.Flush();
            stream.Position = 0;
            e.Response = wv2Captcha.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200, "OK", "");

            def.Complete();
        }

        private void cmsImage_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mniOpenWith.Enabled = cmsImage.SourceControl is PictureBox { ImageLocation: not null };
            mniOpenLocation.Enabled = cmsImage.SourceControl is PictureBox { ImageLocation: not null };
        }

        private void mniCopy_Click(object sender, EventArgs e)
        {
            if (cmsImage.SourceControl is PictureBox ptb)
            {
                Clipboard.SetImage(ptb.Image);
                Toast.Show("Image copied !");
            }
        }

        private void mniOpenWith_Click(object sender, EventArgs e)
        {
            if (cmsImage.SourceControl is PictureBox ptb)
                Process.Start("rundll32.exe", "shell32.dll,OpenAs_RunDLL " + ptb.ImageLocation);
        }

        private void mniOpenLocation_Click(object sender, EventArgs e)
        {
            if (cmsImage.SourceControl is PictureBox ptb)
                Process.Start("explorer.exe", "/select, \"" + ptb.ImageLocation + "\"");
        }
    }
}
