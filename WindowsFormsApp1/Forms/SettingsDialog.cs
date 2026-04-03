using System;
using System.Windows.Forms;
using WindowsFormsApp1.Managers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class SettingsDialog : Form
    {
        private AppSettings _settings;

        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            // Load current settings
            _settings = SettingsManager.LoadSettings();
            LoadSettingsToUI();
        }

        private void LoadSettingsToUI()
        {
            // Instagram API Settings
            numEncryptionKeyId.Value = _settings.InstagramApi.EncryptionKeyId;
            txtPublicKey.Text = _settings.InstagramApi.PublicKeyBase64;
            txtUserAgent.Text = _settings.InstagramApi.UserAgent;
            txtBloksVersioningId.Text = _settings.InstagramApi.BloksVersioningId;
            numRequestTimeout.Value = _settings.InstagramApi.RequestTimeoutSeconds;

            // Application Settings
            chkDownloadAvatars.Checked = _settings.Application.DownloadAvatars;
            txtAvatarFolder.Text = _settings.Application.AvatarCacheFolder;
            numMaxRetries.Value = _settings.Application.MaxLoginRetries;
            numRetryDelay.Value = _settings.Application.RetryDelayMs;
            chkEnableDebugLogging.Checked = _settings.Application.EnableDebugLogging;

            // Proxy Settings
            chkEnableProxy.Checked = _settings.Proxy.Enabled;
            txtProxyHost.Text = _settings.Proxy.Host;
            numProxyPort.Value = _settings.Proxy.Port;
            txtProxyUsername.Text = _settings.Proxy.Username;
            txtProxyPassword.Text = _settings.Proxy.Password;
            cmbProxyType.SelectedIndex = cmbProxyType.Items.IndexOf(_settings.Proxy.ProxyType);
            if (cmbProxyType.SelectedIndex < 0)
                cmbProxyType.SelectedIndex = 0; // Default to HTTP
        }

        private void SaveUIToSettings()
        {
            // Instagram API Settings
            _settings.InstagramApi.EncryptionKeyId = (int)numEncryptionKeyId.Value;
            _settings.InstagramApi.PublicKeyBase64 = txtPublicKey.Text.Trim();
            _settings.InstagramApi.UserAgent = txtUserAgent.Text.Trim();
            _settings.InstagramApi.BloksVersioningId = txtBloksVersioningId.Text.Trim();
            _settings.InstagramApi.RequestTimeoutSeconds = (int)numRequestTimeout.Value;

            // Application Settings
            _settings.Application.DownloadAvatars = chkDownloadAvatars.Checked;
            _settings.Application.AvatarCacheFolder = txtAvatarFolder.Text.Trim();
            _settings.Application.MaxLoginRetries = (int)numMaxRetries.Value;
            _settings.Application.RetryDelayMs = (int)numRetryDelay.Value;
            _settings.Application.EnableDebugLogging = chkEnableDebugLogging.Checked;

            // Proxy Settings
            _settings.Proxy.Enabled = chkEnableProxy.Checked;
            _settings.Proxy.Host = txtProxyHost.Text.Trim();
            _settings.Proxy.Port = (int)numProxyPort.Value;
            _settings.Proxy.Username = txtProxyUsername.Text.Trim();
            _settings.Proxy.Password = txtProxyPassword.Text;
            _settings.Proxy.ProxyType = cmbProxyType.SelectedItem?.ToString() ?? "HTTP";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtPublicKey.Text))
                {
                    MessageBox.Show(
                        "Public Key không được để trống!",
                        "Lỗi Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    tabControl.SelectedTab = tabInstagramApi;
                    txtPublicKey.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUserAgent.Text))
                {
                    MessageBox.Show(
                        "User-Agent không được để trống!",
                        "Lỗi Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    tabControl.SelectedTab = tabInstagramApi;
                    txtUserAgent.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBloksVersioningId.Text))
                {
                    MessageBox.Show(
                        "Bloks Versioning ID không được để trống!",
                        "Lỗi Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    tabControl.SelectedTab = tabInstagramApi;
                    txtBloksVersioningId.Focus();
                    return;
                }

                // Save UI to settings object
                SaveUIToSettings();

                // Save to file
                if (SettingsManager.SaveSettings(_settings))
                {
                    MessageBox.Show(
                        "✅ Đã lưu cài đặt thành công!\n\nCác thay đổi sẽ có hiệu lực ngay lập tức.",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi lưu cài đặt:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ Bạn có chắc muốn khôi phục cài đặt mặc định?\n\nTất cả cài đặt hiện tại sẽ bị xóa!",
                "Xác nhận Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                _settings = SettingsManager.ResetToDefault();
                LoadSettingsToUI();

                MessageBox.Show(
                    "✅ Đã khôi phục cài đặt mặc định!",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                dialog.DefaultExt = "json";
                dialog.FileName = $"appsettings_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                dialog.Title = "Export Settings";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Save current UI to settings first
                    SaveUIToSettings();

                    if (SettingsManager.ExportSettings(dialog.FileName))
                    {
                        MessageBox.Show(
                            $"✅ Đã export cài đặt thành công!\n\nFile: {dialog.FileName}",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                dialog.Title = "Import Settings";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (SettingsManager.ImportSettings(dialog.FileName))
                    {
                        _settings = SettingsManager.LoadSettings();
                        LoadSettingsToUI();

                        MessageBox.Show(
                            "✅ Đã import cài đặt thành công!",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
        }
    }
}
