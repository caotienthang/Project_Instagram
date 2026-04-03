using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Managers
{
    /// <summary>
    /// Settings Manager - Quản lý lưu trữ và đọc cấu hình từ JSON file
    /// </summary>
    public static class SettingsManager
    {
        private static readonly string SettingsFilePath = Path.Combine(Application.StartupPath, "appsettings.json");
        private static AppSettings _cachedSettings;
        private static readonly object _lock = new object();

        /// <summary>
        /// Load settings from JSON file
        /// </summary>
        public static AppSettings LoadSettings()
        {
            lock (_lock)
            {
                // Return cached settings if available
                if (_cachedSettings != null)
                {
                    return _cachedSettings;
                }

                try
                {
                    if (File.Exists(SettingsFilePath))
                    {
                        var json = File.ReadAllText(SettingsFilePath);
                        _cachedSettings = JsonConvert.DeserializeObject<AppSettings>(json);

                        // Validate loaded settings
                        if (_cachedSettings == null || !ValidateSettings(_cachedSettings))
                        {
                            _cachedSettings = CreateDefaultSettings();
                            SaveSettings(_cachedSettings);
                        }
                    }
                    else
                    {
                        // Create default settings if file doesn't exist
                        _cachedSettings = CreateDefaultSettings();
                        SaveSettings(_cachedSettings);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Lỗi khi đọc file cấu hình:\n{ex.Message}\n\nSử dụng cấu hình mặc định.",
                        "Lỗi Settings",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    _cachedSettings = CreateDefaultSettings();
                }

                return _cachedSettings;
            }
        }

        /// <summary>
        /// Save settings to JSON file
        /// </summary>
        public static bool SaveSettings(AppSettings settings)
        {
            lock (_lock)
            {
                try
                {
                    // Validate before saving
                    if (!ValidateSettings(settings))
                    {
                        MessageBox.Show(
                            "Cấu hình không hợp lệ. Vui lòng kiểm tra lại.",
                            "Lỗi Validation",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return false;
                    }

                    // Serialize to JSON with formatting
                    var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                    File.WriteAllText(SettingsFilePath, json);

                    // Update cached settings
                    _cachedSettings = settings;

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Lỗi khi lưu file cấu hình:\n{ex.Message}",
                        "Lỗi Settings",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }
            }
        }

        /// <summary>
        /// Reload settings from file (clears cache)
        /// </summary>
        public static AppSettings ReloadSettings()
        {
            lock (_lock)
            {
                _cachedSettings = null;
                return LoadSettings();
            }
        }

        /// <summary>
        /// Reset to default settings
        /// </summary>
        public static AppSettings ResetToDefault()
        {
            lock (_lock)
            {
                _cachedSettings = CreateDefaultSettings();
                SaveSettings(_cachedSettings);
                return _cachedSettings;
            }
        }

        /// <summary>
        /// Create default settings
        /// </summary>
        private static AppSettings CreateDefaultSettings()
        {
            return new AppSettings
            {
                InstagramApi = new InstagramApiSettings
                {
                    EncryptionKeyId = 228,
                    PublicKeyBase64 = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqb8PlCRf63Tv6/Lo+rZEwZ6uu7qKriAQhlqeSjUnzKqWXFj9iP2MWsMBpZCTSHZRZ8UI2wKZj1zPi/QXTi+NGHUWFogJwJY+1Bk1w21vpQgCvH0nyafAUDuTPTqU4SwVQYBpKruYFA3qQHmvvx5Ym9uMwxutkf2x9llfVOyJG1RamrGxkAb7jLwcy+PRoLHp554aa1Pp3XFkeFDzKyNDoN/VAZFmTR7HRl5H+MgCYCFKoyvqnJHhUaCkqAlHLtKrBzU8jBmsA5z/+7xhxz+9icnz+VYAfwFDHAAZf4aH+aDA6JWG+fohC7nCPJCvjtIl2tErRrAnbVwKDdbT45qlZwIDAQAB",
                    UserAgent = "Instagram 423.0.0.47.66 Android (28/9; 480dpi; 1080x1920; Redmi; 22127RK46C; marlin; qcom; en_US; 923309183)",
                    BloksVersioningId = "899adff463607d5f13a547f7417a9de4a8b4add115ddebc553c1bc5b8d48a28a",
                    RequestTimeoutSeconds = 30
                },
                Application = new ApplicationSettings
                {
                    DownloadAvatars = true,
                    AvatarCacheFolder = "Avatars",
                    MaxLoginRetries = 3,
                    RetryDelayMs = 2000,
                    EnableDebugLogging = false
                },
                Proxy = new ProxySettings
                {
                    Enabled = false,
                    Host = "",
                    Port = 8080,
                    Username = "",
                    Password = "",
                    ProxyType = "HTTP"
                }
            };
        }

        /// <summary>
        /// Validate settings
        /// </summary>
        private static bool ValidateSettings(AppSettings settings)
        {
            if (settings == null)
                return false;

            // Validate Instagram API settings
            if (settings.InstagramApi == null)
                return false;

            if (settings.InstagramApi.EncryptionKeyId < 0)
                return false;

            if (string.IsNullOrWhiteSpace(settings.InstagramApi.PublicKeyBase64))
                return false;

            if (string.IsNullOrWhiteSpace(settings.InstagramApi.UserAgent))
                return false;

            if (string.IsNullOrWhiteSpace(settings.InstagramApi.BloksVersioningId))
                return false;

            if (settings.InstagramApi.RequestTimeoutSeconds <= 0)
                return false;

            // Validate Application settings
            if (settings.Application == null)
                return false;

            if (settings.Application.MaxLoginRetries < 0)
                return false;

            if (settings.Application.RetryDelayMs < 0)
                return false;

            // Validate Proxy settings
            if (settings.Proxy == null)
                return false;

            if (settings.Proxy.Enabled && string.IsNullOrWhiteSpace(settings.Proxy.Host))
                return false;

            if (settings.Proxy.Port < 0 || settings.Proxy.Port > 65535)
                return false;

            return true;
        }

        /// <summary>
        /// Export settings to file
        /// </summary>
        public static bool ExportSettings(string filePath)
        {
            try
            {
                var settings = LoadSettings();
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xuất cấu hình:\n{ex.Message}",
                    "Lỗi Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
        }

        /// <summary>
        /// Import settings from file
        /// </summary>
        public static bool ImportSettings(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show(
                        "File không tồn tại.",
                        "Lỗi Import",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }

                var json = File.ReadAllText(filePath);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);

                if (!ValidateSettings(settings))
                {
                    MessageBox.Show(
                        "File cấu hình không hợp lệ.",
                        "Lỗi Import",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }

                return SaveSettings(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi nhập cấu hình:\n{ex.Message}",
                    "Lỗi Import",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
        }

        /// <summary>
        /// Get settings file path
        /// </summary>
        public static string GetSettingsFilePath()
        {
            return SettingsFilePath;
        }
    }
}
