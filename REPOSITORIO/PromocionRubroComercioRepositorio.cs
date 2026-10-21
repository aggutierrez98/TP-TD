using System.Collections.Generic;
using System.Linq;
using BE;
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

        public List<RubroComercio> ObtenerRubrosComercioPorPromocion(Promocion promocion)
        {
            RubroComercioRepositorio repositorio = new RubroComercioRepositorio(_accesoDatos);
            return _promocionRubroComercioDAL.ObtenerRubrosComercioPorPromocion(promocion, _accesoDatos).Select(item => repositorio.ObtenerPorId(item.Id)).ToList();
        }

        public bool ReemplazarRubrosComercioDePromocion(Promocion promocion, List<RubroComercio> rubrosComercio)
        {
            return _promocionRubroComercioDAL.ReemplazarRubrosComercioDePromocion(promocion, rubrosComercio, _accesoDatos);
        }

        public int ContarPorRubroComercio(RubroComercio rubroComercio)
        {
            return _promocionRubroComercioDAL.ContarPorRubroComercio(rubroComercio, _accesoDatos);
        }
    }
}
