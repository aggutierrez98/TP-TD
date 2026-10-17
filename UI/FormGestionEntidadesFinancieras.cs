using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Pantalla de operador (back-office): catalogo global de entidades financieras (bancos,
    // billeteras virtuales). Grilla maestra + alta/edicion/baja inline; el catalogo de medios
    // de pago de cada entidad se gestiona en FormDetalleEntidadFinanciera (mismo patron que
    // FormGestionComercios -> FormDetalleComercio).
    public partial class FormGestionEntidadesFinancieras : FormTraducibleBase
    {
        private readonly EntidadFinancieraBLL entidadFinancieraBLL = CompositionRoot.EntidadFinanciera;

        public FormGestionEntidadesFinancieras()
        {
            InitializeComponent();
            Load += FormGestionEntidadesFinancieras_Load;
        }

        // View-model plano para bindear la grilla.
        private sealed class FilaEntidadFinanciera
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        private void FormGestionEntidadesFinancieras_Load(object sender, EventArgs e)
        {
            CargarEntidades(null);
        }

        private void CargarEntidades(int? entidadSeleccionadaId)
        {
            try
            {
                List<EntidadFinanciera> entidades = entidadFinancieraBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .ToList();

                dgvEntidadesFinancieras.DataSource = null;
                dgvEntidadesFinancieras.DataSource = entidades
                    .Select(item => new FilaEntidadFinanciera { Id = item.Id, Nombre = item.Nombre })
                    .ToList();

                if (entidadSeleccionadaId.HasValue)
                {
                    SeleccionarFila<FilaEntidadFinanciera>(dgvEntidadesFinancieras, fila => fila.Id == entidadSeleccionadaId.Value);
                }
                else
                {
                    dgvEntidadesFinancieras.ClearSelection();
                    dgvEntidadesFinancieras.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de entidades financieras");
            }
        }

        private void btnAgregarEntidadFinanciera_Click(object sender, EventArgs e)
        {
            try
            {
                EntidadFinanciera creada = entidadFinancieraBLL.Crear(txtbNombreEntidadFinanciera.Text);
                txtbNombreEntidadFinanciera.Clear();
                CargarEntidades(creada.Id);
                UiDialogHelper.MostrarInfo("FormFinancialEntityManagement.Success.Created");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de entidad financiera");
            }
        }

        private void btnEditarEntidadFinanciera_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvEntidadesFinancieras, out FilaEntidadFinanciera fila))
            {
                UiDialogHelper.MostrarWarning("FormFinancialEntityManagement.Warning.SelectEntity");
                return;
            }

            try
            {
                bool actualizada = entidadFinancieraBLL.Actualizar(fila.Id, txtbNombreEntidadFinanciera.Text);
                if (actualizada)
                {
                    CargarEntidades(fila.Id);
                    UiDialogHelper.MostrarInfo("FormFinancialEntityManagement.Success.Updated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de entidad financiera");
            }
        }

        private void btnEliminarEntidadFinanciera_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvEntidadesFinancieras, out FilaEntidadFinanciera fila))
            {
                UiDialogHelper.MostrarWarning("FormFinancialEntityManagement.Warning.SelectEntity");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormFinancialEntityManagement.Confirm.DeleteMessage",
                "FormFinancialEntityManagement.Confirm.DeleteTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminada = entidadFinancieraBLL.Eliminar(fila.Id);
                if (eliminada)
                {
                    txtbNombreEntidadFinanciera.Clear();
                    CargarEntidades(null);
                    UiDialogHelper.MostrarInfo("FormFinancialEntityManagement.Success.Deleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de entidad financiera");
            }
        }

        private void btnVerDetalleEntidadFinanciera_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvEntidadesFinancieras, out FilaEntidadFinanciera fila))
            {
                UiDialogHelper.MostrarWarning("FormFinancialEntityManagement.Warning.SelectEntity");
                return;
            }

            using (FormDetalleEntidadFinanciera formDetalle = new FormDetalleEntidadFinanciera(fila.Id, fila.Nombre))
            {
                formDetalle.ShowDialog(this);
            }
        }

        private void dgvEntidadesFinancieras_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(dgvEntidadesFinancieras, out FilaEntidadFinanciera fila))
            {
                txtbNombreEntidadFinanciera.Text = fila.Nombre;
            }
        }

        // --- Helpers de grilla ---

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
