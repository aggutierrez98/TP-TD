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
            ConfigurarTags();
            Load += FormConfiguracionUsuario_Load;
            FormClosed += FormConfiguracionUsuario_FormClosed;
        }

        private void FormConfiguracionUsuario_Load(object sender, EventArgs e)
        {
            Sesion.GetInstance().RegistrarObservador(observadorIdioma);
            CargarUsuarioActual();
            CargarIdiomas();
            Actualizar(Sesion.GetInstance().IdiomaActual);
        }

        private void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            int? usuarioId = Sesion.GetInstance().GetUsuarioActualId();
            if (!usuarioId.HasValue)
            {
                MessageBox.Show("No hay una sesion activa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                MessageBox.Show("Debe seleccionar un idioma.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool guardado = idiomaBLL.GuardarPreferencia(usuarioId.Value, idioma.Id);
                if (guardado)
                {
                    MessageBox.Show("La configuracion se guardo correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            CargarUsuarioActual();
        }

        private void ConfigurarTags()
        {
            Tag = "FormConfiguracionUsuario.Title";
            lblTituloConfiguracion.Tag = "FormConfiguracionUsuario.TitleLabel";
            lblIdiomaSesion.Tag = "FormConfiguracionUsuario.LanguageLabel";
            btnGuardarConfiguracion.Tag = "FormConfiguracionUsuario.SaveButton";
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

            Idioma idiomaActual = Sesion.GetInstance().IdiomaActual;
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
            Idioma idioma = Sesion.GetInstance().IdiomaActual;
            return idioma == null ? key : idioma.ObtenerTexto(key);
        }

        private void FormConfiguracionUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sesion.GetInstance().DesregistrarObservador(observadorIdioma);
        }
    }
}
