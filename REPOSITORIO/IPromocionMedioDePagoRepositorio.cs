using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionMedioDePagoRepositorio
    {
        List<int> ObtenerMedioDePagoIdsPorPromocion(int promocionId);

        bool ReemplazarMediosDePagoDePromocion(int promocionId, List<int> medioDePagoIds);
    }
}
