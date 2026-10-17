namespace TpIDS
{
    partial class FormGestionProductos
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
            this.lblTituloProductos = new Krypton.Toolkit.KryptonLabel();
            this.lblSinCategoriasProducto = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxProductos = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarProducto = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarProducto = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarProducto = new Krypton.Toolkit.KryptonButton();
            this.tlpCamposProducto = new System.Windows.Forms.TableLayoutPanel();
            this.lblNombreProducto = new Krypton.Toolkit.KryptonLabel();
            this.txtbNombreProducto = new Krypton.Toolkit.KryptonTextBox();
            this.lblCategoriaProducto = new Krypton.Toolkit.KryptonLabel();
            this.cmbCategoriaProducto = new Krypton.Toolkit.KryptonComboBox();
            this.dgvProductos = new Krypton.Toolkit.KryptonDataGridView();
            this.colProductoNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductoCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProductos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProductos.Panel)).BeginInit();
            this.groupBoxProductos.Panel.SuspendLayout();
            this.groupBoxProductos.SuspendLayout();
            this.tlpCamposProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCategoriaProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloProductos
            //
            this.lblTituloProductos.AutoSize = true;
            this.lblTituloProductos.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloProductos.Location = new System.Drawing.Point(18, 15);
            this.lblTituloProductos.Name = "lblTituloProductos";
            this.lblTituloProductos.Size = new System.Drawing.Size(240, 25);
            this.lblTituloProductos.TabIndex = 0;
            this.lblTituloProductos.Tag = "FormProductManagement.TitleLabel";
            this.lblTituloProductos.Values.Text = "Gestion de Productos";
            //
            // groupBoxProductos
            //
            this.groupBoxProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProductos.Panel.Controls.Add(this.btnEliminarProducto);
            this.groupBoxProductos.Panel.Controls.Add(this.btnActualizarProducto);
            this.groupBoxProductos.Panel.Controls.Add(this.btnAgregarProducto);
            this.groupBoxProductos.Panel.Controls.Add(this.tlpCamposProducto);
            this.groupBoxProductos.Panel.Controls.Add(this.dgvProductos);
            this.groupBoxProductos.Panel.Controls.Add(this.lblSinCategoriasProducto);
            this.groupBoxProductos.Location = new System.Drawing.Point(23, 52);
            this.groupBoxProductos.Name = "groupBoxProductos";
            this.groupBoxProductos.Size = new System.Drawing.Size(594, 410);
            this.groupBoxProductos.TabIndex = 1;
            this.groupBoxProductos.TabStop = false;
            this.groupBoxProductos.Tag = "FormProductManagement.ProductGroup";
            this.groupBoxProductos.Values.Heading = "Productos";
            //
            // btnEliminarProducto
            //
            this.btnEliminarProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarProducto.Location = new System.Drawing.Point(254, 45);
            this.btnEliminarProducto.Name = "btnEliminarProducto";
            this.btnEliminarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarProducto.TabIndex = 3;
            this.btnEliminarProducto.Tag = "FormProductManagement.DeleteButton";
            this.btnEliminarProducto.Values.Text = "Eliminar";
            this.btnEliminarProducto.Click += new System.EventHandler(this.btnEliminarProducto_Click);
            //
            // btnActualizarProducto
            //
            this.btnActualizarProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarProducto.Location = new System.Drawing.Point(134, 45);
            this.btnActualizarProducto.Name = "btnActualizarProducto";
            this.btnActualizarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarProducto.TabIndex = 2;
            this.btnActualizarProducto.Tag = "FormProductManagement.UpdateButton";
            this.btnActualizarProducto.Values.Text = "Actualizar";
            this.btnActualizarProducto.Click += new System.EventHandler(this.btnActualizarProducto_Click);
            //
            // btnAgregarProducto
            //
            this.btnAgregarProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarProducto.Location = new System.Drawing.Point(14, 45);
            this.btnAgregarProducto.Name = "btnAgregarProducto";
            this.btnAgregarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarProducto.TabIndex = 1;
            this.btnAgregarProducto.Tag = "FormProductManagement.AddButton";
            this.btnAgregarProducto.Values.Text = "Agregar";
            this.btnAgregarProducto.Click += new System.EventHandler(this.btnAgregarProducto_Click);
            //
            // tlpCamposProducto
            //
            this.tlpCamposProducto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposProducto.ColumnCount = 4;
            this.tlpCamposProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tlpCamposProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tlpCamposProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlpCamposProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpCamposProducto.Controls.Add(this.lblNombreProducto, 0, 0);
            this.tlpCamposProducto.Controls.Add(this.txtbNombreProducto, 1, 0);
            this.tlpCamposProducto.Controls.Add(this.lblCategoriaProducto, 2, 0);
            this.tlpCamposProducto.Controls.Add(this.cmbCategoriaProducto, 3, 0);
            this.tlpCamposProducto.Location = new System.Drawing.Point(11, 9);
            this.tlpCamposProducto.Name = "tlpCamposProducto";
            this.tlpCamposProducto.RowCount = 1;
            this.tlpCamposProducto.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpCamposProducto.Size = new System.Drawing.Size(569, 28);
            this.tlpCamposProducto.TabIndex = 0;
            //
            // lblNombreProducto
            //
            this.lblNombreProducto.AutoSize = true;
            this.lblNombreProducto.Location = new System.Drawing.Point(0, 0);
            this.lblNombreProducto.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblNombreProducto.Name = "lblNombreProducto";
            this.lblNombreProducto.Size = new System.Drawing.Size(50, 13);
            this.lblNombreProducto.TabIndex = 0;
            this.lblNombreProducto.Tag = "FormProductManagement.NombreLabel";
            this.lblNombreProducto.Values.Text = "Nombre:";
            //
            // txtbNombreProducto
            //
            this.txtbNombreProducto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreProducto.Location = new System.Drawing.Point(59, 1);
            this.txtbNombreProducto.Margin = new System.Windows.Forms.Padding(0, 1, 20, 0);
            this.txtbNombreProducto.MaxLength = 150;
            this.txtbNombreProducto.Name = "txtbNombreProducto";
            this.txtbNombreProducto.Size = new System.Drawing.Size(250, 20);
            this.txtbNombreProducto.TabIndex = 1;
            //
            // lblCategoriaProducto
            //
            this.lblCategoriaProducto.AutoSize = true;
            this.lblCategoriaProducto.Location = new System.Drawing.Point(330, 0);
            this.lblCategoriaProducto.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblCategoriaProducto.Name = "lblCategoriaProducto";
            this.lblCategoriaProducto.Size = new System.Drawing.Size(62, 13);
            this.lblCategoriaProducto.TabIndex = 2;
            this.lblCategoriaProducto.Tag = "FormProductManagement.CategoriaLabel";
            this.lblCategoriaProducto.Values.Text = "Categoria:";
            //
            // cmbCategoriaProducto
            //
            this.cmbCategoriaProducto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategoriaProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaProducto.FormattingEnabled = true;
            this.cmbCategoriaProducto.Location = new System.Drawing.Point(400, 0);
            this.cmbCategoriaProducto.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.cmbCategoriaProducto.Name = "cmbCategoriaProducto";
            this.cmbCategoriaProducto.Size = new System.Drawing.Size(169, 21);
            this.cmbCategoriaProducto.TabIndex = 3;
            //
            // dgvProductos
            //
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.AllowUserToDeleteRows = false;
            this.dgvProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProductos.AutoGenerateColumns = false;
            this.dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProductoNombre,
            this.colProductoCategoria});
            this.dgvProductos.Location = new System.Drawing.Point(14, 114);
            this.dgvProductos.MultiSelect = false;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.ReadOnly = true;
            this.dgvProductos.RowHeadersVisible = false;
            this.dgvProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductos.Size = new System.Drawing.Size(566, 255);
            this.dgvProductos.TabIndex = 4;
            this.dgvProductos.Tag = "FormProductManagement.ProductGrid";
            this.dgvProductos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProductos_CellClick);
            // lblSinCategoriasProducto
            this.lblSinCategoriasProducto.Location = new System.Drawing.Point(14, 84);
            this.lblSinCategoriasProducto.Name = "lblSinCategoriasProducto";
            this.lblSinCategoriasProducto.TabIndex = 5;
            this.lblSinCategoriasProducto.Tag = "FormProductManagement.NoCategoriesHint";
            this.lblSinCategoriasProducto.Values.Text = "No hay categorias de producto. Crealas en Operador > Categorias de producto.";
            this.lblSinCategoriasProducto.Visible = false;
            //
            // colProductoNombre
            //
            this.colProductoNombre.DataPropertyName = "Nombre";
            this.colProductoNombre.HeaderText = "Nombre";
            this.colProductoNombre.Name = "Nombre";
            this.colProductoNombre.ReadOnly = true;
            //
            // colProductoCategoria
            //
            this.colProductoCategoria.DataPropertyName = "Categoria";
            this.colProductoCategoria.HeaderText = "Categoria";
            this.colProductoCategoria.Name = "Categoria";
            this.colProductoCategoria.ReadOnly = true;
            //
            // FormGestionProductos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 490);
            this.Controls.Add(this.groupBoxProductos);
            this.Controls.Add(this.lblTituloProductos);
            this.Name = "FormGestionProductos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormProductManagement.Title";
            this.Text = "Gestion de Productos";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProductos.Panel)).EndInit();
            this.groupBoxProductos.Panel.ResumeLayout(false);
            this.groupBoxProductos.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProductos)).EndInit();
            this.groupBoxProductos.ResumeLayout(false);
            this.groupBoxProductos.PerformLayout();
            this.tlpCamposProducto.ResumeLayout(false);
            this.tlpCamposProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCategoriaProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloProductos;
        private Krypton.Toolkit.KryptonLabel lblSinCategoriasProducto;
        private Krypton.Toolkit.KryptonGroupBox groupBoxProductos;
        private Krypton.Toolkit.KryptonButton btnEliminarProducto;
        private Krypton.Toolkit.KryptonButton btnActualizarProducto;
        private Krypton.Toolkit.KryptonButton btnAgregarProducto;
        private System.Windows.Forms.TableLayoutPanel tlpCamposProducto;
        private Krypton.Toolkit.KryptonLabel lblNombreProducto;
        private Krypton.Toolkit.KryptonTextBox txtbNombreProducto;
        private Krypton.Toolkit.KryptonLabel lblCategoriaProducto;
        private Krypton.Toolkit.KryptonComboBox cmbCategoriaProducto;
        private Krypton.Toolkit.KryptonDataGridView dgvProductos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoCategoria;
    }
}
