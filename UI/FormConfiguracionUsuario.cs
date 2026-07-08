using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormConfiguracionUsuario : FormTraducibleBase
    {
        private readonly IdiomaBLL idiomaBLL = new IdiomaBLL();

        public FormConfiguracionUsuario()
        {
            InitializeComponent();
            Load += FormConfiguracionUsuario_Load;
        }

        private void FormConfiguracionUsuario_Load(object sender, EventArgs e)
        {
            CargarUsuarioActual();
            CargarIdiomas();
        }

        private void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            IUsuarioAutenticado usuarioActual = Sesion.GetInstance().GetUsuarioActual();
            if (usuarioActual == null)
            {
                UiDialogHelper.MostrarError("FormUserSettings.Error.NoActiveSession");
                Close();
                return;
            }

            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormUserSettings.Warning.SelectLanguage");
                return;
            }

            try
            {
                bool guardado = idiomaBLL.GuardarPreferencia(usuarioActual, idioma);
                if (guardado)
                {
                    UiDialogHelper.MostrarInfo("FormUserSettings.Success.SettingsSaved");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de configuracion de usuario");
            }
        }

        // El username es estado de sesion, no de idioma: el prefijo lo traduce el helper via Tag
        // y aca solo seteamos el dato.
        private void CargarUsuarioActual()
        {
            string username = Sesion.GetInstance().GetUsuarioActualNombre();
            lblUsuarioActualNombre.Text = string.IsNullOrWhiteSpace(username) ? "-" : username;
        }

        private void CargarIdiomas()
        {
            List<Idioma> idiomas = idiomaBLL.ObtenerIdiomas();
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
            cmbIdiomas.DataSource = idiomas;

            Idioma idiomaActual = IdiomaService.GetInstance().IdiomaActual;
            if (idiomaActual != null)
            {
                Idioma idiomaSeleccionado = idiomas.FirstOrDefault(item => item.Id == idiomaActual.Id);
                if (idiomaSeleccionado != null)
                {
                    cmbIdiomas.SelectedItem = idiomaSeleccionado;
                }
            }
        }
    }
}
