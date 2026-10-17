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
            this.lblTituloMonedas = new System.Windows.Forms.Label();
            this.groupBoxMonedas = new System.Windows.Forms.GroupBox();
            this.btnEliminarMoneda = new System.Windows.Forms.Button();
            this.btnActualizarMoneda = new System.Windows.Forms.Button();
            this.btnAgregarMoneda = new System.Windows.Forms.Button();
            this.txtbNombreMoneda = new System.Windows.Forms.TextBox();
            this.lblNombreMoneda = new System.Windows.Forms.Label();
            this.txtbCodigoMoneda = new System.Windows.Forms.TextBox();
            this.lblCodigoMoneda = new System.Windows.Forms.Label();
            this.dgvMonedas = new System.Windows.Forms.DataGridView();
            this.colMonedaCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMonedaNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxTasas = new System.Windows.Forms.GroupBox();
            this.btnEliminarTasa = new System.Windows.Forms.Button();
            this.btnActualizarTasa = new System.Windows.Forms.Button();
            this.btnAgregarTasa = new System.Windows.Forms.Button();
            this.nudCalculo = new System.Windows.Forms.NumericUpDown();
            this.lblCalculo = new System.Windows.Forms.Label();
            this.cmbMonedaConversion = new System.Windows.Forms.ComboBox();
            this.lblMonedaConversion = new System.Windows.Forms.Label();
            this.cmbMonedaBase = new System.Windows.Forms.ComboBox();
            this.lblMonedaBase = new System.Windows.Forms.Label();
            this.dgvTasas = new System.Windows.Forms.DataGridView();
            this.colTasaMonedaBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTasaMonedaConversion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTasaCalculo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTasaFechaActualizacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxMonedas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonedas)).BeginInit();
            this.groupBoxTasas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalculo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasas)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloMonedas
            //
            this.lblTituloMonedas.AutoSize = true;
            this.lblTituloMonedas.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloMonedas.Location = new System.Drawing.Point(18, 15);
            this.lblTituloMonedas.Name = "lblTituloMonedas";
            this.lblTituloMonedas.Size = new System.Drawing.Size(240, 25);
            this.lblTituloMonedas.TabIndex = 0;
            this.lblTituloMonedas.Tag = "FormCurrencyManagement.TitleLabel";
            this.lblTituloMonedas.Text = "Gestion de Monedas";
            //
            // groupBoxMonedas
            //
            this.groupBoxMonedas.Controls.Add(this.btnEliminarMoneda);
            this.groupBoxMonedas.Controls.Add(this.btnActualizarMoneda);
            this.groupBoxMonedas.Controls.Add(this.btnAgregarMoneda);
            this.groupBoxMonedas.Controls.Add(this.txtbNombreMoneda);
            this.groupBoxMonedas.Controls.Add(this.lblNombreMoneda);
            this.groupBoxMonedas.Controls.Add(this.txtbCodigoMoneda);
            this.groupBoxMonedas.Controls.Add(this.lblCodigoMoneda);
            this.groupBoxMonedas.Controls.Add(this.dgvMonedas);
            this.groupBoxMonedas.Location = new System.Drawing.Point(23, 52);
            this.groupBoxMonedas.Name = "groupBoxMonedas";
            this.groupBoxMonedas.Size = new System.Drawing.Size(594, 260);
            this.groupBoxMonedas.TabIndex = 1;
            this.groupBoxMonedas.TabStop = false;
            this.groupBoxMonedas.Tag = "FormCurrencyManagement.MonedaGroup";
            this.groupBoxMonedas.Text = "Monedas";
            //
            // btnEliminarMoneda
            //
            this.btnEliminarMoneda.Location = new System.Drawing.Point(254, 61);
            this.btnEliminarMoneda.Name = "btnEliminarMoneda";
            this.btnEliminarMoneda.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarMoneda.TabIndex = 6;
            this.btnEliminarMoneda.Tag = "FormCurrencyManagement.DeleteButton";
            this.btnEliminarMoneda.Text = "Eliminar";
            this.btnEliminarMoneda.UseVisualStyleBackColor = true;
            this.btnEliminarMoneda.Click += new System.EventHandler(this.btnEliminarMoneda_Click);
            //
            // btnActualizarMoneda
            //
            this.btnActualizarMoneda.Location = new System.Drawing.Point(134, 61);
            this.btnActualizarMoneda.Name = "btnActualizarMoneda";
            this.btnActualizarMoneda.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarMoneda.TabIndex = 5;
            this.btnActualizarMoneda.Tag = "FormCurrencyManagement.UpdateButton";
            this.btnActualizarMoneda.Text = "Actualizar";
            this.btnActualizarMoneda.UseVisualStyleBackColor = true;
            this.btnActualizarMoneda.Click += new System.EventHandler(this.btnActualizarMoneda_Click);
            //
            // btnAgregarMoneda
            //
            this.btnAgregarMoneda.Location = new System.Drawing.Point(14, 61);
            this.btnAgregarMoneda.Name = "btnAgregarMoneda";
            this.btnAgregarMoneda.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarMoneda.TabIndex = 4;
            this.btnAgregarMoneda.Tag = "FormCurrencyManagement.AddButton";
            this.btnAgregarMoneda.Text = "Agregar";
            this.btnAgregarMoneda.UseVisualStyleBackColor = true;
            this.btnAgregarMoneda.Click += new System.EventHandler(this.btnAgregarMoneda_Click);
            //
            // txtbNombreMoneda
            //
            this.txtbNombreMoneda.Location = new System.Drawing.Point(326, 26);
            this.txtbNombreMoneda.Name = "txtbNombreMoneda";
            this.txtbNombreMoneda.Size = new System.Drawing.Size(254, 20);
            this.txtbNombreMoneda.TabIndex = 3;
            //
            // lblNombreMoneda
            //
            this.lblNombreMoneda.AutoSize = true;
            this.lblNombreMoneda.Location = new System.Drawing.Point(270, 29);
            this.lblNombreMoneda.Name = "lblNombreMoneda";
            this.lblNombreMoneda.Size = new System.Drawing.Size(50, 13);
            this.lblNombreMoneda.TabIndex = 2;
            this.lblNombreMoneda.Tag = "FormCurrencyManagement.NombreLabel";
            this.lblNombreMoneda.Text = "Nombre:";
            //
            // txtbCodigoMoneda
            //
            this.txtbCodigoMoneda.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtbCodigoMoneda.Location = new System.Drawing.Point(70, 26);
            this.txtbCodigoMoneda.MaxLength = 3;
            this.txtbCodigoMoneda.Name = "txtbCodigoMoneda";
            this.txtbCodigoMoneda.Size = new System.Drawing.Size(180, 20);
            this.txtbCodigoMoneda.TabIndex = 1;
            //
            // lblCodigoMoneda
            //
            this.lblCodigoMoneda.AutoSize = true;
            this.lblCodigoMoneda.Location = new System.Drawing.Point(11, 29);
            this.lblCodigoMoneda.Name = "lblCodigoMoneda";
            this.lblCodigoMoneda.Size = new System.Drawing.Size(43, 13);
            this.lblCodigoMoneda.TabIndex = 0;
            this.lblCodigoMoneda.Tag = "FormCurrencyManagement.CodigoLabel";
            this.lblCodigoMoneda.Text = "Codigo:";
            //
            // dgvMonedas
            //
            this.dgvMonedas.AllowUserToAddRows = false;
            this.dgvMonedas.AllowUserToDeleteRows = false;
            this.dgvMonedas.AutoGenerateColumns = false;
            this.dgvMonedas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMonedas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonedas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMonedaCodigo,
            this.colMonedaNombre});
            this.dgvMonedas.Location = new System.Drawing.Point(14, 100);
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
            this.groupBoxTasas.Controls.Add(this.btnEliminarTasa);
            this.groupBoxTasas.Controls.Add(this.btnActualizarTasa);
            this.groupBoxTasas.Controls.Add(this.btnAgregarTasa);
            this.groupBoxTasas.Controls.Add(this.nudCalculo);
            this.groupBoxTasas.Controls.Add(this.lblCalculo);
            this.groupBoxTasas.Controls.Add(this.cmbMonedaConversion);
            this.groupBoxTasas.Controls.Add(this.lblMonedaConversion);
            this.groupBoxTasas.Controls.Add(this.cmbMonedaBase);
            this.groupBoxTasas.Controls.Add(this.lblMonedaBase);
            this.groupBoxTasas.Location = new System.Drawing.Point(23, 322);
            this.groupBoxTasas.Name = "groupBoxTasas";
            this.groupBoxTasas.Size = new System.Drawing.Size(594, 160);
            this.groupBoxTasas.TabIndex = 2;
            this.groupBoxTasas.TabStop = false;
            this.groupBoxTasas.Tag = "FormCurrencyManagement.TasaGroup";
            this.groupBoxTasas.Text = "Tasas de Conversion";
            //
            // btnEliminarTasa
            //
            this.btnEliminarTasa.Location = new System.Drawing.Point(254, 100);
            this.btnEliminarTasa.Name = "btnEliminarTasa";
            this.btnEliminarTasa.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarTasa.TabIndex = 8;
            this.btnEliminarTasa.Tag = "FormCurrencyManagement.DeleteButton";
            this.btnEliminarTasa.Text = "Eliminar";
            this.btnEliminarTasa.UseVisualStyleBackColor = true;
            this.btnEliminarTasa.Click += new System.EventHandler(this.btnEliminarTasa_Click);
            //
            // btnActualizarTasa
            //
            this.btnActualizarTasa.Location = new System.Drawing.Point(134, 100);
            this.btnActualizarTasa.Name = "btnActualizarTasa";
            this.btnActualizarTasa.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarTasa.TabIndex = 7;
            this.btnActualizarTasa.Tag = "FormCurrencyManagement.UpdateButton";
            this.btnActualizarTasa.Text = "Actualizar";
            this.btnActualizarTasa.UseVisualStyleBackColor = true;
            this.btnActualizarTasa.Click += new System.EventHandler(this.btnActualizarTasa_Click);
            //
            // btnAgregarTasa
            //
            this.btnAgregarTasa.Location = new System.Drawing.Point(14, 100);
            this.btnAgregarTasa.Name = "btnAgregarTasa";
            this.btnAgregarTasa.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarTasa.TabIndex = 6;
            this.btnAgregarTasa.Tag = "FormCurrencyManagement.AddButton";
            this.btnAgregarTasa.Text = "Agregar";
            this.btnAgregarTasa.UseVisualStyleBackColor = true;
            this.btnAgregarTasa.Click += new System.EventHandler(this.btnAgregarTasa_Click);
            //
            // nudCalculo
            //
            this.nudCalculo.DecimalPlaces = 6;
            this.nudCalculo.Location = new System.Drawing.Point(110, 58);
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
            //
            // lblCalculo
            //
            this.lblCalculo.AutoSize = true;
            this.lblCalculo.Location = new System.Drawing.Point(11, 42);
            this.lblCalculo.Name = "lblCalculo";
            this.lblCalculo.Size = new System.Drawing.Size(50, 13);
            this.lblCalculo.TabIndex = 4;
            this.lblCalculo.Tag = "FormCurrencyManagement.CalculoLabel";
            this.lblCalculo.Text = "Calculo:";
            //
            // cmbMonedaConversion
            //
            this.cmbMonedaConversion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonedaConversion.FormattingEnabled = true;
            this.cmbMonedaConversion.Location = new System.Drawing.Point(400, 24);
            this.cmbMonedaConversion.Name = "cmbMonedaConversion";
            this.cmbMonedaConversion.Size = new System.Drawing.Size(140, 21);
            this.cmbMonedaConversion.TabIndex = 3;
            //
            // lblMonedaConversion
            //
            this.lblMonedaConversion.AutoSize = true;
            this.lblMonedaConversion.Location = new System.Drawing.Point(270, 28);
            this.lblMonedaConversion.Name = "lblMonedaConversion";
            this.lblMonedaConversion.Size = new System.Drawing.Size(110, 13);
            this.lblMonedaConversion.TabIndex = 2;
            this.lblMonedaConversion.Tag = "FormCurrencyManagement.MonedaConversionComboLabel";
            this.lblMonedaConversion.Text = "Moneda Conversion:";
            //
            // cmbMonedaBase
            //
            this.cmbMonedaBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonedaBase.FormattingEnabled = true;
            this.cmbMonedaBase.Location = new System.Drawing.Point(110, 24);
            this.cmbMonedaBase.Name = "cmbMonedaBase";
            this.cmbMonedaBase.Size = new System.Drawing.Size(140, 21);
            this.cmbMonedaBase.TabIndex = 1;
            //
            // lblMonedaBase
            //
            this.lblMonedaBase.AutoSize = true;
            this.lblMonedaBase.Location = new System.Drawing.Point(11, 28);
            this.lblMonedaBase.Name = "lblMonedaBase";
            this.lblMonedaBase.Size = new System.Drawing.Size(70, 13);
            this.lblMonedaBase.TabIndex = 0;
            this.lblMonedaBase.Tag = "FormCurrencyManagement.MonedaBaseComboLabel";
            this.lblMonedaBase.Text = "Moneda Base:";
            //
            // dgvTasas
            //
            this.dgvTasas.AllowUserToAddRows = false;
            this.dgvTasas.AllowUserToDeleteRows = false;
            this.dgvTasas.AutoGenerateColumns = false;
            this.dgvTasas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTasas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTasas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTasaMonedaBase,
            this.colTasaMonedaConversion,
            this.colTasaCalculo,
            this.colTasaFechaActualizacion});
            this.dgvTasas.Location = new System.Drawing.Point(23, 492);
            this.dgvTasas.MultiSelect = false;
            this.dgvTasas.Name = "dgvTasas";
            this.dgvTasas.RowHeadersVisible = false;
            this.dgvTasas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTasas.Size = new System.Drawing.Size(594, 220);
            this.dgvTasas.TabIndex = 3;
            this.dgvTasas.Tag = "FormCurrencyManagement.TasaGrid";
            this.dgvTasas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTasas_CellClick);
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
            // FormGestionMonedas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 730);
            this.Controls.Add(this.dgvTasas);
            this.Controls.Add(this.groupBoxTasas);
            this.Controls.Add(this.groupBoxMonedas);
            this.Controls.Add(this.lblTituloMonedas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionMonedas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormCurrencyManagement.Title";
            this.Text = "Gestion de Monedas";
            this.groupBoxMonedas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonedas)).EndInit();
            this.groupBoxTasas.ResumeLayout(false);
            this.groupBoxTasas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCalculo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloMonedas;
        private System.Windows.Forms.GroupBox groupBoxMonedas;
        private System.Windows.Forms.Button btnEliminarMoneda;
        private System.Windows.Forms.Button btnActualizarMoneda;
        private System.Windows.Forms.Button btnAgregarMoneda;
        private System.Windows.Forms.TextBox txtbNombreMoneda;
        private System.Windows.Forms.Label lblNombreMoneda;
        private System.Windows.Forms.TextBox txtbCodigoMoneda;
        private System.Windows.Forms.Label lblCodigoMoneda;
        private System.Windows.Forms.DataGridView dgvMonedas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMonedaCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMonedaNombre;
        private System.Windows.Forms.GroupBox groupBoxTasas;
        private System.Windows.Forms.Button btnEliminarTasa;
        private System.Windows.Forms.Button btnActualizarTasa;
        private System.Windows.Forms.Button btnAgregarTasa;
        private System.Windows.Forms.NumericUpDown nudCalculo;
        private System.Windows.Forms.Label lblCalculo;
        private System.Windows.Forms.ComboBox cmbMonedaConversion;
        private System.Windows.Forms.Label lblMonedaConversion;
        private System.Windows.Forms.ComboBox cmbMonedaBase;
        private System.Windows.Forms.Label lblMonedaBase;
        private System.Windows.Forms.DataGridView dgvTasas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaMonedaBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaMonedaConversion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaCalculo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTasaFechaActualizacion;
    }
}
