using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PromocionSegmentoRepositorio : IPromocionSegmentoRepositorio
    {
        private readonly PromocionSegmentoDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionSegmentoRepositorio()
            : this(null)
        {
        }

        internal PromocionSegmentoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new PromocionSegmentoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<string> ObtenerSegmentosPorPromocion(int promocionId)
        {
            return _dal.ObtenerSegmentosPorPromocion(promocionId, _accesoDatos);
        }

        public bool ReemplazarSegmentosDePromocion(int promocionId, List<string> segmentos)
        {
            return _dal.ReemplazarSegmentosDePromocion(promocionId, segmentos ?? new List<string>(), _accesoDatos);
        }
    }
}
