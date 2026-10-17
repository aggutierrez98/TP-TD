namespace TpIDS
{
    partial class FormGestionMediosPago
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
            this.lblTituloMediosPago = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxMedioPago = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarMedioPago = new Krypton.Toolkit.KryptonButton();
            this.btnEditarMedioPago = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarMedioPago = new Krypton.Toolkit.KryptonButton();
            this.txtbMarcaMedioPago = new Krypton.Toolkit.KryptonTextBox();
            this.lblMarcaMedioPago = new Krypton.Toolkit.KryptonLabel();
            this.cmbTipoMedioPago = new Krypton.Toolkit.KryptonComboBox();
            this.lblTipoMedioPago = new Krypton.Toolkit.KryptonLabel();
            this.dgvMediosPago = new Krypton.Toolkit.KryptonDataGridView();
            this.colMedioPagoTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMedioPagoMarca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPago)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPago.Panel)).BeginInit();
            this.groupBoxMedioPago.Panel.SuspendLayout();
            this.groupBoxMedioPago.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPago)).BeginInit();
            this.tlpCamposMedioPago = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposMedioPago.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloMediosPago
            //
            this.lblTituloMediosPago.AutoSize = true;
            this.lblTituloMediosPago.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloMediosPago.Location = new System.Drawing.Point(18, 15);
            this.lblTituloMediosPago.Name = "lblTituloMediosPago";
            this.lblTituloMediosPago.Size = new System.Drawing.Size(260, 25);
            this.lblTituloMediosPago.TabIndex = 0;
            this.lblTituloMediosPago.Tag = "FormPaymentMethodManagement.TitleLabel";
            this.lblTituloMediosPago.Values.Text = "Gestion de Medios de Pago";
            //
            // groupBoxMedioPago
            //
            this.groupBoxMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMedioPago.Panel.Controls.Add(this.btnEliminarMedioPago);
            this.groupBoxMedioPago.Panel.Controls.Add(this.btnEditarMedioPago);
            this.groupBoxMedioPago.Panel.Controls.Add(this.btnAgregarMedioPago);
            this.groupBoxMedioPago.Panel.Controls.Add(this.tlpCamposMedioPago);
            this.groupBoxMedioPago.Location = new System.Drawing.Point(23, 52);
            this.groupBoxMedioPago.Name = "groupBoxMedioPago";
            this.groupBoxMedioPago.Size = new System.Drawing.Size(594, 110);
            this.groupBoxMedioPago.TabIndex = 1;
            this.groupBoxMedioPago.TabStop = false;
            this.groupBoxMedioPago.Tag = "FormPaymentMethodManagement.MedioPagoGroup";
            this.groupBoxMedioPago.Values.Heading = "Medio de pago";
            //
            // btnEliminarMedioPago
            //
            this.btnEliminarMedioPago.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarMedioPago.Location = new System.Drawing.Point(254, 50);
            this.btnEliminarMedioPago.Name = "btnEliminarMedioPago";
            this.btnEliminarMedioPago.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarMedioPago.TabIndex = 6;
            this.btnEliminarMedioPago.Tag = "FormPaymentMethodManagement.DeleteButton";
            this.btnEliminarMedioPago.Values.Text = "Eliminar";
            this.btnEliminarMedioPago.Click += new System.EventHandler(this.btnEliminarMedioPago_Click);
            //
            // btnEditarMedioPago
            //
            this.btnEditarMedioPago.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEditarMedioPago.Location = new System.Drawing.Point(134, 50);
            this.btnEditarMedioPago.Name = "btnEditarMedioPago";
            this.btnEditarMedioPago.Size = new System.Drawing.Size(110, 30);
            this.btnEditarMedioPago.TabIndex = 5;
            this.btnEditarMedioPago.Tag = "FormPaymentMethodManagement.UpdateButton";
            this.btnEditarMedioPago.Values.Text = "Actualizar";
            this.btnEditarMedioPago.Click += new System.EventHandler(this.btnEditarMedioPago_Click);
            //
            // btnAgregarMedioPago
            //
            this.btnAgregarMedioPago.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarMedioPago.Location = new System.Drawing.Point(14, 50);
            this.btnAgregarMedioPago.Name = "btnAgregarMedioPago";
            this.btnAgregarMedioPago.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarMedioPago.TabIndex = 4;
            this.btnAgregarMedioPago.Tag = "FormPaymentMethodManagement.AddButton";
            this.btnAgregarMedioPago.Values.Text = "Agregar";
            this.btnAgregarMedioPago.Click += new System.EventHandler(this.btnAgregarMedioPago_Click);
            //
            // txtbMarcaMedioPago
            //
            this.txtbMarcaMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbMarcaMedioPago.Location = new System.Drawing.Point(315, 1);
            this.txtbMarcaMedioPago.MaxLength = 50;
            this.txtbMarcaMedioPago.Name = "txtbMarcaMedioPago";
            this.txtbMarcaMedioPago.Size = new System.Drawing.Size(254, 20);
            this.txtbMarcaMedioPago.TabIndex = 3;
            this.txtbMarcaMedioPago.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            //
            // lblMarcaMedioPago
            //
            this.lblMarcaMedioPago.AutoSize = true;
            this.lblMarcaMedioPago.Location = new System.Drawing.Point(269, 0);
            this.lblMarcaMedioPago.Name = "lblMarcaMedioPago";
            this.lblMarcaMedioPago.Size = new System.Drawing.Size(40, 13);
            this.lblMarcaMedioPago.TabIndex = 2;
            this.lblMarcaMedioPago.Tag = "FormPaymentMethodManagement.BrandLabel";
            this.lblMarcaMedioPago.Values.Text = "Marca:";
            this.lblMarcaMedioPago.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbTipoMedioPago
            //
            this.cmbTipoMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTipoMedioPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoMedioPago.FormattingEnabled = true;
            this.cmbTipoMedioPago.Location = new System.Drawing.Point(49, 1);
            this.cmbTipoMedioPago.Name = "cmbTipoMedioPago";
            this.cmbTipoMedioPago.Size = new System.Drawing.Size(200, 21);
            this.cmbTipoMedioPago.TabIndex = 1;
            this.cmbTipoMedioPago.Margin = new System.Windows.Forms.Padding(0, 1, 20, 0);
            //
            // lblTipoMedioPago
            //
            this.lblTipoMedioPago.AutoSize = true;
            this.lblTipoMedioPago.Location = new System.Drawing.Point(0, 0);
            this.lblTipoMedioPago.Name = "lblTipoMedioPago";
            this.lblTipoMedioPago.Size = new System.Drawing.Size(31, 13);
            this.lblTipoMedioPago.TabIndex = 0;
            this.lblTipoMedioPago.Tag = "FormPaymentMethodManagement.TypeLabel";
            this.lblTipoMedioPago.Values.Text = "Tipo:";
            this.lblTipoMedioPago.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvMediosPago
            //
            this.dgvMediosPago.AllowUserToAddRows = false;
            this.dgvMediosPago.AllowUserToDeleteRows = false;
            this.dgvMediosPago.AutoGenerateColumns = false;
            this.dgvMediosPago.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMediosPago.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMediosPago.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMediosPago.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMedioPagoTipo,
            this.colMedioPagoMarca});
            this.dgvMediosPago.Location = new System.Drawing.Point(23, 172);
            this.dgvMediosPago.MultiSelect = false;
            this.dgvMediosPago.Name = "dgvMediosPago";
            this.dgvMediosPago.ReadOnly = true;
            this.dgvMediosPago.RowHeadersVisible = false;
            this.dgvMediosPago.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMediosPago.Size = new System.Drawing.Size(594, 280);
            this.dgvMediosPago.TabIndex = 2;
            this.dgvMediosPago.Tag = "FormPaymentMethodManagement.MedioPagoGrid";
            this.dgvMediosPago.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMediosPago_CellClick);
            //
            // colMedioPagoTipo
            //
            this.colMedioPagoTipo.DataPropertyName = "Tipo";
            this.colMedioPagoTipo.HeaderText = "Tipo";
            this.colMedioPagoTipo.Name = "Tipo";
            this.colMedioPagoTipo.ReadOnly = true;
            //
            // colMedioPagoMarca
            //
            this.colMedioPagoMarca.DataPropertyName = "Marca";
            this.colMedioPagoMarca.HeaderText = "Marca";
            this.colMedioPagoMarca.Name = "Marca";
            this.colMedioPagoMarca.ReadOnly = true;
            // 
            // tlpCamposMedioPago
            // 
            this.tlpCamposMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposMedioPago.Controls.Add(this.txtbMarcaMedioPago, 3, 0);
            this.tlpCamposMedioPago.Controls.Add(this.lblMarcaMedioPago, 2, 0);
            this.tlpCamposMedioPago.Controls.Add(this.cmbTipoMedioPago, 1, 0);
            this.tlpCamposMedioPago.Controls.Add(this.lblTipoMedioPago, 0, 0);
            this.tlpCamposMedioPago.Location = new System.Drawing.Point(11, 9);
            this.tlpCamposMedioPago.Name = "tlpCamposMedioPago";
            this.tlpCamposMedioPago.Size = new System.Drawing.Size(569, 26);
            this.tlpCamposMedioPago.ColumnCount = 4;
            this.tlpCamposMedioPago.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tlpCamposMedioPago.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.41F));
            this.tlpCamposMedioPago.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tlpCamposMedioPago.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.59F));
            this.tlpCamposMedioPago.RowCount = 1;
            this.tlpCamposMedioPago.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tlpCamposMedioPago.TabIndex = 0;
            //
            // FormGestionMediosPago
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 470);
            this.Controls.Add(this.dgvMediosPago);
            this.Controls.Add(this.groupBoxMedioPago);
            this.Controls.Add(this.lblTituloMediosPago);
            this.Name = "FormGestionMediosPago";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPaymentMethodManagement.Title";
            this.Text = "Gestion de Medios de Pago";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPago.Panel)).EndInit();
            this.groupBoxMedioPago.Panel.ResumeLayout(false);
            this.groupBoxMedioPago.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPago)).EndInit();
            this.groupBoxMedioPago.ResumeLayout(false);
            this.groupBoxMedioPago.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPago)).EndInit();
            this.tlpCamposMedioPago.ResumeLayout(false);
            this.tlpCamposMedioPago.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloMediosPago;
        private Krypton.Toolkit.KryptonGroupBox groupBoxMedioPago;
        private Krypton.Toolkit.KryptonButton btnEliminarMedioPago;
        private Krypton.Toolkit.KryptonButton btnEditarMedioPago;
        private Krypton.Toolkit.KryptonButton btnAgregarMedioPago;
        private Krypton.Toolkit.KryptonTextBox txtbMarcaMedioPago;
        private Krypton.Toolkit.KryptonLabel lblMarcaMedioPago;
        private Krypton.Toolkit.KryptonComboBox cmbTipoMedioPago;
        private Krypton.Toolkit.KryptonLabel lblTipoMedioPago;
        private Krypton.Toolkit.KryptonDataGridView dgvMediosPago;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoMarca;
        private System.Windows.Forms.TableLayoutPanel tlpCamposMedioPago;
    }
}
