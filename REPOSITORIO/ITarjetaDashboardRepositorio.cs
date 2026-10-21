using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ITarjetaDashboardRepositorio
    {
        TarjetaDashboard ObtenerPorId(int id);

        List<TarjetaDashboard> ObtenerPorDashboard(Dashboard dashboard);

        int InsertarYObtenerId(TarjetaDashboard tarjeta);

        bool Actualizar(TarjetaDashboard tarjeta);

        bool Eliminar(TarjetaDashboard tarjeta);
    }
}
