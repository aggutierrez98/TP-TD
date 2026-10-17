namespace TpIDS
{
    partial class FormGestionCuentasFinancieras
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
            this.lblTituloCuentasFinancieras = new System.Windows.Forms.Label();
            this.groupBoxCuentaFinanciera = new System.Windows.Forms.GroupBox();
            this.btnEliminarCuenta = new System.Windows.Forms.Button();
            this.btnAgregarCuenta = new System.Windows.Forms.Button();
            this.clbMediosPagoContratados = new System.Windows.Forms.CheckedListBox();
            this.lblMediosPagoContratados = new System.Windows.Forms.Label();
            this.cmbTipoCuentaFinanciera = new System.Windows.Forms.ComboBox();
            this.lblTipoCuentaFinanciera = new System.Windows.Forms.Label();
            this.txtbAliasCuenta = new System.Windows.Forms.TextBox();
            this.lblAliasCuenta = new System.Windows.Forms.Label();
            this.cmbEntidadFinancieraCuenta = new System.Windows.Forms.ComboBox();
            this.lblEntidadFinancieraCuenta = new System.Windows.Forms.Label();
            this.dgvCuentasFinancieras = new System.Windows.Forms.DataGridView();
            this.colCuentaAlias = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuentaEntidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuentaTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuentaMediosPago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxCuentaFinanciera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentasFinancieras)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloCuentasFinancieras
            //
            this.lblTituloCuentasFinancieras.AutoSize = true;
            this.lblTituloCuentasFinancieras.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloCuentasFinancieras.Location = new System.Drawing.Point(18, 15);
            this.lblTituloCuentasFinancieras.Name = "lblTituloCuentasFinancieras";
            this.lblTituloCuentasFinancieras.Size = new System.Drawing.Size(260, 25);
            this.lblTituloCuentasFinancieras.TabIndex = 0;
            this.lblTituloCuentasFinancieras.Tag = "FormFinancialAccountManagement.TitleLabel";
            this.lblTituloCuentasFinancieras.Text = "Mis Cuentas Financieras";
            //
            // groupBoxCuentaFinanciera
            //
            this.groupBoxCuentaFinanciera.Controls.Add(this.btnEliminarCuenta);
            this.groupBoxCuentaFinanciera.Controls.Add(this.btnAgregarCuenta);
            this.groupBoxCuentaFinanciera.Controls.Add(this.clbMediosPagoContratados);
            this.groupBoxCuentaFinanciera.Controls.Add(this.lblMediosPagoContratados);
            this.groupBoxCuentaFinanciera.Controls.Add(this.cmbTipoCuentaFinanciera);
            this.groupBoxCuentaFinanciera.Controls.Add(this.lblTipoCuentaFinanciera);
            this.groupBoxCuentaFinanciera.Controls.Add(this.txtbAliasCuenta);
            this.groupBoxCuentaFinanciera.Controls.Add(this.lblAliasCuenta);
            this.groupBoxCuentaFinanciera.Controls.Add(this.cmbEntidadFinancieraCuenta);
            this.groupBoxCuentaFinanciera.Controls.Add(this.lblEntidadFinancieraCuenta);
            this.groupBoxCuentaFinanciera.Location = new System.Drawing.Point(23, 52);
            this.groupBoxCuentaFinanciera.Name = "groupBoxCuentaFinanciera";
            this.groupBoxCuentaFinanciera.Size = new System.Drawing.Size(694, 250);
            this.groupBoxCuentaFinanciera.TabIndex = 1;
            this.groupBoxCuentaFinanciera.TabStop = false;
            this.groupBoxCuentaFinanciera.Tag = "FormFinancialAccountManagement.AccountGroup";
            this.groupBoxCuentaFinanciera.Text = "Nueva cuenta financiera";
            //
            // btnEliminarCuenta
            //
            this.btnEliminarCuenta.Location = new System.Drawing.Point(134, 206);
            this.btnEliminarCuenta.Name = "btnEliminarCuenta";
            this.btnEliminarCuenta.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarCuenta.TabIndex = 9;
            this.btnEliminarCuenta.Tag = "FormFinancialAccountManagement.DeleteButton";
            this.btnEliminarCuenta.Text = "Eliminar";
            this.btnEliminarCuenta.UseVisualStyleBackColor = true;
            this.btnEliminarCuenta.Click += new System.EventHandler(this.btnEliminarCuenta_Click);
            //
            // btnAgregarCuenta
            //
            this.btnAgregarCuenta.Location = new System.Drawing.Point(14, 206);
            this.btnAgregarCuenta.Name = "btnAgregarCuenta";
            this.btnAgregarCuenta.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarCuenta.TabIndex = 8;
            this.btnAgregarCuenta.Tag = "FormFinancialAccountManagement.AddButton";
            this.btnAgregarCuenta.Text = "Agregar";
            this.btnAgregarCuenta.UseVisualStyleBackColor = true;
            this.btnAgregarCuenta.Click += new System.EventHandler(this.btnAgregarCuenta_Click);
            //
            // clbMediosPagoContratados
            //
            this.clbMediosPagoContratados.CheckOnClick = true;
            this.clbMediosPagoContratados.FormattingEnabled = true;
            this.clbMediosPagoContratados.IntegralHeight = false;
            this.clbMediosPagoContratados.Location = new System.Drawing.Point(334, 44);
            this.clbMediosPagoContratados.Name = "clbMediosPagoContratados";
            this.clbMediosPagoContratados.Size = new System.Drawing.Size(346, 150);
            this.clbMediosPagoContratados.TabIndex = 7;
            //
            // lblMediosPagoContratados
            //
            this.lblMediosPagoContratados.AutoSize = true;
            this.lblMediosPagoContratados.Location = new System.Drawing.Point(331, 28);
            this.lblMediosPagoContratados.Name = "lblMediosPagoContratados";
            this.lblMediosPagoContratados.Size = new System.Drawing.Size(150, 13);
            this.lblMediosPagoContratados.TabIndex = 6;
            this.lblMediosPagoContratados.Tag = "FormFinancialAccountManagement.PaymentMethodsLabel";
            this.lblMediosPagoContratados.Text = "Medios de pago contratados:";
            //
            // cmbTipoCuentaFinanciera
            //
            this.cmbTipoCuentaFinanciera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoCuentaFinanciera.FormattingEnabled = true;
            this.cmbTipoCuentaFinanciera.Location = new System.Drawing.Point(14, 144);
            this.cmbTipoCuentaFinanciera.Name = "cmbTipoCuentaFinanciera";
            this.cmbTipoCuentaFinanciera.Size = new System.Drawing.Size(300, 21);
            this.cmbTipoCuentaFinanciera.TabIndex = 5;
            //
            // lblTipoCuentaFinanciera
            //
            this.lblTipoCuentaFinanciera.AutoSize = true;
            this.lblTipoCuentaFinanciera.Location = new System.Drawing.Point(11, 128);
            this.lblTipoCuentaFinanciera.Name = "lblTipoCuentaFinanciera";
            this.lblTipoCuentaFinanciera.Size = new System.Drawing.Size(31, 13);
            this.lblTipoCuentaFinanciera.TabIndex = 4;
            this.lblTipoCuentaFinanciera.Tag = "FormFinancialAccountManagement.TypeLabel";
            this.lblTipoCuentaFinanciera.Text = "Tipo:";
            //
            // txtbAliasCuenta
            //
            this.txtbAliasCuenta.Location = new System.Drawing.Point(14, 94);
            this.txtbAliasCuenta.MaxLength = 100;
            this.txtbAliasCuenta.Name = "txtbAliasCuenta";
            this.txtbAliasCuenta.Size = new System.Drawing.Size(300, 20);
            this.txtbAliasCuenta.TabIndex = 3;
            //
            // lblAliasCuenta
            //
            this.lblAliasCuenta.AutoSize = true;
            this.lblAliasCuenta.Location = new System.Drawing.Point(11, 78);
            this.lblAliasCuenta.Name = "lblAliasCuenta";
            this.lblAliasCuenta.Size = new System.Drawing.Size(32, 13);
            this.lblAliasCuenta.TabIndex = 2;
            this.lblAliasCuenta.Tag = "FormFinancialAccountManagement.AliasLabel";
            this.lblAliasCuenta.Text = "Alias:";
            //
            // cmbEntidadFinancieraCuenta
            //
            this.cmbEntidadFinancieraCuenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntidadFinancieraCuenta.FormattingEnabled = true;
            this.cmbEntidadFinancieraCuenta.Location = new System.Drawing.Point(14, 44);
            this.cmbEntidadFinancieraCuenta.Name = "cmbEntidadFinancieraCuenta";
            this.cmbEntidadFinancieraCuenta.Size = new System.Drawing.Size(300, 21);
            this.cmbEntidadFinancieraCuenta.TabIndex = 1;
            this.cmbEntidadFinancieraCuenta.SelectedIndexChanged += new System.EventHandler(this.cmbEntidadFinancieraCuenta_SelectedIndexChanged);
            //
            // lblEntidadFinancieraCuenta
            //
            this.lblEntidadFinancieraCuenta.AutoSize = true;
            this.lblEntidadFinancieraCuenta.Location = new System.Drawing.Point(11, 28);
            this.lblEntidadFinancieraCuenta.Name = "lblEntidadFinancieraCuenta";
            this.lblEntidadFinancieraCuenta.Size = new System.Drawing.Size(98, 13);
            this.lblEntidadFinancieraCuenta.TabIndex = 0;
            this.lblEntidadFinancieraCuenta.Tag = "FormFinancialAccountManagement.EntityLabel";
            this.lblEntidadFinancieraCuenta.Text = "Entidad financiera:";
            //
            // dgvCuentasFinancieras
            //
            this.dgvCuentasFinancieras.AllowUserToAddRows = false;
            this.dgvCuentasFinancieras.AllowUserToDeleteRows = false;
            this.dgvCuentasFinancieras.AutoGenerateColumns = false;
            this.dgvCuentasFinancieras.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCuentasFinancieras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCuentasFinancieras.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCuentaAlias,
            this.colCuentaEntidad,
            this.colCuentaTipo,
            this.colCuentaMediosPago});
            this.dgvCuentasFinancieras.Location = new System.Drawing.Point(23, 312);
            this.dgvCuentasFinancieras.MultiSelect = false;
            this.dgvCuentasFinancieras.Name = "dgvCuentasFinancieras";
            this.dgvCuentasFinancieras.ReadOnly = true;
            this.dgvCuentasFinancieras.RowHeadersVisible = false;
            this.dgvCuentasFinancieras.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCuentasFinancieras.Size = new System.Drawing.Size(694, 260);
            this.dgvCuentasFinancieras.TabIndex = 2;
            this.dgvCuentasFinancieras.Tag = "FormFinancialAccountManagement.AccountGrid";
            //
            // colCuentaAlias
            //
            this.colCuentaAlias.DataPropertyName = "Alias";
            this.colCuentaAlias.HeaderText = "Alias";
            this.colCuentaAlias.Name = "Alias";
            this.colCuentaAlias.ReadOnly = true;
            //
            // colCuentaEntidad
            //
            this.colCuentaEntidad.DataPropertyName = "Entidad";
            this.colCuentaEntidad.HeaderText = "Entidad";
            this.colCuentaEntidad.Name = "Entidad";
            this.colCuentaEntidad.ReadOnly = true;
            //
            // colCuentaTipo
            //
            this.colCuentaTipo.DataPropertyName = "Tipo";
            this.colCuentaTipo.HeaderText = "Tipo";
            this.colCuentaTipo.Name = "Tipo";
            this.colCuentaTipo.ReadOnly = true;
            //
            // colCuentaMediosPago
            //
            this.colCuentaMediosPago.DataPropertyName = "MediosPago";
            this.colCuentaMediosPago.FillWeight = 200F;
            this.colCuentaMediosPago.HeaderText = "Medios de pago";
            this.colCuentaMediosPago.Name = "MediosPago";
            this.colCuentaMediosPago.ReadOnly = true;
            //
            // FormGestionCuentasFinancieras
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 590);
            this.Controls.Add(this.dgvCuentasFinancieras);
            this.Controls.Add(this.groupBoxCuentaFinanciera);
            this.Controls.Add(this.lblTituloCuentasFinancieras);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionCuentasFinancieras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormFinancialAccountManagement.Title";
            this.Text = "Mis Cuentas Financieras";
            this.groupBoxCuentaFinanciera.ResumeLayout(false);
            this.groupBoxCuentaFinanciera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentasFinancieras)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloCuentasFinancieras;
        private System.Windows.Forms.GroupBox groupBoxCuentaFinanciera;
        private System.Windows.Forms.Button btnEliminarCuenta;
        private System.Windows.Forms.Button btnAgregarCuenta;
        private System.Windows.Forms.CheckedListBox clbMediosPagoContratados;
        private System.Windows.Forms.Label lblMediosPagoContratados;
        private System.Windows.Forms.ComboBox cmbTipoCuentaFinanciera;
        private System.Windows.Forms.Label lblTipoCuentaFinanciera;
        private System.Windows.Forms.TextBox txtbAliasCuenta;
        private System.Windows.Forms.Label lblAliasCuenta;
        private System.Windows.Forms.ComboBox cmbEntidadFinancieraCuenta;
        private System.Windows.Forms.Label lblEntidadFinancieraCuenta;
        private System.Windows.Forms.DataGridView dgvCuentasFinancieras;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaAlias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaEntidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaMediosPago;
    }
}
