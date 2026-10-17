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
            this.lblTituloDetalleEntidadFinanciera = new Krypton.Toolkit.KryptonLabel();
            this.pnlEntidadFinancieraDetalle = new System.Windows.Forms.FlowLayoutPanel();
            this.lblEntidadFinancieraPrefijo = new Krypton.Toolkit.KryptonLabel();
            this.lblEntidadFinancieraNombreDetalle = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxMedioPagoEntidad = new Krypton.Toolkit.KryptonGroupBox();
            this.btnQuitarMedioPagoEntidad = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarMedioPagoEntidad = new Krypton.Toolkit.KryptonButton();
            this.txtbSegmentoMedioPagoEntidad = new Krypton.Toolkit.KryptonTextBox();
            this.lblSegmentoMedioPagoEntidad = new Krypton.Toolkit.KryptonLabel();
            this.cmbMedioPagoEntidad = new Krypton.Toolkit.KryptonComboBox();
            this.lblMedioPagoEntidad = new Krypton.Toolkit.KryptonLabel();
            this.dgvMediosPagoEntidad = new Krypton.Toolkit.KryptonDataGridView();
            this.colMedioPagoEntidadTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMedioPagoEntidadMarca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMedioPagoEntidadSegmento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlEntidadFinancieraDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPagoEntidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPagoEntidad.Panel)).BeginInit();
            this.groupBoxMedioPagoEntidad.Panel.SuspendLayout();
            this.groupBoxMedioPagoEntidad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPagoEntidad)).BeginInit();
            this.tlpCamposMedioPagoEntidad = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposMedioPagoEntidad.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloDetalleEntidadFinanciera
            //
            this.lblTituloDetalleEntidadFinanciera.AutoSize = true;
            this.lblTituloDetalleEntidadFinanciera.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloDetalleEntidadFinanciera.Location = new System.Drawing.Point(18, 15);
            this.lblTituloDetalleEntidadFinanciera.Name = "lblTituloDetalleEntidadFinanciera";
            this.lblTituloDetalleEntidadFinanciera.Size = new System.Drawing.Size(300, 25);
            this.lblTituloDetalleEntidadFinanciera.TabIndex = 0;
            this.lblTituloDetalleEntidadFinanciera.Tag = "FormFinancialEntityDetail.TitleLabel";
            this.lblTituloDetalleEntidadFinanciera.Values.Text = "Detalle de la entidad financiera";
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
            this.lblEntidadFinancieraPrefijo.Values.Text = "Entidad:";
            //
            // lblEntidadFinancieraNombreDetalle
            //
            this.lblEntidadFinancieraNombreDetalle.AutoSize = true;
            this.lblEntidadFinancieraNombreDetalle.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblEntidadFinancieraNombreDetalle.Name = "lblEntidadFinancieraNombreDetalle";
            this.lblEntidadFinancieraNombreDetalle.Size = new System.Drawing.Size(10, 13);
            this.lblEntidadFinancieraNombreDetalle.TabIndex = 1;
            this.lblEntidadFinancieraNombreDetalle.Values.Text = "-";
            //
            // groupBoxMedioPagoEntidad
            //
            this.groupBoxMedioPagoEntidad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMedioPagoEntidad.Panel.Controls.Add(this.btnQuitarMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Panel.Controls.Add(this.btnAgregarMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Panel.Controls.Add(this.tlpCamposMedioPagoEntidad);
            this.groupBoxMedioPagoEntidad.Location = new System.Drawing.Point(23, 75);
            this.groupBoxMedioPagoEntidad.Name = "groupBoxMedioPagoEntidad";
            this.groupBoxMedioPagoEntidad.Size = new System.Drawing.Size(594, 120);
            this.groupBoxMedioPagoEntidad.TabIndex = 2;
            this.groupBoxMedioPagoEntidad.TabStop = false;
            this.groupBoxMedioPagoEntidad.Tag = "FormFinancialEntityDetail.PaymentMethodGroup";
            this.groupBoxMedioPagoEntidad.Values.Heading = "Medios de pago de esta entidad";
            //
            // btnQuitarMedioPagoEntidad
            //
            this.btnQuitarMedioPagoEntidad.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnQuitarMedioPagoEntidad.Location = new System.Drawing.Point(134, 60);
            this.btnQuitarMedioPagoEntidad.Name = "btnQuitarMedioPagoEntidad";
            this.btnQuitarMedioPagoEntidad.Size = new System.Drawing.Size(110, 30);
            this.btnQuitarMedioPagoEntidad.TabIndex = 5;
            this.btnQuitarMedioPagoEntidad.Tag = "FormFinancialEntityDetail.RemoveButton";
            this.btnQuitarMedioPagoEntidad.Values.Text = "Quitar";
            this.btnQuitarMedioPagoEntidad.Click += new System.EventHandler(this.btnQuitarMedioPagoEntidad_Click);
            //
            // btnAgregarMedioPagoEntidad
            //
            this.btnAgregarMedioPagoEntidad.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarMedioPagoEntidad.Location = new System.Drawing.Point(14, 60);
            this.btnAgregarMedioPagoEntidad.Name = "btnAgregarMedioPagoEntidad";
            this.btnAgregarMedioPagoEntidad.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarMedioPagoEntidad.TabIndex = 4;
            this.btnAgregarMedioPagoEntidad.Tag = "FormFinancialEntityDetail.AddButton";
            this.btnAgregarMedioPagoEntidad.Values.Text = "Agregar";
            this.btnAgregarMedioPagoEntidad.Click += new System.EventHandler(this.btnAgregarMedioPagoEntidad_Click);
            //
            // txtbSegmentoMedioPagoEntidad
            //
            this.txtbSegmentoMedioPagoEntidad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbSegmentoMedioPagoEntidad.Location = new System.Drawing.Point(323, 20);
            this.txtbSegmentoMedioPagoEntidad.MaxLength = 50;
            this.txtbSegmentoMedioPagoEntidad.Name = "txtbSegmentoMedioPagoEntidad";
            this.txtbSegmentoMedioPagoEntidad.Size = new System.Drawing.Size(246, 20);
            this.txtbSegmentoMedioPagoEntidad.TabIndex = 3;
            this.txtbSegmentoMedioPagoEntidad.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblSegmentoMedioPagoEntidad
            //
            this.lblSegmentoMedioPagoEntidad.AutoSize = true;
            this.lblSegmentoMedioPagoEntidad.Location = new System.Drawing.Point(320, 0);
            this.lblSegmentoMedioPagoEntidad.Name = "lblSegmentoMedioPagoEntidad";
            this.lblSegmentoMedioPagoEntidad.Size = new System.Drawing.Size(58, 13);
            this.lblSegmentoMedioPagoEntidad.TabIndex = 2;
            this.lblSegmentoMedioPagoEntidad.Tag = "FormFinancialEntityDetail.SegmentLabel";
            this.lblSegmentoMedioPagoEntidad.Values.Text = "Segmento:";
            this.lblSegmentoMedioPagoEntidad.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbMedioPagoEntidad
            //
            this.cmbMedioPagoEntidad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMedioPagoEntidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMedioPagoEntidad.FormattingEnabled = true;
            this.cmbMedioPagoEntidad.Location = new System.Drawing.Point(3, 20);
            this.cmbMedioPagoEntidad.Name = "cmbMedioPagoEntidad";
            this.cmbMedioPagoEntidad.Size = new System.Drawing.Size(300, 21);
            this.cmbMedioPagoEntidad.TabIndex = 1;
            this.cmbMedioPagoEntidad.Margin = new System.Windows.Forms.Padding(3, 0, 17, 0);
            //
            // lblMedioPagoEntidad
            //
            this.lblMedioPagoEntidad.AutoSize = true;
            this.lblMedioPagoEntidad.Location = new System.Drawing.Point(0, 0);
            this.lblMedioPagoEntidad.Name = "lblMedioPagoEntidad";
            this.lblMedioPagoEntidad.Size = new System.Drawing.Size(84, 13);
            this.lblMedioPagoEntidad.TabIndex = 0;
            this.lblMedioPagoEntidad.Tag = "FormFinancialEntityDetail.PaymentMethodLabel";
            this.lblMedioPagoEntidad.Values.Text = "Medio de pago:";
            this.lblMedioPagoEntidad.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvMediosPagoEntidad
            //
            this.dgvMediosPagoEntidad.AllowUserToAddRows = false;
            this.dgvMediosPagoEntidad.AllowUserToDeleteRows = false;
            this.dgvMediosPagoEntidad.AutoGenerateColumns = false;
            this.dgvMediosPagoEntidad.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMediosPagoEntidad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            // tlpCamposMedioPagoEntidad
            // 
            this.tlpCamposMedioPagoEntidad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposMedioPagoEntidad.Controls.Add(this.txtbSegmentoMedioPagoEntidad, 1, 1);
            this.tlpCamposMedioPagoEntidad.Controls.Add(this.lblSegmentoMedioPagoEntidad, 1, 0);
            this.tlpCamposMedioPagoEntidad.Controls.Add(this.cmbMedioPagoEntidad, 0, 1);
            this.tlpCamposMedioPagoEntidad.Controls.Add(this.lblMedioPagoEntidad, 0, 0);
            this.tlpCamposMedioPagoEntidad.Location = new System.Drawing.Point(11, 8);
            this.tlpCamposMedioPagoEntidad.Name = "tlpCamposMedioPagoEntidad";
            this.tlpCamposMedioPagoEntidad.Size = new System.Drawing.Size(569, 45);
            this.tlpCamposMedioPagoEntidad.ColumnCount = 2;
            this.tlpCamposMedioPagoEntidad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.24F));
            this.tlpCamposMedioPagoEntidad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.76F));
            this.tlpCamposMedioPagoEntidad.RowCount = 2;
            this.tlpCamposMedioPagoEntidad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposMedioPagoEntidad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposMedioPagoEntidad.TabIndex = 0;
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
            this.Name = "FormDetalleEntidadFinanciera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormFinancialEntityDetail.Title";
            this.Text = "Detalle de la entidad financiera";
            this.pnlEntidadFinancieraDetalle.ResumeLayout(false);
            this.pnlEntidadFinancieraDetalle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPagoEntidad.Panel)).EndInit();
            this.groupBoxMedioPagoEntidad.Panel.ResumeLayout(false);
            this.groupBoxMedioPagoEntidad.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxMedioPagoEntidad)).EndInit();
            this.groupBoxMedioPagoEntidad.ResumeLayout(false);
            this.groupBoxMedioPagoEntidad.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMediosPagoEntidad)).EndInit();
            this.tlpCamposMedioPagoEntidad.ResumeLayout(false);
            this.tlpCamposMedioPagoEntidad.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloDetalleEntidadFinanciera;
        private System.Windows.Forms.FlowLayoutPanel pnlEntidadFinancieraDetalle;
        private Krypton.Toolkit.KryptonLabel lblEntidadFinancieraPrefijo;
        private Krypton.Toolkit.KryptonLabel lblEntidadFinancieraNombreDetalle;
        private Krypton.Toolkit.KryptonGroupBox groupBoxMedioPagoEntidad;
        private Krypton.Toolkit.KryptonButton btnQuitarMedioPagoEntidad;
        private Krypton.Toolkit.KryptonButton btnAgregarMedioPagoEntidad;
        private Krypton.Toolkit.KryptonTextBox txtbSegmentoMedioPagoEntidad;
        private Krypton.Toolkit.KryptonLabel lblSegmentoMedioPagoEntidad;
        private Krypton.Toolkit.KryptonComboBox cmbMedioPagoEntidad;
        private Krypton.Toolkit.KryptonLabel lblMedioPagoEntidad;
        private Krypton.Toolkit.KryptonDataGridView dgvMediosPagoEntidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoEntidadTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoEntidadMarca;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedioPagoEntidadSegmento;
        private System.Windows.Forms.TableLayoutPanel tlpCamposMedioPagoEntidad;
    }
}
