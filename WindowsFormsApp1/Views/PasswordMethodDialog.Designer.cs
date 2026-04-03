using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    partial class PasswordMethodDialog
    {
        private System.ComponentModel.IContainer components = null;

        private Panel  _headerPanel;
        private Label  _lblTitle;
        private Label  _lblSubtitle;
        private Label  _lblMessage;
        private Button btnComputer;
        private Button btnPhone;
        private Button btnCancel;

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
            _lblMessage  = new Label();
            btnComputer  = new Button();
            btnPhone     = new Button();
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

            _lblTitle.Text      = "🔑  Đổi Mật Khẩu";
            _lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.BackColor = Color.Transparent;
            _lblTitle.Location  = new Point(20, 8);
            _lblTitle.AutoSize  = true;

            _lblSubtitle.Text      = "Chọn phương thức đổi mật khẩu";
            _lblSubtitle.Font      = new Font("Segoe UI", 8.5F);
            _lblSubtitle.ForeColor = Color.FromArgb(105, 115, 155);
            _lblSubtitle.BackColor = Color.Transparent;
            _lblSubtitle.Location  = new Point(22, 40);
            _lblSubtitle.AutoSize  = true;

            // ═══════════════════════════════════════════════════════════
            // DESCRIPTION
            // ═══════════════════════════════════════════════════════════
            _lblMessage.Text      = "Máy tính: mở trình duyệt với cookie tài khoản, đổi trực tiếp trên web.\r\nĐiện thoại: cập nhật mật khẩu thủ công vào database nội bộ.";
            _lblMessage.Font      = new Font("Segoe UI", 9F);
            _lblMessage.ForeColor = Color.FromArgb(80, 90, 120);
            _lblMessage.Location  = new Point(20, 72);
            _lblMessage.Size      = new Size(410, 36);
            _lblMessage.AutoSize  = false;

            // ═══════════════════════════════════════════════════════════
            // BUTTON — MÁY TÍNH  (left card)
            // ═══════════════════════════════════════════════════════════
            btnComputer.Text      = "🖥️  MÁY TÍNH\r\nMở WebView2\r\n(Đổi trực tiếp trên web)";
            btnComputer.BackColor = Color.FromArgb(180, 100, 20);
            btnComputer.ForeColor = Color.White;
            btnComputer.FlatStyle = FlatStyle.Flat;
            btnComputer.FlatAppearance.BorderSize         = 0;
            btnComputer.FlatAppearance.MouseOverBackColor = Color.FromArgb(145, 78, 12);
            btnComputer.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnComputer.Cursor    = Cursors.Hand;
            btnComputer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnComputer.Location  = new Point(20, 118);
            btnComputer.Size      = new Size(195, 95);
            btnComputer.Click    += new System.EventHandler(this.btnComputer_Click);

            // ═══════════════════════════════════════════════════════════
            // BUTTON — ĐIỆN THOẠI  (right card)
            // ═══════════════════════════════════════════════════════════
            btnPhone.Text      = "📱  ĐIỆN THOẠI\r\nNhập thủ công\r\n(Lưu vào database)";
            btnPhone.BackColor = Color.FromArgb(64, 93, 230);
            btnPhone.ForeColor = Color.White;
            btnPhone.FlatStyle = FlatStyle.Flat;
            btnPhone.FlatAppearance.BorderSize         = 0;
            btnPhone.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 75, 195);
            btnPhone.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPhone.Cursor    = Cursors.Hand;
            btnPhone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnPhone.Location  = new Point(235, 118);
            btnPhone.Size      = new Size(195, 95);
            btnPhone.Click    += new System.EventHandler(this.btnPhone_Click);

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
            btnCancel.Location  = new Point(165, 228);
            btnCancel.Size      = new Size(120, 34);
            btnCancel.Click    += new System.EventHandler(this.btnCancel_Click);

            // ═══════════════════════════════════════════════════════════
            // FORM
            // ═══════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = AutoScaleMode.Font;
            this.BackColor           = Color.FromArgb(235, 238, 248);
            this.ClientSize          = new System.Drawing.Size(450, 278);
            this.Font                = new Font("Segoe UI", 9F);
            this.FormBorderStyle     = FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = FormStartPosition.CenterParent;
            this.Text                = "Đổi Mật Khẩu";

            this.Controls.Add(btnCancel);
            this.Controls.Add(btnPhone);
            this.Controls.Add(btnComputer);
            this.Controls.Add(_lblMessage);
            this.Controls.Add(_headerPanel);

            _headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
