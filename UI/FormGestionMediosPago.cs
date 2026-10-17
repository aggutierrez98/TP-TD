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
    // Pantalla de operador (back-office): catalogo global de medios de pago (Tipo + Marca),
    // independiente de la entidad que los emite. Mismo patron grilla + alta/edicion/baja
    // inline que FormGestionMonedas. El producto concreto de cada entidad (con Segmento) se
    // gestiona en FormDetalleEntidadFinanciera.
    public partial class FormGestionMediosPago : FormTraducibleBase
    {
        private readonly MedioPagoBLL medioPagoBLL = CompositionRoot.MedioPago;

        public FormGestionMediosPago()
        {
            InitializeComponent();
            Load += FormGestionMediosPago_Load;
        }

        // View-model plano para bindear la grilla. Tipo queda como enum: la celda se
        // traduce en dgvMediosPago_CellFormatting (se re-traduce sola al cambiar el idioma).
        private sealed class FilaMedioPago
        {
            public int Id { get; set; }
            public TipoMedioPago Tipo { get; set; }
            public string Marca { get; set; }
        }

        private void FormGestionMediosPago_Load(object sender, EventArgs e)
        {
            dgvMediosPago.CellFormatting += dgvMediosPago_CellFormatting;
            CargarTipos();
            CargarMediosPago(null);
        }

        private void CargarTipos()
        {
            cmbTipoMedioPago.DisplayMember = "Texto";
            cmbTipoMedioPago.Items.Clear();
            foreach (TipoMedioPago tipo in EtiquetasFinanzas.TiposMedioPago)
            {
                cmbTipoMedioPago.Items.Add(new OpcionTraducible(EtiquetasFinanzas.ClaveTipoMedioPago(tipo), tipo));
            }

            cmbTipoMedioPago.SelectedIndex = -1;
        }

        private void CargarMediosPago(int? medioPagoSeleccionadoId)
        {
            try
            {
                List<MedioPago> mediosPago = medioPagoBLL.ObtenerTodos()
                    .OrderBy(item => item.Tipo)
                    .ThenBy(item => item.Marca)
                    .ToList();

                dgvMediosPago.DataSource = null;
                dgvMediosPago.DataSource = mediosPago.Select(ProyectarFilaMedioPago).ToList();

                if (medioPagoSeleccionadoId.HasValue)
                {
                    SeleccionarFila<FilaMedioPago>(dgvMediosPago, fila => fila.Id == medioPagoSeleccionadoId.Value);
                }
                else
                {
                    dgvMediosPago.ClearSelection();
                    dgvMediosPago.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de medios de pago");
            }
        }

        private static FilaMedioPago ProyectarFilaMedioPago(MedioPago medioPago)
        {
            return new FilaMedioPago { Id = medioPago.Id, Tipo = medioPago.Tipo, Marca = medioPago.Marca };
        }

        private void btnAgregarMedioPago_Click(object sender, EventArgs e)
        {
            if (!ObtenerTipoSeleccionado(out TipoMedioPago tipo))
            {
                UiDialogHelper.MostrarWarning("FormPaymentMethodManagement.Warning.SelectType");
                return;
            }

            try
            {
                MedioPago creado = medioPagoBLL.Crear(tipo, txtbMarcaMedioPago.Text);
                LimpiarFormulario();
                CargarMediosPago(creado.Id);
                UiDialogHelper.MostrarInfo("FormPaymentMethodManagement.Success.Created");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de medio de pago");
            }
        }

        private void btnEditarMedioPago_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvMediosPago, out FilaMedioPago fila))
            {
                UiDialogHelper.MostrarWarning("FormPaymentMethodManagement.Warning.SelectMedioPago");
                return;
            }

            if (!ObtenerTipoSeleccionado(out TipoMedioPago tipo))
            {
                UiDialogHelper.MostrarWarning("FormPaymentMethodManagement.Warning.SelectType");
                return;
            }

            try
            {
                bool actualizado = medioPagoBLL.Actualizar(fila.Id, tipo, txtbMarcaMedioPago.Text);
                if (actualizado)
                {
                    LimpiarFormulario();
                    CargarMediosPago(fila.Id);
                    UiDialogHelper.MostrarInfo("FormPaymentMethodManagement.Success.Updated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de medio de pago");
            }
        }

        private void btnEliminarMedioPago_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvMediosPago, out FilaMedioPago fila))
            {
                UiDialogHelper.MostrarWarning("FormPaymentMethodManagement.Warning.SelectMedioPago");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormPaymentMethodManagement.Confirm.DeleteMessage",
                "FormPaymentMethodManagement.Confirm.DeleteTitle",
                MessageBoxIcon.Warning,
                DescribirMedioPago(fila));

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = medioPagoBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    LimpiarFormulario();
                    CargarMediosPago(null);
                    UiDialogHelper.MostrarInfo("FormPaymentMethodManagement.Success.Deleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de medio de pago");
            }
        }

        private void dgvMediosPago_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(dgvMediosPago, out FilaMedioPago fila))
            {
                SeleccionarTipoEnCombo(fila.Tipo);
                txtbMarcaMedioPago.Text = fila.Marca;
            }
        }

        private void dgvMediosPago_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || !(e.Value is TipoMedioPago tipo))
            {
                return;
            }

            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma == null)
            {
                return;
            }

            e.Value = idioma.ObtenerTraduccion(EtiquetasFinanzas.ClaveTipoMedioPago(tipo));
            e.FormattingApplied = true;
        }

        private bool ObtenerTipoSeleccionado(out TipoMedioPago tipo)
        {
            if (cmbTipoMedioPago.SelectedItem is OpcionTraducible opcion && opcion.Valor is TipoMedioPago valor)
            {
                tipo = valor;
                return true;
            }

            tipo = default(TipoMedioPago);
            return false;
        }

        private void SeleccionarTipoEnCombo(TipoMedioPago tipo)
        {
            for (int i = 0; i < cmbTipoMedioPago.Items.Count; i++)
            {
                if (cmbTipoMedioPago.Items[i] is OpcionTraducible opcion && opcion.Valor is TipoMedioPago valor && valor == tipo)
                {
                    cmbTipoMedioPago.SelectedIndex = i;
                    return;
                }
            }

            cmbTipoMedioPago.SelectedIndex = -1;
        }

        private static string DescribirMedioPago(FilaMedioPago fila)
        {
            string tipo = UiDialogHelper.Traducir(EtiquetasFinanzas.ClaveTipoMedioPago(fila.Tipo));
            return string.IsNullOrWhiteSpace(fila.Marca) ? tipo : string.Format("{0} {1}", tipo, fila.Marca);
        }

        private void LimpiarFormulario()
        {
            cmbTipoMedioPago.SelectedIndex = -1;
            txtbMarcaMedioPago.Clear();
            cmbTipoMedioPago.Focus();
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
