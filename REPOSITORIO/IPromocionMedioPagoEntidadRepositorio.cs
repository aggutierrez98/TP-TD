using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionMedioPagoEntidadRepositorio
    {
        List<int> ObtenerMedioPagoEntidadIdsPorPromocion(int promocionId);

        bool ReemplazarMediosPagoEntidadDePromocion(int promocionId, List<int> medioPagoEntidadIds);
    }
}
