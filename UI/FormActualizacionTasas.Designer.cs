using System.Drawing;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace TpIDS
{
    partial class FormActualizacionTasas
    {
        private KryptonLabel lblTituloActualizacionTasas, lblRequestsMesActualizacionTasas, lblDetalleActualizacionTasas;
        private KryptonButton btnActualizarAhoraTasas;
        private KryptonDataGridView dgvEjecucionesTasas, dgvDetalleEjecucionTasas;
        private TableLayoutPanel tlpGrillasActualizacionTasas;

        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize = new Size(980, 560);
            lblTituloActualizacionTasas = new KryptonLabel { Name = "lblTituloActualizacionTasas", Text = "Actualizacion de tasas", Tag = "FormExchangeRateUpdate.TitleLabel", Location = new Point(18, 14), AutoSize = true };
            lblTituloActualizacionTasas.StateCommon.ShortText.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRequestsMesActualizacionTasas = new KryptonLabel { Name = "lblRequestsMesActualizacionTasas", Location = new Point(18, 54), Size = new Size(780, 28), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            btnActualizarAhoraTasas = new KryptonButton { Name = "btnActualizarAhoraTasas", Text = "Actualizar ahora", Tag = "FormExchangeRateUpdate.UpdateNowButton", ButtonStyle = ButtonStyle.Custom1, Location = new Point(822, 52), Size = new Size(140, 30), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnActualizarAhoraTasas.Click += btnActualizarAhoraTasas_Click;

            dgvEjecucionesTasas = CrearGrilla("dgvEjecucionesTasas", "FormExchangeRateUpdate.Grid");
            dgvEjecucionesTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaInicio", DataPropertyName = "FechaInicio", HeaderText = "Fecha de inicio", FillWeight = 22, SortMode = DataGridViewColumnSortMode.NotSortable, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
            dgvEjecucionesTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Origen", DataPropertyName = "Origen", HeaderText = "Origen", FillWeight = 14, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvEjecucionesTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Resultado", DataPropertyName = "Resultado", HeaderText = "Resultado", FillWeight = 14, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvEjecucionesTasas.Columns.Add(new DataGridViewCheckBoxColumn { Name = "ConsumioRequest", DataPropertyName = "ConsumioRequest", HeaderText = "Consumio request", FillWeight = 14, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvEjecucionesTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Mensaje", DataPropertyName = "Mensaje", HeaderText = "Mensaje", FillWeight = 50, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvEjecucionesTasas.CellFormatting += dgvEjecucionesTasas_CellFormatting;
            dgvEjecucionesTasas.SelectionChanged += dgvEjecucionesTasas_SelectionChanged;

            lblDetalleActualizacionTasas = new KryptonLabel { Name = "lblDetalleActualizacionTasas", Text = "Detalle de la ejecucion seleccionada", Tag = "FormExchangeRateUpdate.DetailLabel", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom, Margin = new Padding(0) };

            dgvDetalleEjecucionTasas = CrearGrilla("dgvDetalleEjecucionTasas", "FormExchangeRateUpdate.DetailGrid");
            dgvDetalleEjecucionTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "MonedaBase", DataPropertyName = "MonedaBase", HeaderText = "Moneda base", FillWeight = 18, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvDetalleEjecucionTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "MonedaConversion", DataPropertyName = "MonedaConversion", HeaderText = "Moneda de conversion", FillWeight = 22, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvDetalleEjecucionTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "ValorAnterior", DataPropertyName = "ValorAnterior", HeaderText = "Valor anterior", FillWeight = 20, SortMode = DataGridViewColumnSortMode.NotSortable, DefaultCellStyle = new DataGridViewCellStyle { Format = "N6", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvDetalleEjecucionTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "ValorNuevo", DataPropertyName = "ValorNuevo", HeaderText = "Valor nuevo", FillWeight = 20, SortMode = DataGridViewColumnSortMode.NotSortable, DefaultCellStyle = new DataGridViewCellStyle { Format = "N6", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvDetalleEjecucionTasas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", DataPropertyName = "Estado", HeaderText = "Estado", FillWeight = 20, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgvDetalleEjecucionTasas.CellFormatting += dgvDetalleEjecucionTasas_CellFormatting;

            tlpGrillasActualizacionTasas = new TableLayoutPanel { Name = "tlpGrillasActualizacionTasas", Location = new Point(18, 94), Size = new Size(944, 446), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, ColumnCount = 1, RowCount = 3 };
            tlpGrillasActualizacionTasas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpGrillasActualizacionTasas.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            tlpGrillasActualizacionTasas.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpGrillasActualizacionTasas.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            tlpGrillasActualizacionTasas.Controls.Add(dgvEjecucionesTasas, 0, 0);
            tlpGrillasActualizacionTasas.Controls.Add(lblDetalleActualizacionTasas, 0, 1);
            tlpGrillasActualizacionTasas.Controls.Add(dgvDetalleEjecucionTasas, 0, 2);

            Controls.AddRange(new Control[] { lblTituloActualizacionTasas, lblRequestsMesActualizacionTasas, btnActualizarAhoraTasas, tlpGrillasActualizacionTasas });
            AutoScaleMode = AutoScaleMode.Font;
            Name = "FormActualizacionTasas";
            StartPosition = FormStartPosition.CenterParent;
            Tag = "FormExchangeRateUpdate.Title";
            Text = "Actualizacion de tasas";
            ResumeLayout(false);
            PerformLayout();
        }

        private static KryptonDataGridView CrearGrilla(string nombre, string tag)
        {
            return new KryptonDataGridView
            {
                Name = nombre,
                Tag = tag,
                Margin = new Padding(0, 0, 0, 6),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
        }
    }
}
