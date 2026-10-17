using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;

namespace TpIDS
{
    public partial class FormMisNotificaciones : FormTraducibleBase
    {
        private readonly NotificacionBLL notificacionBLL = CompositionRoot.Notificacion;
        private readonly ISesion sesion = CompositionRoot.Sesion;
        private FiltroNotificacion filtroAplicado = new FiltroNotificacion();
        private PaginaNotificaciones resultado = new PaginaNotificaciones();
        private bool ocupado;
        public event EventHandler NotificacionLeida;

        public FormMisNotificaciones()
        {
            InitializeComponent();
            Load += FormMisNotificaciones_Load;
        }

        private async void FormMisNotificaciones_Load(object sender, EventArgs e)
        {
            MaximizarEnAreaMdi();
            await CargarPaginaAsync(1);
        }

        private async Task CargarPaginaAsync(int pagina, FiltroNotificacion filtro = null)
        {
            if (ocupado) return;
            int? usuarioId = sesion.GetUsuarioActualId();
            CambiarOcupado(true);
            try
            {
                FiltroNotificacion consulta = filtro ?? filtroAplicado;
                var datos = await Task.Run(() => notificacionBLL.BuscarPersonales(consulta, pagina));
                if (IsDisposed || Disposing || usuarioId != sesion.GetUsuarioActualId()) return;
                filtroAplicado = consulta;
                resultado = datos;
                dgvNotificacionesPersonales.DataSource = datos.Elementos;
                ActualizarResumen();
            }
            catch (Exception ex)
            {
                if (!IsDisposed && !Disposing && usuarioId == sesion.GetUsuarioActualId())
                    UiExceptionHandler.Mostrar(ex, "Consulta de notificaciones personales");
            }
            finally { if (!IsDisposed && !Disposing) CambiarOcupado(false); }
        }

        private async void btnBuscarNotificaciones_Click(object sender, EventArgs e)
        {
            await CargarPaginaAsync(1, new FiltroNotificacion {
                FechaDesde = chkbDesdeNotificaciones.Checked ? (DateTime?)dtpDesdeNotificaciones.Value.Date : null,
                FechaHasta = chkbHastaNotificaciones.Checked ? (DateTime?)dtpHastaNotificaciones.Value.Date : null });
        }

        private async void btnAnteriorNotificaciones_Click(object sender, EventArgs e) { await CargarPaginaAsync(resultado.Pagina - 1); }
        private async void btnSiguienteNotificaciones_Click(object sender, EventArgs e) { await CargarPaginaAsync(resultado.Pagina + 1); }
        private async void btnDetalleNotificacion_Click(object sender, EventArgs e) { await LeerSeleccionadaAsync(true); }
        private async void btnMarcarNotificacionLeida_Click(object sender, EventArgs e) { await LeerSeleccionadaAsync(false); }
        private async void dgvNotificacionesPersonales_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) await LeerSeleccionadaAsync(true);
        }

        private async Task LeerSeleccionadaAsync(bool abrirDetalle)
        {
            var seleccionada = dgvNotificacionesPersonales.CurrentRow?.DataBoundItem as Notificacion;
            if (ocupado || seleccionada == null) return;
            int? usuarioId = sesion.GetUsuarioActualId();
            CambiarOcupado(true);
            try
            {
                var leida = await Task.Run(() => notificacionBLL.LeerPersonal(seleccionada.Id));
                if (IsDisposed || Disposing || usuarioId != sesion.GetUsuarioActualId()) return;
                seleccionada.Leida = true;
                dgvNotificacionesPersonales.Invalidate();
                NotificacionLeida?.Invoke(this, EventArgs.Empty);
                if (abrirDetalle)
                    using (var detalle = new FormDetalleNotificacion(leida)) detalle.ShowDialog(this);
            }
            catch (Exception ex)
            {
                if (!IsDisposed && !Disposing && usuarioId == sesion.GetUsuarioActualId())
                    UiExceptionHandler.Mostrar(ex, "Lectura de notificación personal");
            }
            finally { if (!IsDisposed && !Disposing) CambiarOcupado(false); }
        }

        private void dgvNotificacionesPersonales_SelectionChanged(object sender, EventArgs e) { ActualizarAcciones(); }
        private void dgvNotificacionesPersonales_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var n = dgvNotificacionesPersonales.Rows[e.RowIndex].DataBoundItem as Notificacion;
            if (n == null) return;
            string columna = dgvNotificacionesPersonales.Columns[e.ColumnIndex].Name;
            if (columna == "Leida") e.Value = UiDialogHelper.Traducir(n.Leida ? "FormMyNotifications.Value.Read" : "FormMyNotifications.Value.Unread");
            else if (columna == "Tipo") e.Value = TextoTipo(n);
            else return;
            e.FormattingApplied = true;
        }

        internal static string TextoTipo(Notificacion n)
        {
            if (n is RecordatorioCompra) return UiDialogHelper.Traducir("FormMyNotifications.Value.PurchaseReminder");
            if (n is RecordatorioInversion) return UiDialogHelper.Traducir("FormMyNotifications.Value.InvestmentReminder");
            return UiDialogHelper.Traducir("FormMyNotifications.Value.ManualNotification");
        }

        private void CambiarOcupado(bool valor)
        {
            ocupado = valor;
            groupBoxFiltrosNotificaciones.Enabled = !valor;
            dgvNotificacionesPersonales.Enabled = !valor;
            ActualizarAcciones();
        }

        private void ActualizarAcciones()
        {
            var n = dgvNotificacionesPersonales.CurrentRow?.DataBoundItem as Notificacion;
            btnDetalleNotificacion.Enabled = !ocupado && n != null;
            btnMarcarNotificacionLeida.Enabled = !ocupado && n != null && !n.Leida;
            btnAnteriorNotificaciones.Enabled = !ocupado && resultado.Pagina > 1;
            btnSiguienteNotificaciones.Enabled = !ocupado && resultado.Pagina < resultado.TotalPaginas;
        }

        private void ActualizarResumen()
        {
            lblPaginaNotificaciones.Text = UiDialogHelper.Traducir("FormMyNotifications.PageSummary", resultado.Total == 0 ? 0 : resultado.Pagina, resultado.TotalPaginas, resultado.Total);
            lblSinNotificaciones.Visible = resultado.Total == 0;
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            Text = UiDialogHelper.Traducir("FormMyNotifications.Title");
            ActualizarResumen();
        }
    }
}
