using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ICategoriaProductoRepositorio
    {
        CategoriaProducto ObtenerPorId(int id);

        List<CategoriaProducto> ObtenerTodas();

        int InsertarYObtenerId(CategoriaProducto categoria);

        bool Actualizar(CategoriaProducto categoria);

        bool Eliminar(CategoriaProducto categoria);
    }
}
