using System;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    public partial class FormLogin : FormTraducibleBase
    {
        private readonly BitacoraBLL gestorBitacora = CompositionRoot.GestorBitacora;
        private readonly UsuarioBLL gestorUsuario = CompositionRoot.Usuario;

        public FormLogin()
        {
            InitializeComponent();
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

                Hide();

                FormPrincipal formPrincipal = new FormPrincipal();
                formPrincipal.Show();

                formPrincipal.FormClosed += (s, args) =>
                {
                    gestorUsuario.CerrarSesionActual();
                    Show();
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

    }
}
