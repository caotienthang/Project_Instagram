using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Views
{
    public partial class TwoFactorManagerDialog : Form
    {
        private readonly TwoFactorService _service;
        private readonly AccountInfo      _account;
        private readonly InstagramSession _session;
        private TwoFactorStatus           _status;

        // Row width inside the FlowLayoutPanel (460 - padding 14*2 - scrollbar ~17)
        private const int RowWidth  = 408;
        private const int RowHeight = 48;

        public bool NeedsRefresh     { get; private set; }
        public bool FinalTotpEnabled { get; private set; }

        public event Action<string> OnLog;

        public TwoFactorManagerDialog(AccountInfo account, TwoFactorStatus status,
                                      TwoFactorService service, InstagramSession session)
        {
            InitializeComponent();
            _account = account;
            _status  = status;
            _service = service;
            _session = session;

            FinalTotpEnabled   = status.IsTotpEnabled;
            lblTitle.Text      = $"🔐  Quản lý 2FA — @{account.Username}";
            btnAdd.Enabled     = status.CanAddAdditional;

            RenderDevices();
        }

        // ── Render device rows ──────────────────────────────────────
        private void RenderDevices()
        {
            pnlDevices.SuspendLayout();
            pnlDevices.Controls.Clear();

            if (!_status.IsTotpEnabled)
            {
                lblSubtitle.Text = "🔓  2FA chưa được bật";
                pnlDevices.ResumeLayout();
                return;
            }

            bool canRemove = _status.Seeds.Count >= 2;

            foreach (var seed in _status.Seeds)
                pnlDevices.Controls.Add(CreateDeviceRow(seed, canRemove));

            lblSubtitle.Text = _status.Seeds.Count > 0
                ? $"Thiết bị đã kết nối ({_status.Seeds.Count})"
                : "Chưa có thiết bị nào";

            pnlDevices.ResumeLayout();
        }

        private Panel CreateDeviceRow(TotpSeed seed, bool canRemove)
        {
            var row = new Panel
            {
                Size      = new Size(RowWidth, RowHeight),
                BackColor = Color.FromArgb(40, 44, 72),
                Margin    = new Padding(0, 0, 0, 6),
                Tag       = seed.Id
            };

            var lbl = new Label
            {
                Text      = $"📱  {seed.Name}  |  ID: {seed.Id}",
                Font      = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location  = new Point(12, 0),
                Size      = new Size(RowWidth - 105, RowHeight),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var btnRemove = new Button
            {
                Text      = "Gỡ",
                Tag       = seed,
                Size      = new Size(76, 30),
                Location  = new Point(RowWidth - 90, (RowHeight - 30) / 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = canRemove ? Color.FromArgb(192, 57, 43) : Color.FromArgb(65, 70, 100),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Enabled   = canRemove
            };
            btnRemove.FlatAppearance.BorderSize         = 0;
            btnRemove.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 76, 60);
            btnRemove.Click += async (s, e) => await RemoveDevice_ClickAsync((TotpSeed)((Button)s).Tag);

            row.Controls.Add(lbl);
            row.Controls.Add(btnRemove);
            return row;
        }

        // ── Remove device ────────────────────────────────────────────
        private async Task RemoveDevice_ClickAsync(TotpSeed seed)
        {
            SetStatus($"⏳ Đang gỡ \"{seed.Name}\"…", Color.FromArgb(241, 196, 15));
            SetBusy(true);

            var result = await _service.DisableTotpAsync(_account.FbAccountId, _session, seed.Id);

            if (result.Success)
            {
                _status.Seeds.RemoveAll(s => s.Id == seed.Id);
                if (_status.Seeds.Count == 0)
                {
                    _status.IsTotpEnabled    = false;
                    _status.CanAddAdditional = true;
                    _status.CanDisable       = false;
                }

                FinalTotpEnabled = _status.IsTotpEnabled;
                NeedsRefresh     = true;

                OnLog?.Invoke($"[{_account.Username}] ✅ Đã gỡ thiết bị: {seed.Name}");
                SetStatus($"✅ Đã gỡ: {seed.Name}", Color.FromArgb(46, 204, 113));
                btnAdd.Enabled = _status.CanAddAdditional;
                RenderDevices();
            }
            else
            {
                OnLog?.Invoke($"[{_account.Username}] ❌ Gỡ thất bại: {result.Message}");
                SetStatus($"❌ {result.Message}", Color.FromArgb(231, 76, 60));
            }

            SetBusy(false);
        }

        // ── Add device ───────────────────────────────────────────────
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string keyNickname = null;
            if (_status.Seeds.Count > 0)
            {
                keyNickname = PromptDeviceName();
                if (keyNickname == null) return;
                if (string.IsNullOrEmpty(keyNickname))
                {
                    SetStatus("⚠️ Vui lòng nhập tên thiết bị.", Color.FromArgb(241, 196, 15));
                    return;
                }
            }

            SetStatus("⏳ Đang tạo key TOTP mới…", Color.FromArgb(241, 196, 15));
            SetBusy(true);

            var key = await _service.GenerateTotpKeyAsync(_account.FbAccountId, _session, keyNickname);
            if (!key.Success)
            {
                SetStatus($"❌ {key.Message}", Color.FromArgb(231, 76, 60));
                SetBusy(false);
                return;
            }


            using (var dlg = new TwoFactorAddDeviceDialog(key, _service, _session, _account.FbAccountId))
            {
                dlg.ShowDialog(this);

                if (dlg.DeviceAdded)
                {
                    OnLog?.Invoke($"[{_account.Username}] ✅ Thêm thiết bị 2FA thành công");
                    NeedsRefresh     = true;
                    FinalTotpEnabled = true;
                    await RefreshStatusAsync();
                }
            }
        }

        private string PromptDeviceName()
        {
            string name = null;
            using (var frm = new Form())
            {
                frm.Text            = "Tên thiết bị";
                frm.Size            = new Size(340, 155);
                frm.StartPosition   = FormStartPosition.CenterParent;
                frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                frm.MaximizeBox     = false;
                frm.MinimizeBox     = false;
                frm.BackColor       = Color.FromArgb(30, 33, 58);

                var lbl = new Label
                {
                    Text      = "Nhập tên thiết bị:",
                    Location  = new Point(14, 14),
                    AutoSize  = true,
                    ForeColor = Color.White,
                    Font      = new Font("Segoe UI", 9.5F)
                };
                var txt = new TextBox
                {
                    Location  = new Point(14, 38),
                    Width     = 295,
                    Font      = new Font("Segoe UI", 9.5F),
                    MaxLength = 50
                };
                var btnOk = new Button
                {
                    Text         = "OK",
                    DialogResult = DialogResult.OK,
                    Location     = new Point(155, 74),
                    Width        = 76,
                    FlatStyle    = FlatStyle.Flat,
                    BackColor    = Color.FromArgb(46, 204, 113),
                    ForeColor    = Color.White,
                    Font         = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnOk.FlatAppearance.BorderSize = 0;
                var btnCancel = new Button
                {
                    Text         = "Hủy",
                    DialogResult = DialogResult.Cancel,
                    Location     = new Point(238, 74),
                    Width        = 76,
                    FlatStyle    = FlatStyle.Flat,
                    BackColor    = Color.FromArgb(65, 70, 100),
                    ForeColor    = Color.White,
                    Font         = new Font("Segoe UI", 9F)
                };
                btnCancel.FlatAppearance.BorderSize = 0;

                frm.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
                frm.AcceptButton = btnOk;
                frm.CancelButton = btnCancel;

                if (frm.ShowDialog(this) == DialogResult.OK)
                    name = txt.Text.Trim();
            }
            return name;
        }

        // ── Refresh status from API ──────────────────────────────────
        private async Task RefreshStatusAsync()
        {
            SetStatus("⏳ Đang tải lại danh sách…", Color.FromArgb(241, 196, 15));
            var newStatus = await _service.GetStatusAsync(_account.FbAccountId, _session);
            if (string.IsNullOrEmpty(newStatus.Error))
            {
                _status          = newStatus;
                FinalTotpEnabled = _status.IsTotpEnabled;
                btnAdd.Enabled   = _status.CanAddAdditional;
                RenderDevices();
                SetStatus("", Color.FromArgb(100, 220, 140));
            }
            else
            {
                SetStatus($"❌ Reload: {newStatus.Error}", Color.FromArgb(231, 76, 60));
            }
        }

        // ── Helpers ──────────────────────────────────────────────────
        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void SetStatus(string msg, Color color)
        {
            lblStatus.Text      = msg;
            lblStatus.ForeColor = color;
            Application.DoEvents();
        }

        private void SetBusy(bool busy)
        {
            btnAdd.Enabled   = !busy && (_status?.CanAddAdditional ?? false);
            btnClose.Enabled = !busy;

            bool canRemove = !busy && (_status?.Seeds.Count >= 2);
            foreach (Control row in pnlDevices.Controls)
                foreach (Control c in row.Controls)
                    if (c is Button b && b.Text == "Gỡ")
                    {
                        b.Enabled   = canRemove;
                        b.BackColor = canRemove
                            ? Color.FromArgb(192, 57, 43)
                            : Color.FromArgb(65, 70, 100);
                    }
        }
    }
}
