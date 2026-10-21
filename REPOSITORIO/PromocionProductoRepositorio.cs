using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionProductoRepositorio : IPromocionProductoRepositorio
    {
        private readonly PromocionProductoDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionProductoRepositorio()
            : this(null)
        {
        }

        internal PromocionProductoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new PromocionProductoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<Producto> ObtenerProductosPorPromocion(Promocion promocion, bool excluidos)
        {
            ProductoRepositorio repositorio = new ProductoRepositorio(_accesoDatos);
            return _dal.ObtenerProductosPorPromocion(promocion, excluidos, _accesoDatos).Select(item => repositorio.ObtenerPorId(item.Id)).ToList();
        }

        public bool ReemplazarProductosDePromocion(Promocion promocion, List<Producto> incluidos, List<Producto> excluidos)
        {
            return _dal.ReemplazarProductosDePromocion(promocion, incluidos ?? new List<Producto>(), excluidos ?? new List<Producto>(), _accesoDatos);
        }

        public int ContarPorProducto(Producto producto)
        {
            return _dal.ContarPorProducto(producto, _accesoDatos);
        }
    }
}
