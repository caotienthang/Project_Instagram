using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    partial class PhoneLoginDialog
    {
        private System.ComponentModel.IContainer components = null;

        private Panel   _headerPanel;
        private Label   _lblTitle;
        private Label   _lblSubtitle;
        private Label   _lblUsername;
        private TextBox txtUsername;
        private Label   _lblPassword;
        private TextBox txtPassword;
        private Label   lblStatus;
        private Button  btnConfirm;
        private Button  btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _headerPanel = new Panel();
            _lblTitle    = new Label();
            _lblSubtitle = new Label();
            _lblUsername = new Label();
            txtUsername  = new TextBox();
            _lblPassword = new Label();
            txtPassword  = new TextBox();
            lblStatus    = new Label();
            btnConfirm   = new Button();
            btnCancel    = new Button();

            _headerPanel.SuspendLayout();
            this.SuspendLayout();

            // ═══════════════════════════════════════════════════════════
            // HEADER  (62px · dark navy)
            // ═══════════════════════════════════════════════════════════
            _headerPanel.BackColor = Color.FromArgb(18, 18, 35);
            _headerPanel.Dock      = DockStyle.Top;
            _headerPanel.Height    = 62;
            _headerPanel.Controls.Add(_lblSubtitle);
            _headerPanel.Controls.Add(_lblTitle);

            _lblTitle.Text      = "📱  Đăng nhập thủ công";
            _lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.BackColor = Color.Transparent;
            _lblTitle.Location  = new Point(20, 8);
            _lblTitle.AutoSize  = true;

            _lblSubtitle.Text      = "Nhập thông tin tài khoản Instagram";
            _lblSubtitle.Font      = new Font("Segoe UI", 8.5F);
            _lblSubtitle.ForeColor = Color.FromArgb(105, 115, 155);
            _lblSubtitle.BackColor = Color.Transparent;
            _lblSubtitle.Location  = new Point(22, 40);
            _lblSubtitle.AutoSize  = true;

            // ═══════════════════════════════════════════════════════════
            // USERNAME / EMAIL
            // ═══════════════════════════════════════════════════════════
            _lblUsername.Text      = "Tên đăng nhập / Email:";
            _lblUsername.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblUsername.ForeColor = Color.FromArgb(45, 50, 70);
            _lblUsername.Location  = new Point(24, 76);
            _lblUsername.AutoSize  = true;

            txtUsername.Font        = new Font("Segoe UI", 10F);
            txtUsername.Location    = new Point(24, 96);
            txtUsername.Size        = new Size(372, 28);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.BackColor   = Color.FromArgb(240, 242, 250);
            txtUsername.ForeColor   = Color.FromArgb(30, 35, 60);
            txtUsername.MaxLength   = 100;

            // ═══════════════════════════════════════════════════════════
            // PASSWORD
            // ═══════════════════════════════════════════════════════════
            _lblPassword.Text      = "Mật khẩu:";
            _lblPassword.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblPassword.ForeColor = Color.FromArgb(45, 50, 70);
            _lblPassword.Location  = new Point(24, 140);
            _lblPassword.AutoSize  = true;

            txtPassword.Font         = new Font("Segoe UI", 10F);
            txtPassword.Location     = new Point(24, 160);
            txtPassword.Size         = new Size(372, 28);
            txtPassword.BorderStyle  = BorderStyle.FixedSingle;
            txtPassword.BackColor    = Color.FromArgb(240, 242, 250);
            txtPassword.ForeColor    = Color.FromArgb(30, 35, 60);
            txtPassword.PasswordChar = '●';
            txtPassword.MaxLength    = 128;

            // ═══════════════════════════════════════════════════════════
            // STATUS LABEL
            // ═══════════════════════════════════════════════════════════
            lblStatus.Text      = "";
            lblStatus.Font      = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
            lblStatus.Location  = new Point(24, 202);
            lblStatus.Size      = new Size(372, 20);
            lblStatus.AutoSize  = false;

            // ═══════════════════════════════════════════════════════════
            // BUTTON — CONFIRM
            // ═══════════════════════════════════════════════════════════
            btnConfirm.Text      = "✅  Thêm tài khoản";
            btnConfirm.BackColor = Color.FromArgb(39, 174, 96);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.FlatAppearance.BorderSize         = 0;
            btnConfirm.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 148, 78);
            btnConfirm.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnConfirm.Cursor    = Cursors.Hand;
            btnConfirm.Location  = new Point(24, 234);
            btnConfirm.Size      = new Size(175, 36);
            btnConfirm.Click    += new System.EventHandler(this.btnConfirm_Click);

            // ═══════════════════════════════════════════════════════════
            // BUTTON — CANCEL
            // ═══════════════════════════════════════════════════════════
            btnCancel.Text      = "✕   Hủy";
            btnCancel.BackColor = Color.FromArgb(85, 88, 120);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize         = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(105, 108, 142);
            btnCancel.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.Cursor    = Cursors.Hand;
            btnCancel.Location  = new Point(212, 234);
            btnCancel.Size      = new Size(120, 36);
            btnCancel.Click    += new System.EventHandler(this.btnCancel_Click);

            // ═══════════════════════════════════════════════════════════
            // FORM
            // ═══════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = AutoScaleMode.Font;
            this.BackColor           = Color.FromArgb(235, 238, 248);
            this.ClientSize          = new System.Drawing.Size(420, 290);
            this.Font                = new Font("Segoe UI", 9F);
            this.FormBorderStyle     = FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = FormStartPosition.CenterParent;
            this.Text                = "Đăng nhập thủ công";
            this.AcceptButton        = btnConfirm;
            this.CancelButton        = btnCancel;

            this.Controls.Add(btnCancel);
            this.Controls.Add(btnConfirm);
            this.Controls.Add(lblStatus);
            this.Controls.Add(txtPassword);
            this.Controls.Add(_lblPassword);
            this.Controls.Add(txtUsername);
            this.Controls.Add(_lblUsername);
            this.Controls.Add(_headerPanel);

            _headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
