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

        public List<Producto> ObtenerPorComercio(int comercioId)
        {
            List<Producto> productos = _productoDAL.ObtenerPorComercio(comercioId, _accesoDatos);
            productos.ForEach(Hidratar);
            return productos;
        }

        private void Hidratar(Producto producto)
        {
            if (producto == null)
            {
                return;
            }

            producto.Comercio = new ComercioRepositorio(_accesoDatos).ObtenerPorId(producto.Comercio.Id);
            producto.CategoriaProducto = new CategoriaProductoRepositorio(_accesoDatos).ObtenerPorId(producto.CategoriaProducto.Id);
            producto.Moneda = new MonedaRepositorio(_accesoDatos).ObtenerPorId(producto.Moneda.Id);
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
    }
}
