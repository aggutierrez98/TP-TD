using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IComercioProductoRepositorio
    {
        List<ComercioProducto> ObtenerPorComercio(int comercioId);

        ComercioProducto ObtenerPorId(int comercioId, int productoId);

        bool Insertar(ComercioProducto comercioProducto);

        bool ActualizarPrecio(int comercioId, int productoId, decimal precio);

        bool Eliminar(int comercioId, int productoId);

        int ContarPorProducto(int productoId);
    }
}
