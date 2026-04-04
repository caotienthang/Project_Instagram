using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Views;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class DashBoard : Form
    {
        private List<AccountInfo> _accounts;
        private readonly AvatarService      _avatarService;
        private readonly InstagramPostService _postService;
        private readonly TwoFactorService   _twoFactorService;
        private bool _allSelected  = false;
        private bool _searchActive = false;

        // ── Status badge colors (new dark palette) ──────────────────
        private static readonly Color ClrReady   = Color.FromArgb(46,  204, 113);
        private static readonly Color ClrWorking = Color.FromArgb(241, 196,  15);
        private static readonly Color ClrSuccess = Color.FromArgb(39,  174,  96);
        private static readonly Color ClrError   = Color.FromArgb(231,  76,  60);
        private static readonly Color ClrDefault = Color.FromArgb(149, 165, 166);

        public DashBoard()
        {
            InitializeComponent();
            _avatarService    = new AvatarService();
            _postService      = new InstagramPostService();
            _twoFactorService = new TwoFactorService();
            SetupSearchPlaceholder();
            SetupGrid();
            LoadData();
        }

        // ═══════════════════════════════════════════════════════════
        // GRID SETUP
        // ═══════════════════════════════════════════════════════════
        private void SetupGrid()
        {
            grid.Columns.Clear();

            // Checkbox
            grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Select", HeaderText = "", Width = 36, Resizable = DataGridViewTriState.False
            });

            // Hidden ID
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id", Visible = false
            });

            // Text columns
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username",  MinimumWidth = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Full Name", MinimumWidth = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email",    HeaderText = "Email",     MinimumWidth = 160 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone",    HeaderText = "Phone",     MinimumWidth = 110 });

            // Avatar image
            grid.Columns.Add(new DataGridViewImageColumn
            {
                Name = "Avatar", HeaderText = "Avatar",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 64, Resizable = DataGridViewTriState.False
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Birthday", HeaderText = "Birthday", MinimumWidth = 90 });

            // Status / Log
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status", HeaderText = "Status / Log", MinimumWidth = 160
            });

            // Get-Cookie action button
            grid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "GetCookie", HeaderText = "Action",
                Text = "🍪 Get Cookie", UseColumnTextForButtonValue = true,
                Width = 110, Resizable = DataGridViewTriState.False
            });

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowTemplate.Height  = 54;
            grid.AllowUserToAddRows  = false;
        }

        // ═══════════════════════════════════════════════════════════
        // LOAD DATA
        // ═══════════════════════════════════════════════════════════
        private void LoadData()
        {
            _accounts = AccountRepository.GetAll();
            grid.Rows.Clear();

            foreach (var acc in _accounts)
            {
                int rowIndex = grid.Rows.Add(
                    false, acc.Id,
                    acc.Username, acc.FullName, acc.Email, acc.Phone,
                    LoadAvatar(acc.LocalPathAvatar),
                    acc.Birthday,
                    "Ready"
                );
                ApplyStatusStyle(grid.Rows[rowIndex], "ready");
            }

            UpdateSelectedCount();
        }

        private Image LoadAvatar(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    return Image.FromStream(fs);
            }
            catch { return null; }
        }

        // ═══════════════════════════════════════════════════════════
        // STATUS STYLING
        // ═══════════════════════════════════════════════════════════
        private void ApplyStatusStyle(DataGridViewRow row, string statusType)
        {
            Color bg;
            switch ((statusType ?? "").ToLower())
            {
                case "ready":    bg = ClrReady;   break;
                case "working":  bg = ClrWorking; break;
                case "success":  bg = ClrSuccess; break;
                case "error":    bg = ClrError;   break;
                default:         bg = ClrDefault; break;
            }
            row.Cells["Status"].Style.BackColor          = bg;
            row.Cells["Status"].Style.ForeColor          = Color.White;
            row.Cells["Status"].Style.SelectionBackColor = bg;
            row.Cells["Status"].Style.SelectionForeColor = Color.White;
            row.Cells["Status"].Style.Font               = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            row.Cells["Status"].Style.Alignment          = DataGridViewContentAlignment.MiddleCenter;
        }

        private void UpdateAccountStatus(int accountId, string statusText, string statusType = "working")
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells["Id"].Value != null &&
                    Convert.ToInt32(row.Cells["Id"].Value) == accountId)
                {
                    row.Cells["Status"].Value = statusText;
                    ApplyStatusStyle(row, statusType);
                    grid.Refresh();
                    Application.DoEvents();
                    break;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // SELECTED COUNT
        // ═══════════════════════════════════════════════════════════
        private void UpdateSelectedCount()
        {
            int count = grid.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["Select"].Value != null && (bool)r.Cells["Select"].Value == true);
            lblSelectedCount.Text = $"{count} selected";
        }

        // ═══════════════════════════════════════════════════════════
        // SEARCH  (placeholder behaviour for .NET 4.8)
        // ═══════════════════════════════════════════════════════════
        private void SetupSearchPlaceholder()
        {
            txtSearch.GotFocus += (s, e) =>
            {
                if (!_searchActive)
                {
                    txtSearch.Text      = "";
                    txtSearch.ForeColor = Color.FromArgb(200, 205, 230);
                    _searchActive       = true;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    _searchActive       = false;
                    txtSearch.Text      = "🔍  Search accounts...";
                    txtSearch.ForeColor = Color.FromArgb(105, 115, 155);
                }
            };
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!_searchActive) return;
            string q = txtSearch.Text.ToLower();
            foreach (DataGridViewRow row in grid.Rows)
            {
                string user  = (row.Cells["Username"].Value ?? "").ToString().ToLower();
                string name  = (row.Cells["FullName"].Value ?? "").ToString().ToLower();
                string email = (row.Cells["Email"].Value    ?? "").ToString().ToLower();
                row.Visible  = string.IsNullOrEmpty(q) || user.Contains(q) || name.Contains(q) || email.Contains(q);
            }
            UpdateSelectedCount();
        }

        // ═══════════════════════════════════════════════════════════
        // GRID EVENTS
        // ═══════════════════════════════════════════════════════════
        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Checkbox column → update count
            if (e.ColumnIndex == grid.Columns["Select"].Index)
            {
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                UpdateSelectedCount();
                return;
            }

            // Get-Cookie button column
            if (e.ColumnIndex == grid.Columns["GetCookie"].Index)
            {
                int accountId = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["Id"].Value);
                var account   = _accounts.FirstOrDefault(x => x.Id == accountId);
                if (account == null) return;

                // Hỏi user chọn phương thức
                var deviceType = DeviceSelectionDialog.ShowGetCookieDialog(account.Username, this);

                if (deviceType == DeviceSelectionDialog.DeviceType.None)
                    return; // User cancelled

                if (deviceType == DeviceSelectionDialog.DeviceType.Computer)
                {
                    // Computer: WebView2 (existing logic)
                    AppendLog($"🍪 Getting cookie for: {account.Username} (WebView2)");

                    using (var form = new AccountsCenterForm(account))
                        form.ShowDialog();

                    LoadData();
                    AppendLog($"✅ Cookie updated: {account.Username}");
                }
                else if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
                {
                    // Phone: Login via API
                    AppendLog($"📱 Getting session for: {account.Username} (Phone API)");

                    using (var form = new PhoneLoginDialog(account))
                    {
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            LoadData();
                            AppendLog($"✅ Session updated: {account.Username}");
                        }
                        else
                        {
                            AppendLog($"❌ Login cancelled: {account.Username}");
                        }
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — SELECT ALL / DESELECT ALL
        // ═══════════════════════════════════════════════════════════
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            _allSelected = !_allSelected;
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Visible)
                    row.Cells["Select"].Value = _allSelected;
            }
            btnSelectAll.Text = _allSelected ? "☑  Deselect All" : "☐  Select All";
            UpdateSelectedCount();
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — CHECK 2FA (selected accounts)
        // ═══════════════════════════════════════════════════════════
        private async void btn2FA_Click(object sender, EventArgs e)
        {
            var selected = GetCheckedAccounts();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least 1 account.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var account in selected)
            {
                if (string.IsNullOrEmpty(account.FbAccountId))
                {
                    AppendLog($"[{account.Username}] ❌ No FbAccountId — please use \"Get Cookie\" first.");
                    UpdateAccountStatus(account.Id, "No FbAccountId", "error");
                    continue;
                }

                var session = InstagramSessionRepository.GetByAccountId(account.Id);
                if (session == null)
                {
                    AppendLog($"[{account.Username}] ❌ No session — please use \"Get Cookie\" first.");
                    UpdateAccountStatus(account.Id, "No session", "error");
                    continue;
                }

                UpdateAccountStatus(account.Id, "Checking 2FA…", "working");
                AppendLog($"[{account.Username}] 🔐 Checking 2FA status…");

                try
                {
                    TwoFactorStatus status;

                    // Determine which API to use based on available sessions
                    bool hasPhoneSession = !string.IsNullOrWhiteSpace(session.AuthorizationPhone);
                    bool hasComputerSession = !string.IsNullOrWhiteSpace(session.Cookie);

                    // Always ask user to choose method (unless only one is available)
                    DeviceSelectionDialog.DeviceType deviceType;

                    if (hasPhoneSession && hasComputerSession)
                    {
                        // Both methods available - ask user
                        deviceType = DeviceSelectionDialog.ShowGetCookieDialog(account.Username, this);
                        if (deviceType == DeviceSelectionDialog.DeviceType.None)
                            continue; // User cancelled
                    }
                    else if (hasPhoneSession)
                    {
                        deviceType = DeviceSelectionDialog.DeviceType.Phone;
                        AppendLog($"[{account.Username}] 📱 Using Phone API (only available method)");
                    }
                    else if (hasComputerSession)
                    {
                        deviceType = DeviceSelectionDialog.DeviceType.Computer;
                        AppendLog($"[{account.Username}] 💻 Using Computer API (only available method)");
                    }
                    else
                    {
                        AppendLog($"[{account.Username}] ❌ No valid session found");
                        UpdateAccountStatus(account.Id, "No session", "error");
                        continue;
                    }

                    // Call appropriate API based on selection
                    if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
                    {
                        var phoneSvc = new TwoFactorPhoneService();
                        status = await phoneSvc.GetStatusByPhoneAsync(account.FbAccountId, session);
                    }
                    else
                    {
                        status = await _twoFactorService.GetStatusAsync(account.FbAccountId, session);
                    }

                    if (!string.IsNullOrEmpty(status.Error))
                    {
                        AppendLog($"[{account.Username}] ❌ {status.Error}");
                        UpdateAccountStatus(account.Id, "2FA error", "error");
                        continue;
                    }

                    AppendLog($"[{account.Username}] {(status.IsTotpEnabled ? "🔐 ENABLED" : "🔓 DISABLED")} — " +
                              $"Devices: {status.Seeds.Count}  SMS: {(status.IsSmsEnabled ? "ON" : "OFF")}");

                    UpdateAccountStatus(account.Id,
                        status.IsTotpEnabled ? "2FA: ON ✓" : "2FA: OFF",
                        status.IsTotpEnabled ? "success"  : "working");

                    using (var dlg = new TwoFactorManagerDialog(account, status, _twoFactorService, session))
                    {
                        dlg.OnLog += msg => AppendLog(msg);
                        dlg.ShowDialog(this);

                        if (dlg.NeedsRefresh)
                            UpdateAccountStatus(account.Id,
                                dlg.FinalTotpEnabled ? "2FA: ON ✓" : "2FA: OFF",
                                dlg.FinalTotpEnabled ? "success"  : "working");
                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"[{account.Username}] ❌ 2FA error: {ex.Message}");
                    UpdateAccountStatus(account.Id, "2FA error", "error");
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — CHANGE AVATAR
        // ═══════════════════════════════════════════════════════════
        private async void btnChangeAvatar_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedAccounts = GetCheckedAccounts();
                if (selectedAccounts.Count == 0)
                {
                    MessageBox.Show("Please select at least 1 account.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ImageSourceDialog.SourceType sourceType;
                using (var dialog = new ImageSourceDialog())
                {
                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    sourceType = dialog.SelectedSource;
                }
                if (sourceType == ImageSourceDialog.SourceType.None) return;

                var imagePaths = new List<string>();

                if (sourceType == ImageSourceDialog.SourceType.Folder)
                {
                    using (var fd = new FolderBrowserDialog
                        { Description = "Select folder containing avatar images", ShowNewFolderButton = false })
                    {
                        if (fd.ShowDialog() != DialogResult.OK) return;
                        imagePaths = Directory.GetFiles(fd.SelectedPath)
                            .Where(f => IsImageFile(f)).ToList();

                        if (imagePaths.Count == 0)
                        {
                            MessageBox.Show("No image files found in the selected folder.", "Empty Folder",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        AppendLog($"📁 Folder: {fd.SelectedPath}  ({imagePaths.Count} images)");
                    }
                }
                else
                {
                    using (var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png", Title = "Select Avatar Image" })
                    {
                        if (ofd.ShowDialog() != DialogResult.OK) return;
                        imagePaths.Add(ofd.FileName);
                        AppendLog($"📄 File: {Path.GetFileName(ofd.FileName)}");
                    }
                }

                var rng = new Random();
                foreach (var account in selectedAccounts)
                {
                    UpdateAccountStatus(account.Id, "Changing avatar…", "working");
                    var session = InstagramSessionRepository.GetByAccountId(account.Id);
                    if (session == null)
                    {
                        AppendLog($"[{account.Username}] ❌ No session found");
                        UpdateAccountStatus(account.Id, "No session", "error");
                        continue;
                    }
                    string img = imagePaths[rng.Next(imagePaths.Count)];
                    AppendLog($"[{account.Username}] 📷 → {Path.GetFileName(img)}");
                    var result = await _avatarService.UploadAvatar(img, session);
                    if (result.success)
                    {
                        account.LocalPathAvatar = result.localPath;
                        account.LinkAvatar = result.remoteUrl;
                        AccountRepository.UpdateAvatar(account.Id, result.localPath, result.remoteUrl);
                        AppendLog($"[{account.Username}] ✅ Avatar changed");
                        UpdateAccountStatus(account.Id, "Avatar updated ✓", "success");
                    }
                    else
                    {
                        AppendLog($"[{account.Username}] ❌ {result.message}");
                        UpdateAccountStatus(account.Id, "Failed", "error");
                    }
                    await Task.Delay(rng.Next(1000, 2000));
                }

                LoadData();
            }
            catch (Exception ex)
            {
                AppendLog($"❌ Error: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — POST
        // ═══════════════════════════════════════════════════════════
        private void btnPost_Click(object sender, EventArgs e)
        {
            var selectedAccounts = GetCheckedAccounts();
            if (selectedAccounts.Count == 0)
            {
                MessageBox.Show("Please select at least 1 account.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            postContainer.Controls.Clear();
            var postPanel = new PostPanel(selectedAccounts);

            postPanel.OnLog          += msg  => AppendLog(msg);
            postPanel.OnStatusUpdate += (id, text, type) => UpdateAccountStatus(id, text, type);
            postPanel.OnClose        += () =>
            {
                postContainer.Visible = false;
                postContainer.Controls.Clear();
                logPanel.Visible = true;
            };

            postContainer.Controls.Add(postPanel);
            logPanel.Visible      = false;
            postContainer.Visible = true;
            postContainer.BringToFront();
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — REFRESH
        // ═══════════════════════════════════════════════════════════
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            AppendLog("🔄 Data refreshed.");
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — SETTINGS
        // ═══════════════════════════════════════════════════════════
        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var dialog = new SettingsDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AppendLog("⚙️ Settings updated successfully.");
                    MessageBox.Show(
                        "✅ Cài đặt đã được cập nhật!\n\nCác thay đổi sẽ có hiệu lực cho các thao tác tiếp theo.",
                        "Settings Updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — CHANGE PASSWORD
        // ═══════════════════════════════════════════════════════════
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            var selected = GetCheckedAccounts();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select 1 account to change password.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (selected.Count > 1)
            {
                MessageBox.Show("Please select only 1 account at a time to change password.", "Multiple Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var account = selected[0];

            // Sử dụng dialog chung
            var deviceType = DeviceSelectionDialog.ShowChangePasswordDialog(this);

            if (deviceType == DeviceSelectionDialog.DeviceType.Computer)
            {
                var session = InstagramSessionRepository.GetByAccountId(account.Id);
                if (session == null)
                {
                    MessageBox.Show(
                        $"Tài khoản @{account.Username} chưa có session.\nVui lòng dùng \"Get Cookie\" trước.",
                        "Không có Session", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AppendLog($"[{account.Username}] 🔑 Mở trang đổi mật khẩu qua WebView2...");
                using (var form = new ChangePasswordWebForm(account, session))
                    form.ShowDialog(this);
                AppendLog($"[{account.Username}] ✅ Hoàn tất đổi mật khẩu (session đã xóa).");
            }
            else if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
            {
                using (var dlg = new ChangePasswordDialog(account))
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                        AppendLog($"[{account.Username}] 🔑 Password updated successfully.");
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // BUTTON — ADD ACCOUNT
        // ═══════════════════════════════════════════════════════════
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            // Sử dụng dialog chung
            var deviceType = DeviceSelectionDialog.ShowAddAccountDialog(this);

            if (deviceType == DeviceSelectionDialog.DeviceType.Computer)
            {
                AppendLog("🖥️ Mở WebView2 đăng nhập tài khoản mới...");
                using (var form = new AccountsCenterForm())
                    form.ShowDialog(this);
                LoadData();
                AppendLog("✅ Cập nhật danh sách tài khoản.");
            }
            else if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
            {
                AppendLog("📱 Thêm tài khoản thủ công...");
                using (var form = new PhoneLoginDialog())
                {
                    if (form.ShowDialog(this) == DialogResult.OK && form.SavedAccount != null)
                    {
                        LoadData();
                        AppendLog($"✅ Đã thêm tài khoản: @{form.SavedAccount.Username}");
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════
        private List<AccountInfo> GetCheckedAccounts()
        {
            var list = new List<AccountInfo>();
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells["Select"].Value != null && (bool)row.Cells["Select"].Value == true)
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    var acc = _accounts.FirstOrDefault(x => x.Id == id);
                    if (acc != null) list.Add(acc);
                }
            }
            return list;
        }

        private void AppendLog(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}]  {message}\n";
            logBox.AppendText(line);
            logBox.ScrollToCaret();
        }

        private static bool IsImageFile(string path) =>
            path.EndsWith(".jpg",  StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".png",  StringComparison.OrdinalIgnoreCase);
    }
}
