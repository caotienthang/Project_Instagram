using System;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    public partial class PasswordMethodDialog : Form
    {
        public enum ChangeMethod { None, Computer, Phone }

        public ChangeMethod SelectedMethod { get; private set; } = ChangeMethod.None;

        public PasswordMethodDialog()
        {
            InitializeComponent();
        }

        private void btnComputer_Click(object sender, EventArgs e)
        {
            SelectedMethod = ChangeMethod.Computer;
            DialogResult   = DialogResult.OK;
            Close();
        }

        private void btnPhone_Click(object sender, EventArgs e)
        {
            SelectedMethod = ChangeMethod.Phone;
            DialogResult   = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedMethod = ChangeMethod.None;
            DialogResult   = DialogResult.Cancel;
            Close();
        }
    }
}
