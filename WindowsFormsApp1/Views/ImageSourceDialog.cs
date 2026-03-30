using System;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    public partial class ImageSourceDialog : Form
    {
        public enum SourceType
        {
            None,
            Folder,
            File
        }

        public SourceType SelectedSource { get; private set; }

        public ImageSourceDialog()
        {
            InitializeComponent();
            SelectedSource = SourceType.None;
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            SelectedSource = SourceType.Folder;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            SelectedSource = SourceType.File;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedSource = SourceType.None;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
