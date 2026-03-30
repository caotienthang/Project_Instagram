using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class AvatarService
    {
        public async Task<(bool success, string message, string localPath)> UploadAvatar(string filePath, InstagramSession session)
        {
            try
            {
                var client = HttpClientFactory.Create(useCookies: false);

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

                    return (true, "Đổi avatar thành công", localPath);
                }

                return (false, json, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
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

            using (var client = HttpClientFactory.Create())
            {
                var bytes = await client.GetByteArrayAsync(url);
                File.WriteAllBytes(filePath, bytes);
            }

            return filePath;
        }
    }
}
