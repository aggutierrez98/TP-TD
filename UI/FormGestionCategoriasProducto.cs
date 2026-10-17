using System;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    public partial class FormGestionCategoriasProducto : FormTraducibleBase
    {
        private readonly CategoriaProductoBLL _categoriaProductoBLL = CompositionRoot.CategoriaProducto;

        public FormGestionCategoriasProducto()
        {
            InitializeComponent();
            Load += FormGestionCategoriasProducto_Load;
        }

        private sealed class FilaCategoriaProducto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        private void FormGestionCategoriasProducto_Load(object sender, EventArgs e)
        {
            CargarCategorias(null);
        }

        private void CargarCategorias(int? categoriaSeleccionadaId)
        {
            try
            {
                dgvCategoriasProducto.DataSource = null;
                dgvCategoriasProducto.DataSource = _categoriaProductoBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .Select(item => new FilaCategoriaProducto { Id = item.Id, Nombre = item.Nombre })
                    .ToList();
                dgvCategoriasProducto.ClearSelection();
                dgvCategoriasProducto.CurrentCell = null;
                if (categoriaSeleccionadaId.HasValue)
                {
                    DataGridViewRow fila = dgvCategoriasProducto.Rows.Cast<DataGridViewRow>()
                        .FirstOrDefault(item => item.DataBoundItem is FilaCategoriaProducto categoria
                            && categoria.Id == categoriaSeleccionadaId.Value);
                    if (fila != null)
                    {
                        fila.Selected = true;
                        dgvCategoriasProducto.CurrentCell = fila.Cells[0];
                    }
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de categorias de producto");
            }
        }

        private void btnAgregarCategoriaProducto_Click(object sender, EventArgs e)
        {
            try
            {
                CategoriaProducto creada = _categoriaProductoBLL.Crear(txtbNombreCategoriaProducto.Text);
                txtbNombreCategoriaProducto.Clear();
                CargarCategorias(creada.Id);
                UiDialogHelper.MostrarInfo("FormProductCategoryManagement.Success.CategoriaCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de categoria de producto");
            }
        }

        private void btnActualizarCategoriaProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaCategoriaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormProductCategoryManagement.Warning.SelectCategoria");
                return;
            }

            try
            {
                if (_categoriaProductoBLL.Actualizar(fila.Id, txtbNombreCategoriaProducto.Text))
                {
                    CargarCategorias(fila.Id);
                    UiDialogHelper.MostrarInfo("FormProductCategoryManagement.Success.CategoriaUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de categoria de producto");
            }
        }

        private void btnEliminarCategoriaProducto_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaCategoriaProducto fila))
            {
                UiDialogHelper.MostrarWarning("FormProductCategoryManagement.Warning.SelectCategoria");
                return;
            }

            if (UiDialogHelper.Confirmar("FormProductCategoryManagement.Confirm.DeleteCategoriaMessage",
                "FormProductCategoryManagement.Confirm.DeleteCategoriaTitle", MessageBoxIcon.Warning,
                fila.Nombre) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (_categoriaProductoBLL.Eliminar(fila.Id))
                {
                    txtbNombreCategoriaProducto.Clear();
                    CargarCategorias(null);
                    UiDialogHelper.MostrarInfo("FormProductCategoryManagement.Success.CategoriaDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de categoria de producto");
            }
        }

        private void dgvCategoriasProducto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && ObtenerFilaSeleccionada(out FilaCategoriaProducto fila))
            {
                txtbNombreCategoriaProducto.Text = fila.Nombre;
            }
        }

        private bool ObtenerFilaSeleccionada(out FilaCategoriaProducto fila)
        {
            fila = dgvCategoriasProducto.CurrentRow?.DataBoundItem as FilaCategoriaProducto;
            return fila != null;
        }
    }
}
