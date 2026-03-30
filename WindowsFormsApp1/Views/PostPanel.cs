using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Views
{
    public partial class PostPanel : UserControl
    {
        private readonly InstagramPostService _postService;
        private List<AccountInfo> _selectedAccounts;

        public List<string> SelectedImagePaths { get; private set; }

        public List<string> Contents
        {
            get
            {
                return _contentFields
                    .Where(tb => tb.ForeColor != Color.Gray && !string.IsNullOrWhiteSpace(tb.Text))
                    .Select(tb => tb.Text)
                    .ToList();
            }
        }

        public event Action<string> OnLog; // Event để gửi log về DashBoard
        public event Action<int, string, string> OnStatusUpdate; // Event để update status (accountId, text, type)
        public event Action OnClose;

        public PostPanel(List<AccountInfo> selectedAccounts)
        {
            InitializeComponent();
            _postService = new InstagramPostService();
            _selectedAccounts = selectedAccounts;
            SelectedImagePaths = new List<string>();
        }

        private void AddContentField()
        {
            int index = _contentFields.Count;

            var textBox = new TextBox
            {
                Multiline  = true,
                Height     = 82,
                Left       = 0,
                Top        = index * 92,
                Width      = Math.Max(200, _contentPanel.ClientSize.Width),
                Anchor     = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font       = new Font("Segoe UI", 9.5F),
                Text       = $"Caption {index + 1}…",
                ForeColor  = Color.FromArgb(160, 165, 185),
                BackColor  = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            textBox.Enter += (s, e) =>
            {
                if (textBox.ForeColor == Color.FromArgb(160, 165, 185))
                {
                    textBox.Text      = "";
                    textBox.ForeColor = Color.FromArgb(30, 35, 60);
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text      = $"Caption {index + 1}…";
                    textBox.ForeColor = Color.FromArgb(160, 165, 185);
                }
            };

            _contentFields.Add(textBox);
            _contentPanel.Controls.Add(textBox);
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                ofd.Title = "Chọn ảnh để đăng";
                ofd.Multiselect = true; // Cho phép chọn nhiều ảnh

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SelectedImagePaths = ofd.FileNames.ToList();

                    _lblImageCount.Text      = $"{SelectedImagePaths.Count} image(s) selected";
                    _lblImageCount.ForeColor = Color.FromArgb(39, 174, 96);

                    // Clear preview cũ
                    _flowImages.Controls.Clear();

                    // Hiển thị preview cho từng ảnh
                    foreach (var imagePath in SelectedImagePaths)
                    {
                        try
                        {
                            var picBox = new PictureBox
                            {
                                Width = 100,
                                Height = 100,
                                SizeMode = PictureBoxSizeMode.Zoom,
                                BorderStyle = BorderStyle.FixedSingle,
                                Margin = new Padding(5)
                            };

                            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                picBox.Image = Image.FromStream(fs);
                            }

                            // Thêm tooltip hiển thị tên file
                            var tooltip = new ToolTip();
                            tooltip.SetToolTip(picBox, Path.GetFileName(imagePath));

                            _flowImages.Controls.Add(picBox);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi load ảnh {Path.GetFileName(imagePath)}: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void BtnAddContent_Click(object sender, EventArgs e)
        {
            AddContentField();
        }

        private async void BtnPost_Click(object sender, EventArgs e)
        {
            if (SelectedImagePaths == null || SelectedImagePaths.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 ảnh!");
                return;
            }

            if (Contents.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập ít nhất 1 nội dung!");
                return;
            }

            // Ẩn panel ngay lập tức, chạy ngầm
            OnClose?.Invoke();

            // Chạy async đăng bài
            await HandlePostAsync();
        }

        private async Task HandlePostAsync()
        {
            try
            {
                var imagePaths = SelectedImagePaths;
                var contents = Contents;

                string imageInfo = imagePaths.Count == 1 
                    ? $"Ảnh: {Path.GetFileName(imagePaths[0])}" 
                    : $"Ảnh: {imagePaths.Count} ảnh (sidecar)";

                OnLog?.Invoke($"\n========== BẮT ĐẦU ĐĂNG BÀI ==========\n");
                OnLog?.Invoke($"{imageInfo}\n");
                OnLog?.Invoke($"Số nội dung: {contents.Count}\n");
                OnLog?.Invoke($"Số account: {_selectedAccounts.Count}\n");
                OnLog?.Invoke($"→ Mỗi account sẽ đăng {contents.Count} bài\n");
                OnLog?.Invoke($"→ Sử dụng: {(imagePaths.Count == 1 ? "ConfigureSingle" : "ConfigureSidecar")}\n\n");

                var random = new Random();
                int totalPosts = 0;
                int successPosts = 0;

                foreach (var account in _selectedAccounts)
                {
                    var session = InstagramSessionRepository.GetByAccountId(account.Id);

                    if (session == null)
                    {
                        OnLog?.Invoke($"[{account.Username}] ❌ Không tìm thấy session, bỏ qua\n\n");
                        OnStatusUpdate?.Invoke(account.Id, "Lỗi: Không có session", "error");
                        continue;
                    }

                    // Update status: Đang đăng bài
                    OnStatusUpdate?.Invoke(account.Id, $"Đang đăng bài (0/{contents.Count})", "working");
                    OnLog?.Invoke($"━━━ [{account.Username}] Bắt đầu đăng {contents.Count} bài ━━━\n");

                    // Đăng từng content cho account này
                    int accountSuccessPosts = 0;
                    for (int i = 0; i < contents.Count; i++)
                    {
                        string caption = contents[i];
                        totalPosts++;

                        // Update status với progress
                        OnStatusUpdate?.Invoke(account.Id, $"Đang đăng bài ({i + 1}/{contents.Count})", "working");

                        OnLog?.Invoke($"[{account.Username}] 📝 Đang đăng bài {i + 1}/{contents.Count}...\n");
                        OnLog?.Invoke($"  Caption: {(caption.Length > 50 ? caption.Substring(0, 50) + "..." : caption)}\n");
                        OnLog?.Invoke($"  Số ảnh: {imagePaths.Count}\n");

                        // Sử dụng InstagramPostService - tự động chọn Single hoặc Sidecar
                        var result = await _postService.PostMedia(imagePaths, caption, session);

                        // Parse result để kiểm tra thành công
                        bool success = !result.Contains("fail") && !result.Contains("error") && !result.Contains("SESSION DIE");

                        if (success)
                        {
                            successPosts++;
                            accountSuccessPosts++;
                            OnLog?.Invoke($"[{account.Username}] ✅ Bài {i + 1} thành công!\n");
                        }
                        else
                        {
                            OnLog?.Invoke($"[{account.Username}] ❌ Bài {i + 1} lỗi: {result}\n");
                        }

                        // Delay 2-4s giữa các post
                        if (i < contents.Count - 1)
                        {
                            int delayMs = random.Next(2000, 4000);
                            OnLog?.Invoke($"  ⏱️ Chờ {delayMs / 1000}s...\n");
                            await Task.Delay(delayMs);
                        }
                    }

                    OnLog?.Invoke($"[{account.Username}] Hoàn tất {contents.Count} bài\n\n");

                    // Update status: Hoàn tất
                    OnStatusUpdate?.Invoke(account.Id, $"Đăng {accountSuccessPosts}/{contents.Count} bài", "success");

                    // Delay giữa các account
                    if (_selectedAccounts.IndexOf(account) < _selectedAccounts.Count - 1)
                    {
                        int accountDelayMs = random.Next(3000, 5000);
                        OnLog?.Invoke($"⏱️ Chuyển sang account tiếp theo sau {accountDelayMs / 1000}s...\n\n");
                        await Task.Delay(accountDelayMs);
                    }
                }

                OnLog?.Invoke($"========== HOÀN TẤT ==========\n");
                OnLog?.Invoke($"Tổng: {totalPosts} bài | Thành công: {successPosts} | Thất bại: {totalPosts - successPosts}\n\n");

                MessageBox.Show($"Đã đăng {successPosts}/{totalPosts} bài cho {_selectedAccounts.Count} account!");
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"❌ Lỗi: {ex.Message}\n");
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            OnClose?.Invoke();
        }
    }
}
