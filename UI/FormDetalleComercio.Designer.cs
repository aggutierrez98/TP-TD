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
            this.lblTituloDetalleComercio = new Krypton.Toolkit.KryptonLabel();
            this.pnlComercioDetalle = new System.Windows.Forms.FlowLayoutPanel();
            this.lblComercioPrefijo = new Krypton.Toolkit.KryptonLabel();
            this.lblComercioNombreDetalle = new Krypton.Toolkit.KryptonLabel();
            this.groupBoxProducto = new Krypton.Toolkit.KryptonGroupBox();
            this.btnQuitarProducto = new Krypton.Toolkit.KryptonButton();
            this.btnActualizarProducto = new Krypton.Toolkit.KryptonButton();
            this.btnAgregarProducto = new Krypton.Toolkit.KryptonButton();
            this.cmbMonedaProducto = new Krypton.Toolkit.KryptonComboBox();
            this.lblMonedaProducto = new Krypton.Toolkit.KryptonLabel();
            this.txtbPrecioProducto = new Krypton.Toolkit.KryptonTextBox();
            this.lblPrecioProducto = new Krypton.Toolkit.KryptonLabel();
            this.cmbProductoCatalogo = new Krypton.Toolkit.KryptonComboBox();
            this.lblProductoCatalogo = new Krypton.Toolkit.KryptonLabel();
            this.dgvProductos = new Krypton.Toolkit.KryptonDataGridView();
            this.colProductoNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductoCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductoPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductoMoneda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxPromociones = new Krypton.Toolkit.KryptonGroupBox();
            this.dgvPromociones = new Krypton.Toolkit.KryptonDataGridView();
            this.colPromocionDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionDescuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionTopeDescuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionDiasSemana = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionVigenciaDesde = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPromocionVigenciaHasta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlComercioDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProducto.Panel)).BeginInit();
            this.groupBoxProducto.Panel.SuspendLayout();
            this.groupBoxProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPromociones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPromociones.Panel)).BeginInit();
            this.groupBoxPromociones.Panel.SuspendLayout();
            this.groupBoxPromociones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromociones)).BeginInit();
            this.tlpCamposProducto = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCamposProducto.SuspendLayout();
            this.tlpZonasDetalleComercio = new System.Windows.Forms.TableLayoutPanel();
            this.tlpZonasDetalleComercio.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTituloDetalleComercio
            //
            this.lblTituloDetalleComercio.AutoSize = true;
            this.lblTituloDetalleComercio.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTituloDetalleComercio.Location = new System.Drawing.Point(18, 15);
            this.lblTituloDetalleComercio.Name = "lblTituloDetalleComercio";
            this.lblTituloDetalleComercio.Size = new System.Drawing.Size(260, 25);
            this.lblTituloDetalleComercio.TabIndex = 0;
            this.lblTituloDetalleComercio.Tag = "FormMerchantDetail.TitleLabel";
            this.lblTituloDetalleComercio.Values.Text = "Detalle del comercio";
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
            this.lblComercioPrefijo.Values.Text = "Comercio:";
            //
            // lblComercioNombreDetalle
            //
            this.lblComercioNombreDetalle.AutoSize = true;
            this.lblComercioNombreDetalle.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblComercioNombreDetalle.Name = "lblComercioNombreDetalle";
            this.lblComercioNombreDetalle.Size = new System.Drawing.Size(10, 13);
            this.lblComercioNombreDetalle.TabIndex = 1;
            this.lblComercioNombreDetalle.Values.Text = "-";
            //
            // groupBoxProducto
            //
            this.groupBoxProducto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProducto.Panel.Controls.Add(this.btnQuitarProducto);
            this.groupBoxProducto.Panel.Controls.Add(this.btnActualizarProducto);
            this.groupBoxProducto.Panel.Controls.Add(this.btnAgregarProducto);
            this.groupBoxProducto.Panel.Controls.Add(this.tlpCamposProducto);
            this.groupBoxProducto.Location = new System.Drawing.Point(0, 0);
            this.groupBoxProducto.Name = "groupBoxProducto";
            this.groupBoxProducto.Size = new System.Drawing.Size(594, 150);
            this.groupBoxProducto.TabIndex = 2;
            this.groupBoxProducto.TabStop = false;
            this.groupBoxProducto.Tag = "FormMerchantDetail.ProductoGroup";
            this.groupBoxProducto.Values.Heading = "Productos";
            this.groupBoxProducto.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // btnQuitarProducto
            //
            this.btnQuitarProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnQuitarProducto.Location = new System.Drawing.Point(254, 92);
            this.btnQuitarProducto.Name = "btnQuitarProducto";
            this.btnQuitarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnQuitarProducto.TabIndex = 10;
            this.btnQuitarProducto.Tag = "FormMerchantDetail.RemoveProductoButton";
            this.btnQuitarProducto.Values.Text = "Quitar";
            this.btnQuitarProducto.Click += new System.EventHandler(this.btnQuitarProducto_Click);
            //
            // btnActualizarProducto
            //
            this.btnActualizarProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnActualizarProducto.Location = new System.Drawing.Point(134, 92);
            this.btnActualizarProducto.Name = "btnActualizarProducto";
            this.btnActualizarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnActualizarProducto.TabIndex = 9;
            this.btnActualizarProducto.Tag = "FormMerchantDetail.UpdateProductoButton";
            this.btnActualizarProducto.Values.Text = "Actualizar";
            this.btnActualizarProducto.Click += new System.EventHandler(this.btnActualizarProducto_Click);
            //
            // btnAgregarProducto
            //
            this.btnAgregarProducto.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnAgregarProducto.Location = new System.Drawing.Point(14, 92);
            this.btnAgregarProducto.Name = "btnAgregarProducto";
            this.btnAgregarProducto.Size = new System.Drawing.Size(110, 30);
            this.btnAgregarProducto.TabIndex = 8;
            this.btnAgregarProducto.Tag = "FormMerchantDetail.AddProductoButton";
            this.btnAgregarProducto.Values.Text = "Agregar";
            this.btnAgregarProducto.Click += new System.EventHandler(this.btnAgregarProducto_Click);
            //
            // cmbMonedaProducto
            //
            this.cmbMonedaProducto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMonedaProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonedaProducto.FormattingEnabled = true;
            this.cmbMonedaProducto.Location = new System.Drawing.Point(383, 20);
            this.cmbMonedaProducto.Name = "cmbMonedaProducto";
            this.cmbMonedaProducto.Size = new System.Drawing.Size(140, 21);
            this.cmbMonedaProducto.TabIndex = 7;
            this.cmbMonedaProducto.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            //
            // lblMonedaProducto
            //
            this.lblMonedaProducto.AutoSize = true;
            this.lblMonedaProducto.Location = new System.Drawing.Point(380, 0);
            this.lblMonedaProducto.Name = "lblMonedaProducto";
            this.lblMonedaProducto.Size = new System.Drawing.Size(49, 13);
            this.lblMonedaProducto.TabIndex = 6;
            this.lblMonedaProducto.Tag = "FormMerchantDetail.MonedaComboLabel";
            this.lblMonedaProducto.Values.Text = "Moneda:";
            this.lblMonedaProducto.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // txtbPrecioProducto
            //
            this.txtbPrecioProducto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbPrecioProducto.Location = new System.Drawing.Point(293, 20);
            this.txtbPrecioProducto.Name = "txtbPrecioProducto";
            this.txtbPrecioProducto.Size = new System.Drawing.Size(80, 20);
            this.txtbPrecioProducto.TabIndex = 5;
            this.txtbPrecioProducto.Margin = new System.Windows.Forms.Padding(3, 0, 7, 0);
            //
            // lblPrecioProducto
            //
            this.lblPrecioProducto.AutoSize = true;
            this.lblPrecioProducto.Location = new System.Drawing.Point(290, 0);
            this.lblPrecioProducto.Name = "lblPrecioProducto";
            this.lblPrecioProducto.Size = new System.Drawing.Size(41, 13);
            this.lblPrecioProducto.TabIndex = 4;
            this.lblPrecioProducto.Tag = "FormMerchantDetail.PrecioLabel";
            this.lblPrecioProducto.Values.Text = "Precio:";
            this.lblPrecioProducto.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // cmbProductoCatalogo
            //
            this.cmbProductoCatalogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProductoCatalogo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductoCatalogo.FormattingEnabled = true;
            this.cmbProductoCatalogo.Location = new System.Drawing.Point(3, 20);
            this.cmbProductoCatalogo.Name = "cmbProductoCatalogo";
            this.cmbProductoCatalogo.Size = new System.Drawing.Size(180, 21);
            this.cmbProductoCatalogo.TabIndex = 1;
            this.cmbProductoCatalogo.Margin = new System.Windows.Forms.Padding(3, 0, 7, 0);
            //
            // lblProductoCatalogo
            //
            this.lblProductoCatalogo.AutoSize = true;
            this.lblProductoCatalogo.Location = new System.Drawing.Point(0, 0);
            this.lblProductoCatalogo.Name = "lblProductoCatalogo";
            this.lblProductoCatalogo.Size = new System.Drawing.Size(53, 13);
            this.lblProductoCatalogo.TabIndex = 0;
            this.lblProductoCatalogo.Tag = "FormMerchantDetail.ProductoComboLabel";
            this.lblProductoCatalogo.Values.Text = "Producto:";
            this.lblProductoCatalogo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            //
            // dgvProductos
            //
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.AllowUserToDeleteRows = false;
            this.dgvProductos.AutoGenerateColumns = false;
            this.dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProductoNombre,
            this.colProductoCategoria,
            this.colProductoPrecio,
            this.colProductoMoneda});
            this.dgvProductos.Location = new System.Drawing.Point(0, 160);
            this.dgvProductos.MultiSelect = false;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.RowHeadersVisible = false;
            this.dgvProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductos.Size = new System.Drawing.Size(594, 150);
            this.dgvProductos.TabIndex = 3;
            this.dgvProductos.Tag = "FormMerchantDetail.ProductoGrid";
            this.dgvProductos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProductos_CellClick);
            this.dgvProductos.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
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
            // colProductoMoneda
            //
            this.colProductoMoneda.DataPropertyName = "Moneda";
            this.colProductoMoneda.HeaderText = "Moneda";
            this.colProductoMoneda.Name = "Moneda";
            this.colProductoMoneda.ReadOnly = true;
            //
            // groupBoxPromociones
            //
            this.groupBoxPromociones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPromociones.Panel.Controls.Add(this.dgvPromociones);
            this.groupBoxPromociones.Location = new System.Drawing.Point(0, 320);
            this.groupBoxPromociones.Name = "groupBoxPromociones";
            this.groupBoxPromociones.Size = new System.Drawing.Size(594, 190);
            this.groupBoxPromociones.TabIndex = 6;
            this.groupBoxPromociones.TabStop = false;
            this.groupBoxPromociones.Tag = "FormMerchantDetail.PromocionGroup";
            this.groupBoxPromociones.Values.Heading = "Promociones del comercio";
            this.groupBoxPromociones.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            //
            // dgvPromociones
            //
            this.dgvPromociones.AllowUserToAddRows = false;
            this.dgvPromociones.AllowUserToDeleteRows = false;
            this.dgvPromociones.AutoGenerateColumns = false;
            this.dgvPromociones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPromociones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPromociones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPromociones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPromocionDescripcion,
            this.colPromocionDescuento,
            this.colPromocionTopeDescuento,
            this.colPromocionDiasSemana,
            this.colPromocionVigenciaDesde,
            this.colPromocionVigenciaHasta});
            this.dgvPromociones.Location = new System.Drawing.Point(10, 4);
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
            // colPromocionTopeDescuento
            //
            this.colPromocionTopeDescuento.DataPropertyName = "TopeDescuento";
            this.colPromocionTopeDescuento.HeaderText = "Tope";
            this.colPromocionTopeDescuento.Name = "TopeDescuento";
            this.colPromocionTopeDescuento.ReadOnly = true;
            //
            // colPromocionDiasSemana
            //
            this.colPromocionDiasSemana.DataPropertyName = "DiasSemana";
            this.colPromocionDiasSemana.HeaderText = "Dias";
            this.colPromocionDiasSemana.Name = "DiasSemana";
            this.colPromocionDiasSemana.ReadOnly = true;
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
            // tlpCamposProducto
            // 
            this.tlpCamposProducto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCamposProducto.Controls.Add(this.cmbMonedaProducto, 2, 1);
            this.tlpCamposProducto.Controls.Add(this.lblMonedaProducto, 2, 0);
            this.tlpCamposProducto.Controls.Add(this.txtbPrecioProducto, 1, 1);
            this.tlpCamposProducto.Controls.Add(this.lblPrecioProducto, 1, 0);
            this.tlpCamposProducto.Controls.Add(this.cmbProductoCatalogo, 0, 1);
            this.tlpCamposProducto.Controls.Add(this.lblProductoCatalogo, 0, 0);
            this.tlpCamposProducto.Location = new System.Drawing.Point(11, 8);
            this.tlpCamposProducto.Name = "tlpCamposProducto";
            this.tlpCamposProducto.Size = new System.Drawing.Size(523, 45);
            this.tlpCamposProducto.ColumnCount = 3;
            this.tlpCamposProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpCamposProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCamposProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpCamposProducto.RowCount = 2;
            this.tlpCamposProducto.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCamposProducto.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpCamposProducto.TabIndex = 0;
            // 
            // tlpZonasDetalleComercio
            // 
            this.tlpZonasDetalleComercio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpZonasDetalleComercio.Controls.Add(this.groupBoxPromociones, 0, 2);
            this.tlpZonasDetalleComercio.Controls.Add(this.dgvProductos, 0, 1);
            this.tlpZonasDetalleComercio.Controls.Add(this.groupBoxProducto, 0, 0);
            this.tlpZonasDetalleComercio.Location = new System.Drawing.Point(23, 75);
            this.tlpZonasDetalleComercio.Name = "tlpZonasDetalleComercio";
            this.tlpZonasDetalleComercio.Size = new System.Drawing.Size(594, 514);
            this.tlpZonasDetalleComercio.ColumnCount = 1;
            this.tlpZonasDetalleComercio.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpZonasDetalleComercio.RowCount = 3;
            this.tlpZonasDetalleComercio.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tlpZonasDetalleComercio.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpZonasDetalleComercio.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tlpZonasDetalleComercio.TabIndex = 2;
            //
            // FormDetalleComercio
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 620);
            this.Controls.Add(this.tlpZonasDetalleComercio);
            this.Controls.Add(this.pnlComercioDetalle);
            this.Controls.Add(this.lblTituloDetalleComercio);
            this.Name = "FormDetalleComercio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "FormMerchantDetail.Title";
            this.Text = "Detalle del comercio";
            this.pnlComercioDetalle.ResumeLayout(false);
            this.pnlComercioDetalle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProducto.Panel)).EndInit();
            this.groupBoxProducto.Panel.ResumeLayout(false);
            this.groupBoxProducto.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxProducto)).EndInit();
            this.groupBoxProducto.ResumeLayout(false);
            this.groupBoxProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPromociones.Panel)).EndInit();
            this.groupBoxPromociones.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBoxPromociones)).EndInit();
            this.groupBoxPromociones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPromociones)).EndInit();
            this.tlpCamposProducto.ResumeLayout(false);
            this.tlpCamposProducto.PerformLayout();
            this.tlpZonasDetalleComercio.ResumeLayout(false);
            this.tlpZonasDetalleComercio.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblTituloDetalleComercio;
        private System.Windows.Forms.FlowLayoutPanel pnlComercioDetalle;
        private Krypton.Toolkit.KryptonLabel lblComercioPrefijo;
        private Krypton.Toolkit.KryptonLabel lblComercioNombreDetalle;
        private Krypton.Toolkit.KryptonGroupBox groupBoxProducto;
        private Krypton.Toolkit.KryptonButton btnQuitarProducto;
        private Krypton.Toolkit.KryptonButton btnActualizarProducto;
        private Krypton.Toolkit.KryptonButton btnAgregarProducto;
        private Krypton.Toolkit.KryptonComboBox cmbMonedaProducto;
        private Krypton.Toolkit.KryptonLabel lblMonedaProducto;
        private Krypton.Toolkit.KryptonTextBox txtbPrecioProducto;
        private Krypton.Toolkit.KryptonLabel lblPrecioProducto;
        private Krypton.Toolkit.KryptonComboBox cmbProductoCatalogo;
        private Krypton.Toolkit.KryptonLabel lblProductoCatalogo;
        private Krypton.Toolkit.KryptonDataGridView dgvProductos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoPrecio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductoMoneda;
        private Krypton.Toolkit.KryptonGroupBox groupBoxPromociones;
        private Krypton.Toolkit.KryptonDataGridView dgvPromociones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDescuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionTopeDescuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionDiasSemana;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionVigenciaDesde;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPromocionVigenciaHasta;
        private System.Windows.Forms.TableLayoutPanel tlpCamposProducto;
        private System.Windows.Forms.TableLayoutPanel tlpZonasDetalleComercio;
    }
}
