using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class ChangePasswordDialog : Form
    {
        private readonly AccountInfo _account;
        private readonly ChangePasswordService _changePasswordService;

        public ChangePasswordDialog(AccountInfo account)
        {
            InitializeComponent();
            _account = account;
            _changePasswordService = new ChangePasswordService();
            lblUsernameValue.Text = "@" + account.Username;
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            string currentPwd = txtCurrentPassword.Text;
            string newPwd     = txtNewPassword.Text;
            string confirmPwd = txtConfirmPassword.Text;

            // Basic validation
            if (string.IsNullOrWhiteSpace(currentPwd))
            {
                MessageBox.Show("Current password cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(newPwd))
            {
                MessageBox.Show("New password cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNewPassword.Focus();
                return;
            }

            if (newPwd != confirmPwd)
            {
                MessageBox.Show("New password and confirm password do not match.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            // Validate session & account info
            var session = InstagramSessionRepository.GetByAccountId(_account.Id);
            if (session == null || string.IsNullOrWhiteSpace(session.AuthorizationPhone))
            {
                MessageBox.Show(
                    "No session found for this account.\nPlease use \"Get Cookie\" first to obtain authorization.",
                    "Session Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_account.FbAccountId))
            {
                MessageBox.Show(
                    "Facebook Account ID not found.\nPlease use \"Get Cookie\" first.",
                    "Account ID Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Disable UI during API call
            btnConfirm.Enabled = false;
            btnCancel.Enabled = false;
            txtCurrentPassword.Enabled = false;
            txtNewPassword.Enabled = false;
            txtConfirmPassword.Enabled = false;
            btnConfirm.Text = "⏳ Changing...";

            try
            {
                // Call API
                var (success, message) = await _changePasswordService.ChangePasswordAsync(
                    session,
                    _account.FbAccountId,
                    _account.Username,
                    _account.LinkAvatar,
                    currentPwd,
                    newPwd
                );

                if (success)
                {
                    // Update password in database
                    AccountRepository.UpdatePassword(_account.Id, newPwd);
                    _account.Password = newPwd;

                    MessageBox.Show(
                        message ?? $"Password for @{_account.Username} changed successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        message ?? "Failed to change password. Please try again.",
                        "Change Password Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    // Re-enable UI
                    btnConfirm.Enabled = true;
                    btnCancel.Enabled = true;
                    txtCurrentPassword.Enabled = true;
                    txtNewPassword.Enabled = true;
                    txtConfirmPassword.Enabled = true;
                    btnConfirm.Text = "✓ Confirm";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error changing password:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                // Re-enable UI
                btnConfirm.Enabled = true;
                btnCancel.Enabled = true;
                txtCurrentPassword.Enabled = true;
                txtNewPassword.Enabled = true;
                txtConfirmPassword.Enabled = true;
                btnConfirm.Text = "✓ Confirm";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
