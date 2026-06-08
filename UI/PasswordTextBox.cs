using System;
using System.Windows.Forms;
using TpIDS.Properties;

namespace TpIDS
{
    public partial class PasswordTextBox : UserControl
    {
        private bool _mostrarContrasena;

        public PasswordTextBox()
        {
            InitializeComponent();
            pbTogglePassword.Image = Resources.image_show;
            txtbInput.UseSystemPasswordChar = true;
            txtbInput.TextChanged += txtbInput_TextChanged;
            Enter += PasswordTextBox_Enter;
        }

        public override string Text
        {
            get { return txtbInput.Text; }
            set { txtbInput.Text = value; }
        }

        public void Clear()
        {
            txtbInput.Clear();
        }

        private void pbTogglePassword_Click(object sender, EventArgs e)
        {
            _mostrarContrasena = !_mostrarContrasena;
            txtbInput.UseSystemPasswordChar = !_mostrarContrasena;
            pbTogglePassword.Image = _mostrarContrasena
                ? Resources.image_hide
                : Resources.image_show;
        }

        private void txtbInput_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(e);
        }

        private void PasswordTextBox_Enter(object sender, EventArgs e)
        {
            txtbInput.Focus();
        }
    }
}
