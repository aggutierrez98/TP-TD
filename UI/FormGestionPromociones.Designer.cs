namespace TpIDS
{
    partial class FormGestionPromociones
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblBuscarTextoPromocion = new Krypton.Toolkit.KryptonLabel();
            this.txtbBuscarTextoPromocion = new Krypton.Toolkit.KryptonTextBox();
            this.lblBuscarComercioPromocion = new Krypton.Toolkit.KryptonLabel();
            this.cmbComercioBusquedaPromocion = new Krypton.Toolkit.KryptonComboBox();
            this.btnBuscarPromociones = new Krypton.Toolkit.KryptonButton();
            this.tlpBusquedaPromociones = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxBusquedaPromociones = new Krypton.Toolkit.KryptonGroupBox();
            this.dgvPromociones = new Krypton.Toolkit.KryptonDataGridView();
            this.colPromocionDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionDescuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionTopeDescuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionDiasSemana = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionVigenciaDesde = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionVigenciaHasta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxListadoPromociones = new Krypton.Toolkit.KryptonGroupBox();
            this.lblDescripcionPromocion = new Krypton.Toolkit.KryptonLabel();
            this.txtbDescripcionPromocion = new Krypton.Toolkit.KryptonTextBox();
            this.lblDescuentoPromocion = new Krypton.Toolkit.KryptonLabel();
            this.txtbDescuentoPromocion = new Krypton.Toolkit.KryptonTextBox();
            this.lblTopePromocion = new Krypton.Toolkit.KryptonLabel();
            this.txtbTopePromocion = new Krypton.Toolkit.KryptonTextBox();
            this.lblVigenciaDesdePromocion = new Krypton.Toolkit.KryptonLabel();
            this.dtpVigenciaDesdePromocion = new TpIDS.DateTimePickerAccesible();
            this.lblVigenciaHastaPromocion = new Krypton.Toolkit.KryptonLabel();
            this.dtpVigenciaHastaPromocion = new TpIDS.DateTimePickerAccesible();
            this.lblDiasPromocion = new Krypton.Toolkit.KryptonLabel();
            this.chkbDiaLunes = new TpIDS.CheckBoxAccesible();
            this.chkbDiaMartes = new TpIDS.CheckBoxAccesible();
            this.chkbDiaMiercoles = new TpIDS.CheckBoxAccesible();
            this.chkbDiaJueves = new TpIDS.CheckBoxAccesible();
            this.chkbDiaViernes = new TpIDS.CheckBoxAccesible();
            this.chkbDiaSabado = new TpIDS.CheckBoxAccesible();
            this.chkbDiaDomingo = new TpIDS.CheckBoxAccesible();
            this.flpDiasPromocion = new System.Windows.Forms.FlowLayoutPanel();
            this.tlpDatosPromocion = new System.Windows.Forms.TableLayoutPanel();
            this.lblLeyendaFiltrosPromocion = new Krypton.Toolkit.KryptonLabel();
            this.lblAdvertenciaCategoriaExcluida = new Krypton.Toolkit.KryptonLabel();
            this.lblFiltroComercios = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoComercios = new System.Windows.Forms.Panel();
            this.clbComercios = new System.Windows.Forms.CheckedListBox();
            this.lblVacioComercios = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroRubrosComercio = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoRubrosComercio = new System.Windows.Forms.Panel();
            this.clbRubrosComercio = new System.Windows.Forms.CheckedListBox();
            this.lblVacioRubrosComercio = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroEntidadesFinancieras = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoEntidadesFinancieras = new System.Windows.Forms.Panel();
            this.clbEntidadesFinancieras = new System.Windows.Forms.CheckedListBox();
            this.lblVacioEntidadesFinancieras = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroMediosPago = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoMediosPago = new System.Windows.Forms.Panel();
            this.clbMediosPago = new System.Windows.Forms.CheckedListBox();
            this.lblVacioMediosPago = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroSegmentos = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoSegmentos = new System.Windows.Forms.Panel();
            this.clbSegmentos = new System.Windows.Forms.CheckedListBox();
            this.lblVacioSegmentos = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroProductosIncluidos = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoProductosIncluidos = new System.Windows.Forms.Panel();
            this.clbProductosIncluidos = new System.Windows.Forms.CheckedListBox();
            this.lblVacioProductosIncluidos = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroProductosExcluidos = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoProductosExcluidos = new System.Windows.Forms.Panel();
            this.clbProductosExcluidos = new System.Windows.Forms.CheckedListBox();
            this.lblVacioProductosExcluidos = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroCategoriasIncluidas = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoCategoriasIncluidas = new System.Windows.Forms.Panel();
            this.clbCategoriasIncluidas = new System.Windows.Forms.CheckedListBox();
            this.lblVacioCategoriasIncluidas = new Krypton.Toolkit.KryptonWrapLabel();
            this.lblFiltroCategoriasExcluidas = new Krypton.Toolkit.KryptonLabel();
            this.pnlMarcoCategoriasExcluidas = new System.Windows.Forms.Panel();
            this.clbCategoriasExcluidas = new System.Windows.Forms.CheckedListBox();
            this.lblVacioCategoriasExcluidas = new Krypton.Toolkit.KryptonWrapLabel();
            this.tlpFiltrosPromocion = new System.Windows.Forms.TableLayoutPanel();
            this.pnlEditorPromocion = new Krypton.Toolkit.KryptonPanel();
            this.btnAgregarPromocion = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarPromocion = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarPromocion = new Krypton.Toolkit.KryptonButton();
            this.btnLimpiarPromocion = new Krypton.Toolkit.KryptonButton();
            this.groupBoxEditorPromocion = new Krypton.Toolkit.KryptonGroupBox();
            this.tlpPromociones = new System.Windows.Forms.TableLayoutPanel();
            this.lblTituloPromociones = new Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxBusquedaPromociones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxBusquedaPromociones.Panel)).BeginInit();
            this.groupBoxBusquedaPromociones.Panel.SuspendLayout();
            this.groupBoxBusquedaPromociones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxListadoPromociones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxListadoPromociones.Panel)).BeginInit();
            this.groupBoxListadoPromociones.Panel.SuspendLayout();
            this.groupBoxListadoPromociones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorPromocion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorPromocion.Panel)).BeginInit();
            this.groupBoxEditorPromocion.Panel.SuspendLayout();
            this.groupBoxEditorPromocion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlEditorPromocion)).BeginInit();
            this.pnlEditorPromocion.SuspendLayout();
            this.tlpBusquedaPromociones.SuspendLayout();
            this.tlpDatosPromocion.SuspendLayout();
            this.tlpFiltrosPromocion.SuspendLayout();
            this.tlpPromociones.SuspendLayout();
            this.flpDiasPromocion.SuspendLayout();
            this.pnlMarcoComercios.SuspendLayout();
            this.pnlMarcoRubrosComercio.SuspendLayout();
            this.pnlMarcoEntidadesFinancieras.SuspendLayout();
            this.pnlMarcoMediosPago.SuspendLayout();
            this.pnlMarcoSegmentos.SuspendLayout();
            this.pnlMarcoProductosIncluidos.SuspendLayout();
            this.pnlMarcoProductosExcluidos.SuspendLayout();
            this.pnlMarcoCategoriasIncluidas.SuspendLayout();
            this.pnlMarcoCategoriasExcluidas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbComercioBusquedaPromocion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromociones)).BeginInit();
            this.SuspendLayout();
            //
            // lblBuscarTextoPromocion
            //
            this.lblBuscarTextoPromocion.AutoSize = true;
            this.lblBuscarTextoPromocion.Location = new System.Drawing.Point(0, 0);
            this.lblBuscarTextoPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblBuscarTextoPromocion.Name = "lblBuscarTextoPromocion";
            this.lblBuscarTextoPromocion.Size = new System.Drawing.Size(40, 13);
            this.lblBuscarTextoPromocion.TabIndex = 0;
            this.lblBuscarTextoPromocion.Tag = "FormPromotionManagement.SearchTextLabel";
            this.lblBuscarTextoPromocion.Values.Text = "Texto:";
            //
            // txtbBuscarTextoPromocion
            //
            this.txtbBuscarTextoPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbBuscarTextoPromocion.Location = new System.Drawing.Point(52, 1);
            this.txtbBuscarTextoPromocion.Margin = new System.Windows.Forms.Padding(0, 1, 16, 0);
            this.txtbBuscarTextoPromocion.MaxLength = 100;
            this.txtbBuscarTextoPromocion.Name = "txtbBuscarTextoPromocion";
            this.txtbBuscarTextoPromocion.Size = new System.Drawing.Size(300, 20);
            this.txtbBuscarTextoPromocion.TabIndex = 1;
            //
            // lblBuscarComercioPromocion
            //
            this.lblBuscarComercioPromocion.AutoSize = true;
            this.lblBuscarComercioPromocion.Location = new System.Drawing.Point(368, 0);
            this.lblBuscarComercioPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblBuscarComercioPromocion.Name = "lblBuscarComercioPromocion";
            this.lblBuscarComercioPromocion.Size = new System.Drawing.Size(58, 13);
            this.lblBuscarComercioPromocion.TabIndex = 2;
            this.lblBuscarComercioPromocion.Tag = "FormPromotionManagement.SearchMerchantLabel";
            this.lblBuscarComercioPromocion.Values.Text = "Comercio:";
            //
            // cmbComercioBusquedaPromocion
            //
            this.cmbComercioBusquedaPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbComercioBusquedaPromocion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComercioBusquedaPromocion.FormattingEnabled = true;
            this.cmbComercioBusquedaPromocion.Location = new System.Drawing.Point(438, 0);
            this.cmbComercioBusquedaPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.cmbComercioBusquedaPromocion.Name = "cmbComercioBusquedaPromocion";
            this.cmbComercioBusquedaPromocion.Size = new System.Drawing.Size(300, 21);
            this.cmbComercioBusquedaPromocion.TabIndex = 3;
            //
            // btnBuscarPromociones
            //
            this.btnBuscarPromociones.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnBuscarPromociones.Location = new System.Drawing.Point(754, 0);
            this.btnBuscarPromociones.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btnBuscarPromociones.Name = "btnBuscarPromociones";
            this.btnBuscarPromociones.Size = new System.Drawing.Size(110, 26);
            this.btnBuscarPromociones.TabIndex = 4;
            this.btnBuscarPromociones.Tag = "FormPromotionManagement.SearchButton";
            this.btnBuscarPromociones.Values.Text = "Buscar";
            this.btnBuscarPromociones.Click += new System.EventHandler(this.btnBuscarPromociones_Click);
            //
            // tlpBusquedaPromociones
            //
            this.tlpBusquedaPromociones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpBusquedaPromociones.ColumnCount = 5;
            this.tlpBusquedaPromociones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tlpBusquedaPromociones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tlpBusquedaPromociones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlpBusquedaPromociones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpBusquedaPromociones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpBusquedaPromociones.Controls.Add(this.lblBuscarTextoPromocion, 0, 0);
            this.tlpBusquedaPromociones.Controls.Add(this.txtbBuscarTextoPromocion, 1, 0);
            this.tlpBusquedaPromociones.Controls.Add(this.lblBuscarComercioPromocion, 2, 0);
            this.tlpBusquedaPromociones.Controls.Add(this.cmbComercioBusquedaPromocion, 3, 0);
            this.tlpBusquedaPromociones.Controls.Add(this.btnBuscarPromociones, 4, 0);
            this.tlpBusquedaPromociones.Location = new System.Drawing.Point(8, 4);
            this.tlpBusquedaPromociones.Name = "tlpBusquedaPromociones";
            this.tlpBusquedaPromociones.RowCount = 1;
            this.tlpBusquedaPromociones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpBusquedaPromociones.Size = new System.Drawing.Size(910, 28);
            this.tlpBusquedaPromociones.TabIndex = 5;
            //
            // groupBoxBusquedaPromociones
            //
            this.groupBoxBusquedaPromociones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBusquedaPromociones.Panel.Controls.Add(this.tlpBusquedaPromociones);
            this.groupBoxBusquedaPromociones.Location = new System.Drawing.Point(0, 0);
            this.groupBoxBusquedaPromociones.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.groupBoxBusquedaPromociones.Name = "groupBoxBusquedaPromociones";
            this.groupBoxBusquedaPromociones.Size = new System.Drawing.Size(940, 62);
            this.groupBoxBusquedaPromociones.TabIndex = 6;
            this.groupBoxBusquedaPromociones.TabStop = false;
            this.groupBoxBusquedaPromociones.Tag = "FormPromotionManagement.SearchGroup";
            this.groupBoxBusquedaPromociones.Values.Heading = "Buscar";
            //
            // dgvPromociones
            //
            this.dgvPromociones.AllowUserToAddRows = false;
            this.dgvPromociones.AllowUserToDeleteRows = false;
            this.dgvPromociones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPromociones.AutoGenerateColumns = false;
            this.dgvPromociones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPromociones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPromociones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.colPromocionDescripcion,
                        this.colPromocionDescuento,
                        this.colPromocionTopeDescuento,
                        this.colPromocionDiasSemana,
                        this.colPromocionVigenciaDesde,
                        this.colPromocionVigenciaHasta});
            this.dgvPromociones.Location = new System.Drawing.Point(6, 4);
            this.dgvPromociones.MultiSelect = false;
            this.dgvPromociones.Name = "dgvPromociones";
            this.dgvPromociones.ReadOnly = true;
            this.dgvPromociones.RowHeadersVisible = false;
            this.dgvPromociones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPromociones.Size = new System.Drawing.Size(918, 120);
            this.dgvPromociones.TabIndex = 7;
            this.dgvPromociones.Tag = "FormPromotionManagement.PromotionGrid";
            this.dgvPromociones.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPromociones_CellClick);
            this.dgvPromociones.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPromociones_CellFormatting);
            //
            // colPromocionDescripcion
            //
            this.colPromocionDescripcion.DataPropertyName = "Descripcion";
            this.colPromocionDescripcion.FillWeight = 38F;
            this.colPromocionDescripcion.HeaderText = "Descripcion";
            this.colPromocionDescripcion.Name = "Descripcion";
            this.colPromocionDescripcion.ReadOnly = true;
            //
            // colPromocionDescuento
            //
            this.colPromocionDescuento.DataPropertyName = "Descuento";
            this.colPromocionDescuento.FillWeight = 10F;
            this.colPromocionDescuento.HeaderText = "Descuento";
            this.colPromocionDescuento.Name = "Descuento";
            this.colPromocionDescuento.ReadOnly = true;
            //
            // colPromocionTopeDescuento
            //
            this.colPromocionTopeDescuento.DataPropertyName = "TopeDescuento";
            this.colPromocionTopeDescuento.FillWeight = 12F;
            this.colPromocionTopeDescuento.HeaderText = "Tope";
            this.colPromocionTopeDescuento.Name = "TopeDescuento";
            this.colPromocionTopeDescuento.ReadOnly = true;
            //
            // colPromocionDiasSemana
            //
            this.colPromocionDiasSemana.DataPropertyName = "DiasSemana";
            this.colPromocionDiasSemana.FillWeight = 20F;
            this.colPromocionDiasSemana.HeaderText = "Dias";
            this.colPromocionDiasSemana.Name = "DiasSemana";
            this.colPromocionDiasSemana.ReadOnly = true;
            //
            // colPromocionVigenciaDesde
            //
            this.colPromocionVigenciaDesde.DataPropertyName = "VigenciaDesde";
            this.colPromocionVigenciaDesde.FillWeight = 10F;
            this.colPromocionVigenciaDesde.HeaderText = "Vigencia desde";
            this.colPromocionVigenciaDesde.Name = "VigenciaDesde";
            this.colPromocionVigenciaDesde.ReadOnly = true;
            //
            // colPromocionVigenciaHasta
            //
            this.colPromocionVigenciaHasta.DataPropertyName = "VigenciaHasta";
            this.colPromocionVigenciaHasta.FillWeight = 10F;
            this.colPromocionVigenciaHasta.HeaderText = "Vigencia hasta";
            this.colPromocionVigenciaHasta.Name = "VigenciaHasta";
            this.colPromocionVigenciaHasta.ReadOnly = true;
            //
            // groupBoxListadoPromociones
            //
            this.groupBoxListadoPromociones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxListadoPromociones.Panel.Controls.Add(this.dgvPromociones);
            this.groupBoxListadoPromociones.Location = new System.Drawing.Point(0, 66);
            this.groupBoxListadoPromociones.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.groupBoxListadoPromociones.Name = "groupBoxListadoPromociones";
            this.groupBoxListadoPromociones.Size = new System.Drawing.Size(940, 162);
            this.groupBoxListadoPromociones.TabIndex = 8;
            this.groupBoxListadoPromociones.TabStop = false;
            this.groupBoxListadoPromociones.Tag = "FormPromotionManagement.ListGroup";
            this.groupBoxListadoPromociones.Values.Heading = "Promociones";
            //
            // lblDescripcionPromocion
            //
            this.lblDescripcionPromocion.AutoSize = true;
            this.lblDescripcionPromocion.Location = new System.Drawing.Point(0, 0);
            this.lblDescripcionPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblDescripcionPromocion.Name = "lblDescripcionPromocion";
            this.lblDescripcionPromocion.Size = new System.Drawing.Size(66, 13);
            this.lblDescripcionPromocion.TabIndex = 9;
            this.lblDescripcionPromocion.Tag = "FormPromotionManagement.DescriptionLabel";
            this.lblDescripcionPromocion.Values.Text = "Descripcion:";
            //
            // txtbDescripcionPromocion
            //
            this.txtbDescripcionPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbDescripcionPromocion.Location = new System.Drawing.Point(90, 1);
            this.txtbDescripcionPromocion.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.txtbDescripcionPromocion.MaxLength = 200;
            this.txtbDescripcionPromocion.Name = "txtbDescripcionPromocion";
            this.txtbDescripcionPromocion.Size = new System.Drawing.Size(800, 20);
            this.txtbDescripcionPromocion.TabIndex = 10;
            //
            // lblDescuentoPromocion
            //
            this.lblDescuentoPromocion.AutoSize = true;
            this.lblDescuentoPromocion.Location = new System.Drawing.Point(0, 30);
            this.lblDescuentoPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblDescuentoPromocion.Name = "lblDescuentoPromocion";
            this.lblDescuentoPromocion.Size = new System.Drawing.Size(80, 13);
            this.lblDescuentoPromocion.TabIndex = 11;
            this.lblDescuentoPromocion.Tag = "FormPromotionManagement.DiscountLabel";
            this.lblDescuentoPromocion.Values.Text = "Descuento (%):";
            //
            // txtbDescuentoPromocion
            //
            this.txtbDescuentoPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbDescuentoPromocion.Location = new System.Drawing.Point(90, 31);
            this.txtbDescuentoPromocion.Margin = new System.Windows.Forms.Padding(0, 1, 20, 0);
            this.txtbDescuentoPromocion.MaxLength = 10;
            this.txtbDescuentoPromocion.Name = "txtbDescuentoPromocion";
            this.txtbDescuentoPromocion.Size = new System.Drawing.Size(300, 20);
            this.txtbDescuentoPromocion.TabIndex = 12;
            //
            // lblTopePromocion
            //
            this.lblTopePromocion.AutoSize = true;
            this.lblTopePromocion.Location = new System.Drawing.Point(410, 30);
            this.lblTopePromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblTopePromocion.Name = "lblTopePromocion";
            this.lblTopePromocion.Size = new System.Drawing.Size(50, 13);
            this.lblTopePromocion.TabIndex = 13;
            this.lblTopePromocion.Tag = "FormPromotionManagement.CapLabel";
            this.lblTopePromocion.Values.Text = "Tope ($):";
            //
            // txtbTopePromocion
            //
            this.txtbTopePromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbTopePromocion.Location = new System.Drawing.Point(500, 31);
            this.txtbTopePromocion.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.txtbTopePromocion.MaxLength = 15;
            this.txtbTopePromocion.Name = "txtbTopePromocion";
            this.txtbTopePromocion.Size = new System.Drawing.Size(300, 20);
            this.txtbTopePromocion.TabIndex = 14;
            //
            // lblVigenciaDesdePromocion
            //
            this.lblVigenciaDesdePromocion.AutoSize = true;
            this.lblVigenciaDesdePromocion.Location = new System.Drawing.Point(0, 60);
            this.lblVigenciaDesdePromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblVigenciaDesdePromocion.Name = "lblVigenciaDesdePromocion";
            this.lblVigenciaDesdePromocion.Size = new System.Drawing.Size(88, 13);
            this.lblVigenciaDesdePromocion.TabIndex = 15;
            this.lblVigenciaDesdePromocion.Tag = "FormPromotionManagement.ValidFromLabel";
            this.lblVigenciaDesdePromocion.Values.Text = "Vigencia desde:";
            //
            // dtpVigenciaDesdePromocion
            //
            this.dtpVigenciaDesdePromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpVigenciaDesdePromocion.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVigenciaDesdePromocion.Location = new System.Drawing.Point(90, 60);
            this.dtpVigenciaDesdePromocion.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.dtpVigenciaDesdePromocion.Name = "dtpVigenciaDesdePromocion";
            this.dtpVigenciaDesdePromocion.Size = new System.Drawing.Size(300, 20);
            this.dtpVigenciaDesdePromocion.TabIndex = 16;
            //
            // lblVigenciaHastaPromocion
            //
            this.lblVigenciaHastaPromocion.AutoSize = true;
            this.lblVigenciaHastaPromocion.Location = new System.Drawing.Point(410, 60);
            this.lblVigenciaHastaPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblVigenciaHastaPromocion.Name = "lblVigenciaHastaPromocion";
            this.lblVigenciaHastaPromocion.Size = new System.Drawing.Size(88, 13);
            this.lblVigenciaHastaPromocion.TabIndex = 17;
            this.lblVigenciaHastaPromocion.Tag = "FormPromotionManagement.ValidToLabel";
            this.lblVigenciaHastaPromocion.Values.Text = "Vigencia hasta:";
            //
            // dtpVigenciaHastaPromocion
            //
            this.dtpVigenciaHastaPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpVigenciaHastaPromocion.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVigenciaHastaPromocion.Location = new System.Drawing.Point(500, 60);
            this.dtpVigenciaHastaPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.dtpVigenciaHastaPromocion.Name = "dtpVigenciaHastaPromocion";
            this.dtpVigenciaHastaPromocion.Size = new System.Drawing.Size(300, 20);
            this.dtpVigenciaHastaPromocion.TabIndex = 18;
            //
            // lblDiasPromocion
            //
            this.lblDiasPromocion.AutoSize = true;
            this.lblDiasPromocion.Location = new System.Drawing.Point(0, 90);
            this.lblDiasPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblDiasPromocion.Name = "lblDiasPromocion";
            this.lblDiasPromocion.Size = new System.Drawing.Size(30, 13);
            this.lblDiasPromocion.TabIndex = 19;
            this.lblDiasPromocion.Tag = "FormPromotionManagement.DaysLabel";
            this.lblDiasPromocion.Values.Text = "Dias:";
            //
            // chkbDiaLunes
            //
            this.chkbDiaLunes.Location = new System.Drawing.Point(0, 0);
            this.chkbDiaLunes.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.chkbDiaLunes.Name = "chkbDiaLunes";
            this.chkbDiaLunes.Size = new System.Drawing.Size(90, 20);
            this.chkbDiaLunes.TabIndex = 20;
            this.chkbDiaLunes.Tag = "FormMerchantDetail.Value.Day.Lunes";
            this.chkbDiaLunes.Values.Text = "Lunes";
            //
            // chkbDiaMartes
            //
            this.chkbDiaMartes.Location = new System.Drawing.Point(0, 0);
            this.chkbDiaMartes.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.chkbDiaMartes.Name = "chkbDiaMartes";
            this.chkbDiaMartes.Size = new System.Drawing.Size(90, 20);
            this.chkbDiaMartes.TabIndex = 21;
            this.chkbDiaMartes.Tag = "FormMerchantDetail.Value.Day.Martes";
            this.chkbDiaMartes.Values.Text = "Martes";
            //
            // chkbDiaMiercoles
            //
            this.chkbDiaMiercoles.Location = new System.Drawing.Point(0, 0);
            this.chkbDiaMiercoles.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.chkbDiaMiercoles.Name = "chkbDiaMiercoles";
            this.chkbDiaMiercoles.Size = new System.Drawing.Size(90, 20);
            this.chkbDiaMiercoles.TabIndex = 22;
            this.chkbDiaMiercoles.Tag = "FormMerchantDetail.Value.Day.Miercoles";
            this.chkbDiaMiercoles.Values.Text = "Miercoles";
            //
            // chkbDiaJueves
            //
            this.chkbDiaJueves.Location = new System.Drawing.Point(0, 0);
            this.chkbDiaJueves.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.chkbDiaJueves.Name = "chkbDiaJueves";
            this.chkbDiaJueves.Size = new System.Drawing.Size(90, 20);
            this.chkbDiaJueves.TabIndex = 23;
            this.chkbDiaJueves.Tag = "FormMerchantDetail.Value.Day.Jueves";
            this.chkbDiaJueves.Values.Text = "Jueves";
            //
            // chkbDiaViernes
            //
            this.chkbDiaViernes.Location = new System.Drawing.Point(0, 0);
            this.chkbDiaViernes.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.chkbDiaViernes.Name = "chkbDiaViernes";
            this.chkbDiaViernes.Size = new System.Drawing.Size(90, 20);
            this.chkbDiaViernes.TabIndex = 24;
            this.chkbDiaViernes.Tag = "FormMerchantDetail.Value.Day.Viernes";
            this.chkbDiaViernes.Values.Text = "Viernes";
            //
            // chkbDiaSabado
            //
            this.chkbDiaSabado.Location = new System.Drawing.Point(0, 0);
            this.chkbDiaSabado.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.chkbDiaSabado.Name = "chkbDiaSabado";
            this.chkbDiaSabado.Size = new System.Drawing.Size(90, 20);
            this.chkbDiaSabado.TabIndex = 25;
            this.chkbDiaSabado.Tag = "FormMerchantDetail.Value.Day.Sabado";
            this.chkbDiaSabado.Values.Text = "Sabado";
            //
            // chkbDiaDomingo
            //
            this.chkbDiaDomingo.Location = new System.Drawing.Point(0, 0);
            this.chkbDiaDomingo.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            this.chkbDiaDomingo.Name = "chkbDiaDomingo";
            this.chkbDiaDomingo.Size = new System.Drawing.Size(90, 20);
            this.chkbDiaDomingo.TabIndex = 26;
            this.chkbDiaDomingo.Tag = "FormMerchantDetail.Value.Day.Domingo";
            this.chkbDiaDomingo.Values.Text = "Domingo";
            //
            // flpDiasPromocion
            //
            this.flpDiasPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flpDiasPromocion.Controls.Add(this.chkbDiaLunes);
            this.flpDiasPromocion.Controls.Add(this.chkbDiaMartes);
            this.flpDiasPromocion.Controls.Add(this.chkbDiaMiercoles);
            this.flpDiasPromocion.Controls.Add(this.chkbDiaJueves);
            this.flpDiasPromocion.Controls.Add(this.chkbDiaViernes);
            this.flpDiasPromocion.Controls.Add(this.chkbDiaSabado);
            this.flpDiasPromocion.Controls.Add(this.chkbDiaDomingo);
            this.flpDiasPromocion.Location = new System.Drawing.Point(90, 90);
            this.flpDiasPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.flpDiasPromocion.Name = "flpDiasPromocion";
            this.flpDiasPromocion.Size = new System.Drawing.Size(810, 28);
            this.flpDiasPromocion.TabIndex = 27;
            //
            // tlpDatosPromocion
            //
            this.tlpDatosPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpDatosPromocion.ColumnCount = 4;
            this.tlpDatosPromocion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpDatosPromocion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpDatosPromocion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpDatosPromocion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpDatosPromocion.Controls.Add(this.lblDescripcionPromocion, 0, 0);
            this.tlpDatosPromocion.Controls.Add(this.txtbDescripcionPromocion, 1, 0);
            this.tlpDatosPromocion.Controls.Add(this.lblDescuentoPromocion, 0, 1);
            this.tlpDatosPromocion.Controls.Add(this.txtbDescuentoPromocion, 1, 1);
            this.tlpDatosPromocion.Controls.Add(this.lblTopePromocion, 2, 1);
            this.tlpDatosPromocion.Controls.Add(this.txtbTopePromocion, 3, 1);
            this.tlpDatosPromocion.Controls.Add(this.lblVigenciaDesdePromocion, 0, 2);
            this.tlpDatosPromocion.Controls.Add(this.dtpVigenciaDesdePromocion, 1, 2);
            this.tlpDatosPromocion.Controls.Add(this.lblVigenciaHastaPromocion, 2, 2);
            this.tlpDatosPromocion.Controls.Add(this.dtpVigenciaHastaPromocion, 3, 2);
            this.tlpDatosPromocion.Controls.Add(this.lblDiasPromocion, 0, 3);
            this.tlpDatosPromocion.Controls.Add(this.flpDiasPromocion, 1, 3);
            this.tlpDatosPromocion.SetColumnSpan(this.txtbDescripcionPromocion, 3);
            this.tlpDatosPromocion.SetColumnSpan(this.flpDiasPromocion, 3);
            this.tlpDatosPromocion.Location = new System.Drawing.Point(0, 0);
            this.tlpDatosPromocion.Name = "tlpDatosPromocion";
            this.tlpDatosPromocion.RowCount = 4;
            this.tlpDatosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpDatosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpDatosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpDatosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpDatosPromocion.Size = new System.Drawing.Size(900, 120);
            this.tlpDatosPromocion.TabIndex = 28;
            //
            // lblLeyendaFiltrosPromocion
            //
            this.lblLeyendaFiltrosPromocion.AutoSize = true;
            this.lblLeyendaFiltrosPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeyendaFiltrosPromocion.Location = new System.Drawing.Point(0, 124);
            this.lblLeyendaFiltrosPromocion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblLeyendaFiltrosPromocion.Name = "lblLeyendaFiltrosPromocion";
            this.lblLeyendaFiltrosPromocion.Size = new System.Drawing.Size(700, 13);
            this.lblLeyendaFiltrosPromocion.TabIndex = 29;
            this.lblLeyendaFiltrosPromocion.Tag = "FormPromotionManagement.FiltersLegend";
            this.lblLeyendaFiltrosPromocion.Values.Text = "Filtros opcionales: una lista vacia no restringe (aplica a todos). En las listas de excluidos, la exclusion gana.";
            //
            // lblAdvertenciaCategoriaExcluida
            //
            this.lblAdvertenciaCategoriaExcluida.AutoSize = true;
            this.lblAdvertenciaCategoriaExcluida.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAdvertenciaCategoriaExcluida.Location = new System.Drawing.Point(0, 146);
            this.lblAdvertenciaCategoriaExcluida.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblAdvertenciaCategoriaExcluida.Name = "lblAdvertenciaCategoriaExcluida";
            this.lblAdvertenciaCategoriaExcluida.Size = new System.Drawing.Size(700, 13);
            this.lblAdvertenciaCategoriaExcluida.TabIndex = 30;
            this.lblAdvertenciaCategoriaExcluida.Tag = "FormPromotionManagement.Warning.IncludedProductInExcludedCategory";
            this.lblAdvertenciaCategoriaExcluida.Values.Text = "Advertencia: hay un producto incluido que pertenece a una categoria excluida; ese producto quedara fuera.";
            //
            // lblFiltroComercios
            //
            this.lblFiltroComercios.AutoSize = true;
            this.lblFiltroComercios.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroComercios.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroComercios.Name = "lblFiltroComercios";
            this.lblFiltroComercios.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroComercios.TabIndex = 31;
            this.lblFiltroComercios.Tag = "FormPromotionManagement.MerchantsLabel";
            this.lblFiltroComercios.Values.Text = "Comercios";
            //
            // clbComercios
            //
            this.clbComercios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbComercios.CheckOnClick = true;
            this.clbComercios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbComercios.FormattingEnabled = true;
            this.clbComercios.IntegralHeight = false;
            this.clbComercios.Location = new System.Drawing.Point(1, 1);
            this.clbComercios.Name = "clbComercios";
            this.clbComercios.Size = new System.Drawing.Size(288, 110);
            this.clbComercios.TabIndex = 32;
            //
            // lblVacioComercios
            //
            this.lblVacioComercios.AutoSize = false;
            this.lblVacioComercios.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioComercios.Name = "lblVacioComercios";
            this.lblVacioComercios.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioComercios.Size = new System.Drawing.Size(288, 40);
            this.lblVacioComercios.TabIndex = 0;
            this.lblVacioComercios.Tag = "FormPromotionManagement.Empty.Merchants";
            this.lblVacioComercios.Text = "No hay comercios cargados. Se dan de alta en Operador > Comercios.";
            this.clbComercios.Controls.Add(this.lblVacioComercios);
            //
            // pnlMarcoComercios
            //
            this.pnlMarcoComercios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoComercios.Controls.Add(this.clbComercios);
            this.pnlMarcoComercios.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoComercios.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoComercios.Name = "pnlMarcoComercios";
            this.pnlMarcoComercios.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoComercios.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoComercios.TabIndex = 33;
            //
            // lblFiltroRubrosComercio
            //
            this.lblFiltroRubrosComercio.AutoSize = true;
            this.lblFiltroRubrosComercio.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroRubrosComercio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroRubrosComercio.Name = "lblFiltroRubrosComercio";
            this.lblFiltroRubrosComercio.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroRubrosComercio.TabIndex = 34;
            this.lblFiltroRubrosComercio.Tag = "FormPromotionManagement.MerchantCategoriesLabel";
            this.lblFiltroRubrosComercio.Values.Text = "Rubros de comercio";
            //
            // clbRubrosComercio
            //
            this.clbRubrosComercio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbRubrosComercio.CheckOnClick = true;
            this.clbRubrosComercio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbRubrosComercio.FormattingEnabled = true;
            this.clbRubrosComercio.IntegralHeight = false;
            this.clbRubrosComercio.Location = new System.Drawing.Point(1, 1);
            this.clbRubrosComercio.Name = "clbRubrosComercio";
            this.clbRubrosComercio.Size = new System.Drawing.Size(288, 110);
            this.clbRubrosComercio.TabIndex = 35;
            //
            // lblVacioRubrosComercio
            //
            this.lblVacioRubrosComercio.AutoSize = false;
            this.lblVacioRubrosComercio.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioRubrosComercio.Name = "lblVacioRubrosComercio";
            this.lblVacioRubrosComercio.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioRubrosComercio.Size = new System.Drawing.Size(288, 40);
            this.lblVacioRubrosComercio.TabIndex = 0;
            this.lblVacioRubrosComercio.Tag = "FormPromotionManagement.Empty.MerchantCategories";
            this.lblVacioRubrosComercio.Text = "No hay rubros cargados. Se dan de alta en Operador > Comercios.";
            this.clbRubrosComercio.Controls.Add(this.lblVacioRubrosComercio);
            //
            // pnlMarcoRubrosComercio
            //
            this.pnlMarcoRubrosComercio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoRubrosComercio.Controls.Add(this.clbRubrosComercio);
            this.pnlMarcoRubrosComercio.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoRubrosComercio.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoRubrosComercio.Name = "pnlMarcoRubrosComercio";
            this.pnlMarcoRubrosComercio.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoRubrosComercio.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoRubrosComercio.TabIndex = 36;
            //
            // lblFiltroEntidadesFinancieras
            //
            this.lblFiltroEntidadesFinancieras.AutoSize = true;
            this.lblFiltroEntidadesFinancieras.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroEntidadesFinancieras.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroEntidadesFinancieras.Name = "lblFiltroEntidadesFinancieras";
            this.lblFiltroEntidadesFinancieras.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroEntidadesFinancieras.TabIndex = 37;
            this.lblFiltroEntidadesFinancieras.Tag = "FormPromotionManagement.FinancialEntitiesLabel";
            this.lblFiltroEntidadesFinancieras.Values.Text = "Entidades financieras";
            //
            // clbEntidadesFinancieras
            //
            this.clbEntidadesFinancieras.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbEntidadesFinancieras.CheckOnClick = true;
            this.clbEntidadesFinancieras.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbEntidadesFinancieras.FormattingEnabled = true;
            this.clbEntidadesFinancieras.IntegralHeight = false;
            this.clbEntidadesFinancieras.Location = new System.Drawing.Point(1, 1);
            this.clbEntidadesFinancieras.Name = "clbEntidadesFinancieras";
            this.clbEntidadesFinancieras.Size = new System.Drawing.Size(288, 110);
            this.clbEntidadesFinancieras.TabIndex = 38;
            //
            // lblVacioEntidadesFinancieras
            //
            this.lblVacioEntidadesFinancieras.AutoSize = false;
            this.lblVacioEntidadesFinancieras.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioEntidadesFinancieras.Name = "lblVacioEntidadesFinancieras";
            this.lblVacioEntidadesFinancieras.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioEntidadesFinancieras.Size = new System.Drawing.Size(288, 40);
            this.lblVacioEntidadesFinancieras.TabIndex = 0;
            this.lblVacioEntidadesFinancieras.Tag = "FormPromotionManagement.Empty.FinancialEntities";
            this.lblVacioEntidadesFinancieras.Text = "No hay entidades cargadas. Se dan de alta en Operador > Entidades financieras.";
            this.clbEntidadesFinancieras.Controls.Add(this.lblVacioEntidadesFinancieras);
            //
            // pnlMarcoEntidadesFinancieras
            //
            this.pnlMarcoEntidadesFinancieras.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoEntidadesFinancieras.Controls.Add(this.clbEntidadesFinancieras);
            this.pnlMarcoEntidadesFinancieras.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoEntidadesFinancieras.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoEntidadesFinancieras.Name = "pnlMarcoEntidadesFinancieras";
            this.pnlMarcoEntidadesFinancieras.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoEntidadesFinancieras.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoEntidadesFinancieras.TabIndex = 39;
            //
            // lblFiltroMediosPago
            //
            this.lblFiltroMediosPago.AutoSize = true;
            this.lblFiltroMediosPago.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroMediosPago.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroMediosPago.Name = "lblFiltroMediosPago";
            this.lblFiltroMediosPago.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroMediosPago.TabIndex = 40;
            this.lblFiltroMediosPago.Tag = "FormPromotionManagement.PaymentMethodsLabel";
            this.lblFiltroMediosPago.Values.Text = "Medios de pago";
            //
            // clbMediosPago
            //
            this.clbMediosPago.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbMediosPago.CheckOnClick = true;
            this.clbMediosPago.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbMediosPago.FormattingEnabled = true;
            this.clbMediosPago.IntegralHeight = false;
            this.clbMediosPago.Location = new System.Drawing.Point(1, 1);
            this.clbMediosPago.Name = "clbMediosPago";
            this.clbMediosPago.Size = new System.Drawing.Size(288, 110);
            this.clbMediosPago.TabIndex = 41;
            //
            // lblVacioMediosPago
            //
            this.lblVacioMediosPago.AutoSize = false;
            this.lblVacioMediosPago.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioMediosPago.Name = "lblVacioMediosPago";
            this.lblVacioMediosPago.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioMediosPago.Size = new System.Drawing.Size(288, 40);
            this.lblVacioMediosPago.TabIndex = 0;
            this.lblVacioMediosPago.Tag = "FormPromotionManagement.Empty.PaymentMethods";
            this.lblVacioMediosPago.Text = "No hay medios de pago cargados. Se dan de alta en Operador > Medios de pago.";
            this.clbMediosPago.Controls.Add(this.lblVacioMediosPago);
            //
            // pnlMarcoMediosPago
            //
            this.pnlMarcoMediosPago.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoMediosPago.Controls.Add(this.clbMediosPago);
            this.pnlMarcoMediosPago.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoMediosPago.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoMediosPago.Name = "pnlMarcoMediosPago";
            this.pnlMarcoMediosPago.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoMediosPago.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoMediosPago.TabIndex = 42;
            //
            // lblFiltroSegmentos
            //
            this.lblFiltroSegmentos.AutoSize = true;
            this.lblFiltroSegmentos.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroSegmentos.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroSegmentos.Name = "lblFiltroSegmentos";
            this.lblFiltroSegmentos.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroSegmentos.TabIndex = 43;
            this.lblFiltroSegmentos.Tag = "FormPromotionManagement.SegmentsLabel";
            this.lblFiltroSegmentos.Values.Text = "Segmentos";
            //
            // clbSegmentos
            //
            this.clbSegmentos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbSegmentos.CheckOnClick = true;
            this.clbSegmentos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbSegmentos.FormattingEnabled = true;
            this.clbSegmentos.IntegralHeight = false;
            this.clbSegmentos.Location = new System.Drawing.Point(1, 1);
            this.clbSegmentos.Name = "clbSegmentos";
            this.clbSegmentos.Size = new System.Drawing.Size(288, 110);
            this.clbSegmentos.TabIndex = 44;
            //
            // lblVacioSegmentos
            //
            this.lblVacioSegmentos.AutoSize = false;
            this.lblVacioSegmentos.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioSegmentos.Name = "lblVacioSegmentos";
            this.lblVacioSegmentos.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioSegmentos.Size = new System.Drawing.Size(288, 40);
            this.lblVacioSegmentos.TabIndex = 0;
            this.lblVacioSegmentos.Tag = "FormPromotionManagement.Empty.Segments";
            this.lblVacioSegmentos.Text = "No hay segmentos cargados. Salen de los medios de pago de cada entidad (Operador > Entidades financieras).";
            this.clbSegmentos.Controls.Add(this.lblVacioSegmentos);
            //
            // pnlMarcoSegmentos
            //
            this.pnlMarcoSegmentos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoSegmentos.Controls.Add(this.clbSegmentos);
            this.pnlMarcoSegmentos.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoSegmentos.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoSegmentos.Name = "pnlMarcoSegmentos";
            this.pnlMarcoSegmentos.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoSegmentos.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoSegmentos.TabIndex = 45;
            //
            // lblFiltroProductosIncluidos
            //
            this.lblFiltroProductosIncluidos.AutoSize = true;
            this.lblFiltroProductosIncluidos.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroProductosIncluidos.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroProductosIncluidos.Name = "lblFiltroProductosIncluidos";
            this.lblFiltroProductosIncluidos.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroProductosIncluidos.TabIndex = 46;
            this.lblFiltroProductosIncluidos.Tag = "FormPromotionManagement.IncludedProductsLabel";
            this.lblFiltroProductosIncluidos.Values.Text = "Productos incluidos";
            //
            // clbProductosIncluidos
            //
            this.clbProductosIncluidos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbProductosIncluidos.CheckOnClick = true;
            this.clbProductosIncluidos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbProductosIncluidos.FormattingEnabled = true;
            this.clbProductosIncluidos.IntegralHeight = false;
            this.clbProductosIncluidos.Location = new System.Drawing.Point(1, 1);
            this.clbProductosIncluidos.Name = "clbProductosIncluidos";
            this.clbProductosIncluidos.Size = new System.Drawing.Size(288, 110);
            this.clbProductosIncluidos.TabIndex = 47;
            this.clbProductosIncluidos.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbProductosIncluidos_ItemCheck);
            //
            // lblVacioProductosIncluidos
            //
            this.lblVacioProductosIncluidos.AutoSize = false;
            this.lblVacioProductosIncluidos.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioProductosIncluidos.Name = "lblVacioProductosIncluidos";
            this.lblVacioProductosIncluidos.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioProductosIncluidos.Size = new System.Drawing.Size(288, 40);
            this.lblVacioProductosIncluidos.TabIndex = 0;
            this.lblVacioProductosIncluidos.Tag = "FormPromotionManagement.Empty.Products";
            this.lblVacioProductosIncluidos.Text = "No hay productos cargados. Se dan de alta en Operador > Productos.";
            this.clbProductosIncluidos.Controls.Add(this.lblVacioProductosIncluidos);
            //
            // pnlMarcoProductosIncluidos
            //
            this.pnlMarcoProductosIncluidos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoProductosIncluidos.Controls.Add(this.clbProductosIncluidos);
            this.pnlMarcoProductosIncluidos.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoProductosIncluidos.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoProductosIncluidos.Name = "pnlMarcoProductosIncluidos";
            this.pnlMarcoProductosIncluidos.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoProductosIncluidos.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoProductosIncluidos.TabIndex = 48;
            //
            // lblFiltroProductosExcluidos
            //
            this.lblFiltroProductosExcluidos.AutoSize = true;
            this.lblFiltroProductosExcluidos.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroProductosExcluidos.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroProductosExcluidos.Name = "lblFiltroProductosExcluidos";
            this.lblFiltroProductosExcluidos.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroProductosExcluidos.TabIndex = 49;
            this.lblFiltroProductosExcluidos.Tag = "FormPromotionManagement.ExcludedProductsLabel";
            this.lblFiltroProductosExcluidos.Values.Text = "Productos excluidos";
            //
            // clbProductosExcluidos
            //
            this.clbProductosExcluidos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbProductosExcluidos.CheckOnClick = true;
            this.clbProductosExcluidos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbProductosExcluidos.FormattingEnabled = true;
            this.clbProductosExcluidos.IntegralHeight = false;
            this.clbProductosExcluidos.Location = new System.Drawing.Point(1, 1);
            this.clbProductosExcluidos.Name = "clbProductosExcluidos";
            this.clbProductosExcluidos.Size = new System.Drawing.Size(288, 110);
            this.clbProductosExcluidos.TabIndex = 50;
            //
            // lblVacioProductosExcluidos
            //
            this.lblVacioProductosExcluidos.AutoSize = false;
            this.lblVacioProductosExcluidos.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioProductosExcluidos.Name = "lblVacioProductosExcluidos";
            this.lblVacioProductosExcluidos.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioProductosExcluidos.Size = new System.Drawing.Size(288, 40);
            this.lblVacioProductosExcluidos.TabIndex = 0;
            this.lblVacioProductosExcluidos.Tag = "FormPromotionManagement.Empty.Products";
            this.lblVacioProductosExcluidos.Text = "No hay productos cargados. Se dan de alta en Operador > Productos.";
            this.clbProductosExcluidos.Controls.Add(this.lblVacioProductosExcluidos);
            //
            // pnlMarcoProductosExcluidos
            //
            this.pnlMarcoProductosExcluidos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoProductosExcluidos.Controls.Add(this.clbProductosExcluidos);
            this.pnlMarcoProductosExcluidos.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoProductosExcluidos.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoProductosExcluidos.Name = "pnlMarcoProductosExcluidos";
            this.pnlMarcoProductosExcluidos.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoProductosExcluidos.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoProductosExcluidos.TabIndex = 51;
            //
            // lblFiltroCategoriasIncluidas
            //
            this.lblFiltroCategoriasIncluidas.AutoSize = true;
            this.lblFiltroCategoriasIncluidas.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroCategoriasIncluidas.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroCategoriasIncluidas.Name = "lblFiltroCategoriasIncluidas";
            this.lblFiltroCategoriasIncluidas.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroCategoriasIncluidas.TabIndex = 52;
            this.lblFiltroCategoriasIncluidas.Tag = "FormPromotionManagement.IncludedCategoriesLabel";
            this.lblFiltroCategoriasIncluidas.Values.Text = "Categorias incluidas";
            //
            // clbCategoriasIncluidas
            //
            this.clbCategoriasIncluidas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbCategoriasIncluidas.CheckOnClick = true;
            this.clbCategoriasIncluidas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbCategoriasIncluidas.FormattingEnabled = true;
            this.clbCategoriasIncluidas.IntegralHeight = false;
            this.clbCategoriasIncluidas.Location = new System.Drawing.Point(1, 1);
            this.clbCategoriasIncluidas.Name = "clbCategoriasIncluidas";
            this.clbCategoriasIncluidas.Size = new System.Drawing.Size(288, 110);
            this.clbCategoriasIncluidas.TabIndex = 53;
            //
            // lblVacioCategoriasIncluidas
            //
            this.lblVacioCategoriasIncluidas.AutoSize = false;
            this.lblVacioCategoriasIncluidas.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioCategoriasIncluidas.Name = "lblVacioCategoriasIncluidas";
            this.lblVacioCategoriasIncluidas.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioCategoriasIncluidas.Size = new System.Drawing.Size(288, 40);
            this.lblVacioCategoriasIncluidas.TabIndex = 0;
            this.lblVacioCategoriasIncluidas.Tag = "FormPromotionManagement.Empty.ProductCategories";
            this.lblVacioCategoriasIncluidas.Text = "No hay categorias cargadas. Se gestionan en el detalle de un comercio (Operador > Comercios).";
            this.clbCategoriasIncluidas.Controls.Add(this.lblVacioCategoriasIncluidas);
            //
            // pnlMarcoCategoriasIncluidas
            //
            this.pnlMarcoCategoriasIncluidas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoCategoriasIncluidas.Controls.Add(this.clbCategoriasIncluidas);
            this.pnlMarcoCategoriasIncluidas.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoCategoriasIncluidas.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoCategoriasIncluidas.Name = "pnlMarcoCategoriasIncluidas";
            this.pnlMarcoCategoriasIncluidas.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoCategoriasIncluidas.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoCategoriasIncluidas.TabIndex = 54;
            //
            // lblFiltroCategoriasExcluidas
            //
            this.lblFiltroCategoriasExcluidas.AutoSize = true;
            this.lblFiltroCategoriasExcluidas.Location = new System.Drawing.Point(0, 0);
            this.lblFiltroCategoriasExcluidas.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblFiltroCategoriasExcluidas.Name = "lblFiltroCategoriasExcluidas";
            this.lblFiltroCategoriasExcluidas.Size = new System.Drawing.Size(100, 13);
            this.lblFiltroCategoriasExcluidas.TabIndex = 55;
            this.lblFiltroCategoriasExcluidas.Tag = "FormPromotionManagement.ExcludedCategoriesLabel";
            this.lblFiltroCategoriasExcluidas.Values.Text = "Categorias excluidas";
            //
            // clbCategoriasExcluidas
            //
            this.clbCategoriasExcluidas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbCategoriasExcluidas.CheckOnClick = true;
            this.clbCategoriasExcluidas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbCategoriasExcluidas.FormattingEnabled = true;
            this.clbCategoriasExcluidas.IntegralHeight = false;
            this.clbCategoriasExcluidas.Location = new System.Drawing.Point(1, 1);
            this.clbCategoriasExcluidas.Name = "clbCategoriasExcluidas";
            this.clbCategoriasExcluidas.Size = new System.Drawing.Size(288, 110);
            this.clbCategoriasExcluidas.TabIndex = 56;
            this.clbCategoriasExcluidas.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbCategoriasExcluidas_ItemCheck);
            //
            // lblVacioCategoriasExcluidas
            //
            this.lblVacioCategoriasExcluidas.AutoSize = false;
            this.lblVacioCategoriasExcluidas.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVacioCategoriasExcluidas.Name = "lblVacioCategoriasExcluidas";
            this.lblVacioCategoriasExcluidas.Padding = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.lblVacioCategoriasExcluidas.Size = new System.Drawing.Size(288, 40);
            this.lblVacioCategoriasExcluidas.TabIndex = 0;
            this.lblVacioCategoriasExcluidas.Tag = "FormPromotionManagement.Empty.ProductCategories";
            this.lblVacioCategoriasExcluidas.Text = "No hay categorias cargadas. Se gestionan en el detalle de un comercio (Operador > Comercios).";
            this.clbCategoriasExcluidas.Controls.Add(this.lblVacioCategoriasExcluidas);
            //
            // pnlMarcoCategoriasExcluidas
            //
            this.pnlMarcoCategoriasExcluidas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMarcoCategoriasExcluidas.Controls.Add(this.clbCategoriasExcluidas);
            this.pnlMarcoCategoriasExcluidas.Location = new System.Drawing.Point(0, 0);
            this.pnlMarcoCategoriasExcluidas.Margin = new System.Windows.Forms.Padding(0, 0, 10, 6);
            this.pnlMarcoCategoriasExcluidas.Name = "pnlMarcoCategoriasExcluidas";
            this.pnlMarcoCategoriasExcluidas.Padding = new System.Windows.Forms.Padding(1);
            this.pnlMarcoCategoriasExcluidas.Size = new System.Drawing.Size(290, 112);
            this.pnlMarcoCategoriasExcluidas.TabIndex = 57;
            //
            // tlpFiltrosPromocion
            //
            this.tlpFiltrosPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpFiltrosPromocion.ColumnCount = 3;
            this.tlpFiltrosPromocion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpFiltrosPromocion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpFiltrosPromocion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroComercios, 0, 0);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoComercios, 0, 1);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroRubrosComercio, 1, 0);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoRubrosComercio, 1, 1);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroEntidadesFinancieras, 2, 0);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoEntidadesFinancieras, 2, 1);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroMediosPago, 0, 2);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoMediosPago, 0, 3);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroSegmentos, 1, 2);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoSegmentos, 1, 3);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroProductosIncluidos, 2, 2);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoProductosIncluidos, 2, 3);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroProductosExcluidos, 0, 4);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoProductosExcluidos, 0, 5);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroCategoriasIncluidas, 1, 4);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoCategoriasIncluidas, 1, 5);
            this.tlpFiltrosPromocion.Controls.Add(this.lblFiltroCategoriasExcluidas, 2, 4);
            this.tlpFiltrosPromocion.Controls.Add(this.pnlMarcoCategoriasExcluidas, 2, 5);
            this.tlpFiltrosPromocion.Location = new System.Drawing.Point(0, 170);
            this.tlpFiltrosPromocion.Name = "tlpFiltrosPromocion";
            this.tlpFiltrosPromocion.RowCount = 6;
            this.tlpFiltrosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlpFiltrosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tlpFiltrosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlpFiltrosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tlpFiltrosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlpFiltrosPromocion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tlpFiltrosPromocion.Size = new System.Drawing.Size(900, 402);
            this.tlpFiltrosPromocion.TabIndex = 58;
            //
            // pnlEditorPromocion
            //
            this.pnlEditorPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlEditorPromocion.AutoScroll = true;
            this.pnlEditorPromocion.Controls.Add(this.tlpFiltrosPromocion);
            this.pnlEditorPromocion.Controls.Add(this.lblAdvertenciaCategoriaExcluida);
            this.pnlEditorPromocion.Controls.Add(this.lblLeyendaFiltrosPromocion);
            this.pnlEditorPromocion.Controls.Add(this.tlpDatosPromocion);
            this.pnlEditorPromocion.Location = new System.Drawing.Point(6, 4);
            this.pnlEditorPromocion.Name = "pnlEditorPromocion";
            this.pnlEditorPromocion.Size = new System.Drawing.Size(918, 276);
            this.pnlEditorPromocion.TabIndex = 59;
            //
            // btnAgregarPromocion
            //
            this.btnAgregarPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgregarPromocion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarPromocion.Location = new System.Drawing.Point(6, 286);
            this.btnAgregarPromocion.Name = "btnAgregarPromocion";
            this.btnAgregarPromocion.Size = new System.Drawing.Size(110, 28);
            this.btnAgregarPromocion.TabIndex = 60;
            this.btnAgregarPromocion.Tag = "FormPromotionManagement.AddButton";
            this.btnAgregarPromocion.Values.Text = "Agregar";
            this.btnAgregarPromocion.Click += new System.EventHandler(this.btnAgregarPromocion_Click);
            //
            // btnActualizarPromocion
            //
            this.btnActualizarPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnActualizarPromocion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarPromocion.Location = new System.Drawing.Point(126, 286);
            this.btnActualizarPromocion.Name = "btnActualizarPromocion";
            this.btnActualizarPromocion.Size = new System.Drawing.Size(110, 28);
            this.btnActualizarPromocion.TabIndex = 61;
            this.btnActualizarPromocion.Tag = "FormPromotionManagement.UpdateButton";
            this.btnActualizarPromocion.Values.Text = "Actualizar";
            this.btnActualizarPromocion.Click += new System.EventHandler(this.btnActualizarPromocion_Click);
            //
            // btnEliminarPromocion
            //
            this.btnEliminarPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEliminarPromocion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarPromocion.Location = new System.Drawing.Point(246, 286);
            this.btnEliminarPromocion.Name = "btnEliminarPromocion";
            this.btnEliminarPromocion.Size = new System.Drawing.Size(110, 28);
            this.btnEliminarPromocion.TabIndex = 62;
            this.btnEliminarPromocion.Tag = "FormPromotionManagement.DeleteButton";
            this.btnEliminarPromocion.Values.Text = "Eliminar";
            this.btnEliminarPromocion.Click += new System.EventHandler(this.btnEliminarPromocion_Click);
            //
            // btnLimpiarPromocion
            //
            this.btnLimpiarPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLimpiarPromocion.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnLimpiarPromocion.Location = new System.Drawing.Point(366, 286);
            this.btnLimpiarPromocion.Name = "btnLimpiarPromocion";
            this.btnLimpiarPromocion.Size = new System.Drawing.Size(110, 28);
            this.btnLimpiarPromocion.TabIndex = 63;
            this.btnLimpiarPromocion.Tag = "FormPromotionManagement.ClearButton";
            this.btnLimpiarPromocion.Values.Text = "Limpiar";
            this.btnLimpiarPromocion.Click += new System.EventHandler(this.btnLimpiarPromocion_Click);
            //
            // groupBoxEditorPromocion
            //
            this.groupBoxEditorPromocion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEditorPromocion.Panel.Controls.Add(this.pnlEditorPromocion);
            this.groupBoxEditorPromocion.Panel.Controls.Add(this.btnAgregarPromocion);
            this.groupBoxEditorPromocion.Panel.Controls.Add(this.btnActualizarPromocion);
            this.groupBoxEditorPromocion.Panel.Controls.Add(this.btnEliminarPromocion);
            this.groupBoxEditorPromocion.Panel.Controls.Add(this.btnLimpiarPromocion);
            this.groupBoxEditorPromocion.Location = new System.Drawing.Point(0, 232);
            this.groupBoxEditorPromocion.Name = "groupBoxEditorPromocion";
            this.groupBoxEditorPromocion.Size = new System.Drawing.Size(940, 348);
            this.groupBoxEditorPromocion.TabIndex = 64;
            this.groupBoxEditorPromocion.TabStop = false;
            this.groupBoxEditorPromocion.Tag = "FormPromotionManagement.EditorGroup";
            this.groupBoxEditorPromocion.Values.Heading = "Promocion";
            //
            // tlpPromociones
            //
            this.tlpPromociones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpPromociones.ColumnCount = 1;
            this.tlpPromociones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPromociones.Controls.Add(this.groupBoxBusquedaPromociones, 0, 0);
            this.tlpPromociones.Controls.Add(this.groupBoxListadoPromociones, 0, 1);
            this.tlpPromociones.Controls.Add(this.groupBoxEditorPromocion, 0, 2);
            this.tlpPromociones.Location = new System.Drawing.Point(10, 46);
            this.tlpPromociones.Name = "tlpPromociones";
            this.tlpPromociones.RowCount = 3;
            this.tlpPromociones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tlpPromociones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tlpPromociones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68F));
            this.tlpPromociones.Size = new System.Drawing.Size(940, 584);
            this.tlpPromociones.TabIndex = 65;
            //
            // lblTituloPromociones
            //
            this.lblTituloPromociones.AutoSize = true;
            this.lblTituloPromociones.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloPromociones.Location = new System.Drawing.Point(18, 12);
            this.lblTituloPromociones.Name = "lblTituloPromociones";
            this.lblTituloPromociones.Size = new System.Drawing.Size(250, 25);
            this.lblTituloPromociones.TabIndex = 66;
            this.lblTituloPromociones.Tag = "FormPromotionManagement.TitleLabel";
            this.lblTituloPromociones.Values.Text = "Gestion de Promociones";
            //
            // FormGestionPromociones
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 640);
            this.Controls.Add(this.tlpPromociones);
            this.Controls.Add(this.lblTituloPromociones);
            this.Name = "FormGestionPromociones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPromotionManagement.Title";
            this.Text = "Gestion de Promociones";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxBusquedaPromociones.Panel)).EndInit();
            this.groupBoxBusquedaPromociones.Panel.ResumeLayout(false);
            this.groupBoxBusquedaPromociones.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxBusquedaPromociones)).EndInit();
            this.groupBoxBusquedaPromociones.ResumeLayout(false);
            this.groupBoxBusquedaPromociones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxListadoPromociones.Panel)).EndInit();
            this.groupBoxListadoPromociones.Panel.ResumeLayout(false);
            this.groupBoxListadoPromociones.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxListadoPromociones)).EndInit();
            this.groupBoxListadoPromociones.ResumeLayout(false);
            this.groupBoxListadoPromociones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorPromocion.Panel)).EndInit();
            this.groupBoxEditorPromocion.Panel.ResumeLayout(false);
            this.groupBoxEditorPromocion.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEditorPromocion)).EndInit();
            this.groupBoxEditorPromocion.ResumeLayout(false);
            this.groupBoxEditorPromocion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlEditorPromocion)).EndInit();
            this.pnlEditorPromocion.ResumeLayout(false);
            this.pnlEditorPromocion.PerformLayout();
            this.tlpBusquedaPromociones.ResumeLayout(false);
            this.tlpBusquedaPromociones.PerformLayout();
            this.tlpDatosPromocion.ResumeLayout(false);
            this.tlpDatosPromocion.PerformLayout();
            this.tlpFiltrosPromocion.ResumeLayout(false);
            this.tlpFiltrosPromocion.PerformLayout();
            this.tlpPromociones.ResumeLayout(false);
            this.tlpPromociones.PerformLayout();
            this.flpDiasPromocion.ResumeLayout(false);
            this.pnlMarcoComercios.ResumeLayout(false);
            this.pnlMarcoRubrosComercio.ResumeLayout(false);
            this.pnlMarcoEntidadesFinancieras.ResumeLayout(false);
            this.pnlMarcoMediosPago.ResumeLayout(false);
            this.pnlMarcoSegmentos.ResumeLayout(false);
            this.pnlMarcoProductosIncluidos.ResumeLayout(false);
            this.pnlMarcoProductosExcluidos.ResumeLayout(false);
            this.pnlMarcoCategoriasIncluidas.ResumeLayout(false);
            this.pnlMarcoCategoriasExcluidas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbComercioBusquedaPromocion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromociones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloPromociones;
        private Krypton.Toolkit.KryptonLabel lblBuscarTextoPromocion;
        private Krypton.Toolkit.KryptonTextBox txtbBuscarTextoPromocion;
        private Krypton.Toolkit.KryptonLabel lblBuscarComercioPromocion;
        private Krypton.Toolkit.KryptonComboBox cmbComercioBusquedaPromocion;
        private Krypton.Toolkit.KryptonButton btnBuscarPromociones;
        private System.Windows.Forms.TableLayoutPanel tlpBusquedaPromociones;
        private Krypton.Toolkit.KryptonGroupBox groupBoxBusquedaPromociones;
        private Krypton.Toolkit.KryptonDataGridView dgvPromociones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDescuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionTopeDescuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDiasSemana;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionVigenciaDesde;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionVigenciaHasta;
        private Krypton.Toolkit.KryptonGroupBox groupBoxListadoPromociones;
        private Krypton.Toolkit.KryptonLabel lblDescripcionPromocion;
        private Krypton.Toolkit.KryptonTextBox txtbDescripcionPromocion;
        private Krypton.Toolkit.KryptonLabel lblDescuentoPromocion;
        private Krypton.Toolkit.KryptonTextBox txtbDescuentoPromocion;
        private Krypton.Toolkit.KryptonLabel lblTopePromocion;
        private Krypton.Toolkit.KryptonTextBox txtbTopePromocion;
        private Krypton.Toolkit.KryptonLabel lblVigenciaDesdePromocion;
        private TpIDS.DateTimePickerAccesible dtpVigenciaDesdePromocion;
        private Krypton.Toolkit.KryptonLabel lblVigenciaHastaPromocion;
        private TpIDS.DateTimePickerAccesible dtpVigenciaHastaPromocion;
        private Krypton.Toolkit.KryptonLabel lblDiasPromocion;
        private TpIDS.CheckBoxAccesible chkbDiaLunes;
        private TpIDS.CheckBoxAccesible chkbDiaMartes;
        private TpIDS.CheckBoxAccesible chkbDiaMiercoles;
        private TpIDS.CheckBoxAccesible chkbDiaJueves;
        private TpIDS.CheckBoxAccesible chkbDiaViernes;
        private TpIDS.CheckBoxAccesible chkbDiaSabado;
        private TpIDS.CheckBoxAccesible chkbDiaDomingo;
        private System.Windows.Forms.FlowLayoutPanel flpDiasPromocion;
        private System.Windows.Forms.TableLayoutPanel tlpDatosPromocion;
        private Krypton.Toolkit.KryptonLabel lblLeyendaFiltrosPromocion;
        private Krypton.Toolkit.KryptonLabel lblAdvertenciaCategoriaExcluida;
        private Krypton.Toolkit.KryptonLabel lblFiltroComercios;
        private System.Windows.Forms.Panel pnlMarcoComercios;
        private System.Windows.Forms.CheckedListBox clbComercios;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioComercios;
        private Krypton.Toolkit.KryptonLabel lblFiltroRubrosComercio;
        private System.Windows.Forms.Panel pnlMarcoRubrosComercio;
        private System.Windows.Forms.CheckedListBox clbRubrosComercio;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioRubrosComercio;
        private Krypton.Toolkit.KryptonLabel lblFiltroEntidadesFinancieras;
        private System.Windows.Forms.Panel pnlMarcoEntidadesFinancieras;
        private System.Windows.Forms.CheckedListBox clbEntidadesFinancieras;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioEntidadesFinancieras;
        private Krypton.Toolkit.KryptonLabel lblFiltroMediosPago;
        private System.Windows.Forms.Panel pnlMarcoMediosPago;
        private System.Windows.Forms.CheckedListBox clbMediosPago;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioMediosPago;
        private Krypton.Toolkit.KryptonLabel lblFiltroSegmentos;
        private System.Windows.Forms.Panel pnlMarcoSegmentos;
        private System.Windows.Forms.CheckedListBox clbSegmentos;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioSegmentos;
        private Krypton.Toolkit.KryptonLabel lblFiltroProductosIncluidos;
        private System.Windows.Forms.Panel pnlMarcoProductosIncluidos;
        private System.Windows.Forms.CheckedListBox clbProductosIncluidos;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioProductosIncluidos;
        private Krypton.Toolkit.KryptonLabel lblFiltroProductosExcluidos;
        private System.Windows.Forms.Panel pnlMarcoProductosExcluidos;
        private System.Windows.Forms.CheckedListBox clbProductosExcluidos;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioProductosExcluidos;
        private Krypton.Toolkit.KryptonLabel lblFiltroCategoriasIncluidas;
        private System.Windows.Forms.Panel pnlMarcoCategoriasIncluidas;
        private System.Windows.Forms.CheckedListBox clbCategoriasIncluidas;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioCategoriasIncluidas;
        private Krypton.Toolkit.KryptonLabel lblFiltroCategoriasExcluidas;
        private System.Windows.Forms.Panel pnlMarcoCategoriasExcluidas;
        private System.Windows.Forms.CheckedListBox clbCategoriasExcluidas;
        private Krypton.Toolkit.KryptonWrapLabel lblVacioCategoriasExcluidas;
        private System.Windows.Forms.TableLayoutPanel tlpFiltrosPromocion;
        private Krypton.Toolkit.KryptonPanel pnlEditorPromocion;
        private Krypton.Toolkit.KryptonButton btnAgregarPromocion;
        private Krypton.Toolkit.KryptonButton btnActualizarPromocion;
        private Krypton.Toolkit.KryptonButton btnEliminarPromocion;
        private Krypton.Toolkit.KryptonButton btnLimpiarPromocion;
        private Krypton.Toolkit.KryptonGroupBox groupBoxEditorPromocion;
        private System.Windows.Forms.TableLayoutPanel tlpPromociones;
    }
}
