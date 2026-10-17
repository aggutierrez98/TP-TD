using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionProductoRepositorio
    {
        List<int> ObtenerProductoIdsPorPromocion(int promocionId, bool excluidos);

        bool ReemplazarProductosDePromocion(int promocionId, List<int> incluidos, List<int> excluidos);

        // Promociones que incluyen o excluyen el producto: la baja del producto se bloquea si hay alguna.
        int ContarPorProducto(int productoId);
    }
}
