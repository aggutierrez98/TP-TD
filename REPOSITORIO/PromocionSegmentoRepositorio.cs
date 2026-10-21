using System.Collections.Generic;
using BE;
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

        public List<string> ObtenerSegmentosPorPromocion(Promocion promocion)
        {
            return _dal.ObtenerSegmentosPorPromocion(promocion, _accesoDatos);
        }

        public bool ReemplazarSegmentosDePromocion(Promocion promocion, List<string> segmentos)
        {
            return _dal.ReemplazarSegmentosDePromocion(promocion, segmentos ?? new List<string>(), _accesoDatos);
        }
    }
}
