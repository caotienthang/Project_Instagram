using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    partial class TwoFactorAddDeviceDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.picQrCode = new System.Windows.Forms.PictureBox();
            this.lblKeyTitle = new System.Windows.Forms.Label();
            this.lblKeyText = new System.Windows.Forms.Label();
            this.lblCodeHint = new System.Windows.Forms.Label();
            this.pnlCode = new System.Windows.Forms.Panel();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).BeginInit();
            this.pnlCode.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(35)))));
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(482, 55);
            this.pnlHeader.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(239, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "➕  Thêm thiết bị 2FA";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(24)))), ((int)(((byte)(45)))));
            this.pnlFooter.Controls.Add(this.btnCancel);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 445);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(482, 58);
            this.pnlFooter.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(58)))), ((int)(((byte)(90)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(75)))), ((int)(((byte)(115)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(14, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 34);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(33)))), ((int)(((byte)(58)))));
            this.pnlContent.Controls.Add(this.lblInstruction);
            this.pnlContent.Controls.Add(this.picQrCode);
            this.pnlContent.Controls.Add(this.lblKeyTitle);
            this.pnlContent.Controls.Add(this.lblKeyText);
            this.pnlContent.Controls.Add(this.lblCodeHint);
            this.pnlContent.Controls.Add(this.pnlCode);
            this.pnlContent.Controls.Add(this.lblStatus);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 55);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(20, 12, 20, 8);
            this.pnlContent.Size = new System.Drawing.Size(482, 390);
            this.pnlContent.TabIndex = 0;
            // 
            // lblInstruction
            // 
            this.lblInstruction.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblInstruction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(220)))));
            this.lblInstruction.Location = new System.Drawing.Point(20, 12);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(400, 22);
            this.lblInstruction.TabIndex = 0;
            this.lblInstruction.Text = "Quét mã QR bằng ứng dụng xác thực của bạn";
            // 
            // picQrCode
            // 
            this.picQrCode.BackColor = System.Drawing.Color.White;
            this.picQrCode.Location = new System.Drawing.Point(115, 40);
            this.picQrCode.Name = "picQrCode";
            this.picQrCode.Size = new System.Drawing.Size(190, 190);
            this.picQrCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQrCode.TabIndex = 1;
            this.picQrCode.TabStop = false;
            // 
            // lblKeyTitle
            // 
            this.lblKeyTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblKeyTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblKeyTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(145)))), ((int)(((byte)(185)))));
            this.lblKeyTitle.Location = new System.Drawing.Point(20, 240);
            this.lblKeyTitle.Name = "lblKeyTitle";
            this.lblKeyTitle.Size = new System.Drawing.Size(110, 20);
            this.lblKeyTitle.TabIndex = 2;
            this.lblKeyTitle.Text = "Nhập thủ công:";
            // 
            // lblKeyText
            // 
            this.lblKeyText.BackColor = System.Drawing.Color.Transparent;
            this.lblKeyText.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.lblKeyText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            this.lblKeyText.Location = new System.Drawing.Point(20, 262);
            this.lblKeyText.Name = "lblKeyText";
            this.lblKeyText.Size = new System.Drawing.Size(400, 22);
            this.lblKeyText.TabIndex = 3;
            // 
            // lblCodeHint
            // 
            this.lblCodeHint.BackColor = System.Drawing.Color.Transparent;
            this.lblCodeHint.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCodeHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(220)))));
            this.lblCodeHint.Location = new System.Drawing.Point(20, 296);
            this.lblCodeHint.Name = "lblCodeHint";
            this.lblCodeHint.Size = new System.Drawing.Size(300, 20);
            this.lblCodeHint.TabIndex = 4;
            this.lblCodeHint.Text = "Nhập mã 6 chữ số từ ứng dụng xác thực:";
            // 
            // pnlCode
            // 
            this.pnlCode.BackColor = System.Drawing.Color.Transparent;
            this.pnlCode.Controls.Add(this.txtCode);
            this.pnlCode.Controls.Add(this.btnConfirm);
            this.pnlCode.Location = new System.Drawing.Point(20, 322);
            this.pnlCode.Name = "pnlCode";
            this.pnlCode.Size = new System.Drawing.Size(400, 38);
            this.pnlCode.TabIndex = 5;
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCode.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold);
            this.txtCode.ForeColor = System.Drawing.Color.White;
            this.txtCode.Location = new System.Drawing.Point(0, 4);
            this.txtCode.MaxLength = 6;
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(120, 35);
            this.txtCode.TabIndex = 0;
            this.txtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCode_KeyPress);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatAppearance.BorderSize = 0;
            this.btnConfirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(128)))), ((int)(((byte)(106)))));
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(130, 2);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(130, 34);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "✔  Xác nhận";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(220)))), ((int)(((byte)(140)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 368);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(400, 20);
            this.lblStatus.TabIndex = 6;
            // 
            // TwoFactorAddDeviceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(33)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(482, 503);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TwoFactorAddDeviceDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm thiết bị 2FA";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).EndInit();
            this.pnlCode.ResumeLayout(false);
            this.pnlCode.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel      pnlHeader;
        private System.Windows.Forms.Label      lblTitle;
        private System.Windows.Forms.Panel      pnlFooter;
        private System.Windows.Forms.Button     btnCancel;
        private System.Windows.Forms.Panel      pnlContent;
        private System.Windows.Forms.Label      lblInstruction;
        private System.Windows.Forms.PictureBox picQrCode;
        private System.Windows.Forms.Label      lblKeyTitle;
        private System.Windows.Forms.Label      lblKeyText;
        private System.Windows.Forms.Label      lblCodeHint;
        private System.Windows.Forms.Panel      pnlCode;
        private System.Windows.Forms.TextBox    txtCode;
        private System.Windows.Forms.Button     btnConfirm;
        private System.Windows.Forms.Label      lblStatus;
    }
}
