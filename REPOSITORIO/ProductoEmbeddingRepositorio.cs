using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ProductoEmbeddingRepositorio : IProductoEmbeddingRepositorio
    {
        private readonly ProductoEmbeddingDAL _productoEmbeddingDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ProductoEmbeddingRepositorio()
            : this(null)
        {
        }

        internal ProductoEmbeddingRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _productoEmbeddingDAL = new ProductoEmbeddingDAL();
            _accesoDatos = accesoDatos;
        }

        public List<ProductoEmbedding> ObtenerTodos()
        {
            return _productoEmbeddingDAL.ObtenerTodos(_accesoDatos);
        }

        public void Guardar(ProductoEmbedding embedding)
        {
            _productoEmbeddingDAL.Guardar(embedding, _accesoDatos);
        }
    }
}
