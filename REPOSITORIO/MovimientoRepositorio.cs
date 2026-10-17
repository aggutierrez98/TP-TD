using System;
using System.Collections.Generic;
using ABSTRACCIONES;
using BE;
using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class MovimientoRepositorio : IMovimientoRepositorio
    {
        private readonly MovimientoDAL _movimientoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public MovimientoRepositorio()
            : this(null)
        {
        }

        internal MovimientoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _movimientoDAL = new MovimientoDAL();
            _accesoDatos = accesoDatos;
        }

        public Movimiento ObtenerPorId(int id)
        {
            Movimiento movimiento = _movimientoDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(movimiento);
            return movimiento;
        }

        public List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro)
        {
            List<Movimiento> movimientos = _movimientoDAL.ObtenerPorUsuario(usuarioId, filtro, _accesoDatos);
            movimientos.ForEach(Hidratar);
            return movimientos;
        }

        public List<TotalPorCategoria> ObtenerTotalesPorCategoria(int usuarioId, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorCategoria(usuarioId, tipo, desde, hasta, _accesoDatos);
        }

        public List<TotalPorComercio> ObtenerTotalesPorComercio(int usuarioId, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorComercio(usuarioId, tipo, desde, hasta, _accesoDatos);
        }

        public List<TotalPorDia> ObtenerTotalesPorDia(int usuarioId, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorDia(usuarioId, tipo, desde, hasta, _accesoDatos);
        }

        public List<TotalPorMes> ObtenerTotalesPorMes(int usuarioId, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorMes(usuarioId, desde, hasta, _accesoDatos);
        }

        public ResumenMovimientos ObtenerResumen(int usuarioId, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerResumen(usuarioId, desde, hasta, _accesoDatos);
        }

        private void Hidratar(Movimiento movimiento)
        {
            if (movimiento == null)
            {
                return;
            }

            movimiento.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(movimiento.Usuario.Id);
            movimiento.Categoria = new CategoriaRepositorio(_accesoDatos).ObtenerPorId(movimiento.Categoria.Id);
            if (movimiento.Comercio != null && movimiento.Comercio.Id > 0)
            {
                movimiento.Comercio = new ComercioRepositorio(_accesoDatos).ObtenerPorId(movimiento.Comercio.Id);
            }
        }

        public int InsertarYObtenerId(Movimiento movimiento)
        {
            return _movimientoDAL.InsertarYObtenerId(movimiento, _accesoDatos);
        }

        public bool Actualizar(Movimiento movimiento)
        {
            return _movimientoDAL.Actualizar(movimiento, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _movimientoDAL.Eliminar(id, _accesoDatos);
        }

        public int ContarPorComercio(int comercioId)
        {
            return _movimientoDAL.ContarPorComercio(comercioId, _accesoDatos);
        }
    }
}
