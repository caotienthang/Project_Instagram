using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WindowsFormsApp1.Managers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Service để đổi mật khẩu Instagram qua Phone API
    /// </summary>
    public class ChangePasswordService
    {
        private const string CHANGE_PASSWORD_URL = "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.change_password.submit_password/";
        
        private static AppSettings Settings => SettingsManager.LoadSettings();

        // Reuse password encryption từ InstagramPhoneLoginService
        private readonly InstagramPhoneLoginService _loginService;

        public ChangePasswordService()
        {
            _loginService = new InstagramPhoneLoginService();
        }

        /// <summary>
        /// Đổi mật khẩu Instagram
        /// </summary>
        /// <param name="session">Session chứa authorization token</param>
        /// <param name="accountId">Facebook Account ID</param>
        /// <param name="username">Instagram username</param>
        /// <param name="currentPassword">Mật khẩu hiện tại</param>
        /// <param name="newPassword">Mật khẩu mới</param>
        /// <returns>Tuple (success, message)</returns>
        public async Task<(bool success, string message)> ChangePasswordAsync(
            InstagramSession session,
            string accountId,
            string username,
            string linkAvatar,
            string currentPassword,
            string newPassword)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(currentPassword))
                    return (false, "Current password is required");

                if (string.IsNullOrWhiteSpace(newPassword))
                    return (false, "New password is required");

                if (string.IsNullOrWhiteSpace(accountId))
                    return (false, "Account ID is required");

                if (session == null || string.IsNullOrWhiteSpace(session.AuthorizationPhone))
                    return (false, "Authorization token is required");

                // Encrypt passwords
                string encryptedCurrent = _loginService.EncryptPassword(currentPassword);
                string encryptedNew = _loginService.EncryptPassword(newPassword);
                string encryptedConfirm = _loginService.EncryptPassword(newPassword); // Same as new password

                // Build payload
                var payload = BuildPayload(accountId, username, linkAvatar, encryptedCurrent, encryptedNew, encryptedConfirm);

                // Send request
                using (var client = HttpClientFactory.Create(useCookies: false))
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, CHANGE_PASSWORD_URL);

                    // Headers
                    request.Headers.Add("User-Agent", Settings.InstagramApi.UserAgent);
                    request.Headers.Add("Authorization", session.AuthorizationPhone);

                    // Body
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var response = await client.SendAsync(request);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse response
                    return ParseResponse(responseBody);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Build request payload
        /// </summary>
        private string BuildPayload(
            string accountId,
            string username,
            string linkAvatar,
            string encryptedCurrent,
            string encryptedNew,
            string encryptedConfirm)
        {
            // Client input params
            var clientParams = new JObject
            {
                ["new_password_confirm"] = encryptedConfirm,
                ["account_type"] = 1,
                ["account_id"] = accountId,
                ["account_name"] = username,
                ["new_password"] = encryptedNew,
                ["should_logout"] = 0,
                ["current_password"] = encryptedCurrent
            };

            // Server params - profile_picture_url set to null theo yêu cầu
            var serverParams = new JObject
            {
                ["profile_picture_url"] = linkAvatar,
                ["is_standalone_bottom_sheet"] = 0
            };

            // Combined params
            var paramsObj = new JObject
            {
                ["client_input_params"] = clientParams,
                ["server_params"] = serverParams
            };

            // URL encode
            string paramsJson = paramsObj.ToString(Newtonsoft.Json.Formatting.None);
            string paramsEncoded = HttpUtility.UrlEncode(paramsJson);

            // Build final payload
            string bloksVersioningId = Settings.InstagramApi.BloksVersioningId;
            return $"params={paramsEncoded}&bloks_versioning_id={bloksVersioningId}";
        }

        /// <summary>
        /// Parse response để lấy success/error message
        /// </summary>
        private (bool success, string message) ParseResponse(string responseBody)
        {
            try
            {
                var json = JObject.Parse(responseBody);
                string status = json["status"]?.ToString();

                if (status != "ok")
                {
                    return (false, "API returned non-ok status");
                }

                // Lấy action string - hỗ trợ nhiều dạng response
                string action = ExtractAction(json);

                if (!string.IsNullOrEmpty(action))
                {
                    // Normalize: unescape any escaped quotes and trim whitespace
                    action = Regex.Unescape(action).Trim();
                }

                if (string.IsNullOrEmpty(action))
                    return (false, "No action found in response");

                // Check error message
                // Pattern: dq8 "FX_CHANGE_PASSWORD:new_password_error_message" "Create a new password..."
                var errorMatch = Regex.Match(action, @"dq8\s+""[^""]*""\s+""([^""]*)""");
                if (errorMatch.Success)
                {
                    string errorMsg = errorMatch.Groups[1].Value;
                    return (false, errorMsg);
                }

                // Check success message
                // Pattern: dhv "You changed your Instagram password for {username}"
                var successMatch = Regex.Match(action, @"dhv\s+""([^""]*)""");
                if (successMatch.Success)
                {
                    string successMsg = successMatch.Groups[1].Value;
                    return (true, successMsg);
                }

                // Fallback: nếu không có error thì coi như success
                return (true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to parse response: {ex.Message}");
            }
        }

        /// <summary>
        /// Try to extract the first string-valued "action" property from the response.
        /// Supports multiple response shapes returned by Instagram Bloks.
        /// </summary>
        private string ExtractAction(JToken token)
        {
            if (token == null) return null;

            // Direct fields
            var direct = token["action"]?.ToString();
            if (!string.IsNullOrEmpty(direct)) return direct;

            // layout.action
            var layoutAction = token["layout"]? ["action"]?.ToString();
            if (!string.IsNullOrEmpty(layoutAction)) return layoutAction;

            // layout.bloks_payload.action
            var bpAction = token["layout"]? ["bloks_payload"]? ["action"]?.ToString();
            if (!string.IsNullOrEmpty(bpAction)) return bpAction;

            // layout.bloks_payload.tree... search deeper
            var treeAction = token.SelectToken("layout.bloks_payload.tree..action")?.ToString();
            if (!string.IsNullOrEmpty(treeAction)) return treeAction;

            // Fallback: recursive search for any property named "action"
            string found = RecursiveFindAction(token);
            return found;
        }

        private string RecursiveFindAction(JToken token)
        {
            if (token == null) return null;

            if (token.Type == JTokenType.Object)
            {
                foreach (var prop in token.Children<JProperty>())
                {
                    if (string.Equals(prop.Name, "action", StringComparison.OrdinalIgnoreCase) && prop.Value.Type == JTokenType.String)
                        return prop.Value.ToString();

                    var inner = RecursiveFindAction(prop.Value);
                    if (!string.IsNullOrEmpty(inner)) return inner;
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    var inner = RecursiveFindAction(item);
                    if (!string.IsNullOrEmpty(inner)) return inner;
                }
            }

            return null;
        }
    }
}
