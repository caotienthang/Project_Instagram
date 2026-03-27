using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class InstagramPostService
    {
        // ===== PUBLIC API =====
        public async Task<string> PostMedia(List<string> paths, string caption, InstagramSession session)
        {
            if (paths == null || paths.Count == 0)
                return "No images to post";

            var uploadIds = new List<string>();

            foreach (var path in paths)
            {
                // Nếu file quá lớn hoặc cần resize, tách file có thể xử lý ở đây
                var bytes = File.ReadAllBytes(path); // 👈 đọc nhị phân
                var resJson = await UploadImage(bytes, session);
                if (resJson == null) return "Upload fail";

                var obj = JsonConvert.DeserializeObject<JObject>(resJson);
                var uploadId = obj?["upload_id"]?.ToString();
                if (string.IsNullOrEmpty(uploadId)) return "Upload fail";

                uploadIds.Add(uploadId);
            }

            // Tự động phân nhánh 1 media vs nhiều media
            if (uploadIds.Count == 1)
                return await ConfigureSingle(uploadIds[0], caption, session);
            else
                return await ConfigureSidecar(uploadIds, caption, session);
        }

        // ===== UPLOAD IMAGE =====
        private async Task<string> UploadImage(byte[] bytes, InstagramSession session)
        {
            try
            {
                var client = new HttpClient(new HttpClientHandler { UseCookies = false });
                string csrf = GetCookieValue(session.Cookie, "csrftoken");

                client.DefaultRequestHeaders.Add("cookie", session.Cookie);
                client.DefaultRequestHeaders.Add("x-csrftoken", csrf);
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0");
                client.DefaultRequestHeaders.Add("referer", "https://www.instagram.com/");
                client.DefaultRequestHeaders.Add("x-ig-app-id", "936619743392459");

                // 🔥 OFFSET
                client.DefaultRequestHeaders.Remove("Offset"); 
                client.DefaultRequestHeaders.TryAddWithoutValidation("Offset", "0"); 
                string uploadId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); 
                // 🔥 QUAN TRỌNG: đọc binary thật byte[]
                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Headers.Add("X-Entity-Type", "image/jpeg");
                content.Headers.Add("X-Entity-Length", bytes.Length.ToString());
                content.Headers.Add("X-Instagram-Rupload-Params",
                    JsonConvert.SerializeObject(new { upload_id = uploadId, media_type = 1 }));
                content.Headers.ContentLength = bytes.Length;

                var res = await client.PostAsync($"https://i.instagram.com/rupload_igphoto/fb_uploader_{uploadId}", content);
                var json = await res.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // ===== CONFIGURE SINGLE IMAGE =====
        private async Task<string> ConfigureSingle(string uploadId, string caption, InstagramSession session)
        {
            try
            {
                var client = CreateHttp(session);

                var payload = new Dictionary<string, string>
                {
                    { "archive_only", "false" },
                    { "caption", caption },
                    { "clips_share_preview_to_feed", "1" },
                    { "disable_comments", "0" },
                    { "disable_oa_reuse", "false" },
                    { "igtv_share_preview_to_feed", "1" },
                    { "is_meta_only_post", "0" },
                    { "is_unified_video", "1" },
                    { "like_and_view_counts_disabled", "0" },
                    { "media_share_flow", "creation_flow" },
                    { "share_to_facebook", "" },
                    { "share_to_fb_destination_type", "USER" },
                    { "source_type", "library" },
                    { "upload_id", uploadId },
                    { "video_subtitles_enabled", "0" },
                    { "jazoest", "22707" }
                };

                var content = new FormUrlEncodedContent(payload);
                var res = await client.PostAsync("https://www.instagram.com/api/v1/media/configure/", content);
                return await HandleResponse(res);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // ===== CONFIGURE MULTI IMAGE (SIDE CAR) =====
        private async Task<string> ConfigureSidecar(List<string> uploadIds, string caption, InstagramSession session)
        {
            try
            {
                var client = CreateHttp(session);

                var childrenMetadata = uploadIds.Select(id => new Dictionary<string, string>
                {
                    { "upload_id", id }
                }).ToList();

                var payload = new Dictionary<string, object>
                {
                    { "archive_only", false },
                    { "caption", caption },
                    { "children_metadata", childrenMetadata },
                    { "client_sidecar_id", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() },
                    { "disable_comments", "0" },
                    { "is_meta_only_post", false },
                    { "is_open_to_public_submission", false },
                    { "like_and_view_counts_disabled", 0 },
                    { "media_share_flow", "creation_flow" },
                    { "share_to_facebook", "" },
                    { "share_to_fb_destination_type", "USER" },
                    { "source_type", "library" },
                    { "jazoest", "22707" }
                };

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "signed_body", "SIGNATURE." + JsonConvert.SerializeObject(payload) }
                });

                var res = await client.PostAsync("https://www.instagram.com/api/v1/media/configure_sidecar/", content);
                return await HandleResponse(res);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // ===== HELPERS =====
        private HttpClient CreateHttp(InstagramSession session)
        {
            var handler = new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false };
            var client = new HttpClient(handler);
            string csrf = GetCookieValue(session.Cookie, "csrftoken");

            client.DefaultRequestHeaders.Add("cookie", session.Cookie);
            client.DefaultRequestHeaders.Add("x-csrftoken", csrf);
            client.DefaultRequestHeaders.Add("x-ig-app-id", "936619743392459");
            client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0");
            client.DefaultRequestHeaders.Add("referer", "https://www.instagram.com/");
            client.DefaultRequestHeaders.Add("origin", "https://www.instagram.com");
            client.DefaultRequestHeaders.Add("accept", "*/*");

            return client;
        }

        private async Task<string> HandleResponse(HttpResponseMessage res)
        {
            if ((int)res.StatusCode == 302) return "SESSION DIE / COOKIE INVALID";
            return await res.Content.ReadAsStringAsync();
        }

        private string GetCookieValue(string cookie, string key)
        {
            var parts = cookie.Split(';');
            foreach (var part in parts)
            {
                var kv = part.Trim().Split('=');
                if (kv.Length == 2 && kv[0] == key) return kv[1];
            }
            return "";
        }
    }
}