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
            Load += FormLogin_Load;
            FormClosed += FormLogin_FormClosed;
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbNickname.Text) || string.IsNullOrWhiteSpace(txtbPassword.Text))
            {
                UiDialogHelper.MostrarWarning("FormLogin.Warning.CompleteFields");
                return;
            }

            try
            {
                Usuario usuario = gestorUsuario.Login(txtbNickname.Text, txtbPassword.Text);

                if (usuario == null)
                {
                    UiDialogHelper.MostrarError("FormLogin.Error.InvalidCredentials");
                    return;
                }

                txtbNickname.Clear();
                txtbPassword.Clear();

                FormGestionUsuarios formGestion = new FormGestionUsuarios();
                formGestion.Show();
                Hide();

                formGestion.FormClosed += (s, args) =>
                {
                    gestorUsuario.CerrarSesionActual();
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

        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                gestorIdioma.InicializarAplicacion();
                IdiomaService.GetInstance().RegistrarObservador(observadorIdioma);
                Actualizar(IdiomaService.GetInstance().IdiomaActual);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Inicializacion de idiomas");
                Close();
            }
        }

        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(observadorIdioma);
        }
    }
}
