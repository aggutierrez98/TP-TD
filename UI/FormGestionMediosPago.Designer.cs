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
            this.lblTituloMediosPago = new System.Windows.Forms.Label();
            this.groupBoxMedioPago = new System.Windows.Forms.GroupBox();
            this.btnEliminarMedioPago = new System.Windows.Forms.Button();
            this.btnEditarMedioPago = new System.Windows.Forms.Button();
            this.btnAgregarMedioPago = new System.Windows.Forms.Button();
            this.txtbMarcaMedioPago = new System.Windows.Forms.TextBox();
            this.lblMarcaMedioPago = new System.Windows.Forms.Label();
            this.cmbTipoMedioPago = new System.Windows.Forms.ComboBox();
            this.lblTipoMedioPago = new System.Windows.Forms.Label();
            this.dgvMediosPago = new System.Windows.Forms.DataGridView();
            this.colMedioPagoTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMedioPagoMarca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxMedioPago.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPago)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloMediosPago
            //
            this.lblTituloMediosPago.AutoSize = true;
            this.lblTituloMediosPago.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloMediosPago.Location = new System.Drawing.Point(18, 15);
            this.lblTituloMediosPago.Name = "lblTituloMediosPago";
            this.lblTituloMediosPago.Size = new System.Drawing.Size(260, 25);
            this.lblTituloMediosPago.TabIndex = 0;
            this.lblTituloMediosPago.Tag = "FormPaymentMethodManagement.TitleLabel";
            this.lblTituloMediosPago.Text = "Gestion de Medios de Pago";
            //
            // groupBoxMedioPago
            //
            this.groupBoxMedioPago.Controls.Add(this.btnEliminarMedioPago);
            this.groupBoxMedioPago.Controls.Add(this.btnEditarMedioPago);
            this.groupBoxMedioPago.Controls.Add(this.btnAgregarMedioPago);
            this.groupBoxMedioPago.Controls.Add(this.txtbMarcaMedioPago);
            this.groupBoxMedioPago.Controls.Add(this.lblMarcaMedioPago);
            this.groupBoxMedioPago.Controls.Add(this.cmbTipoMedioPago);
            this.groupBoxMedioPago.Controls.Add(this.lblTipoMedioPago);
            this.groupBoxMedioPago.Location = new System.Drawing.Point(23, 52);
            this.groupBoxMedioPago.Name = "groupBoxMedioPago";
            this.groupBoxMedioPago.Size = new System.Drawing.Size(594, 110);
            this.groupBoxMedioPago.TabIndex = 1;
            this.groupBoxMedioPago.TabStop = false;
            this.groupBoxMedioPago.Tag = "FormPaymentMethodManagement.MedioPagoGroup";
            this.groupBoxMedioPago.Text = "Medio de pago";
            //
            // btnEliminarMedioPago
            //
            this.btnEliminarMedioPago.Location = new System.Drawing.Point(254, 66);
            this.btnEliminarMedioPago.Name = "btnEliminarMedioPago";
            this.btnEliminarMedioPago.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarMedioPago.TabIndex = 6;
            this.btnEliminarMedioPago.Tag = "FormPaymentMethodManagement.DeleteButton";
            this.btnEliminarMedioPago.Text = "Eliminar";
            this.btnEliminarMedioPago.UseVisualStyleBackColor = true;
            this.btnEliminarMedioPago.Click += new System.EventHandler(this.btnEliminarMedioPago_Click);
            //
            // btnEditarMedioPago
            //
            this.btnEditarMedioPago.Location = new System.Drawing.Point(134, 66);
            this.btnEditarMedioPago.Name = "btnEditarMedioPago";
            this.btnEditarMedioPago.Size = new System.Drawing.Size(110, 30);
            this.btnEditarMedioPago.TabIndex = 5;
            this.btnEditarMedioPago.Tag = "FormPaymentMethodManagement.UpdateButton";
            this.btnEditarMedioPago.Text = "Actualizar";
            this.btnEditarMedioPago.UseVisualStyleBackColor = true;
            this.btnEditarMedioPago.Click += new System.EventHandler(this.btnEditarMedioPago_Click);
            //
            // btnAgregarMedioPago
            //
            this.btnAgregarMedioPago.Location = new System.Drawing.Point(14, 66);
            this.btnAgregarMedioPago.Name = "btnAgregarMedioPago";
            this.btnAgregarMedioPago.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarMedioPago.TabIndex = 4;
            this.btnAgregarMedioPago.Tag = "FormPaymentMethodManagement.AddButton";
            this.btnAgregarMedioPago.Text = "Agregar";
            this.btnAgregarMedioPago.UseVisualStyleBackColor = true;
            this.btnAgregarMedioPago.Click += new System.EventHandler(this.btnAgregarMedioPago_Click);
            //
            // txtbMarcaMedioPago
            //
            this.txtbMarcaMedioPago.Location = new System.Drawing.Point(326, 26);
            this.txtbMarcaMedioPago.MaxLength = 50;
            this.txtbMarcaMedioPago.Name = "txtbMarcaMedioPago";
            this.txtbMarcaMedioPago.Size = new System.Drawing.Size(254, 20);
            this.txtbMarcaMedioPago.TabIndex = 3;
            //
            // lblMarcaMedioPago
            //
            this.lblMarcaMedioPago.AutoSize = true;
            this.lblMarcaMedioPago.Location = new System.Drawing.Point(280, 29);
            this.lblMarcaMedioPago.Name = "lblMarcaMedioPago";
            this.lblMarcaMedioPago.Size = new System.Drawing.Size(40, 13);
            this.lblMarcaMedioPago.TabIndex = 2;
            this.lblMarcaMedioPago.Tag = "FormPaymentMethodManagement.BrandLabel";
            this.lblMarcaMedioPago.Text = "Marca:";
            //
            // cmbTipoMedioPago
            //
            this.cmbTipoMedioPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoMedioPago.FormattingEnabled = true;
            this.cmbTipoMedioPago.Location = new System.Drawing.Point(60, 26);
            this.cmbTipoMedioPago.Name = "cmbTipoMedioPago";
            this.cmbTipoMedioPago.Size = new System.Drawing.Size(200, 21);
            this.cmbTipoMedioPago.TabIndex = 1;
            //
            // lblTipoMedioPago
            //
            this.lblTipoMedioPago.AutoSize = true;
            this.lblTipoMedioPago.Location = new System.Drawing.Point(11, 29);
            this.lblTipoMedioPago.Name = "lblTipoMedioPago";
            this.lblTipoMedioPago.Size = new System.Drawing.Size(31, 13);
            this.lblTipoMedioPago.TabIndex = 0;
            this.lblTipoMedioPago.Tag = "FormPaymentMethodManagement.TypeLabel";
            this.lblTipoMedioPago.Text = "Tipo:";
            //
            // dgvMediosPago
            //
            this.dgvMediosPago.AllowUserToAddRows = false;
            this.dgvMediosPago.AllowUserToDeleteRows = false;
            this.dgvMediosPago.AutoGenerateColumns = false;
            this.dgvMediosPago.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
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
            // FormGestionMediosPago
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 470);
            this.Controls.Add(this.dgvMediosPago);
            this.Controls.Add(this.groupBoxMedioPago);
            this.Controls.Add(this.lblTituloMediosPago);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionMediosPago";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormPaymentMethodManagement.Title";
            this.Text = "Gestion de Medios de Pago";
            this.groupBoxMedioPago.ResumeLayout(false);
            this.groupBoxMedioPago.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPago)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloMediosPago;
        private System.Windows.Forms.GroupBox groupBoxMedioPago;
        private System.Windows.Forms.Button btnEliminarMedioPago;
        private System.Windows.Forms.Button btnEditarMedioPago;
        private System.Windows.Forms.Button btnAgregarMedioPago;
        private System.Windows.Forms.TextBox txtbMarcaMedioPago;
        private System.Windows.Forms.Label lblMarcaMedioPago;
        private System.Windows.Forms.ComboBox cmbTipoMedioPago;
        private System.Windows.Forms.Label lblTipoMedioPago;
        private System.Windows.Forms.DataGridView dgvMediosPago;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoMarca;
    }
}
