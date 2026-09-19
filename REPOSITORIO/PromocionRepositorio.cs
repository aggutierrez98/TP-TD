using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionRepositorio : IPromocionRepositorio
    {
        private readonly PromocionDAL _promocionDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionRepositorio()
            : this(null)
        {
        }

        internal PromocionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionDAL = new PromocionDAL();
            _accesoDatos = accesoDatos;
        }

        public Promocion ObtenerPorId(int id)
        {
            return _promocionDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<Promocion> ObtenerTodas()
        {
            return _promocionDAL.ObtenerTodas(_accesoDatos);
        }

        public int InsertarYObtenerId(Promocion promocion)
        {
            return _promocionDAL.InsertarYObtenerId(promocion, _accesoDatos);
        }

        public bool Actualizar(Promocion promocion)
        {
            return _promocionDAL.Actualizar(promocion, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _promocionDAL.Eliminar(id, _accesoDatos);
        }
    }
}
