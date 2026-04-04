using System;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    /// <summary>
    /// Dialog chung để chọn "Máy tính" hoặc "Điện thoại"
    /// Dễ dàng mở rộng cho các tùy chọn khác sau này
    /// </summary>
    public partial class DeviceSelectionDialog : Form
    {
        // ══════════════════════════════════════════════════════════════
        // ENUMS & PROPERTIES
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Các loại thiết bị có thể chọn (dễ mở rộng)
        /// </summary>
        public enum DeviceType
        {
            None,
            Computer,
            Phone
        }

        /// <summary>
        /// Loại thiết bị người dùng đã chọn
        /// </summary>
        public DeviceType SelectedDevice { get; private set; } = DeviceType.None;

        // ══════════════════════════════════════════════════════════════
        // PRIVATE FIELDS
        // ══════════════════════════════════════════════════════════════

        private readonly string _title;
        private readonly string _message;
        private readonly string _computerLabel;
        private readonly string _phoneLabel;
        private readonly bool _showCancelButton;

        // ══════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Tạo dialog với custom title và message
        /// </summary>
        /// <param name="title">Tiêu đề dialog</param>
        /// <param name="message">Mô tả yêu cầu</param>
        /// <param name="computerLabel">Label cho nút Computer (mặc định: "🖥️ Máy tính")</param>
        /// <param name="phoneLabel">Label cho nút Phone (mặc định: "📱 Điện thoại")</param>
        /// <param name="showCancelButton">Hiện nút Cancel hay không (mặc định: true)</param>
        public DeviceSelectionDialog(
            string title = "Chọn Phương Thức",
            string message = "Bạn muốn sử dụng phương thức nào?",
            string computerLabel = "🖥️ Máy tính",
            string phoneLabel = "📱 Điện thoại",
            bool showCancelButton = true)
        {
            _title = title;
            _message = message;
            _computerLabel = computerLabel;
            _phoneLabel = phoneLabel;
            _showCancelButton = showCancelButton;

            InitializeComponent();
        }

        // ══════════════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ══════════════════════════════════════════════════════════════

        private void BtnComputer_Click(object sender, EventArgs e)
        {
            SelectedDevice = DeviceType.Computer;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnPhone_Click(object sender, EventArgs e)
        {
            SelectedDevice = DeviceType.Phone;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SelectedDevice = DeviceType.None;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ══════════════════════════════════════════════════════════════
        // STATIC HELPER METHODS - Sử dụng nhanh
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Hiện dialog chọn phương thức thêm account
        /// </summary>
        public static DeviceType ShowAddAccountDialog(IWin32Window owner = null)
        {
            using (var dlg = new DeviceSelectionDialog(
                title: "Thêm Tài Khoản",
                message: "Bạn muốn thêm tài khoản bằng phương thức nào?",
                computerLabel: "🖥️ WebView2 (Tự động)",
                phoneLabel: "📱 Thủ công (Email/Phone)"
            ))
            {
                return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedDevice : DeviceType.None;
            }
        }

        /// <summary>
        /// Hiện dialog chọn phương thức đổi password
        /// </summary>
        public static DeviceType ShowChangePasswordDialog(IWin32Window owner = null)
        {
            using (var dlg = new DeviceSelectionDialog(
                title: "Đổi Mật Khẩu",
                message: "Bạn muốn đổi mật khẩu bằng phương thức nào?",
                computerLabel: "🖥️ WebView2 (Session)",
                phoneLabel: "📱 Thủ công (Email/Phone)"
            ))
            {
                return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedDevice : DeviceType.None;
            }
        }

        /// <summary>
        /// Hiện dialog chọn phương thức verify account
        /// </summary>
        public static DeviceType ShowVerifyAccountDialog(IWin32Window owner = null)
        {
            using (var dlg = new DeviceSelectionDialog(
                title: "Xác Minh Tài Khoản",
                message: "Bạn muốn xác minh tài khoản bằng phương thức nào?",
                computerLabel: "🖥️ Qua WebView2",
                phoneLabel: "📱 Qua Email/Phone"
            ))
            {
                return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedDevice : DeviceType.None;
            }
        }

        /// <summary>
        /// Hiện dialog chọn phương thức get cookie
        /// </summary>
        public static DeviceType ShowGetCookieDialog(string username, IWin32Window owner = null)
        {
            using (var dlg = new DeviceSelectionDialog(
                title: "Get Cookie",
                message: $"Lấy cookie cho @{username} bằng phương thức nào?",
                computerLabel: "🖥️ WebView2 (Tự động)",
                phoneLabel: "📱 Login Phone (API)"
            ))
            {
                return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedDevice : DeviceType.None;
            }
        }

        /// <summary>
        /// Hiện dialog chung với custom title/message
        /// </summary>
        public static DeviceType ShowCustomDialog(
            string title,
            string message,
            string computerLabel = "🖥️ Máy tính",
            string phoneLabel = "📱 Điện thoại",
            IWin32Window owner = null)
        {
            using (var dlg = new DeviceSelectionDialog(title, message, computerLabel, phoneLabel))
            {
                return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedDevice : DeviceType.None;
            }
        }
    }
}
