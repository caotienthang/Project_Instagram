using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    partial class DashBoard
    {
        private System.ComponentModel.IContainer components = null;

        // Layout panels
        private System.Windows.Forms.Panel header;
        private System.Windows.Forms.Panel toolbar;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel logPanel;
        private System.Windows.Forms.Panel postContainer;

        // Header controls
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;

        // Toolbar controls
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnChangeAvatar;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btn2FA;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSelectedCount;

        // Grid
        private System.Windows.Forms.DataGridView grid;

        // Log panel
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.RichTextBox logBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            var colHeaderStyle  = new System.Windows.Forms.DataGridViewCellStyle();
            var rowStyle        = new System.Windows.Forms.DataGridViewCellStyle();
            var altRowStyle     = new System.Windows.Forms.DataGridViewCellStyle();

            this.header         = new System.Windows.Forms.Panel();
            this.lblTitle       = new System.Windows.Forms.Label();
            this.lblSubtitle    = new System.Windows.Forms.Label();
            this.toolbar        = new System.Windows.Forms.Panel();
            this.btnPost        = new System.Windows.Forms.Button();
            this.btnChangeAvatar = new System.Windows.Forms.Button();
            this.btnSelectAll   = new System.Windows.Forms.Button();
            this.btnRefresh     = new System.Windows.Forms.Button();
            this.btn2FA        = new System.Windows.Forms.Button();
            this.txtSearch      = new System.Windows.Forms.TextBox();
            this.lblSelectedCount = new System.Windows.Forms.Label();
            this.mainPanel      = new System.Windows.Forms.Panel();
            this.grid           = new System.Windows.Forms.DataGridView();
            this.postContainer  = new System.Windows.Forms.Panel();
            this.logPanel       = new System.Windows.Forms.Panel();
            this.logLabel       = new System.Windows.Forms.Label();
            this.logBox         = new System.Windows.Forms.RichTextBox();

            this.header.SuspendLayout();
            this.toolbar.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.logPanel.SuspendLayout();
            this.SuspendLayout();

            // ═══════════════════════════════════════════════════════════
            // HEADER  (65px · dark navy)
            // ═══════════════════════════════════════════════════════════
            this.header.BackColor = System.Drawing.Color.FromArgb(18, 18, 35);
            this.header.Dock      = System.Windows.Forms.DockStyle.Top;
            this.header.Height    = 65;
            this.header.Controls.Add(this.lblSubtitle);
            this.header.Controls.Add(this.lblTitle);

            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Location  = new System.Drawing.Point(22, 10);
            this.lblTitle.AutoSize  = true;
            this.lblTitle.Text      = "📸  Instagram MMO Tool";

            this.lblSubtitle.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(105, 115, 155);
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Location  = new System.Drawing.Point(25, 42);
            this.lblSubtitle.AutoSize  = true;
            this.lblSubtitle.Text      = "Multi-Account Manager  ·  Auto Post  ·  Avatar Changer";

            // ═══════════════════════════════════════════════════════════
            // TOOLBAR  (58px · dark)
            // ═══════════════════════════════════════════════════════════
            this.toolbar.BackColor = System.Drawing.Color.FromArgb(26, 28, 52);
            this.toolbar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Height    = 58;
            this.toolbar.Controls.Add(this.btnPost);
            this.toolbar.Controls.Add(this.btnChangeAvatar);
            this.toolbar.Controls.Add(this.btnSelectAll);
            this.toolbar.Controls.Add(this.btnRefresh);
            this.toolbar.Controls.Add(this.btn2FA);
            this.toolbar.Controls.Add(this.txtSearch);
            this.toolbar.Controls.Add(this.lblSelectedCount);

            // --- btnPost ---
            this.btnPost.Text      = "📤  Post to Instagram";
            this.btnPost.BackColor = System.Drawing.Color.FromArgb(225, 48, 108);
            this.btnPost.ForeColor = System.Drawing.Color.White;
            this.btnPost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPost.FlatAppearance.BorderSize           = 0;
            this.btnPost.FlatAppearance.MouseOverBackColor   = System.Drawing.Color.FromArgb(190, 35, 88);
            this.btnPost.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPost.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnPost.Location  = new System.Drawing.Point(12, 11);
            this.btnPost.Size      = new System.Drawing.Size(168, 36);
            this.btnPost.TabIndex  = 0;
            this.btnPost.Click    += new System.EventHandler(this.btnPost_Click);

            // --- btnChangeAvatar ---
            this.btnChangeAvatar.Text      = "🖼️  Change Avatar";
            this.btnChangeAvatar.BackColor = System.Drawing.Color.FromArgb(131, 58, 180);
            this.btnChangeAvatar.ForeColor = System.Drawing.Color.White;
            this.btnChangeAvatar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeAvatar.FlatAppearance.BorderSize         = 0;
            this.btnChangeAvatar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(105, 45, 148);
            this.btnChangeAvatar.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnChangeAvatar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnChangeAvatar.Location  = new System.Drawing.Point(190, 11);
            this.btnChangeAvatar.Size      = new System.Drawing.Size(152, 36);
            this.btnChangeAvatar.TabIndex  = 1;
            this.btnChangeAvatar.Click    += new System.EventHandler(this.btnChangeAvatar_Click);

            // --- btnSelectAll ---
            this.btnSelectAll.Text      = "☐  Select All";
            this.btnSelectAll.BackColor = System.Drawing.Color.FromArgb(64, 93, 230);
            this.btnSelectAll.ForeColor = System.Drawing.Color.White;
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectAll.FlatAppearance.BorderSize         = 0;
            this.btnSelectAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50, 75, 195);
            this.btnSelectAll.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSelectAll.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnSelectAll.Location  = new System.Drawing.Point(352, 11);
            this.btnSelectAll.Size      = new System.Drawing.Size(120, 36);
            this.btnSelectAll.TabIndex  = 2;
            this.btnSelectAll.Click    += new System.EventHandler(this.btnSelectAll_Click);

            // --- btnRefresh ---
            this.btnRefresh.Text      = "🔄  Refresh";
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(52, 58, 90);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize         = 0;
            this.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(68, 75, 115);
            this.btnRefresh.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Location  = new System.Drawing.Point(482, 11);
            this.btnRefresh.Size      = new System.Drawing.Size(112, 36);
            this.btnRefresh.TabIndex  = 3;
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            // --- btn2FA ---
            this.btn2FA.Text      = "🔐  Check 2FA";
            this.btn2FA.BackColor = System.Drawing.Color.FromArgb(22, 160, 133);
            this.btn2FA.ForeColor = System.Drawing.Color.White;
            this.btn2FA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn2FA.FlatAppearance.BorderSize         = 0;
            this.btn2FA.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(17, 128, 106);
            this.btn2FA.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btn2FA.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn2FA.Location  = new System.Drawing.Point(604, 11);
            this.btn2FA.Size      = new System.Drawing.Size(130, 36);
            this.btn2FA.TabIndex  = 5;
            this.btn2FA.Click    += new System.EventHandler(this.btn2FA_Click);

            // --- txtSearch  (right-anchored) ---
            this.txtSearch.BackColor   = System.Drawing.Color.FromArgb(40, 43, 72);
            this.txtSearch.ForeColor   = System.Drawing.Color.FromArgb(175, 180, 210);
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font        = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearch.Location    = new System.Drawing.Point(850, 18);
            this.txtSearch.Size        = new System.Drawing.Size(220, 28);
            this.txtSearch.Anchor      = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.txtSearch.TabIndex    = 4;
            this.txtSearch.Text        = "🔍  Search accounts...";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // --- lblSelectedCount (right-anchored) ---
            this.lblSelectedCount.ForeColor   = System.Drawing.Color.FromArgb(52, 210, 120);
            this.lblSelectedCount.Font        = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblSelectedCount.Location    = new System.Drawing.Point(1082, 20);
            this.lblSelectedCount.Size        = new System.Drawing.Size(105, 22);
            this.lblSelectedCount.Anchor      = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblSelectedCount.Text        = "0 selected";
            this.lblSelectedCount.TextAlign   = System.Drawing.ContentAlignment.MiddleLeft;

            // ═══════════════════════════════════════════════════════════
            // GRID  (fills mainPanel)
            // ═══════════════════════════════════════════════════════════
            colHeaderStyle.BackColor        = System.Drawing.Color.FromArgb(32, 36, 68);
            colHeaderStyle.ForeColor        = System.Drawing.Color.White;
            colHeaderStyle.Font             = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            colHeaderStyle.Alignment        = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            colHeaderStyle.WrapMode         = System.Windows.Forms.DataGridViewTriState.False;
            colHeaderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(32, 36, 68);
            colHeaderStyle.SelectionForeColor = System.Drawing.Color.White;
            colHeaderStyle.Padding          = new System.Windows.Forms.Padding(10, 0, 0, 0);

            rowStyle.BackColor              = System.Drawing.Color.White;
            rowStyle.ForeColor              = System.Drawing.Color.FromArgb(45, 50, 70);
            rowStyle.Font                   = new System.Drawing.Font("Segoe UI", 9.5F);
            rowStyle.SelectionBackColor     = System.Drawing.Color.FromArgb(232, 243, 255);
            rowStyle.SelectionForeColor     = System.Drawing.Color.FromArgb(25, 40, 90);
            rowStyle.Padding                = new System.Windows.Forms.Padding(8, 0, 0, 0);

            altRowStyle.BackColor           = System.Drawing.Color.FromArgb(248, 250, 255);
            altRowStyle.ForeColor           = System.Drawing.Color.FromArgb(45, 50, 70);
            altRowStyle.Font                = new System.Drawing.Font("Segoe UI", 9.5F);
            altRowStyle.SelectionBackColor  = System.Drawing.Color.FromArgb(232, 243, 255);
            altRowStyle.SelectionForeColor  = System.Drawing.Color.FromArgb(25, 40, 90);
            altRowStyle.Padding             = new System.Windows.Forms.Padding(8, 0, 0, 0);

            this.grid.BackgroundColor               = System.Drawing.Color.White;
            this.grid.ColumnHeadersDefaultCellStyle = colHeaderStyle;
            this.grid.ColumnHeadersHeight           = 42;
            this.grid.DefaultCellStyle              = rowStyle;
            this.grid.AlternatingRowsDefaultCellStyle = altRowStyle;
            this.grid.Dock                          = System.Windows.Forms.DockStyle.Fill;
            this.grid.EnableHeadersVisualStyles     = false;
            this.grid.GridColor                     = System.Drawing.Color.FromArgb(228, 232, 248);
            this.grid.BorderStyle                   = System.Windows.Forms.BorderStyle.None;
            this.grid.CellBorderStyle               = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grid.RowHeadersVisible             = false;
            this.grid.RowTemplate.Height            = 54;
            this.grid.SelectionMode                 = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.TabIndex                      = 0;
            this.grid.CellContentClick             += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellContentClick);

            // ═══════════════════════════════════════════════════════════
            // POST CONTAINER  (overlaps grid when visible)
            // ═══════════════════════════════════════════════════════════
            this.postContainer.BackColor = System.Drawing.Color.FromArgb(235, 238, 248);
            this.postContainer.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.postContainer.Visible   = false;

            // ═══════════════════════════════════════════════════════════
            // MAIN PANEL
            // ═══════════════════════════════════════════════════════════
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Controls.Add(this.postContainer);
            this.mainPanel.Controls.Add(this.grid);

            // ═══════════════════════════════════════════════════════════
            // LOG PANEL  (120px · terminal dark)
            // ═══════════════════════════════════════════════════════════
            this.logPanel.BackColor = System.Drawing.Color.FromArgb(13, 15, 28);
            this.logPanel.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.logPanel.Height    = 122;
            this.logPanel.Controls.Add(this.logLabel);
            this.logPanel.Controls.Add(this.logBox);

            this.logLabel.Text      = "◉  ACTIVITY LOG";
            this.logLabel.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.logLabel.ForeColor = System.Drawing.Color.FromArgb(52, 210, 120);
            this.logLabel.Location  = new System.Drawing.Point(14, 6);
            this.logLabel.AutoSize  = true;

            this.logBox.BackColor   = System.Drawing.Color.FromArgb(13, 15, 28);
            this.logBox.ForeColor   = System.Drawing.Color.FromArgb(72, 225, 140);
            this.logBox.Font        = new System.Drawing.Font("Consolas", 9F);
            this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logBox.Dock        = System.Windows.Forms.DockStyle.Bottom;
            this.logBox.Height      = 96;
            this.logBox.ReadOnly    = true;
            this.logBox.TabIndex    = 1;
            this.logBox.Text        = "";

            // ═══════════════════════════════════════════════════════════
            // FORM
            // ═══════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(235, 238, 248);
            this.ClientSize          = new System.Drawing.Size(1280, 800);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize         = new System.Drawing.Size(1100, 650);
            this.Text                = "📸 Instagram MMO Tool";
            this.WindowState         = System.Windows.Forms.FormWindowState.Maximized;

            // Order matters for DockStyle resolution (last added = first docked)
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.logPanel);
            this.Controls.Add(this.toolbar);
            this.Controls.Add(this.header);

            this.header.ResumeLayout(false);
            this.header.PerformLayout();
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.logPanel.ResumeLayout(false);
            this.logPanel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}