using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class DashBoard : Form
    {
        private List<AccountInfo> _accounts;
        public DashBoard()
        {
            InitializeComponent();
            SetupGrid();
            LoadData();
        }

        // ================= SETUP GRID =================
        private void SetupGrid()
        {
            grid.Columns.Clear();

            // checkbox
            grid.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                Width = 30
            });

            // ID (ẩn)
            grid.Columns.Add("Id", "Id");
            grid.Columns["Id"].Visible = false;

            grid.Columns.Add("Username", "Username");
            grid.Columns.Add("FullName", "Full Name");
            grid.Columns.Add("Email", "Email");
            grid.Columns.Add("Phone", "Phone");

            // Avatar (image)
            var avatarCol = new DataGridViewImageColumn();
            avatarCol.Name = "Avatar";
            avatarCol.HeaderText = "Avatar";
            avatarCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            avatarCol.Width = 60;
            grid.Columns.Add(avatarCol);

            grid.Columns.Add("Birthday", "Birthday");
            grid.Columns.Add("Status", "Status");

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowTemplate.Height = 50;
            grid.AllowUserToAddRows = false;
        }

        // ================= LOAD DATA =================
        private void LoadData()
        {
            _accounts = AccountRepository.GetAll();

            grid.Rows.Clear();

            foreach (var acc in _accounts)
            {
                Image avatarImg = LoadAvatar(acc.Avatar);

                int rowIndex = grid.Rows.Add(
                    false,
                    acc.Id,
                    acc.Username,
                    acc.FullName,
                    acc.Email,
                    acc.Phone,
                    avatarImg,
                    acc.Birthday,
                    acc.Status
                );

                SetStatusColor(rowIndex, acc.Status);
            }
        }

        // ================= LOAD AVATAR =================
        private Image LoadAvatar(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return null;

                // ✅ tránh lock file
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    return Image.FromStream(fs);
                }
            }
            catch
            {
                return null;
            }
        }

        // ================= COLOR STATUS =================
        private void SetStatusColor(int rowIndex, string status)
        {
            var row = grid.Rows[rowIndex];

            switch ((status ?? "").ToLower())
            {
                case "live":
                    row.Cells["Status"].Style.BackColor = Color.LightGreen;
                    break;

                case "running":
                    row.Cells["Status"].Style.BackColor = Color.LightYellow;
                    break;

                case "error":
                    row.Cells["Status"].Style.BackColor = Color.IndianRed;
                    row.Cells["Status"].Style.ForeColor = Color.White;
                    break;

                default:
                    row.Cells["Status"].Style.BackColor = Color.LightGray;
                    break;
            }
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["Id"].Value);

            var account = _accounts.FirstOrDefault(x => x.Id == id);

            if (account == null) return;

            var form = new AccountDetailForm(account);
            form.Show();
        }
        private void btnGetCookie_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Kiểm tra chọn dòng
                if (grid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn 1 account!");
                    return;
                }

                var row = grid.SelectedRows[0];

                int accountId = Convert.ToInt32(row.Cells["Id"].Value);

                // 2. Tìm account trong list
                var account = _accounts.FirstOrDefault(x => x.Id == accountId);

                if (account == null)
                {
                    MessageBox.Show("Không tìm thấy account!");
                    return;
                }

                logBox.AppendText($"Đang lấy cookie cho: {account.Username}\n");

                // 3. Mở form lấy cookie (truyền account vào)
                using (var form = new AccountsCenterForm(account))
                {
                    var result = form.ShowDialog();
                }

                // 4. Reload lại grid sau khi update session
                LoadData();

                logBox.AppendText($"Đã cập nhật cookie cho: {account.Username}\n");
            }
            catch (Exception ex)
            {
                logBox.AppendText("Lỗi: " + ex.Message + "\n");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AccountsCenterForm())
            {
                var result = form.ShowDialog();
                LoadData();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
