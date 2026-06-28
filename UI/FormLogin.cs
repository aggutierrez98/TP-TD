using System;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    public partial class FormLogin : FormTraducibleBase
    {
        private readonly BitacoraBLL gestorBitacora = new BitacoraBLL();
        private readonly IdiomaBLL gestorIdioma = new IdiomaBLL();
        private readonly UsuarioBLL gestorUsuario = new UsuarioBLL();

        public FormLogin()
        {
            InitializeComponent();
            Load += FormLogin_Load;
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbNickname.Text.Trim()) || string.IsNullOrWhiteSpace(txtbPassword.Text.Trim()))
            {
                gestorBitacora.RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.LoginEmptyFields", "UsuarioBLL");
                UiDialogHelper.MostrarWarning("FormLogin.Warning.CompleteFields");
                return;
            }

            try
            {
                Usuario usuario = new Usuario()
                {
                    Username = txtbNickname.Text
                };

                Usuario usuarioEncontrado = gestorUsuario.Login(usuario, txtbPassword.Text);

                if (usuarioEncontrado == null)
                {
                    UiDialogHelper.MostrarError("FormLogin.Error.InvalidCredentials");
                    return;
                }

                txtbNickname.Clear();
                txtbPassword.Clear();

                FormGestionUsuarios formGestion = new FormGestionUsuarios();
                formGestion.Show();

                formGestion.FormClosed += (s, args) =>
                {
                    gestorUsuario.CerrarSesionActual();
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
