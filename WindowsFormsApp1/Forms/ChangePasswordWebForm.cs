using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class ChangePasswordWebForm : Form
    {
        private readonly AccountInfo      _account;
        private readonly InstagramSession _session;
        private string _tempFolder;

        public ChangePasswordWebForm(AccountInfo account, InstagramSession session)
        {
            InitializeComponent();
            _account   = account;
            _session   = session;
            this.Text  = $"Đổi mật khẩu — @{account.Username}";

            this.Load       += ChangePasswordWebForm_Load;
            this.FormClosed += ChangePasswordWebForm_FormClosed;
        }

        // ── Load: khởi tạo WebView2, inject cookie rồi điều hướng ──
        private async void ChangePasswordWebForm_Load(object sender, EventArgs e)
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), "WebView2_ChangePwd_" + Guid.NewGuid().ToString("N"));

            var env = await CoreWebView2Environment.CreateAsync(null, _tempFolder);
            await webView.EnsureCoreWebView2Async(env);

            // Inject cookies trước khi navigate để trang nhận đúng session
            InjectCookies();

            webView.CoreWebView2.Navigate("https://accountscenter.instagram.com/password_and_security/");
        }

        // ── Inject toàn bộ cookies vào CookieManager trước khi điều hướng ──
        private void InjectCookies()
        {
            var cookieManager = webView.CoreWebView2.CookieManager;
            var parts = _session.Cookie.Split(';');

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                int eqIdx   = trimmed.IndexOf('=');
                if (eqIdx < 0) continue;

                var name  = trimmed.Substring(0, eqIdx).Trim();
                var value = trimmed.Substring(eqIdx + 1).Trim();
                if (string.IsNullOrEmpty(name)) continue;

                // Inject cho cả 2 domain cần thiết
                foreach (var domain in new[] { ".instagram.com", ".accountscenter.instagram.com" })
                {
                    try
                    {
                        var cookie = cookieManager.CreateCookie(name, value, domain, "/");
                        cookieManager.AddOrUpdateCookie(cookie);
                    }
                    catch { }
                }
            }
        }

        // ── Form closed: xóa session (password đổi → cookie cũ không hợp lệ) ──
        private void ChangePasswordWebForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (_account != null && _account.Id > 0)
                    InstagramSessionRepository.DeleteByAccountId(_account.Id);
            }
            catch { }

            // Dọn dẹp thư mục WebView2 tạm
            try
            {
                if (!string.IsNullOrEmpty(_tempFolder) && Directory.Exists(_tempFolder))
                    Directory.Delete(_tempFolder, true);
            }
            catch { }
        }
    }
}
