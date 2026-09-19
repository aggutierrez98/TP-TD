using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IMonedaRepositorio
    {
        Moneda ObtenerPorId(int id);

        List<Moneda> ObtenerTodas();

        int InsertarYObtenerId(Moneda moneda);

        bool Actualizar(Moneda moneda);

        bool Eliminar(int id);
    }
}
