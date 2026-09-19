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

        private void Hidratar(Movimiento movimiento)
        {
            if (movimiento == null)
            {
                return;
            }

            movimiento.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(movimiento.Usuario.Id);
            movimiento.Categoria = new CategoriaRepositorio(_accesoDatos).ObtenerPorId(movimiento.Categoria.Id);
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
