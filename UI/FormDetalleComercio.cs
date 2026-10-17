using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    // Pantalla de operador (back-office) abierta en modo modal desde FormGestionComercios,
    // igual patron de constructor que FormHistorialUsuario. Dos secciones independientes
    // sobre un mismo comercio: Productos (se eligen del catalogo y se les pone precio y moneda
    // para este comercio; el catalogo se administra en FormGestionProductos) y Promociones del
    // comercio (solo lectura: las genera un proceso automatico fuera de esta tanda).
    public partial class FormDetalleComercio : FormTraducibleBase
    {
        private readonly int _comercioId;
        private readonly ProductoBLL _productoBLL = CompositionRoot.Producto;
        private readonly ComercioProductoBLL _comercioProductoBLL = CompositionRoot.ComercioProducto;
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
            public int MonedaId { get; set; }
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public decimal Precio { get; set; }
            public string Moneda { get; set; }
        }

        private sealed class FilaPromocion
        {
            public string Descripcion { get; set; }
            public decimal Descuento { get; set; }

            // null = sin tope; se muestra "-" en dgvPromociones_CellFormatting.
            public decimal? TopeDescuento { get; set; }

            // Lista vacia = todos los dias; se traduce en dgvPromociones_CellFormatting.
            public List<DiaSemana> DiasSemana { get; set; }

            public DateTime VigenciaDesde { get; set; }
            public DateTime VigenciaHasta { get; set; }
        }

        private void FormDetalleComercio_Load(object sender, EventArgs e)
        {
            dgvPromociones.CellFormatting += dgvPromociones_CellFormatting;
            CargarMonedas();
            CargarCatalogoProductos();
            CargarProductos(null);
            CargarPromociones();
            ModoEdicionProducto(false);
        }

        // --- Productos del comercio ---

        private void CargarProductos(int? productoSeleccionadoId)
        {
            try
            {
                List<ComercioProducto> asignaciones = _comercioProductoBLL.ObtenerPorComercio(_comercioId);

                dgvProductos.DataSource = null;
                dgvProductos.DataSource = asignaciones
                    .OrderBy(item => item.Producto.Nombre)
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

        private static FilaProducto ProyectarFilaProducto(ComercioProducto asignacion)
        {
            return new FilaProducto
            {
                Id = asignacion.Producto.Id,
                MonedaId = asignacion.Moneda?.Id ?? 0,
                Nombre = asignacion.Producto.Nombre,
                Categoria = asignacion.Producto.CategoriaProducto?.Nombre ?? string.Empty,
                Precio = asignacion.Precio,
                Moneda = asignacion.Moneda?.Codigo ?? string.Empty
            };
        }

        private void CargarCatalogoProductos()
        {
            try
            {
                List<Producto> catalogo = _productoBLL.ObtenerTodos()
                    .OrderBy(item => item.Nombre)
                    .ToList();

                cmbProductoCatalogo.DataSource = null;
                cmbProductoCatalogo.DisplayMember = "Nombre";
                cmbProductoCatalogo.ValueMember = "Id";
                cmbProductoCatalogo.DataSource = catalogo;
                cmbProductoCatalogo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga del catalogo de productos");
            }
        }

        private void CargarMonedas()
        {
            try
            {
                List<Moneda> monedas = _comercioProductoBLL.ObtenerMonedas()
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
            if (!(cmbProductoCatalogo.SelectedItem is Producto productoSeleccionado))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectProductoCatalogo");
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
                _comercioProductoBLL.Asignar(_comercioId, productoSeleccionado.Id, monedaSeleccionada.Id, precioParseado);
                LimpiarFormularioProducto();
                CargarProductos(productoSeleccionado.Id);
                UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.ProductoAssigned");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Asignacion de producto al comercio");
            }
        }

        private void btnActualizarProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvProductos, out FilaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectProducto");
                return;
            }

            if (!decimal.TryParse(txtbPrecioProducto.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precioParseado) || precioParseado <= 0)
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.InvalidPrecio");
                return;
            }

            try
            {
                bool actualizado = _comercioProductoBLL.ActualizarPrecio(_comercioId, fila.Id, precioParseado);
                if (actualizado)
                {
                    CargarProductos(fila.Id);
                    UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.ProductoUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de precio de producto");
            }
        }

        private void btnQuitarProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvProductos, out FilaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantDetail.Warning.SelectProducto");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMerchantDetail.Confirm.RemoveProductoMessage",
                "FormMerchantDetail.Confirm.RemoveProductoTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool quitado = _comercioProductoBLL.Quitar(_comercioId, fila.Id);
                if (quitado)
                {
                    LimpiarFormularioProducto();
                    CargarProductos(null);
                    UiDialogHelper.MostrarInfo("FormMerchantDetail.Success.ProductoRemoved");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de producto del comercio");
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
                SeleccionarEnCombo(cmbProductoCatalogo, item => item is Producto producto && producto.Id == fila.Id);
                SeleccionarEnCombo(cmbMonedaProducto, item => item is Moneda moneda && moneda.Id == fila.MonedaId);
                txtbPrecioProducto.Text = fila.Precio.ToString(CultureInfo.InvariantCulture);

                // Una vez asignado el producto, solo se edita el precio: ni el producto ni la
                // moneda se vuelven a cambiar (para otra moneda, se quita y se vuelve a asignar).
                ModoEdicionProducto(true);
            }
        }

        private static void SeleccionarEnCombo(Krypton.Toolkit.KryptonComboBox combo, Func<object, bool> predicado)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (predicado(combo.Items[i]))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = -1;
        }

        private void ModoEdicionProducto(bool editando)
        {
            cmbProductoCatalogo.Enabled = !editando;
            cmbMonedaProducto.Enabled = !editando;
        }

        private void LimpiarFormularioProducto()
        {
            cmbProductoCatalogo.SelectedIndex = -1;
            txtbPrecioProducto.Clear();
            dgvProductos.ClearSelection();
            ModoEdicionProducto(false);
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
                TopeDescuento = promocion.TopeDescuento,
                DiasSemana = promocion.DiasSemana ?? new List<DiaSemana>(),
                VigenciaDesde = promocion.VigenciaDesde,
                VigenciaHasta = promocion.VigenciaHasta
            };
        }

        private void dgvPromociones_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            string columna = dgvPromociones.Columns[e.ColumnIndex].Name;
            if (string.Equals(columna, "TopeDescuento", StringComparison.Ordinal) && e.Value == null)
            {
                e.Value = "-";
                e.FormattingApplied = true;
                return;
            }

            if (!string.Equals(columna, "DiasSemana", StringComparison.Ordinal) || !(e.Value is List<DiaSemana> dias))
            {
                return;
            }

            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma == null)
            {
                return;
            }

            e.Value = dias.Count == 0
                ? idioma.ObtenerTraduccion(EtiquetasFinanzas.ClaveTodosLosDias)
                : string.Join(", ", dias.OrderBy(dia => dia).Select(dia => idioma.ObtenerTraduccion(EtiquetasFinanzas.ClaveDiaSemana(dia))));
            e.FormattingApplied = true;
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
