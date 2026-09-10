using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Komorebi {
    static class Program {
        [STAThread] static int Main(string[] args) {
            Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
            var smoke = Array.IndexOf(args, "--smoke-test") >= 0;
            using (var form = new MainForm(smoke)) { Application.Run(form); return form.ExitCode; }
        }
    }
    sealed class MainForm : Form {
        const string Home = "https://komorebi.local/index.html";
        readonly WebView2 view = new WebView2 { Dock = DockStyle.Fill };
        readonly bool smoke; readonly Timer timer = new Timer { Interval = 90000 };
        public int ExitCode = 0;
        public MainForm(bool smoke) {
            this.smoke = smoke; Text = "木漏れ日-日语学习唯美手帐";
            Size = new Size(1200, 850); MinimumSize = new Size(760, 620); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(view); Shown += Initialize;
            timer.Tick += (s,e) => { ExitCode = 3; File.WriteAllText("smoke-result.txt", "TIMEOUT"); Close(); };
            if (smoke) timer.Start();
        }
        async void Initialize(object sender, EventArgs args) {
            try {
                string data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Komorebi", "WebView2");
                if (smoke) data = Path.Combine(Path.GetTempPath(), "KomorebiSmoke-" + Guid.NewGuid());
                var environment = await CoreWebView2Environment.CreateAsync(null, data);
                await view.EnsureCoreWebView2Async(environment);
                view.CoreWebView2.SetVirtualHostNameToFolderMapping("komorebi.local", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web"), CoreWebView2HostResourceAccessKind.DenyCors);
                view.CoreWebView2.Settings.AreDevToolsEnabled = smoke;
                view.CoreWebView2.NewWindowRequested += (s,e) => { e.Handled = true; OpenExternal(e.Uri); };
                view.CoreWebView2.NavigationStarting += (s,e) => {
                    if (e.Uri != Home) { e.Cancel = true; OpenExternal(e.Uri); }
                };
                view.CoreWebView2.WebMessageReceived += (s,e) => {
                    if (e.Source != Home) return;
                    try {
                        var json = new JavaScriptSerializer { MaxJsonLength = 16000000 };
                        var message = json.Deserialize<SaveMessage>(e.WebMessageAsJson);
                        if (message == null || message.kind != "saveFile" || message.text == null || message.text.Length > 8000000) return;
                        using (var dialog = new SaveFileDialog { FileName = Path.GetFileName(message.name ?? "export.csv"), OverwritePrompt = true }) {
                            if (dialog.ShowDialog(this) == DialogResult.OK) File.WriteAllText(dialog.FileName, message.text, new UTF8Encoding(false));
                        }
                    } catch (Exception ex) { MessageBox.Show(this, "无法导出文件：" + ex.Message); }
                };
                view.CoreWebView2.NavigationCompleted += async (s,e) => {
                    if (!smoke) return;
                    try {
                        if (!e.IsSuccess) throw new Exception(e.WebErrorStatus.ToString());
                        for (int retry = 0; retry < 80; retry++) {
                            if (await view.CoreWebView2.ExecuteScriptAsync("typeof rubyReady !== 'undefined' && rubyReady") == "true") break;
                            await System.Threading.Tasks.Task.Delay(500);
                        }
                        var result = await view.CoreWebView2.ExecuteScriptAsync(@"(()=>{try{
                          if(!rubyReady)throw Error('Offline furigana engine not ready');
                          if(!document.querySelector('.word')||DATA.length<22000)throw Error('Dictionary not loaded');
                          navigate('dictionary');const input=document.querySelector('#search');input.value='インフルエンザ';input.dispatchEvent(new Event('input'));if(!document.querySelector('.row'))throw Error('Search failed');
                          const words=validateCSV('单词,假名,释义,例句\n桜,さくら,樱花,桜が咲いています。');
                          if(importWords(words).added!==1)throw Error('Import failed');
                          if(importWords(words).skipped!==1)throw Error('Dedup failed');
                          if(JSON.parse(localStorage.getItem('komorebi-v1')).custom.length!==1)throw Error('Persistence failed');
                          navigate('import');document.querySelector('#custom-study').click();
                          if(document.querySelector('.word').textContent!=='桜')throw Error('Custom study failed');
                          navigate('dictionary');filter='custom';renderList();
                          if(document.querySelectorAll('.row').length!==1)throw Error('Search filter failed');
                          return 'PASS: dictionary, CSV import, deduplication, local persistence, custom study, list';
                        }catch(e){return 'FAIL: '+e.message}})()");
                        File.WriteAllText("smoke-result.txt", result); ExitCode = result.Contains("PASS:") ? 0 : 2;
                    } catch (Exception ex) { File.WriteAllText("smoke-result.txt", ex.ToString()); ExitCode = 2; }
                    timer.Stop(); Close();
                };
                view.CoreWebView2.Navigate(Home);
            } catch (Exception ex) {
                ExitCode = 1;
                if (smoke) { File.WriteAllText("smoke-result.txt", ex.ToString()); Close(); return; }
                if (MessageBox.Show(this, "应用需要 Microsoft Edge WebView2 Runtime。请安装或修复后重试。\n\n是否打开微软官方下载页？\n\n" + ex.Message, "启动失败", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    OpenExternal("https://developer.microsoft.com/microsoft-edge/webview2/");
                Close();
            }
        }
        static void OpenExternal(string value) {
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == "https" || uri.Scheme == "http") && uri.Host != "komorebi.local")
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
        }
        protected override void Dispose(bool disposing) { if (disposing) { timer.Dispose(); view.Dispose(); } base.Dispose(disposing); }
        public class SaveMessage { public string kind { get; set; } public string name { get; set; } public string text { get; set; } }
    }
}
