using System.Collections.Generic;
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
            return _movimientoDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<Movimiento> ObtenerPorUsuario(int usuarioId, FiltroMovimiento filtro)
        {
            return _movimientoDAL.ObtenerPorUsuario(usuarioId, filtro, _accesoDatos);
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
    }
}
