using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormConfiguracionUsuario : Form
    {
        private readonly IdiomaBLL idiomaBLL = new IdiomaBLL();
        private readonly Action<Idioma> observadorIdioma;

        public FormConfiguracionUsuario()
        {
            InitializeComponent();
            observadorIdioma = Actualizar;
            Load += FormConfiguracionUsuario_Load;
            FormClosed += FormConfiguracionUsuario_FormClosed;
        }

        private void FormConfiguracionUsuario_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(observadorIdioma);
            CargarUsuarioActual();
            CargarIdiomas();
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
        }

        private void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            int? usuarioId = Sesion.GetInstance().GetUsuarioActualId();
            if (!usuarioId.HasValue)
            {
                UiDialogHelper.MostrarError("FormConfiguracionUsuario.Error.NoActiveSession");
                Close();
                return;
            }

            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormConfiguracionUsuario.Warning.SelectLanguage");
                return;
            }

            try
            {
                bool guardado = idiomaBLL.GuardarPreferencia(usuarioId.Value, idioma.Id);
                if (guardado)
                {
                    UiDialogHelper.MostrarInfo("FormConfiguracionUsuario.Success.SettingsSaved");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de configuracion de usuario");
            }
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            CargarUsuarioActual();
        }

        private void CargarUsuarioActual()
        {
            string prefijo = ObtenerTextoActual("FormConfiguracionUsuario.CurrentUserPrefix");
            string username = Sesion.GetInstance().GetUsuarioActualNombre();
            lblUsuarioActual.Text = string.Format("{0} {1}", prefijo, string.IsNullOrWhiteSpace(username) ? "-" : username);
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

        private string ObtenerTextoActual(string key)
        {
            Idioma idioma = IdiomaService.GetInstance().IdiomaActual;
            return idioma == null ? key : idioma.ObtenerTraduccion(key);
        }

        private void FormConfiguracionUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(observadorIdioma);
        }
    }
}
