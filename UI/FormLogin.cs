using System;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormLogin : FormTraducibleBase
    {
        private readonly IdiomaBLL gestorIdioma = new IdiomaBLL();
        private readonly UsuarioBLL gestorUsuario = new UsuarioBLL();

        public FormLogin()
        {
            InitializeComponent();
            Load += FormLogin_Load;
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

        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                gestorIdioma.InicializarAplicacion();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Inicializacion de idiomas");
                Close();
            }
        }
    }
}
