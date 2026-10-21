using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionRubroComercioRepositorio
    {
        List<RubroComercio> ObtenerRubrosComercioPorPromocion(Promocion promocion);

        bool ReemplazarRubrosComercioDePromocion(Promocion promocion, List<RubroComercio> rubrosComercio);

        int ContarPorRubroComercio(RubroComercio rubroComercio);
    }
}
