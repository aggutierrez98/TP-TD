using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormHistorialUsuario : FormTraducibleBase
    {
        private readonly UsuarioBLL _usuarioBLL;
        private readonly int _usuarioId;
        private readonly string _username;

        public FormHistorialUsuario(int usuarioId, string username)
        {
            _usuarioBLL = new UsuarioBLL();
            _usuarioId = usuarioId;
            _username = username;
            InitializeComponent();
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
                    idioma);
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
                    idioma);
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
                Accion = UiDialogHelper.TraducirSiExiste(snap.Accion),
                Observaciones = UiDialogHelper.TraducirSiExiste(snap.Observaciones),
                Username = snap.Username,
                Activo = snap.Activo,
                IdiomaId = snap.IdiomaId
            };
        }

        private CambioEstadoFila ProyectarCambioEstadoFila(ResultadoComparacionEstado diff, Idioma idioma)
        {
            return new CambioEstadoFila
            {
                Campo = UiDialogHelper.TraducirSiExiste(diff.Campo),
                ValorAnterior = TraducirValor(diff.ValorAnterior),
                ValorNuevo = TraducirValor(diff.ValorNuevo)
            };
        }

        protected override void OnIdiomaActualizado(Idioma idioma)
        {
            base.OnIdiomaActualizado(idioma);
            ActualizarUsuarioLabel();
            CargarHistorial();
        }

        private string TraducirValor(string valor)
        {
            if (valor == null || valor == "-")
            {
                return valor ?? string.Empty;
            }

            return UiDialogHelper.TraducirSiExiste(valor);
        }

        private void ActualizarUsuarioLabel()
        {
            const string key = "FormHistorialUsuario.UserPrefix";
            string prefijo = UiDialogHelper.TraducirSiExiste(key);
            if (string.Equals(prefijo, key, StringComparison.Ordinal))
            {
                prefijo = "Usuario:";
            }

            lblUsuarioHistorial.Text = string.Format(
                "{0} {1}",
                prefijo,
                _username);
        }
    }
}
