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
        private readonly IdiomaBLL gestorIdioma = new IdiomaBLL();
        private readonly UsuarioBLL gestorUsuario = new UsuarioBLL();
        private readonly Action<Idioma> observadorIdioma;

        public FormLogin()
        {
            InitializeComponent();
            observadorIdioma = Actualizar;
            pbPassword.Image = Resources.image_show;
            ConfigurarTags();
            Load += FormLogin_Load;
            FormClosed += FormLogin_FormClosed;
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
                UiExceptionHandler.Mostrar(ex, "Login");
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
                txtbPassword.PasswordChar = '*';
                pbPassword.Image = Resources.image_show;
            }
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
        }

        private void ConfigurarTags()
        {
            Tag = "FormLogin.Title";
            lblNickname.Tag = "FormLogin.NicknameLabel";
            lblPassword.Tag = "FormLogin.PasswordLabel";
            btnIngreso.Tag = "FormLogin.LoginButton";
            lblRegistro.Tag = "FormLogin.RegisterPrompt";
            btnRegistro.Tag = "FormLogin.RegisterButton";
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                gestorIdioma.InicializarAplicacion();
                Sesion.GetInstance().RegistrarObservador(observadorIdioma);
                Actualizar(Sesion.GetInstance().IdiomaActual);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Inicializacion de idiomas");
                Close();
            }
        }

        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sesion.GetInstance().DesregistrarObservador(observadorIdioma);
        }
    }
}
