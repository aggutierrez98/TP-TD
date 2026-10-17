using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PromocionMedioPagoRepositorio : IPromocionMedioPagoRepositorio
    {
        private readonly PromocionMedioPagoDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionMedioPagoRepositorio()
            : this(null)
        {
        }

        internal PromocionMedioPagoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new PromocionMedioPagoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerMedioPagoIdsPorPromocion(int promocionId)
        {
            return _dal.ObtenerMedioPagoIdsPorPromocion(promocionId, _accesoDatos);
        }

        public bool ReemplazarMediosPagoDePromocion(int promocionId, List<int> medioPagoIds)
        {
            return _dal.ReemplazarMediosPagoDePromocion(promocionId, medioPagoIds ?? new List<int>(), _accesoDatos);
        }

        public int ContarPorMedioPago(int medioPagoId)
        {
            return _dal.ContarPorMedioPago(medioPagoId, _accesoDatos);
        }
    }
}
