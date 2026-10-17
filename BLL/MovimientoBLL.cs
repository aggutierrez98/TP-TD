using System;
using System.Collections.Generic;
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

        internal MovimientoBLL(
            IMovimientoRepositorio movimientoRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IComercioRepositorio comercioRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL,
            IEscaneoTicketService escaneoTicketService)
        {
            _movimientoRepositorio = movimientoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _comercioRepositorio = comercioRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
            _escaneoTicketService = escaneoTicketService;
        }

        public List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro)
        {
            return _movimientoRepositorio.ObtenerPorUsuario(usuarioId, filtro);
        }

        public List<TotalPorCategoria> ObtenerTotalesPorCategoria(TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            return _movimientoRepositorio.ObtenerTotalesPorCategoria(usuarioActualId, tipo, desde, hasta);
        }

        public List<TotalPorMes> ObtenerTotalesPorMes(DateTime desde, DateTime hasta)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            return _movimientoRepositorio.ObtenerTotalesPorMes(usuarioActualId, desde, hasta);
        }

        public ResumenMovimientos ObtenerResumenBalance(DateTime desde, DateTime hasta)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            return _movimientoRepositorio.ObtenerResumen(usuarioActualId, desde, hasta);
        }

        // Arma un Movimiento "borrador" a partir de una imagen de ticket escaneada.
        // Nunca lo persiste: el usuario revisa/corrige en la UI y confirma con el
        // alta normal (Crear). Categoria queda null a proposito, la elige el usuario.
        public async Task<EscaneoBorradorResultado> EscanearBorradorAsync(byte[] imagenBytes)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            try
            {
                TicketEscaneado resultado = await _escaneoTicketService.EscanearTicketAsync(imagenBytes).ConfigureAwait(false);

                Movimiento borrador = new Movimiento
                {
                    Usuario = new Usuario { Id = usuarioActualId },
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

        public int Crear(int categoriaId, TipoMovimiento tipo, decimal monto, DateTime fecha, string descripcion, int? comercioId = null)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            ValidarMonto(monto);
            ValidarCategoriaVisible(categoriaId, usuarioActualId);
            ValidarComercio(comercioId);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Movimiento nuevoMovimiento = new Movimiento
                    {
                        Usuario = new Usuario { Id = usuarioActualId },
                        Categoria = new Categoria { Id = categoriaId },
                        Comercio = comercioId.HasValue ? new Comercio { Id = comercioId.Value } : null,
                        Tipo = tipo,
                        Monto = monto,
                        Fecha = fecha,
                        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim()
                    };

                    int id = unidad.Movimientos.InsertarYObtenerId(nuevoMovimiento);
                    _bitacoraBLL.RegistrarOperacionInterno("FormMovements.Value.Activity.MovementCreation", "FormMovements.Value.Info.MovementCreation", "MovimientoBLL", unidad);
                    unidad.Save();
                    return id;
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

        public bool Actualizar(int id, int categoriaId, TipoMovimiento tipo, decimal monto, DateTime fecha, string descripcion, int? comercioId = null)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            ValidarMonto(monto);
            ValidarCategoriaVisible(categoriaId, usuarioActualId);
            ValidarComercio(comercioId);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Movimiento movimiento = unidad.Movimientos.ObtenerPorId(id);
                    ValidarPropiedad(movimiento, usuarioActualId);

                    movimiento.Categoria = new Categoria { Id = categoriaId };
                    movimiento.Comercio = comercioId.HasValue ? new Comercio { Id = comercioId.Value } : null;
                    movimiento.Tipo = tipo;
                    movimiento.Monto = monto;
                    movimiento.Fecha = fecha;
                    movimiento.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();

                    bool actualizado = unidad.Movimientos.Actualizar(movimiento);
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

        public bool Eliminar(int id)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Movimiento movimiento = unidad.Movimientos.ObtenerPorId(id);
                    ValidarPropiedad(movimiento, usuarioActualId);

                    bool eliminado = unidad.Movimientos.Eliminar(id);
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

        private static void ValidarMonto(decimal monto)
        {
            if (monto <= 0)
            {
                throw new ExcepcionNegocio("Error.Movement.AmountMustBePositive");
            }
        }

        // La categoria debe existir y ser visible para el usuario actual
        // (global o propia); nunca la de otro usuario.
        private void ValidarCategoriaVisible(int categoriaId, int usuarioActualId)
        {
            Categoria categoria = _categoriaRepositorio.ObtenerPorId(categoriaId);
            if (categoria == null)
            {
                throw new ExcepcionNegocio("Error.Movement.CategoryNotFound");
            }

            if (categoria.Usuario != null && categoria.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.Movement.CategoryNotAllowed");
            }
        }

        // Un movimiento solo puede ser editado/eliminado por su dueno.
        private static void ValidarPropiedad(Movimiento movimiento, int usuarioActualId)
        {
            if (movimiento == null)
            {
                throw new ExcepcionNegocio("Error.Movement.NotFound");
            }

            if (movimiento.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.Movement.NotOwned");
            }
        }

        private void ValidarComercio(int? comercioId)
        {
            if (!comercioId.HasValue)
            {
                return;
            }

            Comercio comercio = _comercioRepositorio.ObtenerPorId(comercioId.Value);
            if (comercio == null)
            {
                throw new ExcepcionNegocio("Error.Movement.CommerceNotFound");
            }
        }
    }
}
