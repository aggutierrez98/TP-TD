using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PromocionMedioDePagoRepositorio : IPromocionMedioDePagoRepositorio
    {
        private readonly PromocionMedioDePagoDAL _promocionMedioDePagoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionMedioDePagoRepositorio()
            : this(null)
        {
        }

        internal PromocionMedioDePagoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionMedioDePagoDAL = new PromocionMedioDePagoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerMedioDePagoIdsPorPromocion(int promocionId)
        {
            return _promocionMedioDePagoDAL.ObtenerMedioDePagoIdsPorPromocion(promocionId, _accesoDatos);
        }

        public bool ReemplazarMediosDePagoDePromocion(int promocionId, List<int> medioDePagoIds)
        {
            return _promocionMedioDePagoDAL.ReemplazarMediosDePagoDePromocion(promocionId, medioDePagoIds, _accesoDatos);
        }
    }
}
