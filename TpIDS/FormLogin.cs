using System;
using System.Drawing;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormLogin : Form
    {
        private bool showPassword = false;
        private UsuarioBLL gestorUsuario = new UsuarioBLL();

        public FormLogin()
        {
            InitializeComponent();
            try { pictureBoxPassword.Image = Image.FromFile(@"Images\show.png"); } catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxNickname.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Debe completar todos los campos", "Error");
                return;
            }

            try
            {
                Usuario usuario = gestorUsuario.Login(textBoxNickname.Text, textBoxPassword.Text);
                Sesion.GetInstance().IniciarSesion(usuario);

                if (usuario == null)
                {
                    MessageBox.Show("Credenciales incorrectas", "Error");
                    return;
                }

                textBoxNickname.Clear();
                textBoxPassword.Clear();

                // Navegar al formulario de gestión de usuarios
                FormGestionUsuarios formGestion = new FormGestionUsuarios();
                formGestion.Show();
                this.Hide();

                formGestion.FormClosed += (s, args) =>
                {
                    Sesion.GetInstance().CerrarSesion();
                    this.Close();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el login: " + ex.Message, "Error");
            }
        }

        private void buttonRegistro_Click(object sender, EventArgs e)
        {
            new FormRegistro().ShowDialog();
        }

        private void pictureBoxPassword_Click(object sender, EventArgs e)
        {
            showPassword = !showPassword;
            if (showPassword)
            {
                textBoxPassword.PasswordChar = '\0';
                try { pictureBoxPassword.Image = Image.FromFile(@"Images\hide.png"); } catch { }
            }
            else
            {
                textBoxPassword.PasswordChar = '●';
                try { pictureBoxPassword.Image = Image.FromFile(@"Images\show.png"); } catch { }
            }
        }
    }
}
