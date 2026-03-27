using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    partial class DashBoard
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel sidebar;
        private System.Windows.Forms.Panel header;
        private System.Windows.Forms.Panel toolbar;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel logPanel;

        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.TextBox txtSearch;

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnGetCookie;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label logLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.sidebar = new System.Windows.Forms.Panel();
            this.header = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.toolbar = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnGetCookie = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.grid = new System.Windows.Forms.DataGridView();
            this.logPanel = new System.Windows.Forms.Panel();
            this.logLabel = new System.Windows.Forms.Label();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.header.SuspendLayout();
            this.toolbar.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.logPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidebar
            // 
            this.sidebar.BackColor = System.Drawing.SystemColors.Menu;
            this.sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidebar.Location = new System.Drawing.Point(0, 0);
            this.sidebar.Name = "sidebar";
            this.sidebar.Size = new System.Drawing.Size(200, 700);
            this.sidebar.TabIndex = 4;
            // 
            // header
            // 
            this.header.BackColor = System.Drawing.Color.Indigo;
            this.header.Controls.Add(this.lblTitle);
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Location = new System.Drawing.Point(200, 0);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(1000, 50);
            this.header.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ACCOUNTS MANAGER";
            // 
            // toolbar
            // 
            this.toolbar.BackColor = System.Drawing.Color.Lavender;
            this.toolbar.Controls.Add(this.btnAdd);
            this.toolbar.Controls.Add(this.btnStart);
            this.toolbar.Controls.Add(this.btnStop);
            this.toolbar.Controls.Add(this.btnDelete);
            this.toolbar.Controls.Add(this.btnRefresh);
            this.toolbar.Controls.Add(this.btnGetCookie);
            this.toolbar.Controls.Add(this.txtSearch);
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Location = new System.Drawing.Point(200, 50);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(1000, 50);
            this.toolbar.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.Black;
            this.btnAdd.Location = new System.Drawing.Point(10, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "+ Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.LightGreen;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.Color.Black;
            this.btnStart.Location = new System.Drawing.Point(100, 10);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "▶ Start";
            this.btnStart.UseVisualStyleBackColor = false;
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.IndianRed;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(190, 10);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "⏹ Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.LightCoral;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(280, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "❌ Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Khaki;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.Location = new System.Drawing.Point(380, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnGetCookie
            // 
            this.btnGetCookie.BackColor = System.Drawing.Color.MediumPurple;
            this.btnGetCookie.FlatAppearance.BorderSize = 0;
            this.btnGetCookie.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetCookie.ForeColor = System.Drawing.Color.White;
            this.btnGetCookie.Location = new System.Drawing.Point(470, 10);
            this.btnGetCookie.Name = "btnGetCookie";
            this.btnGetCookie.Size = new System.Drawing.Size(100, 23);
            this.btnGetCookie.TabIndex = 6;
            this.btnGetCookie.Text = "🍪 Get Cookie";
            this.btnGetCookie.UseVisualStyleBackColor = false;
            this.btnGetCookie.Click += new System.EventHandler(this.btnGetCookie_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(780, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 22);
            this.txtSearch.TabIndex = 5;
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(47)))));
            this.mainPanel.Controls.Add(this.grid);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(200, 100);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1000, 450);
            this.mainPanel.TabIndex = 0;
            // 
            // grid
            // 
            this.grid.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grid.ColumnHeadersHeight = 29;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.EnableHeadersVisualStyles = false;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.RowHeadersWidth = 51;
            this.grid.RowTemplate.Height = 50;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(1000, 450);
            this.grid.TabIndex = 0;
            this.grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellDoubleClick);
            // 
            // logPanel
            // 
            this.logPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(35)))));
            this.logPanel.Controls.Add(this.logLabel);
            this.logPanel.Controls.Add(this.logBox);
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logPanel.Location = new System.Drawing.Point(200, 550);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(1000, 150);
            this.logPanel.TabIndex = 1;
            // 
            // logLabel
            // 
            this.logLabel.ForeColor = System.Drawing.Color.White;
            this.logLabel.Location = new System.Drawing.Point(10, 5);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(100, 23);
            this.logLabel.TabIndex = 0;
            this.logLabel.Text = "LOG";
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.Color.Black;
            this.logBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logBox.ForeColor = System.Drawing.Color.LightGreen;
            this.logBox.Location = new System.Drawing.Point(0, 30);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(1000, 120);
            this.logBox.TabIndex = 1;
            this.logBox.Text = "";
            // 
            // DashBoard
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(47)))));
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.logPanel);
            this.Controls.Add(this.toolbar);
            this.Controls.Add(this.header);
            this.Controls.Add(this.sidebar);
            this.Name = "DashBoard";
            this.Text = "Instagram Automation Tool";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.header.ResumeLayout(false);
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.logPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}