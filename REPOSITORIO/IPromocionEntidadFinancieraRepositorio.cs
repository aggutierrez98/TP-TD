using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionEntidadFinancieraRepositorio
    {
        List<int> ObtenerEntidadFinancieraIdsPorPromocion(int promocionId);

        bool ReemplazarEntidadesFinancierasDePromocion(int promocionId, List<int> entidadFinancieraIds);

        int ContarPorEntidadFinanciera(int entidadFinancieraId);
    }
}
