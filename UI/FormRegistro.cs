using System;
using System.Windows.Forms;
using BLL;
using SERVICIOS;
using TpIDS.Properties;

namespace TpIDS
{
    public partial class FormRegistro : Form
    {
        private bool showPassword = false;
        private readonly UsuarioBLL gestorJugador = new UsuarioBLL();
        private readonly Action<Idioma> observadorIdioma;

        public FormRegistro()
        {
            InitializeComponent();
            observadorIdioma = Actualizar;
            pbPassword.Image = Resources.image_show;
            Load += FormRegistro_Load;
            FormClosed += FormRegistro_FormClosed;
        }

        private void btnRegistro_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbNickname.Text) || string.IsNullOrWhiteSpace(txtbPassword.Text))
            {
                UiDialogHelper.MostrarWarning("FormRegistro.Warning.CompleteFields");
                return;
            }

            try
            {
                bool success = gestorJugador.Registrar(txtbNickname.Text, txtbPassword.Text);

                if (success)
                {
                    UiDialogHelper.MostrarInfoConTitulo(
                        "FormRegistro.Success.UserRegisteredMessage",
                        "FormRegistro.Success.UserRegisteredTitle");
                    Close();
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Registro de usuario");
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
                txtbPassword.PasswordChar = '*';
                pbPassword.Image = Resources.image_show;
            }
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
        }

        private void FormRegistro_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(observadorIdioma);
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
        }

        private void FormRegistro_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(observadorIdioma);
        }
    }
}
