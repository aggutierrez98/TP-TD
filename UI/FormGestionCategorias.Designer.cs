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
            this.lblTituloGestionCategorias = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxCategoria = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarCategoria = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarCategoria = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarCategoria = new Krypton.Toolkit.KryptonButton();
            this.txtbNombreCategoria = new Krypton.Toolkit.KryptonTextBox();
            this.lblNombreCategoria = new Krypton.Toolkit.KryptonLabel();
            this.dgvCategorias = new Krypton.Toolkit.KryptonDataGridView();
            this.colCategoriasNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategoriasScope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoria.Panel)).BeginInit();
            this.groupBoxCategoria.Panel.SuspendLayout();
            this.groupBoxCategoria.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloGestionCategorias
            //
            this.lblTituloGestionCategorias.Location = new System.Drawing.Point(18, 15);
            this.lblTituloGestionCategorias.Name = "lblTituloGestionCategorias";
            this.lblTituloGestionCategorias.Size = new System.Drawing.Size(219, 25);
            this.lblTituloGestionCategorias.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloGestionCategorias.TabIndex = 0;
            this.lblTituloGestionCategorias.Tag = "FormCategories.TitleLabel";
            this.lblTituloGestionCategorias.Values.Text = "Gestion de categorias";
            //
            // groupBoxCategoria
            //
            this.groupBoxCategoria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCategoria.Location = new System.Drawing.Point(23, 52);
            this.groupBoxCategoria.Name = "groupBoxCategoria";
            //
            // groupBoxCategoria.Panel
            //
            this.groupBoxCategoria.Panel.Controls.Add(this.btnEliminarCategoria);
            this.groupBoxCategoria.Panel.Controls.Add(this.btnActualizarCategoria);
            this.groupBoxCategoria.Panel.Controls.Add(this.btnAgregarCategoria);
            this.groupBoxCategoria.Panel.Controls.Add(this.txtbNombreCategoria);
            this.groupBoxCategoria.Panel.Controls.Add(this.lblNombreCategoria);
            this.groupBoxCategoria.Size = new System.Drawing.Size(454, 116);
            this.groupBoxCategoria.TabIndex = 1;
            this.groupBoxCategoria.TabStop = false;
            this.groupBoxCategoria.Tag = "FormCategories.CategoryGroup";
            this.groupBoxCategoria.Values.Heading = "Datos de la categoria";
            //
            // btnEliminarCategoria
            //
            this.btnEliminarCategoria.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarCategoria.Location = new System.Drawing.Point(294, 45);
            this.btnEliminarCategoria.Name = "btnEliminarCategoria";
            this.btnEliminarCategoria.Size = new System.Drawing.Size(135, 30);
            this.btnEliminarCategoria.TabIndex = 4;
            this.btnEliminarCategoria.Tag = "FormCategories.DeleteButton";
            this.btnEliminarCategoria.Values.Text = "Eliminar categoria";
            this.btnEliminarCategoria.Click += new System.EventHandler(this.btnEliminarCategoria_Click);
            //
            // btnActualizarCategoria
            //
            this.btnActualizarCategoria.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarCategoria.Location = new System.Drawing.Point(154, 45);
            this.btnActualizarCategoria.Name = "btnActualizarCategoria";
            this.btnActualizarCategoria.Size = new System.Drawing.Size(135, 30);
            this.btnActualizarCategoria.TabIndex = 3;
            this.btnActualizarCategoria.Tag = "FormCategories.UpdateButton";
            this.btnActualizarCategoria.Values.Text = "Actualizar categoria";
            this.btnActualizarCategoria.Click += new System.EventHandler(this.btnActualizarCategoria_Click);
            //
            // btnAgregarCategoria
            //
            this.btnAgregarCategoria.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarCategoria.Location = new System.Drawing.Point(14, 45);
            this.btnAgregarCategoria.Name = "btnAgregarCategoria";
            this.btnAgregarCategoria.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarCategoria.TabIndex = 2;
            this.btnAgregarCategoria.Tag = "FormCategories.AddButton";
            this.btnAgregarCategoria.Values.Text = "Agregar categoria";
            this.btnAgregarCategoria.Click += new System.EventHandler(this.btnAgregarCategoria_Click);
            //
            // txtbNombreCategoria
            //
            this.txtbNombreCategoria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbNombreCategoria.Location = new System.Drawing.Point(14, 21);
            this.txtbNombreCategoria.Name = "txtbNombreCategoria";
            this.txtbNombreCategoria.Size = new System.Drawing.Size(415, 20);
            this.txtbNombreCategoria.TabIndex = 1;
            //
            // lblNombreCategoria
            //
            this.lblNombreCategoria.Location = new System.Drawing.Point(11, 0);
            this.lblNombreCategoria.Name = "lblNombreCategoria";
            this.lblNombreCategoria.Size = new System.Drawing.Size(115, 20);
            this.lblNombreCategoria.TabIndex = 0;
            this.lblNombreCategoria.Tag = "FormCategories.NameLabel";
            this.lblNombreCategoria.Values.Text = "Nombre de la categoria:";
            //
            // dgvCategorias
            //
            this.dgvCategorias.AllowUserToAddRows = false;
            this.dgvCategorias.AllowUserToDeleteRows = false;
            this.dgvCategorias.AutoGenerateColumns = false;
            this.dgvCategorias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCategorias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            this.Name = "FormGestionCategorias";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormCategories.Title";
            this.Text = "Gestion de categorias";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoria.Panel)).EndInit();
            this.groupBoxCategoria.Panel.ResumeLayout(false);
            this.groupBoxCategoria.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxCategoria)).EndInit();
            this.groupBoxCategoria.ResumeLayout(false);
            this.groupBoxCategoria.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategorias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloGestionCategorias;
        private Krypton.Toolkit.KryptonGroupBox groupBoxCategoria;
        private Krypton.Toolkit.KryptonButton btnEliminarCategoria;
        private Krypton.Toolkit.KryptonButton btnActualizarCategoria;
        private Krypton.Toolkit.KryptonButton btnAgregarCategoria;
        private Krypton.Toolkit.KryptonTextBox txtbNombreCategoria;
        private Krypton.Toolkit.KryptonLabel lblNombreCategoria;
        private Krypton.Toolkit.KryptonDataGridView dgvCategorias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategoriasNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategoriasScope;
    }
}
