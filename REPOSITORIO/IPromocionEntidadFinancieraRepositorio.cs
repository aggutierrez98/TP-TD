using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionEntidadFinancieraRepositorio
    {
        List<EntidadFinanciera> ObtenerEntidadesFinancierasPorPromocion(Promocion promocion);

        bool ReemplazarEntidadesFinancierasDePromocion(Promocion promocion, List<EntidadFinanciera> entidadesFinancieras);

        int ContarPorEntidadFinanciera(EntidadFinanciera entidadFinanciera);
    }
}
