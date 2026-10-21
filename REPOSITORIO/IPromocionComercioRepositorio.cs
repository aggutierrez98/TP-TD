using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionComercioRepositorio
    {
        List<Comercio> ObtenerComerciosPorPromocion(Promocion promocion);

        List<Promocion> ObtenerPromocionesPorComercio(Comercio comercio);

        bool ReemplazarComerciosDePromocion(Promocion promocion, List<Comercio> comercios);
    }
}
