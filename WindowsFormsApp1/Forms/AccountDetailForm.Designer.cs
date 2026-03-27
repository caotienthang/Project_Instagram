using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    partial class AccountDetailForm
    {
        private System.ComponentModel.IContainer components = null;

        private PictureBox picAvatar;
        private Label lblName;
        private Button btnPost;
        private Button btnChangeAvatar;
        private Panel panelContainer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.picAvatar = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnChangeAvatar = new System.Windows.Forms.Button();
            this.panelContainer = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // picAvatar
            // 
            this.picAvatar.Location = new System.Drawing.Point(20, 20);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(100, 100);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 0;
            this.picAvatar.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblName.Location = new System.Drawing.Point(140, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(220, 30);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Username";
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.Color.LightGreen;
            this.btnPost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPost.Location = new System.Drawing.Point(20, 150);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(100, 35);
            this.btnPost.TabIndex = 2;
            this.btnPost.Text = "Đăng bài";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnChangeAvatar
            // 
            this.btnChangeAvatar.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnChangeAvatar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeAvatar.Location = new System.Drawing.Point(140, 150);
            this.btnChangeAvatar.Name = "btnChangeAvatar";
            this.btnChangeAvatar.Size = new System.Drawing.Size(140, 35);
            this.btnChangeAvatar.TabIndex = 3;
            this.btnChangeAvatar.Text = "Đổi Avatar";
            this.btnChangeAvatar.UseVisualStyleBackColor = false;
            this.btnChangeAvatar.Click += new System.EventHandler(this.btnChangeAvatar_Click);
            // 
            // panelContainer
            // 
            this.panelContainer.Location = new System.Drawing.Point(20, 191);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(700, 400);
            this.panelContainer.TabIndex = 4;
            this.panelContainer.Visible = false;
            // 
            // AccountDetailForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1845, 844);
            this.Controls.Add(this.panelContainer);
            this.Controls.Add(this.picAvatar);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.btnChangeAvatar);
            this.Name = "AccountDetailForm";
            this.Text = "Account Detail";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.ResumeLayout(false);

        }
    }
}