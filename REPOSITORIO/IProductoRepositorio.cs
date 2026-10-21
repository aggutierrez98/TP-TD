using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IProductoRepositorio
    {
        Producto ObtenerPorId(int id);

        List<Producto> ObtenerTodos();

        int InsertarYObtenerId(Producto producto);

        bool Actualizar(Producto producto);

        bool Eliminar(Producto producto);

        int ContarPorCategoria(CategoriaProducto categoria);
    }
}
