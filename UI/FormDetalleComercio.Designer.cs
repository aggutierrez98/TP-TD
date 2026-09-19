namespace TpIDS
{
    partial class FormDetalleComercio
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
            this.lblTituloDetalleComercio = new System.Windows.Forms.Label();
            this.pnlComercioDetalle = new System.Windows.Forms.FlowLayoutPanel();
            this.lblComercioPrefijo = new System.Windows.Forms.Label();
            this.lblComercioNombreDetalle = new System.Windows.Forms.Label();
            this.groupBoxProducto = new System.Windows.Forms.GroupBox();
            this.btnEliminarProducto = new System.Windows.Forms.Button();
            this.btnActualizarProducto = new System.Windows.Forms.Button();
            this.btnAgregarProducto = new System.Windows.Forms.Button();
            this.cmbMonedaProducto = new System.Windows.Forms.ComboBox();
            this.lblMonedaProducto = new System.Windows.Forms.Label();
            this.txtbPrecioProducto = new System.Windows.Forms.TextBox();
            this.lblPrecioProducto = new System.Windows.Forms.Label();
            this.cmbCategoriaProducto = new System.Windows.Forms.ComboBox();
            this.lblCategoriaProducto = new System.Windows.Forms.Label();
            this.txtbNombreProducto = new System.Windows.Forms.TextBox();
            this.lblNombreProducto = new System.Windows.Forms.Label();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.colProductoNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductoCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductoPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxCategoriaProducto = new System.Windows.Forms.GroupBox();
            this.btnEliminarCategoriaProducto = new System.Windows.Forms.Button();
            this.btnActualizarCategoriaProducto = new System.Windows.Forms.Button();
            this.btnAgregarCategoriaProducto = new System.Windows.Forms.Button();
            this.txtbNombreCategoriaProducto = new System.Windows.Forms.TextBox();
            this.lblNombreCategoriaProducto = new System.Windows.Forms.Label();
            this.dgvCategoriasProducto = new System.Windows.Forms.DataGridView();
            this.colCategoriaProductoNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxPromociones = new System.Windows.Forms.GroupBox();
            this.dgvPromociones = new System.Windows.Forms.DataGridView();
            this.colPromocionDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionDescuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionVigenciaDesde = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionVigenciaHasta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlComercioDetalle.SuspendLayout();
            this.groupBoxProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.groupBoxCategoriaProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategoriasProducto)).BeginInit();
            this.groupBoxPromociones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromociones)).BeginInit();
            this.SuspendLayout();
            //
            // lblTituloDetalleComercio
            //
            this.lblTituloDetalleComercio.AutoSize = true;
            this.lblTituloDetalleComercio.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloDetalleComercio.Location = new System.Drawing.Point(18, 15);
            this.lblTituloDetalleComercio.Name = "lblTituloDetalleComercio";
            this.lblTituloDetalleComercio.Size = new System.Drawing.Size(260, 25);
            this.lblTituloDetalleComercio.TabIndex = 0;
            this.lblTituloDetalleComercio.Tag = "FormMerchantDetail.TitleLabel";
            this.lblTituloDetalleComercio.Text = "Detalle del comercio";
            //
            // pnlComercioDetalle
            //
            this.pnlComercioDetalle.AutoSize = true;
            this.pnlComercioDetalle.Controls.Add(this.lblComercioPrefijo);
            this.pnlComercioDetalle.Controls.Add(this.lblComercioNombreDetalle);
            this.pnlComercioDetalle.Location = new System.Drawing.Point(20, 45);
            this.pnlComercioDetalle.Name = "pnlComercioDetalle";
            this.pnlComercioDetalle.Size = new System.Drawing.Size(60, 13);
            this.pnlComercioDetalle.TabIndex = 1;
            this.pnlComercioDetalle.WrapContents = false;
            //
            // lblComercioPrefijo
            //
            this.lblComercioPrefijo.AutoSize = true;
            this.lblComercioPrefijo.Margin = new System.Windows.Forms.Padding(0);
            this.lblComercioPrefijo.Name = "lblComercioPrefijo";
            this.lblComercioPrefijo.Size = new System.Drawing.Size(59, 13);
            this.lblComercioPrefijo.TabIndex = 0;
            this.lblComercioPrefijo.Tag = "FormMerchantDetail.ComercioPrefix";
            this.lblComercioPrefijo.Text = "Comercio:";
            //
            // lblComercioNombreDetalle
            //
            this.lblComercioNombreDetalle.AutoSize = true;
            this.lblComercioNombreDetalle.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblComercioNombreDetalle.Name = "lblComercioNombreDetalle";
            this.lblComercioNombreDetalle.Size = new System.Drawing.Size(10, 13);
            this.lblComercioNombreDetalle.TabIndex = 1;
            this.lblComercioNombreDetalle.Text = "-";
            //
            // groupBoxProducto
            //
            this.groupBoxProducto.Controls.Add(this.btnEliminarProducto);
            this.groupBoxProducto.Controls.Add(this.btnActualizarProducto);
            this.groupBoxProducto.Controls.Add(this.btnAgregarProducto);
            this.groupBoxProducto.Controls.Add(this.cmbMonedaProducto);
            this.groupBoxProducto.Controls.Add(this.lblMonedaProducto);
            this.groupBoxProducto.Controls.Add(this.txtbPrecioProducto);
            this.groupBoxProducto.Controls.Add(this.lblPrecioProducto);
            this.groupBoxProducto.Controls.Add(this.cmbCategoriaProducto);
            this.groupBoxProducto.Controls.Add(this.lblCategoriaProducto);
            this.groupBoxProducto.Controls.Add(this.txtbNombreProducto);
            this.groupBoxProducto.Controls.Add(this.lblNombreProducto);
            this.groupBoxProducto.Location = new System.Drawing.Point(23, 75);
            this.groupBoxProducto.Name = "groupBoxProducto";
            this.groupBoxProducto.Size = new System.Drawing.Size(594, 150);
            this.groupBoxProducto.TabIndex = 2;
            this.groupBoxProducto.TabStop = false;
            this.groupBoxProducto.Tag = "FormMerchantDetail.ProductoGroup";
            this.groupBoxProducto.Text = "Productos";
            //
            // btnEliminarProducto
            //
            this.btnEliminarProducto.Location = new System.Drawing.Point(254, 108);
            this.btnEliminarProducto.Name = "btnEliminarProducto";
            this.btnEliminarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarProducto.TabIndex = 10;
            this.btnEliminarProducto.Tag = "FormMerchantDetail.DeleteProductoButton";
            this.btnEliminarProducto.Text = "Eliminar";
            this.btnEliminarProducto.UseVisualStyleBackColor = true;
            this.btnEliminarProducto.Click += new System.EventHandler(this.btnEliminarProducto_Click);
            //
            // btnActualizarProducto
            //
            this.btnActualizarProducto.Location = new System.Drawing.Point(134, 108);
            this.btnActualizarProducto.Name = "btnActualizarProducto";
            this.btnActualizarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarProducto.TabIndex = 9;
            this.btnActualizarProducto.Tag = "FormMerchantDetail.UpdateProductoButton";
            this.btnActualizarProducto.Text = "Actualizar";
            this.btnActualizarProducto.UseVisualStyleBackColor = true;
            this.btnActualizarProducto.Click += new System.EventHandler(this.btnActualizarProducto_Click);
            //
            // btnAgregarProducto
            //
            this.btnAgregarProducto.Location = new System.Drawing.Point(14, 108);
            this.btnAgregarProducto.Name = "btnAgregarProducto";
            this.btnAgregarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarProducto.TabIndex = 8;
            this.btnAgregarProducto.Tag = "FormMerchantDetail.AddProductoButton";
            this.btnAgregarProducto.Text = "Agregar";
            this.btnAgregarProducto.UseVisualStyleBackColor = true;
            this.btnAgregarProducto.Click += new System.EventHandler(this.btnAgregarProducto_Click);
            //
            // cmbMonedaProducto
            //
            this.cmbMonedaProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonedaProducto.FormattingEnabled = true;
            this.cmbMonedaProducto.Location = new System.Drawing.Point(394, 44);
            this.cmbMonedaProducto.Name = "cmbMonedaProducto";
            this.cmbMonedaProducto.Size = new System.Drawing.Size(140, 21);
            this.cmbMonedaProducto.TabIndex = 7;
            //
            // lblMonedaProducto
            //
            this.lblMonedaProducto.AutoSize = true;
            this.lblMonedaProducto.Location = new System.Drawing.Point(391, 28);
            this.lblMonedaProducto.Name = "lblMonedaProducto";
            this.lblMonedaProducto.Size = new System.Drawing.Size(49, 13);
            this.lblMonedaProducto.TabIndex = 6;
            this.lblMonedaProducto.Tag = "FormMerchantDetail.MonedaComboLabel";
            this.lblMonedaProducto.Text = "Moneda:";
            //
            // txtbPrecioProducto
            //
            this.txtbPrecioProducto.Location = new System.Drawing.Point(304, 44);
            this.txtbPrecioProducto.Name = "txtbPrecioProducto";
            this.txtbPrecioProducto.Size = new System.Drawing.Size(80, 20);
            this.txtbPrecioProducto.TabIndex = 5;
            //
            // lblPrecioProducto
            //
            this.lblPrecioProducto.AutoSize = true;
            this.lblPrecioProducto.Location = new System.Drawing.Point(301, 28);
            this.lblPrecioProducto.Name = "lblPrecioProducto";
            this.lblPrecioProducto.Size = new System.Drawing.Size(41, 13);
            this.lblPrecioProducto.TabIndex = 4;
            this.lblPrecioProducto.Tag = "FormMerchantDetail.PrecioLabel";
            this.lblPrecioProducto.Text = "Precio:";
            //
            // cmbCategoriaProducto
            //
            this.cmbCategoriaProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoriaProducto.FormattingEnabled = true;
            this.cmbCategoriaProducto.Location = new System.Drawing.Point(154, 44);
            this.cmbCategoriaProducto.Name = "cmbCategoriaProducto";
            this.cmbCategoriaProducto.Size = new System.Drawing.Size(140, 21);
            this.cmbCategoriaProducto.TabIndex = 3;
            //
            // lblCategoriaProducto
            //
            this.lblCategoriaProducto.AutoSize = true;
            this.lblCategoriaProducto.Location = new System.Drawing.Point(151, 28);
            this.lblCategoriaProducto.Name = "lblCategoriaProducto";
            this.lblCategoriaProducto.Size = new System.Drawing.Size(56, 13);
            this.lblCategoriaProducto.TabIndex = 2;
            this.lblCategoriaProducto.Tag = "FormMerchantDetail.CategoriaComboLabel";
            this.lblCategoriaProducto.Text = "Categoria:";
            //
            // txtbNombreProducto
            //
            this.txtbNombreProducto.Location = new System.Drawing.Point(14, 44);
            this.txtbNombreProducto.Name = "txtbNombreProducto";
            this.txtbNombreProducto.Size = new System.Drawing.Size(130, 20);
            this.txtbNombreProducto.TabIndex = 1;
            //
            // lblNombreProducto
            //
            this.lblNombreProducto.AutoSize = true;
            this.lblNombreProducto.Location = new System.Drawing.Point(11, 28);
            this.lblNombreProducto.Name = "lblNombreProducto";
            this.lblNombreProducto.Size = new System.Drawing.Size(53, 13);
            this.lblNombreProducto.TabIndex = 0;
            this.lblNombreProducto.Tag = "FormMerchantDetail.NombreProductoLabel";
            this.lblNombreProducto.Text = "Nombre:";
            //
            // dgvProductos
            //
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.AllowUserToDeleteRows = false;
            this.dgvProductos.AutoGenerateColumns = false;
            this.dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProductoNombre,
            this.colProductoCategoria,
            this.colProductoPrecio});
            this.dgvProductos.Location = new System.Drawing.Point(23, 235);
            this.dgvProductos.MultiSelect = false;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.RowHeadersVisible = false;
            this.dgvProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductos.Size = new System.Drawing.Size(594, 150);
            this.dgvProductos.TabIndex = 3;
            this.dgvProductos.Tag = "FormMerchantDetail.ProductoGrid";
            this.dgvProductos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProductos_CellClick);
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
            // colProductoPrecio
            //
            this.colProductoPrecio.DataPropertyName = "Precio";
            this.colProductoPrecio.HeaderText = "Precio";
            this.colProductoPrecio.Name = "Precio";
            this.colProductoPrecio.ReadOnly = true;
            //
            // groupBoxCategoriaProducto
            //
            this.groupBoxCategoriaProducto.Controls.Add(this.btnEliminarCategoriaProducto);
            this.groupBoxCategoriaProducto.Controls.Add(this.btnActualizarCategoriaProducto);
            this.groupBoxCategoriaProducto.Controls.Add(this.btnAgregarCategoriaProducto);
            this.groupBoxCategoriaProducto.Controls.Add(this.txtbNombreCategoriaProducto);
            this.groupBoxCategoriaProducto.Controls.Add(this.lblNombreCategoriaProducto);
            this.groupBoxCategoriaProducto.Location = new System.Drawing.Point(23, 395);
            this.groupBoxCategoriaProducto.Name = "groupBoxCategoriaProducto";
            this.groupBoxCategoriaProducto.Size = new System.Drawing.Size(594, 110);
            this.groupBoxCategoriaProducto.TabIndex = 4;
            this.groupBoxCategoriaProducto.TabStop = false;
            this.groupBoxCategoriaProducto.Tag = "FormMerchantDetail.CategoriaGroup";
            this.groupBoxCategoriaProducto.Text = "Categorias";
            //
            // btnEliminarCategoriaProducto
            //
            this.btnEliminarCategoriaProducto.Location = new System.Drawing.Point(254, 68);
            this.btnEliminarCategoriaProducto.Name = "btnEliminarCategoriaProducto";
            this.btnEliminarCategoriaProducto.Size = new System.Drawing.Size(110, 30);
            this.btnEliminarCategoriaProducto.TabIndex = 4;
            this.btnEliminarCategoriaProducto.Tag = "FormMerchantDetail.DeleteCategoriaButton";
            this.btnEliminarCategoriaProducto.Text = "Eliminar";
            this.btnEliminarCategoriaProducto.UseVisualStyleBackColor = true;
            this.btnEliminarCategoriaProducto.Click += new System.EventHandler(this.btnEliminarCategoriaProducto_Click);
            //
            // btnActualizarCategoriaProducto
            //
            this.btnActualizarCategoriaProducto.Location = new System.Drawing.Point(134, 68);
            this.btnActualizarCategoriaProducto.Name = "btnActualizarCategoriaProducto";
            this.btnActualizarCategoriaProducto.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarCategoriaProducto.TabIndex = 3;
            this.btnActualizarCategoriaProducto.Tag = "FormMerchantDetail.UpdateCategoriaButton";
            this.btnActualizarCategoriaProducto.Text = "Actualizar";
            this.btnActualizarCategoriaProducto.UseVisualStyleBackColor = true;
            this.btnActualizarCategoriaProducto.Click += new System.EventHandler(this.btnActualizarCategoriaProducto_Click);
            //
            // btnAgregarCategoriaProducto
            //
            this.btnAgregarCategoriaProducto.Location = new System.Drawing.Point(14, 68);
            this.btnAgregarCategoriaProducto.Name = "btnAgregarCategoriaProducto";
            this.btnAgregarCategoriaProducto.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarCategoriaProducto.TabIndex = 2;
            this.btnAgregarCategoriaProducto.Tag = "FormMerchantDetail.AddCategoriaButton";
            this.btnAgregarCategoriaProducto.Text = "Agregar";
            this.btnAgregarCategoriaProducto.UseVisualStyleBackColor = true;
            this.btnAgregarCategoriaProducto.Click += new System.EventHandler(this.btnAgregarCategoriaProducto_Click);
            //
            // txtbNombreCategoriaProducto
            //
            this.txtbNombreCategoriaProducto.Location = new System.Drawing.Point(14, 44);
            this.txtbNombreCategoriaProducto.Name = "txtbNombreCategoriaProducto";
            this.txtbNombreCategoriaProducto.Size = new System.Drawing.Size(300, 20);
            this.txtbNombreCategoriaProducto.TabIndex = 1;
            //
            // lblNombreCategoriaProducto
            //
            this.lblNombreCategoriaProducto.AutoSize = true;
            this.lblNombreCategoriaProducto.Location = new System.Drawing.Point(11, 28);
            this.lblNombreCategoriaProducto.Name = "lblNombreCategoriaProducto";
            this.lblNombreCategoriaProducto.Size = new System.Drawing.Size(53, 13);
            this.lblNombreCategoriaProducto.TabIndex = 0;
            this.lblNombreCategoriaProducto.Tag = "FormMerchantDetail.NombreCategoriaLabel";
            this.lblNombreCategoriaProducto.Text = "Nombre:";
            //
            // dgvCategoriasProducto
            //
            this.dgvCategoriasProducto.AllowUserToAddRows = false;
            this.dgvCategoriasProducto.AllowUserToDeleteRows = false;
            this.dgvCategoriasProducto.AutoGenerateColumns = false;
            this.dgvCategoriasProducto.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCategoriasProducto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCategoriasProducto.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCategoriaProductoNombre});
            this.dgvCategoriasProducto.Location = new System.Drawing.Point(23, 515);
            this.dgvCategoriasProducto.MultiSelect = false;
            this.dgvCategoriasProducto.Name = "dgvCategoriasProducto";
            this.dgvCategoriasProducto.RowHeadersVisible = false;
            this.dgvCategoriasProducto.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCategoriasProducto.Size = new System.Drawing.Size(594, 130);
            this.dgvCategoriasProducto.TabIndex = 5;
            this.dgvCategoriasProducto.Tag = "FormMerchantDetail.CategoriaGrid";
            this.dgvCategoriasProducto.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCategoriasProducto_CellClick);
            //
            // colCategoriaProductoNombre
            //
            this.colCategoriaProductoNombre.DataPropertyName = "Nombre";
            this.colCategoriaProductoNombre.HeaderText = "Nombre";
            this.colCategoriaProductoNombre.Name = "Nombre";
            this.colCategoriaProductoNombre.ReadOnly = true;
            //
            // groupBoxPromociones
            //
            this.groupBoxPromociones.Controls.Add(this.dgvPromociones);
            this.groupBoxPromociones.Location = new System.Drawing.Point(23, 655);
            this.groupBoxPromociones.Name = "groupBoxPromociones";
            this.groupBoxPromociones.Size = new System.Drawing.Size(594, 190);
            this.groupBoxPromociones.TabIndex = 6;
            this.groupBoxPromociones.TabStop = false;
            this.groupBoxPromociones.Tag = "FormMerchantDetail.PromocionGroup";
            this.groupBoxPromociones.Text = "Promociones del comercio";
            //
            // dgvPromociones
            //
            this.dgvPromociones.AllowUserToAddRows = false;
            this.dgvPromociones.AllowUserToDeleteRows = false;
            this.dgvPromociones.AutoGenerateColumns = false;
            this.dgvPromociones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPromociones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPromociones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPromocionDescripcion,
            this.colPromocionDescuento,
            this.colPromocionVigenciaDesde,
            this.colPromocionVigenciaHasta});
            this.dgvPromociones.Location = new System.Drawing.Point(10, 20);
            this.dgvPromociones.MultiSelect = false;
            this.dgvPromociones.Name = "dgvPromociones";
            this.dgvPromociones.ReadOnly = true;
            this.dgvPromociones.RowHeadersVisible = false;
            this.dgvPromociones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPromociones.Size = new System.Drawing.Size(574, 160);
            this.dgvPromociones.TabIndex = 0;
            this.dgvPromociones.Tag = "FormMerchantDetail.PromocionGrid";
            //
            // colPromocionDescripcion
            //
            this.colPromocionDescripcion.DataPropertyName = "Descripcion";
            this.colPromocionDescripcion.HeaderText = "Descripcion";
            this.colPromocionDescripcion.Name = "Descripcion";
            this.colPromocionDescripcion.ReadOnly = true;
            //
            // colPromocionDescuento
            //
            this.colPromocionDescuento.DataPropertyName = "Descuento";
            this.colPromocionDescuento.HeaderText = "Descuento";
            this.colPromocionDescuento.Name = "Descuento";
            this.colPromocionDescuento.ReadOnly = true;
            //
            // colPromocionVigenciaDesde
            //
            this.colPromocionVigenciaDesde.DataPropertyName = "VigenciaDesde";
            this.colPromocionVigenciaDesde.HeaderText = "VigenciaDesde";
            this.colPromocionVigenciaDesde.Name = "VigenciaDesde";
            this.colPromocionVigenciaDesde.ReadOnly = true;
            //
            // colPromocionVigenciaHasta
            //
            this.colPromocionVigenciaHasta.DataPropertyName = "VigenciaHasta";
            this.colPromocionVigenciaHasta.HeaderText = "VigenciaHasta";
            this.colPromocionVigenciaHasta.Name = "VigenciaHasta";
            this.colPromocionVigenciaHasta.ReadOnly = true;
            //
            // FormDetalleComercio
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 880);
            this.Controls.Add(this.groupBoxPromociones);
            this.Controls.Add(this.dgvCategoriasProducto);
            this.Controls.Add(this.groupBoxCategoriaProducto);
            this.Controls.Add(this.dgvProductos);
            this.Controls.Add(this.groupBoxProducto);
            this.Controls.Add(this.pnlComercioDetalle);
            this.Controls.Add(this.lblTituloDetalleComercio);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormDetalleComercio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMerchantDetail.Title";
            this.Text = "Detalle del comercio";
            this.pnlComercioDetalle.ResumeLayout(false);
            this.pnlComercioDetalle.PerformLayout();
            this.groupBoxProducto.ResumeLayout(false);
            this.groupBoxProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.groupBoxCategoriaProducto.ResumeLayout(false);
            this.groupBoxCategoriaProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategoriasProducto)).EndInit();
            this.groupBoxPromociones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromociones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTituloDetalleComercio;
        private System.Windows.Forms.FlowLayoutPanel pnlComercioDetalle;
        private System.Windows.Forms.Label lblComercioPrefijo;
        private System.Windows.Forms.Label lblComercioNombreDetalle;
        private System.Windows.Forms.GroupBox groupBoxProducto;
        private System.Windows.Forms.Button btnEliminarProducto;
        private System.Windows.Forms.Button btnActualizarProducto;
        private System.Windows.Forms.Button btnAgregarProducto;
        private System.Windows.Forms.ComboBox cmbMonedaProducto;
        private System.Windows.Forms.Label lblMonedaProducto;
        private System.Windows.Forms.TextBox txtbPrecioProducto;
        private System.Windows.Forms.Label lblPrecioProducto;
        private System.Windows.Forms.ComboBox cmbCategoriaProducto;
        private System.Windows.Forms.Label lblCategoriaProducto;
        private System.Windows.Forms.TextBox txtbNombreProducto;
        private System.Windows.Forms.Label lblNombreProducto;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoPrecio;
        private System.Windows.Forms.GroupBox groupBoxCategoriaProducto;
        private System.Windows.Forms.Button btnEliminarCategoriaProducto;
        private System.Windows.Forms.Button btnActualizarCategoriaProducto;
        private System.Windows.Forms.Button btnAgregarCategoriaProducto;
        private System.Windows.Forms.TextBox txtbNombreCategoriaProducto;
        private System.Windows.Forms.Label lblNombreCategoriaProducto;
        private System.Windows.Forms.DataGridView dgvCategoriasProducto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategoriaProductoNombre;
        private System.Windows.Forms.GroupBox groupBoxPromociones;
        private System.Windows.Forms.DataGridView dgvPromociones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDescuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionVigenciaDesde;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionVigenciaHasta;
    }
}
