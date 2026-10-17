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
            this.lblTituloListaDeCompras = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxItem = new Krypton.Toolkit.KryptonGroupBox();
            this.btnEliminarItem = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarItem = new Krypton.Toolkit.KryptonButton();
            this.txtbCantidadItem = new Krypton.Toolkit.KryptonTextBox();
            this.lblCantidadItem = new Krypton.Toolkit.KryptonLabel();
            this.txtbDescripcionItem = new Krypton.Toolkit.KryptonTextBox();
            this.lblDescripcionItem = new Krypton.Toolkit.KryptonLabel();
            this.dgvItems = new Krypton.Toolkit.KryptonDataGridView();
            this.colItemDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemComprado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxItem.Panel)).BeginInit();
            this.groupBoxItem.Panel.SuspendLayout();
            this.groupBoxItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.tlpCamposItem = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposItem.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloListaDeCompras
            //
            this.lblTituloListaDeCompras.AutoSize = true;
            this.lblTituloListaDeCompras.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloListaDeCompras.Location = new System.Drawing.Point(18, 15);
            this.lblTituloListaDeCompras.Name = "lblTituloListaDeCompras";
            this.lblTituloListaDeCompras.Size = new System.Drawing.Size(219, 25);
            this.lblTituloListaDeCompras.TabIndex = 0;
            this.lblTituloListaDeCompras.Tag = "FormShoppingList.TitleLabel";
            this.lblTituloListaDeCompras.Values.Text = "Lista de compras";
            //
            // groupBoxItem
            //
            this.groupBoxItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxItem.Panel.Controls.Add(this.btnEliminarItem);
            this.groupBoxItem.Panel.Controls.Add(this.btnAgregarItem);
            this.groupBoxItem.Panel.Controls.Add(this.tlpCamposItem);
            this.groupBoxItem.Location = new System.Drawing.Point(23, 52);
            this.groupBoxItem.Name = "groupBoxItem";
            this.groupBoxItem.Size = new System.Drawing.Size(454, 116);
            this.groupBoxItem.TabIndex = 1;
            this.groupBoxItem.TabStop = false;
            this.groupBoxItem.Tag = "FormShoppingList.ItemGroup";
            this.groupBoxItem.Values.Heading = "Datos del item";
            //
            // btnEliminarItem
            //
            this.btnEliminarItem.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnEliminarItem.Location = new System.Drawing.Point(294, 63);
            this.btnEliminarItem.Name = "btnEliminarItem";
            this.btnEliminarItem.Size = new System.Drawing.Size(135, 30);
            this.btnEliminarItem.TabIndex = 5;
            this.btnEliminarItem.Tag = "FormShoppingList.DeleteButton";
            this.btnEliminarItem.Values.Text = "Eliminar item";
            this.btnEliminarItem.Click += new System.EventHandler(this.btnEliminarItem_Click);
            //
            // btnAgregarItem
            //
            this.btnAgregarItem.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarItem.Location = new System.Drawing.Point(14, 63);
            this.btnAgregarItem.Name = "btnAgregarItem";
            this.btnAgregarItem.Size = new System.Drawing.Size(135, 30);
            this.btnAgregarItem.TabIndex = 4;
            this.btnAgregarItem.Tag = "FormShoppingList.AddButton";
            this.btnAgregarItem.Values.Text = "Agregar item";
            this.btnAgregarItem.Click += new System.EventHandler(this.btnAgregarItem_Click);
            //
            // txtbCantidadItem
            //
            this.txtbCantidadItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbCantidadItem.Location = new System.Drawing.Point(329, 20);
            this.txtbCantidadItem.Name = "txtbCantidadItem";
            this.txtbCantidadItem.Size = new System.Drawing.Size(89, 20);
            this.txtbCantidadItem.TabIndex = 3;
            this.txtbCantidadItem.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblCantidadItem
            //
            this.lblCantidadItem.AutoSize = true;
            this.lblCantidadItem.Location = new System.Drawing.Point(326, 0);
            this.lblCantidadItem.Name = "lblCantidadItem";
            this.lblCantidadItem.Size = new System.Drawing.Size(60, 13);
            this.lblCantidadItem.TabIndex = 2;
            this.lblCantidadItem.Tag = "FormShoppingList.QuantityLabel";
            this.lblCantidadItem.Values.Text = "Cantidad:";
            this.lblCantidadItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // txtbDescripcionItem
            //
            this.txtbDescripcionItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbDescripcionItem.Location = new System.Drawing.Point(3, 20);
            this.txtbDescripcionItem.Name = "txtbDescripcionItem";
            this.txtbDescripcionItem.Size = new System.Drawing.Size(310, 20);
            this.txtbDescripcionItem.TabIndex = 1;
            this.txtbDescripcionItem.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
            //
            // lblDescripcionItem
            //
            this.lblDescripcionItem.AutoSize = true;
            this.lblDescripcionItem.Location = new System.Drawing.Point(0, 0);
            this.lblDescripcionItem.Name = "lblDescripcionItem";
            this.lblDescripcionItem.Size = new System.Drawing.Size(115, 13);
            this.lblDescripcionItem.TabIndex = 0;
            this.lblDescripcionItem.Tag = "FormShoppingList.DescriptionLabel";
            this.lblDescripcionItem.Values.Text = "Descripcion:";
            this.lblDescripcionItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvItems
            //
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AutoGenerateColumns = false;
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            // tlpCamposItem
            // 
            this.tlpCamposItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposItem.Controls.Add(this.txtbCantidadItem, 1, 1);
            this.tlpCamposItem.Controls.Add(this.lblCantidadItem, 1, 0);
            this.tlpCamposItem.Controls.Add(this.txtbDescripcionItem, 0, 1);
            this.tlpCamposItem.Controls.Add(this.lblDescripcionItem, 0, 0);
            this.tlpCamposItem.Location = new System.Drawing.Point(11, 8);
            this.tlpCamposItem.Name = "tlpCamposItem";
            this.tlpCamposItem.Size = new System.Drawing.Size(418, 44);
            this.tlpCamposItem.ColumnCount = 2;
            this.tlpCamposItem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.99F));
            this.tlpCamposItem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.01F));
            this.tlpCamposItem.RowCount = 2;
            this.tlpCamposItem.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposItem.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpCamposItem.TabIndex = 0;
            //
            // FormListaDeCompras
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 486);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.groupBoxItem);
            this.Controls.Add(this.lblTituloListaDeCompras);
            this.Name = "FormListaDeCompras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormShoppingList.Title";
            this.Text = "Lista de compras";
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxItem.Panel)).EndInit();
            this.groupBoxItem.Panel.ResumeLayout(false);
            this.groupBoxItem.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxItem)).EndInit();
            this.groupBoxItem.ResumeLayout(false);
            this.groupBoxItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.tlpCamposItem.ResumeLayout(false);
            this.tlpCamposItem.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloListaDeCompras;
        private Krypton.Toolkit.KryptonGroupBox groupBoxItem;
        private Krypton.Toolkit.KryptonButton btnEliminarItem;
        private Krypton.Toolkit.KryptonButton btnAgregarItem;
        private Krypton.Toolkit.KryptonTextBox txtbCantidadItem;
        private Krypton.Toolkit.KryptonLabel lblCantidadItem;
        private Krypton.Toolkit.KryptonTextBox txtbDescripcionItem;
        private Krypton.Toolkit.KryptonLabel lblDescripcionItem;
        private Krypton.Toolkit.KryptonDataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCantidad;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colItemComprado;
        private System.Windows.Forms.TableLayoutPanel tlpCamposItem;
    }
}
