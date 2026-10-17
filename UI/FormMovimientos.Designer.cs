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
            this.lblTituloMovimientos = new System.Windows.Forms.Label();
            this.groupBoxFiltros = new System.Windows.Forms.GroupBox();
            this.btnBuscarMovimientos = new System.Windows.Forms.Button();
            this.cmbTipoFiltro = new System.Windows.Forms.ComboBox();
            this.lblTipoFiltro = new System.Windows.Forms.Label();
            this.cmbCategoriaFiltro = new System.Windows.Forms.ComboBox();
            this.lblCategoriaFiltro = new System.Windows.Forms.Label();
            this.dtpFechaHastaMovimientos = new System.Windows.Forms.DateTimePicker();
            this.chkbFechaHastaMovimientos = new System.Windows.Forms.CheckBox();
            this.dtpFechaDesdeMovimientos = new System.Windows.Forms.DateTimePicker();
            this.chkbFechaDesdeMovimientos = new System.Windows.Forms.CheckBox();
            this.dgvMovimientos = new System.Windows.Forms.DataGridView();
            this.colMovimientosFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosComercio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosMonto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovimientosDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxEdicion = new System.Windows.Forms.GroupBox();
            this.btnSugerirCategoriaMovimiento = new System.Windows.Forms.Button();
            this.cmbComercioMovimiento = new System.Windows.Forms.ComboBox();
            this.lblComercioMovimiento = new System.Windows.Forms.Label();
            this.btnEliminarMovimiento = new System.Windows.Forms.Button();
            this.btnActualizarMovimiento = new System.Windows.Forms.Button();
            this.btnAgregarMovimiento = new System.Windows.Forms.Button();
            this.txtbDescripcionMovimiento = new System.Windows.Forms.TextBox();
            this.lblDescripcionMovimiento = new System.Windows.Forms.Label();
            this.dtpFechaMovimiento = new System.Windows.Forms.DateTimePicker();
            this.lblFechaMovimiento = new System.Windows.Forms.Label();
            this.txtbMontoMovimiento = new System.Windows.Forms.TextBox();
            this.lblMontoMovimiento = new System.Windows.Forms.Label();
            this.cmbTipoMovimiento = new System.Windows.Forms.ComboBox();
            this.lblTipoMovimiento = new System.Windows.Forms.Label();
            this.cmbCategoriaMovimiento = new System.Windows.Forms.ComboBox();
            this.lblCategoriaMovimiento = new System.Windows.Forms.Label();
            this.btnEscanearTicket = new System.Windows.Forms.Button();
            this.pnlEscaneoTicket = new System.Windows.Forms.Panel();
            this.prgbEscaneoTicket = new System.Windows.Forms.ProgressBar();
            this.lblEscaneoTicketEnCurso = new System.Windows.Forms.Label();
            this.groupBoxFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).BeginInit();
            this.groupBoxEdicion.SuspendLayout();
            this.pnlEscaneoTicket.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloMovimientos
            //
            this.lblTituloMovimientos.AutoSize = true;
            this.lblTituloMovimientos.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloMovimientos.Location = new System.Drawing.Point(18, 15);
            this.lblTituloMovimientos.Name = "lblTituloMovimientos";
            this.lblTituloMovimientos.Size = new System.Drawing.Size(230, 25);
            this.lblTituloMovimientos.TabIndex = 0;
            this.lblTituloMovimientos.Tag = "FormMovements.TitleLabel";
            this.lblTituloMovimientos.Text = "Gestion de movimientos";
            //
            // groupBoxFiltros
            //
            this.groupBoxFiltros.Controls.Add(this.btnBuscarMovimientos);
            this.groupBoxFiltros.Controls.Add(this.cmbTipoFiltro);
            this.groupBoxFiltros.Controls.Add(this.lblTipoFiltro);
            this.groupBoxFiltros.Controls.Add(this.cmbCategoriaFiltro);
            this.groupBoxFiltros.Controls.Add(this.lblCategoriaFiltro);
            this.groupBoxFiltros.Controls.Add(this.dtpFechaHastaMovimientos);
            this.groupBoxFiltros.Controls.Add(this.chkbFechaHastaMovimientos);
            this.groupBoxFiltros.Controls.Add(this.dtpFechaDesdeMovimientos);
            this.groupBoxFiltros.Controls.Add(this.chkbFechaDesdeMovimientos);
            this.groupBoxFiltros.Location = new System.Drawing.Point(23, 52);
            this.groupBoxFiltros.Name = "groupBoxFiltros";
            this.groupBoxFiltros.Size = new System.Drawing.Size(714, 100);
            this.groupBoxFiltros.TabIndex = 1;
            this.groupBoxFiltros.TabStop = false;
            this.groupBoxFiltros.Tag = "FormMovements.FiltersGroup";
            this.groupBoxFiltros.Text = "Filtros";
            //
            // btnBuscarMovimientos
            //
            this.btnBuscarMovimientos.Location = new System.Drawing.Point(590, 61);
            this.btnBuscarMovimientos.Name = "btnBuscarMovimientos";
            this.btnBuscarMovimientos.Size = new System.Drawing.Size(110, 27);
            this.btnBuscarMovimientos.TabIndex = 8;
            this.btnBuscarMovimientos.Tag = "FormMovements.SearchButton";
            this.btnBuscarMovimientos.Text = "Buscar";
            this.btnBuscarMovimientos.UseVisualStyleBackColor = true;
            this.btnBuscarMovimientos.Click += new System.EventHandler(this.btnBuscarMovimientos_Click);
            //
            // cmbTipoFiltro
            //
            this.cmbTipoFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoFiltro.FormattingEnabled = true;
            this.cmbTipoFiltro.Location = new System.Drawing.Point(430, 63);
            this.cmbTipoFiltro.Name = "cmbTipoFiltro";
            this.cmbTipoFiltro.Size = new System.Drawing.Size(140, 21);
            this.cmbTipoFiltro.TabIndex = 7;
            //
            // lblTipoFiltro
            //
            this.lblTipoFiltro.AutoSize = true;
            this.lblTipoFiltro.Location = new System.Drawing.Point(427, 47);
            this.lblTipoFiltro.Name = "lblTipoFiltro";
            this.lblTipoFiltro.Size = new System.Drawing.Size(33, 13);
            this.lblTipoFiltro.TabIndex = 6;
            this.lblTipoFiltro.Tag = "FormMovements.TypeLabel";
            this.lblTipoFiltro.Text = "Tipo:";
            //
            // cmbCategoriaFiltro
            //
            this.cmbCategoriaFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaFiltro.FormattingEnabled = true;
            this.cmbCategoriaFiltro.Location = new System.Drawing.Point(215, 63);
            this.cmbCategoriaFiltro.Name = "cmbCategoriaFiltro";
            this.cmbCategoriaFiltro.Size = new System.Drawing.Size(190, 21);
            this.cmbCategoriaFiltro.TabIndex = 5;
            //
            // lblCategoriaFiltro
            //
            this.lblCategoriaFiltro.AutoSize = true;
            this.lblCategoriaFiltro.Location = new System.Drawing.Point(212, 47);
            this.lblCategoriaFiltro.Name = "lblCategoriaFiltro";
            this.lblCategoriaFiltro.Size = new System.Drawing.Size(58, 13);
            this.lblCategoriaFiltro.TabIndex = 4;
            this.lblCategoriaFiltro.Tag = "FormMovements.CategoryLabel";
            this.lblCategoriaFiltro.Text = "Categoria:";
            //
            // dtpFechaHastaMovimientos
            //
            this.dtpFechaHastaMovimientos.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaHastaMovimientos.Location = new System.Drawing.Point(318, 22);
            this.dtpFechaHastaMovimientos.Name = "dtpFechaHastaMovimientos";
            this.dtpFechaHastaMovimientos.Size = new System.Drawing.Size(90, 20);
            this.dtpFechaHastaMovimientos.TabIndex = 3;
            //
            // chkbFechaHastaMovimientos
            //
            this.chkbFechaHastaMovimientos.AutoSize = true;
            this.chkbFechaHastaMovimientos.Location = new System.Drawing.Point(217, 24);
            this.chkbFechaHastaMovimientos.Name = "chkbFechaHastaMovimientos";
            this.chkbFechaHastaMovimientos.Size = new System.Drawing.Size(95, 17);
            this.chkbFechaHastaMovimientos.TabIndex = 2;
            this.chkbFechaHastaMovimientos.Tag = "FormMovements.ToDateCheck";
            this.chkbFechaHastaMovimientos.Text = "Fecha hasta:";
            this.chkbFechaHastaMovimientos.UseVisualStyleBackColor = true;
            //
            // dtpFechaDesdeMovimientos
            //
            this.dtpFechaDesdeMovimientos.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaDesdeMovimientos.Location = new System.Drawing.Point(120, 22);
            this.dtpFechaDesdeMovimientos.Name = "dtpFechaDesdeMovimientos";
            this.dtpFechaDesdeMovimientos.Size = new System.Drawing.Size(90, 20);
            this.dtpFechaDesdeMovimientos.TabIndex = 1;
            //
            // chkbFechaDesdeMovimientos
            //
            this.chkbFechaDesdeMovimientos.AutoSize = true;
            this.chkbFechaDesdeMovimientos.Location = new System.Drawing.Point(14, 24);
            this.chkbFechaDesdeMovimientos.Name = "chkbFechaDesdeMovimientos";
            this.chkbFechaDesdeMovimientos.Size = new System.Drawing.Size(100, 17);
            this.chkbFechaDesdeMovimientos.TabIndex = 0;
            this.chkbFechaDesdeMovimientos.Tag = "FormMovements.FromDateCheck";
            this.chkbFechaDesdeMovimientos.Text = "Fecha desde:";
            this.chkbFechaDesdeMovimientos.UseVisualStyleBackColor = true;
            //
            // dgvMovimientos
            //
            this.dgvMovimientos.AllowUserToAddRows = false;
            this.dgvMovimientos.AllowUserToDeleteRows = false;
            this.dgvMovimientos.AutoGenerateColumns = false;
            this.dgvMovimientos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMovimientos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMovimientosFecha,
            this.colMovimientosTipo,
            this.colMovimientosCategoria,
            this.colMovimientosComercio,
            this.colMovimientosMonto,
            this.colMovimientosDescripcion});
            this.dgvMovimientos.Location = new System.Drawing.Point(23, 152);
            this.dgvMovimientos.MultiSelect = false;
            this.dgvMovimientos.Name = "dgvMovimientos";
            this.dgvMovimientos.ReadOnly = true;
            this.dgvMovimientos.RowHeadersVisible = false;
            this.dgvMovimientos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMovimientos.Size = new System.Drawing.Size(714, 220);
            this.dgvMovimientos.TabIndex = 2;
            this.dgvMovimientos.Tag = "FormMovements.Grid";
            this.dgvMovimientos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMovimientos_CellClick);
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
            this.groupBoxEdicion.Controls.Add(this.btnSugerirCategoriaMovimiento);
            this.groupBoxEdicion.Controls.Add(this.cmbComercioMovimiento);
            this.groupBoxEdicion.Controls.Add(this.lblComercioMovimiento);
            this.groupBoxEdicion.Controls.Add(this.btnEscanearTicket);
            this.groupBoxEdicion.Controls.Add(this.btnEliminarMovimiento);
            this.groupBoxEdicion.Controls.Add(this.btnActualizarMovimiento);
            this.groupBoxEdicion.Controls.Add(this.btnAgregarMovimiento);
            this.groupBoxEdicion.Controls.Add(this.txtbDescripcionMovimiento);
            this.groupBoxEdicion.Controls.Add(this.lblDescripcionMovimiento);
            this.groupBoxEdicion.Controls.Add(this.dtpFechaMovimiento);
            this.groupBoxEdicion.Controls.Add(this.lblFechaMovimiento);
            this.groupBoxEdicion.Controls.Add(this.txtbMontoMovimiento);
            this.groupBoxEdicion.Controls.Add(this.lblMontoMovimiento);
            this.groupBoxEdicion.Controls.Add(this.cmbTipoMovimiento);
            this.groupBoxEdicion.Controls.Add(this.lblTipoMovimiento);
            this.groupBoxEdicion.Controls.Add(this.cmbCategoriaMovimiento);
            this.groupBoxEdicion.Controls.Add(this.lblCategoriaMovimiento);
            this.groupBoxEdicion.Location = new System.Drawing.Point(23, 380);
            this.groupBoxEdicion.Name = "groupBoxEdicion";
            this.groupBoxEdicion.Size = new System.Drawing.Size(714, 155);
            this.groupBoxEdicion.TabIndex = 3;
            this.groupBoxEdicion.TabStop = false;
            this.groupBoxEdicion.Tag = "FormMovements.MovementGroup";
            this.groupBoxEdicion.Text = "Datos del movimiento";
            //
            // btnEliminarMovimiento
            //
            this.btnEliminarMovimiento.Location = new System.Drawing.Point(334, 112);
            this.btnEliminarMovimiento.Name = "btnEliminarMovimiento";
            this.btnEliminarMovimiento.Size = new System.Drawing.Size(150, 30);
            this.btnEliminarMovimiento.TabIndex = 15;
            this.btnEliminarMovimiento.Tag = "FormMovements.DeleteButton";
            this.btnEliminarMovimiento.Text = "Eliminar movimiento";
            this.btnEliminarMovimiento.UseVisualStyleBackColor = true;
            this.btnEliminarMovimiento.Click += new System.EventHandler(this.btnEliminarMovimiento_Click);
            //
            // btnActualizarMovimiento
            //
            this.btnActualizarMovimiento.Location = new System.Drawing.Point(174, 112);
            this.btnActualizarMovimiento.Name = "btnActualizarMovimiento";
            this.btnActualizarMovimiento.Size = new System.Drawing.Size(150, 30);
            this.btnActualizarMovimiento.TabIndex = 14;
            this.btnActualizarMovimiento.Tag = "FormMovements.UpdateButton";
            this.btnActualizarMovimiento.Text = "Actualizar movimiento";
            this.btnActualizarMovimiento.UseVisualStyleBackColor = true;
            this.btnActualizarMovimiento.Click += new System.EventHandler(this.btnActualizarMovimiento_Click);
            //
            // btnAgregarMovimiento
            //
            this.btnAgregarMovimiento.Location = new System.Drawing.Point(14, 112);
            this.btnAgregarMovimiento.Name = "btnAgregarMovimiento";
            this.btnAgregarMovimiento.Size = new System.Drawing.Size(150, 30);
            this.btnAgregarMovimiento.TabIndex = 13;
            this.btnAgregarMovimiento.Tag = "FormMovements.AddButton";
            this.btnAgregarMovimiento.Text = "Agregar movimiento";
            this.btnAgregarMovimiento.UseVisualStyleBackColor = true;
            this.btnAgregarMovimiento.Click += new System.EventHandler(this.btnAgregarMovimiento_Click);
            //
            // btnEscanearTicket
            //
            this.btnEscanearTicket.Location = new System.Drawing.Point(494, 112);
            this.btnEscanearTicket.Name = "btnEscanearTicket";
            this.btnEscanearTicket.Size = new System.Drawing.Size(150, 30);
            this.btnEscanearTicket.TabIndex = 16;
            this.btnEscanearTicket.Tag = "FormMovements.ScanButton";
            this.btnEscanearTicket.Text = "Escanear ticket";
            this.btnEscanearTicket.UseVisualStyleBackColor = true;
            this.btnEscanearTicket.Click += new System.EventHandler(this.btnEscanearTicket_Click);
            //
            // pnlEscaneoTicket
            //
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
            this.lblEscaneoTicketEnCurso.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEscaneoTicketEnCurso.Location = new System.Drawing.Point(116, 19);
            this.lblEscaneoTicketEnCurso.Name = "lblEscaneoTicketEnCurso";
            this.lblEscaneoTicketEnCurso.Size = new System.Drawing.Size(143, 19);
            this.lblEscaneoTicketEnCurso.TabIndex = 0;
            this.lblEscaneoTicketEnCurso.Tag = "FormMovements.Scan.ProgressLabel";
            this.lblEscaneoTicketEnCurso.Text = "Analizando ticket...";
            //
            // txtbDescripcionMovimiento
            //
            // 
            // btnSugerirCategoriaMovimiento
            // 
            this.btnSugerirCategoriaMovimiento.Location = new System.Drawing.Point(590, 71);
            this.btnSugerirCategoriaMovimiento.Name = "btnSugerirCategoriaMovimiento";
            this.btnSugerirCategoriaMovimiento.Size = new System.Drawing.Size(110, 25);
            this.btnSugerirCategoriaMovimiento.TabIndex = 7;
            this.btnSugerirCategoriaMovimiento.Tag = "FormMovements.SuggestCategoryButton";
            this.btnSugerirCategoriaMovimiento.Text = "Sugerir cat.";
            this.btnSugerirCategoriaMovimiento.UseVisualStyleBackColor = true;
            this.btnSugerirCategoriaMovimiento.Click += new System.EventHandler(this.btnSugerirCategoriaMovimiento_Click);
            // 
            // cmbComercioMovimiento
            // 
            this.cmbComercioMovimiento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComercioMovimiento.FormattingEnabled = true;
            this.cmbComercioMovimiento.Location = new System.Drawing.Point(14, 73);
            this.cmbComercioMovimiento.Name = "cmbComercioMovimiento";
            this.cmbComercioMovimiento.Size = new System.Drawing.Size(190, 21);
            this.cmbComercioMovimiento.TabIndex = 5;
            // 
            // lblComercioMovimiento
            // 
            this.lblComercioMovimiento.AutoSize = true;
            this.lblComercioMovimiento.Location = new System.Drawing.Point(11, 57);
            this.lblComercioMovimiento.Name = "lblComercioMovimiento";
            this.lblComercioMovimiento.Size = new System.Drawing.Size(107, 13);
            this.lblComercioMovimiento.TabIndex = 4;
            this.lblComercioMovimiento.Tag = "FormMovements.CommerceLabel";
            this.lblComercioMovimiento.Text = "Comercio (opcional):";
            // 
            // txtbDescripcionMovimiento
            // 
            this.txtbDescripcionMovimiento.Location = new System.Drawing.Point(220, 73);
            this.txtbDescripcionMovimiento.Name = "txtbDescripcionMovimiento";
            this.txtbDescripcionMovimiento.Size = new System.Drawing.Size(355, 20);
            this.txtbDescripcionMovimiento.TabIndex = 6;
            this.txtbDescripcionMovimiento.Leave += new System.EventHandler(this.txtbDescripcionMovimiento_Leave);
            // 
            // lblDescripcionMovimiento
            // 
            this.lblDescripcionMovimiento.AutoSize = true;
            this.lblDescripcionMovimiento.Location = new System.Drawing.Point(217, 57);
            this.lblDescripcionMovimiento.Name = "lblDescripcionMovimiento";
            this.lblDescripcionMovimiento.Size = new System.Drawing.Size(66, 13);
            this.lblDescripcionMovimiento.TabIndex = 5;
            this.lblDescripcionMovimiento.Tag = "FormMovements.DescriptionLabel";
            this.lblDescripcionMovimiento.Text = "Descripcion:";
            // 
            // dtpFechaMovimiento
            // 
            this.dtpFechaMovimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaMovimiento.Location = new System.Drawing.Point(540, 31);
            this.dtpFechaMovimiento.Name = "dtpFechaMovimiento";
            this.dtpFechaMovimiento.Size = new System.Drawing.Size(160, 20);
            this.dtpFechaMovimiento.TabIndex = 3;
            // 
            // lblFechaMovimiento
            // 
            this.lblFechaMovimiento.AutoSize = true;
            this.lblFechaMovimiento.Location = new System.Drawing.Point(537, 15);
            this.lblFechaMovimiento.Name = "lblFechaMovimiento";
            this.lblFechaMovimiento.Size = new System.Drawing.Size(41, 13);
            this.lblFechaMovimiento.TabIndex = 2;
            this.lblFechaMovimiento.Tag = "FormMovements.DateLabel";
            this.lblFechaMovimiento.Text = "Fecha:";
            // 
            // txtbMontoMovimiento
            // 
            this.txtbMontoMovimiento.Location = new System.Drawing.Point(380, 31);
            this.txtbMontoMovimiento.Name = "txtbMontoMovimiento";
            this.txtbMontoMovimiento.Size = new System.Drawing.Size(140, 20);
            this.txtbMontoMovimiento.TabIndex = 2;
            // 
            // lblMontoMovimiento
            // 
            this.lblMontoMovimiento.AutoSize = true;
            this.lblMontoMovimiento.Location = new System.Drawing.Point(377, 15);
            this.lblMontoMovimiento.Name = "lblMontoMovimiento";
            this.lblMontoMovimiento.Size = new System.Drawing.Size(41, 13);
            this.lblMontoMovimiento.TabIndex = 1;
            this.lblMontoMovimiento.Tag = "FormMovements.AmountLabel";
            this.lblMontoMovimiento.Text = "Monto:";
            // 
            // cmbTipoMovimiento
            // 
            this.cmbTipoMovimiento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoMovimiento.FormattingEnabled = true;
            this.cmbTipoMovimiento.Location = new System.Drawing.Point(220, 31);
            this.cmbTipoMovimiento.Name = "cmbTipoMovimiento";
            this.cmbTipoMovimiento.Size = new System.Drawing.Size(140, 21);
            this.cmbTipoMovimiento.TabIndex = 1;
            // 
            // lblTipoMovimiento
            // 
            this.lblTipoMovimiento.AutoSize = true;
            this.lblTipoMovimiento.Location = new System.Drawing.Point(217, 15);
            this.lblTipoMovimiento.Name = "lblTipoMovimiento";
            this.lblTipoMovimiento.Size = new System.Drawing.Size(33, 13);
            this.lblTipoMovimiento.TabIndex = 0;
            this.lblTipoMovimiento.Tag = "FormMovements.TypeLabel";
            this.lblTipoMovimiento.Text = "Tipo:";
            // 
            // cmbCategoriaMovimiento
            // 
            this.cmbCategoriaMovimiento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaMovimiento.FormattingEnabled = true;
            this.cmbCategoriaMovimiento.Location = new System.Drawing.Point(14, 31);
            this.cmbCategoriaMovimiento.Name = "cmbCategoriaMovimiento";
            this.cmbCategoriaMovimiento.Size = new System.Drawing.Size(190, 21);
            this.cmbCategoriaMovimiento.TabIndex = 0;
            // 
            // lblCategoriaMovimiento
            // 
            this.lblCategoriaMovimiento.AutoSize = true;
            this.lblCategoriaMovimiento.Location = new System.Drawing.Point(11, 15);
            this.lblCategoriaMovimiento.Name = "lblCategoriaMovimiento";
            this.lblCategoriaMovimiento.Size = new System.Drawing.Size(58, 13);
            this.lblCategoriaMovimiento.TabIndex = 0;
            this.lblCategoriaMovimiento.Tag = "FormMovements.CategoryLabel";
            this.lblCategoriaMovimiento.Text = "Categoria:";
            // 
            // FormMovimientos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 550);
            this.Controls.Add(this.pnlEscaneoTicket);
            this.Controls.Add(this.groupBoxEdicion);
            this.Controls.Add(this.dgvMovimientos);
            this.Controls.Add(this.groupBoxFiltros);
            this.Controls.Add(this.lblTituloMovimientos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMovimientos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMovements.Title";
            this.Text = "Gestion de movimientos";
            this.groupBoxFiltros.ResumeLayout(false);
            this.groupBoxFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).EndInit();
            this.groupBoxEdicion.ResumeLayout(false);
            this.groupBoxEdicion.PerformLayout();
            this.pnlEscaneoTicket.ResumeLayout(false);
            this.pnlEscaneoTicket.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloMovimientos;
        private System.Windows.Forms.GroupBox groupBoxFiltros;
        private System.Windows.Forms.CheckBox chkbFechaDesdeMovimientos;
        private System.Windows.Forms.DateTimePicker dtpFechaDesdeMovimientos;
        private System.Windows.Forms.CheckBox chkbFechaHastaMovimientos;
        private System.Windows.Forms.DateTimePicker dtpFechaHastaMovimientos;
        private System.Windows.Forms.Label lblCategoriaFiltro;
        private System.Windows.Forms.ComboBox cmbCategoriaFiltro;
        private System.Windows.Forms.Label lblTipoFiltro;
        private System.Windows.Forms.ComboBox cmbTipoFiltro;
        private System.Windows.Forms.Button btnBuscarMovimientos;
        private System.Windows.Forms.DataGridView dgvMovimientos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosComercio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosMonto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovimientosDescripcion;
        private System.Windows.Forms.GroupBox groupBoxEdicion;
        private System.Windows.Forms.Label lblCategoriaMovimiento;
        private System.Windows.Forms.ComboBox cmbCategoriaMovimiento;
        private System.Windows.Forms.Label lblTipoMovimiento;
        private System.Windows.Forms.ComboBox cmbTipoMovimiento;
        private System.Windows.Forms.Label lblMontoMovimiento;
        private System.Windows.Forms.TextBox txtbMontoMovimiento;
        private System.Windows.Forms.Label lblFechaMovimiento;
        private System.Windows.Forms.DateTimePicker dtpFechaMovimiento;
        private System.Windows.Forms.Label lblComercioMovimiento;
        private System.Windows.Forms.ComboBox cmbComercioMovimiento;
        private System.Windows.Forms.Label lblDescripcionMovimiento;
        private System.Windows.Forms.TextBox txtbDescripcionMovimiento;
        private System.Windows.Forms.Button btnSugerirCategoriaMovimiento;
        private System.Windows.Forms.Button btnAgregarMovimiento;
        private System.Windows.Forms.Button btnActualizarMovimiento;
        private System.Windows.Forms.Button btnEliminarMovimiento;
        private System.Windows.Forms.Button btnEscanearTicket;
        private System.Windows.Forms.Panel pnlEscaneoTicket;
        private System.Windows.Forms.ProgressBar prgbEscaneoTicket;
        private System.Windows.Forms.Label lblEscaneoTicketEnCurso;
    }
}
