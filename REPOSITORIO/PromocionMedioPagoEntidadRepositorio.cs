using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PromocionMedioPagoEntidadRepositorio : IPromocionMedioPagoEntidadRepositorio
    {
        private readonly PromocionMedioPagoEntidadDAL _promocionMedioPagoEntidadDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionMedioPagoEntidadRepositorio()
            : this(null)
        {
        }

        internal PromocionMedioPagoEntidadRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionMedioPagoEntidadDAL = new PromocionMedioPagoEntidadDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerMedioPagoEntidadIdsPorPromocion(int promocionId)
        {
            return _promocionMedioPagoEntidadDAL.ObtenerMedioPagoEntidadIdsPorPromocion(promocionId, _accesoDatos);
        }

        public bool ReemplazarMediosPagoEntidadDePromocion(int promocionId, List<int> medioPagoEntidadIds)
        {
            return _promocionMedioPagoEntidadDAL.ReemplazarMediosPagoEntidadDePromocion(promocionId, medioPagoEntidadIds, _accesoDatos);
        }
    }
}
