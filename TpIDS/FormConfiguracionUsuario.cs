using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormConfiguracionUsuario : Form, IObservador
    {
        private readonly IdiomaBLL idiomaBLL = new IdiomaBLL();

        public FormConfiguracionUsuario()
        {
            InitializeComponent();
            ConfigurarTags();
            Load += FormConfiguracionUsuario_Load;
            FormClosed += FormConfiguracionUsuario_FormClosed;
        }

        private void FormConfiguracionUsuario_Load(object sender, EventArgs e)
        {
            Sesion.GetInstance().RegistrarObservador(this);
            CargarUsuarioActual();
            CargarIdiomas();
            Actualizar(Sesion.GetInstance().IdiomaActual);
        }

        private void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            Usuario usuario = Sesion.GetInstance().GetUsuarioActualUsuario();
            if (usuario == null)
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
                bool guardado = idiomaBLL.GuardarPreferencia(usuario.Id, idioma.Id);
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

        public void Actualizar(IIdioma idioma)
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
            Usuario usuario = Sesion.GetInstance().GetUsuarioActualUsuario();
            string prefijo = ObtenerTextoActual("FormConfiguracionUsuario.CurrentUserPrefix");
            lblUsuarioActual.Text = string.Format("{0} {1}", prefijo, usuario == null ? "-" : usuario.Username);
        }

        private void CargarIdiomas()
        {
            List<Idioma> idiomas = idiomaBLL.ObtenerIdiomas();
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
            cmbIdiomas.DataSource = idiomas;

            IIdioma idiomaActual = Sesion.GetInstance().IdiomaActual;
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
            IIdioma idioma = Sesion.GetInstance().IdiomaActual;
            return idioma == null ? key : idioma.ObtenerTexto(key);
        }

        private void FormConfiguracionUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sesion.GetInstance().DesregistrarObservador(this);
        }
    }
}
