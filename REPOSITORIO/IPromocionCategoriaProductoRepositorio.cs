using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IPromocionCategoriaProductoRepositorio
    {
        List<CategoriaProducto> ObtenerCategoriasProductoPorPromocion(Promocion promocion, bool excluidos);

        bool ReemplazarCategoriasProductoDePromocion(Promocion promocion, List<CategoriaProducto> incluidos, List<CategoriaProducto> excluidos);

        // Promociones que incluyen o excluyen la categoria: la baja de la categoria se bloquea si hay alguna.
        int ContarPorCategoriaProducto(CategoriaProducto categoriaProducto);
    }
}
