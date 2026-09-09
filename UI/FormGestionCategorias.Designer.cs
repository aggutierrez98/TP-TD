namespace TpIDS
{
    partial class FormGestionCategorias
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
            this.lblTituloGestionCategorias = new System.Windows.Forms.Label();
            this.groupBoxCategoria = new System.Windows.Forms.GroupBox();
            this.btnEliminarCategoria = new System.Windows.Forms.Button();
            this.btnActualizarCategoria = new System.Windows.Forms.Button();
            this.btnAgregarCategoria = new System.Windows.Forms.Button();
            this.txtbNombreCategoria = new System.Windows.Forms.TextBox();
            this.lblNombreCategoria = new System.Windows.Forms.Label();
            this.dgvCategorias = new System.Windows.Forms.DataGridView();
            this.colCategoriasNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategoriasScope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxCategoria.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloGestionCategorias
            //
            this.lblTituloGestionCategorias.AutoSize = true;
            this.lblTituloGestionCategorias.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionCategorias.Location = new System.Drawing.Point(18, 15);
            this.lblTituloGestionCategorias.Name = "lblTituloGestionCategorias";
            this.lblTituloGestionCategorias.Size = new System.Drawing.Size(219, 25);
            this.lblTituloGestionCategorias.TabIndex = 0;
            this.lblTituloGestionCategorias.Tag = "FormCategories.TitleLabel";
            this.lblTituloGestionCategorias.Text = "Gestion de categorias";
            //
            // groupBoxCategoria
            //
            this.groupBoxCategoria.Controls.Add(this.btnEliminarCategoria);
            this.groupBoxCategoria.Controls.Add(this.btnActualizarCategoria);
            this.groupBoxCategoria.Controls.Add(this.btnAgregarCategoria);
            this.groupBoxCategoria.Controls.Add(this.txtbNombreCategoria);
            this.groupBoxCategoria.Controls.Add(this.lblNombreCategoria);
            this.groupBoxCategoria.Location = new System.Drawing.Point(23, 52);
            this.groupBoxCategoria.Name = "groupBoxCategoria";
            this.groupBoxCategoria.Size = new System.Drawing.Size(454, 116);
            this.groupBoxCategoria.TabIndex = 1;
            this.groupBoxCategoria.TabStop = false;
            this.groupBoxCategoria.Tag = "FormCategories.CategoryGroup";
            this.groupBoxCategoria.Text = "Datos de la categoria";
            //
            // btnEliminarCategoria
            //
            this.btnEliminarCategoria.Location = new System.Drawing.Point(294, 69);
            this.btnEliminarCategoria.Name = "btnEliminarCategoria";
            this.btnEliminarCategoria.Size = new System.Drawing.Size(135, 30);
            this.btnEliminarCategoria.TabIndex = 4;
            this.btnEliminarCategoria.Tag = "FormCategories.DeleteButton";
            this.btnEliminarCategoria.Text = "Eliminar categoria";
            this.btnEliminarCategoria.UseVisualStyleBackColor = true;
            this.btnEliminarCategoria.Click += new System.EventHandler(this.btnEliminarCategoria_Click);
            //
            // btnActualizarCategoria
            //
            this.btnActualizarCategoria.Location = new System.Drawing.Point(154, 69);
            this.btnActualizarCategoria.Name = "btnActualizarCategoria";
            this.btnActualizarCategoria.Size = new System.Drawing.Size(135, 30);
            this.btnActualizarCategoria.TabIndex = 3;
            this.btnActualizarCategoria.Tag = "FormCategories.UpdateButton";
            this.btnActualizarCategoria.Text = "Actualizar categoria";
            this.btnActualizarCategoria.UseVisualStyleBackColor = true;
            this.btnActualizarCategoria.Click += new System.EventHandler(this.btnActualizarCategoria_Click);
            //
            // btnAgregarCategoria
            //
            this.btnAgregarCategoria.Location = new System.Drawing.Point(14, 69);
            this.btnAgregarCategoria.Name = "btnAgregarCategoria";
            this.btnAgregarCategoria.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarCategoria.TabIndex = 2;
            this.btnAgregarCategoria.Tag = "FormCategories.AddButton";
            this.btnAgregarCategoria.Text = "Agregar categoria";
            this.btnAgregarCategoria.UseVisualStyleBackColor = true;
            this.btnAgregarCategoria.Click += new System.EventHandler(this.btnAgregarCategoria_Click);
            //
            // txtbNombreCategoria
            //
            this.txtbNombreCategoria.Location = new System.Drawing.Point(14, 31);
            this.txtbNombreCategoria.Name = "txtbNombreCategoria";
            this.txtbNombreCategoria.Size = new System.Drawing.Size(415, 20);
            this.txtbNombreCategoria.TabIndex = 1;
            //
            // lblNombreCategoria
            //
            this.lblNombreCategoria.AutoSize = true;
            this.lblNombreCategoria.Location = new System.Drawing.Point(11, 15);
            this.lblNombreCategoria.Name = "lblNombreCategoria";
            this.lblNombreCategoria.Size = new System.Drawing.Size(115, 13);
            this.lblNombreCategoria.TabIndex = 0;
            this.lblNombreCategoria.Tag = "FormCategories.NameLabel";
            this.lblNombreCategoria.Text = "Nombre de la categoria:";
            //
            // dgvCategorias
            //
            this.dgvCategorias.AllowUserToAddRows = false;
            this.dgvCategorias.AllowUserToDeleteRows = false;
            this.dgvCategorias.AutoGenerateColumns = false;
            this.dgvCategorias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCategorias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCategorias.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCategoriasNombre,
            this.colCategoriasScope});
            this.dgvCategorias.Location = new System.Drawing.Point(23, 185);
            this.dgvCategorias.MultiSelect = false;
            this.dgvCategorias.Name = "dgvCategorias";
            this.dgvCategorias.ReadOnly = true;
            this.dgvCategorias.RowHeadersVisible = false;
            this.dgvCategorias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCategorias.Size = new System.Drawing.Size(454, 280);
            this.dgvCategorias.TabIndex = 2;
            this.dgvCategorias.Tag = "FormCategories.Grid";
            this.dgvCategorias.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCategorias_CellClick);
            //
            // colCategoriasNombre
            //
            this.colCategoriasNombre.DataPropertyName = "Nombre";
            this.colCategoriasNombre.HeaderText = "Nombre";
            this.colCategoriasNombre.Name = "Nombre";
            this.colCategoriasNombre.ReadOnly = true;
            //
            // colCategoriasScope
            //
            this.colCategoriasScope.DataPropertyName = "Scope";
            this.colCategoriasScope.HeaderText = "Alcance";
            this.colCategoriasScope.Name = "Scope";
            this.colCategoriasScope.ReadOnly = true;
            //
            // FormGestionCategorias
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 486);
            this.Controls.Add(this.dgvCategorias);
            this.Controls.Add(this.groupBoxCategoria);
            this.Controls.Add(this.lblTituloGestionCategorias);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormGestionCategorias";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormCategories.Title";
            this.Text = "Gestion de categorias";
            this.groupBoxCategoria.ResumeLayout(false);
            this.groupBoxCategoria.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloGestionCategorias;
        private System.Windows.Forms.GroupBox groupBoxCategoria;
        private System.Windows.Forms.Button btnEliminarCategoria;
        private System.Windows.Forms.Button btnActualizarCategoria;
        private System.Windows.Forms.Button btnAgregarCategoria;
        private System.Windows.Forms.TextBox txtbNombreCategoria;
        private System.Windows.Forms.Label lblNombreCategoria;
        private System.Windows.Forms.DataGridView dgvCategorias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategoriasNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategoriasScope;
    }
}
