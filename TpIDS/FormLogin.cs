using System;
using System.Drawing;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    public partial class FormLogin : Form
    {
        private bool showPassword = false;
        private UsuarioBLL gestorJugador = new UsuarioBLL();
        private Usuario usuario;

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
                usuario = gestorJugador.Login(textBoxNickname.Text, textBoxPassword.Text);

                if (usuario == null)
                {
                    MessageBox.Show("Credenciales incorrectas", "Error");
                    return;
                }

                textBoxNickname.Clear();
                textBoxPassword.Clear();
                
                MessageBox.Show($"Bienvenido, {usuario.Username}!", "Éxito");
                // new FormMenu(usuario).ShowDialog(); // Uncomment when Menu is ready
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
