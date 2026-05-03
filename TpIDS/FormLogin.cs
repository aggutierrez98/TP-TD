using System;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;
using TpIDS.Properties;

namespace TpIDS
{
    public partial class FormLogin : Form
    {
        private bool showPassword = false;
        private readonly UsuarioBLL gestorUsuario = new UsuarioBLL();

        public FormLogin()
        {
            InitializeComponent();
            pbPassword.Image = Resources.image_show;
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbNickname.Text) || string.IsNullOrWhiteSpace(txtbPassword.Text))
            {
                MessageBox.Show("Debe completar todos los campos", "Error");
                return;
            }

            try
            {
                Usuario usuario = gestorUsuario.Login(txtbNickname.Text, txtbPassword.Text);

                if (usuario == null)
                {
                    MessageBox.Show("Credenciales incorrectas", "Error");
                    return;
                }

                txtbNickname.Clear();
                txtbPassword.Clear();

                FormGestionUsuarios formGestion = new FormGestionUsuarios();
                formGestion.Show();
                Hide();

                formGestion.FormClosed += (s, args) =>
                {
                    Sesion.GetInstance().CerrarSesion();
                    Close();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el login: " + ex.Message, "Error");
            }
        }

        private void btnRegistro_Click(object sender, EventArgs e)
        {
            new FormRegistro().ShowDialog();
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
