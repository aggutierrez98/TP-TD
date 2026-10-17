using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    public partial class FormActualizacionTasas : FormTraducibleBase
    {
        private const int CantidadEjecuciones = 50;

        private readonly ActualizacionTasasBLL actualizacionTasasBLL = CompositionRoot.ActualizacionTasas;
        private bool ocupado;

        public FormActualizacionTasas()
        {
            InitializeComponent();
            Load += FormActualizacionTasas_Load;
        }

        // View-models planos para bindear las grillas.
        private sealed class FilaEjecucion
        {
            public int Id { get; set; }
            public DateTime FechaInicio { get; set; }
            public OrigenActualizacionTasas Origen { get; set; }
            public ResultadoActualizacionTasas Resultado { get; set; }
            public bool ConsumioRequest { get; set; }
            public string Mensaje { get; set; }
        }

        private sealed class FilaDetalle
        {
            public string MonedaBase { get; set; }
            public string MonedaConversion { get; set; }
            public decimal ValorAnterior { get; set; }
            public decimal? ValorNuevo { get; set; }
            public EstadoActualizacionTasa Estado { get; set; }
        }

        private void FormActualizacionTasas_Load(object sender, EventArgs e)
        {
            MaximizarEnAreaMdi();
            Refrescar();
        }

        // El contador no tiene Tag (su texto lleva numeros): se rearma al cambiar el idioma.
        public override void Actualizar(ABSTRACCIONES.IObservado observado)
        {
            base.Actualizar(observado);
            ActualizarContador();
            dgvEjecucionesTasas.Invalidate();
            dgvDetalleEjecucionTasas.Invalidate();
        }

        private void Refrescar()
        {
            try
            {
                List<EjecucionActualizacionTasas> ejecuciones = actualizacionTasasBLL.ObtenerUltimasEjecuciones(CantidadEjecuciones);

                dgvEjecucionesTasas.DataSource = null;
                dgvEjecucionesTasas.DataSource = ejecuciones.Select(ProyectarFilaEjecucion).ToList();
                ActualizarContador();
                CargarDetalle();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de historial de tasas");
            }
        }

        private void ActualizarContador()
        {
            try
            {
                int usadas = actualizacionTasasBLL.ObtenerRequestsConsumidasDelMes();
                lblRequestsMesActualizacionTasas.Text = UiDialogHelper.Traducir(
                    "FormExchangeRateUpdate.RequestsCounter", usadas, ActualizacionTasasBLL.LimiteMensualDeRequests);
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de requests del mes");
            }
        }

        private void CargarDetalle()
        {
            FilaEjecucion seleccionada = dgvEjecucionesTasas.CurrentRow?.DataBoundItem as FilaEjecucion;
            if (seleccionada == null)
            {
                dgvDetalleEjecucionTasas.DataSource = null;
                return;
            }

            try
            {
                dgvDetalleEjecucionTasas.DataSource = null;
                dgvDetalleEjecucionTasas.DataSource = actualizacionTasasBLL.ObtenerDetalleDeEjecucion(seleccionada.Id)
                    .Select(ProyectarFilaDetalle)
                    .ToList();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de detalle de actualizacion de tasas");
            }
        }

        private void dgvEjecucionesTasas_SelectionChanged(object sender, EventArgs e)
        {
            CargarDetalle();
        }

        private async void btnActualizarAhoraTasas_Click(object sender, EventArgs e)
        {
            if (ocupado)
            {
                return;
            }

            CambiarOcupado(true);
            try
            {
                // Task.Run mantiene la UI libre durante la consulta HTTP y la escritura en la base.
                ResultadoEjecucionActualizacionTasas resultado = await Task.Run(
                    () => actualizacionTasasBLL.EjecutarAsync(OrigenActualizacionTasas.Manual, CancellationToken.None));
                if (IsDisposed || Disposing)
                {
                    return;
                }

                Refrescar();
                MostrarResultado(resultado);
            }
            catch (Exception ex)
            {
                if (!IsDisposed && !Disposing)
                {
                    UiExceptionHandler.Mostrar(ex, "Actualizacion manual de tasas");
                }
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    CambiarOcupado(false);
                }
            }
        }

        private static void MostrarResultado(ResultadoEjecucionActualizacionTasas resultado)
        {
            EjecucionActualizacionTasas ejecucion = resultado.Ejecucion;
            switch (ejecucion.Resultado)
            {
                case ResultadoActualizacionTasas.Exitosa:
                    UiDialogHelper.MostrarInfo(
                        "FormExchangeRateUpdate.Success.Updated",
                        ContarDetalles(ejecucion, EstadoActualizacionTasa.Actualizado),
                        ContarDetalles(ejecucion, EstadoActualizacionTasa.SaltadoManual),
                        ContarDetalles(ejecucion, EstadoActualizacionTasa.SaltadoSinDato));
                    break;
                case ResultadoActualizacionTasas.Omitida:
                    UiDialogHelper.MostrarInfo(
                        "FormExchangeRateUpdate.Info.Skipped",
                        resultado.ProximaConsultaPermitida.HasValue
                            ? resultado.ProximaConsultaPermitida.Value.ToString("g")
                            : string.Empty);
                    break;
                default:
                    UiDialogHelper.MostrarWarning(
                        "FormExchangeRateUpdate.Warning.RunFailed",
                        UiDialogHelper.TraducirSiExiste(ejecucion.Mensaje));
                    break;
            }
        }

        private static int ContarDetalles(EjecucionActualizacionTasas ejecucion, EstadoActualizacionTasa estado)
        {
            return ejecucion.Detalles.Count(detalle => detalle.Estado == estado);
        }

        private void CambiarOcupado(bool valor)
        {
            ocupado = valor;
            btnActualizarAhoraTasas.Enabled = !valor;
            UseWaitCursor = valor;
        }

        // Origen, Resultado y Estado guardan el codigo de dominio (enum): se traduce al pintar la
        // celda; Mensaje guarda la key cruda del BLL o del proveedor.
        private void dgvEjecucionesTasas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            switch (dgvEjecucionesTasas.Columns[e.ColumnIndex].Name)
            {
                case "Origen":
                    e.Value = UiDialogHelper.Traducir("FormExchangeRateUpdate.Value.Origin." + e.Value);
                    e.FormattingApplied = true;
                    break;
                case "Resultado":
                    e.Value = UiDialogHelper.Traducir("FormExchangeRateUpdate.Value.Result." + e.Value);
                    e.FormattingApplied = true;
                    break;
                case "Mensaje":
                    e.Value = UiDialogHelper.TraducirSiExiste(e.Value.ToString());
                    e.FormattingApplied = true;
                    break;
            }
        }

        private void dgvDetalleEjecucionTasas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            if (dgvDetalleEjecucionTasas.Columns[e.ColumnIndex].Name == "Estado")
            {
                e.Value = UiDialogHelper.Traducir("FormExchangeRateUpdate.Value.PairStatus." + e.Value);
                e.FormattingApplied = true;
            }
        }

        private static FilaEjecucion ProyectarFilaEjecucion(EjecucionActualizacionTasas ejecucion)
        {
            return new FilaEjecucion
            {
                Id = ejecucion.Id,
                FechaInicio = ejecucion.FechaInicio,
                Origen = ejecucion.Origen,
                Resultado = ejecucion.Resultado,
                ConsumioRequest = ejecucion.ConsumioRequest,
                Mensaje = ejecucion.Mensaje
            };
        }

        private static FilaDetalle ProyectarFilaDetalle(ActualizacionTasaDetalle detalle)
        {
            return new FilaDetalle
            {
                MonedaBase = detalle.MonedaBase?.Codigo,
                MonedaConversion = detalle.MonedaConversion?.Codigo,
                ValorAnterior = detalle.ValorAnterior,
                ValorNuevo = detalle.ValorNuevo,
                Estado = detalle.Estado
            };
        }
    }
}
