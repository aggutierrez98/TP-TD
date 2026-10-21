using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionEntidadFinancieraRepositorio : IPromocionEntidadFinancieraRepositorio
    {
        private readonly PromocionEntidadFinancieraDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionEntidadFinancieraRepositorio()
            : this(null)
        {
        }

        internal PromocionEntidadFinancieraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new PromocionEntidadFinancieraDAL();
            _accesoDatos = accesoDatos;
        }

        public List<EntidadFinanciera> ObtenerEntidadesFinancierasPorPromocion(Promocion promocion)
        {
            EntidadFinancieraRepositorio repositorio = new EntidadFinancieraRepositorio(_accesoDatos);
            return _dal.ObtenerEntidadesFinancierasPorPromocion(promocion, _accesoDatos).Select(item => repositorio.ObtenerPorId(item.Id)).ToList();
        }

        public bool ReemplazarEntidadesFinancierasDePromocion(Promocion promocion, List<EntidadFinanciera> entidadesFinancieras)
        {
            return _dal.ReemplazarEntidadesFinancierasDePromocion(promocion, entidadesFinancieras ?? new List<EntidadFinanciera>(), _accesoDatos);
        }

        public int ContarPorEntidadFinanciera(EntidadFinanciera entidadFinanciera)
        {
            return _dal.ContarPorEntidadFinanciera(entidadFinanciera, _accesoDatos);
        }
    }
}
