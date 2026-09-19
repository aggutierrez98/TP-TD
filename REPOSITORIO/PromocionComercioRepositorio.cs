using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PromocionComercioRepositorio : IPromocionComercioRepositorio
    {
        private readonly PromocionComercioDAL _promocionComercioDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionComercioRepositorio()
            : this(null)
        {
        }

        internal PromocionComercioRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionComercioDAL = new PromocionComercioDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerComercioIdsPorPromocion(int promocionId)
        {
            return _promocionComercioDAL.ObtenerComercioIdsPorPromocion(promocionId, _accesoDatos);
        }

        public List<int> ObtenerPromocionIdsPorComercio(int comercioId)
        {
            return _promocionComercioDAL.ObtenerPromocionIdsPorComercio(comercioId, _accesoDatos);
        }

        public bool ReemplazarComerciosDePromocion(int promocionId, List<int> comercioIds)
        {
            return _promocionComercioDAL.ReemplazarComerciosDePromocion(promocionId, comercioIds, _accesoDatos);
        }
    }
}
