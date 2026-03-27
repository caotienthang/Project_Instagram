using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Views
{
    public partial class PostPanel : UserControl
    {
        private AccountInfo _account;
        private List<string> _imagePaths = new List<string>();

        public event Action OnClose;

        public PostPanel(AccountInfo account)
        {
            InitializeComponent();
            _account = account;
        }

        private void btnSelectImages_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Images|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _imagePaths = ofd.FileNames.ToList();

                flowImages.Controls.Clear();

                foreach (var path in _imagePaths)
                {
                    var pic = new PictureBox()
                    {
                        Width = 80,
                        Height = 80,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(path)
                    };

                    flowImages.Controls.Add(pic);
                }
            }
        }

        private async void btnPost_Click(object sender, EventArgs e)
        {
            if (_imagePaths == null || _imagePaths.Count == 0)
            {
                MessageBox.Show("Chọn ảnh trước");
                return;
            }

            try
            {
                var session = InstagramSessionRepository.GetByAccountId(_account.Id);
                if (session == null)
                {
                    MessageBox.Show("Không tìm thấy session Instagram");
                    return;
                }

                MessageBox.Show("Post Thành Công");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                OnClose?.Invoke();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            OnClose?.Invoke();
        }
    }
}
