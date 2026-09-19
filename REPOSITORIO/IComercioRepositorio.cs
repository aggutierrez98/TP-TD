using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IComercioRepositorio
    {
        Comercio ObtenerPorId(int id);

        List<Comercio> ObtenerPorRubro(int rubroComercioId);

        List<Comercio> ObtenerTodos();

        int InsertarYObtenerId(Comercio comercio);

        bool Actualizar(Comercio comercio);

        bool Eliminar(int id);
    }
}
