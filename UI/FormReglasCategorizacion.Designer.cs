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
            this.lblTituloGestionReglas = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxRegla = new Krypton.Toolkit.KryptonGroupBox();
            this.btnLimpiarRegla = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarRegla = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarRegla = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarRegla = new Krypton.Toolkit.KryptonButton();
            this.lstbComerciosRegla = new System.Windows.Forms.CheckedListBox();
            this.lblComerciosRegla = new Krypton.Toolkit.KryptonLabel();
            this.txtbPalabrasClaveRegla = new Krypton.Toolkit.KryptonTextBox();
            this.lblPalabrasClaveRegla = new Krypton.Toolkit.KryptonLabel();
            this.cmbCategoriaRegla = new Krypton.Toolkit.KryptonComboBox();
            this.lblCategoriaRegla = new Krypton.Toolkit.KryptonLabel();
            this.txtbNombreRegla = new Krypton.Toolkit.KryptonTextBox();
            this.lblNombreRegla = new Krypton.Toolkit.KryptonLabel();
            this.dgvReglas = new Krypton.Toolkit.KryptonDataGridView();
            this.colReglasNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReglasCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReglasPalabrasClave = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReglasComercios = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRegla)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRegla.Panel)).BeginInit();
            this.groupBoxRegla.Panel.SuspendLayout();
            this.groupBoxRegla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReglas)).BeginInit();
            this.tlpCamposRegla = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposRegla.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTituloGestionReglas
            // 
            this.lblTituloGestionReglas.AutoSize = true;
            this.lblTituloGestionReglas.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionReglas.Location = new System.Drawing.Point(18, 15);
            this.lblTituloGestionReglas.Name = "lblTituloGestionReglas";
            this.lblTituloGestionReglas.Size = new System.Drawing.Size(342, 25);
            this.lblTituloGestionReglas.TabIndex = 0;
            this.lblTituloGestionReglas.Tag = "FormCategorizationRules.TitleLabel";
            this.lblTituloGestionReglas.Values.Text = "Gestion de reglas de categorizacion";
            // 
            // groupBoxRegla
            // 
            this.groupBoxRegla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRegla.Panel.Controls.Add(this.btnLimpiarRegla);
            this.groupBoxRegla.Panel.Controls.Add(this.btnEliminarRegla);
            this.groupBoxRegla.Panel.Controls.Add(this.btnActualizarRegla);
            this.groupBoxRegla.Panel.Controls.Add(this.btnAgregarRegla);
            this.groupBoxRegla.Panel.Controls.Add(this.tlpCamposRegla);
            this.groupBoxRegla.Location = new System.Drawing.Point(23, 52);
            this.groupBoxRegla.Name = "groupBoxRegla";
            this.groupBoxRegla.Size = new System.Drawing.Size(740, 205);
            this.groupBoxRegla.TabIndex = 1;
            this.groupBoxRegla.TabStop = false;
            this.groupBoxRegla.Tag = "FormCategorizationRules.RuleGroup";
            this.groupBoxRegla.Values.Heading = "Datos de la regla";
            // 
            // btnLimpiarRegla
            // 
            this.btnLimpiarRegla.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnLimpiarRegla.Location = new System.Drawing.Point(449, 146);
            this.btnLimpiarRegla.Name = "btnLimpiarRegla";
            this.btnLimpiarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnLimpiarRegla.TabIndex = 11;
            this.btnLimpiarRegla.Tag = "FormCategorizationRules.ClearButton";
            this.btnLimpiarRegla.Values.Text = "Limpiar campos";
            this.btnLimpiarRegla.Click += new System.EventHandler(this.btnLimpiarRegla_Click);
            // 
            // btnEliminarRegla
            // 
            this.btnEliminarRegla.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarRegla.Location = new System.Drawing.Point(304, 146);
            this.btnEliminarRegla.Name = "btnEliminarRegla";
            this.btnEliminarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnEliminarRegla.TabIndex = 10;
            this.btnEliminarRegla.Tag = "FormCategorizationRules.DeleteButton";
            this.btnEliminarRegla.Values.Text = "Eliminar regla";
            this.btnEliminarRegla.Click += new System.EventHandler(this.btnEliminarRegla_Click);
            // 
            // btnActualizarRegla
            // 
            this.btnActualizarRegla.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarRegla.Location = new System.Drawing.Point(159, 146);
            this.btnActualizarRegla.Name = "btnActualizarRegla";
            this.btnActualizarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnActualizarRegla.TabIndex = 9;
            this.btnActualizarRegla.Tag = "FormCategorizationRules.UpdateButton";
            this.btnActualizarRegla.Values.Text = "Actualizar regla";
            this.btnActualizarRegla.Click += new System.EventHandler(this.btnActualizarRegla_Click);
            // 
            // btnAgregarRegla
            // 
            this.btnAgregarRegla.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarRegla.Location = new System.Drawing.Point(14, 146);
            this.btnAgregarRegla.Name = "btnAgregarRegla";
            this.btnAgregarRegla.Size = new System.Drawing.Size(130, 30);
            this.btnAgregarRegla.TabIndex = 8;
            this.btnAgregarRegla.Tag = "FormCategorizationRules.AddButton";
            this.btnAgregarRegla.Values.Text = "Agregar regla";
            this.btnAgregarRegla.Click += new System.EventHandler(this.btnAgregarRegla_Click);
            // 
            // lstbComerciosRegla
            // 
            this.lstbComerciosRegla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lstbComerciosRegla.CheckOnClick = true;
            this.lstbComerciosRegla.FormattingEnabled = true;
            this.lstbComerciosRegla.Location = new System.Drawing.Point(354, 20);
            this.lstbComerciosRegla.Name = "lstbComerciosRegla";
            this.lstbComerciosRegla.Size = new System.Drawing.Size(355, 109);
            this.lstbComerciosRegla.TabIndex = 7;
            this.lstbComerciosRegla.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // lblComerciosRegla
            // 
            this.lblComerciosRegla.AutoSize = true;
            this.lblComerciosRegla.Location = new System.Drawing.Point(351, 0);
            this.lblComerciosRegla.Name = "lblComerciosRegla";
            this.lblComerciosRegla.Size = new System.Drawing.Size(155, 13);
            this.lblComerciosRegla.TabIndex = 6;
            this.lblComerciosRegla.Tag = "FormCategorizationRules.MerchantsLabel";
            this.lblComerciosRegla.Values.Text = "Comercios asociados (opcional):";
            this.lblComerciosRegla.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // txtbPalabrasClaveRegla
            // 
            this.txtbPalabrasClaveRegla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbPalabrasClaveRegla.Location = new System.Drawing.Point(3, 109);
            this.txtbPalabrasClaveRegla.Name = "txtbPalabrasClaveRegla";
            this.txtbPalabrasClaveRegla.Size = new System.Drawing.Size(330, 20);
            this.txtbPalabrasClaveRegla.TabIndex = 5;
            this.txtbPalabrasClaveRegla.Margin = new System.Windows.Forms.Padding(3, 0, 18, 0);
            // 
            // lblPalabrasClaveRegla
            // 
            this.lblPalabrasClaveRegla.AutoSize = true;
            this.lblPalabrasClaveRegla.Location = new System.Drawing.Point(0, 89);
            this.lblPalabrasClaveRegla.Name = "lblPalabrasClaveRegla";
            this.lblPalabrasClaveRegla.Size = new System.Drawing.Size(171, 13);
            this.lblPalabrasClaveRegla.TabIndex = 4;
            this.lblPalabrasClaveRegla.Tag = "FormCategorizationRules.KeywordsLabel";
            this.lblPalabrasClaveRegla.Values.Text = "Palabras clave (separadas por comas):";
            this.lblPalabrasClaveRegla.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // cmbCategoriaRegla
            // 
            this.cmbCategoriaRegla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategoriaRegla.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaRegla.FormattingEnabled = true;
            this.cmbCategoriaRegla.Location = new System.Drawing.Point(3, 64);
            this.cmbCategoriaRegla.Name = "cmbCategoriaRegla";
            this.cmbCategoriaRegla.Size = new System.Drawing.Size(330, 21);
            this.cmbCategoriaRegla.TabIndex = 3;
            this.cmbCategoriaRegla.Margin = new System.Windows.Forms.Padding(3, 0, 18, 0);
            // 
            // lblCategoriaRegla
            // 
            this.lblCategoriaRegla.AutoSize = true;
            this.lblCategoriaRegla.Location = new System.Drawing.Point(0, 44);
            this.lblCategoriaRegla.Name = "lblCategoriaRegla";
            this.lblCategoriaRegla.Size = new System.Drawing.Size(55, 13);
            this.lblCategoriaRegla.TabIndex = 2;
            this.lblCategoriaRegla.Tag = "FormCategorizationRules.CategoryLabel";
            this.lblCategoriaRegla.Values.Text = "Categoria:";
            this.lblCategoriaRegla.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // txtbNombreRegla
            // 
            this.txtbNombreRegla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreRegla.Location = new System.Drawing.Point(3, 20);
            this.txtbNombreRegla.Name = "txtbNombreRegla";
            this.txtbNombreRegla.Size = new System.Drawing.Size(330, 20);
            this.txtbNombreRegla.TabIndex = 1;
            this.txtbNombreRegla.Margin = new System.Windows.Forms.Padding(3, 0, 18, 0);
            // 
            // lblNombreRegla
            // 
            this.lblNombreRegla.AutoSize = true;
            this.lblNombreRegla.Location = new System.Drawing.Point(0, 0);
            this.lblNombreRegla.Name = "lblNombreRegla";
            this.lblNombreRegla.Size = new System.Drawing.Size(103, 13);
            this.lblNombreRegla.TabIndex = 0;
            this.lblNombreRegla.Tag = "FormCategorizationRules.NameLabel";
            this.lblNombreRegla.Values.Text = "Nombre de la regla:";
            this.lblNombreRegla.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            // 
            // dgvReglas
            // 
            this.dgvReglas.AllowUserToAddRows = false;
            this.dgvReglas.AllowUserToDeleteRows = false;
            this.dgvReglas.AutoGenerateColumns = false;
            this.dgvReglas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReglas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            this.dgvReglas.RowHeadersVisible = false;
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
            // tlpCamposRegla
            // 
            this.tlpCamposRegla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposRegla.Controls.Add(this.lstbComerciosRegla, 1, 1);
            this.tlpCamposRegla.SetRowSpan(this.lstbComerciosRegla, 5);
            this.tlpCamposRegla.Controls.Add(this.lblComerciosRegla, 1, 0);
            this.tlpCamposRegla.Controls.Add(this.txtbPalabrasClaveRegla, 0, 5);
            this.tlpCamposRegla.Controls.Add(this.lblPalabrasClaveRegla, 0, 4);
            this.tlpCamposRegla.Controls.Add(this.cmbCategoriaRegla, 0, 3);
            this.tlpCamposRegla.Controls.Add(this.lblCategoriaRegla, 0, 2);
            this.tlpCamposRegla.Controls.Add(this.txtbNombreRegla, 0, 1);
            this.tlpCamposRegla.Controls.Add(this.lblNombreRegla, 0, 0);
            this.tlpCamposRegla.Location = new System.Drawing.Point(11, 0);
            this.tlpCamposRegla.Name = "tlpCamposRegla";
            this.tlpCamposRegla.Size = new System.Drawing.Size(709, 133);
            this.tlpCamposRegla.ColumnCount = 2;
            this.tlpCamposRegla.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.51F));
            this.tlpCamposRegla.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.49F));
            this.tlpCamposRegla.RowCount = 6;
            this.tlpCamposRegla.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposRegla.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpCamposRegla.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposRegla.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposRegla.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposRegla.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpCamposRegla.TabIndex = 0;
            // 
            // FormReglasCategorizacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 530);
            this.Controls.Add(this.dgvReglas);
            this.Controls.Add(this.groupBoxRegla);
            this.Controls.Add(this.lblTituloGestionReglas);
            this.Name = "FormReglasCategorizacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormCategorizationRules.Title";
            this.Text = "Gestion de reglas de categorizacion";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRegla.Panel)).EndInit();
            this.groupBoxRegla.Panel.ResumeLayout(false);
            this.groupBoxRegla.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxRegla)).EndInit();
            this.groupBoxRegla.ResumeLayout(false);
            this.groupBoxRegla.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReglas)).EndInit();
            this.tlpCamposRegla.ResumeLayout(false);
            this.tlpCamposRegla.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblTituloGestionReglas;
        private Krypton.Toolkit.KryptonGroupBox groupBoxRegla;
        private Krypton.Toolkit.KryptonLabel lblNombreRegla;
        private Krypton.Toolkit.KryptonTextBox txtbNombreRegla;
        private Krypton.Toolkit.KryptonLabel lblCategoriaRegla;
        private Krypton.Toolkit.KryptonComboBox cmbCategoriaRegla;
        private Krypton.Toolkit.KryptonLabel lblPalabrasClaveRegla;
        private Krypton.Toolkit.KryptonTextBox txtbPalabrasClaveRegla;
        private Krypton.Toolkit.KryptonLabel lblComerciosRegla;
        private System.Windows.Forms.CheckedListBox lstbComerciosRegla;
        private Krypton.Toolkit.KryptonButton btnAgregarRegla;
        private Krypton.Toolkit.KryptonButton btnActualizarRegla;
        private Krypton.Toolkit.KryptonButton btnEliminarRegla;
        private Krypton.Toolkit.KryptonButton btnLimpiarRegla;
        private Krypton.Toolkit.KryptonDataGridView dgvReglas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasPalabrasClave;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReglasComercios;
        private System.Windows.Forms.TableLayoutPanel tlpCamposRegla;
    }
}
