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
        private readonly IdiomaBLL idiomaBLL = CompositionRoot.Idioma;
        private readonly ISesion _sesion;
        private readonly IObservado _servicioIdioma;

        public FormConfiguracionUsuario()
            : this(CompositionRoot.Sesion, CompositionRoot.ServicioIdioma)
        {
        }

        public FormConfiguracionUsuario(ISesion sesion, IObservado servicioIdioma)
        {
            _sesion = sesion;
            _servicioIdioma = servicioIdioma;
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
            int? usuarioId = _sesion.GetUsuarioActualId();
            if (!usuarioId.HasValue)
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
                bool guardado = idiomaBLL.GuardarPreferencia(usuarioId.Value, idioma.Id);
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
            string username = _sesion.GetUsuarioActualNombre();
            lblUsuarioActualNombre.Text = string.IsNullOrWhiteSpace(username) ? "-" : username;
        }

        private void CargarIdiomas()
        {
            List<Idioma> idiomas = idiomaBLL.ObtenerIdiomas();
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
            cmbIdiomas.DataSource = idiomas;

            IIdioma idiomaActual = _servicioIdioma.IdiomaActual;
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
