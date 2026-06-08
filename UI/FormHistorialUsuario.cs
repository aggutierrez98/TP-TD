using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormHistorialUsuario : Form
    {
        private readonly UsuarioBLL _usuarioBLL;
        private readonly Action<Idioma> _observadorIdioma;
        private readonly int _usuarioId;
        private readonly string _username;

        private static readonly Dictionary<string, string> AccionKeys = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Alta",                         "FormHistorialUsuario.Value.Action.Alta" },
            { "Edicion",                      "FormHistorialUsuario.Value.Action.Edicion" },
            { "BajaLogica",                   "FormHistorialUsuario.Value.Action.BajaLogica" },
            { "Reactivacion",                 "FormHistorialUsuario.Value.Action.Reactivacion" },
            { "Restauracion",                 "FormHistorialUsuario.Value.Action.Restauracion" },
            { "ActualizacionIdiomaPreferido", "FormHistorialUsuario.Value.Action.ActualizacionIdiomaPreferido" }
        };

        private static readonly Dictionary<string, string> ObservacionesKeys = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Alta inicial del usuario.",                              "FormHistorialUsuario.Value.Obs.AltaInicial" },
            { "Desactivacion del usuario desde gestion.",              "FormHistorialUsuario.Value.Obs.Desactivacion" },
            { "Reactivacion del usuario desde gestion.",               "FormHistorialUsuario.Value.Obs.Reactivacion" },
            { "Actualizacion del password del usuario.",               "FormHistorialUsuario.Value.Obs.ActualizacionPassword" },
            { "Actualizacion del nombre de usuario.",                  "FormHistorialUsuario.Value.Obs.ActualizacionUsername" },
            { "Actualizacion del usuario.",                            "FormHistorialUsuario.Value.Obs.ActualizacionUsuario" },
            { "Restauracion desde el historial.",                      "FormHistorialUsuario.Value.Obs.Restauracion" },
            { "Actualizacion de la preferencia de idioma del usuario.", "FormHistorialUsuario.Value.Obs.ActualizacionIdioma" }
        };

        private static readonly Dictionary<string, string> CampoKeys = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Username",    "FormHistorialUsuario.Value.Field.Username" },
            { "PasswordHash","FormHistorialUsuario.Value.Field.PasswordHash" },
            { "Activo",      "FormHistorialUsuario.Value.Field.Activo" },
            { "IdiomaId",    "FormHistorialUsuario.Value.Field.IdiomaId" },
            { "Permisos",    "FormHistorialUsuario.Value.Field.Permisos" },
            { "Sin cambios", "FormHistorialUsuario.Value.Field.SinCambios" }
        };

        public FormHistorialUsuario(int usuarioId, string username)
        {


            _usuarioBLL = new UsuarioBLL();
            _observadorIdioma = Actualizar;
            _usuarioId = usuarioId;
            _username = username;
            InitializeComponent();
            Load += FormHistorialUsuario_Load;
            FormClosed += FormHistorialUsuario_FormClosed;
        }

        private void FormHistorialUsuario_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(_observadorIdioma);
            lblUsuarioHistorial.Text = "Usuario: " + _username;
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
        }

        private void btnRefrescarHistorial_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void btnRestaurarVersion_Click(object sender, EventArgs e)
        {
            if (dgvHistorialUsuario.SelectedRows.Count == 0)
            {
                UiDialogHelper.MostrarWarning("FormHistorialUsuario.Warning.SelectVersion");
                return;
            }

            int historialId = Convert.ToInt32(dgvHistorialUsuario.SelectedRows[0].Cells["HistorialId"].Value);
            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormHistorialUsuario.Confirm.RestoreVersionMessage",
                "FormHistorialUsuario.Confirm.RestoreVersionTitle",
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _usuarioBLL.RestaurarVersion(historialId);
                UiDialogHelper.MostrarInfo("FormHistorialUsuario.Success.VersionRestored");
                CargarHistorial();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Restauracion de usuario");
            }
        }

        private void dgvHistorialUsuario_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHistorialUsuario.SelectedRows.Count == 0)
            {
                dgvCambiosEstado.DataSource = null;
                return;
            }

            int historialId = Convert.ToInt32(dgvHistorialUsuario.SelectedRows[0].Cells["HistorialId"].Value);
            try
            {
                Idioma idioma = IdiomaService.GetInstance().IdiomaActual;
                List<ResultadoComparacionEstado> diferencias = _usuarioBLL.ObtenerDiferenciasConVersionAnterior(historialId);
                GrillaTraduccionHelper.Recargar(
                    dgvCambiosEstado,
                    diferencias,
                    ProyectarCambioEstadoFila,
                    idioma,
                    g => ActualizarEncabezadosCambios(idioma, g));
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de diferencias de historial");
            }
        }

        private void CargarHistorial()
        {
            try
            {
                List<UsuarioHistorialSnapshot> historial = _usuarioBLL.ObtenerHistorialEstados(_usuarioId);
                Idioma idioma = IdiomaService.GetInstance().IdiomaActual;
                GrillaTraduccionHelper.Recargar(
                    dgvHistorialUsuario,
                    historial,
                    ProyectarHistorialFila,
                    idioma,
                    g => ActualizarEncabezadosHistorial(idioma, g));
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de historial de usuario");
            }
        }

        private HistorialUsuarioFila ProyectarHistorialFila(UsuarioHistorialSnapshot snap, Idioma idioma)
        {
            return new HistorialUsuarioFila
            {
                HistorialId = snap.HistorialId,
                UsuarioId = snap.UsuarioId,
                FechaHora = snap.FechaHora,
                ActorUsuarioId = snap.ActorUsuarioId,
                ActorUsernameSnapshot = snap.ActorUsernameSnapshot,
                Accion = TraducirAccion(snap.Accion, idioma),
                Observaciones = TraducirObservaciones(snap.Observaciones, idioma),
                Username = snap.Username,
                Activo = snap.Activo,
                IdiomaId = snap.IdiomaId
            };
        }

        private CambioEstadoFila ProyectarCambioEstadoFila(ResultadoComparacionEstado diff, Idioma idioma)
        {
            return new CambioEstadoFila
            {
                Campo = TraducirCampo(diff.Campo, idioma),
                ValorAnterior = TraducirValor(diff.Campo, diff.ValorAnterior, idioma),
                ValorNuevo = TraducirValor(diff.Campo, diff.ValorNuevo, idioma)
            };
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            CargarHistorial();
            ActualizarEncabezadosCambios(idioma);
        }

        private void ActualizarEncabezadosHistorial(Idioma idioma, DataGridView grilla = null)
        {
            DataGridView g = grilla ?? dgvHistorialUsuario;
            if (idioma == null || g.Columns.Count == 0)
            {
                return;
            }

            GrillaTraduccionHelper.SetHeader(g, "HistorialId",           "FormHistorialUsuario.Grid.HistorialId", idioma);
            GrillaTraduccionHelper.SetHeader(g, "UsuarioId",             "FormHistorialUsuario.Grid.UsuarioId", idioma);
            GrillaTraduccionHelper.SetHeader(g, "FechaHora",             "FormHistorialUsuario.Grid.FechaHora", idioma);
            GrillaTraduccionHelper.SetHeader(g, "ActorUsuarioId",        "FormHistorialUsuario.Grid.ActorUsuarioId", idioma);
            GrillaTraduccionHelper.SetHeader(g, "ActorUsernameSnapshot", "FormHistorialUsuario.Grid.ActorUsernameSnapshot", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Accion",                "FormHistorialUsuario.Grid.Accion", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Observaciones",         "FormHistorialUsuario.Grid.Observaciones", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Username",              "FormHistorialUsuario.Grid.Username", idioma);
            GrillaTraduccionHelper.SetHeader(g, "Activo",                "FormHistorialUsuario.Grid.Activo", idioma);
            GrillaTraduccionHelper.SetHeader(g, "IdiomaId",              "FormHistorialUsuario.Grid.IdiomaId", idioma);
        }

        private void ActualizarEncabezadosCambios(Idioma idioma, DataGridView grilla = null)
        {
            DataGridView g = grilla ?? dgvCambiosEstado;
            if (idioma == null || g.Columns.Count == 0)
            {
                return;
            }

            GrillaTraduccionHelper.SetHeader(g, "Campo",        "FormHistorialUsuario.DiffGrid.Campo", idioma);
            GrillaTraduccionHelper.SetHeader(g, "ValorAnterior","FormHistorialUsuario.DiffGrid.ValorAnterior", idioma);
            GrillaTraduccionHelper.SetHeader(g, "ValorNuevo",   "FormHistorialUsuario.DiffGrid.ValorNuevo", idioma);
        }

        private string TraducirAccion(string accion, Idioma idioma)
        {
            if (idioma == null || accion == null)
            {
                return accion ?? string.Empty;
            }

            string key;
            return AccionKeys.TryGetValue(accion, out key) ? idioma.ObtenerTraduccion(key) : accion;
        }

        private string TraducirObservaciones(string obs, Idioma idioma)
        {
            if (idioma == null || obs == null)
            {
                return obs ?? string.Empty;
            }

            string key;
            return ObservacionesKeys.TryGetValue(obs, out key) ? idioma.ObtenerTraduccion(key) : obs;
        }

        private string TraducirCampo(string campo, Idioma idioma)
        {
            if (idioma == null || campo == null)
            {
                return campo ?? string.Empty;
            }

            string key;
            return CampoKeys.TryGetValue(campo, out key) ? idioma.ObtenerTraduccion(key) : campo;
        }

        private string TraducirValor(string campo, string valor, Idioma idioma)
        {
            if (idioma == null || valor == null || valor == "-")
            {
                return valor ?? string.Empty;
            }

            switch (campo)
            {
                case "PasswordHash":
                    if (valor == "Configurada") return idioma.ObtenerTraduccion("FormHistorialUsuario.Value.Marker.Configurada");
                    if (valor == "Actualizada") return idioma.ObtenerTraduccion("FormHistorialUsuario.Value.Marker.Actualizada");
                    break;
                case "Activo":
                    if (valor == "True")  return idioma.ObtenerTraduccion("FormHistorialUsuario.Value.Marker.True");
                    if (valor == "False") return idioma.ObtenerTraduccion("FormHistorialUsuario.Value.Marker.False");
                    break;
            }

            return valor;
        }

        private void FormHistorialUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(_observadorIdioma);
        }
    }
}
