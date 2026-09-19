using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IRubroComercioRepositorio
    {
        RubroComercio ObtenerPorId(int id);

        List<RubroComercio> ObtenerTodas();

        int InsertarYObtenerId(RubroComercio rubro);

        bool Actualizar(RubroComercio rubro);

        bool Eliminar(int id);
    }
}
