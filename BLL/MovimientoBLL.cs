using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Resultado de EscanearBorradorAsync: el Movimiento en memoria mas el texto
    // crudo detectado, para que la UI lo muestre si algun campo no se pudo parsear.
    public class EscaneoBorradorResultado
    {
        public Movimiento Borrador { get; set; }

        public string TextoCrudo { get; set; }
    }

    public class MovimientoBLL
    {
        private readonly IMovimientoRepositorio _movimientoRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IComercioRepositorio _comercioRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;
        private readonly IEscaneoTicketService _escaneoTicketService;
        private readonly IReporteTabularPdfService _reportePdfService;

        internal MovimientoBLL(
            IMovimientoRepositorio movimientoRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IComercioRepositorio comercioRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL,
            IEscaneoTicketService escaneoTicketService,
            IReporteTabularPdfService reportePdfService)
        {
            _movimientoRepositorio = movimientoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _comercioRepositorio = comercioRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
            _escaneoTicketService = escaneoTicketService;
            _reportePdfService = reportePdfService;
        }

        public byte[] ExportarPdf(ReporteMovimientosPdf reporte)
        {
            if (reporte == null || !_sesion.HaySesionActiva() || reporte.Usuario == null || reporte.Usuario.Id != _sesion.ObtenerUsuarioActual().Id)
            {
                throw new ExcepcionNegocio("Error.Movement.ExportUserMismatch");
            }
            if (reporte.Filas == null || reporte.Filas.Count == 0)
            {
                throw new ExcepcionNegocio("Error.Movement.ExportEmpty");
            }
            if (reporte.Encabezados == null || reporte.Encabezados.Count != 6 ||
                reporte.Filas.Any(fila => fila == null || fila.Count != 6))
            {
                throw new ExcepcionNegocio("Error.Movement.ExportInvalidColumns");
            }

            try
            {
                return _reportePdfService.Generar(reporte);
            }
            catch (ExcepcionTecnica) { throw; }
            catch (Exception ex)
            {
                throw new ExcepcionTecnica("Error.Movement.ExportFailed", ex);
            }
        }

        public List<Movimiento> ObtenerParaUsuarioActual(FiltroMovimiento filtro)
        {
            return _movimientoRepositorio.ObtenerPorUsuario(_sesion.ObtenerUsuarioActual(), filtro);
        }

        public List<TotalPorCategoria> ObtenerTotalesPorCategoria(TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            return _movimientoRepositorio.ObtenerTotalesPorCategoria(_sesion.ObtenerUsuarioActual(), tipo, desde, hasta);
        }

        // Egresos del rango por comercio: los de mayor gasto (hasta "cantidad") ordenados de mayor a menor;
        // lo gastado sin comercio asociado se informa aparte y no es un comercio mas.
        public GastoPorComercio ObtenerGastoPorComercio(DateTime desde, DateTime hasta, int cantidad)
        {
            List<TotalPorComercio> totales = _movimientoRepositorio.ObtenerTotalesPorComercio(_sesion.ObtenerUsuarioActual(), TipoMovimiento.Egreso, desde, hasta);

            return new GastoPorComercio
            {
                Comercios = totales
                    .Where(item => item.ComercioNombre != null)
                    .OrderByDescending(item => item.Total)
                    .Take(Math.Max(0, cantidad))
                    .ToList(),
                TotalSinComercio = totales.Where(item => item.ComercioNombre == null).Sum(item => item.Total)
            };
        }

        // Acumulado de egresos por dia del mes de "hoy" (hasta hoy) y del mes anterior (completo), con la
        // comparacion al mismo dia. Los dias sin egresos repiten el acumulado del dia anterior.
        public GastoAcumuladoMensual ObtenerGastoAcumuladoMensual(DateTime hoy)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();
            DateTime fecha = hoy.Date;
            DateTime inicioMesActual = new DateTime(fecha.Year, fecha.Month, 1);
            DateTime inicioMesAnterior = inicioMesActual.AddMonths(-1);
            int diasMesAnterior = DateTime.DaysInMonth(inicioMesAnterior.Year, inicioMesAnterior.Month);

            Dictionary<DateTime, decimal> totalPorDia = _movimientoRepositorio
                .ObtenerTotalesPorDia(usuarioActual, TipoMovimiento.Egreso, inicioMesAnterior, fecha)
                .GroupBy(item => item.Fecha.Date)
                .ToDictionary(grupo => grupo.Key, grupo => grupo.Sum(item => item.Total));

            List<decimal> mesActual = AcumularPorDia(inicioMesActual, fecha.Day, totalPorDia);
            List<decimal> mesAnterior = AcumularPorDia(inicioMesAnterior, diasMesAnterior, totalPorDia);

            decimal acumuladoActual = mesActual[mesActual.Count - 1];
            decimal anteriorAlMismoDia = mesAnterior[Math.Min(fecha.Day, diasMesAnterior) - 1];

            return new GastoAcumuladoMensual
            {
                MesActual = mesActual,
                MesAnterior = mesAnterior,
                AcumuladoActual = acumuladoActual,
                AcumuladoAnteriorAlMismoDia = anteriorAlMismoDia,
                VariacionPorcentual = anteriorAlMismoDia > 0
                    ? Math.Round((acumuladoActual - anteriorAlMismoDia) / anteriorAlMismoDia * 100m, 1)
                    : (decimal?)null
            };
        }

        private static List<decimal> AcumularPorDia(DateTime inicioMes, int dias, Dictionary<DateTime, decimal> totalPorDia)
        {
            List<decimal> acumulados = new List<decimal>(dias);
            decimal acumulado = 0m;
            for (int dia = 0; dia < dias; dia++)
            {
                if (totalPorDia.TryGetValue(inicioMes.AddDays(dia), out decimal total))
                {
                    acumulado += total;
                }

                acumulados.Add(acumulado);
            }

            return acumulados;
        }

        public List<TotalPorMes> ObtenerTotalesPorMes(DateTime desde, DateTime hasta)
        {
            return _movimientoRepositorio.ObtenerTotalesPorMes(_sesion.ObtenerUsuarioActual(), desde, hasta);
        }

        public ResumenMovimientos ObtenerResumenBalance(DateTime desde, DateTime hasta)
        {
            return _movimientoRepositorio.ObtenerResumen(_sesion.ObtenerUsuarioActual(), desde, hasta);
        }

        // Arma un Movimiento "borrador" a partir de una imagen de ticket escaneada.
        // Nunca lo persiste: el usuario revisa/corrige en la UI y confirma con el
        // alta normal (Crear). Categoria queda null a proposito, la elige el usuario.
        public async Task<EscaneoBorradorResultado> EscanearBorradorAsync(byte[] imagenBytes)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            try
            {
                ITicketEscaneado resultado = await _escaneoTicketService.EscanearTicketAsync(imagenBytes).ConfigureAwait(false);

                Movimiento borrador = new Movimiento
                {
                    Usuario = usuarioActual,
                    Categoria = null,
                    Tipo = TipoMovimiento.Egreso,
                    Monto = resultado.Monto ?? 0m,
                    Fecha = resultado.Fecha ?? DateTime.Today,
                    Descripcion = resultado.Descripcion
                };

                return new EscaneoBorradorResultado
                {
                    Borrador = borrador,
                    TextoCrudo = resultado.TextoCrudo
                };
            }
            catch (ExcepcionNegocio)
            {
                throw;
            }
            catch (ExcepcionTecnica)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExcepcionTecnica("Error.Ticket.ScanFailed", ex);
            }
        }

        // Devuelve la misma instancia recibida, con Id, Usuario (el de sesion) y la
        // categoria/comercio persistidos cargados.
        public Movimiento Crear(Movimiento movimiento)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();
            ValidarMonto(movimiento?.Monto ?? 0m);
            Categoria categoria = ValidarCategoriaVisible(movimiento.Categoria, usuarioActual);
            Comercio comercio = ValidarComercio(movimiento.Comercio);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    movimiento.Usuario = usuarioActual;
                    movimiento.Categoria = categoria;
                    movimiento.Comercio = comercio;
                    movimiento.Descripcion = NormalizarDescripcion(movimiento.Descripcion);

                    movimiento.Id = unidad.Movimientos.InsertarYObtenerId(movimiento);
                    _bitacoraBLL.RegistrarOperacionInterno("FormMovements.Value.Activity.MovementCreation", "FormMovements.Value.Info.MovementCreation", "MovimientoBLL", unidad);
                    unidad.Save();
                    return movimiento;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Movement.CreateFailed", ex);
                }
            }
        }

        // Recibe el movimiento con los valores nuevos. Valida la propiedad sobre el
        // persistido (no confia en el Usuario recibido) y solo copia los campos editables.
        public bool Actualizar(Movimiento movimiento)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();
            ValidarMonto(movimiento?.Monto ?? 0m);
            Categoria categoria = ValidarCategoriaVisible(movimiento.Categoria, usuarioActual);
            Comercio comercio = ValidarComercio(movimiento.Comercio);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Movimiento persistido = unidad.Movimientos.ObtenerPorId(movimiento.Id);
                    ValidarPropiedad(persistido, usuarioActual);

                    persistido.Categoria = categoria;
                    persistido.Comercio = comercio;
                    persistido.Tipo = movimiento.Tipo;
                    persistido.Monto = movimiento.Monto;
                    persistido.Fecha = movimiento.Fecha;
                    persistido.Descripcion = NormalizarDescripcion(movimiento.Descripcion);

                    bool actualizado = unidad.Movimientos.Actualizar(persistido);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.Movement.UpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMovements.Value.Activity.MovementUpdate", "FormMovements.Value.Info.MovementUpdate", "MovimientoBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Movement.UpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(Movimiento movimiento)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Movimiento persistido = movimiento == null ? null : unidad.Movimientos.ObtenerPorId(movimiento.Id);
                    ValidarPropiedad(persistido, usuarioActual);

                    bool eliminado = unidad.Movimientos.Eliminar(persistido);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.Movement.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMovements.Value.Activity.MovementDeletion", "FormMovements.Value.Info.MovementDeletion", "MovimientoBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Movement.DeleteFailed", ex);
                }
            }
        }

        private static string NormalizarDescripcion(string descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
        }

        private static void ValidarMonto(decimal monto)
        {
            if (monto <= 0)
            {
                throw new ExcepcionNegocio("Error.Movement.AmountMustBePositive");
            }
        }

        // La categoria debe existir y ser visible para el usuario actual
        // (global o propia); nunca la de otro usuario. Devuelve la persistida.
        private Categoria ValidarCategoriaVisible(Categoria recibida, Usuario usuarioActual)
        {
            Categoria categoria = recibida == null ? null : _categoriaRepositorio.ObtenerPorId(recibida.Id);
            if (categoria == null)
            {
                throw new ExcepcionNegocio("Error.Movement.CategoryNotFound");
            }

            if (categoria.Usuario != null && categoria.Usuario.Id != usuarioActual.Id)
            {
                throw new ExcepcionNegocio("Error.Movement.CategoryNotAllowed");
            }

            return categoria;
        }

        // Un movimiento solo puede ser editado/eliminado por su dueno.
        private static void ValidarPropiedad(Movimiento movimiento, Usuario usuarioActual)
        {
            if (movimiento == null)
            {
                throw new ExcepcionNegocio("Error.Movement.NotFound");
            }

            if (movimiento.Usuario.Id != usuarioActual.Id)
            {
                throw new ExcepcionNegocio("Error.Movement.NotOwned");
            }
        }

        // El comercio es opcional; si viene, debe existir. Devuelve el persistido.
        private Comercio ValidarComercio(Comercio recibido)
        {
            if (recibido == null)
            {
                return null;
            }

            Comercio comercio = _comercioRepositorio.ObtenerPorId(recibido.Id);
            if (comercio == null)
            {
                throw new ExcepcionNegocio("Error.Movement.CommerceNotFound");
            }

            return comercio;
        }
    }
}
