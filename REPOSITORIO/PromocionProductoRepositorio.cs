using System.Collections.Generic;
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

        public List<int> ObtenerProductoIdsPorPromocion(int promocionId, bool excluidos)
        {
            return _dal.ObtenerProductoIdsPorPromocion(promocionId, excluidos, _accesoDatos);
        }

        public bool ReemplazarProductosDePromocion(int promocionId, List<int> incluidos, List<int> excluidos)
        {
            return _dal.ReemplazarProductosDePromocion(promocionId, incluidos ?? new List<int>(), excluidos ?? new List<int>(), _accesoDatos);
        }

        public int ContarPorProducto(int productoId)
        {
            return _dal.ContarPorProducto(productoId, _accesoDatos);
        }
    }
}
