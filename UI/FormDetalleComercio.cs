using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Pantalla de operador (back-office) abierta en modo modal desde FormGestionComercios,
    // igual patron de constructor que FormHistorialUsuario. Tres secciones independientes
    // sobre un mismo comercio: Productos (CRUD completo), Categorias de producto (CRUD
    // completo, catalogo global no exclusivo del comercio) y Promociones del comercio
    // (solo lectura: las genera un proceso automatico fuera de esta tanda).
    public partial class FormDetalleComercio : FormTraducibleBase
    {
        private readonly int _comercioId;
        private readonly ProductoBLL _productoBLL = CompositionRoot.Producto;
        private readonly CategoriaProductoBLL _categoriaProductoBLL = CompositionRoot.CategoriaProducto;
        private readonly PromocionBLL _promocionBLL = CompositionRoot.Promocion;

        public FormDetalleComercio(int comercioId, string nombreComercio)
        {
            _comercioId = comercioId;
            InitializeComponent();

            // El nombre es dato fijo del constructor, no depende del idioma: el prefijo
            // lo traduce el helper via Tag y aca solo seteamos el dato (igual criterio que
            // FormHistorialUsuario con el username).
            lblComercioNombreDetalle.Text = string.IsNullOrWhiteSpace(nombreComercio) ? "-" : nombreComercio;

            Load += FormDetalleComercio_Load;
        }

        // View-models planos para bindear las grillas.
        private sealed class FilaProducto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public decimal Precio { get; set; }
        }

        private sealed class FilaCategoriaProducto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        private sealed class FilaPromocion
        {
            public string Descripcion { get; set; }
            public decimal Descuento { get; set; }
            public DateTime VigenciaDesde { get; set; }
            public DateTime VigenciaHasta { get; set; }
        }

        private void FormDetalleComercio_Load(object sender, EventArgs e)
        {
            CargarMonedas();
            CargarCategorias(null);
            CargarProductos(null);
            CargarPromociones();
            MostrarSeccionMonedaProducto(true);
        }

        // --- Productos ---

        private void CargarProductos(int? productoSeleccionadoId)
        {
            try
            {
                List<Producto> productos = _productoBLL.ObtenerPorComercio(_comercioId);

                dgvProductos.DataSource = null;
                dgvProductos.DataSource = productos
                    .OrderBy(item => item.Nombre)
                    .Select(ProyectarFilaProducto)
                    .ToList();

                if (productoSeleccionadoId.HasValue)
                {
                    SeleccionarFila<FilaProducto>(dgvProductos, fila => fila.Id == productoSeleccionadoId.Value);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de productos");
            }
        }

        private static FilaProducto ProyectarFilaProducto(Producto producto)
        {
            return new FilaProducto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Categoria = producto.CategoriaProducto?.Nombre ?? string.Empty,
                Precio = producto.Precio
            };
        }

        private void CargarMonedas()
        {
            try
            {
                List<Moneda> monedas = _productoBLL.ObtenerMonedas()
                    .OrderBy(item => item.Nombre)
                    .ToList();

                cmbMonedaProducto.DataSource = null;
                cmbMonedaProducto.DisplayMember = "Nombre";
                cmbMonedaProducto.ValueMember = "Id";
                cmbMonedaProducto.DataSource = monedas;
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de monedas");
            }
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (!(cmbCategoriaProducto.SelectedItem is CategoriaProducto categoriaSeleccionada))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectCategoria");
                return;
            }

            if (!(cmbMonedaProducto.SelectedItem is Moneda monedaSeleccionada))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectMoneda");
                return;
            }

            if (!decimal.TryParse(txtbPrecioProducto.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precioParseado) || precioParseado <= 0)
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.InvalidPrecio");
                return;
            }

            try
            {
                Producto creado = _productoBLL.Crear(_comercioId, txtbNombreProducto.Text, categoriaSeleccionada.Id, monedaSeleccionada.Id, precioParseado);
                LimpiarFormularioProducto();
                CargarProductos(creado.Id);
                UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.ProductoCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de producto");
            }
        }

        private void btnActualizarProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvProductos, out FilaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectProducto");
                return;
            }

            if (!(cmbCategoriaProducto.SelectedItem is CategoriaProducto categoriaSeleccionada))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectCategoria");
                return;
            }

            if (!decimal.TryParse(txtbPrecioProducto.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precioParseado) || precioParseado <= 0)
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.InvalidPrecio");
                return;
            }

            try
            {
                bool actualizado = _productoBLL.Actualizar(fila.Id, txtbNombreProducto.Text, categoriaSeleccionada.Id, precioParseado);
                if (actualizado)
                {
                    CargarProductos(fila.Id);
                    UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.ProductoUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de producto");
            }
        }

        private void btnEliminarProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvProductos, out FilaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectProducto");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMerchantDetail.Confirm.DeleteProductoMessage",
                "FormMerchantDetail.Confirm.DeleteProductoTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = _productoBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    LimpiarFormularioProducto();
                    CargarProductos(null);
                    UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.ProductoDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de producto");
            }
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(dgvProductos, out FilaProducto fila))
            {
                txtbNombreProducto.Text = fila.Nombre;
                txtbPrecioProducto.Text = fila.Precio.ToString(CultureInfo.InvariantCulture);
                SeleccionarCategoriaEnCombo(fila.Categoria);

                // Una vez creado el producto, la Moneda no se vuelve a editar.
                MostrarSeccionMonedaProducto(false);
            }
        }

        private void SeleccionarCategoriaEnCombo(string nombreCategoria)
        {
            for (int i = 0; i < cmbCategoriaProducto.Items.Count; i++)
            {
                if (cmbCategoriaProducto.Items[i] is CategoriaProducto categoria && string.Equals(categoria.Nombre, nombreCategoria, StringComparison.Ordinal))
                {
                    cmbCategoriaProducto.SelectedIndex = i;
                    return;
                }
            }
        }

        private void MostrarSeccionMonedaProducto(bool mostrar)
        {
            lblMonedaProducto.Visible = mostrar;
            cmbMonedaProducto.Visible = mostrar;
            cmbMonedaProducto.Enabled = mostrar;
        }

        private void LimpiarFormularioProducto()
        {
            txtbNombreProducto.Clear();
            txtbPrecioProducto.Clear();
            dgvProductos.ClearSelection();
            MostrarSeccionMonedaProducto(true);
        }

        // --- Categorias de producto ---

        private void CargarCategorias(int? categoriaSeleccionadaId)
        {
            try
            {
                List<CategoriaProducto> categorias = _categoriaProductoBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .ToList();

                cmbCategoriaProducto.DataSource = null;
                cmbCategoriaProducto.DisplayMember = "Nombre";
                cmbCategoriaProducto.ValueMember = "Id";
                cmbCategoriaProducto.DataSource = categorias;

                dgvCategoriasProducto.DataSource = null;
                dgvCategoriasProducto.DataSource = categorias.Select(ProyectarFilaCategoria).ToList();

                if (categoriaSeleccionadaId.HasValue)
                {
                    SeleccionarFila<FilaCategoriaProducto>(dgvCategoriasProducto, fila => fila.Id == categoriaSeleccionadaId.Value);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de categorias de producto");
            }
        }

        private static FilaCategoriaProducto ProyectarFilaCategoria(CategoriaProducto categoria)
        {
            return new FilaCategoriaProducto { Id = categoria.Id, Nombre = categoria.Nombre };
        }

        private void btnAgregarCategoriaProducto_Click(object sender, EventArgs e)
        {
            try
            {
                CategoriaProducto creada = _categoriaProductoBLL.Crear(txtbNombreCategoriaProducto.Text);
                txtbNombreCategoriaProducto.Clear();
                CargarCategorias(creada.Id);
                CargarProductos(null);
                UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.CategoriaCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de categoria de producto");
            }
        }

        private void btnActualizarCategoriaProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvCategoriasProducto, out FilaCategoriaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectCategoria");
                return;
            }

            try
            {
                bool actualizado = _categoriaProductoBLL.Actualizar(fila.Id, txtbNombreCategoriaProducto.Text);
                if (actualizado)
                {
                    CargarCategorias(fila.Id);
                    CargarProductos(null);
                    UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.CategoriaUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de categoria de producto");
            }
        }

        private void btnEliminarCategoriaProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvCategoriasProducto, out FilaCategoriaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectCategoria");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMerchantDetail.Confirm.DeleteCategoriaMessage",
                "FormMerchantDetail.Confirm.DeleteCategoriaTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = _categoriaProductoBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    txtbNombreCategoriaProducto.Clear();
                    CargarCategorias(null);
                    CargarProductos(null);
                    UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.CategoriaDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de categoria de producto");
            }
        }

        private void dgvCategoriasProducto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(dgvCategoriasProducto, out FilaCategoriaProducto fila))
            {
                txtbNombreCategoriaProducto.Text = fila.Nombre;
            }
        }

        // --- Promociones del comercio (solo lectura) ---

        private void CargarPromociones()
        {
            try
            {
                List<Promocion> promociones = _promocionBLL.ObtenerPorComercio(_comercioId);

                dgvPromociones.DataSource = null;
                dgvPromociones.DataSource = promociones.Select(ProyectarFilaPromocion).ToList();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de promociones del comercio");
            }
        }

        private static FilaPromocion ProyectarFilaPromocion(Promocion promocion)
        {
            return new FilaPromocion
            {
                Descripcion = promocion.Descripcion,
                Descuento = promocion.Descuento,
                VigenciaDesde = promocion.VigenciaDesde,
                VigenciaHasta = promocion.VigenciaHasta
            };
        }

        // --- Helpers comunes a las grillas ---

        private static bool ObtenerFilaSeleccionada<T>(DataGridView grid, out T fila) where T : class
        {
            fila = grid.CurrentRow?.DataBoundItem as T;
            return fila != null;
        }

        private static void SeleccionarFila<T>(DataGridView grid, Func<T, bool> predicado) where T : class
        {
            DataGridViewRow fila = grid.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(item => item.DataBoundItem is T dato && predicado(dato));

            if (fila == null)
            {
                return;
            }

            grid.ClearSelection();
            fila.Selected = true;
            if (fila.Cells.Count > 0)
            {
                grid.CurrentCell = fila.Cells[0];
            }
        }
    }
}
