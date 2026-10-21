using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionCategorias : FormTraducibleBase
    {
        private const string ScopeGlobal = "Global";
        private const string ScopePropia = "Propia";

        private readonly CategoriaBLL categoriaBLL = CompositionRoot.Categoria;
        private readonly ISesion sesion = CompositionRoot.Sesion;

        public FormGestionCategorias()
        {
            InitializeComponent();
            Load += FormGestionCategorias_Load;
        }

        private void FormGestionCategorias_Load(object sender, EventArgs e)
        {
            dgvCategorias.CellFormatting += dgvCategorias_CellFormatting;
            CargarCategorias(null);
        }

        // View-model plano para la grilla. La grilla no autogenera columnas, asi que
        // Categoria (la entidad que se pasa al BLL) no se muestra.
        private sealed class FilaCategoria
        {
            public Categoria Categoria { get; set; }
            public string Nombre { get; set; }
            public string Scope { get; set; }
            public bool EsPropia { get; set; }
        }

        private void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            try
            {
                Categoria creada = categoriaBLL.Crear(new Categoria { Nombre = txtbNombreCategoria.Text });
                txtbNombreCategoria.Clear();
                CargarCategorias(creada.Id);
                UiDialogHelper.MostrarInfo("FormCategories.Success.CategoryCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de categoria");
            }
        }

        private void btnActualizarCategoria_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaCategoria fila))
            {
                return;
            }

            if (!fila.EsPropia)
            {
                UiDialogHelper.MostrarWarning("FormCategories.Warning.CannotEditGlobal");
                return;
            }

            try
            {
                fila.Categoria.Nombre = txtbNombreCategoria.Text;
                bool actualizado = categoriaBLL.Actualizar(fila.Categoria);
                if (actualizado)
                {
                    CargarCategorias(fila.Categoria.Id);
                    UiDialogHelper.MostrarInfo("FormCategories.Success.CategoryUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de categoria");
            }
        }

        private void btnEliminarCategoria_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaCategoria fila))
            {
                return;
            }

            if (!fila.EsPropia)
            {
                UiDialogHelper.MostrarWarning("FormCategories.Warning.CannotEditGlobal");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormCategories.Confirm.DeleteCategoryMessage",
                "FormCategories.Confirm.DeleteCategoryTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = categoriaBLL.Eliminar(fila.Categoria);
                if (eliminado)
                {
                    txtbNombreCategoria.Clear();
                    CargarCategorias(null);
                    UiDialogHelper.MostrarInfo("FormCategories.Success.CategoryDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de categoria");
            }
        }

        private void dgvCategorias_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(out FilaCategoria fila))
            {
                txtbNombreCategoria.Text = fila.Nombre;
            }
        }

        private void dgvCategorias_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            if (!string.Equals(dgvCategorias.Columns[e.ColumnIndex].Name, "Scope", StringComparison.Ordinal))
            {
                return;
            }

            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma == null)
            {
                return;
            }

            string key = string.Equals(e.Value.ToString(), ScopeGlobal, StringComparison.Ordinal)
                ? "FormCategories.Value.Scope.Global"
                : "FormCategories.Value.Scope.Own";

            e.Value = idioma.ObtenerTraduccion(key);
            e.FormattingApplied = true;
        }

        private void CargarCategorias(int? categoriaSeleccionadaId)
        {
            int usuarioActualId = sesion.GetUsuarioActual().Id;
            List<Categoria> categorias = categoriaBLL.ObtenerVisiblesParaUsuarioActual();

            dgvCategorias.DataSource = null;
            dgvCategorias.DataSource = categorias
                .OrderBy(item => item.Usuario != null)  
                .ThenBy(item => item.Nombre)
                .Select(item => ProyectarFila(item, usuarioActualId))
                .ToList();

            if (categoriaSeleccionadaId.HasValue)
            {
                SeleccionarCategoria(categoriaSeleccionadaId.Value);
            }
        }

        private void SeleccionarCategoria(int id)
        {
            DataGridViewRow fila = dgvCategorias.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(item => item.DataBoundItem is FilaCategoria filaCategoria && filaCategoria.Categoria.Id == id);

            if (fila == null)
            {
                return;
            }

            dgvCategorias.ClearSelection();
            fila.Selected = true;
            if (fila.Cells.Count > 0)
            {
                dgvCategorias.CurrentCell = fila.Cells[0];
            }
        }

        private bool ObtenerFilaSeleccionada(out FilaCategoria fila)
        {
            fila = dgvCategorias.CurrentRow?.DataBoundItem as FilaCategoria;
            if (fila == null)
            {
                UiDialogHelper.MostrarWarning("FormCategories.Warning.SelectCategory");
                return false;
            }

            return true;
        }

        private static FilaCategoria ProyectarFila(Categoria categoria, int usuarioActualId)
        {
            bool esPropia = categoria.Usuario != null && categoria.Usuario.Id == usuarioActualId;
            return new FilaCategoria
            {
                Categoria = categoria,
                Nombre = categoria.Nombre,
                Scope = categoria.EsGlobal ? ScopeGlobal : ScopePropia,
                EsPropia = esPropia
            };
        }
    }
}
