using System.Collections.Generic;
using System.Linq;
using BE;
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

        public List<Comercio> ObtenerComerciosPorPromocion(Promocion promocion)
        {
            ComercioRepositorio repositorio = new ComercioRepositorio(_accesoDatos);
            return _promocionComercioDAL.ObtenerComerciosPorPromocion(promocion, _accesoDatos).Select(item => repositorio.ObtenerPorId(item.Id)).ToList();
        }

        public List<Promocion> ObtenerPromocionesPorComercio(Comercio comercio)
        {
            PromocionRepositorio repositorio = new PromocionRepositorio(_accesoDatos);
            return _promocionComercioDAL.ObtenerPromocionesPorComercio(comercio, _accesoDatos).Select(item => repositorio.ObtenerPorId(item.Id)).ToList();
        }

        public bool ReemplazarComerciosDePromocion(Promocion promocion, List<Comercio> comercios)
        {
            return _promocionComercioDAL.ReemplazarComerciosDePromocion(promocion, comercios, _accesoDatos);
        }
    }
}
