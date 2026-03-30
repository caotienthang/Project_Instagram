using System;
using System.Net.Http;

namespace WindowsFormsApp1.Services
{
    public static class HttpClientFactory
    {
        private static string _proxyUrl;

        // Set proxy globally for all services
        public static void SetProxy(string proxyUrl)
        {
            _proxyUrl = proxyUrl;
        }

        // Create HttpClient configured with optional proxy and cookie handling
        public static HttpClient Create(bool useCookies = true)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = useCookies,
                AllowAutoRedirect = false
            };

            if (!string.IsNullOrEmpty(_proxyUrl))
            {
                try
                {
                    var proxyUri = new Uri(_proxyUrl);
                    var webProxy = new System.Net.WebProxy(proxyUri)
                    {
                        Credentials = string.IsNullOrEmpty(proxyUri.UserInfo) ? null : new System.Net.NetworkCredential(
                            Uri.UnescapeDataString(proxyUri.UserInfo.Split(':')[0]),
                            Uri.UnescapeDataString(proxyUri.UserInfo.Split(':').Length > 1 ? proxyUri.UserInfo.Split(':')[1] : ""))
                    };
                    handler.UseProxy = true;
                    handler.Proxy = webProxy;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Invalid proxy URL '{_proxyUrl}': {ex.Message}");
                }
            }

            return new HttpClient(handler);
        }
    }
}
