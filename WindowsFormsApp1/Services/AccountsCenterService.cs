using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class AccountsCenterService
    {
        private readonly CoreWebView2 _webView;

        public AccountsCenterService(CoreWebView2 webView)
        {
            _webView = webView;
        }

        // 🌐 PROXY: Proxy settings are now managed automatically by HttpClientFactory
        // No need for manual SetProxy - it's loaded from Settings automatically

        public async Task<(AccountInfo account, InstagramSession session)> GetAccountAndSession()
        {
            // ===== 1. TOKEN =====
            string script = @"
        (function(){
            try {
                return JSON.stringify({
                    fb_dtsg: window.require?.('DTSGInitialData')?.token || null,
                    lsd: window.require?.('LSD')?.token || null
                });
            } catch(e) {
                return JSON.stringify({ error: e.toString() });
            }
        })();";

            var raw = await _webView.ExecuteScriptAsync(script);
            var jsonString = JsonConvert.DeserializeObject<string>(raw);
            var token = JsonConvert.DeserializeObject<JObject>(jsonString);

            string fb_dtsg = token["fb_dtsg"]?.ToString();
            string lsd = token["lsd"]?.ToString();

            if (string.IsNullOrEmpty(fb_dtsg) || string.IsNullOrEmpty(lsd))
                throw new Exception("Missing token");

            // ===== 2. COOKIE =====
            var cookies = await _webView.CookieManager.GetCookiesAsync(null);
            string cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));

            // ===== 3. CALL API =====
            var client = HttpClientFactory.Create(useCookies: false);
            client.DefaultRequestHeaders.Add("cookie", cookieHeader);
            client.DefaultRequestHeaders.Add("x-fb-lsd", lsd);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0"); 
            client.DefaultRequestHeaders.Add("origin", "https://accountscenter.instagram.com"); 
            client.DefaultRequestHeaders.Add("referer", "https://accountscenter.instagram.com/");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "fb_dtsg", fb_dtsg },
                { "lsd", lsd },
                { "fb_api_req_friendly_name", "FXAccountsCenterProfilesPageV2Query" },
                { "doc_id", "26703827865891512" },
                { "variables", JsonConvert.SerializeObject(new {
                    device_id = "device_id_fetch_ig_did",
                    flow = "IG_WEB_SETTINGS",
                    @interface = "IG_WEB",
                    platform = "INSTAGRAM",
                    scale = 1
                })}
            });

            var res = await client.PostAsync("https://accountscenter.instagram.com/api/graphql/", content);
            var json = await res.Content.ReadAsStringAsync();

            var account = await ParseAccount(json);

            var session = new InstagramSession
            {
                Cookie = cookieHeader,
                FbDtsg = fb_dtsg,
                Lsd = lsd,
                CreatedAt = DateTime.Now
            };

            return (account, session);
        }
        public async Task<AccountInfo> ParseAccount(string json)
        {
            var data = JsonConvert.DeserializeObject<JObject>(json);
            JToken identity = null;
            var businessIdentities = data["data"]?["fx_identity_management"]?["identities_and_central_identities"]?["business_identities"];
            if (businessIdentities != null && businessIdentities.Any())
            {
                identity = businessIdentities[0];
            }
            else
            {
                var linkedIdentities = data["data"]?["fx_identity_management"]?["identities_and_central_identities"]?["linked_identities_to_pci"];
                if (linkedIdentities != null && linkedIdentities.Any())
                {
                    identity = linkedIdentities[0];
                }
            }
            if (identity == null) throw new Exception("Parse lỗi");

            // Extract fbAccountId from fx_accounts_management
            string fbAccountId = null;
            var fxAccounts = data["data"]?["fx_accounts_management"]?["accounts"];
            if (fxAccounts != null && fxAccounts.Any())
                fbAccountId = fxAccounts[0]?["id"]?.ToString();

            var nodes = data["data"]?["fxcal_settings"]?["node"]?["personal_info_section"]?["nodes"];

            List<string> emails = new List<string>();
            List<string> phones = new List<string>();
            string birthday = "";

            if (nodes != null)
            {
                string contact = nodes[0]?["navigation_row_subtitle"]?.ToString();

                if (!string.IsNullOrEmpty(contact))
                {
                    foreach (var part in contact.Split(','))
                    {
                        var v = part.Trim();
                        if (v.Contains("@")) emails.Add(v);
                        if (v.StartsWith("+")) phones.Add(v);
                    }
                }

                birthday = nodes[1]?["navigation_row_subtitle"]?.ToString();
            }

            string avatarUrl = identity["profile_picture_info"]?["profile_picture_url"]?.ToString();
            string avatarLocalPath = null;

            if (!string.IsNullOrEmpty(avatarUrl))
            {
                try
                {
                    string avatarFolder = Path.Combine(Application.StartupPath, "Avatars");
                    if (!Directory.Exists(avatarFolder))
                        Directory.CreateDirectory(avatarFolder);

                    string username = identity["username"]?.ToString() ?? "avatar";

                    // Tạo hash URL thành chuỗi hex để tránh trùng tên
                    string fileHash;
                    using (var sha = SHA256.Create())
                    {
                        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(avatarUrl));
                        var sb = new StringBuilder();
                        foreach (var b in hashBytes)
                            sb.Append(b.ToString("x2"));
                        fileHash = sb.ToString().Substring(0, 8);
                    }

                    string fileName = $"{username}_{fileHash}.jpg";
                    avatarLocalPath = Path.Combine(avatarFolder, fileName);

                    // Chỉ tải nếu file chưa tồn tại
                    if (!File.Exists(avatarLocalPath))
                    {
                        using (var http = HttpClientFactory.Create())
                        {
                            var bytes = await http.GetByteArrayAsync(avatarUrl);
                            File.WriteAllBytes(avatarLocalPath, bytes); // sync, .NET Framework compatible
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi tải avatar: {ex.Message}");
                }
            }

            return new AccountInfo
            {
                FbAccountId = fbAccountId,
                FullName  = identity["full_name"]?.ToString(),
                Username  = identity["username"]?.ToString(),
                Email     = string.Join(";", emails),
                Phone     = string.Join(";", phones),
                Birthday  = birthday,
                Avatar    = avatarLocalPath,
                Status    = "Active"
            };
        }
    }
}