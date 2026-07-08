using System;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormLogin : FormTraducibleBase
    {
        private readonly UsuarioBLL gestorUsuario = new UsuarioBLL();

        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbNickname.Text) || string.IsNullOrWhiteSpace(txtbPassword.Text))
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.LoginEmptyFields", "UsuarioBLL");
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
    }
}
