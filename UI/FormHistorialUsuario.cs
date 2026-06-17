using System;
using System.Collections.Generic;
using System.Linq;
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
            // Configurar columnas (tags) una sola vez; los datos se cargan/refrescan abajo.
            GrillaColumnasBuilder.Configurar<HistorialUsuarioGridItem>(dgvHistorialUsuario);
            GrillaColumnasBuilder.Configurar<CambioEstadoGridItem>(dgvCambiosEstado);
            RefrescarDatos();
        }

        private void btnRefrescarHistorial_Click(object sender, EventArgs e)
        {
            RefrescarDatos();
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
                RefrescarDatos();
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
                List<ResultadoComparacionEstado> diferencias = _usuarioBLL.ObtenerDiferenciasConVersionAnterior(historialId);
                dgvCambiosEstado.DataSource = null;
                dgvCambiosEstado.DataSource = diferencias.Select(ProyectarCambioEstadoGridItem).ToList();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de diferencias de historial");
            }
        }

        private void RefrescarDatos()
        {
            try
            {
                List<UsuarioHistorialSnapshot> historial = _usuarioBLL.ObtenerHistorialEstados(_usuarioId);
                dgvHistorialUsuario.DataSource = null;
                dgvHistorialUsuario.DataSource = historial.Select(ProyectarHistorialFila).ToList();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de historial de usuario");
            }
        }

        // Las celdas guardan la clave cruda; la traduccion ocurre en tiempo de render
        // (columnas marcadas con TraducirValor en HistorialUsuarioGridItem / CambioEstadoGridItem).
        private HistorialUsuarioGridItem ProyectarHistorialFila(UsuarioHistorialSnapshot snap)
        {
            return new HistorialUsuarioGridItem
            {
                HistorialId = snap.HistorialId,
                UsuarioId = snap.UsuarioId,
                FechaHora = snap.FechaHora,
                ActorUsuarioId = snap.ActorUsuarioId,
                ActorUsernameSnapshot = snap.ActorUsernameSnapshot,
                Accion = snap.Accion,
                Observaciones = snap.Observaciones,
                Username = snap.Username,
                Activo = snap.Activo,
                IdiomaId = snap.IdiomaId
            };
        }

        private CambioEstadoGridItem ProyectarCambioEstadoGridItem(ResultadoComparacionEstado diff)
        {
            return new CambioEstadoGridItem
            {
                Campo = diff.Campo,
                ValorAnterior = diff.ValorAnterior,
                ValorNuevo = diff.ValorNuevo
            };
        }
    }
}
