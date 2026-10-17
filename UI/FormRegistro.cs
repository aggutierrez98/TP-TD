using System;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormRegistro : FormTraducibleBase
    {
        private readonly UsuarioBLL gestorJugador = CompositionRoot.Usuario;

        public FormRegistro()
        {
            InitializeComponent();

            // El bloque crece con la ventana hasta el doble de su ancho de diseño y despues queda centrado.
            LayoutHelper.CentrarConAnchoMaximo(pnlBloqueRegistro, pnlBloqueRegistro.Width * 2);
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
                

                BE.Usuario usuario = new BE.Usuario()
                {
                    Username = txtbNickname.Text
                };

                bool success = gestorJugador.Registrar(usuario, txtbPassword.Text);

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
