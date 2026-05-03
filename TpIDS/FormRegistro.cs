using System;
using System.Windows.Forms;
using BLL;
using TpIDS.Properties;

namespace TpIDS
{
    public partial class FormRegistro : Form
    {
        private bool showPassword = false;
        private readonly UsuarioBLL gestorJugador = new UsuarioBLL();

        public FormRegistro()
        {
            InitializeComponent();
            pbPassword.Image = Resources.image_show;
        }

        private void btnRegistro_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbNickname.Text) || string.IsNullOrWhiteSpace(txtbPassword.Text))
            {
                MessageBox.Show("Debe completar todos los campos", "Error");
                return;
            }

            try
            {
                bool success = gestorJugador.Registrar(txtbNickname.Text, txtbPassword.Text);

                if (success)
                {
                    MessageBox.Show("Usuario registrado con exito", "Ahora usa las credenciales para ingresar...");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de registro: " + ex.Message, "Error");
            }
        }

        private void pbPassword_Click(object sender, EventArgs e)
        {
            showPassword = !showPassword;
            if (showPassword)
            {
                txtbPassword.PasswordChar = '\0';
                pbPassword.Image = Resources.image_hide;
            }
            else
            {
                txtbPassword.PasswordChar = '●';
                pbPassword.Image = Resources.image_show;
            }
        }
    }
}
