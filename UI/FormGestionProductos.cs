using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Catalogo de productos (back-office): independiente de los comercios. Que comercio vende un
    // producto y a que precio se administra desde el detalle del comercio.
    public partial class FormGestionProductos : FormTraducibleBase
    {
        private readonly ProductoBLL _productoBLL = CompositionRoot.Producto;
        private readonly CategoriaProductoBLL _categoriaProductoBLL = CompositionRoot.CategoriaProducto;

        public FormGestionProductos()
        {
            InitializeComponent();
            Load += FormGestionProductos_Load;
        }

        // View-model plano para bindear la grilla.
        private sealed class FilaProducto
        {
            public int Id { get; set; }
            public int CategoriaId { get; set; }
            public string Nombre { get; set; }
            public string Categoria { get; set; }
        }

        private void FormGestionProductos_Load(object sender, EventArgs e)
        {
            CargarCategorias();
            CargarProductos(null);
        }

        private void CargarCategorias()
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
                cmbCategoriaProducto.SelectedIndex = -1;
                lblSinCategoriasProducto.Visible = categorias.Count == 0;
                btnAgregarProducto.Enabled = categorias.Count > 0;
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de categorias de producto");
            }
        }

        private void CargarProductos(int? productoSeleccionadoId)
        {
            try
            {
                List<Producto> productos = _productoBLL.ObtenerTodos();

                dgvProductos.DataSource = null;
                dgvProductos.DataSource = productos.Select(ProyectarFilaProducto).ToList();

                if (productoSeleccionadoId.HasValue)
                {
                    SeleccionarFila(dgvProductos, fila => fila.Id == productoSeleccionadoId.Value);
                }
                else
                {
                    dgvProductos.ClearSelection();
                    dgvProductos.CurrentCell = null;
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
                CategoriaId = producto.CategoriaProducto?.Id ?? 0,
                Nombre = producto.Nombre,
                Categoria = producto.CategoriaProducto?.Nombre ?? string.Empty
            };
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (!(cmbCategoriaProducto.SelectedItem is CategoriaProducto categoriaSeleccionada))
            {
                UiDialogHelper.MostrarWarning("FormProductManagement.Warning.SelectCategoria");
                return;
            }

            try
            {
                Producto creado = _productoBLL.Crear(txtbNombreProducto.Text, categoriaSeleccionada.Id);
                LimpiarFormulario();
                CargarProductos(creado.Id);
                UiDialogHelper.MostrarInfo("FormProductManagement.Success.ProductoCreated");
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
                UiDialogHelper.MostrarWarning("FormProductManagement.Warning.SelectProducto");
                return;
            }

            if (!(cmbCategoriaProducto.SelectedItem is CategoriaProducto categoriaSeleccionada))
            {
                UiDialogHelper.MostrarWarning("FormProductManagement.Warning.SelectCategoria");
                return;
            }

            try
            {
                bool actualizado = _productoBLL.Actualizar(fila.Id, txtbNombreProducto.Text, categoriaSeleccionada.Id);
                if (actualizado)
                {
                    LimpiarFormulario();
                    CargarProductos(fila.Id);
                    UiDialogHelper.MostrarInfo("FormProductManagement.Success.ProductoUpdated");
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
                UiDialogHelper.MostrarWarning("FormProductManagement.Warning.SelectProducto");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormProductManagement.Confirm.DeleteProductoMessage",
                "FormProductManagement.Confirm.DeleteProductoTitle",
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
                    LimpiarFormulario();
                    CargarProductos(null);
                    UiDialogHelper.MostrarInfo("FormProductManagement.Success.ProductoDeleted");
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
                SeleccionarCategoriaEnCombo(fila.CategoriaId);
            }
        }

        private void SeleccionarCategoriaEnCombo(int categoriaId)
        {
            for (int i = 0; i < cmbCategoriaProducto.Items.Count; i++)
            {
                if (cmbCategoriaProducto.Items[i] is CategoriaProducto categoria && categoria.Id == categoriaId)
                {
                    cmbCategoriaProducto.SelectedIndex = i;
                    return;
                }
            }

            cmbCategoriaProducto.SelectedIndex = -1;
        }

        private void LimpiarFormulario()
        {
            txtbNombreProducto.Clear();
            cmbCategoriaProducto.SelectedIndex = -1;
            txtbNombreProducto.Focus();
        }

        private static bool ObtenerFilaSeleccionada(DataGridView grid, out FilaProducto fila)
        {
            fila = grid.CurrentRow?.DataBoundItem as FilaProducto;
            return fila != null;
        }

        private static void SeleccionarFila(DataGridView grid, Func<FilaProducto, bool> predicado)
        {
            DataGridViewRow fila = grid.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(item => item.DataBoundItem is FilaProducto dato && predicado(dato));

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
