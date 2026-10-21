using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Orquesta una corrida de actualizacion de tasas. La usan por igual el ejecutable
    // programado (origen Programada) y el boton "Actualizar ahora" (origen Manual): una
    // sola consulta a la API en base USD, cruces calculados localmente solo para los pares
    // existentes, y un registro en el historial de cada corrida, tambien de las omitidas
    // por el tope horario y de las fallidas. Sin reintentos: una falla espera a la
    // siguiente corrida. El tope se verifica dentro de la unidad de trabajo, con bloqueo
    // sobre la ultima ejecucion exitosa, para que dos procesos no consulten a la vez.
    public class ActualizacionTasasBLL
    {
        private const int DecimalesDelValor = 6;

        private static readonly TimeSpan TopeConsulta = TimeSpan.FromMinutes(60);

        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IProveedorTasasCambio _proveedorTasasCambio;
        private readonly Func<DateTime> _reloj;

        internal ActualizacionTasasBLL(
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IProveedorTasasCambio proveedorTasasCambio,
            Func<DateTime> reloj)
        {
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _proveedorTasasCambio = proveedorTasasCambio;
            _reloj = reloj;
        }

        // Limite mensual del plan gratuito de Open Exchange Rates; la pantalla lo muestra junto al consumo.
        public const int LimiteMensualDeRequests = 1000;

        // Consultas de solo lectura para la pantalla de historial. Las ejecuciones vienen de la mas nueva a la mas antigua.
        public List<EjecucionActualizacionTasas> ObtenerUltimasEjecuciones(int cantidad)
        {
            return Leer(unidad => unidad.EjecucionesActualizacionTasas.ObtenerRecientes(cantidad));
        }

        public List<ActualizacionTasaDetalle> ObtenerDetalleDeEjecucion(EjecucionActualizacionTasas ejecucion)
        {
            return Leer(unidad => unidad.EjecucionesActualizacionTasas.ObtenerDetallePorEjecucion(ejecucion));
        }

        // Requests consumidas en el mes calendario del reloj inyectado, exitosas o fallidas (estimacion: la API puede facturar con otro corte).
        public int ObtenerRequestsConsumidasDelMes()
        {
            return Leer(unidad => unidad.EjecucionesActualizacionTasas.ContarRequestsDelMes(_reloj()));
        }

        private T Leer<T>(Func<IUnidadDeTrabajo, T> consulta)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    return consulta(unidad);
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.TasasCambio.HistoryReadFailed", ex);
                }
            }
        }

        public async Task<ResultadoEjecucionActualizacionTasas> EjecutarAsync(OrigenActualizacionTasas origen, CancellationToken cancellationToken)
        {
            DateTime inicio = _reloj();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    EjecucionActualizacionTasas ultimaExitosa = unidad.EjecucionesActualizacionTasas.ObtenerUltimaExitosaConBloqueo();
                    if (ultimaExitosa != null && inicio - ultimaExitosa.FechaInicio < TopeConsulta)
                    {
                        EjecucionActualizacionTasas omitida = NuevaEjecucion(inicio, origen, ResultadoActualizacionTasas.Omitida, false, "Info.TasasCambio.OmitidaPorTope");
                        return Guardar(unidad, omitida, ultimaExitosa.FechaInicio + TopeConsulta);
                    }

                    ITasasCambioObtenidas obtenidas;
                    try
                    {
                        obtenidas = await _proveedorTasasCambio.ObtenerTasasAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        Guardar(unidad, NuevaEjecucion(inicio, origen, ResultadoActualizacionTasas.Fallida, true, "Error.TasasCambio.Cancelled"), null);
                        throw;
                    }
                    catch (ProveedorTasasException ex)
                    {
                        bool consumio = ex.Estado != EstadoProveedorTasas.ClaveFaltante;
                        return Guardar(unidad, NuevaEjecucion(inicio, origen, ResultadoActualizacionTasas.Fallida, consumio, ex.Message), null);
                    }
                    catch (Exception)
                    {
                        return Guardar(unidad, NuevaEjecucion(inicio, origen, ResultadoActualizacionTasas.Fallida, true, "Error.TasasCambio.Unexpected"), null);
                    }

                    if (obtenidas == null || obtenidas.Tasas == null)
                    {
                        return Guardar(unidad, NuevaEjecucion(inicio, origen, ResultadoActualizacionTasas.Fallida, true, "Error.TasasCambio.Provider." + EstadoProveedorTasas.RespuestaInvalida), null);
                    }

                    EjecucionActualizacionTasas exitosa = NuevaEjecucion(inicio, origen, ResultadoActualizacionTasas.Exitosa, true, null);
                    ActualizarPares(unidad, obtenidas, exitosa);
                    return Guardar(unidad, exitosa, null);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.TasasCambio.RunFailed", ex);
                }
            }
        }

        private static EjecucionActualizacionTasas NuevaEjecucion(DateTime inicio, OrigenActualizacionTasas origen, ResultadoActualizacionTasas resultado, bool consumioRequest, string mensaje)
        {
            return new EjecucionActualizacionTasas
            {
                FechaInicio = inicio,
                Origen = origen,
                Resultado = resultado,
                ConsumioRequest = consumioRequest,
                Mensaje = mensaje
            };
        }

        private static void ActualizarPares(IUnidadDeTrabajo unidad, ITasasCambioObtenidas obtenidas, EjecucionActualizacionTasas ejecucion)
        {
            Dictionary<string, decimal> cotizaciones = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, decimal> cotizacion in obtenidas.Tasas)
            {
                cotizaciones[cotizacion.Key] = cotizacion.Value;
            }

            foreach (TasaConversion tasa in unidad.TasasConversion.ObtenerTodas())
            {
                decimal valorAnterior = tasa.CalculoConversion;
                ActualizacionTasaDetalle detalle = new ActualizacionTasaDetalle
                {
                    MonedaBase = tasa.MonedaBase,
                    MonedaConversion = tasa.MonedaConversion,
                    ValorAnterior = valorAnterior
                };

                if (tasa.EsManual)
                {
                    detalle.Estado = EstadoActualizacionTasa.SaltadoManual;
                }
                else
                {
                    decimal? valorNuevo = CalcularCruce(tasa, obtenidas.MonedaBase, cotizaciones);
                    if (!valorNuevo.HasValue)
                    {
                        detalle.Estado = EstadoActualizacionTasa.SaltadoSinDato;
                    }
                    else if (unidad.TasasConversion.ActualizarValorAutomatico(tasa, valorNuevo.Value))
                    {
                        detalle.Estado = EstadoActualizacionTasa.Actualizado;
                        detalle.ValorNuevo = valorNuevo.Value;
                    }
                    else
                    {
                        // El operador la edito a mano despues de leerla: la escritura la rechazo.
                        detalle.Estado = EstadoActualizacionTasa.SaltadoManual;
                    }
                }

                ejecucion.Detalles.Add(detalle);
            }
        }

        // 1 unidad de la base = cotizacion(conversion) / cotizacion(base) unidades de la
        // conversion, con cotizaciones expresadas en la moneda base del proveedor (que vale 1).
        private static decimal? CalcularCruce(TasaConversion tasa, string monedaBaseProveedor, Dictionary<string, decimal> cotizaciones)
        {
            decimal? cotizacionBase = Cotizar(tasa.MonedaBase.Codigo, monedaBaseProveedor, cotizaciones);
            decimal? cotizacionConversion = Cotizar(tasa.MonedaConversion.Codigo, monedaBaseProveedor, cotizaciones);
            if (!cotizacionBase.HasValue || !cotizacionConversion.HasValue)
            {
                return null;
            }

            decimal valor = Math.Round(cotizacionConversion.Value / cotizacionBase.Value, DecimalesDelValor, MidpointRounding.AwayFromZero);
            return valor > 0 ? valor : (decimal?)null;
        }

        private static decimal? Cotizar(string codigo, string monedaBaseProveedor, Dictionary<string, decimal> cotizaciones)
        {
            if (string.Equals(codigo, monedaBaseProveedor, StringComparison.OrdinalIgnoreCase))
            {
                return 1m;
            }

            decimal cotizacion;
            if (codigo != null && cotizaciones.TryGetValue(codigo, out cotizacion) && cotizacion > 0)
            {
                return cotizacion;
            }

            return null;
        }

        private static ResultadoEjecucionActualizacionTasas Guardar(IUnidadDeTrabajo unidad, EjecucionActualizacionTasas ejecucion, DateTime? proximaConsultaPermitida)
        {
            ejecucion.Id = unidad.EjecucionesActualizacionTasas.InsertarYObtenerId(ejecucion);
            foreach (ActualizacionTasaDetalle detalle in ejecucion.Detalles)
            {
                detalle.Ejecucion = ejecucion;
                detalle.Id = unidad.EjecucionesActualizacionTasas.InsertarDetalleYObtenerId(detalle);
            }

            unidad.Save();
            return new ResultadoEjecucionActualizacionTasas
            {
                Ejecucion = ejecucion,
                ProximaConsultaPermitida = proximaConsultaPermitida
            };
        }
    }
}
