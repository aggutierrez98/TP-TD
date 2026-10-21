using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IComercioReglaCategorizacionRepositorio
    {
        List<Comercio> ObtenerComerciosPorReglaCategorizacion(ReglaCategorizacion reglaCategorizacion);

        bool ReemplazarComerciosDeReglaCategorizacion(ReglaCategorizacion reglaCategorizacion, IEnumerable<Comercio> comercios);

        int ContarPorComercio(Comercio comercio);
    }
}
