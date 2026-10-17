using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IPromocionCategoriaProductoRepositorio
    {
        List<int> ObtenerCategoriaProductoIdsPorPromocion(int promocionId, bool excluidos);

        bool ReemplazarCategoriasProductoDePromocion(int promocionId, List<int> incluidos, List<int> excluidos);

        // Promociones que incluyen o excluyen la categoria: la baja de la categoria se bloquea si hay alguna.
        int ContarPorCategoriaProducto(int categoriaProductoId);
    }
}
