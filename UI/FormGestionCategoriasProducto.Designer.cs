namespace TpIDS
{
    partial class FormGestionCategoriasProducto
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTituloCategoriasProducto = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxCategoriasProducto = new Krypton.Toolkit.KryptonGroupBox();
            this.lblNombreCategoriaProducto = new Krypton.Toolkit.KryptonLabel();
            this.txtbNombreCategoriaProducto = new Krypton.Toolkit.KryptonTextBox();
            this.btnAgregarCategoriaProducto = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarCategoriaProducto = new Krypton.Toolkit.KryptonButton();
            this.btnEliminarCategoriaProducto = new Krypton.Toolkit.KryptonButton();
            this.dgvCategoriasProducto = new Krypton.Toolkit.KryptonDataGridView();
            this.colCategoriaProductoNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoriasProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoriasProducto.Panel)).BeginInit();
            this.groupBoxCategoriasProducto.Panel.SuspendLayout();
            this.groupBoxCategoriasProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategoriasProducto)).BeginInit();
            this.SuspendLayout();
            // lblTituloCategoriasProducto
            this.lblTituloCategoriasProducto.Location = new System.Drawing.Point(18, 15);
            this.lblTituloCategoriasProducto.Name = "lblTituloCategoriasProducto";
            this.lblTituloCategoriasProducto.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloCategoriasProducto.TabIndex = 0;
            this.lblTituloCategoriasProducto.Tag = "FormProductCategoryManagement.TitleLabel";
            this.lblTituloCategoriasProducto.Values.Text = "Gestion de Categorias de producto";
            // groupBoxCategoriasProducto
            this.groupBoxCategoriasProducto.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.groupBoxCategoriasProducto.Location = new System.Drawing.Point(23, 52);
            this.groupBoxCategoriasProducto.Name = "groupBoxCategoriasProducto";
            this.groupBoxCategoriasProducto.Size = new System.Drawing.Size(594, 410);
            this.groupBoxCategoriasProducto.TabIndex = 1;
            this.groupBoxCategoriasProducto.Tag = "FormProductCategoryManagement.CategoryGroup";
            this.groupBoxCategoriasProducto.Values.Heading = "Categorias de producto";
            this.groupBoxCategoriasProducto.Panel.Controls.Add(this.lblNombreCategoriaProducto);
            this.groupBoxCategoriasProducto.Panel.Controls.Add(this.txtbNombreCategoriaProducto);
            this.groupBoxCategoriasProducto.Panel.Controls.Add(this.btnAgregarCategoriaProducto);
            this.groupBoxCategoriasProducto.Panel.Controls.Add(this.btnActualizarCategoriaProducto);
            this.groupBoxCategoriasProducto.Panel.Controls.Add(this.btnEliminarCategoriaProducto);
            this.groupBoxCategoriasProducto.Panel.Controls.Add(this.dgvCategoriasProducto);
            // lblNombreCategoriaProducto
            this.lblNombreCategoriaProducto.Location = new System.Drawing.Point(14, 9);
            this.lblNombreCategoriaProducto.Name = "lblNombreCategoriaProducto";
            this.lblNombreCategoriaProducto.TabIndex = 0;
            this.lblNombreCategoriaProducto.Tag = "FormProductCategoryManagement.NombreLabel";
            this.lblNombreCategoriaProducto.Values.Text = "Nombre:";
            // txtbNombreCategoriaProducto
            this.txtbNombreCategoriaProducto.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtbNombreCategoriaProducto.Location = new System.Drawing.Point(85, 9);
            this.txtbNombreCategoriaProducto.Name = "txtbNombreCategoriaProducto";
            this.txtbNombreCategoriaProducto.MaxLength = 150;
            this.txtbNombreCategoriaProducto.Size = new System.Drawing.Size(495, 23);
            this.txtbNombreCategoriaProducto.TabIndex = 1;
            // btnAgregarCategoriaProducto
            this.btnAgregarCategoriaProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarCategoriaProducto.Location = new System.Drawing.Point(14, 45);
            this.btnAgregarCategoriaProducto.Name = "btnAgregarCategoriaProducto";
            this.btnAgregarCategoriaProducto.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarCategoriaProducto.TabIndex = 2;
            this.btnAgregarCategoriaProducto.Tag = "FormProductCategoryManagement.AddButton";
            this.btnAgregarCategoriaProducto.Values.Text = "Agregar";
            this.btnAgregarCategoriaProducto.Click += new System.EventHandler(this.btnAgregarCategoriaProducto_Click);
            // btnActualizarCategoriaProducto
            this.btnActualizarCategoriaProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarCategoriaProducto.Location = new System.Drawing.Point(134, 45);
            this.btnActualizarCategoriaProducto.Name = "btnActualizarCategoriaProducto";
            this.btnActualizarCategoriaProducto.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarCategoriaProducto.TabIndex = 3;
            this.btnActualizarCategoriaProducto.Tag = "FormProductCategoryManagement.UpdateButton";
            this.btnActualizarCategoriaProducto.Values.Text = "Actualizar";
            this.btnActualizarCategoriaProducto.Click += new System.EventHandler(this.btnActualizarCategoriaProducto_Click);
            // btnEliminarCategoriaProducto
            this.btnEliminarCategoriaProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarCategoriaProducto.Location = new System.Drawing.Point(254, 45);
            this.btnEliminarCategoriaProducto.Name = "btnEliminarCategoriaProducto";
            this.btnEliminarCategoriaProducto.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarCategoriaProducto.TabIndex = 4;
            this.btnEliminarCategoriaProducto.Tag = "FormProductCategoryManagement.DeleteButton";
            this.btnEliminarCategoriaProducto.Values.Text = "Eliminar";
            this.btnEliminarCategoriaProducto.Click += new System.EventHandler(this.btnEliminarCategoriaProducto_Click);
            // dgvCategoriasProducto
            this.dgvCategoriasProducto.AllowUserToAddRows = false;
            this.dgvCategoriasProducto.AllowUserToDeleteRows = false;
            this.dgvCategoriasProducto.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.dgvCategoriasProducto.AutoGenerateColumns = false;
            this.dgvCategoriasProducto.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCategoriasProducto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCategoriasProducto.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.colCategoriaProductoNombre });
            this.dgvCategoriasProducto.Location = new System.Drawing.Point(14, 84);
            this.dgvCategoriasProducto.MultiSelect = false;
            this.dgvCategoriasProducto.Name = "dgvCategoriasProducto";
            this.dgvCategoriasProducto.ReadOnly = true;
            this.dgvCategoriasProducto.RowHeadersVisible = false;
            this.dgvCategoriasProducto.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCategoriasProducto.Size = new System.Drawing.Size(566, 285);
            this.dgvCategoriasProducto.TabIndex = 5;
            this.dgvCategoriasProducto.Tag = "FormProductCategoryManagement.CategoryGrid";
            this.dgvCategoriasProducto.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCategoriasProducto_CellClick);
            // colCategoriaProductoNombre
            this.colCategoriaProductoNombre.DataPropertyName = "Nombre";
            this.colCategoriaProductoNombre.HeaderText = "Nombre";
            this.colCategoriaProductoNombre.Name = "Nombre";
            this.colCategoriaProductoNombre.ReadOnly = true;
            // FormGestionCategoriasProducto
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 490);
            this.Controls.Add(this.groupBoxCategoriasProducto);
            this.Controls.Add(this.lblTituloCategoriasProducto);
            this.Name = "FormGestionCategoriasProducto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormProductCategoryManagement.Title";
            this.Text = "Gestion de Categorias de producto";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoriasProducto.Panel)).EndInit();
            this.groupBoxCategoriasProducto.Panel.ResumeLayout(false);
            this.groupBoxCategoriasProducto.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoriasProducto)).EndInit();
            this.groupBoxCategoriasProducto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategoriasProducto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Krypton.Toolkit.KryptonLabel lblTituloCategoriasProducto;
        private Krypton.Toolkit.KryptonGroupBox groupBoxCategoriasProducto;
        private Krypton.Toolkit.KryptonLabel lblNombreCategoriaProducto;
        private Krypton.Toolkit.KryptonTextBox txtbNombreCategoriaProducto;
        private Krypton.Toolkit.KryptonButton btnAgregarCategoriaProducto;
        private Krypton.Toolkit.KryptonButton btnActualizarCategoriaProducto;
        private Krypton.Toolkit.KryptonButton btnEliminarCategoriaProducto;
        private Krypton.Toolkit.KryptonDataGridView dgvCategoriasProducto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategoriaProductoNombre;
    }
}
