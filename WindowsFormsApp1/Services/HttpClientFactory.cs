using System;
using System.Net;
using System.Net.Http;
using WindowsFormsApp1.Managers;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Factory to create HttpClient instances with global proxy support from Settings
    /// Proxy settings được apply tự động cho TẤT CẢ HTTP requests trong app
    /// </summary>
    public static class HttpClientFactory
    {
        /// <summary>
        /// Create HttpClient configured with proxy from settings (if enabled)
        /// </summary>
        /// <param name="useCookies">Enable cookie handling</param>
        /// <returns>Configured HttpClient instance</returns>
        public static HttpClient Create(bool useCookies = true)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = useCookies,
                AllowAutoRedirect = false
            };

            // 🌐 PROXY: Load proxy settings and apply if enabled
            try
            {
                var settings = SettingsManager.LoadSettings();
                var proxySettings = settings.Proxy;

                if (proxySettings.Enabled && !string.IsNullOrWhiteSpace(proxySettings.Host))
                {
                    // Build proxy URL based on proxy type
                    var proxyScheme = proxySettings.ProxyType.ToLower();
                    if (!proxyScheme.Contains("://"))
                    {
                        // Convert proxy type to scheme
                        if (proxyScheme.StartsWith("socks"))
                            proxyScheme = "socks"; // HttpClient uses "socks" for both SOCKS4/5
                        else
                            proxyScheme = "http"; // Default to http for HTTP/HTTPS
                    }

                    var proxyUrl = $"{proxyScheme}://{proxySettings.Host}:{proxySettings.Port}";
                    var proxyUri = new Uri(proxyUrl);

                    var webProxy = new WebProxy(proxyUri)
                    {
                        BypassProxyOnLocal = false
                    };

                    // Add authentication if provided
                    if (!string.IsNullOrWhiteSpace(proxySettings.Username))
                    {
                        webProxy.Credentials = new NetworkCredential(
                            proxySettings.Username,
                            proxySettings.Password ?? string.Empty
                        );
                    }

                    handler.UseProxy = true;
                    handler.Proxy = webProxy;

                    // Debug logging
                    if (settings.Application.EnableDebugLogging)
                    {
                        Console.WriteLine($"[HttpClientFactory] ✅ Proxy enabled: {proxyUrl}");
                        Console.WriteLine($"[HttpClientFactory] 📡 Proxy type: {proxySettings.ProxyType}");
                        Console.WriteLine($"[HttpClientFactory] 🔐 Auth: {(!string.IsNullOrWhiteSpace(proxySettings.Username) ? "Yes" : "No")}");
                    }
                }
                else
                {
                    handler.UseProxy = false;

                    if (settings.Application.EnableDebugLogging)
                    {
                        Console.WriteLine("[HttpClientFactory] ⚠️ Proxy disabled");
                    }
                }

                // Set timeout from settings
                var timeout = TimeSpan.FromSeconds(settings.InstagramApi.RequestTimeoutSeconds);
                return new HttpClient(handler) { Timeout = timeout };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HttpClientFactory] ❌ Error configuring proxy: {ex.Message}");
                // Continue without proxy if error occurs
                handler.UseProxy = false;
                return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
            }
        }
    }
}
