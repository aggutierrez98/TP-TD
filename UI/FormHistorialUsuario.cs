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

            // El username es dato fijo del constructor, no depende del idioma:
            // el prefijo lo traduce el helper via Tag y aca solo seteamos el dato.
            lblUsuarioHistorialNombre.Text = string.IsNullOrWhiteSpace(_username) ? "-" : _username;

            Load += FormHistorialUsuario_Load;
        }

        private void FormHistorialUsuario_Load(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void btnRefrescarHistorial_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void btnRestaurarVersion_Click(object sender, EventArgs e)
        {
            if (dgvHistorialUsuario.SelectedRows.Count == 0)
            {
                UiDialogHelper.MostrarWarning("FormUserHistory.Warning.SelectVersion");
                return;
            }

            int historialId = Convert.ToInt32(dgvHistorialUsuario.SelectedRows[0].Cells["HistorialId"].Value);
            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormUserHistory.Confirm.RestoreVersionMessage",
                "FormUserHistory.Confirm.RestoreVersionTitle",
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _usuarioBLL.RestaurarVersion(historialId);
                UiDialogHelper.MostrarInfo("FormUserHistory.Success.VersionRestored");
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
                GrillaFormHelper.Recargar(
                    dgvCambiosEstado,
                    diferencias,
                    ProyectarCambioEstadoGridItem,
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
                GrillaFormHelper.Recargar(
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

        private HistorialUsuarioGridItem ProyectarHistorialFila(UsuarioHistorialSnapshot snap, Idioma idioma)
        {
            return new HistorialUsuarioGridItem
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

        private CambioEstadoGridItem ProyectarCambioEstadoGridItem(ResultadoComparacionEstado diff, Idioma idioma)
        {
            return new CambioEstadoGridItem
            {
                Campo = UiDialogHelper.TraducirSiExiste(diff.Campo),
                ValorAnterior = TraducirValor(diff.ValorAnterior),
                ValorNuevo = TraducirValor(diff.ValorNuevo)
            };
        }

        private string TraducirValor(string valor)
        {
            if (valor == null || valor == "-")
            {
                return valor ?? string.Empty;
            }

            return UiDialogHelper.TraducirSiExiste(valor);
        }
    }
}
