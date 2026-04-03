using System;

namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Application settings model
    /// </summary>
    public class AppSettings
    {
        // Instagram API Settings
        public InstagramApiSettings InstagramApi { get; set; } = new InstagramApiSettings();

        // Application Settings
        public ApplicationSettings Application { get; set; } = new ApplicationSettings();

        // Proxy Settings (for future use)
        public ProxySettings Proxy { get; set; } = new ProxySettings();
    }

    public class InstagramApiSettings
    {
        /// <summary>
        /// Instagram encryption key ID (default: 228)
        /// </summary>
        public int EncryptionKeyId { get; set; } = 228;

        /// <summary>
        /// Instagram RSA public key (Base64 encoded PEM format)
        /// </summary>
        public string PublicKeyBase64 { get; set; } = "LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0KTUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FROEFNSUlCQ2dLQ0FRRUFxYjhQbENSZjYzVHY2L0xvK3JaRQp3WjZ1dTdxS3JpQVFobHFlU2pVbnpLcVdYRmo5aVAyTVdzTUJwWkNUU0haUlo4VUkyd0taajF6UGkvUVhUaStOCkdIVVdGb2dKd0pZKzFCazF3MjF2cFFnQ3ZIMG55YWZBVUR1VFBUcVU0U3dWUVlCcEtydVlGQTNxUUhtdnZ4NVkKbTl1TXd4dXRrZjJ4OWxsZlZPeUpHMVJhbXJHeGtBYjdqTHdjeStQUm9MSHA1NTRhYTFQcDNYRmtlRkR6S3lORApvTi9WQVpGbVRSN0hSbDVIK01nQ1lDRktveXZxbkpIaFVhQ2txQWxITHRLckJ6VThqQm1zQTV6Lys3eGh4eis5CmljbnorVllBZndGREhBQVpmNGFIK2FEQTZKV0crZm9oQzduQ1BKQ3ZqdElsMnRFclJyQW5iVndLRGRiVDQ1cWwKWndJREFRQUIKLS0tLS1FTkQgUFVCTElDIEtFWS0tLS0tCg==";

        /// <summary>
        /// User-Agent for Instagram API requests
        /// </summary>
        public string UserAgent { get; set; } = "Instagram 423.0.0.47.66 Android (28/9; 480dpi; 1080x1920; Redmi; 22127RK46C; marlin; qcom; en_US; 923309183)";

        /// <summary>
        /// Bloks versioning ID
        /// </summary>
        public string BloksVersioningId { get; set; } = "899adff463607d5f13a547f7417a9de4a8b4add115ddebc553c1bc5b8d48a28a";

        /// <summary>
        /// Request timeout in seconds
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = 30;
    }

    public class ApplicationSettings
    {
        /// <summary>
        /// Download avatar images
        /// </summary>
        public bool DownloadAvatars { get; set; } = true;

        /// <summary>
        /// Avatar cache folder
        /// </summary>
        public string AvatarCacheFolder { get; set; } = "Avatars";

        /// <summary>
        /// Maximum login retry attempts
        /// </summary>
        public int MaxLoginRetries { get; set; } = 3;

        /// <summary>
        /// Delay between retries (milliseconds)
        /// </summary>
        public int RetryDelayMs { get; set; } = 2000;

        /// <summary>
        /// Enable debug logging
        /// </summary>
        public bool EnableDebugLogging { get; set; } = false;
    }

    public class ProxySettings
    {
        /// <summary>
        /// Enable proxy
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Proxy server address
        /// </summary>
        public string Host { get; set; } = "";

        /// <summary>
        /// Proxy server port
        /// </summary>
        public int Port { get; set; } = 8080;

        /// <summary>
        /// Proxy username (if authentication required)
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// Proxy password (if authentication required)
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Proxy type (HTTP, SOCKS5, etc.)
        /// </summary>
        public string ProxyType { get; set; } = "HTTP";
    }
}
