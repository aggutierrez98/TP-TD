using System;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormRegistro : FormTraducibleBase
    {
        private readonly UsuarioBLL gestorJugador = new UsuarioBLL();

        public FormRegistro()
        {
            InitializeComponent();
        }

        private void btnRegistro_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbNickname.Text) || string.IsNullOrWhiteSpace(txtbPassword.Text))
            {
                UiDialogHelper.MostrarWarning("FormRegistration.Warning.CompleteFields");
                return;
            }

            try
            {
                bool success = gestorJugador.Registrar(txtbNickname.Text, txtbPassword.Text);

                if (success)
                {
                    UiDialogHelper.MostrarInfoConTitulo(
                        "FormRegistration.Success.UserRegisteredMessage",
                        "FormRegistration.Success.UserRegisteredTitle");
                    Close();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Registro de usuario");
            }
        }

    }
}
