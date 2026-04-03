using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Managers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class InstagramPhoneLoginService
    {
        private const string LOGIN_URL = "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.bloks.caa.login.async.send_login_request/";

        // 🎯 SETTINGS: Now loaded from appsettings.json instead of hardcoded
        private static AppSettings Settings => SettingsManager.LoadSettings();

        // 🚀 OPTIMIZATION: Cache RSA Public Key (parse once) - Now uses settings
        private static readonly Lazy<AsymmetricKeyParameter> _cachedPublicKey = new Lazy<AsymmetricKeyParameter>(() =>
        {
            try
            {
                var publicKeyBase64 = Settings.InstagramApi.PublicKeyBase64;
                byte[] keyBytes;

                // Auto-detect format: PEM (old format) vs DER (new format)
                try
                {
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(publicKeyBase64));

                    // Check if it's PEM format (contains "-----BEGIN PUBLIC KEY-----")
                    if (decoded.Contains("-----BEGIN PUBLIC KEY-----"))
                    {
                        // OLD FORMAT: PEM wrapped, need to decode twice
                        var publicKeyPem = decoded
                            .Replace("-----BEGIN PUBLIC KEY-----", "")
                            .Replace("-----END PUBLIC KEY-----", "")
                            .Replace("\n", "")
                            .Replace("\r", "");
                        keyBytes = Convert.FromBase64String(publicKeyPem);
                    }
                    else
                    {
                        // NEW FORMAT: Pure DER base64, already decoded
                        keyBytes = Convert.FromBase64String(publicKeyBase64);
                    }
                }
                catch
                {
                    // If first decode fails, assume it's new format (pure DER base64)
                    keyBytes = Convert.FromBase64String(publicKeyBase64);
                }

                return PublicKeyFactory.CreateKey(keyBytes);
            }
            catch (Exception ex)
            {
                // Fallback to hardcoded default if settings key is invalid
                const string DEFAULT_PUBLIC_KEY_BASE64 = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqb8PlCRf63Tv6/Lo+rZEwZ6uu7qKriAQhlqeSjUnzKqWXFj9iP2MWsMBpZCTSHZRZ8UI2wKZj1zPi/QXTi+NGHUWFogJwJY+1Bk1w21vpQgCvH0nyafAUDuTPTqU4SwVQYBpKruYFA3qQHmvvx5Ym9uMwxutkf2x9llfVOyJG1RamrGxkAb7jLwcy+PRoLHp554aa1Pp3XFkeFDzKyNDoN/VAZFmTR7HRl5H+MgCYCFKoyvqnJHhUaCkqAlHLtKrBzU8jBmsA5z/+7xhxz+9icnz+VYAfwFDHAAZf4aH+aDA6JWG+fohC7nCPJCvjtIl2tErRrAnbVwKDdbT45qlZwIDAQAB";

                Console.WriteLine($"⚠️ Lỗi khi load RSA Public Key từ settings: {ex.Message}");
                Console.WriteLine("⚠️ Sử dụng default key thay thế. Vui lòng kiểm tra lại PublicKeyBase64 trong Settings.");

                var keyBytes = Convert.FromBase64String(DEFAULT_PUBLIC_KEY_BASE64);
                return PublicKeyFactory.CreateKey(keyBytes);
            }
        });

        // 🚀 OPTIMIZATION: Compiled Regex patterns (cached)
        private static readonly Regex _error2FARegex = new Regex(
            @"\(dkc\s+""([^""]+)""\s+""two_factor_login""\s+""([^""]+)""",
            RegexOptions.Compiled | RegexOptions.Singleline
        );

        private static readonly Regex _errorDialogRegex = new Regex(
            @"fom 13799[^""]*40\s*""([^""]*)""\s*35\s*""([^""]*)""",
            RegexOptions.Compiled
        );

        private static readonly Regex _loginResponseRegex = new Regex(
            @"\\""login_response\\"":\\""(.*?)\\"",\\""headers\\""",
            RegexOptions.Compiled
        );

        private static readonly Regex _headersRegex = new Regex(
            @"\\""headers\\"":\\""(.*?)\\"",\\""cookies\\""",
            RegexOptions.Compiled
        );

        private static readonly Regex _unicodeRegex = new Regex(
            @"\\u([0-9A-Fa-f]{4})",
            RegexOptions.Compiled
        );

        // 🌐 PROXY: Create new HttpClient each time to pick up latest settings
        // Note: Trong production nên cache và recreate khi settings thay đổi
        private static HttpClient GetHttpClient()
        {
            return HttpClientFactory.Create(useCookies: false);
        }

        public class LoginResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string SessionId { get; set; }
            public string DsUserId { get; set; }
            public string CsrfToken { get; set; }
            public string Authorization { get; set; }

            // User info from login response
            public string FbAccountId { get; set; }      // fbid_v2
            public string PhoneAccountId { get; set; }   // pk_id
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Avatar { get; set; }
            public string Phone { get; set; }

            // 2FA support
            public bool Requires2FA { get; set; }
            public string TwoFactorContext { get; set; }  // two_step_verification_context
            public string DeviceId { get; set; }          // android-xxxxx
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            try
            {
                // 🌐 PROXY: Get HttpClient với settings hiện tại (bao gồm proxy)
                var httpClient = GetHttpClient();

                // 🚀 OPTIMIZATION: Generate IDs efficiently
                var guidString = Guid.NewGuid().ToString("N");
                var androidId = string.Concat("android-", guidString.Substring(0, 16));
                var familyDeviceId = Guid.NewGuid().ToString();

                // Encrypt password (uses cached public key)
                string encryptedPassword = EncryptPassword(password);

                // Build request body
                var bodyContent = BuildRequestBody(username, encryptedPassword, androidId, familyDeviceId);

                // Set headers
                SetRequestHeaders(httpClient);

                // Send request with ConfigureAwait(false) for better performance
                var content = new StringContent(bodyContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(LOGIN_URL, content).ConfigureAwait(false);

                // Read and decompress response
                string responseBody = await ReadResponseAsync(response).ConfigureAwait(false);

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
                // 🚀 OPTIMIZATION: Direct content read with buffer pooling
                var contentEncoding = response.Content.Headers.ContentEncoding;
                bool isGzipped = contentEncoding.Contains("gzip");

                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    // If not explicitly marked as gzip, check magic bytes
                    if (!isGzipped)
                    {
                        // Rent buffer from pool instead of allocating new array
                        var buffer = ArrayPool<byte>.Shared.Rent(2);
                        try
                        {
                            var bytesRead = await responseStream.ReadAsync(buffer, 0, 2).ConfigureAwait(false);
                            if (bytesRead == 2 && buffer[0] == 0x1F && buffer[1] == 0x8B)
                            {
                                isGzipped = true;
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(buffer);
                        }

                        // Reset stream or re-read
                        if (responseStream.CanSeek)
                        {
                            responseStream.Seek(0, SeekOrigin.Begin);
                        }
                        else
                        {
                            using (var newStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                return await DecompressAndReadAsync(newStream, isGzipped).ConfigureAwait(false);
                            }
                        }
                    }

                    return await DecompressAndReadAsync(responseStream, isGzipped).ConfigureAwait(false);
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
                // 🚀 OPTIMIZATION: Use pooled buffer for decompression
                using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress, leaveOpen: true))
                using (var reader = new StreamReader(gzipStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: false))
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
            else
            {
                using (var reader = new StreamReader(responseStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: false))
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
        }

        public string EncryptPassword(string encrypt_password)
        {
            try
            {
                // 🚀 OPTIMIZATION: Use cached public key instead of parsing every time
                var rsaPublicKey = _cachedPublicKey.Value;

                // Timestamp
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Generate AES KEY + IV
                var random = new SecureRandom();
                byte[] aesKey = new byte[32]; // AES-256
                byte[] iv = new byte[12];     // GCM IV
                random.NextBytes(aesKey);
                random.NextBytes(iv);

                // AAD uses timestamp string
                byte[] aad = Encoding.UTF8.GetBytes(timestamp.ToString());

                // Plaintext
                byte[] plaintext = Encoding.UTF8.GetBytes(encrypt_password);

                // AES-GCM Encryption
                byte[] encrypted = AesGcmEncrypt(aesKey, iv, plaintext, aad);

                // Split ciphertext and tag
                int ciphertextLength = encrypted.Length - 16;
                byte[] ciphertext = new byte[ciphertextLength];
                byte[] tag = new byte[16];
                Buffer.BlockCopy(encrypted, 0, ciphertext, 0, ciphertextLength);
                Buffer.BlockCopy(encrypted, ciphertextLength, tag, 0, 16);

                // RSA PKCS1_v1_5 encryption of AES key
                byte[] encryptedKey = RsaPkcs1Encrypt(rsaPublicKey, aesKey);

                // 🚀 OPTIMIZATION: Use MemoryStream with initial capacity
                using (var ms = new MemoryStream(2 + iv.Length + 2 + encryptedKey.Length + tag.Length + ciphertext.Length))
                using (var bw = new BinaryWriter(ms))
                {
                    // 🎯 SETTINGS: Use encryption key ID from settings
                    var encryptionKeyId = Settings.InstagramApi.EncryptionKeyId;

                    bw.Write((byte)1); // version
                    bw.Write((byte)encryptionKeyId);
                    bw.Write(iv);

                    // Length of encryptedKey - LITTLE-ENDIAN
                    ushort len = (ushort)encryptedKey.Length;
                    bw.Write((byte)(len & 0xFF));
                    bw.Write((byte)(len >> 8));

                    bw.Write(encryptedKey);
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

        private byte[] RsaPkcs1Encrypt(AsymmetricKeyParameter publicKey, byte[] data)
        {
            var engine = new Org.BouncyCastle.Crypto.Encodings.Pkcs1Encoding(
                new Org.BouncyCastle.Crypto.Engines.RsaEngine()
            );

            engine.Init(true, publicKey);
            return engine.ProcessBlock(data, 0, data.Length);
        }

        private string BuildRequestBody(string username, string encryptedPassword, string androidId, string familyDeviceId)
        {
            // 🚀 OPTIMIZATION: Use StringBuilder for better performance
            var clientInputParams = new
            {
                password = encryptedPassword,
                contact_point = username
            };

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

            // 🚀 OPTIMIZATION: Serialize once with optimized settings
            var paramsJson = JsonConvert.SerializeObject(paramsObj, Formatting.None);
            var encodedParams = HttpUtility.UrlEncode(paramsJson);

            // 🎯 SETTINGS: Use bloks versioning ID from settings
            var bloksVersioningId = Settings.InstagramApi.BloksVersioningId;

            // Use StringBuilder for concatenation
            return new StringBuilder(encodedParams.Length + bloksVersioningId.Length + 30)
                .Append("params=")
                .Append(encodedParams)
                .Append("&bloks_versioning_id=")
                .Append(bloksVersioningId)
                .ToString();
        }

        private void SetRequestHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();

            // 🎯 SETTINGS: Use User-Agent from settings
            var userAgent = Settings.InstagramApi.UserAgent;
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        private LoginResult ParseLoginResponse(HttpResponseMessage response, string responseBody)
        {
            try
            {
                string sessionId = null;
                string dsUserId = null;
                string csrfToken = null;
                string authorization = null;
                string fbAccountId = null;
                string phoneAccountId = null;
                string username = null;
                string fullName = null;
                string avatar = null;
                string phone = null;

                // 🚀 OPTIMIZATION: Extract cookies efficiently
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        int startIdx, endIdx;

                        if ((startIdx = cookie.IndexOf("sessionid=", StringComparison.Ordinal)) >= 0)
                        {
                            startIdx += 10; // Length of "sessionid="
                            endIdx = cookie.IndexOf(';', startIdx);
                            sessionId = endIdx > startIdx ? cookie.Substring(startIdx, endIdx - startIdx) : cookie.Substring(startIdx);
                        }
                        else if ((startIdx = cookie.IndexOf("ds_user_id=", StringComparison.Ordinal)) >= 0)
                        {
                            startIdx += 11;
                            endIdx = cookie.IndexOf(';', startIdx);
                            dsUserId = endIdx > startIdx ? cookie.Substring(startIdx, endIdx - startIdx) : cookie.Substring(startIdx);
                        }
                        else if ((startIdx = cookie.IndexOf("csrftoken=", StringComparison.Ordinal)) >= 0)
                        {
                            startIdx += 10;
                            endIdx = cookie.IndexOf(';', startIdx);
                            csrfToken = endIdx > startIdx ? cookie.Substring(startIdx, endIdx - startIdx) : cookie.Substring(startIdx);
                        }
                    }
                }

                // 🚀 OPTIMIZATION: Parse JSON with optimized settings
                JObject json;
                using (var stringReader = new StringReader(responseBody))
                using (var jsonReader = new JsonTextReader(stringReader) { MaxDepth = 128 })
                {
                    json = JObject.Load(jsonReader);
                }

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
                        // 🚀 OPTIMIZATION: Use cached compiled regex
                        if (actionField.Contains("[trait:error_2fa_bloks]"))
                        {
                            var blockMatch = _error2FARegex.Match(actionField);
                            if (blockMatch.Success)
                            {
                                return new LoginResult
                                {
                                    Success = false,
                                    Requires2FA = true,
                                    Message = "⚠️ Tài khoản yêu cầu mã xác thực 2FA",
                                    TwoFactorContext = blockMatch.Groups[1].Value,
                                    DeviceId = blockMatch.Groups[2].Value,
                                    CsrfToken = csrfToken
                                };
                            }
                        }

                        // Check for "login_failed" event
                        if (actionField.Contains("login_failed") || actionField.Contains("login_wrong_password"))
                        {
                            var errorMatch = _errorDialogRegex.Match(actionField);
                            if (errorMatch.Success)
                            {
                                var errorMessage = DecodeUnicode(errorMatch.Groups[2].Value);
                                return new LoginResult { Success = false, Message = $"❌ {errorMessage}" };
                            }

                            // Fallback
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
                    }

                    // Extract login_response directly from action field if it contains it
                    if (!string.IsNullOrEmpty(actionField) && actionField.Contains("login_response"))
                    {
                        try
                        {
                            // 🚀 OPTIMIZATION: Use cached regex
                            var loginResponseMatch = _loginResponseRegex.Match(actionField);
                            if (loginResponseMatch.Success)
                            {
                                var loginResponseJson = loginResponseMatch.Groups[1].Value
                                    .Replace("\\\\\\\"", "\"")
                                    .Replace("\\\\/", "/")
                                    .Replace("\\\\\\\\", "\\");

                                try
                                {
                                    var loginData = JObject.Parse(loginResponseJson);
                                    var loggedInUser = loginData["logged_in_user"];
                                    if (loggedInUser != null)
                                    {
                                        fbAccountId = loggedInUser["fbid_v2"]?.ToString();
                                        phoneAccountId = loggedInUser["pk_id"]?.ToString();
                                        username = loggedInUser["username"]?.ToString();
                                        fullName = loggedInUser["full_name"]?.ToString();
                                        avatar = loggedInUser["profile_pic_url"]?.ToString();
                                        phone = loggedInUser["phone_number"]?.ToString();
                                    }

                                    if (string.IsNullOrEmpty(dsUserId))
                                    {
                                        dsUserId = loginData["logged_in_user"]?["pk"]?.ToString() ??
                                                  loginData["logged_in_user"]?["pk_id"]?.ToString();
                                    }
                                }
                                catch { }
                            }

                            // Extract headers JSON string (using cached regex)
                            var headersMatch = _headersRegex.Match(actionField);
                            if (headersMatch.Success)
                            {
                                var headersJson = headersMatch.Groups[1].Value
                                    .Replace("\\\\\\\"", "\"")
                                    .Replace("\\\\\\\\", "\\");

                                try
                                {
                                    var headersData = JObject.Parse(headersJson);
                                    var authHeader = headersData["IG-Set-Authorization"]?.ToString();

                                    if (!string.IsNullOrEmpty(authHeader))
                                    {
                                        authorization = authHeader;

                                        // Decode base64 JWT
                                        if (authorization.StartsWith("Bearer IGT:2:"))
                                        {
                                            var jwtPart = authorization.Substring(13); // Length of "Bearer IGT:2:"
                                            try
                                            {
                                                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(jwtPart));
                                                var jwtData = JObject.Parse(decoded);

                                                if (string.IsNullOrEmpty(sessionId))
                                                {
                                                    sessionId = jwtData["sessionid"]?.ToString();
                                                    if (!string.IsNullOrEmpty(sessionId))
                                                    {
                                                        sessionId = HttpUtility.UrlDecode(sessionId);
                                                    }
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
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

                                    // Extract user info
                                    var loggedInUser = loginData["logged_in_user"];
                                    if (loggedInUser != null)
                                    {
                                        fbAccountId = loggedInUser["fbid_v2"]?.ToString();
                                        phoneAccountId = loggedInUser["pk_id"]?.ToString();
                                        username = loggedInUser["username"]?.ToString();
                                        fullName = loggedInUser["full_name"]?.ToString();
                                        avatar = loggedInUser["profile_pic_url"]?.ToString();
                                        phone = loggedInUser["phone_number"]?.ToString();
                                    }

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
                    // Download avatar if available
                    string avatarLocalPath = null;
                    if (!string.IsNullOrEmpty(avatar))
                    {
                        string prefix = "https://instagram.fbmv1-1.fna.fbcdn.net/";
                        avatar = avatar.Replace("\\/", "/");
                        if (avatar.StartsWith(prefix))
                        {
                            avatar = avatar.Substring(prefix.Length);
                        }
                        avatar = avatar.Replace("//", "/");
                        avatar = prefix + avatar;
                        avatarLocalPath = DownloadAvatar(avatar, username);
                    }

                    return new LoginResult
                    {
                        Success = true,
                        Message = "✅ Đăng nhập thành công!",
                        SessionId = sessionId,
                        DsUserId = dsUserId,
                        CsrfToken = csrfToken,
                        Authorization = authorization,
                        FbAccountId = fbAccountId,
                        PhoneAccountId = phoneAccountId,
                        Username = username,
                        FullName = fullName,
                        Avatar = avatarLocalPath ?? avatar, // Use local path if download successful, otherwise use URL
                        Phone = phone
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

        public async Task<LoginResult> Verify2FA(string code, string twoFactorContext, string deviceId, string csrfToken)
        {
            try
            {
                // 🚀 OPTIMIZATION: Reuse shared HttpClient
                var paramsJson = new JObject
                {
                    ["client_input_params"] = new JObject
                    {
                        ["block_store_machine_id"] = "",
                        ["code"] = code,
                        ["device_id"] = deviceId
                    },
                    ["server_params"] = new JObject
                    {
                        ["INTERNAL__latency_qpl_marker_id"] = 36707139,
                        ["challenge"] = "totp",
                        ["INTERNAL__latency_qpl_instance_id"] = 74494577800297,
                        ["two_step_verification_context"] = twoFactorContext,
                        ["flow_source"] = "two_factor_login"
                    }
                };

                // Build body content (optimized with StringBuilder)
                var encodedParams = HttpUtility.UrlEncode(paramsJson.ToString(Formatting.None));

                // 🎯 SETTINGS: Use bloks versioning ID from settings
                var bloksVersioningId = Settings.InstagramApi.BloksVersioningId;

                var bodyContent = new StringBuilder(encodedParams.Length + bloksVersioningId.Length + 30)
                    .Append("params=")
                    .Append(encodedParams)
                    .Append("&bloks_versioning_id=")
                    .Append(bloksVersioningId)
                    .ToString();

                // 🌐 PROXY: Get HttpClient với settings hiện tại
                var httpClient = GetHttpClient();

                // Set headers
                SetRequestHeaders(httpClient);

                // Send request with ConfigureAwait(false)
                var content = new StringContent(bodyContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(
                    "https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.two_step_verification.verify_code.async/", 
                    content
                ).ConfigureAwait(false);

                // Read and decompress response
                string responseBody = await ReadResponseAsync(response).ConfigureAwait(false);

                // Parse response
                return ParseLoginResponse(response, responseBody);
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = $"❌ Lỗi xác thực 2FA: {ex.Message}"
                };
            }
        }

        private string DownloadAvatar(string avatarUrl, string username)
        {
            // 🎯 SETTINGS: Check if avatar download is enabled
            if (!Settings.Application.DownloadAvatars || string.IsNullOrEmpty(avatarUrl))
                return null;

            try
            {
                // 🎯 SETTINGS: Use avatar folder from settings
                string avatarFolder = Path.Combine(Application.StartupPath, Settings.Application.AvatarCacheFolder);
                if (!Directory.Exists(avatarFolder))
                    Directory.CreateDirectory(avatarFolder);

                // 🚀 OPTIMIZATION: Use simpler hash calculation
                string fileHash;
                using (var sha = SHA256.Create())
                {
                    var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(avatarUrl));
                    // Convert to hex more efficiently
                    fileHash = BitConverter.ToString(hashBytes, 0, 4).Replace("-", "").ToLowerInvariant();
                }

                string fileName = $"{username ?? "avatar"}_{fileHash}.jpg";
                string avatarLocalPath = Path.Combine(avatarFolder, fileName);

                // Only download if file doesn't exist
                if (!File.Exists(avatarLocalPath))
                {
                    // 🌐 PROXY: Use HttpClient with proxy settings
                    using (var httpClient = GetHttpClient())
                    {
                        var bytes = httpClient.GetByteArrayAsync(avatarUrl).GetAwaiter().GetResult();
                        File.WriteAllBytes(avatarLocalPath, bytes);
                    }
                }

                return avatarLocalPath;
            }
            catch (Exception ex)
            {
                if (Settings.Application.EnableDebugLogging)
                {
                    Console.WriteLine($"Lỗi tải avatar: {ex.Message}");
                }
                return null;
            }
        }

        private string DecodeUnicode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 🚀 OPTIMIZATION: Use cached compiled regex
            return _unicodeRegex.Replace(
                input,
                match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString()
            );
        }
    }
}
