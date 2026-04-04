using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class AccountsCenterForm : Form
    {
        private AccountsCenterService _service;
        private AccountInfo _account;

        private bool _redirected = false;
        private bool _fetched = false;
        private string _tempFolder;

        public AccountsCenterForm(AccountInfo account = null)
        {
            InitializeComponent();
            _account = account;

            this.Load += AccountsCenterForm_Load;
            this.FormClosed += AccountsCenterForm_FormClosed;
        }

        private async void AccountsCenterForm_Load(object sender, EventArgs e)
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), "WebView2_" + Guid.NewGuid().ToString("N"));

            var env = await CoreWebView2Environment.CreateAsync(null, _tempFolder);
            await webView21.EnsureCoreWebView2Async(env);

            _service = new AccountsCenterService(webView21.CoreWebView2);
            webView21.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted;

            webView21.CoreWebView2.Navigate("https://www.instagram.com/accounts/login/");
        }

        private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                string url = webView21.Source?.ToString() ?? "";

                if (!_redirected && url.Contains("instagram.com") && !url.Contains("accountscenter"))
                {
                    if (!await IsLoggedIn()) return;

                    _redirected = true;
                    webView21.CoreWebView2.Navigate("https://accountscenter.instagram.com/");
                    return;
                }

                if (url.Contains("accountscenter.instagram.com") && !_fetched)
                {
                    string ready = await webView21.CoreWebView2.ExecuteScriptAsync("document.readyState");
                    if (!ready.Contains("complete")) return;
                    _fetched = true;

                    var (acc, session) = await _service.GetAccountAndSession();

                    AccountInfo dbAcc;

                    if (_account == null)
                    {
                        // ADD - Insert new account
                        AccountRepository.Insert(acc);
                        dbAcc = AccountRepository.GetByUsername(acc.Username);
                    }
                    else
                    {
                        // UPDATE - Use UpdateFromComputer to preserve phone data
                        if (acc.Username != _account.Username)
                        {
                            MessageBox.Show("Sai account login!");
                            return;
                        }

                        acc.Id = _account.Id;
                        AccountRepository.UpdateFromComputer(acc);
                        dbAcc = acc;
                    }

                    session.AccountId = dbAcc.Id;
                    InstagramSessionRepository.UpsertComputer(session);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<bool> IsLoggedIn()
        {
            var result = await webView21.CoreWebView2.ExecuteScriptAsync("document.cookie.includes('ds_user_id')");
            return result?.ToLower().Contains("true") == true;
        }

        private void AccountsCenterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (Directory.Exists(_tempFolder))
                    Directory.Delete(_tempFolder, true);
            }
            catch { }
        }
    }
}