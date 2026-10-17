using System.Drawing;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace TpIDS
{
    partial class FormMisNotificaciones
    {
        private KryptonGroupBox groupBoxFiltrosNotificaciones;
        private CheckBoxAccesible chkbDesdeNotificaciones, chkbHastaNotificaciones;
        private DateTimePickerAccesible dtpDesdeNotificaciones, dtpHastaNotificaciones;
        private KryptonButton btnBuscarNotificaciones, btnAnteriorNotificaciones, btnSiguienteNotificaciones, btnDetalleNotificacion, btnMarcarNotificacionLeida;
        private KryptonDataGridView dgvNotificacionesPersonales;
        private KryptonLabel lblTituloNotificacionesPersonales, lblPaginaNotificaciones, lblSinNotificaciones;

        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize = new Size(780, 564);
            lblTituloNotificacionesPersonales = new KryptonLabel { Name = "lblTituloNotificacionesPersonales", Text = "Mis notificaciones", Tag = "FormMyNotifications.Title", Location = new Point(18, 14), AutoSize = true };
            lblTituloNotificacionesPersonales.StateCommon.ShortText.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            groupBoxFiltrosNotificaciones = new KryptonGroupBox { Name = "groupBoxFiltrosNotificaciones", Text = "Filtros", Tag = "FormMyNotifications.Filters", Location = new Point(18, 52), Size = new Size(740, 78), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            ((System.ComponentModel.ISupportInitialize)groupBoxFiltrosNotificaciones).BeginInit();
            groupBoxFiltrosNotificaciones.Values.Heading = "Filtros";
            chkbDesdeNotificaciones = new CheckBoxAccesible { Name = "chkbDesdeNotificaciones", Text = "Desde:", Tag = "FormMyNotifications.From", Location = new Point(10, 13), AutoSize = true };
            dtpDesdeNotificaciones = new DateTimePickerAccesible { Name = "dtpDesdeNotificaciones", Format = DateTimePickerFormat.Short, Location = new Point(88, 11), Size = new Size(126, 25) };
            chkbHastaNotificaciones = new CheckBoxAccesible { Name = "chkbHastaNotificaciones", Text = "Hasta:", Tag = "FormMyNotifications.To", Location = new Point(234, 13), AutoSize = true };
            dtpHastaNotificaciones = new DateTimePickerAccesible { Name = "dtpHastaNotificaciones", Format = DateTimePickerFormat.Short, Location = new Point(310, 11), Size = new Size(126, 25) };
            btnBuscarNotificaciones = new KryptonButton { Name = "btnBuscarNotificaciones", Text = "Buscar", Tag = "FormMyNotifications.Search", ButtonStyle = ButtonStyle.Custom1, Location = new Point(595, 10), Size = new Size(120, 30), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnBuscarNotificaciones.Click += btnBuscarNotificaciones_Click;
            groupBoxFiltrosNotificaciones.Panel.Controls.AddRange(new Control[] { chkbDesdeNotificaciones, dtpDesdeNotificaciones, chkbHastaNotificaciones, dtpHastaNotificaciones, btnBuscarNotificaciones });
            ((System.ComponentModel.ISupportInitialize)groupBoxFiltrosNotificaciones).EndInit();
            dgvNotificacionesPersonales = new KryptonDataGridView { Name = "dgvNotificacionesPersonales", Tag = "FormMyNotifications.Grid", Location = new Point(18, 145), Size = new Size(740, 310), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AutoGenerateColumns = false, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, MultiSelect = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvNotificacionesPersonales.Columns.Add(new DataGridViewTextBoxColumn { Name = "Leida", DataPropertyName = "Leida", HeaderText = "Estado", FillWeight = 16, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvNotificacionesPersonales.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fecha", DataPropertyName = "Fecha", HeaderText = "Fecha", FillWeight = 22, SortMode = DataGridViewColumnSortMode.NotSortable, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
            dgvNotificacionesPersonales.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tipo", DataPropertyName = "Id", HeaderText = "Tipo", FillWeight = 25, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvNotificacionesPersonales.Columns.Add(new DataGridViewTextBoxColumn { Name = "Mensaje", DataPropertyName = "Mensaje", HeaderText = "Mensaje", FillWeight = 65, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvNotificacionesPersonales.SelectionChanged += dgvNotificacionesPersonales_SelectionChanged;
            dgvNotificacionesPersonales.CellFormatting += dgvNotificacionesPersonales_CellFormatting;
            dgvNotificacionesPersonales.CellDoubleClick += dgvNotificacionesPersonales_CellDoubleClick;
            lblSinNotificaciones = new KryptonLabel { Name = "lblSinNotificaciones", Text = "No hay notificaciones para estas fechas.", Tag = "FormMyNotifications.Empty", Location = new Point(30, 186), AutoSize = true, Visible = false };
            lblPaginaNotificaciones = new KryptonLabel { Name = "lblPaginaNotificaciones", Location = new Point(18, 470), Size = new Size(420, 28), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnAnteriorNotificaciones = new KryptonButton { Name = "btnAnteriorNotificaciones", Text = "Anterior", Tag = "FormMyNotifications.Previous", ButtonStyle = ButtonStyle.Custom1, Location = new Point(498, 466), Size = new Size(120, 30), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnSiguienteNotificaciones = new KryptonButton { Name = "btnSiguienteNotificaciones", Text = "Siguiente", Tag = "FormMyNotifications.Next", ButtonStyle = ButtonStyle.Custom1, Location = new Point(638, 466), Size = new Size(120, 30), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnAnteriorNotificaciones.Click += btnAnteriorNotificaciones_Click;
            btnSiguienteNotificaciones.Click += btnSiguienteNotificaciones_Click;
            btnDetalleNotificacion = new KryptonButton { Name = "btnDetalleNotificacion", Text = "Ver detalle", Tag = "FormMyNotifications.Details", ButtonStyle = ButtonStyle.Custom1, Location = new Point(18, 514), Size = new Size(140, 30), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnMarcarNotificacionLeida = new KryptonButton { Name = "btnMarcarNotificacionLeida", Text = "Marcar como leída", Tag = "FormMyNotifications.MarkRead", ButtonStyle = ButtonStyle.Custom1, Location = new Point(174, 514), Size = new Size(190, 30), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnDetalleNotificacion.Click += btnDetalleNotificacion_Click;
            btnMarcarNotificacionLeida.Click += btnMarcarNotificacionLeida_Click;
            Controls.AddRange(new Control[] { lblTituloNotificacionesPersonales, groupBoxFiltrosNotificaciones, dgvNotificacionesPersonales, lblSinNotificaciones, lblPaginaNotificaciones, btnAnteriorNotificaciones, btnSiguienteNotificaciones, btnDetalleNotificacion, btnMarcarNotificacionLeida });
            AutoScaleMode = AutoScaleMode.Font;
            Name = "FormMisNotificaciones";
            Text = "Mis notificaciones";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
