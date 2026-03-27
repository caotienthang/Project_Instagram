using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Views;

namespace WindowsFormsApp1.Forms
{
    public partial class AccountDetailForm : Form
    {
        private AccountInfo _account;

        public AccountDetailForm(AccountInfo account)
        {
            InitializeComponent();
            _account = account;

            LoadData();
        }

        private void LoadData()
        {
            lblName.Text = _account.Username;

            // 🔥 load avatar nếu có
            if (!string.IsNullOrEmpty(_account.Avatar) && File.Exists(_account.Avatar))
            {
                try
                {
                    using (var fs = new FileStream(_account.Avatar, FileMode.Open, FileAccess.Read))
                    {
                        picAvatar.Image = Image.FromStream(fs);
                    }
                }
                catch
                {
                    picAvatar.Image = null;
                }
            }
            else
            {
                picAvatar.Image = null; // hoặc ảnh default
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            panelContainer.Controls.Clear();
            panelContainer.Visible = true;
            var postPanel = new PostPanel(_account);
            postPanel.OnClose += () =>
            {
                panelContainer.Visible = false;
                panelContainer.Controls.Clear();
            };

            panelContainer.Controls.Add(postPanel);
        }

        private async void btnChangeAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string filePath = ofd.FileName;

                var session = InstagramSessionRepository.GetByAccountId(_account.Id);

                if (session == null)
                {
                    MessageBox.Show("Không tìm thấy session");
                    return;
                }

                var result = await UploadAvatar(filePath, session);

                MessageBox.Show(result);
            }
        }
        private async Task<string> UploadAvatar(string filePath, InstagramSession session)
        {
            try
            {
                var client = new HttpClient(new HttpClientHandler
                {
                    UseCookies = false
                });

                // ===== COOKIE =====
                client.DefaultRequestHeaders.Add("cookie", session.Cookie);

                // ===== LẤY csrftoken từ cookie =====
                string csrf = GetCookieValue(session.Cookie, "csrftoken");

                client.DefaultRequestHeaders.Add("x-csrftoken", csrf);
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0");
                client.DefaultRequestHeaders.Add("referer", "https://www.instagram.com/accounts/edit/");

                // ===== MULTIPART =====
                var content = new MultipartFormDataContent();

                var fileBytes = File.ReadAllBytes(filePath);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                content.Add(fileContent, "profile_pic", Path.GetFileName(filePath));

                // jazoest (có thể hardcode)
                content.Add(new StringContent("22707"), "jazoest");

                var res = await client.PostAsync(
                    "https://www.instagram.com/api/v1/web/accounts/web_change_profile_picture/",
                    content
                );

                var json = await res.Content.ReadAsStringAsync();

                // ===== PARSE =====
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                if (data.status == "ok")
                {
                    string newAvatarUrl = data.profile_pic_url_hd;

                    // 👉 download lại avatar về local
                    string localPath = await DownloadAvatarToLocal(newAvatarUrl);

                    // 👉 update DB
                    _account.Avatar = localPath;
                    AccountRepository.UpdateAvatar(_account.Id, localPath);

                    // 👉 update UI
                    picAvatar.Image = Image.FromFile(localPath);

                    return "Đổi avatar thành công";
                }

                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private string GetCookieValue(string cookie, string key)
        {
            var parts = cookie.Split(';');

            foreach (var part in parts)
            {
                var kv = part.Trim().Split('=');
                if (kv.Length == 2 && kv[0] == key)
                    return kv[1];
            }

            return "";
        }
        private async Task<string> DownloadAvatarToLocal(string url)
        {
            string folder = Path.Combine(Application.StartupPath, "AvatarCache");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, Guid.NewGuid() + ".jpg");

            using (var client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(url);
                File.WriteAllBytes(filePath, bytes);
            }

            return filePath;
        }
    }
}
