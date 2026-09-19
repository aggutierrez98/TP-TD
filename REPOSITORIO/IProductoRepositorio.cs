using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IProductoRepositorio
    {
        Producto ObtenerPorId(int id);

        List<Producto> ObtenerPorComercio(int comercioId);

        int InsertarYObtenerId(Producto producto);

        bool Actualizar(Producto producto);

        bool Eliminar(int id);
    }
}
