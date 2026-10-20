using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IProductoEmbeddingRepositorio
    {
        List<ProductoEmbedding> ObtenerTodos();

        // Inserta la fila del producto o la reemplaza si ya existia.
        void Guardar(ProductoEmbedding embedding);
    }
}
