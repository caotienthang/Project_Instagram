using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    partial class ChangePasswordDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel       pnlHeader;
        private System.Windows.Forms.Label       lblTitle;
        private System.Windows.Forms.Panel       pnlContent;
        private System.Windows.Forms.Label       lblUsernameCaption;
        private System.Windows.Forms.Label       lblUsernameValue;
        private System.Windows.Forms.Label       lblCurrentPassword;
        private System.Windows.Forms.TextBox     txtCurrentPassword;
        private System.Windows.Forms.Label       lblNewPassword;
        private System.Windows.Forms.TextBox     txtNewPassword;
        private System.Windows.Forms.Label       lblConfirmPassword;
        private System.Windows.Forms.TextBox     txtConfirmPassword;
        private System.Windows.Forms.Panel       pnlFooter;
        private System.Windows.Forms.Button      btnConfirm;
        private System.Windows.Forms.Button      btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader          = new System.Windows.Forms.Panel();
            this.lblTitle           = new System.Windows.Forms.Label();
            this.pnlContent         = new System.Windows.Forms.Panel();
            this.lblUsernameCaption = new System.Windows.Forms.Label();
            this.lblUsernameValue   = new System.Windows.Forms.Label();
            this.lblCurrentPassword = new System.Windows.Forms.Label();
            this.txtCurrentPassword = new System.Windows.Forms.TextBox();
            this.lblNewPassword     = new System.Windows.Forms.Label();
            this.txtNewPassword     = new System.Windows.Forms.TextBox();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.pnlFooter          = new System.Windows.Forms.Panel();
            this.btnConfirm         = new System.Windows.Forms.Button();
            this.btnCancel          = new System.Windows.Forms.Button();

            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();

            // ── Header ──────────────────────────────────────────────
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(18, 18, 35);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 55;
            this.pnlHeader.Controls.Add(this.lblTitle);

            this.lblTitle.Text      = "🔑  Change Password";
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Location  = new System.Drawing.Point(18, 14);
            this.lblTitle.AutoSize  = true;

            // ── Content ─────────────────────────────────────────────
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(30, 33, 58);
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;

            // Username row
            this.lblUsernameCaption.Text      = "Account:";
            this.lblUsernameCaption.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblUsernameCaption.ForeColor = System.Drawing.Color.FromArgb(150, 160, 200);
            this.lblUsernameCaption.BackColor = System.Drawing.Color.Transparent;
            this.lblUsernameCaption.Location  = new System.Drawing.Point(20, 22);
            this.lblUsernameCaption.Size      = new System.Drawing.Size(150, 22);
            this.lblUsernameCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblUsernameValue.Text      = "";
            this.lblUsernameValue.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblUsernameValue.ForeColor = System.Drawing.Color.FromArgb(52, 210, 120);
            this.lblUsernameValue.BackColor = System.Drawing.Color.Transparent;
            this.lblUsernameValue.Location  = new System.Drawing.Point(178, 22);
            this.lblUsernameValue.Size      = new System.Drawing.Size(210, 22);
            this.lblUsernameValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Current Password row
            this.lblCurrentPassword.Text      = "Current Password:";
            this.lblCurrentPassword.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCurrentPassword.ForeColor = System.Drawing.Color.FromArgb(200, 205, 230);
            this.lblCurrentPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentPassword.Location  = new System.Drawing.Point(20, 68);
            this.lblCurrentPassword.Size      = new System.Drawing.Size(150, 26);
            this.lblCurrentPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtCurrentPassword.BackColor    = System.Drawing.Color.FromArgb(40, 43, 72);
            this.txtCurrentPassword.ForeColor    = System.Drawing.Color.FromArgb(220, 225, 255);
            this.txtCurrentPassword.BorderStyle  = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCurrentPassword.Font         = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCurrentPassword.Location     = new System.Drawing.Point(178, 66);
            this.txtCurrentPassword.Size         = new System.Drawing.Size(210, 28);
            this.txtCurrentPassword.PasswordChar = '●';
            this.txtCurrentPassword.TabIndex     = 0;

            // New Password row
            this.lblNewPassword.Text      = "New Password:";
            this.lblNewPassword.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblNewPassword.ForeColor = System.Drawing.Color.FromArgb(200, 205, 230);
            this.lblNewPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblNewPassword.Location  = new System.Drawing.Point(20, 112);
            this.lblNewPassword.Size      = new System.Drawing.Size(150, 26);
            this.lblNewPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtNewPassword.BackColor    = System.Drawing.Color.FromArgb(40, 43, 72);
            this.txtNewPassword.ForeColor    = System.Drawing.Color.FromArgb(220, 225, 255);
            this.txtNewPassword.BorderStyle  = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewPassword.Font         = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewPassword.Location     = new System.Drawing.Point(178, 110);
            this.txtNewPassword.Size         = new System.Drawing.Size(210, 28);
            this.txtNewPassword.PasswordChar = '●';
            this.txtNewPassword.TabIndex     = 1;

            // Confirm Password row
            this.lblConfirmPassword.Text      = "Confirm Password:";
            this.lblConfirmPassword.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(200, 205, 230);
            this.lblConfirmPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblConfirmPassword.Location  = new System.Drawing.Point(20, 156);
            this.lblConfirmPassword.Size      = new System.Drawing.Size(150, 26);
            this.lblConfirmPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtConfirmPassword.BackColor    = System.Drawing.Color.FromArgb(40, 43, 72);
            this.txtConfirmPassword.ForeColor    = System.Drawing.Color.FromArgb(220, 225, 255);
            this.txtConfirmPassword.BorderStyle  = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConfirmPassword.Font         = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location     = new System.Drawing.Point(178, 154);
            this.txtConfirmPassword.Size         = new System.Drawing.Size(210, 28);
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.TabIndex     = 2;

            this.pnlContent.Controls.Add(this.txtConfirmPassword);
            this.pnlContent.Controls.Add(this.lblConfirmPassword);
            this.pnlContent.Controls.Add(this.txtNewPassword);
            this.pnlContent.Controls.Add(this.lblNewPassword);
            this.pnlContent.Controls.Add(this.txtCurrentPassword);
            this.pnlContent.Controls.Add(this.lblCurrentPassword);
            this.pnlContent.Controls.Add(this.lblUsernameValue);
            this.pnlContent.Controls.Add(this.lblUsernameCaption);

            // ── Footer ──────────────────────────────────────────────
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(22, 24, 45);
            this.pnlFooter.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height    = 60;
            this.pnlFooter.Controls.Add(this.btnConfirm);
            this.pnlFooter.Controls.Add(this.btnCancel);

            this.btnConfirm.Text      = "✔  Confirm";
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(22, 160, 133);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.FlatAppearance.BorderSize         = 0;
            this.btnConfirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(17, 128, 106);
            this.btnConfirm.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.Location  = new System.Drawing.Point(14, 13);
            this.btnConfirm.Size      = new System.Drawing.Size(120, 34);
            this.btnConfirm.TabIndex  = 3;
            this.btnConfirm.Click    += new System.EventHandler(this.btnConfirm_Click);

            this.btnCancel.Text      = "✖  Cancel";
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(52, 58, 90);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize         = 0;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(68, 75, 115);
            this.btnCancel.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Location  = new System.Drawing.Point(146, 13);
            this.btnCancel.Size      = new System.Drawing.Size(110, 34);
            this.btnCancel.TabIndex  = 4;
            this.btnCancel.Click    += new System.EventHandler(this.btnCancel_Click);

            // ── Form ────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(30, 33, 58);
            this.ClientSize          = new System.Drawing.Size(420, 325);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                = "Change Password";

            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
