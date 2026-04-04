using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Views
{
    public partial class TwoFactorAddDeviceDialog : Form
    {
        private readonly TwoFactorService _service;
        private readonly InstagramSession _session;
        private readonly string           _accountId;
        private readonly string           _qrCodeUri;
        private readonly string           _keyText;
        private readonly Func<string, Task<TwoFactorActionResult>> _confirmCallback;

        public bool DeviceAdded { get; private set; }

        public TwoFactorAddDeviceDialog(TotpKeyResult key,
                                        TwoFactorService service,
                                        InstagramSession session,
                                        string accountId,
                                        Func<string, Task<TwoFactorActionResult>> confirmCallback = null)
        {
            InitializeComponent();
            _service   = service;
            _session   = session;
            _accountId = accountId;
            _qrCodeUri = key.QrCodeUri;
            _keyText   = key.KeyText;
            _confirmCallback = confirmCallback ?? (async code => await _service.ConfirmTotpAsync(_accountId, code, _session));

            lblKeyText.Text = key.KeyText ?? "";

            this.Shown += async (s, e) => await LoadQrCodeAsync();
        }

        // ── Load QR image from URL ───────────────────────────────────
        private async Task LoadQrCodeAsync()
        {
            SetStatus("⏳ Đang tải mã QR…", Color.FromArgb(241, 196, 15));
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0");
                    byte[] bytes = await client.GetByteArrayAsync(_qrCodeUri);
                    using (var ms = new MemoryStream(bytes))
                        picQrCode.Image = new Bitmap(ms);
                }
                SetStatus("", Color.White);
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Không tải được QR: {ex.Message}", Color.FromArgb(231, 76, 60));
            }
        }

        // ── Confirm button ───────────────────────────────────────────
        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            if (code.Length != 6)
            {
                SetStatus("⚠️ Vui lòng nhập đúng 6 chữ số.", Color.FromArgb(241, 196, 15));
                return;
            }

            SetStatus("⏳ Đang xác nhận…", Color.FromArgb(241, 196, 15));
            SetBusy(true);

            var result = await _confirmCallback(code);

            if (result.Success)
            {
                DeviceAdded = true;
                SetStatus("✅ Thêm thiết bị thành công!", Color.FromArgb(46, 204, 113));
                await Task.Delay(800);
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else
            {
                SetStatus($"❌ {result.Message}", Color.FromArgb(231, 76, 60));
                SetBusy(false);
            }
        }

        // ── Cancel button ────────────────────────────────────────────
        private void btnCancel_Click(object sender, EventArgs e) => Close();

        // ── Only allow digits in the code box ────────────────────────
        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // ── Helpers ──────────────────────────────────────────────────
        private void SetStatus(string msg, Color color)
        {
            lblStatus.Text      = msg;
            lblStatus.ForeColor = color;
            Application.DoEvents();
        }

        private void SetBusy(bool busy)
        {
            btnConfirm.Enabled = !busy;
            btnCancel.Enabled  = !busy;
            txtCode.Enabled    = !busy;
        }
    }
}
