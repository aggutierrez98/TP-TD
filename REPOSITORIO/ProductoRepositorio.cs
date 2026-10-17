using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly ProductoDAL _productoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ProductoRepositorio()
            : this(null)
        {
        }

        internal ProductoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _productoDAL = new ProductoDAL();
            _accesoDatos = accesoDatos;
        }

        public Producto ObtenerPorId(int id)
        {
            Producto producto = _productoDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(producto);
            return producto;
        }

        public List<Producto> ObtenerTodos()
        {
            List<Producto> productos = _productoDAL.ObtenerTodos(_accesoDatos);
            productos.ForEach(Hidratar);
            return productos;
        }

        private void Hidratar(Producto producto)
        {
            if (producto == null)
            {
                return;
            }

            producto.CategoriaProducto = new CategoriaProductoRepositorio(_accesoDatos).ObtenerPorId(producto.CategoriaProducto.Id);
        }

        public int InsertarYObtenerId(Producto producto)
        {
            return _productoDAL.InsertarYObtenerId(producto, _accesoDatos);
        }

        public bool Actualizar(Producto producto)
        {
            return _productoDAL.Actualizar(producto, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _productoDAL.Eliminar(id, _accesoDatos);
        }

        public int ContarPorCategoria(int categoriaProductoId)
        {
            return _productoDAL.ContarPorCategoria(categoriaProductoId, _accesoDatos);
        }
    }
}
