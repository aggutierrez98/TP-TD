using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionRubroComercioRepositorio
    {
        List<int> ObtenerRubroComercioIdsPorPromocion(int promocionId);

        bool ReemplazarRubrosComercioDePromocion(int promocionId, List<int> rubroComercioIds);

        int ContarPorRubroComercio(int rubroComercioId);
    }
}
