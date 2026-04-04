using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WindowsFormsApp1.Managers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class TwoFactorPhoneService
    {
        private const string GENERATE_TOTP_URL = "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.two_factor.totp.generate_key/";
        private static AppSettings Settings => SettingsManager.LoadSettings();

        /// <summary>
        /// Generate a random UUID for family_device_id
        /// </summary>
        private string GenerateFamilyDeviceId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Generate Android device ID in format: android-{16 hex chars}
        /// </summary>
        private string GenerateAndroidDeviceId()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[8];
                rng.GetBytes(randomBytes);
                string hexString = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
                return $"android-{hexString}";
            }
        }

        /// <summary>
        /// Generate machine ID (Base64 encoded random bytes)
        /// </summary>
        private string GenerateMachineId()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[20];
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        /// <summary>
        /// Add a TOTP device using Phone (non-Web) flow by calling Bloks generate_key endpoint.
        /// Returns TotpKeyResult with KeyId/KeyText/QrCodeUri on success.
        /// </summary>
        public async Task<TotpKeyResult> AddTotpByPhoneAsync(string accountId, InstagramSession session, string nickname = null)
        {
            try
            {
                if (session == null || string.IsNullOrWhiteSpace(session.AuthorizationPhone))
                    return new TotpKeyResult { Success = false, Message = "Authorization token is required" };

                // Build params object with dynamically generated device IDs
                string familyDeviceId = GenerateFamilyDeviceId();
                string deviceId = GenerateAndroidDeviceId();
                string machineId = GenerateMachineId();

                var clientInput = new JObject
                {
                    ["family_device_id"] = familyDeviceId,
                    ["device_id"] = deviceId,
                    ["machine_id"] = machineId
                };

                // Add key_nickname if provided (for adding additional devices)
                if (!string.IsNullOrWhiteSpace(nickname))
                {
                    clientInput["key_nickname"] = nickname;
                }

                var serverParams = new JObject
                {
                    ["requested_screen_component_type"] = null,
                    ["account_type"] = 1,
                    ["machine_id"] = null,
                    ["INTERNAL__latency_qpl_marker_id"] = 36707139,
                    ["INTERNAL__latency_qpl_instance_id"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ["account_id"] = accountId
                };

                var paramsObj = new JObject
                {
                    ["client_input_params"] = clientInput,
                    ["server_params"] = serverParams
                };

                string paramsJson = paramsObj.ToString(Newtonsoft.Json.Formatting.None);
                string paramsEncoded = HttpUtility.UrlEncode(paramsJson);
                string bloksVersioningId = Settings.InstagramApi.BloksVersioningId;
                string payload = $"params={paramsEncoded}&bloks_versioning_id={bloksVersioningId}";

                using (var client = HttpClientFactory.Create(useCookies: false))
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, GENERATE_TOTP_URL);
                    req.Headers.Add("User-Agent", Settings.InstagramApi.UserAgent);
                    req.Headers.Add("Authorization", session.AuthorizationPhone);
                    req.Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var res = await client.SendAsync(req);
                    var body = await res.Content.ReadAsStringAsync();

                    // If parsing later fails, save raw response for debugging
                    try
                    {
                        var logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        if (!System.IO.Directory.Exists(logFolder))
                            System.IO.Directory.CreateDirectory(logFolder);

                        var logPath = System.IO.Path.Combine(logFolder, $"twofactor_generate_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json");
                        System.IO.File.WriteAllText(logPath, body);
                    }
                    catch { /* ignore logging errors */ }

                    // Parse action string
                    var json = JObject.Parse(body);
                    string action = ExtractAction(json);
                    if (string.IsNullOrEmpty(action))
                        return new TotpKeyResult { Success = false, Message = "No action found in response" };

                    // Look for error pattern dq8 ... "message"
                    var err = Regex.Match(action, "dq8\\s+\"[^\"]*\"\\s+\"([^\"]*)\"");
                    if (err.Success)
                        return new TotpKeyResult { Success = false, Message = err.Groups[1].Value };

                    // Parse actual response format:
                    // (dkc \"account_id\" \"key_id\" \"key_text\" \"qr_code_uri\" \"INTERNAL_INFRA_screen_id\")
                    // (dkc (eud ...) \"18073988894271003\" \"VBD4 IQC3 RRIX YOMI 325L UDFR NIP2 XS5Z\" \"https://...\" \"rduxj0:8\")

                    // Find the field names pattern
                    string fieldPattern = @"\(dkc\s+\""account_id\""\s+\""key_id\""\s+\""key_text\""\s+\""qr_code_uri\""\s+\""INTERNAL_INFRA_screen_id\""\)";
                    var fieldMatch = Regex.Match(action, fieldPattern);

                    if (fieldMatch.Success)
                    {
                        // After the field names, find the next (dkc ...) block with values
                        int startPos = fieldMatch.Index + fieldMatch.Length;
                        string remaining = action.Substring(startPos);

                        // Extract all quoted strings from the next (dkc ...) block
                        var valuePattern = @"\(dkc[^)]*?\""([^\""]+)\""[^)]*?\""([^\""]+)\""[^)]*?\""([^\""]+)\""";
                        var valueMatch = Regex.Match(remaining, valuePattern);

                        if (valueMatch.Success && valueMatch.Groups.Count >= 4)
                        {
                            string keyId = valueMatch.Groups[1].Value;
                            string keyText = valueMatch.Groups[2].Value;
                            string qrUri = valueMatch.Groups[3].Value;

                            return new TotpKeyResult
                            {
                                Success = true,
                                KeyId = keyId,
                                KeyText = keyText,
                                QrCodeUri = qrUri,
                                FamilyDeviceId = familyDeviceId,
                                DeviceId = deviceId,
                                MachineId = machineId
                            };
                        }
                    }

                    // Fallback: try to extract key_text (groups of 4-characters) and qr code via looser regex
                    var keyTextMatch = Regex.Match(action, @"([A-Z0-9]{4}(?:\s+[A-Z0-9]{4}){2,})");
                    var qrMatch = Regex.Match(action, @"https://www\.facebook\.com/qr/show/code/[^\""\s]+");
                    if (keyTextMatch.Success || qrMatch.Success)
                    {
                        return new TotpKeyResult
                        {
                            Success = true,
                            KeyId = null,
                            KeyText = keyTextMatch.Success ? keyTextMatch.Groups[1].Value : null,
                            QrCodeUri = qrMatch.Success ? qrMatch.Value.Replace(@"\/", "/") : null,
                            FamilyDeviceId = familyDeviceId,
                            DeviceId = deviceId,
                            MachineId = machineId
                        };
                    }

                    return new TotpKeyResult { Success = false, Message = "Unable to parse generate_key response" };
                }
            }
            catch (Exception ex)
            {
                return new TotpKeyResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Confirm TOTP code for phone flow by calling Bloks enable endpoint.
        /// </summary>
        public async Task<TwoFactorActionResult> ConfirmTotpByPhoneAsync(string accountId, string code, InstagramSession session, TotpKeyResult keyResult)
        {
            try
            {
                if (session == null || string.IsNullOrWhiteSpace(session.AuthorizationPhone))
                    return new TwoFactorActionResult { Success = false, Message = "Authorization token is required" };

                if (string.IsNullOrWhiteSpace(code) || code.Length != 6)
                    return new TwoFactorActionResult { Success = false, Message = "Invalid verification code" };

                if (keyResult == null)
                    return new TwoFactorActionResult { Success = false, Message = "Key result is required" };

                // Build params with device IDs from generate_key call
                var clientInput = new JObject
                {
                    ["family_device_id"] = keyResult.FamilyDeviceId,
                    ["device_id"] = keyResult.DeviceId,
                    ["machine_id"] = keyResult.MachineId,
                    ["ap_entrypoint"] = null,
                    ["verification_code"] = code
                };

                var serverParams = new JObject
                {
                    ["account_type"] = 1,
                    ["INTERNAL__latency_qpl_marker_id"] = 36707139,
                    ["INTERNAL__latency_qpl_instance_id"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ["account_id"] = accountId
                };

                var paramsObj = new JObject
                {
                    ["client_input_params"] = clientInput,
                    ["server_params"] = serverParams
                };

                string paramsJson = paramsObj.ToString(Newtonsoft.Json.Formatting.None);
                string paramsEncoded = HttpUtility.UrlEncode(paramsJson);
                string bloksVersioningId = Settings.InstagramApi.BloksVersioningId;
                string payload = $"params={paramsEncoded}&bloks_versioning_id={bloksVersioningId}";

                using (var client = HttpClientFactory.Create(useCookies: false))
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.two_factor.totp.enable/");
                    req.Headers.Add("User-Agent", Settings.InstagramApi.UserAgent);
                    req.Headers.Add("Authorization", session.AuthorizationPhone);
                    req.Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var res = await client.SendAsync(req);
                    var body = await res.Content.ReadAsStringAsync();

                    // Save response for debugging
                    try
                    {
                        var logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        if (!System.IO.Directory.Exists(logFolder))
                            System.IO.Directory.CreateDirectory(logFolder);

                        var logPath = System.IO.Path.Combine(logFolder, $"twofactor_enable_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json");
                        System.IO.File.WriteAllText(logPath, body);
                    }
                    catch { /* ignore logging errors */ }

                    // Parse action string
                    var json = JObject.Parse(body);
                    string action = ExtractAction(json);
                    if (string.IsNullOrEmpty(action))
                        return new TwoFactorActionResult { Success = false, Message = "No action found in response" };

                    // Check for success indicators
                    bool totpEnabled = action.Contains("\"FX_TWO_FACTOR_INFO:totp_enabled\" true");
                    bool statusEnabled = action.Contains("\"FX_TWO_FACTOR_STATUS:is_enabled\" true");
                    bool successMsg = action.Contains("\"ENABLE_TOTP_SUCCESS\"");

                    if (totpEnabled || statusEnabled || successMsg)
                    {
                        return new TwoFactorActionResult
                        {
                            Success = true,
                            Message = "TOTP enabled successfully",
                            TotpSeed = keyResult.KeyId
                        };
                    }

                    // Check for error pattern dq8 ... "message"
                    var err = Regex.Match(action, "dq8\\s+\"[^\"]*\"\\s+\"([^\"]*)\"");
                    if (err.Success)
                        return new TwoFactorActionResult { Success = false, Message = err.Groups[1].Value };

                    return new TwoFactorActionResult { Success = false, Message = "Unable to confirm TOTP code" };
                }
            }
            catch (Exception ex)
            {
                return new TwoFactorActionResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Remove a TOTP device using Phone API.
        /// </summary>
        public async Task<TwoFactorActionResult> RemoveTotpByPhoneAsync(string accountId, string totpKeyId, InstagramSession session)
        {
            try
            {
                if (session == null || string.IsNullOrWhiteSpace(session.AuthorizationPhone))
                    return new TwoFactorActionResult { Success = false, Message = "Authorization token is required" };

                if (string.IsNullOrWhiteSpace(totpKeyId))
                    return new TwoFactorActionResult { Success = false, Message = "TOTP key ID is required" };

                // Build params with dynamic device IDs
                string familyDeviceId = GenerateFamilyDeviceId();
                string deviceId = GenerateAndroidDeviceId();
                string machineId = GenerateMachineId();

                var clientInput = new JObject
                {
                    ["family_device_id"] = familyDeviceId,
                    ["device_id"] = deviceId,
                    ["machine_id"] = machineId
                };

                var serverParams = new JObject
                {
                    ["account_type"] = 1,
                    ["INTERNAL__latency_qpl_marker_id"] = 36707139,
                    ["account_id"] = accountId,
                    ["requested_screen_component_type"] = null,
                    ["machine_id"] = null,
                    ["INTERNAL__latency_qpl_instance_id"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ["totp_key_id"] = totpKeyId
                };

                var paramsObj = new JObject
                {
                    ["client_input_params"] = clientInput,
                    ["server_params"] = serverParams
                };

                string paramsJson = paramsObj.ToString(Newtonsoft.Json.Formatting.None);
                string paramsEncoded = HttpUtility.UrlEncode(paramsJson);
                string bloksVersioningId = Settings.InstagramApi.BloksVersioningId;
                string payload = $"params={paramsEncoded}&bloks_versioning_id={bloksVersioningId}";

                using (var client = HttpClientFactory.Create(useCookies: false))
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, 
                        "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.two_factor.totp.remove_key/");
                    req.Headers.Add("User-Agent", Settings.InstagramApi.UserAgent);
                    req.Headers.Add("Authorization", session.AuthorizationPhone);
                    req.Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var res = await client.SendAsync(req);
                    var body = await res.Content.ReadAsStringAsync();

                    // Save response for debugging
                    try
                    {
                        var logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        if (!System.IO.Directory.Exists(logFolder))
                            System.IO.Directory.CreateDirectory(logFolder);

                        var logPath = System.IO.Path.Combine(logFolder, $"twofactor_remove_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json");
                        System.IO.File.WriteAllText(logPath, body);
                    }
                    catch { /* ignore logging errors */ }

                    // Parse action string
                    var json = JObject.Parse(body);
                    string action = ExtractAction(json);
                    if (string.IsNullOrEmpty(action))
                        return new TwoFactorActionResult { Success = false, Message = "No action found in response" };

                    // Check for success indicators
                    bool removeSuccess = action.Contains("\"REMOVE_TOTP_KEY_SUCCESS\"");
                    bool deviceRemoved = action.Contains("\"Device removed\"");

                    if (removeSuccess || deviceRemoved)
                    {
                        return new TwoFactorActionResult
                        {
                            Success = true,
                            Message = "Device removed successfully"
                        };
                    }

                    // Check for error pattern dq8 ... "message"
                    var err = Regex.Match(action, "dq8\\s+\"[^\"]*\"\\s+\"([^\"]*)\"");
                    if (err.Success)
                        return new TwoFactorActionResult { Success = false, Message = err.Groups[1].Value };

                    return new TwoFactorActionResult { Success = false, Message = "Unable to remove device" };
                }
            }
            catch (Exception ex)
            {
                return new TwoFactorActionResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Disable TOTP (Turn Off 2FA) using Phone API.
        /// This completely disables 2FA for the account.
        /// </summary>
        public async Task<TwoFactorActionResult> DisableTotpByPhoneAsync(string accountId, InstagramSession session)
        {
            try
            {
                if (session == null || string.IsNullOrWhiteSpace(session.AuthorizationPhone))
                    return new TwoFactorActionResult { Success = false, Message = "Authorization token is required" };

                // Build params with dynamic device IDs
                string familyDeviceId = GenerateFamilyDeviceId();
                string deviceId = GenerateAndroidDeviceId();
                string machineId = GenerateMachineId();

                var clientInput = new JObject
                {
                    ["family_device_id"] = familyDeviceId,
                    ["device_id"] = deviceId,
                    ["machine_id"] = machineId
                };

                var serverParams = new JObject
                {
                    ["requested_screen_component_type"] = null,
                    ["account_type"] = 1,
                    ["machine_id"] = null,
                    ["INTERNAL__latency_qpl_marker_id"] = 36707139,
                    ["INTERNAL__latency_qpl_instance_id"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ["account_id"] = accountId
                };

                var paramsObj = new JObject
                {
                    ["client_input_params"] = clientInput,
                    ["server_params"] = serverParams
                };

                string paramsJson = paramsObj.ToString(Newtonsoft.Json.Formatting.None);
                string paramsEncoded = HttpUtility.UrlEncode(paramsJson);
                string bloksVersioningId = Settings.InstagramApi.BloksVersioningId;
                string payload = $"params={paramsEncoded}&bloks_versioning_id={bloksVersioningId}";

                using (var client = HttpClientFactory.Create(useCookies: false))
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, 
                        "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.two_factor.totp.disable/");
                    req.Headers.Add("User-Agent", Settings.InstagramApi.UserAgent);
                    req.Headers.Add("Authorization", session.AuthorizationPhone);
                    req.Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var res = await client.SendAsync(req);
                    var body = await res.Content.ReadAsStringAsync();

                    // Save response for debugging
                    try
                    {
                        var logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        if (!System.IO.Directory.Exists(logFolder))
                            System.IO.Directory.CreateDirectory(logFolder);

                        var logPath = System.IO.Path.Combine(logFolder, $"twofactor_disable_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json");
                        System.IO.File.WriteAllText(logPath, body);
                    }
                    catch { /* ignore logging errors */ }

                    // Parse action string
                    var json = JObject.Parse(body);
                    string action = ExtractAction(json);
                    if (string.IsNullOrEmpty(action))
                        return new TwoFactorActionResult { Success = false, Message = "No action found in response" };

                    // Check for success indicators
                    bool totpDisabled = action.Contains("\"FX_TWO_FACTOR_INFO:totp_enabled\" false");
                    bool statusDisabled = action.Contains("\"FX_TWO_FACTOR_STATUS:is_enabled\" false");
                    bool disableSuccess = action.Contains("\"DISABLE_TOTP_SUCCESS\"");
                    bool methodOff = action.Contains("\"The authentication app security method is OFF\"");

                    if (totpDisabled || statusDisabled || disableSuccess || methodOff)
                    {
                        return new TwoFactorActionResult
                        {
                            Success = true,
                            Message = "2FA disabled successfully"
                        };
                    }

                    // Check for error pattern dq8 ... "message"
                    var err = Regex.Match(action, "dq8\\s+\"[^\"]*\"\\s+\"([^\"]*)\"");
                    if (err.Success)
                        return new TwoFactorActionResult { Success = false, Message = err.Groups[1].Value };

                    return new TwoFactorActionResult { Success = false, Message = "Unable to disable 2FA" };
                }
            }
            catch (Exception ex)
            {
                return new TwoFactorActionResult { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Get 2FA status and TOTP seeds via Phone API.
        /// Returns TwoFactorStatus with list of registered TOTP devices.
        /// </summary>
        public async Task<TwoFactorStatus> GetStatusByPhoneAsync(string accountId, InstagramSession session)
        {
            try
            {
                if (session == null || string.IsNullOrWhiteSpace(session.AuthorizationPhone))
                    return new TwoFactorStatus { Error = "Authorization token is required" };

                // Build params with machine_id
                string machineId = GenerateMachineId();

                var clientInput = new JObject
                {
                    ["machine_id"] = machineId
                };

                var serverParams = new JObject
                {
                    ["account_id"] = accountId,
                    ["INTERNAL_INFRA_screen_id"] = "1"
                };

                var paramsObj = new JObject
                {
                    ["client_input_params"] = clientInput,
                    ["server_params"] = serverParams
                };

                string paramsJson = paramsObj.ToString(Newtonsoft.Json.Formatting.None);
                string paramsEncoded = HttpUtility.UrlEncode(paramsJson);
                string bloksVersioningId = Settings.InstagramApi.BloksVersioningId;
                string payload = $"params={paramsEncoded}&bloks_versioning_id={bloksVersioningId}";

                using (var client = HttpClientFactory.Create(useCookies: false))
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, 
                        "https://i.instagram.com/api/v1/bloks/apps/com.bloks.www.fx.settings.security.two_factor.totp.settings/");
                    req.Headers.Add("User-Agent", Settings.InstagramApi.UserAgent);
                    req.Headers.Add("Authorization", session.AuthorizationPhone);
                    req.Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var res = await client.SendAsync(req);
                    var body = await res.Content.ReadAsStringAsync();

                    // Save response for debugging
                    try
                    {
                        var logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        if (!System.IO.Directory.Exists(logFolder))
                            System.IO.Directory.CreateDirectory(logFolder);

                        var logPath = System.IO.Path.Combine(logFolder, $"twofactor_status_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json");
                        System.IO.File.WriteAllText(logPath, body);
                    }
                    catch { /* ignore logging errors */ }

                    // Parse response - data is nested in layout.bloks_payload.data
                    var json = JObject.Parse(body);
                    var dataArray = json["layout"]?["bloks_payload"]?["data"] as JArray;

                    if (dataArray == null)
                    {
                        // Fallback: try direct data path (for compatibility)
                        dataArray = json["data"] as JArray;
                        if (dataArray == null)
                            return new TwoFactorStatus { Error = "No data array found in response" };
                    }

                    // Find the object with key "FX_TWO_FACTOR_TOTP_SEEDS:totp_seeds"
                    var totpSeedsObj = dataArray
                        .FirstOrDefault(item => 
                            item["type"]?.ToString() == "gs" &&
                            item["data"]?["key"]?.ToString() == "FX_TWO_FACTOR_TOTP_SEEDS:totp_seeds");

                    if (totpSeedsObj == null)
                    {
                        // No TOTP seeds found - 2FA might be disabled
                        return new TwoFactorStatus
                        {
                            IsTotpEnabled = false,
                            CanAddAdditional = true,
                            Seeds = new System.Collections.Generic.List<TotpSeed>()
                        };
                    }

                    // Extract initial array containing device list
                    var initialArray = totpSeedsObj["data"]?["initial"] as JArray;
                    var status = new TwoFactorStatus
                    {
                        IsTotpEnabled = initialArray != null && initialArray.Count > 0,
                        CanAddAdditional = true,
                        Seeds = new System.Collections.Generic.List<TotpSeed>()
                    };

                    if (initialArray != null)
                    {
                        foreach (var device in initialArray)
                        {
                            status.Seeds.Add(new TotpSeed
                            {
                                Id = device["id"]?.ToString(),
                                Name = device["name"]?.ToString()
                            });
                        }
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                return new TwoFactorStatus { Error = ex.Message };
            }
        }

        private string ExtractAction(JToken token)
        {
            if (token == null) return null;
            var direct = token["action"]?.ToString();
            if (!string.IsNullOrEmpty(direct)) return Regex.Unescape(direct).Trim();
            var layoutAction = token["layout"]?["action"]?.ToString();
            if (!string.IsNullOrEmpty(layoutAction)) return Regex.Unescape(layoutAction).Trim();
            var bpAction = token["layout"]?["bloks_payload"]?["action"]?.ToString();
            if (!string.IsNullOrEmpty(bpAction)) return Regex.Unescape(bpAction).Trim();
            var treeAction = token.SelectToken("layout.bloks_payload.tree..action")?.ToString();
            if (!string.IsNullOrEmpty(treeAction)) return Regex.Unescape(treeAction).Trim();
            return RecursiveFindAction(token);
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
