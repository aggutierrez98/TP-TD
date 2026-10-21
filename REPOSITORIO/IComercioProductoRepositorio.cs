using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IComercioProductoRepositorio
    {
        List<ComercioProducto> ObtenerPorComercio(Comercio comercio);

        // La clave es el par (Comercio, Producto): no hay Id propio para ObtenerPorId.
        ComercioProducto ObtenerPorComercioYProducto(Comercio comercio, Producto producto);

        bool Insertar(ComercioProducto comercioProducto);

        // Solo persiste el Precio; Comercio y Producto identifican la fila.
        bool ActualizarPrecio(ComercioProducto comercioProducto);

        bool Eliminar(ComercioProducto comercioProducto);

        int ContarPorProducto(Producto producto);
    }
}
