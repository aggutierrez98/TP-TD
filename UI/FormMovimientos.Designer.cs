namespace TpIDS
{
    partial class FormMovimientos
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
            this.lblTituloMovimientos = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxFiltros = new Krypton.Toolkit.KryptonGroupBox();
            this.btnBuscarMovimientos = new Krypton.Toolkit.KryptonButton();
            this.lblDescripcionFiltro = new Krypton.Toolkit.KryptonLabel();
            this.txtbDescripcionFiltro = new Krypton.Toolkit.KryptonTextBox();
            this.tlpSeleccionMovimientos = new System.Windows.Forms.TableLayoutPanel();
            this.btnSeleccionarTodosMovimientos = new Krypton.Toolkit.KryptonButton();
            this.btnDesmarcarTodosMovimientos = new Krypton.Toolkit.KryptonButton();
            this.lblSeleccionMovimientos = new Krypton.Toolkit.KryptonLabel();
            this.btnExportarMovimientosPdf = new Krypton.Toolkit.KryptonButton();
            this.colMovimientosSeleccionar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cmbTipoFiltro = new Krypton.Toolkit.KryptonComboBox();
            this.lblTipoFiltro = new Krypton.Toolkit.KryptonLabel();
            this.cmbCategoriaFiltro = new Krypton.Toolkit.KryptonComboBox();
            this.lblCategoriaFiltro = new Krypton.Toolkit.KryptonLabel();
            this.dtpFechaHastaMovimientos = new TpIDS.DateTimePickerAccesible();
            this.chkbFechaHastaMovimientos = new TpIDS.CheckBoxAccesible();
            this.dtpFechaDesdeMovimientos = new TpIDS.DateTimePickerAccesible();
            this.chkbFechaDesdeMovimientos = new TpIDS.CheckBoxAccesible();
            this.dgvMovimientos = new Krypton.Toolkit.KryptonDataGridView();
            this.colMovimientosFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosComercio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosMonto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxEdicion = new Krypton.Toolkit.KryptonGroupBox();
            this.btnSugerirCategoriaMovimiento = new Krypton.Toolkit.KryptonButton();
            this.cmbComercioMovimiento = new Krypton.Toolkit.KryptonComboBox();
            this.lblComercioMovimiento = new Krypton.Toolkit.KryptonLabel();
            this.btnEliminarMovimiento = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarMovimiento = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarMovimiento = new Krypton.Toolkit.KryptonButton();
            this.txtbDescripcionMovimiento = new Krypton.Toolkit.KryptonTextBox();
            this.lblDescripcionMovimiento = new Krypton.Toolkit.KryptonLabel();
            this.dtpFechaMovimiento = new TpIDS.DateTimePickerAccesible();
            this.lblFechaMovimiento = new Krypton.Toolkit.KryptonLabel();
            this.txtbMontoMovimiento = new Krypton.Toolkit.KryptonTextBox();
            this.lblMontoMovimiento = new Krypton.Toolkit.KryptonLabel();
            this.cmbTipoMovimiento = new Krypton.Toolkit.KryptonComboBox();
            this.lblTipoMovimiento = new Krypton.Toolkit.KryptonLabel();
            this.cmbCategoriaMovimiento = new Krypton.Toolkit.KryptonComboBox();
            this.lblCategoriaMovimiento = new Krypton.Toolkit.KryptonLabel();
            this.btnEscanearTicket = new Krypton.Toolkit.KryptonButton();
            this.pnlEscaneoTicket = new System.Windows.Forms.Panel();
            this.prgbEscaneoTicket = new System.Windows.Forms.ProgressBar();
            this.lblEscaneoTicketEnCurso = new Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltros)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltros.Panel)).BeginInit();
            this.groupBoxFiltros.Panel.SuspendLayout();
            this.groupBoxFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEdicion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEdicion.Panel)).BeginInit();
            this.groupBoxEdicion.Panel.SuspendLayout();
            this.groupBoxEdicion.SuspendLayout();
            this.pnlEscaneoTicket.SuspendLayout();
            this.tlpFechasFiltro = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFechasFiltro.SuspendLayout();
            this.tlpCamposFiltro = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposFiltro.SuspendLayout();
            this.tlpCamposMovimiento = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposMovimiento.SuspendLayout();
            this.tlpComercioMovimiento = new System.Windows.Forms.TableLayoutPanel();
            this.tlpComercioMovimiento.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloMovimientos
            //
            this.lblTituloMovimientos.AutoSize = true;
            this.lblTituloMovimientos.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloMovimientos.Location = new System.Drawing.Point(18, 15);
            this.lblTituloMovimientos.Name = "lblTituloMovimientos";
            this.lblTituloMovimientos.Size = new System.Drawing.Size(230, 25);
            this.lblTituloMovimientos.TabIndex = 0;
            this.lblTituloMovimientos.Tag = "FormMovements.TitleLabel";
            this.lblTituloMovimientos.Values.Text = "Gestion de movimientos";
            //
            // groupBoxFiltros
            //
            this.groupBoxFiltros.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFiltros.Panel.Controls.Add(this.btnBuscarMovimientos);
            this.groupBoxFiltros.Panel.Controls.Add(this.lblDescripcionFiltro);
            this.groupBoxFiltros.Panel.Controls.Add(this.txtbDescripcionFiltro);
            this.groupBoxFiltros.Panel.Controls.Add(this.tlpCamposFiltro);
            this.groupBoxFiltros.Panel.Controls.Add(this.tlpFechasFiltro);
            this.groupBoxFiltros.Location = new System.Drawing.Point(23, 52);
            this.groupBoxFiltros.Name = "groupBoxFiltros";
            this.groupBoxFiltros.Size = new System.Drawing.Size(714, 140);
            this.groupBoxFiltros.TabIndex = 1;
            this.groupBoxFiltros.TabStop = false;
            this.groupBoxFiltros.Tag = "FormMovements.FiltersGroup";
            this.groupBoxFiltros.Values.Heading = "Filtros";
            //
            // btnBuscarMovimientos
            //
            this.btnBuscarMovimientos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarMovimientos.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnBuscarMovimientos.Location = new System.Drawing.Point(590, 78);
            this.btnBuscarMovimientos.Name = "btnBuscarMovimientos";
            this.btnBuscarMovimientos.Size = new System.Drawing.Size(110, 27);
            this.btnBuscarMovimientos.TabIndex = 8;
            this.btnBuscarMovimientos.Tag = "FormMovements.SearchButton";
            this.btnBuscarMovimientos.Values.Text = "Buscar";
            this.btnBuscarMovimientos.Click += new System.EventHandler(this.btnBuscarMovimientos_Click);
            //
            // Busqueda por descripcion
            //
            this.lblDescripcionFiltro.Name = "lblDescripcionFiltro";
            this.lblDescripcionFiltro.Location = new System.Drawing.Point(14, 80);
            this.lblDescripcionFiltro.AutoSize = true;
            this.lblDescripcionFiltro.Tag = "FormMovements.DescriptionFilterLabel";
            this.lblDescripcionFiltro.Values.Text = "Descripción:";
            this.txtbDescripcionFiltro.Name = "txtbDescripcionFiltro";
            this.txtbDescripcionFiltro.Location = new System.Drawing.Point(110, 78);
            this.txtbDescripcionFiltro.Size = new System.Drawing.Size(460, 27);
            this.txtbDescripcionFiltro.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtbDescripcionFiltro.TabIndex = 7;
            this.txtbDescripcionFiltro.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbDescripcionFiltro_KeyDown);
            //
            // cmbTipoFiltro
            //
            this.cmbTipoFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTipoFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoFiltro.FormattingEnabled = true;
            this.cmbTipoFiltro.Location = new System.Drawing.Point(218, 20);
            this.cmbTipoFiltro.Name = "cmbTipoFiltro";
            this.cmbTipoFiltro.Size = new System.Drawing.Size(140, 21);
            this.cmbTipoFiltro.TabIndex = 7;
            this.cmbTipoFiltro.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblTipoFiltro
            //
            this.lblTipoFiltro.AutoSize = true;
            this.lblTipoFiltro.Location = new System.Drawing.Point(215, 0);
            this.lblTipoFiltro.Name = "lblTipoFiltro";
            this.lblTipoFiltro.Size = new System.Drawing.Size(33, 13);
            this.lblTipoFiltro.TabIndex = 6;
            this.lblTipoFiltro.Tag = "FormMovements.TypeLabel";
            this.lblTipoFiltro.Values.Text = "Tipo:";
            this.lblTipoFiltro.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbCategoriaFiltro
            //
            this.cmbCategoriaFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategoriaFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaFiltro.FormattingEnabled = true;
            this.cmbCategoriaFiltro.Location = new System.Drawing.Point(3, 20);
            this.cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            this.cmbCategoriaFiltro.Size = new System.Drawing.Size(190, 21);
            this.cmbCategoriaFiltro.TabIndex = 5;
            this.cmbCategoriaFiltro.Margin = new System.Windows.Forms.Padding(3, 0, 22, 0);
            //
            // lblCategoriaFiltro
            //
            this.lblCategoriaFiltro.AutoSize = true;
            this.lblCategoriaFiltro.Location = new System.Drawing.Point(0, 0);
            this.lblCategoriaFiltro.Name = "lblCategoriaFiltro";
            this.lblCategoriaFiltro.Size = new System.Drawing.Size(58, 13);
            this.lblCategoriaFiltro.TabIndex = 4;
            this.lblCategoriaFiltro.Tag = "FormMovements.CategoryLabel";
            this.lblCategoriaFiltro.Values.Text = "Categoria:";
            this.lblCategoriaFiltro.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dtpFechaHastaMovimientos
            //
            this.dtpFechaHastaMovimientos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpFechaHastaMovimientos.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaHastaMovimientos.Location = new System.Drawing.Point(304, 0);
            this.dtpFechaHastaMovimientos.Name = "dtpFechaHastaMovimientos";
            this.dtpFechaHastaMovimientos.Size = new System.Drawing.Size(90, 20);
            this.dtpFechaHastaMovimientos.TabIndex = 3;
            this.dtpFechaHastaMovimientos.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // chkbFechaHastaMovimientos
            //
            this.chkbFechaHastaMovimientos.AutoSize = true;
            this.chkbFechaHastaMovimientos.Location = new System.Drawing.Point(203, 2);
            this.chkbFechaHastaMovimientos.Name = "chkbFechaHastaMovimientos";
            this.chkbFechaHastaMovimientos.Size = new System.Drawing.Size(95, 17);
            this.chkbFechaHastaMovimientos.TabIndex = 2;
            this.chkbFechaHastaMovimientos.Tag = "FormMovements.ToDateCheck";
            this.chkbFechaHastaMovimientos.Values.Text = "Fecha hasta:";
            this.chkbFechaHastaMovimientos.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            //
            // dtpFechaDesdeMovimientos
            //
            this.dtpFechaDesdeMovimientos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpFechaDesdeMovimientos.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaDesdeMovimientos.Location = new System.Drawing.Point(106, 0);
            this.dtpFechaDesdeMovimientos.Name = "dtpFechaDesdeMovimientos";
            this.dtpFechaDesdeMovimientos.Size = new System.Drawing.Size(90, 20);
            this.dtpFechaDesdeMovimientos.TabIndex = 1;
            this.dtpFechaDesdeMovimientos.Margin = new System.Windows.Forms.Padding(0, 0, 7, 0);
            //
            // chkbFechaDesdeMovimientos
            //
            this.chkbFechaDesdeMovimientos.AutoSize = true;
            this.chkbFechaDesdeMovimientos.Location = new System.Drawing.Point(0, 2);
            this.chkbFechaDesdeMovimientos.Name = "chkbFechaDesdeMovimientos";
            this.chkbFechaDesdeMovimientos.Size = new System.Drawing.Size(100, 17);
            this.chkbFechaDesdeMovimientos.TabIndex = 0;
            this.chkbFechaDesdeMovimientos.Tag = "FormMovements.FromDateCheck";
            this.chkbFechaDesdeMovimientos.Values.Text = "Fecha desde:";
            this.chkbFechaDesdeMovimientos.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            //
            // dgvMovimientos
            //
            this.dgvMovimientos.AllowUserToAddRows = false;
            this.dgvMovimientos.AllowUserToDeleteRows = false;
            this.dgvMovimientos.AutoGenerateColumns = false;
            this.dgvMovimientos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMovimientos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMovimientos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMovimientosSeleccionar,
            this.colMovimientosFecha,
            this.colMovimientosTipo,
            this.colMovimientosCategoria,
            this.colMovimientosComercio,
            this.colMovimientosMonto,
            this.colMovimientosDescripcion});
            this.dgvMovimientos.Location = new System.Drawing.Point(23, 192);
            this.dgvMovimientos.MultiSelect = false;
            this.dgvMovimientos.Name = "dgvMovimientos";
            this.dgvMovimientos.ReadOnly = false;
            this.dgvMovimientos.RowHeadersVisible = false;
            this.dgvMovimientos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMovimientos.Size = new System.Drawing.Size(714, 220);
            this.dgvMovimientos.TabIndex = 2;
            this.dgvMovimientos.Tag = "FormMovements.Grid";
            this.dgvMovimientos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMovimientos_CellClick);
            this.dgvMovimientos.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvMovimientos_CurrentCellDirtyStateChanged);
            this.dgvMovimientos.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMovimientos_CellValueChanged);
            this.colMovimientosSeleccionar.Name = "Seleccionar";
            this.colMovimientosSeleccionar.HeaderText = "Exportar";
            this.colMovimientosSeleccionar.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colMovimientosSeleccionar.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // Acciones de seleccion/exportacion, debajo de la grilla
            //
            this.tlpSeleccionMovimientos.Name = "tlpSeleccionMovimientos";
            this.tlpSeleccionMovimientos.Location = new System.Drawing.Point(23, 420);
            this.tlpSeleccionMovimientos.Size = new System.Drawing.Size(714, 32);
            this.tlpSeleccionMovimientos.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tlpSeleccionMovimientos.ColumnCount = 4;
            this.tlpSeleccionMovimientos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpSeleccionMovimientos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpSeleccionMovimientos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSeleccionMovimientos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tlpSeleccionMovimientos.RowCount = 1;
            this.tlpSeleccionMovimientos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSeleccionMovimientos.TabIndex = 3;
            this.btnSeleccionarTodosMovimientos.Name = "btnSeleccionarTodosMovimientos";
            this.btnSeleccionarTodosMovimientos.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnSeleccionarTodosMovimientos.Size = new System.Drawing.Size(144, 27);
            this.btnSeleccionarTodosMovimientos.Tag = "FormMovements.SelectAllButton";
            this.btnSeleccionarTodosMovimientos.Values.Text = "Seleccionar todos";
            this.btnSeleccionarTodosMovimientos.Click += new System.EventHandler(this.btnSeleccionarTodosMovimientos_Click);
            this.btnDesmarcarTodosMovimientos.Name = "btnDesmarcarTodosMovimientos";
            this.btnDesmarcarTodosMovimientos.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnDesmarcarTodosMovimientos.Size = new System.Drawing.Size(144, 27);
            this.btnDesmarcarTodosMovimientos.Tag = "FormMovements.DeselectAllButton";
            this.btnDesmarcarTodosMovimientos.Values.Text = "Desmarcar todos";
            this.btnDesmarcarTodosMovimientos.Click += new System.EventHandler(this.btnDesmarcarTodosMovimientos_Click);
            this.lblSeleccionMovimientos.Name = "lblSeleccionMovimientos";
            this.lblSeleccionMovimientos.AutoSize = true;
            this.lblSeleccionMovimientos.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSeleccionMovimientos.Values.Text = "0 de 0 seleccionados";
            this.btnExportarMovimientosPdf.Name = "btnExportarMovimientosPdf";
            this.btnExportarMovimientosPdf.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnExportarMovimientosPdf.Size = new System.Drawing.Size(139, 27);
            this.btnExportarMovimientosPdf.Tag = "FormMovements.ExportPdfButton";
            this.btnExportarMovimientosPdf.Values.Text = "Exportar a PDF";
            this.btnExportarMovimientosPdf.Enabled = false;
            this.btnExportarMovimientosPdf.Click += new System.EventHandler(this.btnExportarMovimientosPdf_Click);
            this.tlpSeleccionMovimientos.Controls.Add(this.btnSeleccionarTodosMovimientos, 0, 0);
            this.tlpSeleccionMovimientos.Controls.Add(this.btnDesmarcarTodosMovimientos, 1, 0);
            this.tlpSeleccionMovimientos.Controls.Add(this.lblSeleccionMovimientos, 2, 0);
            this.tlpSeleccionMovimientos.Controls.Add(this.btnExportarMovimientosPdf, 3, 0);
            //
            // colMovimientosFecha
            //
            this.colMovimientosFecha.DataPropertyName = "Fecha";
            this.colMovimientosFecha.HeaderText = "Fecha";
            this.colMovimientosFecha.Name = "Fecha";
            this.colMovimientosFecha.ReadOnly = true;
            //
            // colMovimientosTipo
            //
            this.colMovimientosTipo.DataPropertyName = "Tipo";
            this.colMovimientosTipo.HeaderText = "Tipo";
            this.colMovimientosTipo.Name = "Tipo";
            this.colMovimientosTipo.ReadOnly = true;
            //
            // colMovimientosCategoria
            //
            this.colMovimientosCategoria.DataPropertyName = "Categoria";
            this.colMovimientosCategoria.HeaderText = "Categoria";
            this.colMovimientosCategoria.Name = "Categoria";
            this.colMovimientosCategoria.ReadOnly = true;
            //
            // colMovimientosComercio
            //
            this.colMovimientosComercio.DataPropertyName = "Comercio";
            this.colMovimientosComercio.HeaderText = "Comercio";
            this.colMovimientosComercio.Name = "Comercio";
            this.colMovimientosComercio.ReadOnly = true;
            //
            // colMovimientosMonto
            //
            this.colMovimientosMonto.DataPropertyName = "Monto";
            this.colMovimientosMonto.HeaderText = "Monto";
            this.colMovimientosMonto.Name = "Monto";
            this.colMovimientosMonto.ReadOnly = true;
            //
            // colMovimientosDescripcion
            //
            this.colMovimientosDescripcion.DataPropertyName = "Descripcion";
            this.colMovimientosDescripcion.HeaderText = "Descripcion";
            this.colMovimientosDescripcion.Name = "Descripcion";
            this.colMovimientosDescripcion.ReadOnly = true;
            //
            // groupBoxEdicion
            //
            this.groupBoxEdicion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEdicion.Panel.Controls.Add(this.tlpComercioMovimiento);
            this.groupBoxEdicion.Panel.Controls.Add(this.btnEscanearTicket);
            this.groupBoxEdicion.Panel.Controls.Add(this.btnEliminarMovimiento);
            this.groupBoxEdicion.Panel.Controls.Add(this.btnActualizarMovimiento);
            this.groupBoxEdicion.Panel.Controls.Add(this.btnAgregarMovimiento);
            this.groupBoxEdicion.Panel.Controls.Add(this.tlpCamposMovimiento);
            this.groupBoxEdicion.Location = new System.Drawing.Point(23, 460);
            this.groupBoxEdicion.Name = "groupBoxEdicion";
            this.groupBoxEdicion.Size = new System.Drawing.Size(714, 163);
            this.groupBoxEdicion.TabIndex = 3;
            this.groupBoxEdicion.TabStop = false;
            this.groupBoxEdicion.Tag = "FormMovements.MovementGroup";
            this.groupBoxEdicion.Values.Heading = "Datos del movimiento";
            //
            // btnEliminarMovimiento
            //
            this.btnEliminarMovimiento.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarMovimiento.Location = new System.Drawing.Point(334, 104);
            this.btnEliminarMovimiento.Name = "btnEliminarMovimiento";
            this.btnEliminarMovimiento.Size = new System.Drawing.Size(150, 30);
            this.btnEliminarMovimiento.TabIndex = 15;
            this.btnEliminarMovimiento.Tag = "FormMovements.DeleteButton";
            this.btnEliminarMovimiento.Values.Text = "Eliminar movimiento";
            this.btnEliminarMovimiento.Click += new System.EventHandler(this.btnEliminarMovimiento_Click);
            //
            // btnActualizarMovimiento
            //
            this.btnActualizarMovimiento.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarMovimiento.Location = new System.Drawing.Point(174, 104);
            this.btnActualizarMovimiento.Name = "btnActualizarMovimiento";
            this.btnActualizarMovimiento.Size = new System.Drawing.Size(150, 30);
            this.btnActualizarMovimiento.TabIndex = 14;
            this.btnActualizarMovimiento.Tag = "FormMovements.UpdateButton";
            this.btnActualizarMovimiento.Values.Text = "Actualizar movimiento";
            this.btnActualizarMovimiento.Click += new System.EventHandler(this.btnActualizarMovimiento_Click);
            //
            // btnAgregarMovimiento
            //
            this.btnAgregarMovimiento.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarMovimiento.Location = new System.Drawing.Point(14, 104);
            this.btnAgregarMovimiento.Name = "btnAgregarMovimiento";
            this.btnAgregarMovimiento.Size = new System.Drawing.Size(150, 30);
            this.btnAgregarMovimiento.TabIndex = 13;
            this.btnAgregarMovimiento.Tag = "FormMovements.AddButton";
            this.btnAgregarMovimiento.Values.Text = "Agregar movimiento";
            this.btnAgregarMovimiento.Click += new System.EventHandler(this.btnAgregarMovimiento_Click);
            //
            // btnEscanearTicket
            //
            this.btnEscanearTicket.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEscanearTicket.Location = new System.Drawing.Point(494, 104);
            this.btnEscanearTicket.Name = "btnEscanearTicket";
            this.btnEscanearTicket.Size = new System.Drawing.Size(150, 30);
            this.btnEscanearTicket.TabIndex = 16;
            this.btnEscanearTicket.Tag = "FormMovements.ScanButton";
            this.btnEscanearTicket.Values.Text = "Escanear ticket";
            this.btnEscanearTicket.Click += new System.EventHandler(this.btnEscanearTicket_Click);
            //
            // pnlEscaneoTicket
            //
            this.pnlEscaneoTicket.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlEscaneoTicket.BackColor = System.Drawing.SystemColors.Control;
            this.pnlEscaneoTicket.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlEscaneoTicket.Controls.Add(this.prgbEscaneoTicket);
            this.pnlEscaneoTicket.Controls.Add(this.lblEscaneoTicketEnCurso);
            this.pnlEscaneoTicket.Location = new System.Drawing.Point(190, 215);
            this.pnlEscaneoTicket.Name = "pnlEscaneoTicket";
            this.pnlEscaneoTicket.Size = new System.Drawing.Size(380, 92);
            this.pnlEscaneoTicket.TabIndex = 4;
            this.pnlEscaneoTicket.Visible = false;
            //
            // prgbEscaneoTicket
            //
            this.prgbEscaneoTicket.Location = new System.Drawing.Point(25, 51);
            this.prgbEscaneoTicket.MarqueeAnimationSpeed = 30;
            this.prgbEscaneoTicket.Name = "prgbEscaneoTicket";
            this.prgbEscaneoTicket.Size = new System.Drawing.Size(328, 18);
            this.prgbEscaneoTicket.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgbEscaneoTicket.TabIndex = 1;
            //
            // lblEscaneoTicketEnCurso
            //
            this.lblEscaneoTicketEnCurso.AutoSize = true;
            this.lblEscaneoTicketEnCurso.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEscaneoTicketEnCurso.Location = new System.Drawing.Point(116, 19);
            this.lblEscaneoTicketEnCurso.Name = "lblEscaneoTicketEnCurso";
            this.lblEscaneoTicketEnCurso.Size = new System.Drawing.Size(143, 19);
            this.lblEscaneoTicketEnCurso.TabIndex = 0;
            this.lblEscaneoTicketEnCurso.Tag = "FormMovements.Scan.ProgressLabel";
            this.lblEscaneoTicketEnCurso.Values.Text = "Analizando ticket...";
            // 
            // btnSugerirCategoriaMovimiento
            // 
            this.btnSugerirCategoriaMovimiento.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnSugerirCategoriaMovimiento.Location = new System.Drawing.Point(579, 18);
            this.btnSugerirCategoriaMovimiento.Name = "btnSugerirCategoriaMovimiento";
            this.btnSugerirCategoriaMovimiento.Size = new System.Drawing.Size(110, 25);
            this.btnSugerirCategoriaMovimiento.TabIndex = 7;
            this.btnSugerirCategoriaMovimiento.Tag = "FormMovements.SuggestCategoryButton";
            this.btnSugerirCategoriaMovimiento.Values.Text = "Sugerir cat.";
            this.btnSugerirCategoriaMovimiento.Click += new System.EventHandler(this.btnSugerirCategoriaMovimiento_Click);
            this.btnSugerirCategoriaMovimiento.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // cmbComercioMovimiento
            // 
            this.cmbComercioMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbComercioMovimiento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComercioMovimiento.FormattingEnabled = true;
            this.cmbComercioMovimiento.Location = new System.Drawing.Point(3, 20);
            this.cmbComercioMovimiento.Name = "cmbComercioMovimiento";
            this.cmbComercioMovimiento.Size = new System.Drawing.Size(190, 21);
            this.cmbComercioMovimiento.TabIndex = 5;
            this.cmbComercioMovimiento.Margin = new System.Windows.Forms.Padding(3, 2, 13, 0);
            // 
            // lblComercioMovimiento
            // 
            this.lblComercioMovimiento.AutoSize = true;
            this.lblComercioMovimiento.Location = new System.Drawing.Point(0, 0);
            this.lblComercioMovimiento.Name = "lblComercioMovimiento";
            this.lblComercioMovimiento.Size = new System.Drawing.Size(107, 13);
            this.lblComercioMovimiento.TabIndex = 4;
            this.lblComercioMovimiento.Tag = "FormMovements.CommerceLabel";
            this.lblComercioMovimiento.Values.Text = "Comercio (opcional):";
            this.lblComercioMovimiento.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // txtbDescripcionMovimiento
            // 
            this.txtbDescripcionMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbDescripcionMovimiento.Location = new System.Drawing.Point(209, 20);
            this.txtbDescripcionMovimiento.Name = "txtbDescripcionMovimiento";
            this.txtbDescripcionMovimiento.Size = new System.Drawing.Size(355, 20);
            this.txtbDescripcionMovimiento.TabIndex = 6;
            this.txtbDescripcionMovimiento.Leave += new System.EventHandler(this.txtbDescripcionMovimiento_Leave);
            this.txtbDescripcionMovimiento.Margin = new System.Windows.Forms.Padding(3, 2, 15, 0);
            // 
            // lblDescripcionMovimiento
            // 
            this.lblDescripcionMovimiento.AutoSize = true;
            this.lblDescripcionMovimiento.Location = new System.Drawing.Point(206, 0);
            this.lblDescripcionMovimiento.Name = "lblDescripcionMovimiento";
            this.lblDescripcionMovimiento.Size = new System.Drawing.Size(66, 13);
            this.lblDescripcionMovimiento.TabIndex = 5;
            this.lblDescripcionMovimiento.Tag = "FormMovements.DescriptionLabel";
            this.lblDescripcionMovimiento.Values.Text = "Descripcion:";
            this.lblDescripcionMovimiento.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // dtpFechaMovimiento
            // 
            this.dtpFechaMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpFechaMovimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaMovimiento.Location = new System.Drawing.Point(529, 15);
            this.dtpFechaMovimiento.Name = "dtpFechaMovimiento";
            this.dtpFechaMovimiento.Size = new System.Drawing.Size(160, 20);
            this.dtpFechaMovimiento.TabIndex = 3;
            this.dtpFechaMovimiento.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // lblFechaMovimiento
            // 
            this.lblFechaMovimiento.AutoSize = true;
            this.lblFechaMovimiento.Location = new System.Drawing.Point(526, 0);
            this.lblFechaMovimiento.Name = "lblFechaMovimiento";
            this.lblFechaMovimiento.Size = new System.Drawing.Size(41, 13);
            this.lblFechaMovimiento.TabIndex = 2;
            this.lblFechaMovimiento.Tag = "FormMovements.DateLabel";
            this.lblFechaMovimiento.Values.Text = "Fecha:";
            this.lblFechaMovimiento.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // txtbMontoMovimiento
            // 
            this.txtbMontoMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbMontoMovimiento.Location = new System.Drawing.Point(369, 15);
            this.txtbMontoMovimiento.Name = "txtbMontoMovimiento";
            this.txtbMontoMovimiento.Size = new System.Drawing.Size(140, 20);
            this.txtbMontoMovimiento.TabIndex = 2;
            this.txtbMontoMovimiento.Margin = new System.Windows.Forms.Padding(3, 0, 17, 0);
            // 
            // lblMontoMovimiento
            // 
            this.lblMontoMovimiento.AutoSize = true;
            this.lblMontoMovimiento.Location = new System.Drawing.Point(366, 0);
            this.lblMontoMovimiento.Name = "lblMontoMovimiento";
            this.lblMontoMovimiento.Size = new System.Drawing.Size(41, 13);
            this.lblMontoMovimiento.TabIndex = 1;
            this.lblMontoMovimiento.Tag = "FormMovements.AmountLabel";
            this.lblMontoMovimiento.Values.Text = "Monto:";
            this.lblMontoMovimiento.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // cmbTipoMovimiento
            // 
            this.cmbTipoMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTipoMovimiento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoMovimiento.FormattingEnabled = true;
            this.cmbTipoMovimiento.Location = new System.Drawing.Point(209, 15);
            this.cmbTipoMovimiento.Name = "cmbTipoMovimiento";
            this.cmbTipoMovimiento.Size = new System.Drawing.Size(140, 21);
            this.cmbTipoMovimiento.TabIndex = 1;
            this.cmbTipoMovimiento.Margin = new System.Windows.Forms.Padding(3, 0, 17, 0);
            // 
            // lblTipoMovimiento
            // 
            this.lblTipoMovimiento.AutoSize = true;
            this.lblTipoMovimiento.Location = new System.Drawing.Point(206, 0);
            this.lblTipoMovimiento.Name = "lblTipoMovimiento";
            this.lblTipoMovimiento.Size = new System.Drawing.Size(33, 13);
            this.lblTipoMovimiento.TabIndex = 0;
            this.lblTipoMovimiento.Tag = "FormMovements.TypeLabel";
            this.lblTipoMovimiento.Values.Text = "Tipo:";
            this.lblTipoMovimiento.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // cmbCategoriaMovimiento
            // 
            this.cmbCategoriaMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategoriaMovimiento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaMovimiento.FormattingEnabled = true;
            this.cmbCategoriaMovimiento.Location = new System.Drawing.Point(3, 15);
            this.cmbCategoriaMovimiento.Name = "cmbCategoriaMovimiento";
            this.cmbCategoriaMovimiento.Size = new System.Drawing.Size(190, 21);
            this.cmbCategoriaMovimiento.TabIndex = 0;
            this.cmbCategoriaMovimiento.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
            // 
            // lblCategoriaMovimiento
            // 
            this.lblCategoriaMovimiento.AutoSize = true;
            this.lblCategoriaMovimiento.Location = new System.Drawing.Point(0, 0);
            this.lblCategoriaMovimiento.Name = "lblCategoriaMovimiento";
            this.lblCategoriaMovimiento.Size = new System.Drawing.Size(58, 13);
            this.lblCategoriaMovimiento.TabIndex = 0;
            this.lblCategoriaMovimiento.Tag = "FormMovements.CategoryLabel";
            this.lblCategoriaMovimiento.Values.Text = "Categoria:";
            this.lblCategoriaMovimiento.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // tlpFechasFiltro
            // 
            this.tlpFechasFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpFechasFiltro.Controls.Add(this.dtpFechaHastaMovimientos, 3, 0);
            this.tlpFechasFiltro.Controls.Add(this.chkbFechaHastaMovimientos, 2, 0);
            this.tlpFechasFiltro.Controls.Add(this.dtpFechaDesdeMovimientos, 1, 0);
            this.tlpFechasFiltro.Controls.Add(this.chkbFechaDesdeMovimientos, 0, 0);
            this.tlpFechasFiltro.Location = new System.Drawing.Point(14, 6);
            this.tlpFechasFiltro.Name = "tlpFechasFiltro";
            this.tlpFechasFiltro.Size = new System.Drawing.Size(394, 23);
            this.tlpFechasFiltro.ColumnCount = 4;
            this.tlpFechasFiltro.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tlpFechasFiltro.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.87F));
            this.tlpFechasFiltro.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.tlpFechasFiltro.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.13F));
            this.tlpFechasFiltro.RowCount = 1;
            this.tlpFechasFiltro.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpFechasFiltro.TabIndex = 0;
            // 
            // tlpCamposFiltro
            // 
            this.tlpCamposFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposFiltro.Controls.Add(this.cmbTipoFiltro, 1, 1);
            this.tlpCamposFiltro.Controls.Add(this.lblTipoFiltro, 1, 0);
            this.tlpCamposFiltro.Controls.Add(this.cmbCategoriaFiltro, 0, 1);
            this.tlpCamposFiltro.Controls.Add(this.lblCategoriaFiltro, 0, 0);
            this.tlpCamposFiltro.Location = new System.Drawing.Point(212, 27);
            this.tlpCamposFiltro.Name = "tlpCamposFiltro";
            this.tlpCamposFiltro.Size = new System.Drawing.Size(358, 45);
            this.tlpCamposFiltro.ColumnCount = 2;
            this.tlpCamposFiltro.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.06F));
            this.tlpCamposFiltro.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.94F));
            this.tlpCamposFiltro.RowCount = 2;
            this.tlpCamposFiltro.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposFiltro.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposFiltro.TabIndex = 4;
            // 
            // tlpCamposMovimiento
            // 
            this.tlpCamposMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposMovimiento.Controls.Add(this.dtpFechaMovimiento, 3, 1);
            this.tlpCamposMovimiento.Controls.Add(this.lblFechaMovimiento, 3, 0);
            this.tlpCamposMovimiento.Controls.Add(this.txtbMontoMovimiento, 2, 1);
            this.tlpCamposMovimiento.Controls.Add(this.lblMontoMovimiento, 2, 0);
            this.tlpCamposMovimiento.Controls.Add(this.cmbTipoMovimiento, 1, 1);
            this.tlpCamposMovimiento.Controls.Add(this.lblTipoMovimiento, 1, 0);
            this.tlpCamposMovimiento.Controls.Add(this.cmbCategoriaMovimiento, 0, 1);
            this.tlpCamposMovimiento.Controls.Add(this.lblCategoriaMovimiento, 0, 0);
            this.tlpCamposMovimiento.Location = new System.Drawing.Point(11, 0);
            this.tlpCamposMovimiento.Name = "tlpCamposMovimiento";
            this.tlpCamposMovimiento.Size = new System.Drawing.Size(689, 40);
            this.tlpCamposMovimiento.ColumnCount = 4;
            this.tlpCamposMovimiento.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.9F));
            this.tlpCamposMovimiento.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.22F));
            this.tlpCamposMovimiento.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.22F));
            this.tlpCamposMovimiento.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.66F));
            this.tlpCamposMovimiento.RowCount = 2;
            this.tlpCamposMovimiento.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpCamposMovimiento.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposMovimiento.TabIndex = 0;
            // 
            // tlpComercioMovimiento
            // 
            this.tlpComercioMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpComercioMovimiento.Controls.Add(this.btnSugerirCategoriaMovimiento, 2, 1);
            this.tlpComercioMovimiento.Controls.Add(this.cmbComercioMovimiento, 0, 1);
            this.tlpComercioMovimiento.Controls.Add(this.lblComercioMovimiento, 0, 0);
            this.tlpComercioMovimiento.Controls.Add(this.txtbDescripcionMovimiento, 1, 1);
            this.tlpComercioMovimiento.Controls.Add(this.lblDescripcionMovimiento, 1, 0);
            this.tlpComercioMovimiento.Location = new System.Drawing.Point(11, 45);
            this.tlpComercioMovimiento.Name = "tlpComercioMovimiento";
            this.tlpComercioMovimiento.Size = new System.Drawing.Size(689, 47);
            this.tlpComercioMovimiento.ColumnCount = 3;
            this.tlpComercioMovimiento.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.58F));
            this.tlpComercioMovimiento.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.42F));
            this.tlpComercioMovimiento.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpComercioMovimiento.RowCount = 2;
            this.tlpComercioMovimiento.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tlpComercioMovimiento.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpComercioMovimiento.TabIndex = 4;
            // 
            // FormMovimientos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 640);
            this.Controls.Add(this.tlpSeleccionMovimientos);
            this.Controls.Add(this.pnlEscaneoTicket);
            this.Controls.Add(this.groupBoxEdicion);
            this.Controls.Add(this.dgvMovimientos);
            this.Controls.Add(this.groupBoxFiltros);
            this.Controls.Add(this.lblTituloMovimientos);
            this.Name = "FormMovimientos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMovements.Title";
            this.Text = "Gestion de movimientos";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltros.Panel)).EndInit();
            this.groupBoxFiltros.Panel.ResumeLayout(false);
            this.groupBoxFiltros.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxFiltros)).EndInit();
            this.groupBoxFiltros.ResumeLayout(false);
            this.groupBoxFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEdicion.Panel)).EndInit();
            this.groupBoxEdicion.Panel.ResumeLayout(false);
            this.groupBoxEdicion.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEdicion)).EndInit();
            this.groupBoxEdicion.ResumeLayout(false);
            this.groupBoxEdicion.PerformLayout();
            this.pnlEscaneoTicket.ResumeLayout(false);
            this.pnlEscaneoTicket.PerformLayout();
            this.tlpFechasFiltro.ResumeLayout(false);
            this.tlpFechasFiltro.PerformLayout();
            this.tlpCamposFiltro.ResumeLayout(false);
            this.tlpCamposFiltro.PerformLayout();
            this.tlpCamposMovimiento.ResumeLayout(false);
            this.tlpCamposMovimiento.PerformLayout();
            this.tlpComercioMovimiento.ResumeLayout(false);
            this.tlpComercioMovimiento.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloMovimientos;
        private Krypton.Toolkit.KryptonGroupBox groupBoxFiltros;
        private TpIDS.CheckBoxAccesible chkbFechaDesdeMovimientos;
        private TpIDS.DateTimePickerAccesible dtpFechaDesdeMovimientos;
        private TpIDS.CheckBoxAccesible chkbFechaHastaMovimientos;
        private TpIDS.DateTimePickerAccesible dtpFechaHastaMovimientos;
        private Krypton.Toolkit.KryptonLabel lblCategoriaFiltro;
        private Krypton.Toolkit.KryptonComboBox cmbCategoriaFiltro;
        private Krypton.Toolkit.KryptonLabel lblTipoFiltro;
        private Krypton.Toolkit.KryptonComboBox cmbTipoFiltro;
        private Krypton.Toolkit.KryptonButton btnBuscarMovimientos;
        private Krypton.Toolkit.KryptonLabel lblDescripcionFiltro;
        private Krypton.Toolkit.KryptonTextBox txtbDescripcionFiltro;
        private System.Windows.Forms.TableLayoutPanel tlpSeleccionMovimientos;
        private Krypton.Toolkit.KryptonButton btnSeleccionarTodosMovimientos;
        private Krypton.Toolkit.KryptonButton btnDesmarcarTodosMovimientos;
        private Krypton.Toolkit.KryptonLabel lblSeleccionMovimientos;
        private Krypton.Toolkit.KryptonButton btnExportarMovimientosPdf;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colMovimientosSeleccionar;
        private Krypton.Toolkit.KryptonDataGridView dgvMovimientos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosComercio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosMonto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosDescripcion;
        private Krypton.Toolkit.KryptonGroupBox groupBoxEdicion;
        private Krypton.Toolkit.KryptonLabel lblCategoriaMovimiento;
        private Krypton.Toolkit.KryptonComboBox cmbCategoriaMovimiento;
        private Krypton.Toolkit.KryptonLabel lblTipoMovimiento;
        private Krypton.Toolkit.KryptonComboBox cmbTipoMovimiento;
        private Krypton.Toolkit.KryptonLabel lblMontoMovimiento;
        private Krypton.Toolkit.KryptonTextBox txtbMontoMovimiento;
        private Krypton.Toolkit.KryptonLabel lblFechaMovimiento;
        private TpIDS.DateTimePickerAccesible dtpFechaMovimiento;
        private Krypton.Toolkit.KryptonLabel lblComercioMovimiento;
        private Krypton.Toolkit.KryptonComboBox cmbComercioMovimiento;
        private Krypton.Toolkit.KryptonLabel lblDescripcionMovimiento;
        private Krypton.Toolkit.KryptonTextBox txtbDescripcionMovimiento;
        private Krypton.Toolkit.KryptonButton btnSugerirCategoriaMovimiento;
        private Krypton.Toolkit.KryptonButton btnAgregarMovimiento;
        private Krypton.Toolkit.KryptonButton btnActualizarMovimiento;
        private Krypton.Toolkit.KryptonButton btnEliminarMovimiento;
        private Krypton.Toolkit.KryptonButton btnEscanearTicket;
        private System.Windows.Forms.Panel pnlEscaneoTicket;
        private System.Windows.Forms.ProgressBar prgbEscaneoTicket;
        private Krypton.Toolkit.KryptonLabel lblEscaneoTicketEnCurso;
        private System.Windows.Forms.TableLayoutPanel tlpFechasFiltro;
        private System.Windows.Forms.TableLayoutPanel tlpCamposFiltro;
        private System.Windows.Forms.TableLayoutPanel tlpCamposMovimiento;
        private System.Windows.Forms.TableLayoutPanel tlpComercioMovimiento;
    }
}
