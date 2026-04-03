using System;
using System.Windows.Forms;

namespace WindowsFormsApp1.Views
{
    public partial class AddAccountDialog : Form
    {
        public enum LoginMethod { None, Computer, Phone }

        public LoginMethod SelectedMethod { get; private set; } = LoginMethod.None;

        public AddAccountDialog()
        {
            InitializeComponent();
        }

        private void btnComputer_Click(object sender, EventArgs e)
        {
            SelectedMethod = LoginMethod.Computer;
            DialogResult   = DialogResult.OK;
            Close();
        }

        private void btnPhone_Click(object sender, EventArgs e)
        {
            SelectedMethod = LoginMethod.Phone;
            DialogResult   = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedMethod = LoginMethod.None;
            DialogResult   = DialogResult.Cancel;
            Close();
        }
    }
}
