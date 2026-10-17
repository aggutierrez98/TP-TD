using System.Collections.Generic;
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

        public List<int> ObtenerCategoriaProductoIdsPorPromocion(int promocionId, bool excluidos)
        {
            return _dal.ObtenerCategoriaProductoIdsPorPromocion(promocionId, excluidos, _accesoDatos);
        }

        public bool ReemplazarCategoriasProductoDePromocion(int promocionId, List<int> incluidos, List<int> excluidos)
        {
            return _dal.ReemplazarCategoriasProductoDePromocion(promocionId, incluidos ?? new List<int>(), excluidos ?? new List<int>(), _accesoDatos);
        }

        public int ContarPorCategoriaProducto(int categoriaProductoId)
        {
            return _dal.ContarPorCategoriaProducto(categoriaProductoId, _accesoDatos);
        }
    }
}
