using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormListaDeCompras : FormTraducibleBase
    {
        private readonly ListaDeComprasBLL listaDeComprasBLL = CompositionRoot.ListaDeCompras;
        private ListaDeCompras listaActual;
        private bool actualizandoGrid;

        public FormListaDeCompras()
        {
            InitializeComponent();
            Load += FormListaDeCompras_Load;
        }

        private void FormListaDeCompras_Load(object sender, EventArgs e)
        {
            dgvItems.CurrentCellDirtyStateChanged += dgvItems_CurrentCellDirtyStateChanged;
            dgvItems.CellValueChanged += dgvItems_CellValueChanged;
            CargarLista();
        }

        // View-model plano para la grilla.
        private sealed class FilaItemLista
        {
            public ItemLista Item { get; set; }
            public string Descripcion { get; set; }
            public int Cantidad { get; set; }
            public bool Comprado { get; set; }
        }

        private void CargarLista()
        {
            try
            {
                ListaDeCompras lista = listaDeComprasBLL.ObtenerOCrearParaUsuarioActual();
                listaActual = lista;

                actualizandoGrid = true;
                try
                {
                    dgvItems.DataSource = null;
                    dgvItems.DataSource = lista.ItemsLista
                        .Select(item => new FilaItemLista
                        {
                            Item = item,
                            Descripcion = item.Descripcion,
                            Cantidad = item.Cantidad,
                            Comprado = item.Comprado
                        })
                        .ToList();
                }
                finally
                {
                    actualizandoGrid = false;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de lista de compras");
            }
        }

        private void btnAgregarItem_Click(object sender, EventArgs e)
        {
            string descripcion = txtbDescripcionItem.Text;

            if (!int.TryParse(txtbCantidadItem.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int cantidad))
            {
                UiDialogHelper.MostrarWarning("FormShoppingList.Warning.InvalidQuantity");
                return;
            }

            try
            {
                listaDeComprasBLL.AgregarItem(new ItemLista { ListaDeCompras = listaActual, Descripcion = descripcion, Cantidad = cantidad });
                LimpiarEdicion();
                CargarLista();
                UiDialogHelper.MostrarInfo("FormShoppingList.Success.ItemCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de item de lista de compras");
            }
        }

        private void btnEliminarItem_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaItemLista fila))
            {
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormShoppingList.Confirm.DeleteItemMessage",
                "FormShoppingList.Confirm.DeleteItemTitle",
                MessageBoxIcon.Warning,
                fila.Descripcion);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = listaDeComprasBLL.EliminarItem(fila.Item);
                if (eliminado)
                {
                    CargarLista();
                    UiDialogHelper.MostrarInfo("FormShoppingList.Success.ItemDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de item de lista de compras");
            }
        }

        private void dgvItems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvItems.IsCurrentCellDirty)
            {
                dgvItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (actualizandoGrid || e.RowIndex < 0)
            {
                return;
            }

            if (!string.Equals(dgvItems.Columns[e.ColumnIndex].Name, "Comprado", StringComparison.Ordinal))
            {
                return;
            }

            if (!(dgvItems.Rows[e.RowIndex].DataBoundItem is FilaItemLista fila))
            {
                return;
            }

            try
            {
                listaDeComprasBLL.MarcarComprado(fila.Item, fila.Comprado);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Actualizacion de item de lista de compras");
                CargarLista();
            }
        }

        private bool ObtenerFilaSeleccionada(out FilaItemLista fila)
        {
            fila = dgvItems.CurrentRow?.DataBoundItem as FilaItemLista;
            if (fila == null)
            {
                UiDialogHelper.MostrarWarning("FormShoppingList.Warning.SelectItem");
                return false;
            }

            return true;
        }

        private void LimpiarEdicion()
        {
            txtbDescripcionItem.Clear();
            txtbCantidadItem.Clear();
        }
    }
}
