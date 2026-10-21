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
    // Lista personal: las cuentas financieras del usuario de la sesion (el scope lo resuelve
    // CuentaFinancieraBLL). Una cuenta se crea (entidad + alias + tipo + medios de pago
    // contratados del catalogo de esa entidad, en una sola transaccion), se edita o se elimina.
    // Seleccionar una fila carga sus datos en el formulario; Actualizar los aplica a esa cuenta
    // y Agregar siempre crea una cuenta nueva. CredencialesCifradas queda fuera de alcance
    // (no se muestra ni se carga).
    public partial class FormGestionCuentasFinancieras : FormTraducibleBase
    {
        private readonly CuentaFinancieraBLL cuentaFinancieraBLL = CompositionRoot.CuentaFinanciera;
        private readonly EntidadFinancieraBLL entidadFinancieraBLL = CompositionRoot.EntidadFinanciera;

        // Evita recargar el catalogo cuando el combo de entidad se mueve por codigo.
        private bool _suspenderCambioEntidad;

        // Evita cargar el formulario mientras se rearma la grilla (el rebind selecciona la
        // primera fila sin que el usuario haya elegido nada).
        private bool _cargandoGrilla;

        public FormGestionCuentasFinancieras()
        {
            InitializeComponent();
            Load += FormGestionCuentasFinancieras_Load;
        }

        // View-model plano para la grilla. Tipo y MediosPago se formatean en CellFormatting
        // para que se re-traduzcan solos al cambiar el idioma.
        private sealed class FilaCuentaFinanciera
        {
            public CuentaFinanciera Cuenta { get; set; }
            public int Id { get; set; }
            public string Alias { get; set; }
            public int EntidadId { get; set; }
            public EntidadFinanciera EntidadFinanciera { get; set; }
            public string Entidad { get; set; }
            public TipoCuentaFinanciera Tipo { get; set; }
            public List<MedioPagoEntidad> MediosPago { get; set; }
        }

        // Item del selector de medios contratados: texto traducido armado al mostrarse.
        private sealed class OpcionMedioPagoEntidad
        {
            public OpcionMedioPagoEntidad(MedioPagoEntidad medioPagoEntidad)
            {
                MedioPagoEntidad = medioPagoEntidad;
            }

            public MedioPagoEntidad MedioPagoEntidad { get; }

            public override string ToString()
            {
                return DescribirMedioPagoEntidad(MedioPagoEntidad);
            }
        }

        private void FormGestionCuentasFinancieras_Load(object sender, EventArgs e)
        {
            dgvCuentasFinancieras.CellFormatting += dgvCuentasFinancieras_CellFormatting;
            CargarTipos();
            CargarEntidades();
            CargarCuentas(null);
            LimpiarFormulario();
        }

        private void CargarTipos()
        {
            cmbTipoCuentaFinanciera.DisplayMember = "Texto";
            cmbTipoCuentaFinanciera.Items.Clear();
            foreach (TipoCuentaFinanciera tipo in EtiquetasFinanzas.TiposCuentaFinanciera)
            {
                cmbTipoCuentaFinanciera.Items.Add(new OpcionTraducible(EtiquetasFinanzas.ClaveTipoCuentaFinanciera(tipo), tipo));
            }

            cmbTipoCuentaFinanciera.SelectedIndex = -1;
        }

        private void CargarEntidades()
        {
            try
            {
                List<EntidadFinanciera> entidades = entidadFinancieraBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .ToList();

                _suspenderCambioEntidad = true;
                try
                {
                    cmbEntidadFinancieraCuenta.DisplayMember = "Nombre";
                    cmbEntidadFinancieraCuenta.Items.Clear();
                    cmbEntidadFinancieraCuenta.Items.AddRange(entidades.Cast<object>().ToArray());
                    cmbEntidadFinancieraCuenta.SelectedIndex = -1;
                }
                finally
                {
                    _suspenderCambioEntidad = false;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de entidades financieras");
            }
        }

        private void CargarCuentas(int? cuentaSeleccionadaId)
        {
            try
            {
                List<CuentaFinanciera> cuentas = cuentaFinancieraBLL.ObtenerMisCuentas();

                _cargandoGrilla = true;
                dgvCuentasFinancieras.DataSource = null;
                dgvCuentasFinancieras.DataSource = cuentas
                    .OrderBy(item => item.Alias)
                    .Select(ProyectarFilaCuenta)
                    .ToList();

                if (cuentaSeleccionadaId.HasValue)
                {
                    SeleccionarFila<FilaCuentaFinanciera>(dgvCuentasFinancieras, fila => fila.Id == cuentaSeleccionadaId.Value);
                }
                else
                {
                    dgvCuentasFinancieras.ClearSelection();
                    dgvCuentasFinancieras.CurrentCell = null;
                }

                _cargandoGrilla = false;
                CargarFormularioDesdeSeleccion();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de cuentas financieras");
            }
            finally
            {
                _cargandoGrilla = false;
            }
        }

        private void dgvCuentasFinancieras_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoGrilla)
            {
                return;
            }

            CargarFormularioDesdeSeleccion();
        }

        // Entidad (sin disparar la recarga del catalogo), alias, tipo y medios contratados
        // marcados dentro del catalogo de la entidad de la cuenta seleccionada.
        private void CargarFormularioDesdeSeleccion()
        {
            if (dgvCuentasFinancieras.SelectedRows.Count == 0
                || !ObtenerFilaSeleccionada(dgvCuentasFinancieras, out FilaCuentaFinanciera fila))
            {
                return;
            }

            try
            {
                List<MedioPagoEntidad> catalogo = fila.EntidadFinanciera == null
                    ? new List<MedioPagoEntidad>()
                    : entidadFinancieraBLL.ObtenerMediosPagoEntidad(fila.EntidadFinanciera);

                _suspenderCambioEntidad = true;
                try
                {
                    cmbEntidadFinancieraCuenta.SelectedItem = cmbEntidadFinancieraCuenta.Items
                        .OfType<EntidadFinanciera>()
                        .FirstOrDefault(item => item.Id == fila.EntidadId);
                }
                finally
                {
                    _suspenderCambioEntidad = false;
                }

                txtbAliasCuenta.Text = fila.Alias;
                cmbTipoCuentaFinanciera.SelectedItem = cmbTipoCuentaFinanciera.Items
                    .OfType<OpcionTraducible>()
                    .FirstOrDefault(item => item.Valor is TipoCuentaFinanciera tipo && tipo == fila.Tipo);

                CargarSelectorMediosPago(
                    catalogo,
                    new HashSet<int>(fila.MediosPago.Select(item => item.Id)));
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de la cuenta financiera seleccionada");
            }
        }

        private static FilaCuentaFinanciera ProyectarFilaCuenta(CuentaFinanciera cuenta)
        {
            return new FilaCuentaFinanciera
            {
                Cuenta = cuenta,
                Id = cuenta.Id,
                Alias = cuenta.Alias,
                EntidadId = cuenta.EntidadFinanciera?.Id ?? 0,
                EntidadFinanciera = cuenta.EntidadFinanciera,
                Entidad = cuenta.EntidadFinanciera?.Nombre ?? string.Empty,
                Tipo = cuenta.Tipo,
                MediosPago = cuenta.MediosPagoEntidad ?? new List<MedioPagoEntidad>()
            };
        }

        private void LimpiarFormulario()
        {
            _suspenderCambioEntidad = true;
            try
            {
                cmbEntidadFinancieraCuenta.SelectedIndex = -1;
            }
            finally
            {
                _suspenderCambioEntidad = false;
            }

            txtbAliasCuenta.Clear();
            cmbTipoCuentaFinanciera.SelectedIndex = -1;
            clbMediosPagoContratados.Items.Clear();
        }

        private void cmbEntidadFinancieraCuenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suspenderCambioEntidad)
            {
                return;
            }

            if (!(cmbEntidadFinancieraCuenta.SelectedItem is EntidadFinanciera entidadSeleccionada))
            {
                clbMediosPagoContratados.Items.Clear();
                return;
            }

            try
            {
                // La cuenta todavia no existe: el catalogo sale de la entidad elegida.
                CargarSelectorMediosPago(entidadFinancieraBLL.ObtenerMediosPagoEntidad(entidadSeleccionada), null);
            }
            catch (Exception ex)
            {
                clbMediosPagoContratados.Items.Clear();
                UiExceptionHandler.Mostrar(ex, "Carga de medios de pago de la entidad");
            }
        }

        private void CargarSelectorMediosPago(List<MedioPagoEntidad> catalogo, HashSet<int> contratadosIds)
        {
            clbMediosPagoContratados.BeginUpdate();
            clbMediosPagoContratados.Items.Clear();
            foreach (MedioPagoEntidad medioPagoEntidad in catalogo
                .Where(item => item.MedioPago != null)
                .OrderBy(item => item.MedioPago.Tipo)
                .ThenBy(item => item.MedioPago.Marca)
                .ThenBy(item => item.Segmento))
            {
                bool contratado = contratadosIds != null && contratadosIds.Contains(medioPagoEntidad.Id);
                clbMediosPagoContratados.Items.Add(new OpcionMedioPagoEntidad(medioPagoEntidad), contratado);
            }

            clbMediosPagoContratados.EndUpdate();
        }

        private List<MedioPagoEntidad> ObtenerMediosPagoMarcados()
        {
            return clbMediosPagoContratados.CheckedItems
                .OfType<OpcionMedioPagoEntidad>()
                .Select(item => item.MedioPagoEntidad)
                .ToList();
        }

        // --- Acciones ---

        private void btnAgregarCuenta_Click(object sender, EventArgs e)
        {
            if (!(cmbEntidadFinancieraCuenta.SelectedItem is EntidadFinanciera entidad))
            {
                UiDialogHelper.MostrarWarning("FormFinancialAccountManagement.Warning.SelectEntity");
                return;
            }

            if (!ObtenerTipoSeleccionado(out TipoCuentaFinanciera tipo))
            {
                UiDialogHelper.MostrarWarning("FormFinancialAccountManagement.Warning.SelectType");
                return;
            }

            try
            {
                CuentaFinanciera creada = cuentaFinancieraBLL.Crear(new CuentaFinanciera
                {
                    EntidadFinanciera = entidad, Alias = txtbAliasCuenta.Text, Tipo = tipo,
                    MediosPagoEntidad = ObtenerMediosPagoMarcados()
                });
                LimpiarFormulario();
                CargarCuentas(creada.Id);
                UiDialogHelper.MostrarInfo("FormFinancialAccountManagement.Success.Created");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de cuenta financiera");
            }
        }

        private void btnActualizarCuenta_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvCuentasFinancieras, out FilaCuentaFinanciera fila))
            {
                UiDialogHelper.MostrarWarning("FormFinancialAccountManagement.Warning.SelectAccount");
                return;
            }

            if (!(cmbEntidadFinancieraCuenta.SelectedItem is EntidadFinanciera entidad))
            {
                UiDialogHelper.MostrarWarning("FormFinancialAccountManagement.Warning.SelectEntity");
                return;
            }

            if (!ObtenerTipoSeleccionado(out TipoCuentaFinanciera tipo))
            {
                UiDialogHelper.MostrarWarning("FormFinancialAccountManagement.Warning.SelectType");
                return;
            }

            try
            {
                fila.Cuenta.EntidadFinanciera = entidad;
                fila.Cuenta.Alias = txtbAliasCuenta.Text;
                fila.Cuenta.Tipo = tipo;
                fila.Cuenta.MediosPagoEntidad = ObtenerMediosPagoMarcados();
                CuentaFinanciera actualizada = cuentaFinancieraBLL.Actualizar(fila.Cuenta);
                CargarCuentas(actualizada.Id);
                UiDialogHelper.MostrarInfo("FormFinancialAccountManagement.Success.Updated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Modificacion de cuenta financiera");
            }
        }

        private void btnEliminarCuenta_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvCuentasFinancieras, out FilaCuentaFinanciera fila))
            {
                UiDialogHelper.MostrarWarning("FormFinancialAccountManagement.Warning.SelectAccount");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormFinancialAccountManagement.Confirm.DeleteMessage",
                "FormFinancialAccountManagement.Confirm.DeleteTitle",
                MessageBoxIcon.Warning,
                fila.Alias);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminada = cuentaFinancieraBLL.Eliminar(fila.Cuenta);
                if (eliminada)
                {
                    CargarCuentas(null);
                    LimpiarFormulario();
                    UiDialogHelper.MostrarInfo("FormFinancialAccountManagement.Success.Deleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de cuenta financiera");
            }
        }

        private void dgvCuentasFinancieras_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null)
            {
                return;
            }

            if (e.Value is TipoCuentaFinanciera tipo)
            {
                e.Value = Traducir(EtiquetasFinanzas.ClaveTipoCuentaFinanciera(tipo));
                e.FormattingApplied = true;
                return;
            }

            if (e.Value is List<MedioPagoEntidad> mediosPago)
            {
                e.Value = string.Join(", ", mediosPago.Select(DescribirMedioPagoEntidad));
                e.FormattingApplied = true;
            }
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);

            // Los items del selector arman su texto traducido en ToString(): se re-agregan
            // conservando las marcas para que se recalcule ante un cambio de idioma en caliente.
            List<object> items = clbMediosPagoContratados.Items.Cast<object>().ToList();
            HashSet<object> marcados = new HashSet<object>(clbMediosPagoContratados.CheckedItems.Cast<object>());
            clbMediosPagoContratados.BeginUpdate();
            clbMediosPagoContratados.Items.Clear();
            foreach (object item in items)
            {
                clbMediosPagoContratados.Items.Add(item, marcados.Contains(item));
            }

            clbMediosPagoContratados.EndUpdate();
        }

        // --- Helpers ---

        private static string DescribirMedioPagoEntidad(MedioPagoEntidad medioPagoEntidad)
        {
            if (medioPagoEntidad?.MedioPago == null)
            {
                return string.Empty;
            }

            string texto = Traducir(EtiquetasFinanzas.ClaveTipoMedioPago(medioPagoEntidad.MedioPago.Tipo));
            if (!string.IsNullOrWhiteSpace(medioPagoEntidad.MedioPago.Marca))
            {
                texto = string.Format("{0} {1}", texto, medioPagoEntidad.MedioPago.Marca);
            }

            if (!string.IsNullOrWhiteSpace(medioPagoEntidad.Segmento))
            {
                texto = string.Format("{0} ({1})", texto, medioPagoEntidad.Segmento);
            }

            return texto;
        }

        private static string Traducir(string key)
        {
            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            return idioma == null ? key : idioma.ObtenerTraduccion(key);
        }

        private bool ObtenerTipoSeleccionado(out TipoCuentaFinanciera tipo)
        {
            if (cmbTipoCuentaFinanciera.SelectedItem is OpcionTraducible opcion && opcion.Valor is TipoCuentaFinanciera valor)
            {
                tipo = valor;
                return true;
            }

            tipo = default(TipoCuentaFinanciera);
            return false;
        }

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
