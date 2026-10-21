using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using BE;
using Krypton.Toolkit;
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
        private readonly PerfilInversorBLL perfilBLL = CompositionRoot.PerfilInversor;
        private readonly KryptonLabel lblPerfilInversor = new KryptonLabel();
        private readonly KryptonLabel lblFechaPerfil = new KryptonLabel();
        private readonly KryptonButton btnCuestionarioPerfil = new KryptonButton();
        private PerfilInversor _perfilActual;
        private bool _perfilCargado;

        public FormConfiguracionUsuario()
            : this(CompositionRoot.Sesion, CompositionRoot.ServicioIdioma)
        {
        }

        public FormConfiguracionUsuario(ISesion sesion, IObservado servicioIdioma)
        {
            _sesion = sesion;
            _servicioIdioma = servicioIdioma;
            InitializeComponent();
            ClientSize = new System.Drawing.Size(620, 410);
            btnGuardarConfiguracion.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnGuardarConfiguracion.Location = new System.Drawing.Point(25, 165);
            lblPerfilInversor.Name = nameof(lblPerfilInversor);
            lblPerfilInversor.SetBounds(25, 235, 570, 28);
            lblPerfilInversor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFechaPerfil.Name = nameof(lblFechaPerfil);
            lblFechaPerfil.SetBounds(25, 275, 570, 28);
            lblFechaPerfil.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnCuestionarioPerfil.Name = nameof(btnCuestionarioPerfil);
            btnCuestionarioPerfil.SetBounds(25, 325, 430, 38);
            btnCuestionarioPerfil.Text = UiDialogHelper.Traducir("FormUserSettings.Button.CompleteProfile");
            btnCuestionarioPerfil.ButtonStyle = ButtonStyle.Custom1;
            btnCuestionarioPerfil.Enabled = false;
            btnCuestionarioPerfil.Click += btnCuestionarioPerfil_Click;
            Controls.AddRange(new Control[] { lblPerfilInversor, lblFechaPerfil, btnCuestionarioPerfil });
            // InitializeComponent ya creo el panel interno de KryptonForm: los
            // controles agregados despues deben quedar por delante de ese panel.
            lblPerfilInversor.BringToFront();
            lblFechaPerfil.BringToFront();
            btnCuestionarioPerfil.BringToFront();
            Load += FormConfiguracionUsuario_Load;
        }

        private async void FormConfiguracionUsuario_Load(object sender, EventArgs e)
        {
            CargarUsuarioActual();
            CargarIdiomas();
            await CargarPerfilAsync();
        }

        private async Task CargarPerfilAsync()
        {
            btnCuestionarioPerfil.Enabled = false;
            try
            {
                var perfil = await Task.Run(() => perfilBLL.ObtenerMiPerfil());
                if (IsDisposed) return;
                _perfilActual = perfil;
                _perfilCargado = true;
                MostrarPerfil();
            }
            catch (Exception ex) { if (!IsDisposed) UiExceptionHandler.Mostrar(ex, nameof(FormConfiguracionUsuario)); }
            finally { if (!IsDisposed) btnCuestionarioPerfil.Enabled = true; }
        }

        private void MostrarPerfil()
        {
            if (!_perfilCargado) return;
            lblPerfilInversor.Text = UiDialogHelper.Traducir("FormUserSettings.Label.InvestorProfile") + ": "
                + UiDialogHelper.Traducir(_perfilActual == null ? "FormUserSettings.Value.NoInvestorProfile" : "FormInvestorProfile.Level." + _perfilActual.NivelRiesgo);
            lblFechaPerfil.Text = _perfilActual == null ? string.Empty : UiDialogHelper.Traducir("FormUserSettings.Value.ProfileDate", _perfilActual.FechaEvaluacionUtc.ToLocalTime().ToString("g"));
            btnCuestionarioPerfil.Text = UiDialogHelper.Traducir(_perfilActual == null ? "FormUserSettings.Button.CompleteProfile" : "FormUserSettings.Button.RepeatProfile");
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            MostrarPerfil();
        }

        private async void btnCuestionarioPerfil_Click(object sender, EventArgs e)
        {
            if (!_perfilCargado) { await CargarPerfilAsync(); return; }
            using (var cuestionario = new FormPerfilInversor())
                if (cuestionario.ShowDialog(this) == DialogResult.OK) await CargarPerfilAsync();
        }

        private void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            int? usuarioId = _sesion.GetUsuarioActual()?.Id;
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
                bool guardado = idiomaBLL.GuardarPreferencia(idioma);
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
