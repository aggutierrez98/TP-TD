using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionProductoRepositorio
    {
        List<Producto> ObtenerProductosPorPromocion(Promocion promocion, bool excluidos);

        bool ReemplazarProductosDePromocion(Promocion promocion, List<Producto> incluidos, List<Producto> excluidos);

        // Promociones que incluyen o excluyen el producto: la baja del producto se bloquea si hay alguna.
        int ContarPorProducto(Producto producto);
    }
}
