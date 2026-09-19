using System;
using System.Collections.Generic;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class MovimientoBLL
    {
        private readonly IMovimientoRepositorio _movimientoRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal MovimientoBLL(
            IMovimientoRepositorio movimientoRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _movimientoRepositorio = movimientoRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro)
        {
            return _movimientoRepositorio.ObtenerPorUsuario(usuarioId, filtro);
        }

        public int Crear(int categoriaId, TipoMovimiento tipo, decimal monto, DateTime fecha, string descripcion)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            ValidarMonto(monto);
            ValidarCategoriaVisible(categoriaId, usuarioActualId);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Movimiento nuevoMovimiento = new Movimiento
                    {
                        Usuario = new Usuario { Id = usuarioActualId },
                        Categoria = new Categoria { Id = categoriaId },
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

        public bool Actualizar(int id, int categoriaId, TipoMovimiento tipo, decimal monto, DateTime fecha, string descripcion)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            ValidarMonto(monto);
            ValidarCategoriaVisible(categoriaId, usuarioActualId);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Movimiento movimiento = unidad.Movimientos.ObtenerPorId(id);
                    ValidarPropiedad(movimiento, usuarioActualId);

                    movimiento.Categoria = new Categoria { Id = categoriaId };
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
    }
}
