using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    partial class ImageSourceDialog
    {
        private System.ComponentModel.IContainer components = null;

        private Panel  _headerPanel;
        private Label  _lblTitle;
        private Label  lblMessage;
        private Button btnFolder;
        private Button btnFile;
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
            lblMessage   = new Label();
            btnFolder    = new Button();
            btnFile      = new Button();
            btnCancel    = new Button();

            _headerPanel.SuspendLayout();
            this.SuspendLayout();

            // ═══════════════════════════════════════════════════════════
            // HEADER  (52px · dark navy)
            // ═══════════════════════════════════════════════════════════
            _headerPanel.BackColor = Color.FromArgb(18, 18, 35);
            _headerPanel.Dock      = DockStyle.Top;
            _headerPanel.Height    = 52;
            _headerPanel.Controls.Add(_lblTitle);

            _lblTitle.Text      = "📂   Choose Image Source";
            _lblTitle.Font      = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Dock      = DockStyle.Fill;
            _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lblTitle.Padding   = new Padding(16, 0, 0, 0);

            // ═══════════════════════════════════════════════════════════
            // SUB-TITLE  (description)
            // ═══════════════════════════════════════════════════════════
            lblMessage.Text      = "Select how you want to provide avatar images for the selected accounts:";
            lblMessage.Font      = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblMessage.ForeColor = Color.FromArgb(80, 90, 120);
            lblMessage.Location  = new Point(20, 62);
            lblMessage.Size      = new Size(420, 36);
            lblMessage.AutoSize  = false;

            // ═══════════════════════════════════════════════════════════
            // BUTTON — FOLDER  (left card)
            // ═══════════════════════════════════════════════════════════
            btnFolder.Text      = "📁  FOLDER\r\nRandom image from folder";
            btnFolder.BackColor = Color.FromArgb(64, 93, 230);
            btnFolder.ForeColor = Color.White;
            btnFolder.FlatStyle = FlatStyle.Flat;
            btnFolder.FlatAppearance.BorderSize         = 0;
            btnFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 75, 195);
            btnFolder.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnFolder.Cursor    = Cursors.Hand;
            btnFolder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnFolder.Location  = new Point(20, 108);
            btnFolder.Size      = new Size(195, 90);
            btnFolder.Click    += new System.EventHandler(this.btnFolder_Click);

            // ═══════════════════════════════════════════════════════════
            // BUTTON — FILE  (right card)
            // ═══════════════════════════════════════════════════════════
            btnFile.Text      = "🖼️  FILE\r\nSame image for all accounts";
            btnFile.BackColor = Color.FromArgb(39, 174, 96);
            btnFile.ForeColor = Color.White;
            btnFile.FlatStyle = FlatStyle.Flat;
            btnFile.FlatAppearance.BorderSize         = 0;
            btnFile.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 148, 78);
            btnFile.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnFile.Cursor    = Cursors.Hand;
            btnFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnFile.Location  = new Point(235, 108);
            btnFile.Size      = new Size(195, 90);
            btnFile.Click    += new System.EventHandler(this.btnFile_Click);

            // ═══════════════════════════════════════════════════════════
            // BUTTON — CANCEL
            // ═══════════════════════════════════════════════════════════
            btnCancel.Text      = "✕   Cancel";
            btnCancel.BackColor = Color.FromArgb(85, 88, 120);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize         = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(105, 108, 142);
            btnCancel.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.Cursor    = Cursors.Hand;
            btnCancel.Location  = new Point(165, 212);
            btnCancel.Size      = new Size(120, 34);
            btnCancel.Click    += new System.EventHandler(this.btnCancel_Click);

            // ═══════════════════════════════════════════════════════════
            // FORM
            // ═══════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = AutoScaleMode.Font;
            this.BackColor           = Color.FromArgb(235, 238, 248);
            this.ClientSize          = new System.Drawing.Size(450, 262);
            this.Font                = new Font("Segoe UI", 9F);
            this.FormBorderStyle     = FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = FormStartPosition.CenterParent;
            this.Text                = "Choose Image Source";

            this.Controls.Add(btnCancel);
            this.Controls.Add(btnFile);
            this.Controls.Add(btnFolder);
            this.Controls.Add(lblMessage);
            this.Controls.Add(_headerPanel);

            _headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
