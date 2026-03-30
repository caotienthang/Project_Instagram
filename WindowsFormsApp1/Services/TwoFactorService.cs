using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class TwoFactorStatus
    {
        public bool IsTotpEnabled    { get; set; }
        public bool IsSmsEnabled     { get; set; }
        public bool IsFidoEnabled    { get; set; }
        public bool CanDisable       { get; set; }
        public bool CanAddAdditional { get; set; }
        public List<TotpSeed> Seeds  { get; set; } = new List<TotpSeed>();
        public string Error          { get; set; }
    }

    public class TotpSeed
    {
        public string Id   { get; set; }
        public string Name { get; set; }
    }

    public class TwoFactorActionResult
    {
        public bool   Success  { get; set; }
        public string Message  { get; set; }
        public string TotpSeed { get; set; }
    }

    public class TotpKeyResult
    {
        public bool   Success   { get; set; }
        public string KeyId     { get; set; }
        public string KeyText   { get; set; }
        public string QrCodeUri { get; set; }
        public string Message   { get; set; }
    }

    public class TwoFactorService
    {
        private const string AcApiUrl     = "https://accountscenter.instagram.com/api/graphql/";
        private const string FriendlyName = "FXAccountsCenterTwoFactorSettingsTOTPManagerDialogQuery";

        // ── Get 2FA status ───────────────────────────────────────────
        public async Task<TwoFactorStatus> GetStatusAsync(string accountId, InstagramSession session)
        {
            var client = BuildAcClient(session);

            var variables = JsonConvert.SerializeObject(new
            {
                account_id   = accountId,
                account_type = "INSTAGRAM",
                @interface   = "IG_WEB"
            });

            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "fb_api_req_friendly_name", FriendlyName                        },
                { "variables",                variables                            },
                { "doc_id",                   "25960508096941189"                               },
                { "fb_api_analytics_tags",    "[\"qpl_active_flow_ids=241970459\"]" },
                { "fb_dtsg",                  session.FbDtsg                      },
                { "lsd",                      session.Lsd                         }
            });

            try
            {
                var res  = await client.PostAsync(AcApiUrl, body);
                var json = await res.Content.ReadAsStringAsync();
                return ParseStatus(json);
            }
            catch (Exception ex)
            {
                return new TwoFactorStatus { Error = ex.Message };
            }
        }

        // ── Remove TOTP device (AccountsCenter GraphQL mutation) ────
        public async Task<TwoFactorActionResult> DisableTotpAsync(string accountId, InstagramSession session, string seedId = null)
        {
            try
            {
                var client = BuildAcClient(session);

                var variables = JsonConvert.SerializeObject(new
                {
                    input = new
                    {
                        actor_id           = accountId,
                        client_mutation_id = Guid.NewGuid().ToString(),
                        account_id         = accountId,
                        account_type       = "INSTAGRAM",
                        method_id          = seedId ?? "",
                        family_device_id   = "device_id_fetch_ig_did"
                    }
                });

                var body = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "fb_dtsg",                  session.FbDtsg                                   },
                    { "lsd",                      session.Lsd                                      },
                    { "fb_api_req_friendly_name", "useFXSettingsTwoFactorRemoveTOTPKeyMutation"    },
                    { "fb_api_analytics_tags",    "[\"qpl_active_flow_ids=241970459\"]"            },
                    { "doc_id",                   "29616201561327421"                              },
                    { "variables",                variables                                        }
                });

                var res  = await client.PostAsync(AcApiUrl, body);
                var json = await res.Content.ReadAsStringAsync();
                var obj  = JObject.Parse(json);

                var data = obj["data"]?["xfb_two_fac_totp_remove_key"];
                if (data?["success"]?.Value<bool>() == true)
                    return new TwoFactorActionResult { Success = true, Message = "Device removed" };

                return new TwoFactorActionResult
                {
                    Success = false,
                    Message = obj["errors"]?[0]?["message"]?.ToString() ?? $"HTTP {(int)res.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new TwoFactorActionResult { Success = false, Message = ex.Message };
            }
        }

        // ── Generate TOTP key — get QR + key text (AccountsCenter mutation) ──
        public async Task<TotpKeyResult> GenerateTotpKeyAsync(string accountId, InstagramSession session, string keyNickname = null)
        {
            try
            {
                var client = BuildAcClient(session);

                var variables = JsonConvert.SerializeObject(new
                {
                    input = new
                    {
                        actor_id           = accountId,
                        client_mutation_id = Guid.NewGuid().ToString(),
                        account_id         = accountId,
                        account_type       = "INSTAGRAM",
                        key_nickname       = keyNickname ?? "",
                        device_id          = "device_id_fetch_ig_did",
                        fdid               = "device_id_fetch_ig_did"
                    }
                });

                var body = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "fb_dtsg",                  session.FbDtsg                                 },
                    { "lsd",                      session.Lsd                                    },
                    { "fb_api_req_friendly_name", "useFXSettingsTwoFactorGenerateTOTPKeyMutation" },
                    { "doc_id",                   "9837172312995248"                             },
                    { "variables",                variables                                      },
                    { "fb_api_analytics_tags",    "[\"qpl_active_flow_ids=241970459\"]"          }
                });

                var res  = await client.PostAsync(AcApiUrl, body);
                var json = await res.Content.ReadAsStringAsync();
                var obj  = JObject.Parse(json);

                var totpKey = obj["data"]?["xfb_two_factor_generate_totp_key"];
                if (totpKey?["success"]?.Value<bool>() == true)
                    return new TotpKeyResult
                    {
                        Success   = true,
                        KeyId     = totpKey["totp_key"]?["key_id"]?.ToString(),
                        KeyText   = totpKey["totp_key"]?["key_text"]?.ToString(),
                        QrCodeUri = totpKey["totp_key"]?["qr_code_uri"]?.ToString()
                    };

                return new TotpKeyResult
                {
                    Success = false,
                    Message = obj["errors"]?[0]?["message"]?.ToString() ?? $"HTTP {(int)res.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new TotpKeyResult { Success = false, Message = ex.Message };
            }
        }

        // ── Confirm TOTP — verify 6-digit code to enable 2FA (AccountsCenter mutation) ──
        public async Task<TwoFactorActionResult> ConfirmTotpAsync(string accountId, string verificationCode, InstagramSession session)
        {
            try
            {
                var client = BuildAcClient(session);

                var variables = JsonConvert.SerializeObject(new
                {
                    input = new
                    {
                        actor_id           = accountId,
                        client_mutation_id = Guid.NewGuid().ToString(),
                        account_id         = accountId,
                        account_type       = "INSTAGRAM",
                        verification_code  = verificationCode,
                        device_id          = "device_id_fetch_ig_did",
                        fdid               = "device_id_fetch_ig_did"
                    }
                });

                var body = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"__a",                      "1"                                          },
                    {"__comet_req",             "24"                                          },
                    { "fb_dtsg",                  session.FbDtsg                              },
                    { "lsd",                      session.Lsd                                 },
                    { "fb_api_caller_class",      "RelayModern"                               },
                    { "fb_api_req_friendly_name", "useFXSettingsTwoFactorEnableTOTPMutation"  },
                    { "fb_api_analytics_tags",    "[\"qpl_active_flow_ids=241970459\"]"       },
                    { "server_timestamps",        "true"                                      },
                    { "doc_id",                   "29164158613231327"                         },
                    { "variables",                variables                                   }
                });

                var res  = await client.PostAsync(AcApiUrl, body);
                var json = await res.Content.ReadAsStringAsync();
                var obj  = JObject.Parse(json);

                var data = obj["data"]?["xfb_two_factor_enable_totp"];
                if (data?["success"]?.Value<bool>() == true)
                    return new TwoFactorActionResult { Success = true, Message = "2FA enabled" };

                var errMsg = data?["error_message"]?.ToString()
                          ?? obj["errors"]?[0]?["message"]?.ToString()
                          ?? $"HTTP {(int)res.StatusCode}";
                return new TwoFactorActionResult { Success = false, Message = errMsg };
            }
            catch (Exception ex)
            {
                return new TwoFactorActionResult { Success = false, Message = ex.Message };
            }
        }
        // ── Helpers ──────────────────────────────────────────────────
        private HttpClient BuildAcClient(InstagramSession session)
        {
            var client = HttpClientFactory.Create(useCookies: false);
            client.DefaultRequestHeaders.Add("cookie",     session.Cookie);
            client.DefaultRequestHeaders.Add("x-fb-lsd",  session.Lsd);
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0");
            client.DefaultRequestHeaders.Add("origin",     "https://accountscenter.instagram.com");
            client.DefaultRequestHeaders.Add("referer", "https://accountscenter.instagram.com/password_and_security/");
            return client;
        }
        private TwoFactorStatus ParseStatus(string json)
        {
            try
            {
                var root   = JsonConvert.DeserializeObject<JObject>(json);

                // data → fxcal_settings → node → two_factor_data_v2
                var tfData = root?["data"]?["fxcal_settings"]?["node"]?["two_factor_data_v2"];

                if (tfData == null)
                    return new TwoFactorStatus { Error = "Cannot read two_factor_data_v2 from response" };

                var seeds = new List<TotpSeed>();
                foreach (var s in tfData["totp_seeds"] ?? new JArray())
                {
                    var id   = s["id"]?.ToString();
                    var name = s["name"]?.ToString();
                    if (!string.IsNullOrEmpty(id))
                        seeds.Add(new TotpSeed { Id = id, Name = name ?? id });
                }

                return new TwoFactorStatus
                {
                    IsTotpEnabled    = tfData["is_totp_enabled"]?.Value<bool>()              ?? false,
                    IsSmsEnabled     = tfData["is_sms_enabled"]?.Value<bool>()               ?? false,
                    IsFidoEnabled    = tfData["is_fido_enabled"]?.Value<bool>()              ?? false,
                    CanDisable       = tfData["can_disable_two_factor_method"]?.Value<bool>() ?? false,
                    CanAddAdditional = tfData["can_add_additional_totp"]?.Value<bool>()       ?? false,
                    Seeds            = seeds
                };
            }
            catch (Exception ex)
            {
                return new TwoFactorStatus { Error = $"Parse error: {ex.Message}" };
            }
        }
    }
}
