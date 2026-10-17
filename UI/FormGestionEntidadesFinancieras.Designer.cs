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
            this.lblTituloEntidadesFinancieras = new System.Windows.Forms.Label();
            this.groupBoxEntidadFinanciera = new System.Windows.Forms.GroupBox();
            this.btnVerDetalleEntidadFinanciera = new System.Windows.Forms.Button();
            this.btnEliminarEntidadFinanciera = new System.Windows.Forms.Button();
            this.btnEditarEntidadFinanciera = new System.Windows.Forms.Button();
            this.btnAgregarEntidadFinanciera = new System.Windows.Forms.Button();
            this.txtbNombreEntidadFinanciera = new System.Windows.Forms.TextBox();
            this.lblNombreEntidadFinanciera = new System.Windows.Forms.Label();
            this.dgvEntidadesFinancieras = new System.Windows.Forms.DataGridView();
            this.colEntidadFinancieraNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxEntidadFinanciera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntidadesFinancieras)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloEntidadesFinancieras
            //
            this.lblTituloEntidadesFinancieras.AutoSize = true;
            this.lblTituloEntidadesFinancieras.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloEntidadesFinancieras.Location = new System.Drawing.Point(18, 15);
            this.lblTituloEntidadesFinancieras.Name = "lblTituloEntidadesFinancieras";
            this.lblTituloEntidadesFinancieras.Size = new System.Drawing.Size(300, 25);
            this.lblTituloEntidadesFinancieras.TabIndex = 0;
            this.lblTituloEntidadesFinancieras.Tag = "FormFinancialEntityManagement.TitleLabel";
            this.lblTituloEntidadesFinancieras.Text = "Gestion de Entidades Financieras";
            //
            // groupBoxEntidadFinanciera
            //
            this.groupBoxEntidadFinanciera.Controls.Add(this.btnVerDetalleEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Controls.Add(this.btnEliminarEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Controls.Add(this.btnEditarEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Controls.Add(this.btnAgregarEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Controls.Add(this.txtbNombreEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Controls.Add(this.lblNombreEntidadFinanciera);
            this.groupBoxEntidadFinanciera.Location = new System.Drawing.Point(23, 52);
            this.groupBoxEntidadFinanciera.Name = "groupBoxEntidadFinanciera";
            this.groupBoxEntidadFinanciera.Size = new System.Drawing.Size(594, 110);
            this.groupBoxEntidadFinanciera.TabIndex = 1;
            this.groupBoxEntidadFinanciera.TabStop = false;
            this.groupBoxEntidadFinanciera.Tag = "FormFinancialEntityManagement.EntityGroup";
            this.groupBoxEntidadFinanciera.Text = "Entidad financiera";
            //
            // btnVerDetalleEntidadFinanciera
            //
            this.btnVerDetalleEntidadFinanciera.Location = new System.Drawing.Point(470, 66);
            this.btnVerDetalleEntidadFinanciera.Name = "btnVerDetalleEntidadFinanciera";
            this.btnVerDetalleEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnVerDetalleEntidadFinanciera.TabIndex = 5;
            this.btnVerDetalleEntidadFinanciera.Tag = "FormFinancialEntityManagement.DetailButton";
            this.btnVerDetalleEntidadFinanciera.Text = "Ver detalle";
            this.btnVerDetalleEntidadFinanciera.UseVisualStyleBackColor = true;
            this.btnVerDetalleEntidadFinanciera.Click += new System.EventHandler(this.btnVerDetalleEntidadFinanciera_Click);
            //
            // btnEliminarEntidadFinanciera
            //
            this.btnEliminarEntidadFinanciera.Location = new System.Drawing.Point(254, 66);
            this.btnEliminarEntidadFinanciera.Name = "btnEliminarEntidadFinanciera";
            this.btnEliminarEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarEntidadFinanciera.TabIndex = 4;
            this.btnEliminarEntidadFinanciera.Tag = "FormFinancialEntityManagement.DeleteButton";
            this.btnEliminarEntidadFinanciera.Text = "Eliminar";
            this.btnEliminarEntidadFinanciera.UseVisualStyleBackColor = true;
            this.btnEliminarEntidadFinanciera.Click += new System.EventHandler(this.btnEliminarEntidadFinanciera_Click);
            //
            // btnEditarEntidadFinanciera
            //
            this.btnEditarEntidadFinanciera.Location = new System.Drawing.Point(134, 66);
            this.btnEditarEntidadFinanciera.Name = "btnEditarEntidadFinanciera";
            this.btnEditarEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnEditarEntidadFinanciera.TabIndex = 3;
            this.btnEditarEntidadFinanciera.Tag = "FormFinancialEntityManagement.UpdateButton";
            this.btnEditarEntidadFinanciera.Text = "Actualizar";
            this.btnEditarEntidadFinanciera.UseVisualStyleBackColor = true;
            this.btnEditarEntidadFinanciera.Click += new System.EventHandler(this.btnEditarEntidadFinanciera_Click);
            //
            // btnAgregarEntidadFinanciera
            //
            this.btnAgregarEntidadFinanciera.Location = new System.Drawing.Point(14, 66);
            this.btnAgregarEntidadFinanciera.Name = "btnAgregarEntidadFinanciera";
            this.btnAgregarEntidadFinanciera.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarEntidadFinanciera.TabIndex = 2;
            this.btnAgregarEntidadFinanciera.Tag = "FormFinancialEntityManagement.AddButton";
            this.btnAgregarEntidadFinanciera.Text = "Agregar";
            this.btnAgregarEntidadFinanciera.UseVisualStyleBackColor = true;
            this.btnAgregarEntidadFinanciera.Click += new System.EventHandler(this.btnAgregarEntidadFinanciera_Click);
            //
            // txtbNombreEntidadFinanciera
            //
            this.txtbNombreEntidadFinanciera.Location = new System.Drawing.Point(70, 26);
            this.txtbNombreEntidadFinanciera.MaxLength = 100;
            this.txtbNombreEntidadFinanciera.Name = "txtbNombreEntidadFinanciera";
            this.txtbNombreEntidadFinanciera.Size = new System.Drawing.Size(510, 20);
            this.txtbNombreEntidadFinanciera.TabIndex = 1;
            //
            // lblNombreEntidadFinanciera
            //
            this.lblNombreEntidadFinanciera.AutoSize = true;
            this.lblNombreEntidadFinanciera.Location = new System.Drawing.Point(11, 29);
            this.lblNombreEntidadFinanciera.Name = "lblNombreEntidadFinanciera";
            this.lblNombreEntidadFinanciera.Size = new System.Drawing.Size(50, 13);
            this.lblNombreEntidadFinanciera.TabIndex = 0;
            this.lblNombreEntidadFinanciera.Tag = "FormFinancialEntityManagement.NameLabel";
            this.lblNombreEntidadFinanciera.Text = "Nombre:";
            //
            // dgvEntidadesFinancieras
            //
            this.dgvEntidadesFinancieras.AllowUserToAddRows = false;
            this.dgvEntidadesFinancieras.AllowUserToDeleteRows = false;
            this.dgvEntidadesFinancieras.AutoGenerateColumns = false;
            this.dgvEntidadesFinancieras.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionEntidadesFinancieras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormFinancialEntityManagement.Title";
            this.Text = "Gestion de Entidades Financieras";
            this.groupBoxEntidadFinanciera.ResumeLayout(false);
            this.groupBoxEntidadFinanciera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntidadesFinancieras)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloEntidadesFinancieras;
        private System.Windows.Forms.GroupBox groupBoxEntidadFinanciera;
        private System.Windows.Forms.Button btnVerDetalleEntidadFinanciera;
        private System.Windows.Forms.Button btnEliminarEntidadFinanciera;
        private System.Windows.Forms.Button btnEditarEntidadFinanciera;
        private System.Windows.Forms.Button btnAgregarEntidadFinanciera;
        private System.Windows.Forms.TextBox txtbNombreEntidadFinanciera;
        private System.Windows.Forms.Label lblNombreEntidadFinanciera;
        private System.Windows.Forms.DataGridView dgvEntidadesFinancieras;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEntidadFinancieraNombre;
    }
}
