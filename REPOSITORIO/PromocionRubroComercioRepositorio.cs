using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PromocionRubroComercioRepositorio : IPromocionRubroComercioRepositorio
    {
        private readonly PromocionRubroComercioDAL _promocionRubroComercioDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionRubroComercioRepositorio()
            : this(null)
        {
        }

        internal PromocionRubroComercioRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionRubroComercioDAL = new PromocionRubroComercioDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerRubroComercioIdsPorPromocion(int promocionId)
        {
            return _promocionRubroComercioDAL.ObtenerRubroComercioIdsPorPromocion(promocionId, _accesoDatos);
        }

        public bool ReemplazarRubrosComercioDePromocion(int promocionId, List<int> rubroComercioIds)
        {
            return _promocionRubroComercioDAL.ReemplazarRubrosComercioDePromocion(promocionId, rubroComercioIds, _accesoDatos);
        }
    }
}
