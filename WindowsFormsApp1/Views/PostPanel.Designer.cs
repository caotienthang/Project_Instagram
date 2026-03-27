using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    partial class PostPanel
    {
        private System.Windows.Forms.FlowLayoutPanel flowImages;
        private System.Windows.Forms.Button btnSelectImages;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtCaption;

        private void InitializeComponent()
        {
            this.flowImages = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSelectImages = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtCaption = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // flowImages
            // 
            this.flowImages.Location = new System.Drawing.Point(20, 60);
            this.flowImages.Name = "flowImages";
            this.flowImages.Size = new System.Drawing.Size(600, 120);
            this.flowImages.TabIndex = 0;
            // 
            // btnSelectImages
            // 
            this.btnSelectImages.Location = new System.Drawing.Point(20, 20);
            this.btnSelectImages.Name = "btnSelectImages";
            this.btnSelectImages.Size = new System.Drawing.Size(75, 23);
            this.btnSelectImages.TabIndex = 2;
            this.btnSelectImages.Text = "Chọn ảnh";
            this.btnSelectImages.Click += new System.EventHandler(this.btnSelectImages_Click);
            // 
            // btnPost
            // 
            this.btnPost.Location = new System.Drawing.Point(20, 280);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 23);
            this.btnPost.TabIndex = 3;
            this.btnPost.Text = "Đăng";
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(120, 280);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtCaption
            // 
            this.txtCaption.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCaption.Location = new System.Drawing.Point(20, 200);
            this.txtCaption.Multiline = true;
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new System.Drawing.Size(600, 60);
            this.txtCaption.TabIndex = 1;
            // 
            // PostPanel
            // 
            this.BackColor = System.Drawing.Color.OldLace;
            this.Controls.Add(this.flowImages);
            this.Controls.Add(this.txtCaption);
            this.Controls.Add(this.btnSelectImages);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.btnClose);
            this.Name = "PostPanel";
            this.Size = new System.Drawing.Size(700, 400);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
