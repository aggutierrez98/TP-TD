using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    // Pantalla de "operador": administra notificaciones de todos los usuarios (back-office).
    // El gate de permiso (gestionnotificaciones) se resuelve aca, no en el BLL, igual que FormGestionUsuarios.
    public partial class FormGestionNotificaciones : FormTraducibleBase
    {
        private readonly NotificacionBLL notificacionBLL = CompositionRoot.Notificacion;
        private readonly UsuarioBLL usuarioBLL = CompositionRoot.Usuario;
        private bool actualizandoGrid;

        public FormGestionNotificaciones()
        {
            InitializeComponent();
            Load += FormGestionNotificaciones_Load;
        }

        // View-model plano para la grilla: mezcla las 3 subclases de Notificacion.
        private sealed class FilaNotificacion
        {
            public Notificacion Notificacion { get; set; }
            public int Id { get; set; }
            public string Usuario { get; set; }
            public string Tipo { get; set; }
            public string Mensaje { get; set; }
            public DateTime Fecha { get; set; }
            public bool Leida { get; set; }
        }

        private void FormGestionNotificaciones_Load(object sender, EventArgs e)
        {
            dgvNotificaciones.CurrentCellDirtyStateChanged += dgvNotificaciones_CurrentCellDirtyStateChanged;
            dgvNotificaciones.CellValueChanged += dgvNotificaciones_CellValueChanged;
            CargarUsuarios();
            CargarNotificaciones();
        }

        private void CargarUsuarios()
        {
            try
            {
                List<Usuario> usuarios = usuarioBLL.ObtenerTodos();
                cmbUsuarioDestino.DataSource = null;
                cmbUsuarioDestino.DisplayMember = "Username";
                cmbUsuarioDestino.ValueMember = "Id";
                cmbUsuarioDestino.DataSource = usuarios;
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de usuarios para notificaciones");
            }
        }

        private void CargarNotificaciones()
        {
            try
            {
                List<Notificacion> notificaciones = notificacionBLL.ObtenerTodas();

                actualizandoGrid = true;
                try
                {
                    dgvNotificaciones.DataSource = null;
                    dgvNotificaciones.DataSource = notificaciones
                        .Select(ProyectarFila)
                        .ToList();
                }
                finally
                {
                    actualizandoGrid = false;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de notificaciones");
            }
        }

        private static FilaNotificacion ProyectarFila(Notificacion notificacion)
        {
            return new FilaNotificacion
            {
                Id = notificacion.Id,
                Notificacion = notificacion,
                Usuario = notificacion.Usuario?.Username ?? string.Empty,
                Tipo = notificacion.GetType().Name,
                Mensaje = VistaPreviaNotificacion.Resumir(notificacion.Mensaje),
                Fecha = notificacion.Fecha,
                Leida = notificacion.Leida
            };
        }

        private void btnAgregarNotificacion_Click(object sender, EventArgs e)
        {
            if (!(cmbUsuarioDestino.SelectedItem is Usuario usuarioDestino))
            {
                UiDialogHelper.MostrarWarning("FormNotificationManagement.Warning.SelectUser");
                return;
            }

            string mensaje = txtbMensajeNotificacion.Text;

            try
            {
                notificacionBLL.CrearManual(new Notificacion { Usuario = usuarioDestino, Mensaje = mensaje });
                txtbMensajeNotificacion.Clear();
                CargarNotificaciones();
                UiDialogHelper.MostrarInfo("FormNotificationManagement.Success.NotificationCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de notificacion");
            }
        }

        private void btnEliminarNotificacion_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaNotificacion fila))
            {
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormNotificationManagement.Confirm.DeleteMessage",
                "FormNotificationManagement.Confirm.DeleteTitle",
                MessageBoxIcon.Warning,
                fila.Mensaje);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminada = notificacionBLL.Eliminar(fila.Notificacion);
                if (eliminada)
                {
                    CargarNotificaciones();
                    UiDialogHelper.MostrarInfo("FormNotificationManagement.Success.NotificationDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de notificacion");
            }
        }

        private void dgvNotificaciones_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvNotificaciones.IsCurrentCellDirty)
            {
                dgvNotificaciones.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvNotificaciones_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (actualizandoGrid || e.RowIndex < 0)
            {
                return;
            }

            if (!string.Equals(dgvNotificaciones.Columns[e.ColumnIndex].Name, "Leida", StringComparison.Ordinal))
            {
                return;
            }

            if (!(dgvNotificaciones.Rows[e.RowIndex].DataBoundItem is FilaNotificacion fila))
            {
                return;
            }

            try
            {
                notificacionBLL.MarcarLeida(fila.Notificacion, fila.Leida);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Actualizacion de notificacion");
                CargarNotificaciones();
            }
        }

        private bool ObtenerFilaSeleccionada(out FilaNotificacion fila)
        {
            fila = dgvNotificaciones.CurrentRow?.DataBoundItem as FilaNotificacion;
            if (fila == null)
            {
                UiDialogHelper.MostrarWarning("FormNotificationManagement.Warning.SelectNotification");
                return false;
            }

            return true;
        }
    }
}
