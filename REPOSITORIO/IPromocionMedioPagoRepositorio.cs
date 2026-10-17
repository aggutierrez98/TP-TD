using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionMedioPagoRepositorio
    {
        List<int> ObtenerMedioPagoIdsPorPromocion(int promocionId);

        bool ReemplazarMediosPagoDePromocion(int promocionId, List<int> medioPagoIds);

        int ContarPorMedioPago(int medioPagoId);
    }
}
