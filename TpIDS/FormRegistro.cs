using System;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;
using SERVICIOS;
using TpIDS.Properties;

namespace TpIDS
{
    public partial class FormRegistro : Form, IObservador
    {
        private bool showPassword = false;
        private readonly UsuarioBLL gestorJugador = new UsuarioBLL();

        public FormRegistro()
        {
            InitializeComponent();
            pbPassword.Image = Resources.image_show;
            ConfigurarTags();
            Load += FormRegistro_Load;
            FormClosed += FormRegistro_FormClosed;
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

        public void Actualizar(IIdioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
        }

        private void ConfigurarTags()
        {
            Tag = "FormRegistro.Title";
            lblNickname.Tag = "FormRegistro.NicknameLabel";
            lblPassword.Tag = "FormRegistro.PasswordLabel";
            btnRegistro.Tag = "FormRegistro.RegisterButton";
        }

        private void FormRegistro_Load(object sender, EventArgs e)
        {
            Sesion.GetInstance().RegistrarObservador(this);
            Actualizar(Sesion.GetInstance().IdiomaActual);
        }

        private void FormRegistro_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sesion.GetInstance().DesregistrarObservador(this);
        }
    }
}
