namespace TpIDS
{
    partial class FormGestionEntidadesFinancieras
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
            this.lblTituloEntidadesFinancieras = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxEntidadFinanciera = new Krypton.Toolkit.KryptonGroupBox();
            this.btnVerDetalleEntidadFinanciera = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarEntidadFinanciera = new Krypton.Toolkit.KryptonButton();
            this.btnEditarEntidadFinanciera = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarEntidadFinanciera = new Krypton.Toolkit.KryptonButton();
            this.txtbNombreEntidadFinanciera = new Krypton.Toolkit.KryptonTextBox();
            this.lblNombreEntidadFinanciera = new Krypton.Toolkit.KryptonLabel();
            this.dgvEntidadesFinancieras = new Krypton.Toolkit.KryptonDataGridView();
            this.colEntidadFinancieraNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEntidadFinanciera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEntidadFinanciera.Panel)).BeginInit();
            this.groupBoxEntidadFinanciera.Panel.SuspendLayout();
            this.groupBoxEntidadFinanciera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntidadesFinancieras)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloEntidadesFinancieras
            //
            this.lblTituloEntidadesFinancieras.AutoSize = true;
            this.lblTituloEntidadesFinancieras.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloEntidadesFinancieras.Location = new System.Drawing.Point(18, 15);
            this.lblTituloEntidadesFinancieras.Name = "lblTituloEntidadesFinancieras";
            this.lblTituloEntidadesFinancieras.Size = new System.Drawing.Size(300, 25);
            this.lblTituloEntidadesFinancieras.TabIndex = 0;
            this.lblTituloEntidadesFinancieras.Tag = "FormFinancialEntityManagement.TitleLabel";
            this.lblTituloEntidadesFinancieras.Values.Text = "Gestion de Entidades Financieras";
            //
            // groupBoxEntidadFinanciera
            //
            this.groupBoxEntidadFinanciera.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEntidadFinanciera.Panel.Controls.Add(this.btnVerDetalleEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Panel.Controls.Add(this.btnEliminarEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Panel.Controls.Add(this.btnEditarEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Panel.Controls.Add(this.btnAgregarEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Panel.Controls.Add(this.txtbNombreEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Panel.Controls.Add(this.lblNombreEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Location = new System.Drawing.Point(23, 52);
            this.groupBoxEntidadFinanciera.Name = "groupBoxEntidadFinanciera";
            this.groupBoxEntidadFinanciera.Size = new System.Drawing.Size(594, 110);
            this.groupBoxEntidadFinanciera.TabIndex = 1;
            this.groupBoxEntidadFinanciera.TabStop = false;
            this.groupBoxEntidadFinanciera.Tag = "FormFinancialEntityManagement.EntityGroup";
            this.groupBoxEntidadFinanciera.Values.Heading = "Entidad financiera";
            //
            // btnVerDetalleEntidadFinanciera
            //
            this.btnVerDetalleEntidadFinanciera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVerDetalleEntidadFinanciera.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnVerDetalleEntidadFinanciera.Location = new System.Drawing.Point(470, 50);
            this.btnVerDetalleEntidadFinanciera.Name = "btnVerDetalleEntidadFinanciera";
            this.btnVerDetalleEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnVerDetalleEntidadFinanciera.TabIndex = 5;
            this.btnVerDetalleEntidadFinanciera.Tag = "FormFinancialEntityManagement.DetailButton";
            this.btnVerDetalleEntidadFinanciera.Values.Text = "Ver detalle";
            this.btnVerDetalleEntidadFinanciera.Click += new System.EventHandler(this.btnVerDetalleEntidadFinanciera_Click);
            //
            // btnEliminarEntidadFinanciera
            //
            this.btnEliminarEntidadFinanciera.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarEntidadFinanciera.Location = new System.Drawing.Point(254, 50);
            this.btnEliminarEntidadFinanciera.Name = "btnEliminarEntidadFinanciera";
            this.btnEliminarEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarEntidadFinanciera.TabIndex = 4;
            this.btnEliminarEntidadFinanciera.Tag = "FormFinancialEntityManagement.DeleteButton";
            this.btnEliminarEntidadFinanciera.Values.Text = "Eliminar";
            this.btnEliminarEntidadFinanciera.Click += new System.EventHandler(this.btnEliminarEntidadFinanciera_Click);
            //
            // btnEditarEntidadFinanciera
            //
            this.btnEditarEntidadFinanciera.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEditarEntidadFinanciera.Location = new System.Drawing.Point(134, 50);
            this.btnEditarEntidadFinanciera.Name = "btnEditarEntidadFinanciera";
            this.btnEditarEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnEditarEntidadFinanciera.TabIndex = 3;
            this.btnEditarEntidadFinanciera.Tag = "FormFinancialEntityManagement.UpdateButton";
            this.btnEditarEntidadFinanciera.Values.Text = "Actualizar";
            this.btnEditarEntidadFinanciera.Click += new System.EventHandler(this.btnEditarEntidadFinanciera_Click);
            //
            // btnAgregarEntidadFinanciera
            //
            this.btnAgregarEntidadFinanciera.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarEntidadFinanciera.Location = new System.Drawing.Point(14, 50);
            this.btnAgregarEntidadFinanciera.Name = "btnAgregarEntidadFinanciera";
            this.btnAgregarEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarEntidadFinanciera.TabIndex = 2;
            this.btnAgregarEntidadFinanciera.Tag = "FormFinancialEntityManagement.AddButton";
            this.btnAgregarEntidadFinanciera.Values.Text = "Agregar";
            this.btnAgregarEntidadFinanciera.Click += new System.EventHandler(this.btnAgregarEntidadFinanciera_Click);
            //
            // txtbNombreEntidadFinanciera
            //
            this.txtbNombreEntidadFinanciera.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreEntidadFinanciera.Location = new System.Drawing.Point(70, 10);
            this.txtbNombreEntidadFinanciera.MaxLength = 100;
            this.txtbNombreEntidadFinanciera.Name = "txtbNombreEntidadFinanciera";
            this.txtbNombreEntidadFinanciera.Size = new System.Drawing.Size(510, 20);
            this.txtbNombreEntidadFinanciera.TabIndex = 1;
            //
            // lblNombreEntidadFinanciera
            //
            this.lblNombreEntidadFinanciera.AutoSize = true;
            this.lblNombreEntidadFinanciera.Location = new System.Drawing.Point(11, 9);
            this.lblNombreEntidadFinanciera.Name = "lblNombreEntidadFinanciera";
            this.lblNombreEntidadFinanciera.Size = new System.Drawing.Size(50, 13);
            this.lblNombreEntidadFinanciera.TabIndex = 0;
            this.lblNombreEntidadFinanciera.Tag = "FormFinancialEntityManagement.NameLabel";
            this.lblNombreEntidadFinanciera.Values.Text = "Nombre:";
            //
            // dgvEntidadesFinancieras
            //
            this.dgvEntidadesFinancieras.AllowUserToAddRows = false;
            this.dgvEntidadesFinancieras.AllowUserToDeleteRows = false;
            this.dgvEntidadesFinancieras.AutoGenerateColumns = false;
            this.dgvEntidadesFinancieras.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEntidadesFinancieras.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEntidadesFinancieras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntidadesFinancieras.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEntidadFinancieraNombre});
            this.dgvEntidadesFinancieras.Location = new System.Drawing.Point(23, 172);
            this.dgvEntidadesFinancieras.MultiSelect = false;
            this.dgvEntidadesFinancieras.Name = "dgvEntidadesFinancieras";
            this.dgvEntidadesFinancieras.ReadOnly = true;
            this.dgvEntidadesFinancieras.RowHeadersVisible = false;
            this.dgvEntidadesFinancieras.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEntidadesFinancieras.Size = new System.Drawing.Size(594, 280);
            this.dgvEntidadesFinancieras.TabIndex = 2;
            this.dgvEntidadesFinancieras.Tag = "FormFinancialEntityManagement.EntityGrid";
            this.dgvEntidadesFinancieras.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEntidadesFinancieras_CellClick);
            //
            // colEntidadFinancieraNombre
            //
            this.colEntidadFinancieraNombre.DataPropertyName = "Nombre";
            this.colEntidadFinancieraNombre.HeaderText = "Nombre";
            this.colEntidadFinancieraNombre.Name = "Nombre";
            this.colEntidadFinancieraNombre.ReadOnly = true;
            //
            // FormGestionEntidadesFinancieras
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 470);
            this.Controls.Add(this.dgvEntidadesFinancieras);
            this.Controls.Add(this.groupBoxEntidadFinanciera);
            this.Controls.Add(this.lblTituloEntidadesFinancieras);
            this.Name = "FormGestionEntidadesFinancieras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormFinancialEntityManagement.Title";
            this.Text = "Gestion de Entidades Financieras";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEntidadFinanciera.Panel)).EndInit();
            this.groupBoxEntidadFinanciera.Panel.ResumeLayout(false);
            this.groupBoxEntidadFinanciera.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxEntidadFinanciera)).EndInit();
            this.groupBoxEntidadFinanciera.ResumeLayout(false);
            this.groupBoxEntidadFinanciera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntidadesFinancieras)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloEntidadesFinancieras;
        private Krypton.Toolkit.KryptonGroupBox groupBoxEntidadFinanciera;
        private Krypton.Toolkit.KryptonButton btnVerDetalleEntidadFinanciera;
        private Krypton.Toolkit.KryptonButton btnEliminarEntidadFinanciera;
        private Krypton.Toolkit.KryptonButton btnEditarEntidadFinanciera;
        private Krypton.Toolkit.KryptonButton btnAgregarEntidadFinanciera;
        private Krypton.Toolkit.KryptonTextBox txtbNombreEntidadFinanciera;
        private Krypton.Toolkit.KryptonLabel lblNombreEntidadFinanciera;
        private Krypton.Toolkit.KryptonDataGridView dgvEntidadesFinancieras;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEntidadFinancieraNombre;
    }
}
