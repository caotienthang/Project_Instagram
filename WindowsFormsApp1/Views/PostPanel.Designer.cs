using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    partial class PostPanel
    {
        private System.ComponentModel.IContainer components = null;

        // ── Layout ───────────────────────────────────────────────────
        private Panel            _headerPanel;
        private TableLayoutPanel _tableMain;
        private Panel            _pnlImageCard;
        private Panel            _pnlImgBtns;
        private Panel            _pnlContentCard;
        private Panel            _pnlHint;
        private Panel            _pnlAddBtnBar;
        private Panel            _pnlActions;

        // ── Header ───────────────────────────────────────────────────
        private Label _lblTitle;

        // ── Image section ────────────────────────────────────────────
        private Label           _lblImage;
        private Button          _btnSelectImage;
        private Label           _lblImageCount;
        private FlowLayoutPanel _flowImages;

        // ── Content section ──────────────────────────────────────────
        private Label  _lblContent;
        private Panel  _contentPanel;
        private Button _btnAddContent;

        // ── Actions ──────────────────────────────────────────────────
        private Button _btnPost;
        private Button _btnClose;

        // ── Data ─────────────────────────────────────────────────────
        private List<TextBox> _contentFields;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _contentFields = new List<TextBox>();

            _headerPanel    = new Panel();
            _lblTitle       = new Label();
            _tableMain      = new TableLayoutPanel();
            _pnlImageCard   = new Panel();
            _lblImage       = new Label();
            _pnlImgBtns     = new Panel();
            _btnSelectImage = new Button();
            _lblImageCount  = new Label();
            _flowImages     = new FlowLayoutPanel();
            _pnlContentCard = new Panel();
            _lblContent     = new Label();
            _pnlHint        = new Panel();
            _contentPanel   = new Panel();
            _pnlAddBtnBar   = new Panel();
            _btnAddContent  = new Button();
            _pnlActions     = new Panel();
            _btnPost        = new Button();
            _btnClose       = new Button();

            _headerPanel.SuspendLayout();
            _tableMain.SuspendLayout();
            _pnlImageCard.SuspendLayout();
            _pnlImgBtns.SuspendLayout();
            _pnlContentCard.SuspendLayout();
            _pnlHint.SuspendLayout();
            _pnlAddBtnBar.SuspendLayout();
            _pnlActions.SuspendLayout();
            this.SuspendLayout();

            // ═══════════════════════════════════════════════════════
            // HEADER  (55px · dark navy)
            // ═══════════════════════════════════════════════════════
            _headerPanel.BackColor = Color.FromArgb(18, 18, 35);
            _headerPanel.Dock      = DockStyle.Top;
            _headerPanel.Height    = 55;
            _headerPanel.Controls.Add(_lblTitle);

            _lblTitle.Text      = "✏️   Create New Post";
            _lblTitle.Font      = new Font("Segoe UI", 14F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.BackColor = Color.Transparent;
            _lblTitle.AutoSize  = false;
            _lblTitle.Dock      = DockStyle.Fill;
            _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lblTitle.Padding   = new Padding(18, 0, 0, 0);

            // ═══════════════════════════════════════════════════════
            // ACTION BAR  (62px · dark)
            // ═══════════════════════════════════════════════════════
            _pnlActions.BackColor = Color.FromArgb(26, 28, 52);
            _pnlActions.Dock      = DockStyle.Bottom;
            _pnlActions.Height    = 62;
            _pnlActions.Controls.Add(_btnClose);
            _pnlActions.Controls.Add(_btnPost);

            _btnPost.Text      = "✅   POST NOW";
            _btnPost.BackColor = Color.FromArgb(39, 174, 96);
            _btnPost.ForeColor = Color.White;
            _btnPost.FlatStyle = FlatStyle.Flat;
            _btnPost.FlatAppearance.BorderSize         = 0;
            _btnPost.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 148, 78);
            _btnPost.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
            _btnPost.Cursor    = Cursors.Hand;
            _btnPost.Location  = new Point(18, 13);
            _btnPost.Size      = new Size(160, 36);
            _btnPost.Click    += BtnPost_Click;

            _btnClose.Text      = "✕   Cancel";
            _btnClose.BackColor = Color.FromArgb(85, 88, 120);
            _btnClose.ForeColor = Color.White;
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.FlatAppearance.BorderSize         = 0;
            _btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(105, 108, 142);
            _btnClose.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
            _btnClose.Cursor    = Cursors.Hand;
            _btnClose.Location  = new Point(190, 13);
            _btnClose.Size      = new Size(130, 36);
            _btnClose.Click    += BtnClose_Click;

            // ═══════════════════════════════════════════════════════
            // TABLE LAYOUT  (2 columns, fills remaining space)
            // ═══════════════════════════════════════════════════════
            _tableMain.Dock        = DockStyle.Fill;
            _tableMain.BackColor   = Color.FromArgb(215, 220, 240);
            _tableMain.ColumnCount = 2;
            _tableMain.RowCount    = 1;
            _tableMain.Padding     = new Padding(10);
            _tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            _tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            _tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // add cards — Dock=Fill, Margin creates the gap between them
            _tableMain.Controls.Add(_pnlImageCard,   0, 0);
            _tableMain.Controls.Add(_pnlContentCard, 1, 0);

            // ── IMAGE CARD ──────────────────────────────────────────
            _pnlImageCard.Dock      = DockStyle.Fill;
            _pnlImageCard.BackColor = Color.White;
            _pnlImageCard.Padding   = new Padding(16, 14, 16, 10);
            _pnlImageCard.Margin    = new Padding(0, 0, 6, 0);

            // Add order: Fill first → resolves last; Top panels after → stack downward
            _pnlImageCard.Controls.Add(_flowImages);
            _pnlImageCard.Controls.Add(_pnlImgBtns);
            _pnlImageCard.Controls.Add(_lblImage);

            _lblImage.Text      = "📷   Image Selection";
            _lblImage.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblImage.ForeColor = Color.FromArgb(32, 36, 68);
            _lblImage.Dock      = DockStyle.Top;
            _lblImage.Height    = 34;
            _lblImage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            _pnlImgBtns.Dock      = DockStyle.Top;
            _pnlImgBtns.Height    = 52;
            _pnlImgBtns.BackColor = Color.Transparent;
            _pnlImgBtns.Controls.Add(_lblImageCount);
            _pnlImgBtns.Controls.Add(_btnSelectImage);

            _btnSelectImage.Text      = "📂   Choose Images";
            _btnSelectImage.BackColor = Color.FromArgb(64, 93, 230);
            _btnSelectImage.ForeColor = Color.White;
            _btnSelectImage.FlatStyle = FlatStyle.Flat;
            _btnSelectImage.FlatAppearance.BorderSize         = 0;
            _btnSelectImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 75, 195);
            _btnSelectImage.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnSelectImage.Cursor    = Cursors.Hand;
            _btnSelectImage.Location  = new Point(0, 8);
            _btnSelectImage.Size      = new Size(165, 36);
            _btnSelectImage.Click    += BtnSelectImage_Click;

            _lblImageCount.Text      = "No images selected";
            _lblImageCount.Font      = new Font("Segoe UI", 9F, FontStyle.Italic);
            _lblImageCount.ForeColor = Color.FromArgb(120, 128, 160);
            _lblImageCount.Location  = new Point(175, 16);
            _lblImageCount.AutoSize  = true;

            _flowImages.Dock        = DockStyle.Fill;
            _flowImages.BackColor   = Color.FromArgb(245, 247, 252);
            _flowImages.AutoScroll  = true;
            _flowImages.BorderStyle = BorderStyle.None;
            _flowImages.Padding     = new Padding(4);

            // ── CONTENT CARD ────────────────────────────────────────
            _pnlContentCard.Dock      = DockStyle.Fill;
            _pnlContentCard.BackColor = Color.White;
            _pnlContentCard.Padding   = new Padding(16, 14, 16, 10);
            _pnlContentCard.Margin    = new Padding(6, 0, 0, 0);

            // Add order: Fill first, Bottom second, then Top panels stack downward
            _pnlContentCard.Controls.Add(_contentPanel);
            _pnlContentCard.Controls.Add(_pnlAddBtnBar);
            _pnlContentCard.Controls.Add(_pnlHint);
            _pnlContentCard.Controls.Add(_lblContent);

            _lblContent.Text      = "✍️   Post Captions";
            _lblContent.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblContent.ForeColor = Color.FromArgb(32, 36, 68);
            _lblContent.Dock      = DockStyle.Top;
            _lblContent.Height    = 34;
            _lblContent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            _pnlHint.Dock      = DockStyle.Top;
            _pnlHint.Height    = 28;
            _pnlHint.BackColor = Color.Transparent;
            _pnlHint.Controls.Add(new Label
            {
                Text      = "Add multiple captions — one will be picked randomly per account.",
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(140, 148, 175),
                Dock      = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            });

            _pnlAddBtnBar.Dock      = DockStyle.Bottom;
            _pnlAddBtnBar.Height    = 50;
            _pnlAddBtnBar.BackColor = Color.Transparent;
            _pnlAddBtnBar.Controls.Add(_btnAddContent);

            _btnAddContent.Text      = "➕   Add Caption";
            _btnAddContent.BackColor = Color.FromArgb(52, 152, 219);
            _btnAddContent.ForeColor = Color.White;
            _btnAddContent.FlatStyle = FlatStyle.Flat;
            _btnAddContent.FlatAppearance.BorderSize         = 0;
            _btnAddContent.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            _btnAddContent.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnAddContent.Cursor    = Cursors.Hand;
            _btnAddContent.Location  = new Point(0, 8);
            _btnAddContent.Size      = new Size(148, 34);
            _btnAddContent.Click    += BtnAddContent_Click;

            _contentPanel.Dock        = DockStyle.Fill;
            _contentPanel.BackColor   = Color.FromArgb(245, 247, 252);
            _contentPanel.AutoScroll  = true;
            _contentPanel.BorderStyle = BorderStyle.None;
            _contentPanel.Padding     = new Padding(6);

            // ═══════════════════════════════════════════════════════
            // COMPOSE FORM  (Fill → Bottom → Top resolved order)
            // ═══════════════════════════════════════════════════════
            this.BackColor = Color.FromArgb(235, 238, 248);
            this.Dock      = DockStyle.Fill;
            this.Controls.Add(_tableMain);
            this.Controls.Add(_pnlActions);
            this.Controls.Add(_headerPanel);

            _headerPanel.ResumeLayout(false);
            _tableMain.ResumeLayout(false);
            _pnlImageCard.ResumeLayout(false);
            _pnlImgBtns.ResumeLayout(false);
            _pnlImgBtns.PerformLayout();
            _pnlContentCard.ResumeLayout(false);
            _pnlHint.ResumeLayout(false);
            _pnlAddBtnBar.ResumeLayout(false);
            _pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);

            AddContentField();
        }
    }
}
