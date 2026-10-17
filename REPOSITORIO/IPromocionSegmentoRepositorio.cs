using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionSegmentoRepositorio
    {
        List<string> ObtenerSegmentosPorPromocion(int promocionId);

        bool ReemplazarSegmentosDePromocion(int promocionId, List<string> segmentos);
    }
}
