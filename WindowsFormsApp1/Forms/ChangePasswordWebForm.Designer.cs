namespace WindowsFormsApp1.Forms
{
    partial class ChangePasswordWebForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            // 
            // webView
            // 
            this.webView.AllowExternalDrop       = true;
            this.webView.CreationProperties      = null;
            this.webView.DefaultBackgroundColor  = System.Drawing.Color.White;
            this.webView.Dock                    = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location                = new System.Drawing.Point(0, 0);
            this.webView.Name                    = "webView";
            this.webView.Size                    = new System.Drawing.Size(1200, 800);
            this.webView.TabIndex                = 0;
            this.webView.ZoomFactor              = 1D;
            // 
            // ChangePasswordWebForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.webView);
            this.Name            = "ChangePasswordWebForm";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "Đổi mật khẩu — Instagram";
            this.WindowState     = System.Windows.Forms.FormWindowState.Maximized;

            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
    }
}
