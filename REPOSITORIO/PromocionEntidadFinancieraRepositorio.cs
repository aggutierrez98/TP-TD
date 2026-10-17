using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PromocionEntidadFinancieraRepositorio : IPromocionEntidadFinancieraRepositorio
    {
        private readonly PromocionEntidadFinancieraDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionEntidadFinancieraRepositorio()
            : this(null)
        {
        }

        internal PromocionEntidadFinancieraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new PromocionEntidadFinancieraDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerEntidadFinancieraIdsPorPromocion(int promocionId)
        {
            return _dal.ObtenerEntidadFinancieraIdsPorPromocion(promocionId, _accesoDatos);
        }

        public bool ReemplazarEntidadesFinancierasDePromocion(int promocionId, List<int> entidadFinancieraIds)
        {
            return _dal.ReemplazarEntidadesFinancierasDePromocion(promocionId, entidadFinancieraIds ?? new List<int>(), _accesoDatos);
        }

        public int ContarPorEntidadFinanciera(int entidadFinancieraId)
        {
            return _dal.ContarPorEntidadFinanciera(entidadFinancieraId, _accesoDatos);
        }
    }
}
