using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Forms;

namespace WindowsFormsApp1.Views
{
    public partial class TwoFactorManagerDialog : Form
    {
        private readonly TwoFactorService _service;
        private readonly TwoFactorPhoneService _phoneService;
        private readonly AccountInfo      _account;
        private readonly InstagramSession _session;
        private TwoFactorStatus           _status;
        private readonly bool             _usePhoneApi;

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

            // Determine if we should use phone API based on session
            _usePhoneApi = !string.IsNullOrWhiteSpace(session.AuthorizationPhone);
            if (_usePhoneApi)
            {
                _phoneService = new TwoFactorPhoneService();
            }

            FinalTotpEnabled   = status.IsTotpEnabled;
            lblTitle.Text      = $"🔐  Quản lý 2FA — @{account.Username}";
            btnAdd.Enabled     = status.CanAddAdditional;
            btnDisable.Enabled = status.IsTotpEnabled; // Only enable if 2FA is ON
            btnDisable.Visible = _usePhoneApi; // Only show for phone API (computer doesn't support disable)

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

            TwoFactorActionResult result;

            // Use consistent API based on how status was checked
            if (_usePhoneApi)
            {
                result = await _phoneService.RemoveTotpByPhoneAsync(_account.FbAccountId, seed.Id, _session);
            }
            else
            {
                result = await _service.DisableTotpAsync(_account.FbAccountId, _session, seed.Id);
            }

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

            // Use consistent API based on how status was checked
            if (_usePhoneApi)
            {
                // Phone API flow
                SetStatus("⏳ Đang tạo key TOTP mới…", Color.FromArgb(241, 196, 15));
                SetBusy(true);

                var phoneResult = await _phoneService.AddTotpByPhoneAsync(_account.FbAccountId, _session, keyNickname);

                if (!phoneResult.Success)
                {
                    OnLog?.Invoke($"[{_account.Username}] ❌ Thêm 2FA (Phone) thất bại: {phoneResult.Message}");
                    SetStatus($"❌ {phoneResult.Message}", Color.FromArgb(231, 76, 60));
                    SetBusy(false);
                    return;
                }

                using (var dlg = new TwoFactorAddDeviceDialog(phoneResult, _service, _session, _account.FbAccountId,
                    async (code) => await _phoneService.ConfirmTotpByPhoneAsync(_account.FbAccountId, code, _session, phoneResult)))
                {
                    dlg.ShowDialog(this);

                    if (dlg.DeviceAdded)
                    {
                        OnLog?.Invoke($"[{_account.Username}] ✅ Thêm thiết bị 2FA (Phone) thành công");
                        NeedsRefresh     = true;
                        FinalTotpEnabled = true;
                        await RefreshStatusAsync();
                    }
                }

                SetBusy(false);
            }
            else
            {
                // Computer API flow
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

                SetBusy(false);
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

            TwoFactorStatus newStatus;

            // Use consistent API based on how status was checked
            if (_usePhoneApi)
            {
                newStatus = await _phoneService.GetStatusByPhoneAsync(_account.FbAccountId, _session);
            }
            else
            {
                newStatus = await _service.GetStatusAsync(_account.FbAccountId, _session);
            }

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

        private async void btnDisable_Click(object sender, EventArgs e)
        {
            // Confirm with user
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn TẮT hoàn toàn 2FA cho tài khoản @{_account.Username}?\n\n" +
                "⚠️ Thao tác này sẽ:\n" +
                "• Xóa TẤT CẢ thiết bị TOTP đã đăng ký\n" +
                "• Tắt bảo mật 2 lớp hoàn toàn\n" +
                "• Không thể hoàn tác!",
                "⚠️ Xác nhận Tắt 2FA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (confirmResult != DialogResult.Yes)
                return;

            SetStatus("⏳ Đang tắt 2FA…", Color.FromArgb(241, 196, 15));
            SetBusy(true);

            // Only phone API supports disable
            if (_usePhoneApi && _phoneService != null)
            {
                var result = await _phoneService.DisableTotpByPhoneAsync(_account.FbAccountId, _session);

                if (result.Success)
                {
                    _status.IsTotpEnabled = false;
                    _status.Seeds.Clear();
                    _status.CanAddAdditional = true;
                    _status.CanDisable = false;

                    FinalTotpEnabled = false;
                    NeedsRefresh = true;

                    OnLog?.Invoke($"[{_account.Username}] ✅ Đã TẮT 2FA thành công");
                    SetStatus("✅ 2FA đã được TẮT", Color.FromArgb(46, 204, 113));

                    btnAdd.Enabled = true;
                    btnDisable.Enabled = false;
                    RenderDevices();

                    MessageBox.Show(
                        "✅ 2FA đã được tắt thành công!\n\n" +
                        "Tài khoản của bạn không còn được bảo vệ bởi xác thực 2 lớp.",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    OnLog?.Invoke($"[{_account.Username}] ❌ Tắt 2FA thất bại: {result.Message}");
                    SetStatus($"❌ {result.Message}", Color.FromArgb(231, 76, 60));
                }
            }
            else
            {
                SetStatus("❌ Chức năng chỉ hỗ trợ Phone API", Color.FromArgb(231, 76, 60));
            }

            SetBusy(false);
        }

        private void SetStatus(string msg, Color color)
        {
            lblStatus.Text      = msg;
            lblStatus.ForeColor = color;
            Application.DoEvents();
        }

        private void SetBusy(bool busy)
        {
            btnAdd.Enabled     = !busy && (_status?.CanAddAdditional ?? false);
            btnDisable.Enabled = !busy && (_status?.IsTotpEnabled ?? false) && _usePhoneApi;
            btnClose.Enabled   = !busy;

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
