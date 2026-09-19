namespace TpIDS
{
    partial class FormListaDeCompras
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
            this.lblTituloListaDeCompras = new System.Windows.Forms.Label();
            this.groupBoxItem = new System.Windows.Forms.GroupBox();
            this.btnEliminarItem = new System.Windows.Forms.Button();
            this.btnAgregarItem = new System.Windows.Forms.Button();
            this.txtbCantidadItem = new System.Windows.Forms.TextBox();
            this.lblCantidadItem = new System.Windows.Forms.Label();
            this.txtbDescripcionItem = new System.Windows.Forms.TextBox();
            this.lblDescripcionItem = new System.Windows.Forms.Label();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.colItemDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemComprado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBoxItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloListaDeCompras
            //
            this.lblTituloListaDeCompras.AutoSize = true;
            this.lblTituloListaDeCompras.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloListaDeCompras.Location = new System.Drawing.Point(18, 15);
            this.lblTituloListaDeCompras.Name = "lblTituloListaDeCompras";
            this.lblTituloListaDeCompras.Size = new System.Drawing.Size(219, 25);
            this.lblTituloListaDeCompras.TabIndex = 0;
            this.lblTituloListaDeCompras.Tag = "FormShoppingList.TitleLabel";
            this.lblTituloListaDeCompras.Text = "Lista de compras";
            //
            // groupBoxItem
            //
            this.groupBoxItem.Controls.Add(this.btnEliminarItem);
            this.groupBoxItem.Controls.Add(this.btnAgregarItem);
            this.groupBoxItem.Controls.Add(this.txtbCantidadItem);
            this.groupBoxItem.Controls.Add(this.lblCantidadItem);
            this.groupBoxItem.Controls.Add(this.txtbDescripcionItem);
            this.groupBoxItem.Controls.Add(this.lblDescripcionItem);
            this.groupBoxItem.Location = new System.Drawing.Point(23, 52);
            this.groupBoxItem.Name = "groupBoxItem";
            this.groupBoxItem.Size = new System.Drawing.Size(454, 116);
            this.groupBoxItem.TabIndex = 1;
            this.groupBoxItem.TabStop = false;
            this.groupBoxItem.Tag = "FormShoppingList.ItemGroup";
            this.groupBoxItem.Text = "Datos del item";
            //
            // btnEliminarItem
            //
            this.btnEliminarItem.Location = new System.Drawing.Point(294, 79);
            this.btnEliminarItem.Name = "btnEliminarItem";
            this.btnEliminarItem.Size = new System.Drawing.Size(135, 30);
            this.btnEliminarItem.TabIndex = 5;
            this.btnEliminarItem.Tag = "FormShoppingList.DeleteButton";
            this.btnEliminarItem.Text = "Eliminar item";
            this.btnEliminarItem.UseVisualStyleBackColor = true;
            this.btnEliminarItem.Click += new System.EventHandler(this.btnEliminarItem_Click);
            //
            // btnAgregarItem
            //
            this.btnAgregarItem.Location = new System.Drawing.Point(14, 79);
            this.btnAgregarItem.Name = "btnAgregarItem";
            this.btnAgregarItem.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarItem.TabIndex = 4;
            this.btnAgregarItem.Tag = "FormShoppingList.AddButton";
            this.btnAgregarItem.Text = "Agregar item";
            this.btnAgregarItem.UseVisualStyleBackColor = true;
            this.btnAgregarItem.Click += new System.EventHandler(this.btnAgregarItem_Click);
            //
            // txtbCantidadItem
            //
            this.txtbCantidadItem.Location = new System.Drawing.Point(340, 44);
            this.txtbCantidadItem.Name = "txtbCantidadItem";
            this.txtbCantidadItem.Size = new System.Drawing.Size(89, 20);
            this.txtbCantidadItem.TabIndex = 3;
            //
            // lblCantidadItem
            //
            this.lblCantidadItem.AutoSize = true;
            this.lblCantidadItem.Location = new System.Drawing.Point(337, 28);
            this.lblCantidadItem.Name = "lblCantidadItem";
            this.lblCantidadItem.Size = new System.Drawing.Size(60, 13);
            this.lblCantidadItem.TabIndex = 2;
            this.lblCantidadItem.Tag = "FormShoppingList.QuantityLabel";
            this.lblCantidadItem.Text = "Cantidad:";
            //
            // txtbDescripcionItem
            //
            this.txtbDescripcionItem.Location = new System.Drawing.Point(14, 44);
            this.txtbDescripcionItem.Name = "txtbDescripcionItem";
            this.txtbDescripcionItem.Size = new System.Drawing.Size(310, 20);
            this.txtbDescripcionItem.TabIndex = 1;
            //
            // lblDescripcionItem
            //
            this.lblDescripcionItem.AutoSize = true;
            this.lblDescripcionItem.Location = new System.Drawing.Point(11, 28);
            this.lblDescripcionItem.Name = "lblDescripcionItem";
            this.lblDescripcionItem.Size = new System.Drawing.Size(115, 13);
            this.lblDescripcionItem.TabIndex = 0;
            this.lblDescripcionItem.Tag = "FormShoppingList.DescriptionLabel";
            this.lblDescripcionItem.Text = "Descripcion:";
            //
            // dgvItems
            //
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AutoGenerateColumns = false;
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItemDescripcion,
            this.colItemCantidad,
            this.colItemComprado});
            this.dgvItems.Location = new System.Drawing.Point(23, 185);
            this.dgvItems.MultiSelect = false;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(454, 280);
            this.dgvItems.TabIndex = 2;
            this.dgvItems.Tag = "FormShoppingList.Grid";
            //
            // colItemDescripcion
            //
            this.colItemDescripcion.DataPropertyName = "Descripcion";
            this.colItemDescripcion.HeaderText = "Descripcion";
            this.colItemDescripcion.Name = "Descripcion";
            this.colItemDescripcion.ReadOnly = true;
            //
            // colItemCantidad
            //
            this.colItemCantidad.DataPropertyName = "Cantidad";
            this.colItemCantidad.HeaderText = "Cantidad";
            this.colItemCantidad.Name = "Cantidad";
            this.colItemCantidad.ReadOnly = true;
            //
            // colItemComprado
            //
            this.colItemComprado.DataPropertyName = "Comprado";
            this.colItemComprado.HeaderText = "Comprado";
            this.colItemComprado.Name = "Comprado";
            //
            // FormListaDeCompras
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 486);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.groupBoxItem);
            this.Controls.Add(this.lblTituloListaDeCompras);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormListaDeCompras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormShoppingList.Title";
            this.Text = "Lista de compras";
            this.groupBoxItem.ResumeLayout(false);
            this.groupBoxItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloListaDeCompras;
        private System.Windows.Forms.GroupBox groupBoxItem;
        private System.Windows.Forms.Button btnEliminarItem;
        private System.Windows.Forms.Button btnAgregarItem;
        private System.Windows.Forms.TextBox txtbCantidadItem;
        private System.Windows.Forms.Label lblCantidadItem;
        private System.Windows.Forms.TextBox txtbDescripcionItem;
        private System.Windows.Forms.Label lblDescripcionItem;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCantidad;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colItemComprado;
    }
}
