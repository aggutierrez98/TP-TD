namespace TpIDS
{
    partial class FormGestionMonedas
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
            this.lblTituloMonedas = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxMonedas = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarMoneda = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarMoneda = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarMoneda = new Krypton.Toolkit.KryptonButton();
            this.txtbNombreMoneda = new Krypton.Toolkit.KryptonTextBox();
            this.lblNombreMoneda = new Krypton.Toolkit.KryptonLabel();
            this.txtbCodigoMoneda = new Krypton.Toolkit.KryptonTextBox();
            this.lblCodigoMoneda = new Krypton.Toolkit.KryptonLabel();
            this.dgvMonedas = new Krypton.Toolkit.KryptonDataGridView();
            this.colMonedaCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMonedaNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxTasas = new Krypton.Toolkit.KryptonGroupBox();
            this.btnLiberarTasa = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarTasa = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarTasa = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarTasa = new Krypton.Toolkit.KryptonButton();
            this.nudCalculo = new System.Windows.Forms.NumericUpDown();
            this.lblCalculo = new Krypton.Toolkit.KryptonLabel();
            this.cmbMonedaConversion = new Krypton.Toolkit.KryptonComboBox();
            this.lblMonedaConversion = new Krypton.Toolkit.KryptonLabel();
            this.cmbMonedaBase = new Krypton.Toolkit.KryptonComboBox();
            this.lblMonedaBase = new Krypton.Toolkit.KryptonLabel();
            this.dgvTasas = new Krypton.Toolkit.KryptonDataGridView();
            this.colTasaMonedaBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTasaMonedaConversion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTasaCalculo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTasaFechaActualizacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTasaEsManual = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMonedas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMonedas.Panel)).BeginInit();
            this.groupBoxMonedas.Panel.SuspendLayout();
            this.groupBoxMonedas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonedas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTasas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTasas.Panel)).BeginInit();
            this.groupBoxTasas.Panel.SuspendLayout();
            this.groupBoxTasas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalculo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasas)).BeginInit();
            this.tlpCamposMoneda = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposMoneda.SuspendLayout();
            this.tlpCamposTasa = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposTasa.SuspendLayout();
            this.tlpZonasMonedas = new System.Windows.Forms.TableLayoutPanel();
            this.tlpZonasMonedas.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloMonedas
            //
            this.lblTituloMonedas.AutoSize = true;
            this.lblTituloMonedas.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloMonedas.Location = new System.Drawing.Point(18, 15);
            this.lblTituloMonedas.Name = "lblTituloMonedas";
            this.lblTituloMonedas.Size = new System.Drawing.Size(240, 25);
            this.lblTituloMonedas.TabIndex = 0;
            this.lblTituloMonedas.Tag = "FormCurrencyManagement.TitleLabel";
            this.lblTituloMonedas.Values.Text = "Gestion de Monedas";
            //
            // groupBoxMonedas
            //
            this.groupBoxMonedas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMonedas.Panel.Controls.Add(this.btnEliminarMoneda);
            this.groupBoxMonedas.Panel.Controls.Add(this.btnActualizarMoneda);
            this.groupBoxMonedas.Panel.Controls.Add(this.btnAgregarMoneda);
            this.groupBoxMonedas.Panel.Controls.Add(this.tlpCamposMoneda);
            this.groupBoxMonedas.Panel.Controls.Add(this.dgvMonedas);
            this.groupBoxMonedas.Location = new System.Drawing.Point(0, 0);
            this.groupBoxMonedas.Name = "groupBoxMonedas";
            this.groupBoxMonedas.Size = new System.Drawing.Size(594, 260);
            this.groupBoxMonedas.TabIndex = 1;
            this.groupBoxMonedas.TabStop = false;
            this.groupBoxMonedas.Tag = "FormCurrencyManagement.MonedaGroup";
            this.groupBoxMonedas.Values.Heading = "Monedas";
            this.groupBoxMonedas.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            //
            // btnEliminarMoneda
            //
            this.btnEliminarMoneda.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarMoneda.Location = new System.Drawing.Point(254, 45);
            this.btnEliminarMoneda.Name = "btnEliminarMoneda";
            this.btnEliminarMoneda.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarMoneda.TabIndex = 6;
            this.btnEliminarMoneda.Tag = "FormCurrencyManagement.DeleteButton";
            this.btnEliminarMoneda.Values.Text = "Eliminar";
            this.btnEliminarMoneda.Click += new System.EventHandler(this.btnEliminarMoneda_Click);
            //
            // btnActualizarMoneda
            //
            this.btnActualizarMoneda.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarMoneda.Location = new System.Drawing.Point(134, 45);
            this.btnActualizarMoneda.Name = "btnActualizarMoneda";
            this.btnActualizarMoneda.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarMoneda.TabIndex = 5;
            this.btnActualizarMoneda.Tag = "FormCurrencyManagement.UpdateButton";
            this.btnActualizarMoneda.Values.Text = "Actualizar";
            this.btnActualizarMoneda.Click += new System.EventHandler(this.btnActualizarMoneda_Click);
            //
            // btnAgregarMoneda
            //
            this.btnAgregarMoneda.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarMoneda.Location = new System.Drawing.Point(14, 45);
            this.btnAgregarMoneda.Name = "btnAgregarMoneda";
            this.btnAgregarMoneda.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarMoneda.TabIndex = 4;
            this.btnAgregarMoneda.Tag = "FormCurrencyManagement.AddButton";
            this.btnAgregarMoneda.Values.Text = "Agregar";
            this.btnAgregarMoneda.Click += new System.EventHandler(this.btnAgregarMoneda_Click);
            //
            // txtbNombreMoneda
            //
            this.txtbNombreMoneda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreMoneda.Location = new System.Drawing.Point(315, 1);
            this.txtbNombreMoneda.Name = "txtbNombreMoneda";
            this.txtbNombreMoneda.Size = new System.Drawing.Size(254, 20);
            this.txtbNombreMoneda.TabIndex = 3;
            this.txtbNombreMoneda.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            //
            // lblNombreMoneda
            //
            this.lblNombreMoneda.AutoSize = true;
            this.lblNombreMoneda.Location = new System.Drawing.Point(259, 0);
            this.lblNombreMoneda.Name = "lblNombreMoneda";
            this.lblNombreMoneda.Size = new System.Drawing.Size(50, 13);
            this.lblNombreMoneda.TabIndex = 2;
            this.lblNombreMoneda.Tag = "FormCurrencyManagement.NombreLabel";
            this.lblNombreMoneda.Values.Text = "Nombre:";
            this.lblNombreMoneda.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // txtbCodigoMoneda
            //
            this.txtbCodigoMoneda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbCodigoMoneda.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbCodigoMoneda.Location = new System.Drawing.Point(59, 1);
            this.txtbCodigoMoneda.MaxLength = 3;
            this.txtbCodigoMoneda.Name = "txtbCodigoMoneda";
            this.txtbCodigoMoneda.Size = new System.Drawing.Size(180, 20);
            this.txtbCodigoMoneda.TabIndex = 1;
            this.txtbCodigoMoneda.Margin = new System.Windows.Forms.Padding(0, 1, 20, 0);
            //
            // lblCodigoMoneda
            //
            this.lblCodigoMoneda.AutoSize = true;
            this.lblCodigoMoneda.Location = new System.Drawing.Point(0, 0);
            this.lblCodigoMoneda.Name = "lblCodigoMoneda";
            this.lblCodigoMoneda.Size = new System.Drawing.Size(43, 13);
            this.lblCodigoMoneda.TabIndex = 0;
            this.lblCodigoMoneda.Tag = "FormCurrencyManagement.CodigoLabel";
            this.lblCodigoMoneda.Values.Text = "Codigo:";
            this.lblCodigoMoneda.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvMonedas
            //
            this.dgvMonedas.AllowUserToAddRows = false;
            this.dgvMonedas.AllowUserToDeleteRows = false;
            this.dgvMonedas.AutoGenerateColumns = false;
            this.dgvMonedas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMonedas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMonedas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonedas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMonedaCodigo,
            this.colMonedaNombre});
            this.dgvMonedas.Location = new System.Drawing.Point(14, 84);
            this.dgvMonedas.MultiSelect = false;
            this.dgvMonedas.Name = "dgvMonedas";
            this.dgvMonedas.ReadOnly = true;
            this.dgvMonedas.RowHeadersVisible = false;
            this.dgvMonedas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMonedas.Size = new System.Drawing.Size(566, 145);
            this.dgvMonedas.TabIndex = 7;
            this.dgvMonedas.Tag = "FormCurrencyManagement.MonedaGrid";
            this.dgvMonedas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMonedas_CellClick);
            //
            // colMonedaCodigo
            //
            this.colMonedaCodigo.DataPropertyName = "Codigo";
            this.colMonedaCodigo.HeaderText = "Codigo";
            this.colMonedaCodigo.Name = "Codigo";
            this.colMonedaCodigo.ReadOnly = true;
            //
            // colMonedaNombre
            //
            this.colMonedaNombre.DataPropertyName = "Nombre";
            this.colMonedaNombre.HeaderText = "Nombre";
            this.colMonedaNombre.Name = "Nombre";
            this.colMonedaNombre.ReadOnly = true;
            //
            // groupBoxTasas
            //
            this.groupBoxTasas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTasas.Panel.Controls.Add(this.btnLiberarTasa);
            this.groupBoxTasas.Panel.Controls.Add(this.btnEliminarTasa);
            this.groupBoxTasas.Panel.Controls.Add(this.btnActualizarTasa);
            this.groupBoxTasas.Panel.Controls.Add(this.btnAgregarTasa);
            this.groupBoxTasas.Panel.Controls.Add(this.tlpCamposTasa);
            this.groupBoxTasas.Location = new System.Drawing.Point(0, 270);
            this.groupBoxTasas.Name = "groupBoxTasas";
            this.groupBoxTasas.Size = new System.Drawing.Size(594, 160);
            this.groupBoxTasas.TabIndex = 2;
            this.groupBoxTasas.TabStop = false;
            this.groupBoxTasas.Tag = "FormCurrencyManagement.TasaGroup";
            this.groupBoxTasas.Values.Heading = "Tasas de Conversion";
            this.groupBoxTasas.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // btnLiberarTasa
            //
            this.btnLiberarTasa.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnLiberarTasa.Enabled = false;
            this.btnLiberarTasa.Location = new System.Drawing.Point(374, 84);
            this.btnLiberarTasa.Name = "btnLiberarTasa";
            this.btnLiberarTasa.Size = new System.Drawing.Size(110, 30);
            this.btnLiberarTasa.TabIndex = 9;
            this.btnLiberarTasa.Tag = "FormCurrencyManagement.ReleaseButton";
            this.btnLiberarTasa.Values.Text = "Liberar";
            this.btnLiberarTasa.Click += new System.EventHandler(this.btnLiberarTasa_Click);
            //
            // btnEliminarTasa
            //
            this.btnEliminarTasa.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarTasa.Location = new System.Drawing.Point(254, 84);
            this.btnEliminarTasa.Name = "btnEliminarTasa";
            this.btnEliminarTasa.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarTasa.TabIndex = 8;
            this.btnEliminarTasa.Tag = "FormCurrencyManagement.DeleteButton";
            this.btnEliminarTasa.Values.Text = "Eliminar";
            this.btnEliminarTasa.Click += new System.EventHandler(this.btnEliminarTasa_Click);
            //
            // btnActualizarTasa
            //
            this.btnActualizarTasa.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarTasa.Location = new System.Drawing.Point(134, 84);
            this.btnActualizarTasa.Name = "btnActualizarTasa";
            this.btnActualizarTasa.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarTasa.TabIndex = 7;
            this.btnActualizarTasa.Tag = "FormCurrencyManagement.UpdateButton";
            this.btnActualizarTasa.Values.Text = "Actualizar";
            this.btnActualizarTasa.Click += new System.EventHandler(this.btnActualizarTasa_Click);
            //
            // btnAgregarTasa
            //
            this.btnAgregarTasa.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarTasa.Location = new System.Drawing.Point(14, 84);
            this.btnAgregarTasa.Name = "btnAgregarTasa";
            this.btnAgregarTasa.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarTasa.TabIndex = 6;
            this.btnAgregarTasa.Tag = "FormCurrencyManagement.AddButton";
            this.btnAgregarTasa.Values.Text = "Agregar";
            this.btnAgregarTasa.Click += new System.EventHandler(this.btnAgregarTasa_Click);
            //
            // nudCalculo
            //
            this.nudCalculo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.nudCalculo.DecimalPlaces = 6;
            this.nudCalculo.Location = new System.Drawing.Point(99, 34);
            this.nudCalculo.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nudCalculo.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudCalculo.Name = "nudCalculo";
            this.nudCalculo.Size = new System.Drawing.Size(160, 20);
            this.nudCalculo.TabIndex = 5;
            this.nudCalculo.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudCalculo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // lblCalculo
            //
            this.lblCalculo.AutoSize = true;
            this.lblCalculo.Location = new System.Drawing.Point(0, 34);
            this.lblCalculo.Name = "lblCalculo";
            this.lblCalculo.Size = new System.Drawing.Size(50, 13);
            this.lblCalculo.TabIndex = 4;
            this.lblCalculo.Tag = "FormCurrencyManagement.CalculoLabel";
            this.lblCalculo.Values.Text = "Calculo:";
            this.lblCalculo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbMonedaConversion
            //
            this.cmbMonedaConversion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMonedaConversion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonedaConversion.FormattingEnabled = true;
            this.cmbMonedaConversion.Location = new System.Drawing.Point(389, 0);
            this.cmbMonedaConversion.Name = "cmbMonedaConversion";
            this.cmbMonedaConversion.Size = new System.Drawing.Size(140, 21);
            this.cmbMonedaConversion.TabIndex = 3;
            this.cmbMonedaConversion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // lblMonedaConversion
            //
            this.lblMonedaConversion.AutoSize = true;
            this.lblMonedaConversion.Location = new System.Drawing.Point(259, 0);
            this.lblMonedaConversion.Name = "lblMonedaConversion";
            this.lblMonedaConversion.Size = new System.Drawing.Size(110, 13);
            this.lblMonedaConversion.TabIndex = 2;
            this.lblMonedaConversion.Tag = "FormCurrencyManagement.MonedaConversionComboLabel";
            this.lblMonedaConversion.Values.Text = "Moneda Conversion:";
            this.lblMonedaConversion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbMonedaBase
            //
            this.cmbMonedaBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMonedaBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonedaBase.FormattingEnabled = true;
            this.cmbMonedaBase.Location = new System.Drawing.Point(99, 0);
            this.cmbMonedaBase.Name = "cmbMonedaBase";
            this.cmbMonedaBase.Size = new System.Drawing.Size(140, 21);
            this.cmbMonedaBase.TabIndex = 1;
            this.cmbMonedaBase.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            //
            // lblMonedaBase
            //
            this.lblMonedaBase.AutoSize = true;
            this.lblMonedaBase.Location = new System.Drawing.Point(0, 0);
            this.lblMonedaBase.Name = "lblMonedaBase";
            this.lblMonedaBase.Size = new System.Drawing.Size(70, 13);
            this.lblMonedaBase.TabIndex = 0;
            this.lblMonedaBase.Tag = "FormCurrencyManagement.MonedaBaseComboLabel";
            this.lblMonedaBase.Values.Text = "Moneda Base:";
            this.lblMonedaBase.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvTasas
            //
            this.dgvTasas.AllowUserToAddRows = false;
            this.dgvTasas.AllowUserToDeleteRows = false;
            this.dgvTasas.AutoGenerateColumns = false;
            this.dgvTasas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTasas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTasas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTasas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTasaMonedaBase,
            this.colTasaMonedaConversion,
            this.colTasaCalculo,
            this.colTasaFechaActualizacion,
            this.colTasaEsManual});
            this.dgvTasas.Location = new System.Drawing.Point(0, 440);
            this.dgvTasas.MultiSelect = false;
            this.dgvTasas.Name = "dgvTasas";
            this.dgvTasas.RowHeadersVisible = false;
            this.dgvTasas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTasas.Size = new System.Drawing.Size(594, 220);
            this.dgvTasas.TabIndex = 3;
            this.dgvTasas.Tag = "FormCurrencyManagement.TasaGrid";
            this.dgvTasas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTasas_CellClick);
            this.dgvTasas.SelectionChanged += new System.EventHandler(this.dgvTasas_SelectionChanged);
            this.dgvTasas.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            //
            // colTasaMonedaBase
            //
            this.colTasaMonedaBase.DataPropertyName = "MonedaBase";
            this.colTasaMonedaBase.HeaderText = "Moneda Base";
            this.colTasaMonedaBase.Name = "MonedaBase";
            this.colTasaMonedaBase.ReadOnly = true;
            //
            // colTasaMonedaConversion
            //
            this.colTasaMonedaConversion.DataPropertyName = "MonedaConversion";
            this.colTasaMonedaConversion.HeaderText = "Moneda Conversion";
            this.colTasaMonedaConversion.Name = "MonedaConversion";
            this.colTasaMonedaConversion.ReadOnly = true;
            //
            // colTasaCalculo
            //
            this.colTasaCalculo.DataPropertyName = "Calculo";
            this.colTasaCalculo.HeaderText = "Calculo";
            this.colTasaCalculo.Name = "Calculo";
            this.colTasaCalculo.ReadOnly = true;
            //
            // colTasaFechaActualizacion
            //
            this.colTasaFechaActualizacion.DataPropertyName = "FechaActualizacion";
            this.colTasaFechaActualizacion.HeaderText = "Fecha Actualizacion";
            this.colTasaFechaActualizacion.Name = "FechaActualizacion";
            this.colTasaFechaActualizacion.ReadOnly = true;
            //
            // colTasaEsManual
            //
            this.colTasaEsManual.DataPropertyName = "EsManual";
            this.colTasaEsManual.HeaderText = "Manual";
            this.colTasaEsManual.Name = "EsManual";
            this.colTasaEsManual.ReadOnly = true;
            // 
            // tlpCamposMoneda
            // 
            this.tlpCamposMoneda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposMoneda.Controls.Add(this.txtbNombreMoneda, 3, 0);
            this.tlpCamposMoneda.Controls.Add(this.lblNombreMoneda, 2, 0);
            this.tlpCamposMoneda.Controls.Add(this.txtbCodigoMoneda, 1, 0);
            this.tlpCamposMoneda.Controls.Add(this.lblCodigoMoneda, 0, 0);
            this.tlpCamposMoneda.Location = new System.Drawing.Point(11, 9);
            this.tlpCamposMoneda.Name = "tlpCamposMoneda";
            this.tlpCamposMoneda.Size = new System.Drawing.Size(569, 25);
            this.tlpCamposMoneda.ColumnCount = 4;
            this.tlpCamposMoneda.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tlpCamposMoneda.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.05F));
            this.tlpCamposMoneda.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tlpCamposMoneda.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.95F));
            this.tlpCamposMoneda.RowCount = 1;
            this.tlpCamposMoneda.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposMoneda.TabIndex = 0;
            // 
            // tlpCamposTasa
            // 
            this.tlpCamposTasa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposTasa.Controls.Add(this.nudCalculo, 1, 1);
            this.tlpCamposTasa.Controls.Add(this.lblCalculo, 0, 1);
            this.tlpCamposTasa.Controls.Add(this.cmbMonedaConversion, 3, 0);
            this.tlpCamposTasa.Controls.Add(this.lblMonedaConversion, 2, 0);
            this.tlpCamposTasa.Controls.Add(this.cmbMonedaBase, 1, 0);
            this.tlpCamposTasa.Controls.Add(this.lblMonedaBase, 0, 0);
            this.tlpCamposTasa.Location = new System.Drawing.Point(11, 8);
            this.tlpCamposTasa.Name = "tlpCamposTasa";
            this.tlpCamposTasa.Size = new System.Drawing.Size(529, 58);
            this.tlpCamposTasa.ColumnCount = 4;
            this.tlpCamposTasa.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tlpCamposTasa.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.33F));
            this.tlpCamposTasa.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tlpCamposTasa.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.67F));
            this.tlpCamposTasa.RowCount = 2;
            this.tlpCamposTasa.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tlpCamposTasa.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpCamposTasa.TabIndex = 0;
            // 
            // tlpZonasMonedas
            // 
            this.tlpZonasMonedas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpZonasMonedas.Controls.Add(this.dgvTasas, 0, 2);
            this.tlpZonasMonedas.Controls.Add(this.groupBoxTasas, 0, 1);
            this.tlpZonasMonedas.Controls.Add(this.groupBoxMonedas, 0, 0);
            this.tlpZonasMonedas.Location = new System.Drawing.Point(23, 52);
            this.tlpZonasMonedas.Name = "tlpZonasMonedas";
            this.tlpZonasMonedas.Size = new System.Drawing.Size(594, 664);
            this.tlpZonasMonedas.ColumnCount = 1;
            this.tlpZonasMonedas.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpZonasMonedas.RowCount = 3;
            this.tlpZonasMonedas.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.66F));
            this.tlpZonasMonedas.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tlpZonasMonedas.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.34F));
            this.tlpZonasMonedas.TabIndex = 1;
            //
            // FormGestionMonedas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 730);
            this.Controls.Add(this.tlpZonasMonedas);
            this.Controls.Add(this.lblTituloMonedas);
            this.Name = "FormGestionMonedas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormCurrencyManagement.Title";
            this.Text = "Gestion de Monedas";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMonedas.Panel)).EndInit();
            this.groupBoxMonedas.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMonedas)).EndInit();
            this.groupBoxMonedas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonedas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTasas.Panel)).EndInit();
            this.groupBoxTasas.Panel.ResumeLayout(false);
            this.groupBoxTasas.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxTasas)).EndInit();
            this.groupBoxTasas.ResumeLayout(false);
            this.groupBoxTasas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalculo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasas)).EndInit();
            this.tlpCamposMoneda.ResumeLayout(false);
            this.tlpCamposMoneda.PerformLayout();
            this.tlpCamposTasa.ResumeLayout(false);
            this.tlpCamposTasa.PerformLayout();
            this.tlpZonasMonedas.ResumeLayout(false);
            this.tlpZonasMonedas.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloMonedas;
        private Krypton.Toolkit.KryptonGroupBox groupBoxMonedas;
        private Krypton.Toolkit.KryptonButton btnEliminarMoneda;
        private Krypton.Toolkit.KryptonButton btnActualizarMoneda;
        private Krypton.Toolkit.KryptonButton btnAgregarMoneda;
        private Krypton.Toolkit.KryptonTextBox txtbNombreMoneda;
        private Krypton.Toolkit.KryptonLabel lblNombreMoneda;
        private Krypton.Toolkit.KryptonTextBox txtbCodigoMoneda;
        private Krypton.Toolkit.KryptonLabel lblCodigoMoneda;
        private Krypton.Toolkit.KryptonDataGridView dgvMonedas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMonedaCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMonedaNombre;
        private Krypton.Toolkit.KryptonGroupBox groupBoxTasas;
        private Krypton.Toolkit.KryptonButton btnLiberarTasa;
        private Krypton.Toolkit.KryptonButton btnEliminarTasa;
        private Krypton.Toolkit.KryptonButton btnActualizarTasa;
        private Krypton.Toolkit.KryptonButton btnAgregarTasa;
        private System.Windows.Forms.NumericUpDown nudCalculo;
        private Krypton.Toolkit.KryptonLabel lblCalculo;
        private Krypton.Toolkit.KryptonComboBox cmbMonedaConversion;
        private Krypton.Toolkit.KryptonLabel lblMonedaConversion;
        private Krypton.Toolkit.KryptonComboBox cmbMonedaBase;
        private Krypton.Toolkit.KryptonLabel lblMonedaBase;
        private Krypton.Toolkit.KryptonDataGridView dgvTasas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaMonedaBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaMonedaConversion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaCalculo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaFechaActualizacion;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTasaEsManual;
        private System.Windows.Forms.TableLayoutPanel tlpCamposMoneda;
        private System.Windows.Forms.TableLayoutPanel tlpCamposTasa;
        private System.Windows.Forms.TableLayoutPanel tlpZonasMonedas;
    }
}
