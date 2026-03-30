using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    partial class TwoFactorManagerDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader   = new System.Windows.Forms.Panel();
            this.lblTitle    = new System.Windows.Forms.Label();
            this.pnlFooter   = new System.Windows.Forms.Panel();
            this.btnAdd      = new System.Windows.Forms.Button();
            this.btnClose    = new System.Windows.Forms.Button();
            this.pnlContent  = new System.Windows.Forms.Panel();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlDevices  = new System.Windows.Forms.FlowLayoutPanel();
            this.lblStatus   = new System.Windows.Forms.Label();

            this.pnlHeader.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();

            // ── Header ──────────────────────────────────────────────
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(18, 18, 35);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 55;
            this.pnlHeader.Controls.Add(this.lblTitle);

            this.lblTitle.Text      = "🔐  Quản lý 2FA";
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Location  = new System.Drawing.Point(18, 14);
            this.lblTitle.AutoSize  = true;

            // ── Footer ──────────────────────────────────────────────
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(22, 24, 45);
            this.pnlFooter.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height    = 58;
            this.pnlFooter.Controls.Add(this.btnAdd);
            this.pnlFooter.Controls.Add(this.btnClose);

            this.btnAdd.Text      = "➕  Thêm thiết bị";
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(22, 160, 133);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.FlatAppearance.BorderSize         = 0;
            this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(17, 128, 106);
            this.btnAdd.Font     = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnAdd.Cursor   = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.Location = new System.Drawing.Point(14, 12);
            this.btnAdd.Size     = new System.Drawing.Size(155, 34);
            this.btnAdd.Click   += new System.EventHandler(this.btnAdd_Click);

            this.btnClose.Text      = "Đóng";
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(52, 58, 90);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize         = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(68, 75, 115);
            this.btnClose.Font     = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnClose.Cursor   = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(180, 12);
            this.btnClose.Size     = new System.Drawing.Size(90, 34);
            this.btnClose.Click   += new System.EventHandler(this.btnClose_Click);

            // ── Content ──────────────────────────────────────────────
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(30, 33, 58);
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;

            this.lblSubtitle.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(150, 160, 200);
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblSubtitle.Height    = 32;
            this.lblSubtitle.Padding   = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlDevices.BackColor      = System.Drawing.Color.FromArgb(30, 33, 58);
            this.pnlDevices.Dock           = System.Windows.Forms.DockStyle.Fill;
            this.pnlDevices.FlowDirection  = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlDevices.WrapContents   = false;
            this.pnlDevices.AutoScroll     = true;
            this.pnlDevices.Padding        = new System.Windows.Forms.Padding(14, 4, 14, 4);

            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(100, 220, 140);
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Height    = 24;
            this.lblStatus.Padding   = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Order: last added = first docked
            this.pnlContent.Controls.Add(this.pnlDevices);   // Fill
            this.pnlContent.Controls.Add(this.lblStatus);    // Bottom
            this.pnlContent.Controls.Add(this.lblSubtitle);  // Top

            // ── Form ──────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(30, 33, 58);
            this.ClientSize          = new System.Drawing.Size(460, 360);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                = "Quản lý 2FA";

            // Order: last added = first docked
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel           pnlHeader;
        private System.Windows.Forms.Label           lblTitle;
        private System.Windows.Forms.Panel           pnlFooter;
        private System.Windows.Forms.Button          btnAdd;
        private System.Windows.Forms.Button          btnClose;
        private System.Windows.Forms.Panel           pnlContent;
        private System.Windows.Forms.Label           lblSubtitle;
        private System.Windows.Forms.FlowLayoutPanel pnlDevices;
        private System.Windows.Forms.Label           lblStatus;
    }
}
