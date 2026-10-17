namespace TpIDS
{
    partial class FormReglasCategorizacion
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
            this.lblTituloGestionReglas = new System.Windows.Forms.Label();
            this.groupBoxRegla = new System.Windows.Forms.GroupBox();
            this.btnLimpiarRegla = new System.Windows.Forms.Button();
            this.btnEliminarRegla = new System.Windows.Forms.Button();
            this.btnActualizarRegla = new System.Windows.Forms.Button();
            this.btnAgregarRegla = new System.Windows.Forms.Button();
            this.lstbComerciosRegla = new System.Windows.Forms.CheckedListBox();
            this.lblComerciosRegla = new System.Windows.Forms.Label();
            this.txtbPalabrasClaveRegla = new System.Windows.Forms.TextBox();
            this.lblPalabrasClaveRegla = new System.Windows.Forms.Label();
            this.cmbCategoriaRegla = new System.Windows.Forms.ComboBox();
            this.lblCategoriaRegla = new System.Windows.Forms.Label();
            this.txtbNombreRegla = new System.Windows.Forms.TextBox();
            this.lblNombreRegla = new System.Windows.Forms.Label();
            this.dgvReglas = new System.Windows.Forms.DataGridView();
            this.colReglasNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReglasCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReglasPalabrasClave = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReglasComercios = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxRegla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReglas)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTituloGestionReglas
            // 
            this.lblTituloGestionReglas.AutoSize = true;
            this.lblTituloGestionReglas.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionReglas.Location = new System.Drawing.Point(18, 15);
            this.lblTituloGestionReglas.Name = "lblTituloGestionReglas";
            this.lblTituloGestionReglas.Size = new System.Drawing.Size(342, 25);
            this.lblTituloGestionReglas.TabIndex = 0;
            this.lblTituloGestionReglas.Tag = "FormCategorizationRules.TitleLabel";
            this.lblTituloGestionReglas.Text = "Gestion de reglas de categorizacion";
            // 
            // groupBoxRegla
            // 
            this.groupBoxRegla.Controls.Add(this.btnLimpiarRegla);
            this.groupBoxRegla.Controls.Add(this.btnEliminarRegla);
            this.groupBoxRegla.Controls.Add(this.btnActualizarRegla);
            this.groupBoxRegla.Controls.Add(this.btnAgregarRegla);
            this.groupBoxRegla.Controls.Add(this.lstbComerciosRegla);
            this.groupBoxRegla.Controls.Add(this.lblComerciosRegla);
            this.groupBoxRegla.Controls.Add(this.txtbPalabrasClaveRegla);
            this.groupBoxRegla.Controls.Add(this.lblPalabrasClaveRegla);
            this.groupBoxRegla.Controls.Add(this.cmbCategoriaRegla);
            this.groupBoxRegla.Controls.Add(this.lblCategoriaRegla);
            this.groupBoxRegla.Controls.Add(this.txtbNombreRegla);
            this.groupBoxRegla.Controls.Add(this.lblNombreRegla);
            this.groupBoxRegla.Location = new System.Drawing.Point(23, 52);
            this.groupBoxRegla.Name = "groupBoxRegla";
            this.groupBoxRegla.Size = new System.Drawing.Size(740, 205);
            this.groupBoxRegla.TabIndex = 1;
            this.groupBoxRegla.TabStop = false;
            this.groupBoxRegla.Tag = "FormCategorizationRules.RuleGroup";
            this.groupBoxRegla.Text = "Datos de la regla";
            // 
            // btnLimpiarRegla
            // 
            this.btnLimpiarRegla.Location = new System.Drawing.Point(449, 162);
            this.btnLimpiarRegla.Name = "btnLimpiarRegla";
            this.btnLimpiarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnLimpiarRegla.TabIndex = 11;
            this.btnLimpiarRegla.Tag = "FormCategorizationRules.ClearButton";
            this.btnLimpiarRegla.Text = "Limpiar campos";
            this.btnLimpiarRegla.UseVisualStyleBackColor = true;
            this.btnLimpiarRegla.Click += new System.EventHandler(this.btnLimpiarRegla_Click);
            // 
            // btnEliminarRegla
            // 
            this.btnEliminarRegla.Location = new System.Drawing.Point(304, 162);
            this.btnEliminarRegla.Name = "btnEliminarRegla";
            this.btnEliminarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnEliminarRegla.TabIndex = 10;
            this.btnEliminarRegla.Tag = "FormCategorizationRules.DeleteButton";
            this.btnEliminarRegla.Text = "Eliminar regla";
            this.btnEliminarRegla.UseVisualStyleBackColor = true;
            this.btnEliminarRegla.Click += new System.EventHandler(this.btnEliminarRegla_Click);
            // 
            // btnActualizarRegla
            // 
            this.btnActualizarRegla.Location = new System.Drawing.Point(159, 162);
            this.btnActualizarRegla.Name = "btnActualizarRegla";
            this.btnActualizarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnActualizarRegla.TabIndex = 9;
            this.btnActualizarRegla.Tag = "FormCategorizationRules.UpdateButton";
            this.btnActualizarRegla.Text = "Actualizar regla";
            this.btnActualizarRegla.UseVisualStyleBackColor = true;
            this.btnActualizarRegla.Click += new System.EventHandler(this.btnActualizarRegla_Click);
            // 
            // btnAgregarRegla
            // 
            this.btnAgregarRegla.Location = new System.Drawing.Point(14, 162);
            this.btnAgregarRegla.Name = "btnAgregarRegla";
            this.btnAgregarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnAgregarRegla.TabIndex = 8;
            this.btnAgregarRegla.Tag = "FormCategorizationRules.AddButton";
            this.btnAgregarRegla.Text = "Agregar regla";
            this.btnAgregarRegla.UseVisualStyleBackColor = true;
            this.btnAgregarRegla.Click += new System.EventHandler(this.btnAgregarRegla_Click);
            // 
            // lstbComerciosRegla
            // 
            this.lstbComerciosRegla.CheckOnClick = true;
            this.lstbComerciosRegla.FormattingEnabled = true;
            this.lstbComerciosRegla.Location = new System.Drawing.Point(365, 36);
            this.lstbComerciosRegla.Name = "lstbComerciosRegla";
            this.lstbComerciosRegla.Size = new System.Drawing.Size(355, 109);
            this.lstbComerciosRegla.TabIndex = 7;
            // 
            // lblComerciosRegla
            // 
            this.lblComerciosRegla.AutoSize = true;
            this.lblComerciosRegla.Location = new System.Drawing.Point(362, 20);
            this.lblComerciosRegla.Name = "lblComerciosRegla";
            this.lblComerciosRegla.Size = new System.Drawing.Size(155, 13);
            this.lblComerciosRegla.TabIndex = 6;
            this.lblComerciosRegla.Tag = "FormCategorizationRules.MerchantsLabel";
            this.lblComerciosRegla.Text = "Comercios asociados (opcional):";
            // 
            // txtbPalabrasClaveRegla
            // 
            this.txtbPalabrasClaveRegla.Location = new System.Drawing.Point(14, 125);
            this.txtbPalabrasClaveRegla.Name = "txtbPalabrasClaveRegla";
            this.txtbPalabrasClaveRegla.Size = new System.Drawing.Size(330, 20);
            this.txtbPalabrasClaveRegla.TabIndex = 5;
            // 
            // lblPalabrasClaveRegla
            // 
            this.lblPalabrasClaveRegla.AutoSize = true;
            this.lblPalabrasClaveRegla.Location = new System.Drawing.Point(11, 109);
            this.lblPalabrasClaveRegla.Name = "lblPalabrasClaveRegla";
            this.lblPalabrasClaveRegla.Size = new System.Drawing.Size(171, 13);
            this.lblPalabrasClaveRegla.TabIndex = 4;
            this.lblPalabrasClaveRegla.Tag = "FormCategorizationRules.KeywordsLabel";
            this.lblPalabrasClaveRegla.Text = "Palabras clave (separadas por comas):";
            // 
            // cmbCategoriaRegla
            // 
            this.cmbCategoriaRegla.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaRegla.FormattingEnabled = true;
            this.cmbCategoriaRegla.Location = new System.Drawing.Point(14, 80);
            this.cmbCategoriaRegla.Name = "cmbCategoriaRegla";
            this.cmbCategoriaRegla.Size = new System.Drawing.Size(330, 21);
            this.cmbCategoriaRegla.TabIndex = 3;
            // 
            // lblCategoriaRegla
            // 
            this.lblCategoriaRegla.AutoSize = true;
            this.lblCategoriaRegla.Location = new System.Drawing.Point(11, 64);
            this.lblCategoriaRegla.Name = "lblCategoriaRegla";
            this.lblCategoriaRegla.Size = new System.Drawing.Size(55, 13);
            this.lblCategoriaRegla.TabIndex = 2;
            this.lblCategoriaRegla.Tag = "FormCategorizationRules.CategoryLabel";
            this.lblCategoriaRegla.Text = "Categoria:";
            // 
            // txtbNombreRegla
            // 
            this.txtbNombreRegla.Location = new System.Drawing.Point(14, 36);
            this.txtbNombreRegla.Name = "txtbNombreRegla";
            this.txtbNombreRegla.Size = new System.Drawing.Size(330, 20);
            this.txtbNombreRegla.TabIndex = 1;
            // 
            // lblNombreRegla
            // 
            this.lblNombreRegla.AutoSize = true;
            this.lblNombreRegla.Location = new System.Drawing.Point(11, 20);
            this.lblNombreRegla.Name = "lblNombreRegla";
            this.lblNombreRegla.Size = new System.Drawing.Size(103, 13);
            this.lblNombreRegla.TabIndex = 0;
            this.lblNombreRegla.Tag = "FormCategorizationRules.NameLabel";
            this.lblNombreRegla.Text = "Nombre de la regla:";
            // 
            // dgvReglas
            // 
            this.dgvReglas.AllowUserToAddRows = false;
            this.dgvReglas.AllowUserToDeleteRows = false;
            this.dgvReglas.AutoGenerateColumns = false;
            this.dgvReglas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReglas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReglas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colReglasNombre,
            this.colReglasCategoria,
            this.colReglasPalabrasClave,
            this.colReglasComercios});
            this.dgvReglas.Location = new System.Drawing.Point(23, 270);
            this.dgvReglas.MultiSelect = false;
            this.dgvReglas.Name = "dgvReglas";
            this.dgvReglas.ReadOnly = true;
            this.dgvReglas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReglas.Size = new System.Drawing.Size(740, 240);
            this.dgvReglas.TabIndex = 2;
            this.dgvReglas.Tag = "FormCategorizationRules.Grid";
            this.dgvReglas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvReglas_CellClick);
            // 
            // colReglasNombre
            // 
            this.colReglasNombre.DataPropertyName = "Nombre";
            this.colReglasNombre.HeaderText = "Nombre";
            this.colReglasNombre.Name = "Nombre";
            this.colReglasNombre.ReadOnly = true;
            // 
            // colReglasCategoria
            // 
            this.colReglasCategoria.DataPropertyName = "Categoria";
            this.colReglasCategoria.HeaderText = "Categoria";
            this.colReglasCategoria.Name = "Categoria";
            this.colReglasCategoria.ReadOnly = true;
            // 
            // colReglasPalabrasClave
            // 
            this.colReglasPalabrasClave.DataPropertyName = "PalabrasClave";
            this.colReglasPalabrasClave.HeaderText = "Palabras clave";
            this.colReglasPalabrasClave.Name = "PalabrasClave";
            this.colReglasPalabrasClave.ReadOnly = true;
            // 
            // colReglasComercios
            // 
            this.colReglasComercios.DataPropertyName = "Comercios";
            this.colReglasComercios.HeaderText = "Comercios asociados";
            this.colReglasComercios.Name = "Comercios";
            this.colReglasComercios.ReadOnly = true;
            // 
            // FormReglasCategorizacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 530);
            this.Controls.Add(this.dgvReglas);
            this.Controls.Add(this.groupBoxRegla);
            this.Controls.Add(this.lblTituloGestionReglas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormReglasCategorizacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormCategorizationRules.Title";
            this.Text = "Gestion de reglas de categorizacion";
            this.groupBoxRegla.ResumeLayout(false);
            this.groupBoxRegla.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReglas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTituloGestionReglas;
        private System.Windows.Forms.GroupBox groupBoxRegla;
        private System.Windows.Forms.Label lblNombreRegla;
        private System.Windows.Forms.TextBox txtbNombreRegla;
        private System.Windows.Forms.Label lblCategoriaRegla;
        private System.Windows.Forms.ComboBox cmbCategoriaRegla;
        private System.Windows.Forms.Label lblPalabrasClaveRegla;
        private System.Windows.Forms.TextBox txtbPalabrasClaveRegla;
        private System.Windows.Forms.Label lblComerciosRegla;
        private System.Windows.Forms.CheckedListBox lstbComerciosRegla;
        private System.Windows.Forms.Button btnAgregarRegla;
        private System.Windows.Forms.Button btnActualizarRegla;
        private System.Windows.Forms.Button btnEliminarRegla;
        private System.Windows.Forms.Button btnLimpiarRegla;
        private System.Windows.Forms.DataGridView dgvReglas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasPalabrasClave;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasComercios;
    }
}
