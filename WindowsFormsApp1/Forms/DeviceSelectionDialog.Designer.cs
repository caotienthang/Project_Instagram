using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    partial class DeviceSelectionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        // UI Components
        private Label lblMessage;
        private FlowLayoutPanel buttonPanel;
        private Button btnComputer;
        private Button btnPhone;
        private Button btnCancel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            
            // ═══════════════════════════════════════════════════════════
            // FORM SETTINGS
            // ═══════════════════════════════════════════════════════════
            this.SuspendLayout();
            
            this.Text = _title;
            this.Size = new Size(480, 240);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.BackColor = Color.FromArgb(44, 62, 80);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(480, 240);
            this.Name = "DeviceSelectionDialog";

            // ═══════════════════════════════════════════════════════════
            // MESSAGE LABEL
            // ═══════════════════════════════════════════════════════════
            this.lblMessage = new Label
            {
                Text = _message,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(20, 30, 20, 10),
                Name = "lblMessage"
            };

            // ═══════════════════════════════════════════════════════════
            // BUTTON PANEL
            // ═══════════════════════════════════════════════════════════
            this.buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(30, 10, 30, 20),
                AutoSize = true,
                Name = "buttonPanel"
            };

            // ═══════════════════════════════════════════════════════════
            // COMPUTER BUTTON
            // ═══════════════════════════════════════════════════════════
            this.btnComputer = CreateStyledButton(
                _computerLabel, 
                Color.FromArgb(52, 152, 219),
                "btnComputer"
            );
            this.btnComputer.Click += BtnComputer_Click;
            this.buttonPanel.Controls.Add(this.btnComputer);

            // ═══════════════════════════════════════════════════════════
            // PHONE BUTTON
            // ═══════════════════════════════════════════════════════════
            this.btnPhone = CreateStyledButton(
                _phoneLabel, 
                Color.FromArgb(46, 204, 113),
                "btnPhone"
            );
            this.btnPhone.Click += BtnPhone_Click;
            this.buttonPanel.Controls.Add(this.btnPhone);

            // ═══════════════════════════════════════════════════════════
            // CANCEL BUTTON (Optional)
            // ═══════════════════════════════════════════════════════════
            if (_showCancelButton)
            {
                this.btnCancel = CreateStyledButton(
                    "❌ Hủy", 
                    Color.FromArgb(149, 165, 166),
                    "btnCancel"
                );
                this.btnCancel.Click += BtnCancel_Click;
                this.buttonPanel.Controls.Add(this.btnCancel);
            }

            // ═══════════════════════════════════════════════════════════
            // ADD CONTROLS TO FORM
            // ═══════════════════════════════════════════════════════════
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.lblMessage);

            // ═══════════════════════════════════════════════════════════
            // SET DEFAULT BUTTONS
            // ═══════════════════════════════════════════════════════════
            this.AcceptButton = this.btnComputer;
            if (_showCancelButton)
                this.CancelButton = this.btnCancel;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        /// <summary>
        /// Tạo button với style đồng nhất
        /// </summary>
        private Button CreateStyledButton(string text, Color backColor, string name)
        {
            var btn = new Button
            {
                Text = text,
                Name = name,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(130, 50),
                Margin = new Padding(5),
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                TabStop = true,
                UseVisualStyleBackColor = false
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.2f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.1f);

            // Hover effect
            var originalColor = backColor;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(originalColor, 0.2f);
            btn.MouseLeave += (s, e) => btn.BackColor = originalColor;

            return btn;
        }
    }
}
