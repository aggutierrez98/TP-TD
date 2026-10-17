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
            this.lblTituloCuentasFinancieras = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxCuentaFinanciera = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarCuenta = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarCuenta = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarCuenta = new Krypton.Toolkit.KryptonButton();
            this.clbMediosPagoContratados = new System.Windows.Forms.CheckedListBox();
            this.lblMediosPagoContratados = new Krypton.Toolkit.KryptonLabel();
            this.cmbTipoCuentaFinanciera = new Krypton.Toolkit.KryptonComboBox();
            this.lblTipoCuentaFinanciera = new Krypton.Toolkit.KryptonLabel();
            this.txtbAliasCuenta = new Krypton.Toolkit.KryptonTextBox();
            this.lblAliasCuenta = new Krypton.Toolkit.KryptonLabel();
            this.cmbEntidadFinancieraCuenta = new Krypton.Toolkit.KryptonComboBox();
            this.lblEntidadFinancieraCuenta = new Krypton.Toolkit.KryptonLabel();
            this.dgvCuentasFinancieras = new Krypton.Toolkit.KryptonDataGridView();
            this.colCuentaAlias = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuentaEntidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuentaTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCuentaMediosPago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCuentaFinanciera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCuentaFinanciera.Panel)).BeginInit();
            this.groupBoxCuentaFinanciera.Panel.SuspendLayout();
            this.groupBoxCuentaFinanciera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentasFinancieras)).BeginInit();
            this.tlpCamposCuenta = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposCuenta.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloCuentasFinancieras
            //
            this.lblTituloCuentasFinancieras.AutoSize = true;
            this.lblTituloCuentasFinancieras.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloCuentasFinancieras.Location = new System.Drawing.Point(18, 15);
            this.lblTituloCuentasFinancieras.Name = "lblTituloCuentasFinancieras";
            this.lblTituloCuentasFinancieras.Size = new System.Drawing.Size(260, 25);
            this.lblTituloCuentasFinancieras.TabIndex = 0;
            this.lblTituloCuentasFinancieras.Tag = "FormFinancialAccountManagement.TitleLabel";
            this.lblTituloCuentasFinancieras.Values.Text = "Mis Cuentas Financieras";
            //
            // groupBoxCuentaFinanciera
            //
            this.groupBoxCuentaFinanciera.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCuentaFinanciera.Panel.Controls.Add(this.btnEliminarCuenta);
            this.groupBoxCuentaFinanciera.Panel.Controls.Add(this.btnActualizarCuenta);
            this.groupBoxCuentaFinanciera.Panel.Controls.Add(this.btnAgregarCuenta);
            this.groupBoxCuentaFinanciera.Panel.Controls.Add(this.tlpCamposCuenta);
            this.groupBoxCuentaFinanciera.Location = new System.Drawing.Point(23, 52);
            this.groupBoxCuentaFinanciera.Name = "groupBoxCuentaFinanciera";
            this.groupBoxCuentaFinanciera.Size = new System.Drawing.Size(694, 250);
            this.groupBoxCuentaFinanciera.TabIndex = 1;
            this.groupBoxCuentaFinanciera.TabStop = false;
            this.groupBoxCuentaFinanciera.Tag = "FormFinancialAccountManagement.AccountGroup";
            this.groupBoxCuentaFinanciera.Values.Heading = "Nueva cuenta financiera";
            //
            // btnEliminarCuenta
            //
            this.btnEliminarCuenta.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarCuenta.Location = new System.Drawing.Point(254, 190);
            this.btnEliminarCuenta.Name = "btnEliminarCuenta";
            this.btnEliminarCuenta.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarCuenta.TabIndex = 10;
            this.btnEliminarCuenta.Tag = "FormFinancialAccountManagement.DeleteButton";
            this.btnEliminarCuenta.Values.Text = "Eliminar";
            this.btnEliminarCuenta.Click += new System.EventHandler(this.btnEliminarCuenta_Click);
            //
            // btnActualizarCuenta
            //
            this.btnActualizarCuenta.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarCuenta.Location = new System.Drawing.Point(134, 190);
            this.btnActualizarCuenta.Name = "btnActualizarCuenta";
            this.btnActualizarCuenta.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarCuenta.TabIndex = 9;
            this.btnActualizarCuenta.Tag = "FormFinancialAccountManagement.UpdateButton";
            this.btnActualizarCuenta.Values.Text = "Actualizar";
            this.btnActualizarCuenta.Click += new System.EventHandler(this.btnActualizarCuenta_Click);
            //
            // btnAgregarCuenta
            //
            this.btnAgregarCuenta.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarCuenta.Location = new System.Drawing.Point(14, 190);
            this.btnAgregarCuenta.Name = "btnAgregarCuenta";
            this.btnAgregarCuenta.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarCuenta.TabIndex = 8;
            this.btnAgregarCuenta.Tag = "FormFinancialAccountManagement.AddButton";
            this.btnAgregarCuenta.Values.Text = "Agregar";
            this.btnAgregarCuenta.Click += new System.EventHandler(this.btnAgregarCuenta_Click);
            //
            // clbMediosPagoContratados
            //
            this.clbMediosPagoContratados.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.clbMediosPagoContratados.CheckOnClick = true;
            this.clbMediosPagoContratados.FormattingEnabled = true;
            this.clbMediosPagoContratados.IntegralHeight = false;
            this.clbMediosPagoContratados.Location = new System.Drawing.Point(323, 20);
            this.clbMediosPagoContratados.Name = "clbMediosPagoContratados";
            this.clbMediosPagoContratados.Size = new System.Drawing.Size(346, 150);
            this.clbMediosPagoContratados.TabIndex = 7;
            this.clbMediosPagoContratados.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblMediosPagoContratados
            //
            this.lblMediosPagoContratados.AutoSize = true;
            this.lblMediosPagoContratados.Location = new System.Drawing.Point(320, 0);
            this.lblMediosPagoContratados.Name = "lblMediosPagoContratados";
            this.lblMediosPagoContratados.Size = new System.Drawing.Size(150, 13);
            this.lblMediosPagoContratados.TabIndex = 6;
            this.lblMediosPagoContratados.Tag = "FormFinancialAccountManagement.PaymentMethodsLabel";
            this.lblMediosPagoContratados.Values.Text = "Medios de pago contratados:";
            this.lblMediosPagoContratados.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbTipoCuentaFinanciera
            //
            this.cmbTipoCuentaFinanciera.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTipoCuentaFinanciera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoCuentaFinanciera.FormattingEnabled = true;
            this.cmbTipoCuentaFinanciera.Location = new System.Drawing.Point(3, 120);
            this.cmbTipoCuentaFinanciera.Name = "cmbTipoCuentaFinanciera";
            this.cmbTipoCuentaFinanciera.Size = new System.Drawing.Size(300, 21);
            this.cmbTipoCuentaFinanciera.TabIndex = 5;
            this.cmbTipoCuentaFinanciera.Margin = new System.Windows.Forms.Padding(3, 0, 17, 0);
            //
            // lblTipoCuentaFinanciera
            //
            this.lblTipoCuentaFinanciera.AutoSize = true;
            this.lblTipoCuentaFinanciera.Location = new System.Drawing.Point(0, 100);
            this.lblTipoCuentaFinanciera.Name = "lblTipoCuentaFinanciera";
            this.lblTipoCuentaFinanciera.Size = new System.Drawing.Size(31, 13);
            this.lblTipoCuentaFinanciera.TabIndex = 4;
            this.lblTipoCuentaFinanciera.Tag = "FormFinancialAccountManagement.TypeLabel";
            this.lblTipoCuentaFinanciera.Values.Text = "Tipo:";
            this.lblTipoCuentaFinanciera.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // txtbAliasCuenta
            //
            this.txtbAliasCuenta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbAliasCuenta.Location = new System.Drawing.Point(3, 70);
            this.txtbAliasCuenta.MaxLength = 100;
            this.txtbAliasCuenta.Name = "txtbAliasCuenta";
            this.txtbAliasCuenta.Size = new System.Drawing.Size(300, 20);
            this.txtbAliasCuenta.TabIndex = 3;
            this.txtbAliasCuenta.Margin = new System.Windows.Forms.Padding(3, 0, 17, 0);
            //
            // lblAliasCuenta
            //
            this.lblAliasCuenta.AutoSize = true;
            this.lblAliasCuenta.Location = new System.Drawing.Point(0, 50);
            this.lblAliasCuenta.Name = "lblAliasCuenta";
            this.lblAliasCuenta.Size = new System.Drawing.Size(32, 13);
            this.lblAliasCuenta.TabIndex = 2;
            this.lblAliasCuenta.Tag = "FormFinancialAccountManagement.AliasLabel";
            this.lblAliasCuenta.Values.Text = "Alias:";
            this.lblAliasCuenta.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbEntidadFinancieraCuenta
            //
            this.cmbEntidadFinancieraCuenta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEntidadFinancieraCuenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntidadFinancieraCuenta.FormattingEnabled = true;
            this.cmbEntidadFinancieraCuenta.Location = new System.Drawing.Point(3, 20);
            this.cmbEntidadFinancieraCuenta.Name = "cmbEntidadFinancieraCuenta";
            this.cmbEntidadFinancieraCuenta.Size = new System.Drawing.Size(300, 21);
            this.cmbEntidadFinancieraCuenta.TabIndex = 1;
            this.cmbEntidadFinancieraCuenta.SelectedIndexChanged += new System.EventHandler(this.cmbEntidadFinancieraCuenta_SelectedIndexChanged);
            this.cmbEntidadFinancieraCuenta.Margin = new System.Windows.Forms.Padding(3, 0, 17, 0);
            //
            // lblEntidadFinancieraCuenta
            //
            this.lblEntidadFinancieraCuenta.AutoSize = true;
            this.lblEntidadFinancieraCuenta.Location = new System.Drawing.Point(0, 0);
            this.lblEntidadFinancieraCuenta.Name = "lblEntidadFinancieraCuenta";
            this.lblEntidadFinancieraCuenta.Size = new System.Drawing.Size(98, 13);
            this.lblEntidadFinancieraCuenta.TabIndex = 0;
            this.lblEntidadFinancieraCuenta.Tag = "FormFinancialAccountManagement.EntityLabel";
            this.lblEntidadFinancieraCuenta.Values.Text = "Entidad financiera:";
            this.lblEntidadFinancieraCuenta.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvCuentasFinancieras
            //
            this.dgvCuentasFinancieras.AllowUserToAddRows = false;
            this.dgvCuentasFinancieras.AllowUserToDeleteRows = false;
            this.dgvCuentasFinancieras.AutoGenerateColumns = false;
            this.dgvCuentasFinancieras.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCuentasFinancieras.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            this.dgvCuentasFinancieras.SelectionChanged += new System.EventHandler(this.dgvCuentasFinancieras_SelectionChanged);
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
            // tlpCamposCuenta
            // 
            this.tlpCamposCuenta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposCuenta.Controls.Add(this.clbMediosPagoContratados, 1, 1);
            this.tlpCamposCuenta.SetRowSpan(this.clbMediosPagoContratados, 5);
            this.tlpCamposCuenta.Controls.Add(this.lblMediosPagoContratados, 1, 0);
            this.tlpCamposCuenta.Controls.Add(this.cmbTipoCuentaFinanciera, 0, 5);
            this.tlpCamposCuenta.Controls.Add(this.lblTipoCuentaFinanciera, 0, 4);
            this.tlpCamposCuenta.Controls.Add(this.txtbAliasCuenta, 0, 3);
            this.tlpCamposCuenta.Controls.Add(this.lblAliasCuenta, 0, 2);
            this.tlpCamposCuenta.Controls.Add(this.cmbEntidadFinancieraCuenta, 0, 1);
            this.tlpCamposCuenta.Controls.Add(this.lblEntidadFinancieraCuenta, 0, 0);
            this.tlpCamposCuenta.Location = new System.Drawing.Point(11, 8);
            this.tlpCamposCuenta.Name = "tlpCamposCuenta";
            this.tlpCamposCuenta.Size = new System.Drawing.Size(669, 174);
            this.tlpCamposCuenta.ColumnCount = 2;
            this.tlpCamposCuenta.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.83F));
            this.tlpCamposCuenta.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.17F));
            this.tlpCamposCuenta.RowCount = 6;
            this.tlpCamposCuenta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposCuenta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpCamposCuenta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposCuenta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpCamposCuenta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposCuenta.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tlpCamposCuenta.TabIndex = 0;
            //
            // FormGestionCuentasFinancieras
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 590);
            this.Controls.Add(this.dgvCuentasFinancieras);
            this.Controls.Add(this.groupBoxCuentaFinanciera);
            this.Controls.Add(this.lblTituloCuentasFinancieras);
            this.Name = "FormGestionCuentasFinancieras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormFinancialAccountManagement.Title";
            this.Text = "Mis Cuentas Financieras";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCuentaFinanciera.Panel)).EndInit();
            this.groupBoxCuentaFinanciera.Panel.ResumeLayout(false);
            this.groupBoxCuentaFinanciera.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCuentaFinanciera)).EndInit();
            this.groupBoxCuentaFinanciera.ResumeLayout(false);
            this.groupBoxCuentaFinanciera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentasFinancieras)).EndInit();
            this.tlpCamposCuenta.ResumeLayout(false);
            this.tlpCamposCuenta.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloCuentasFinancieras;
        private Krypton.Toolkit.KryptonGroupBox groupBoxCuentaFinanciera;
        private Krypton.Toolkit.KryptonButton btnEliminarCuenta;
        private Krypton.Toolkit.KryptonButton btnActualizarCuenta;
        private Krypton.Toolkit.KryptonButton btnAgregarCuenta;
        private System.Windows.Forms.CheckedListBox clbMediosPagoContratados;
        private Krypton.Toolkit.KryptonLabel lblMediosPagoContratados;
        private Krypton.Toolkit.KryptonComboBox cmbTipoCuentaFinanciera;
        private Krypton.Toolkit.KryptonLabel lblTipoCuentaFinanciera;
        private Krypton.Toolkit.KryptonTextBox txtbAliasCuenta;
        private Krypton.Toolkit.KryptonLabel lblAliasCuenta;
        private Krypton.Toolkit.KryptonComboBox cmbEntidadFinancieraCuenta;
        private Krypton.Toolkit.KryptonLabel lblEntidadFinancieraCuenta;
        private Krypton.Toolkit.KryptonDataGridView dgvCuentasFinancieras;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaAlias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaEntidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCuentaMediosPago;
        private System.Windows.Forms.TableLayoutPanel tlpCamposCuenta;
    }
}
