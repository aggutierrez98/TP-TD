using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IComercioReglaCategorizacionRepositorio
    {
        List<int> ObtenerComercioIdsPorReglaCategorizacion(int reglaCategorizacionId);

        bool ReemplazarComerciosDeReglaCategorizacion(int reglaCategorizacionId, List<int> comercioIds);

        int ContarPorComercio(int comercioId);
    }
}
