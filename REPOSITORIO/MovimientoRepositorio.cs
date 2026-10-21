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

        public List<Movimiento> ObtenerPorUsuario(Usuario usuario, FiltroMovimiento filtro)
        {
            List<Movimiento> movimientos = _movimientoDAL.ObtenerPorUsuario(usuario, filtro, _accesoDatos);
            movimientos.ForEach(Hidratar);
            return movimientos;
        }

        public List<TotalPorCategoria> ObtenerTotalesPorCategoria(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorCategoria(usuario, tipo, desde, hasta, _accesoDatos);
        }

        public List<TotalPorComercio> ObtenerTotalesPorComercio(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorComercio(usuario, tipo, desde, hasta, _accesoDatos);
        }

        public List<TotalPorDia> ObtenerTotalesPorDia(Usuario usuario, TipoMovimiento tipo, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorDia(usuario, tipo, desde, hasta, _accesoDatos);
        }

        public List<TotalPorMes> ObtenerTotalesPorMes(Usuario usuario, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerTotalesPorMes(usuario, desde, hasta, _accesoDatos);
        }

        public ResumenMovimientos ObtenerResumen(Usuario usuario, DateTime desde, DateTime hasta)
        {
            return _movimientoDAL.ObtenerResumen(usuario, desde, hasta, _accesoDatos);
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

        public bool Eliminar(Movimiento movimiento)
        {
            return _movimientoDAL.Eliminar(movimiento, _accesoDatos);
        }

        public int ContarPorComercio(Comercio comercio)
        {
            return _movimientoDAL.ContarPorComercio(comercio, _accesoDatos);
        }
    }
}
