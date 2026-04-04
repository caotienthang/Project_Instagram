using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Views
{
    public partial class PhoneLoginDialog : Form
    {
        public AccountInfo SavedAccount { get; private set; }

        private readonly bool _isUpdateMode;
        private readonly AccountInfo _existingAccount;

        /// <summary>
        /// Constructor cho Add Account mode (tạo mới)
        /// </summary>
        public PhoneLoginDialog()
        {
            InitializeComponent();
            _isUpdateMode = false;
        }

        /// <summary>
        /// Constructor cho Get Cookie/Update Session mode (account đã tồn tại)
        /// </summary>
        /// <param name="existingAccount">Account cần update session</param>
        public PhoneLoginDialog(AccountInfo existingAccount)
        {
            InitializeComponent();
            _isUpdateMode = true;
            _existingAccount = existingAccount;

            // Pre-fill thông tin
            if (existingAccount != null)
            {
                txtUsername.Text = existingAccount.Username;

                // Pre-fill password nếu có
                if (!string.IsNullOrWhiteSpace(existingAccount.Password))
                {
                    txtPassword.Text = existingAccount.Password;
                }

                // Change button text
                btnConfirm.Text = "🔄 Update Session";
                this.Text = $"Get Cookie - @{existingAccount.Username}";
            }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                SetStatus("⚠️ Vui lòng nhập Username hoặc Email.", Color.FromArgb(241, 196, 15));
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                SetStatus("⚠️ Vui lòng nhập mật khẩu.", Color.FromArgb(241, 196, 15));
                txtPassword.Focus();
                return;
            }

            // Check account existence ONLY in Add mode
            if (!_isUpdateMode)
            {
                var existing = AccountRepository.GetByUsername(username);
                if (existing != null)
                {
                    SetStatus("⚠️ Tài khoản đã tồn tại trong database.", Color.FromArgb(231, 76, 60));
                    return;
                }
            }

            // Disable controls during login
            btnConfirm.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            SetStatus("🔄 Đang đăng nhập vào Instagram...", Color.FromArgb(52, 152, 219));

            try
            {
                // Call Instagram phone login API
                var loginService = new InstagramPhoneLoginService();
                var result = await loginService.Login(username, password);

                // Check if 2FA is required
                if (result.Requires2FA)
                {
                    SetStatus(result.Message, Color.FromArgb(241, 196, 15));

                    // Prompt for 2FA code using custom dialog
                    string code = null;
                    using (var inputForm = new Form())
                    {
                        inputForm.Text = "Xác thực 2FA";
                        inputForm.Width = 350;
                        inputForm.Height = 150;
                        inputForm.StartPosition = FormStartPosition.CenterParent;
                        inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                        inputForm.MaximizeBox = false;
                        inputForm.MinimizeBox = false;

                        var label = new Label { Left = 20, Top = 20, Width = 300, Text = "Vui lòng nhập mã xác thực 2FA (6 số):" };
                        var textBox = new TextBox { Left = 20, Top = 45, Width = 300, MaxLength = 6 };
                        var buttonOk = new Button { Text = "OK", Left = 150, Width = 75, Top = 75, DialogResult = DialogResult.OK };
                        var buttonCancel = new Button { Text = "Hủy", Left = 230, Width = 75, Top = 75, DialogResult = DialogResult.Cancel };

                        inputForm.Controls.Add(label);
                        inputForm.Controls.Add(textBox);
                        inputForm.Controls.Add(buttonOk);
                        inputForm.Controls.Add(buttonCancel);
                        inputForm.AcceptButton = buttonOk;
                        inputForm.CancelButton = buttonCancel;

                        if (inputForm.ShowDialog(this) == DialogResult.OK)
                        {
                            code = textBox.Text;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(code))
                    {
                        SetStatus("❌ Bạn đã hủy xác thực 2FA.", Color.FromArgb(231, 76, 60));
                        btnConfirm.Enabled = true;
                        txtUsername.Enabled = true;
                        txtPassword.Enabled = true;
                        return;
                    }

                    // Verify 2FA code
                    SetStatus("🔄 Đang xác thực mã 2FA...", Color.FromArgb(52, 152, 219));
                    result = await loginService.Verify2FA(code, result.TwoFactorContext, result.DeviceId, result.CsrfToken);
                }

                if (!result.Success)
                {
                    SetStatus(result.Message, Color.FromArgb(231, 76, 60));
                    btnConfirm.Enabled = true;
                    txtUsername.Enabled = true;
                    txtPassword.Enabled = true;
                    return;
                }

                // Login successful, save account to database with user info from login response
                var acc = new AccountInfo
                {
                    FbAccountId = result.FbAccountId,
                    PhoneAccountId = result.PhoneAccountId,
                    Username = result.Username ?? username,
                    FullName = result.FullName,
                    LocalPathAvatar = result.Avatar,
                    LinkAvatar = result.LinkAvatar,
                    Phone = result.Phone,
                    Status = "Active"
                };

                // Save account and phone session using UpsertPhone (it will auto create/update account)
                InstagramSessionRepository.UpsertPhone(
                    result.FbAccountId,
                    result.PhoneAccountId,
                    result.Username ?? username,
                    result.FullName,
                    result.Avatar,
                    result.LinkAvatar,
                    result.Phone,
                    result.SessionId,
                    result.DsUserId,
                    result.CsrfToken,
                    result.Authorization
                );

                // Get the saved account
                if (_isUpdateMode && _existingAccount != null)
                {
                    // Update mode: use existing account
                    SavedAccount = _existingAccount;

                    // Update password if provided
                    AccountRepository.UpdatePassword(_existingAccount.Id, password);

                    SetStatus("✅ Session updated thành công!", Color.FromArgb(46, 204, 113));
                }
                else
                {
                    // Add mode: get newly created account
                    SavedAccount = AccountRepository.GetByAccountIds(result.FbAccountId, result.PhoneAccountId);

                    if (SavedAccount != null)
                    {
                        // Save password
                        AccountRepository.UpdatePassword(SavedAccount.Id, password);
                    }

                    SetStatus("✅ Đăng nhập thành công!", Color.FromArgb(46, 204, 113));
                }

                // Wait a bit to show success message
                await Task.Delay(1000);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Lỗi: {ex.Message}", Color.FromArgb(231, 76, 60));
                btnConfirm.Enabled = true;
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SetStatus(string msg, Color color)
        {
            lblStatus.Text      = msg;
            lblStatus.ForeColor = color;
        }
    }
}
