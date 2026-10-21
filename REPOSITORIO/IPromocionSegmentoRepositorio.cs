using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionSegmentoRepositorio
    {
        List<string> ObtenerSegmentosPorPromocion(Promocion promocion);

        bool ReemplazarSegmentosDePromocion(Promocion promocion, List<string> segmentos);
    }
}
