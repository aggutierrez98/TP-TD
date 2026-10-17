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
    // Pantalla de operador (back-office) abierta en modo modal desde
    // FormGestionEntidadesFinancieras, mismo patron de constructor que FormDetalleComercio.
    // Una sola seccion: el catalogo de medios de pago que ofrece la entidad (MedioPagoEntidad =
    // MedioPago del catalogo global + Segmento). Solo alta y baja: para "editar" un producto
    // se quita y se vuelve a agregar.
    public partial class FormDetalleEntidadFinanciera : FormTraducibleBase
    {
        private readonly int _entidadFinancieraId;
        private readonly EntidadFinancieraBLL _entidadFinancieraBLL = CompositionRoot.EntidadFinanciera;
        private readonly MedioPagoBLL _medioPagoBLL = CompositionRoot.MedioPago;

        public FormDetalleEntidadFinanciera(int entidadFinancieraId, string nombreEntidad)
        {
            _entidadFinancieraId = entidadFinancieraId;
            InitializeComponent();

            // El nombre es dato fijo del constructor (igual criterio que FormDetalleComercio).
            lblEntidadFinancieraNombreDetalle.Text = string.IsNullOrWhiteSpace(nombreEntidad) ? "-" : nombreEntidad;

            Load += FormDetalleEntidadFinanciera_Load;
        }

        // View-model plano para la grilla. Tipo queda como enum y se traduce en CellFormatting.
        private sealed class FilaMedioPagoEntidad
        {
            public int Id { get; set; }
            public TipoMedioPago Tipo { get; set; }
            public string Marca { get; set; }
            public string Segmento { get; set; }
        }

        // Item del combo del catalogo global: el texto se arma traducido al mostrarse.
        private sealed class OpcionMedioPago
        {
            public OpcionMedioPago(MedioPago medioPago)
            {
                MedioPago = medioPago;
            }

            public MedioPago MedioPago { get; }

            public override string ToString()
            {
                string tipo = UiDialogHelper.Traducir(EtiquetasFinanzas.ClaveTipoMedioPago(MedioPago.Tipo));
                return string.IsNullOrWhiteSpace(MedioPago.Marca) ? tipo : string.Format("{0} {1}", tipo, MedioPago.Marca);
            }
        }

        private void FormDetalleEntidadFinanciera_Load(object sender, EventArgs e)
        {
            dgvMediosPagoEntidad.CellFormatting += dgvMediosPagoEntidad_CellFormatting;
            CargarCatalogoMediosPago();
            CargarMediosPagoEntidad(null);
        }

        private void CargarCatalogoMediosPago()
        {
            try
            {
                List<OpcionMedioPago> opciones = _medioPagoBLL.ObtenerTodos()
                    .OrderBy(item => item.Tipo)
                    .ThenBy(item => item.Marca)
                    .Select(item => new OpcionMedioPago(item))
                    .ToList();

                cmbMedioPagoEntidad.BeginUpdate();
                cmbMedioPagoEntidad.Items.Clear();
                cmbMedioPagoEntidad.Items.AddRange(opciones.Cast<object>().ToArray());
                cmbMedioPagoEntidad.SelectedIndex = -1;
                cmbMedioPagoEntidad.EndUpdate();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de medios de pago");
            }
        }

        private void CargarMediosPagoEntidad(int? medioPagoEntidadSeleccionadoId)
        {
            try
            {
                EntidadFinanciera entidad = _entidadFinancieraBLL.ObtenerPorId(_entidadFinancieraId);
                List<MedioPagoEntidad> mediosPagoEntidad = entidad?.MediosPagoEntidad ?? new List<MedioPagoEntidad>();

                dgvMediosPagoEntidad.DataSource = null;
                dgvMediosPagoEntidad.DataSource = mediosPagoEntidad
                    .Where(item => item.MedioPago != null)
                    .OrderBy(item => item.MedioPago.Tipo)
                    .ThenBy(item => item.MedioPago.Marca)
                    .ThenBy(item => item.Segmento)
                    .Select(item => new FilaMedioPagoEntidad
                    {
                        Id = item.Id,
                        Tipo = item.MedioPago.Tipo,
                        Marca = item.MedioPago.Marca,
                        Segmento = item.Segmento
                    })
                    .ToList();

                if (medioPagoEntidadSeleccionadoId.HasValue)
                {
                    SeleccionarFila<FilaMedioPagoEntidad>(dgvMediosPagoEntidad, fila => fila.Id == medioPagoEntidadSeleccionadoId.Value);
                }
                else
                {
                    dgvMediosPagoEntidad.ClearSelection();
                    dgvMediosPagoEntidad.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de medios de pago de la entidad");
            }
        }

        private void btnAgregarMedioPagoEntidad_Click(object sender, EventArgs e)
        {
            if (!(cmbMedioPagoEntidad.SelectedItem is OpcionMedioPago opcion))
            {
                UiDialogHelper.MostrarWarning("FormFinancialEntityDetail.Warning.SelectMedioPago");
                return;
            }

            try
            {
                MedioPagoEntidad creado = _entidadFinancieraBLL.AgregarMedioPagoEntidad(
                    _entidadFinancieraId,
                    opcion.MedioPago.Id,
                    txtbSegmentoMedioPagoEntidad.Text);
                cmbMedioPagoEntidad.SelectedIndex = -1;
                txtbSegmentoMedioPagoEntidad.Clear();
                CargarMediosPagoEntidad(creado.Id);
                UiDialogHelper.MostrarInfo("FormFinancialEntityDetail.Success.Added");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de medio de pago de la entidad");
            }
        }

        private void btnQuitarMedioPagoEntidad_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvMediosPagoEntidad, out FilaMedioPagoEntidad fila))
            {
                UiDialogHelper.MostrarWarning("FormFinancialEntityDetail.Warning.SelectMedioPagoEntidad");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormFinancialEntityDetail.Confirm.RemoveMessage",
                "FormFinancialEntityDetail.Confirm.RemoveTitle",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool quitado = _entidadFinancieraBLL.QuitarMedioPagoEntidad(fila.Id);
                if (quitado)
                {
                    CargarMediosPagoEntidad(null);
                    UiDialogHelper.MostrarInfo("FormFinancialEntityDetail.Success.Removed");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de medio de pago de la entidad");
            }
        }

        private void dgvMediosPagoEntidad_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            // Los items del combo arman su texto traducido en ToString(): se re-agregan para
            // que el combo lo recalcule ante un cambio de idioma en caliente.
            int seleccion = cmbMedioPagoEntidad.SelectedIndex;
            object[] items = cmbMedioPagoEntidad.Items.Cast<object>().ToArray();
            cmbMedioPagoEntidad.BeginUpdate();
            cmbMedioPagoEntidad.Items.Clear();
            cmbMedioPagoEntidad.Items.AddRange(items);
            cmbMedioPagoEntidad.SelectedIndex = seleccion < items.Length ? seleccion : -1;
            cmbMedioPagoEntidad.EndUpdate();
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
