namespace TpIDS
{
    partial class FormGestionComercios
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
            this.lblTituloComercios = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxComercio = new Krypton.Toolkit.KryptonGroupBox();
            this.btnVerDetalleComercio = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarComercio = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarComercio = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarComercio = new Krypton.Toolkit.KryptonButton();
            this.cmbRubroComercio = new Krypton.Toolkit.KryptonComboBox();
            this.lblRubroComercio = new Krypton.Toolkit.KryptonLabel();
            this.txtbNombreComercio = new Krypton.Toolkit.KryptonTextBox();
            this.lblNombreComercio = new Krypton.Toolkit.KryptonLabel();
            this.txtbDireccionComercio = new Krypton.Toolkit.KryptonTextBox();
            this.lblDireccionComercio = new Krypton.Toolkit.KryptonLabel();
            this.txtbLocalidadComercio = new Krypton.Toolkit.KryptonTextBox();
            this.lblLocalidadComercio = new Krypton.Toolkit.KryptonLabel();
            this.txtbProvinciaComercio = new Krypton.Toolkit.KryptonTextBox();
            this.lblProvinciaComercio = new Krypton.Toolkit.KryptonLabel();
            this.lblCoordenadasTituloComercio = new Krypton.Toolkit.KryptonLabel();
            this.lblCoordenadasComercio = new Krypton.Toolkit.KryptonLabel();
            this.btnUbicarComercioEnMapa = new Krypton.Toolkit.KryptonButton();
            this.dgvComercios = new Krypton.Toolkit.KryptonDataGridView();
            this.colComercioNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComercioRubro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComercioDireccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxRubro = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarRubro = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarRubro = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarRubro = new Krypton.Toolkit.KryptonButton();
            this.txtbNombreRubro = new Krypton.Toolkit.KryptonTextBox();
            this.lblNombreRubro = new Krypton.Toolkit.KryptonLabel();
            this.dgvRubros = new Krypton.Toolkit.KryptonDataGridView();
            this.colRubroNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxComercio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxComercio.Panel)).BeginInit();
            this.groupBoxComercio.Panel.SuspendLayout();
            this.groupBoxComercio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComercios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRubro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRubro.Panel)).BeginInit();
            this.groupBoxRubro.Panel.SuspendLayout();
            this.groupBoxRubro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRubros)).BeginInit();
            this.tlpNombreRubroComercio = new System.Windows.Forms.TableLayoutPanel();
            this.tlpNombreRubroComercio.SuspendLayout();
            this.tlpDireccionComercio = new System.Windows.Forms.TableLayoutPanel();
            this.tlpDireccionComercio.SuspendLayout();
            this.tlpZonasComercios = new System.Windows.Forms.TableLayoutPanel();
            this.tlpZonasComercios.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloComercios
            //
            this.lblTituloComercios.AutoSize = true;
            this.lblTituloComercios.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloComercios.Location = new System.Drawing.Point(18, 15);
            this.lblTituloComercios.Name = "lblTituloComercios";
            this.lblTituloComercios.Size = new System.Drawing.Size(260, 25);
            this.lblTituloComercios.TabIndex = 0;
            this.lblTituloComercios.Tag = "FormMerchantManagement.TitleLabel";
            this.lblTituloComercios.Values.Text = "Gestion de Comercios";
            //
            // groupBoxComercio
            //
            this.groupBoxComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxComercio.Panel.Controls.Add(this.btnVerDetalleComercio);
            this.groupBoxComercio.Panel.Controls.Add(this.btnEliminarComercio);
            this.groupBoxComercio.Panel.Controls.Add(this.btnActualizarComercio);
            this.groupBoxComercio.Panel.Controls.Add(this.btnAgregarComercio);
            this.groupBoxComercio.Panel.Controls.Add(this.btnUbicarComercioEnMapa);
            this.groupBoxComercio.Panel.Controls.Add(this.lblCoordenadasComercio);
            this.groupBoxComercio.Panel.Controls.Add(this.lblCoordenadasTituloComercio);
            this.groupBoxComercio.Panel.Controls.Add(this.tlpDireccionComercio);
            this.groupBoxComercio.Panel.Controls.Add(this.tlpNombreRubroComercio);
            this.groupBoxComercio.Location = new System.Drawing.Point(0, 0);
            this.groupBoxComercio.Name = "groupBoxComercio";
            this.groupBoxComercio.Size = new System.Drawing.Size(594, 220);
            this.groupBoxComercio.TabIndex = 1;
            this.groupBoxComercio.TabStop = false;
            this.groupBoxComercio.Tag = "FormMerchantManagement.ComercioGroup";
            this.groupBoxComercio.Values.Heading = "Comercios";
            this.groupBoxComercio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // btnVerDetalleComercio
            //
            this.btnVerDetalleComercio.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnVerDetalleComercio.Location = new System.Drawing.Point(374, 154);
            this.btnVerDetalleComercio.Name = "btnVerDetalleComercio";
            this.btnVerDetalleComercio.Size = new System.Drawing.Size(200, 30);
            this.btnVerDetalleComercio.TabIndex = 12;
            this.btnVerDetalleComercio.Tag = "FormMerchantManagement.DetailButton";
            this.btnVerDetalleComercio.Values.Text = "Ver detalle";
            this.btnVerDetalleComercio.Click += new System.EventHandler(this.btnVerDetalleComercio_Click);
            //
            // btnEliminarComercio
            //
            this.btnEliminarComercio.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarComercio.Location = new System.Drawing.Point(254, 154);
            this.btnEliminarComercio.Name = "btnEliminarComercio";
            this.btnEliminarComercio.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarComercio.TabIndex = 11;
            this.btnEliminarComercio.Tag = "FormMerchantManagement.DeleteComercioButton";
            this.btnEliminarComercio.Values.Text = "Eliminar";
            this.btnEliminarComercio.Click += new System.EventHandler(this.btnEliminarComercio_Click);
            //
            // btnActualizarComercio
            //
            this.btnActualizarComercio.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarComercio.Location = new System.Drawing.Point(134, 154);
            this.btnActualizarComercio.Name = "btnActualizarComercio";
            this.btnActualizarComercio.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarComercio.TabIndex = 10;
            this.btnActualizarComercio.Tag = "FormMerchantManagement.UpdateComercioButton";
            this.btnActualizarComercio.Values.Text = "Actualizar";
            this.btnActualizarComercio.Click += new System.EventHandler(this.btnActualizarComercio_Click);
            //
            // btnAgregarComercio
            //
            this.btnAgregarComercio.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarComercio.Location = new System.Drawing.Point(14, 154);
            this.btnAgregarComercio.Name = "btnAgregarComercio";
            this.btnAgregarComercio.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarComercio.TabIndex = 9;
            this.btnAgregarComercio.Tag = "FormMerchantManagement.AddComercioButton";
            this.btnAgregarComercio.Values.Text = "Agregar";
            this.btnAgregarComercio.Click += new System.EventHandler(this.btnAgregarComercio_Click);
            //
            // txtbDireccionComercio
            //
            this.txtbDireccionComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbDireccionComercio.Location = new System.Drawing.Point(3, 20);
            this.txtbDireccionComercio.Name = "txtbDireccionComercio";
            this.txtbDireccionComercio.Size = new System.Drawing.Size(250, 20);
            this.txtbDireccionComercio.TabIndex = 5;
            this.txtbDireccionComercio.Margin = new System.Windows.Forms.Padding(3, 0, 11, 0);
            //
            // lblDireccionComercio
            //
            this.lblDireccionComercio.AutoSize = true;
            this.lblDireccionComercio.Location = new System.Drawing.Point(0, 0);
            this.lblDireccionComercio.Name = "lblDireccionComercio";
            this.lblDireccionComercio.Size = new System.Drawing.Size(59, 13);
            this.lblDireccionComercio.TabIndex = 4;
            this.lblDireccionComercio.Tag = "FormMerchantManagement.DireccionComercioLabel";
            this.lblDireccionComercio.Values.Text = "Direccion:";
            this.lblDireccionComercio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // txtbLocalidadComercio
            //
            this.txtbLocalidadComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbLocalidadComercio.Location = new System.Drawing.Point(267, 20);
            this.txtbLocalidadComercio.Name = "txtbLocalidadComercio";
            this.txtbLocalidadComercio.Size = new System.Drawing.Size(145, 20);
            this.txtbLocalidadComercio.TabIndex = 6;
            this.txtbLocalidadComercio.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            //
            // lblLocalidadComercio
            //
            this.lblLocalidadComercio.AutoSize = true;
            this.lblLocalidadComercio.Location = new System.Drawing.Point(264, 0);
            this.lblLocalidadComercio.Name = "lblLocalidadComercio";
            this.lblLocalidadComercio.Size = new System.Drawing.Size(59, 13);
            this.lblLocalidadComercio.TabIndex = 5;
            this.lblLocalidadComercio.Tag = "FormMerchantManagement.LocalidadComercioLabel";
            this.lblLocalidadComercio.Values.Text = "Localidad:";
            this.lblLocalidadComercio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // txtbProvinciaComercio
            //
            this.txtbProvinciaComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbProvinciaComercio.Location = new System.Drawing.Point(423, 20);
            this.txtbProvinciaComercio.Name = "txtbProvinciaComercio";
            this.txtbProvinciaComercio.Size = new System.Drawing.Size(145, 20);
            this.txtbProvinciaComercio.TabIndex = 7;
            this.txtbProvinciaComercio.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblProvinciaComercio
            //
            this.lblProvinciaComercio.AutoSize = true;
            this.lblProvinciaComercio.Location = new System.Drawing.Point(420, 0);
            this.lblProvinciaComercio.Name = "lblProvinciaComercio";
            this.lblProvinciaComercio.Size = new System.Drawing.Size(59, 13);
            this.lblProvinciaComercio.TabIndex = 6;
            this.lblProvinciaComercio.Tag = "FormMerchantManagement.ProvinciaComercioLabel";
            this.lblProvinciaComercio.Values.Text = "Provincia:";
            this.lblProvinciaComercio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // lblCoordenadasTituloComercio
            //
            this.lblCoordenadasTituloComercio.AutoSize = true;
            this.lblCoordenadasTituloComercio.Location = new System.Drawing.Point(11, 110);
            this.lblCoordenadasTituloComercio.Name = "lblCoordenadasTituloComercio";
            this.lblCoordenadasTituloComercio.Size = new System.Drawing.Size(75, 13);
            this.lblCoordenadasTituloComercio.TabIndex = 8;
            this.lblCoordenadasTituloComercio.Tag = "FormMerchantManagement.CoordenadasLabel";
            this.lblCoordenadasTituloComercio.Values.Text = "Coordenadas:";
            //
            // lblCoordenadasComercio
            //
            this.lblCoordenadasComercio.AutoSize = true;
            this.lblCoordenadasComercio.Location = new System.Drawing.Point(100, 110);
            this.lblCoordenadasComercio.Name = "lblCoordenadasComercio";
            this.lblCoordenadasComercio.Size = new System.Drawing.Size(59, 13);
            this.lblCoordenadasComercio.TabIndex = 8;
            this.lblCoordenadasComercio.Values.Text = "Sin ubicar";
            //
            // btnUbicarComercioEnMapa
            //
            this.btnUbicarComercioEnMapa.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnUbicarComercioEnMapa.Location = new System.Drawing.Point(330, 106);
            this.btnUbicarComercioEnMapa.Name = "btnUbicarComercioEnMapa";
            this.btnUbicarComercioEnMapa.Size = new System.Drawing.Size(180, 28);
            this.btnUbicarComercioEnMapa.TabIndex = 8;
            this.btnUbicarComercioEnMapa.Tag = "FormMerchantManagement.LocateOnMapButton";
            this.btnUbicarComercioEnMapa.Values.Text = "Ubicar en el mapa";
            this.btnUbicarComercioEnMapa.Click += new System.EventHandler(this.btnUbicarComercioEnMapa_Click);
            //
            // cmbRubroComercio
            //
            this.cmbRubroComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRubroComercio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRubroComercio.FormattingEnabled = true;
            this.cmbRubroComercio.Location = new System.Drawing.Point(199, 20);
            this.cmbRubroComercio.Name = "cmbRubroComercio";
            this.cmbRubroComercio.Size = new System.Drawing.Size(160, 21);
            this.cmbRubroComercio.TabIndex = 3;
            this.cmbRubroComercio.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblRubroComercio
            //
            this.lblRubroComercio.AutoSize = true;
            this.lblRubroComercio.Location = new System.Drawing.Point(196, 0);
            this.lblRubroComercio.Name = "lblRubroComercio";
            this.lblRubroComercio.Size = new System.Drawing.Size(41, 13);
            this.lblRubroComercio.TabIndex = 2;
            this.lblRubroComercio.Tag = "FormMerchantManagement.RubroComboLabel";
            this.lblRubroComercio.Values.Text = "Rubro:";
            this.lblRubroComercio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // txtbNombreComercio
            //
            this.txtbNombreComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreComercio.Location = new System.Drawing.Point(3, 20);
            this.txtbNombreComercio.Name = "txtbNombreComercio";
            this.txtbNombreComercio.Size = new System.Drawing.Size(180, 20);
            this.txtbNombreComercio.TabIndex = 1;
            this.txtbNombreComercio.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
            //
            // lblNombreComercio
            //
            this.lblNombreComercio.AutoSize = true;
            this.lblNombreComercio.Location = new System.Drawing.Point(0, 0);
            this.lblNombreComercio.Name = "lblNombreComercio";
            this.lblNombreComercio.Size = new System.Drawing.Size(53, 13);
            this.lblNombreComercio.TabIndex = 0;
            this.lblNombreComercio.Tag = "FormMerchantManagement.NombreComercioLabel";
            this.lblNombreComercio.Values.Text = "Nombre:";
            this.lblNombreComercio.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvComercios
            //
            this.dgvComercios.AllowUserToAddRows = false;
            this.dgvComercios.AllowUserToDeleteRows = false;
            this.dgvComercios.AutoGenerateColumns = false;
            this.dgvComercios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvComercios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvComercios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComercios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colComercioNombre,
            this.colComercioRubro,
            this.colComercioDireccion});
            this.dgvComercios.Location = new System.Drawing.Point(0, 233);
            this.dgvComercios.MultiSelect = false;
            this.dgvComercios.Name = "dgvComercios";
            this.dgvComercios.RowHeadersVisible = false;
            this.dgvComercios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComercios.Size = new System.Drawing.Size(594, 180);
            this.dgvComercios.TabIndex = 2;
            this.dgvComercios.Tag = "FormMerchantManagement.ComercioGrid";
            this.dgvComercios.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComercios_CellClick);
            this.dgvComercios.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            //
            // colComercioNombre
            //
            this.colComercioNombre.DataPropertyName = "Nombre";
            this.colComercioNombre.HeaderText = "Nombre";
            this.colComercioNombre.Name = "Nombre";
            this.colComercioNombre.ReadOnly = true;
            //
            // colComercioRubro
            //
            this.colComercioRubro.DataPropertyName = "Rubro";
            this.colComercioRubro.HeaderText = "Rubro";
            this.colComercioRubro.Name = "Rubro";
            this.colComercioRubro.ReadOnly = true;
            //
            // colComercioDireccion
            //
            this.colComercioDireccion.DataPropertyName = "Direccion";
            this.colComercioDireccion.HeaderText = "Direccion";
            this.colComercioDireccion.Name = "Direccion";
            this.colComercioDireccion.ReadOnly = true;
            //
            // groupBoxRubro
            //
            this.groupBoxRubro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRubro.Panel.Controls.Add(this.btnEliminarRubro);
            this.groupBoxRubro.Panel.Controls.Add(this.btnActualizarRubro);
            this.groupBoxRubro.Panel.Controls.Add(this.btnAgregarRubro);
            this.groupBoxRubro.Panel.Controls.Add(this.txtbNombreRubro);
            this.groupBoxRubro.Panel.Controls.Add(this.lblNombreRubro);
            this.groupBoxRubro.Location = new System.Drawing.Point(0, 428);
            this.groupBoxRubro.Name = "groupBoxRubro";
            this.groupBoxRubro.Size = new System.Drawing.Size(594, 110);
            this.groupBoxRubro.TabIndex = 3;
            this.groupBoxRubro.TabStop = false;
            this.groupBoxRubro.Tag = "FormMerchantManagement.RubroGroup";
            this.groupBoxRubro.Values.Heading = "Rubros de comercio";
            this.groupBoxRubro.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // btnEliminarRubro
            //
            this.btnEliminarRubro.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarRubro.Location = new System.Drawing.Point(254, 52);
            this.btnEliminarRubro.Name = "btnEliminarRubro";
            this.btnEliminarRubro.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarRubro.TabIndex = 4;
            this.btnEliminarRubro.Tag = "FormMerchantManagement.DeleteRubroButton";
            this.btnEliminarRubro.Values.Text = "Eliminar";
            this.btnEliminarRubro.Click += new System.EventHandler(this.btnEliminarRubro_Click);
            //
            // btnActualizarRubro
            //
            this.btnActualizarRubro.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarRubro.Location = new System.Drawing.Point(134, 52);
            this.btnActualizarRubro.Name = "btnActualizarRubro";
            this.btnActualizarRubro.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarRubro.TabIndex = 3;
            this.btnActualizarRubro.Tag = "FormMerchantManagement.UpdateRubroButton";
            this.btnActualizarRubro.Values.Text = "Actualizar";
            this.btnActualizarRubro.Click += new System.EventHandler(this.btnActualizarRubro_Click);
            //
            // btnAgregarRubro
            //
            this.btnAgregarRubro.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarRubro.Location = new System.Drawing.Point(14, 52);
            this.btnAgregarRubro.Name = "btnAgregarRubro";
            this.btnAgregarRubro.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarRubro.TabIndex = 2;
            this.btnAgregarRubro.Tag = "FormMerchantManagement.AddRubroButton";
            this.btnAgregarRubro.Values.Text = "Agregar";
            this.btnAgregarRubro.Click += new System.EventHandler(this.btnAgregarRubro_Click);
            //
            // txtbNombreRubro
            //
            this.txtbNombreRubro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreRubro.Location = new System.Drawing.Point(14, 28);
            this.txtbNombreRubro.Name = "txtbNombreRubro";
            this.txtbNombreRubro.Size = new System.Drawing.Size(300, 20);
            this.txtbNombreRubro.TabIndex = 1;
            //
            // lblNombreRubro
            //
            this.lblNombreRubro.AutoSize = true;
            this.lblNombreRubro.Location = new System.Drawing.Point(11, 8);
            this.lblNombreRubro.Name = "lblNombreRubro";
            this.lblNombreRubro.Size = new System.Drawing.Size(53, 13);
            this.lblNombreRubro.TabIndex = 0;
            this.lblNombreRubro.Tag = "FormMerchantManagement.NombreRubroLabel";
            this.lblNombreRubro.Values.Text = "Nombre:";
            //
            // dgvRubros
            //
            this.dgvRubros.AllowUserToAddRows = false;
            this.dgvRubros.AllowUserToDeleteRows = false;
            this.dgvRubros.AutoGenerateColumns = false;
            this.dgvRubros.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRubros.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRubros.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRubros.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRubroNombre});
            this.dgvRubros.Location = new System.Drawing.Point(0, 553);
            this.dgvRubros.MultiSelect = false;
            this.dgvRubros.Name = "dgvRubros";
            this.dgvRubros.RowHeadersVisible = false;
            this.dgvRubros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRubros.Size = new System.Drawing.Size(594, 150);
            this.dgvRubros.TabIndex = 4;
            this.dgvRubros.Tag = "FormMerchantManagement.RubroGrid";
            this.dgvRubros.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRubros_CellClick);
            this.dgvRubros.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            //
            // colRubroNombre
            //
            this.colRubroNombre.DataPropertyName = "Nombre";
            this.colRubroNombre.HeaderText = "Nombre";
            this.colRubroNombre.Name = "Nombre";
            this.colRubroNombre.ReadOnly = true;
            // 
            // tlpNombreRubroComercio
            // 
            this.tlpNombreRubroComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpNombreRubroComercio.Controls.Add(this.cmbRubroComercio, 1, 1);
            this.tlpNombreRubroComercio.Controls.Add(this.lblRubroComercio, 1, 0);
            this.tlpNombreRubroComercio.Controls.Add(this.txtbNombreComercio, 0, 1);
            this.tlpNombreRubroComercio.Controls.Add(this.lblNombreComercio, 0, 0);
            this.tlpNombreRubroComercio.Location = new System.Drawing.Point(11, 8);
            this.tlpNombreRubroComercio.Name = "tlpNombreRubroComercio";
            this.tlpNombreRubroComercio.Size = new System.Drawing.Size(359, 45);
            this.tlpNombreRubroComercio.ColumnCount = 2;
            this.tlpNombreRubroComercio.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.6F));
            this.tlpNombreRubroComercio.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.4F));
            this.tlpNombreRubroComercio.RowCount = 2;
            this.tlpNombreRubroComercio.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpNombreRubroComercio.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpNombreRubroComercio.TabIndex = 0;
            // 
            // tlpDireccionComercio
            // 
            this.tlpDireccionComercio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpDireccionComercio.Controls.Add(this.txtbProvinciaComercio, 2, 1);
            this.tlpDireccionComercio.Controls.Add(this.lblProvinciaComercio, 2, 0);
            this.tlpDireccionComercio.Controls.Add(this.txtbLocalidadComercio, 1, 1);
            this.tlpDireccionComercio.Controls.Add(this.lblLocalidadComercio, 1, 0);
            this.tlpDireccionComercio.Controls.Add(this.txtbDireccionComercio, 0, 1);
            this.tlpDireccionComercio.Controls.Add(this.lblDireccionComercio, 0, 0);
            this.tlpDireccionComercio.Location = new System.Drawing.Point(11, 56);
            this.tlpDireccionComercio.Name = "tlpDireccionComercio";
            this.tlpDireccionComercio.Size = new System.Drawing.Size(568, 44);
            this.tlpDireccionComercio.ColumnCount = 3;
            this.tlpDireccionComercio.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.48F));
            this.tlpDireccionComercio.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.46F));
            this.tlpDireccionComercio.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.06F));
            this.tlpDireccionComercio.RowCount = 2;
            this.tlpDireccionComercio.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpDireccionComercio.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpDireccionComercio.TabIndex = 4;
            // 
            // tlpZonasComercios
            // 
            this.tlpZonasComercios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpZonasComercios.Controls.Add(this.dgvRubros, 0, 3);
            this.tlpZonasComercios.Controls.Add(this.groupBoxRubro, 0, 2);
            this.tlpZonasComercios.Controls.Add(this.dgvComercios, 0, 1);
            this.tlpZonasComercios.Controls.Add(this.groupBoxComercio, 0, 0);
            this.tlpZonasComercios.Location = new System.Drawing.Point(23, 52);
            this.tlpZonasComercios.Name = "tlpZonasComercios";
            this.tlpZonasComercios.Size = new System.Drawing.Size(594, 707);
            this.tlpZonasComercios.ColumnCount = 1;
            this.tlpZonasComercios.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpZonasComercios.RowCount = 4;
            this.tlpZonasComercios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 233F));
            this.tlpZonasComercios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.87F));
            this.tlpZonasComercios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tlpZonasComercios.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.13F));
            this.tlpZonasComercios.TabIndex = 1;
            //
            // FormGestionComercios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 780);
            this.Controls.Add(this.tlpZonasComercios);
            this.Controls.Add(this.lblTituloComercios);
            this.Name = "FormGestionComercios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMerchantManagement.Title";
            this.Text = "Gestion de Comercios";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxComercio.Panel)).EndInit();
            this.groupBoxComercio.Panel.ResumeLayout(false);
            this.groupBoxComercio.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxComercio)).EndInit();
            this.groupBoxComercio.ResumeLayout(false);
            this.groupBoxComercio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComercios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRubro.Panel)).EndInit();
            this.groupBoxRubro.Panel.ResumeLayout(false);
            this.groupBoxRubro.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRubro)).EndInit();
            this.groupBoxRubro.ResumeLayout(false);
            this.groupBoxRubro.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRubros)).EndInit();
            this.tlpNombreRubroComercio.ResumeLayout(false);
            this.tlpNombreRubroComercio.PerformLayout();
            this.tlpDireccionComercio.ResumeLayout(false);
            this.tlpDireccionComercio.PerformLayout();
            this.tlpZonasComercios.ResumeLayout(false);
            this.tlpZonasComercios.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloComercios;
        private Krypton.Toolkit.KryptonGroupBox groupBoxComercio;
        private Krypton.Toolkit.KryptonButton btnVerDetalleComercio;
        private Krypton.Toolkit.KryptonButton btnEliminarComercio;
        private Krypton.Toolkit.KryptonButton btnActualizarComercio;
        private Krypton.Toolkit.KryptonButton btnAgregarComercio;
        private Krypton.Toolkit.KryptonComboBox cmbRubroComercio;
        private Krypton.Toolkit.KryptonLabel lblRubroComercio;
        private Krypton.Toolkit.KryptonTextBox txtbNombreComercio;
        private Krypton.Toolkit.KryptonLabel lblNombreComercio;
        private Krypton.Toolkit.KryptonTextBox txtbDireccionComercio;
        private Krypton.Toolkit.KryptonLabel lblDireccionComercio;
        private Krypton.Toolkit.KryptonTextBox txtbLocalidadComercio;
        private Krypton.Toolkit.KryptonLabel lblLocalidadComercio;
        private Krypton.Toolkit.KryptonTextBox txtbProvinciaComercio;
        private Krypton.Toolkit.KryptonLabel lblProvinciaComercio;
        private Krypton.Toolkit.KryptonLabel lblCoordenadasTituloComercio;
        private Krypton.Toolkit.KryptonLabel lblCoordenadasComercio;
        private Krypton.Toolkit.KryptonButton btnUbicarComercioEnMapa;
        private Krypton.Toolkit.KryptonDataGridView dgvComercios;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComercioNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComercioRubro;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComercioDireccion;
        private Krypton.Toolkit.KryptonGroupBox groupBoxRubro;
        private Krypton.Toolkit.KryptonButton btnEliminarRubro;
        private Krypton.Toolkit.KryptonButton btnActualizarRubro;
        private Krypton.Toolkit.KryptonButton btnAgregarRubro;
        private Krypton.Toolkit.KryptonTextBox txtbNombreRubro;
        private Krypton.Toolkit.KryptonLabel lblNombreRubro;
        private Krypton.Toolkit.KryptonDataGridView dgvRubros;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRubroNombre;
        private System.Windows.Forms.TableLayoutPanel tlpNombreRubroComercio;
        private System.Windows.Forms.TableLayoutPanel tlpDireccionComercio;
        private System.Windows.Forms.TableLayoutPanel tlpZonasComercios;
    }
}
