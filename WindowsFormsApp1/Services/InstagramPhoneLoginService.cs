using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace WindowsFormsApp1.Services
{
    public class InstagramPhoneLoginService
    {
        private const string LOGIN_URL = "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.bloks.caa.login.async.send_login_request/";
        private const int ENCRYPTION_KEY_ID = 228;
        private const string PUBLIC_KEY_BASE64 = "LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0KTUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUFxYjhQbENSZjYzVHY2L0xvK3JaRQp3WjZ1dTdxS3JpQVFobHFlU2pVbnpLcVdYRmo5aVAyTVdzTUJwWkNUU0haUlo0VUkyd0taajF6UGkvUVhUaStOCkdIVVdGb2dKd0pZKzFCazF3MjF2cFFnQ3ZIMG55YWZBVUR1VFBUcVU0U3dWUVlCcEtydVlGQTNxUUhtdnZ4NVkKbTl1TXd4dXRrZjJ4OWxsZlZPeUpHMVJhbXJHeGtBYjdqTHdjeStQUm9MSHA1NTRhYTFQcDNYRmtlRkR6S3lORApvTi9WQVpGbVRSN0hSbDVIK01nQ1lDRktveXZxbkpIaFVhQ2txQWxITHRLckJ6VThqQm1zQTV6Lys3eGh4eis5CmljbnorVllBZndGREhBQVpmNGFIK2FEQTZKV0crZm9oQzduQ1BKQ3ZqdElsMnRFclJyQW5iVndLRGRiVDQ1cWwKWndJREFRQUIKLS0tLS1FTkQgUFVCTElDIEtFWS0tLS0tCg==";

        public class LoginResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string SessionId { get; set; }
            public string DsUserId { get; set; }
            public string CsrfToken { get; set; }
            public string Authorization { get; set; }
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            try
            {
                var httpClient = HttpClientFactory.Create(useCookies: false);

                // Generate device IDs (will be used in both body and headers)
                var androidId = $"android-{Guid.NewGuid().ToString("N").Substring(0, 16)}";
                var familyDeviceId = Guid.NewGuid().ToString();

                // Encrypt password
                string encryptedPassword = EncryptPassword(password);
                //string encryptedPassword = "#PWD_INSTAGRAM:4:1775035015:AeN+uierSrLTq7wLnNsAAZG2XEw9Nglc2PGcV4m97l0mzhrNg2TQ+g4k7fwpY/rfQ911xlTOGutqN/X3P1t5nWgnj9dQDen9tRVVPKV3U8ArUdJh3i7H5ihKfgzpYHObcZ8NqDpY7Kc0b8C8koIcIgrAOWmd+/HJwNmc4+N5+BM98nrQF9KfQbb18Jkik2rB2EH9n7rk/HhKOkmRFWKCNqYMpMxN+yg6vSz49ZKy055hfKhMaT/96Kr941nQpIhZhXKFgb7cyWNE7phuyW7RmaKNm2c53uObdMB5b1I4B8KsChMHOyyozDz6Ndx3hp28TUt6o1Z2Hq4lE7+H+tSeV2m/0scJswazwWRdV1uCoYK9fgXnirdjvVBHlmnh5I1tC0OXXuHg2Uxd";
                // Build request body with device IDs
                var bodyContent = BuildRequestBody(username, encryptedPassword, androidId, familyDeviceId);

                // Set headers
                SetRequestHeaders(httpClient);

                // Send request
                var content = new StringContent(bodyContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(LOGIN_URL, content);

                // Read and decompress response (Instagram sends GZIP compressed data)
                string responseBody = await ReadResponseAsync(response);

                // Parse response
                return ParseLoginResponse(response, responseBody);
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = $"Lỗi kết nối: {ex.Message}"
                };
            }
        }

        private async Task<string> ReadResponseAsync(HttpResponseMessage response)
        {
            try
            {
                // Get the raw response stream
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    // Check if response is GZIP compressed
                    var contentEncoding = response.Content.Headers.ContentEncoding;
                    bool isGzipped = contentEncoding.Contains("gzip");

                    // If not explicitly marked as gzip, check magic bytes
                    if (!isGzipped)
                    {
                        // Read first 2 bytes to check for GZIP magic number (1F 8B)
                        var buffer = new byte[2];
                        var bytesRead = await responseStream.ReadAsync(buffer, 0, 2);

                        if (bytesRead == 2 && buffer[0] == 0x1F && buffer[1] == 0x8B)
                        {
                            isGzipped = true;
                        }

                        // Reset stream position
                        if (responseStream.CanSeek)
                        {
                            responseStream.Seek(0, SeekOrigin.Begin);
                        }
                        else
                        {
                            // If can't seek, we need to re-read the content
                            using (var newStream = await response.Content.ReadAsStreamAsync())
                            {
                                return await DecompressAndReadAsync(newStream, isGzipped);
                            }
                        }
                    }

                    return await DecompressAndReadAsync(responseStream, isGzipped);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi đọc response: {ex.Message}");
            }
        }

        private async Task<string> DecompressAndReadAsync(Stream responseStream, bool isGzipped)
        {
            if (isGzipped)
            {
                // Decompress GZIP data
                using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress, leaveOpen: true))
                using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            else
            {
                // Read as plain text
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public string EncryptPassword(string encrypt_password)
        {
            try
            {
                // ===== 1. Parse public key =====
                var publicKeyPem = Encoding.UTF8.GetString(Convert.FromBase64String(PUBLIC_KEY_BASE64))
                    .Replace("-----BEGIN PUBLIC KEY-----", "")
                    .Replace("-----END PUBLIC KEY-----", "")
                    .Replace("\n", "")
                    .Replace("\r", "");

                var keyBytes = Convert.FromBase64String(publicKeyPem);
                var rsaPublicKey = DecodeX509PublicKey(keyBytes);

                // ===== 2. Timestamp =====
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // ===== 3. AES KEY + IV =====
                var random = new SecureRandom();

                byte[] aesKey = new byte[16]; // AES-128
                byte[] iv = new byte[12];     // GCM IV

                random.NextBytes(aesKey);
                random.NextBytes(iv);

                // ===== 4. AAD =====
                // AAD is empty
                byte[] aad = new byte[0];

                // ===== 5. PLAINTEXT =====
                // passwordBytes is timestamp:password
                string combinedPassword = $"{timestamp}:{encrypt_password}";
                byte[] plaintext = Encoding.UTF8.GetBytes(combinedPassword);

                // ===== 6. AES-GCM =====
                byte[] encrypted = AesGcmEncrypt(aesKey, iv, plaintext, aad);

                // tách ra
                byte[] ciphertext = encrypted.Take(encrypted.Length - 16).ToArray();
                byte[] tag = encrypted.Skip(encrypted.Length - 16).ToArray();

                // ===== 7. RSA OAEP SHA256 =====
                byte[] encryptedKey = RsaEncryptOAEP(rsaPublicKey, aesKey);

                // ===== 7. BUILD PAYLOAD =====
                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write((byte)1); // version
                    bw.Write((byte)ENCRYPTION_KEY_ID);
                    bw.Write(iv);

                    // length of encryptedKey - MUST BE LITTLE-ENDIAN for Instagram App!
                    ushort len = (ushort)encryptedKey.Length; // 256 for RSA-2048
                    bw.Write((byte)(len & 0xFF)); // Low byte (0x00)
                    bw.Write((byte)(len >> 8));   // High byte (0x01)

                    bw.Write(encryptedKey);

                    // Instagram puts TAG first, then ciphertext
                    bw.Write(tag);
                    bw.Write(ciphertext);

                    string base64 = Convert.ToBase64String(ms.ToArray());

                    return $"#PWD_INSTAGRAM:4:{timestamp}:{base64}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Encrypt fail: " + ex.Message);
            }
        }

        private byte[] AesGcmEncrypt(byte[] key, byte[] iv, byte[] data, byte[] aad)
        {
            var cipher = new Org.BouncyCastle.Crypto.Modes.GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(key), 128, iv, aad);
            cipher.Init(true, parameters);

            byte[] output = new byte[cipher.GetOutputSize(data.Length)];
            int len = cipher.ProcessBytes(data, 0, data.Length, output, 0);
            cipher.DoFinal(output, len);

            return output;
        }

        private AsymmetricKeyParameter DecodeX509PublicKey(byte[] x509Key)
        {
            try
            {
                return PublicKeyFactory.CreateKey(x509Key);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse RSA public key: {ex.Message}");
            }
        }

        private byte[] RsaEncryptOAEP(AsymmetricKeyParameter publicKey, byte[] data)
        {
            var engine = new Org.BouncyCastle.Crypto.Encodings.OaepEncoding(
                new Org.BouncyCastle.Crypto.Engines.RsaEngine(),
                new Org.BouncyCastle.Crypto.Digests.Sha256Digest(),   // hash
                new Org.BouncyCastle.Crypto.Digests.Sha256Digest(),   // MGF1 = SHA256
                null
            );

            engine.Init(true, publicKey);
            return engine.ProcessBlock(data, 0, data.Length);
        }

        private string BuildRequestBody(string username, string encryptedPassword, string androidId, string familyDeviceId)
        {
            // Simplified client_input_params (only 2 fields)
            var clientInputParams = new
            {
                password = encryptedPassword,
                contact_point = username
            };

            // Server params with required fields (matching sample)
            var serverParams = new
            {
                is_platform_login = 0,
                device_id = androidId,
                login_surface = "aymh_manual_login",
                INTERNAL__latency_qpl_instance_id = 181284589401156L,
                reg_flow_source = "login_home_native_integration_point",
                is_caa_perf_enabled = 1,
                credential_type = "password",
                is_from_password_entry_page = 0,
                caller = "gslr",
                family_device_id = familyDeviceId
            };

            var paramsObj = new
            {
                client_input_params = clientInputParams,
                server_params = serverParams
            };

            var paramsJson = JsonConvert.SerializeObject(paramsObj);

            // Only params and bloks_versioning_id (no bk_client_context)
            var body = $"params={HttpUtility.UrlEncode(paramsJson)}" +
                      $"&bloks_versioning_id=899adff463607d5f13a547f7417a9de4a8b4add115ddebc553c1bc5b8d48a28a";

            return body;
        }

        private void SetRequestHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();

            // Only 2 basic headers
            client.DefaultRequestHeaders.Add("User-Agent", "Instagram 423.0.0.47.66 Android (28/9; 480dpi; 1080x1920; Redmi; 22127RK46C; marlin; qcom; en_US; 923309183)");
            // Content-Type is set automatically by StringContent
        }

        private LoginResult ParseLoginResponse(HttpResponseMessage response, string responseBody)
        {
            try
            {
                string sessionId = null;
                string dsUserId = null;
                string csrfToken = null;
                string authorization = null;

                // Extract cookies from Set-Cookie headers
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        if (cookie.Contains("sessionid="))
                        {
                            var start = cookie.IndexOf("sessionid=") + "sessionid=".Length;
                            var end = cookie.IndexOf(';', start);
                            sessionId = end > start ? cookie.Substring(start, end - start) : cookie.Substring(start);
                        }
                        else if (cookie.Contains("ds_user_id="))
                        {
                            var start = cookie.IndexOf("ds_user_id=") + "ds_user_id=".Length;
                            var end = cookie.IndexOf(';', start);
                            dsUserId = end > start ? cookie.Substring(start, end - start) : cookie.Substring(start);
                        }
                        else if (cookie.Contains("csrftoken="))
                        {
                            var start = cookie.IndexOf("csrftoken=") + "csrftoken=".Length;
                            var end = cookie.IndexOf(';', start);
                            csrfToken = end > start ? cookie.Substring(start, end - start) : cookie.Substring(start);
                        }
                    }
                }

                // Parse Bloks response
                var json = JObject.Parse(responseBody);

                // Check status
                var status = json["status"]?.ToString();
                if (status == "fail" || json["error_type"] != null)
                {
                    var errorMessage = json["message"]?.ToString() ?? json["error_type"]?.ToString() ?? "Đăng nhập thất bại";
                    return new LoginResult { Success = false, Message = $"❌ {DecodeUnicode(errorMessage)}" };
                }

                // Check for errors in Bloks action field
                var layoutPayload = json["layout"]?["bloks_payload"];
                if (layoutPayload != null)
                {
                    var actionField = layoutPayload["action"]?.ToString();

                    // Check for error indicators in action
                    if (!string.IsNullOrEmpty(actionField))
                    {
                        // Check for "login_failed" event
                        if (actionField.Contains("login_failed") || actionField.Contains("login_wrong_password"))
                        {
                            // Extract error message using regex for fom 13799 (error dialog)
                            // Pattern: 40 "Title" 35 "Message"
                            var errorMatch = System.Text.RegularExpressions.Regex.Match(
                                actionField, 
                                @"fom 13799[^""]*40\s*""([^""]*)""\s*35\s*""([^""]*)"""
                            );

                            if (errorMatch.Success)
                            {
                                var errorTitle = DecodeUnicode(errorMatch.Groups[1].Value);
                                var errorMessage = DecodeUnicode(errorMatch.Groups[2].Value);
                                return new LoginResult 
                                { 
                                    Success = false, 
                                    Message = $"❌ {errorMessage}" 
                                };
                            }

                            // Fallback: check for common error messages
                            if (actionField.Contains("kh\\u00f4ng ch\\u00ednh x\\u00e1c") || 
                                actionField.Contains("incorrect") ||
                                actionField.Contains("wrong_password"))
                            {
                                return new LoginResult 
                                { 
                                    Success = false, 
                                    Message = "❌ Mật khẩu không chính xác. Vui lòng thử lại." 
                                };
                            }
                        }

                        // Check for checkpoint/challenge
                        if (actionField.Contains("checkpoint") || actionField.Contains("challenge"))
                        {
                            return new LoginResult 
                            { 
                                Success = false, 
                                Message = "⚠️ Tài khoản cần xác minh. Vui lòng đăng nhập qua ứng dụng Instagram." 
                            };
                        }

                        // Check for 2FA
                        if (actionField.Contains("two_factor") || actionField.Contains("2fa"))
                        {
                            return new LoginResult 
                            { 
                                Success = false, 
                                Message = "⚠️ Tài khoản có bật 2FA. Tính năng này đang được phát triển." 
                            };
                        }
                    }

                    // Search in embedded_payloads for login_response (success case)
                    var embeddedPayloads = layoutPayload["embedded_payloads"] as JArray;
                    if (embeddedPayloads != null)
                    {
                        foreach (var payload in embeddedPayloads)
                        {
                            var payloadData = payload["payload"]?.ToString();
                            if (!string.IsNullOrEmpty(payloadData) && payloadData.Contains("login_response"))
                            {
                                // Extract login_response JSON string (it's escaped)
                                var loginResponseMatch = System.Text.RegularExpressions.Regex.Match(payloadData, @"\{\\""login_response\\"":\\""(.+?)\\""");
                                if (loginResponseMatch.Success)
                                {
                                    var loginResponseJson = loginResponseMatch.Groups[1].Value
                                        .Replace("\\\\\\\"", "\"")
                                        .Replace("\\\\/", "/")
                                        .Replace("\\\\\\\\", "\\");

                                    var loginData = JObject.Parse(loginResponseJson);

                                    // Extract ds_user_id from logged_in_user.pk
                                    if (string.IsNullOrEmpty(dsUserId))
                                    {
                                        dsUserId = loginData["logged_in_user"]?["pk"]?.ToString() ??
                                                  loginData["logged_in_user"]?["pk_id"]?.ToString();
                                    }
                                }

                                // Extract IG-Set-Authorization from headers in payload
                                var headersMatch = System.Text.RegularExpressions.Regex.Match(payloadData, @"\{\\\\\\""IG-Set-Authorization\\\\\\"":\s*\\\\\\""([^\\]+)\\\\\\""");
                                if (headersMatch.Success)
                                {
                                    authorization = headersMatch.Groups[1].Value.Replace("\\\\\\\"", "\"");

                                    // Decode base64 JWT to extract sessionid
                                    if (authorization.StartsWith("Bearer IGT:2:"))
                                    {
                                        var jwtPart = authorization.Substring("Bearer IGT:2:".Length);
                                        try
                                        {
                                            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(jwtPart));
                                            var jwtData = JObject.Parse(decoded);

                                            if (string.IsNullOrEmpty(sessionId))
                                            {
                                                sessionId = jwtData["sessionid"]?.ToString();
                                                // URL decode sessionid
                                                if (!string.IsNullOrEmpty(sessionId))
                                                {
                                                    sessionId = System.Web.HttpUtility.UrlDecode(sessionId);
                                                }
                                            }

                                            if (string.IsNullOrEmpty(dsUserId))
                                            {
                                                dsUserId = jwtData["ds_user_id"]?.ToString();
                                            }
                                        }
                                        catch { }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }

                // Validate we have minimum required data
                if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(dsUserId))
                {
                    return new LoginResult
                    {
                        Success = true,
                        Message = "✅ Đăng nhập thành công!",
                        SessionId = sessionId,
                        DsUserId = dsUserId,
                        CsrfToken = csrfToken,
                        Authorization = authorization
                    };
                }

                // If we reach here and have no session but status=ok, it might be an unhandled error
                return new LoginResult
                {
                    Success = false,
                    Message = "❌ Không thể đăng nhập. Vui lòng kiểm tra lại thông tin."
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = $"❌ Lỗi xử lý response: {ex.Message}"
                };
            }
        }

        private string DecodeUnicode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Decode Unicode escape sequences like \u1ea1 or \u00f4
            return System.Text.RegularExpressions.Regex.Replace(
                input,
                @"\\u([0-9A-Fa-f]{4})",
                match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString()
            );
        }
    }
}
