using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionCategoriaProductoRepositorio : IPromocionCategoriaProductoRepositorio
    {
        private readonly PromocionCategoriaProductoDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionCategoriaProductoRepositorio()
            : this(null)
        {
        }

        internal PromocionCategoriaProductoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new PromocionCategoriaProductoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<CategoriaProducto> ObtenerCategoriasProductoPorPromocion(Promocion promocion, bool excluidos)
        {
            CategoriaProductoRepositorio repositorio = new CategoriaProductoRepositorio(_accesoDatos);
            return _dal.ObtenerCategoriasProductoPorPromocion(promocion, excluidos, _accesoDatos).Select(item => repositorio.ObtenerPorId(item.Id)).ToList();
        }

        public bool ReemplazarCategoriasProductoDePromocion(Promocion promocion, List<CategoriaProducto> incluidos, List<CategoriaProducto> excluidos)
        {
            return _dal.ReemplazarCategoriasProductoDePromocion(promocion, incluidos ?? new List<CategoriaProducto>(), excluidos ?? new List<CategoriaProducto>(), _accesoDatos);
        }

        public int ContarPorCategoriaProducto(CategoriaProducto categoriaProducto)
        {
            return _dal.ContarPorCategoriaProducto(categoriaProducto, _accesoDatos);
        }
    }
}
