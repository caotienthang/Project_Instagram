using System;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class ChangePasswordDialog : Form
    {
        private readonly AccountInfo _account;

        public ChangePasswordDialog(AccountInfo account)
        {
            InitializeComponent();
            _account = account;
            lblUsernameValue.Text = "@" + account.Username;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string currentPwd = txtCurrentPassword.Text;
            string newPwd     = txtNewPassword.Text;
            string confirmPwd = txtConfirmPassword.Text;

            // If the account has a stored password, verify it
            if (!string.IsNullOrEmpty(_account.Password) && currentPwd != _account.Password)
            {
                MessageBox.Show("Current password is incorrect.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(newPwd))
            {
                MessageBox.Show("New password cannot be empty.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNewPassword.Focus();
                return;
            }

            if (newPwd != confirmPwd)
            {
                MessageBox.Show("New password and confirm password do not match.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            AccountRepository.UpdatePassword(_account.Id, newPwd);
            _account.Password = newPwd;

            MessageBox.Show($"Password for @{_account.Username} changed successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
