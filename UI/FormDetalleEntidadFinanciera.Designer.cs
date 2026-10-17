namespace TpIDS
{
    partial class FormDetalleEntidadFinanciera
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
            this.lblTituloDetalleEntidadFinanciera = new System.Windows.Forms.Label();
            this.pnlEntidadFinancieraDetalle = new System.Windows.Forms.FlowLayoutPanel();
            this.lblEntidadFinancieraPrefijo = new System.Windows.Forms.Label();
            this.lblEntidadFinancieraNombreDetalle = new System.Windows.Forms.Label();
            this.groupBoxMedioPagoEntidad = new System.Windows.Forms.GroupBox();
            this.btnQuitarMedioPagoEntidad = new System.Windows.Forms.Button();
            this.btnAgregarMedioPagoEntidad = new System.Windows.Forms.Button();
            this.txtbSegmentoMedioPagoEntidad = new System.Windows.Forms.TextBox();
            this.lblSegmentoMedioPagoEntidad = new System.Windows.Forms.Label();
            this.cmbMedioPagoEntidad = new System.Windows.Forms.ComboBox();
            this.lblMedioPagoEntidad = new System.Windows.Forms.Label();
            this.dgvMediosPagoEntidad = new System.Windows.Forms.DataGridView();
            this.colMedioPagoEntidadTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMedioPagoEntidadMarca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMedioPagoEntidadSegmento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlEntidadFinancieraDetalle.SuspendLayout();
            this.groupBoxMedioPagoEntidad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPagoEntidad)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloDetalleEntidadFinanciera
            //
            this.lblTituloDetalleEntidadFinanciera.AutoSize = true;
            this.lblTituloDetalleEntidadFinanciera.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloDetalleEntidadFinanciera.Location = new System.Drawing.Point(18, 15);
            this.lblTituloDetalleEntidadFinanciera.Name = "lblTituloDetalleEntidadFinanciera";
            this.lblTituloDetalleEntidadFinanciera.Size = new System.Drawing.Size(300, 25);
            this.lblTituloDetalleEntidadFinanciera.TabIndex = 0;
            this.lblTituloDetalleEntidadFinanciera.Tag = "FormFinancialEntityDetail.TitleLabel";
            this.lblTituloDetalleEntidadFinanciera.Text = "Detalle de la entidad financiera";
            //
            // pnlEntidadFinancieraDetalle
            //
            this.pnlEntidadFinancieraDetalle.AutoSize = true;
            this.pnlEntidadFinancieraDetalle.Controls.Add(this.lblEntidadFinancieraPrefijo);
            this.pnlEntidadFinancieraDetalle.Controls.Add(this.lblEntidadFinancieraNombreDetalle);
            this.pnlEntidadFinancieraDetalle.Location = new System.Drawing.Point(20, 45);
            this.pnlEntidadFinancieraDetalle.Name = "pnlEntidadFinancieraDetalle";
            this.pnlEntidadFinancieraDetalle.Size = new System.Drawing.Size(60, 13);
            this.pnlEntidadFinancieraDetalle.TabIndex = 1;
            this.pnlEntidadFinancieraDetalle.WrapContents = false;
            //
            // lblEntidadFinancieraPrefijo
            //
            this.lblEntidadFinancieraPrefijo.AutoSize = true;
            this.lblEntidadFinancieraPrefijo.Margin = new System.Windows.Forms.Padding(0);
            this.lblEntidadFinancieraPrefijo.Name = "lblEntidadFinancieraPrefijo";
            this.lblEntidadFinancieraPrefijo.Size = new System.Drawing.Size(46, 13);
            this.lblEntidadFinancieraPrefijo.TabIndex = 0;
            this.lblEntidadFinancieraPrefijo.Tag = "FormFinancialEntityDetail.EntityPrefix";
            this.lblEntidadFinancieraPrefijo.Text = "Entidad:";
            //
            // lblEntidadFinancieraNombreDetalle
            //
            this.lblEntidadFinancieraNombreDetalle.AutoSize = true;
            this.lblEntidadFinancieraNombreDetalle.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblEntidadFinancieraNombreDetalle.Name = "lblEntidadFinancieraNombreDetalle";
            this.lblEntidadFinancieraNombreDetalle.Size = new System.Drawing.Size(10, 13);
            this.lblEntidadFinancieraNombreDetalle.TabIndex = 1;
            this.lblEntidadFinancieraNombreDetalle.Text = "-";
            //
            // groupBoxMedioPagoEntidad
            //
            this.groupBoxMedioPagoEntidad.Controls.Add(this.btnQuitarMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Controls.Add(this.btnAgregarMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Controls.Add(this.txtbSegmentoMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Controls.Add(this.lblSegmentoMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Controls.Add(this.cmbMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Controls.Add(this.lblMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Location = new System.Drawing.Point(23, 75);
            this.groupBoxMedioPagoEntidad.Name = "groupBoxMedioPagoEntidad";
            this.groupBoxMedioPagoEntidad.Size = new System.Drawing.Size(594, 120);
            this.groupBoxMedioPagoEntidad.TabIndex = 2;
            this.groupBoxMedioPagoEntidad.TabStop = false;
            this.groupBoxMedioPagoEntidad.Tag = "FormFinancialEntityDetail.PaymentMethodGroup";
            this.groupBoxMedioPagoEntidad.Text = "Medios de pago de esta entidad";
            //
            // btnQuitarMedioPagoEntidad
            //
            this.btnQuitarMedioPagoEntidad.Location = new System.Drawing.Point(134, 76);
            this.btnQuitarMedioPagoEntidad.Name = "btnQuitarMedioPagoEntidad";
            this.btnQuitarMedioPagoEntidad.Size = new System.Drawing.Size(110, 30);
            this.btnQuitarMedioPagoEntidad.TabIndex = 5;
            this.btnQuitarMedioPagoEntidad.Tag = "FormFinancialEntityDetail.RemoveButton";
            this.btnQuitarMedioPagoEntidad.Text = "Quitar";
            this.btnQuitarMedioPagoEntidad.UseVisualStyleBackColor = true;
            this.btnQuitarMedioPagoEntidad.Click += new System.EventHandler(this.btnQuitarMedioPagoEntidad_Click);
            //
            // btnAgregarMedioPagoEntidad
            //
            this.btnAgregarMedioPagoEntidad.Location = new System.Drawing.Point(14, 76);
            this.btnAgregarMedioPagoEntidad.Name = "btnAgregarMedioPagoEntidad";
            this.btnAgregarMedioPagoEntidad.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarMedioPagoEntidad.TabIndex = 4;
            this.btnAgregarMedioPagoEntidad.Tag = "FormFinancialEntityDetail.AddButton";
            this.btnAgregarMedioPagoEntidad.Text = "Agregar";
            this.btnAgregarMedioPagoEntidad.UseVisualStyleBackColor = true;
            this.btnAgregarMedioPagoEntidad.Click += new System.EventHandler(this.btnAgregarMedioPagoEntidad_Click);
            //
            // txtbSegmentoMedioPagoEntidad
            //
            this.txtbSegmentoMedioPagoEntidad.Location = new System.Drawing.Point(334, 44);
            this.txtbSegmentoMedioPagoEntidad.MaxLength = 50;
            this.txtbSegmentoMedioPagoEntidad.Name = "txtbSegmentoMedioPagoEntidad";
            this.txtbSegmentoMedioPagoEntidad.Size = new System.Drawing.Size(246, 20);
            this.txtbSegmentoMedioPagoEntidad.TabIndex = 3;
            //
            // lblSegmentoMedioPagoEntidad
            //
            this.lblSegmentoMedioPagoEntidad.AutoSize = true;
            this.lblSegmentoMedioPagoEntidad.Location = new System.Drawing.Point(331, 28);
            this.lblSegmentoMedioPagoEntidad.Name = "lblSegmentoMedioPagoEntidad";
            this.lblSegmentoMedioPagoEntidad.Size = new System.Drawing.Size(58, 13);
            this.lblSegmentoMedioPagoEntidad.TabIndex = 2;
            this.lblSegmentoMedioPagoEntidad.Tag = "FormFinancialEntityDetail.SegmentLabel";
            this.lblSegmentoMedioPagoEntidad.Text = "Segmento:";
            //
            // cmbMedioPagoEntidad
            //
            this.cmbMedioPagoEntidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMedioPagoEntidad.FormattingEnabled = true;
            this.cmbMedioPagoEntidad.Location = new System.Drawing.Point(14, 44);
            this.cmbMedioPagoEntidad.Name = "cmbMedioPagoEntidad";
            this.cmbMedioPagoEntidad.Size = new System.Drawing.Size(300, 21);
            this.cmbMedioPagoEntidad.TabIndex = 1;
            //
            // lblMedioPagoEntidad
            //
            this.lblMedioPagoEntidad.AutoSize = true;
            this.lblMedioPagoEntidad.Location = new System.Drawing.Point(11, 28);
            this.lblMedioPagoEntidad.Name = "lblMedioPagoEntidad";
            this.lblMedioPagoEntidad.Size = new System.Drawing.Size(84, 13);
            this.lblMedioPagoEntidad.TabIndex = 0;
            this.lblMedioPagoEntidad.Tag = "FormFinancialEntityDetail.PaymentMethodLabel";
            this.lblMedioPagoEntidad.Text = "Medio de pago:";
            //
            // dgvMediosPagoEntidad
            //
            this.dgvMediosPagoEntidad.AllowUserToAddRows = false;
            this.dgvMediosPagoEntidad.AllowUserToDeleteRows = false;
            this.dgvMediosPagoEntidad.AutoGenerateColumns = false;
            this.dgvMediosPagoEntidad.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMediosPagoEntidad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMediosPagoEntidad.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMedioPagoEntidadTipo,
            this.colMedioPagoEntidadMarca,
            this.colMedioPagoEntidadSegmento});
            this.dgvMediosPagoEntidad.Location = new System.Drawing.Point(23, 205);
            this.dgvMediosPagoEntidad.MultiSelect = false;
            this.dgvMediosPagoEntidad.Name = "dgvMediosPagoEntidad";
            this.dgvMediosPagoEntidad.ReadOnly = true;
            this.dgvMediosPagoEntidad.RowHeadersVisible = false;
            this.dgvMediosPagoEntidad.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMediosPagoEntidad.Size = new System.Drawing.Size(594, 250);
            this.dgvMediosPagoEntidad.TabIndex = 3;
            this.dgvMediosPagoEntidad.Tag = "FormFinancialEntityDetail.PaymentMethodGrid";
            //
            // colMedioPagoEntidadTipo
            //
            this.colMedioPagoEntidadTipo.DataPropertyName = "Tipo";
            this.colMedioPagoEntidadTipo.HeaderText = "Tipo";
            this.colMedioPagoEntidadTipo.Name = "Tipo";
            this.colMedioPagoEntidadTipo.ReadOnly = true;
            //
            // colMedioPagoEntidadMarca
            //
            this.colMedioPagoEntidadMarca.DataPropertyName = "Marca";
            this.colMedioPagoEntidadMarca.HeaderText = "Marca";
            this.colMedioPagoEntidadMarca.Name = "Marca";
            this.colMedioPagoEntidadMarca.ReadOnly = true;
            //
            // colMedioPagoEntidadSegmento
            //
            this.colMedioPagoEntidadSegmento.DataPropertyName = "Segmento";
            this.colMedioPagoEntidadSegmento.HeaderText = "Segmento";
            this.colMedioPagoEntidadSegmento.Name = "Segmento";
            this.colMedioPagoEntidadSegmento.ReadOnly = true;
            //
            // FormDetalleEntidadFinanciera
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 475);
            this.Controls.Add(this.dgvMediosPagoEntidad);
            this.Controls.Add(this.groupBoxMedioPagoEntidad);
            this.Controls.Add(this.pnlEntidadFinancieraDetalle);
            this.Controls.Add(this.lblTituloDetalleEntidadFinanciera);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormDetalleEntidadFinanciera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormFinancialEntityDetail.Title";
            this.Text = "Detalle de la entidad financiera";
            this.pnlEntidadFinancieraDetalle.ResumeLayout(false);
            this.pnlEntidadFinancieraDetalle.PerformLayout();
            this.groupBoxMedioPagoEntidad.ResumeLayout(false);
            this.groupBoxMedioPagoEntidad.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPagoEntidad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloDetalleEntidadFinanciera;
        private System.Windows.Forms.FlowLayoutPanel pnlEntidadFinancieraDetalle;
        private System.Windows.Forms.Label lblEntidadFinancieraPrefijo;
        private System.Windows.Forms.Label lblEntidadFinancieraNombreDetalle;
        private System.Windows.Forms.GroupBox groupBoxMedioPagoEntidad;
        private System.Windows.Forms.Button btnQuitarMedioPagoEntidad;
        private System.Windows.Forms.Button btnAgregarMedioPagoEntidad;
        private System.Windows.Forms.TextBox txtbSegmentoMedioPagoEntidad;
        private System.Windows.Forms.Label lblSegmentoMedioPagoEntidad;
        private System.Windows.Forms.ComboBox cmbMedioPagoEntidad;
        private System.Windows.Forms.Label lblMedioPagoEntidad;
        private System.Windows.Forms.DataGridView dgvMediosPagoEntidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoEntidadTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoEntidadMarca;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoEntidadSegmento;
    }
}
