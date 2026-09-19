using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionComercioRepositorio
    {
        List<int> ObtenerComercioIdsPorPromocion(int promocionId);

        List<int> ObtenerPromocionIdsPorComercio(int comercioId);

        bool ReemplazarComerciosDePromocion(int promocionId, List<int> comercioIds);
    }
}
