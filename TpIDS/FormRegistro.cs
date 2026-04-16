using System;
using System.Drawing;
using System.Windows.Forms;
using BLL;

namespace TpIDS
{
    public partial class FormRegistro : Form
    {
        private bool showPassword = false;
        private UsuarioBLL gestorJugador = new UsuarioBLL();

        public FormRegistro()
        {
            InitializeComponent();
            try { pictureBoxPassword.Image = Image.FromFile(@"Images\show.png"); } catch { }
        }

        private void buttonRegistro_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxNickname.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Debe completar todos los campos", "Error");
                return;
            }

            try
            {
                bool success = gestorJugador.Registrar(textBoxNickname.Text, textBoxPassword.Text);
                if (success)
                {
                    MessageBox.Show("Usuario registrado con éxito", "Ahora usa las credenciales para ingresar...");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de registro: " + ex.Message, "Error");
            }
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
