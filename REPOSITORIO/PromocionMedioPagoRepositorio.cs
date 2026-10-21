using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionMedioPagoRepositorio : IPromocionMedioPagoRepositorio
    {
        private readonly PromocionMedioPagoDAL _dal;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionMedioPagoRepositorio()
            : this(null)
        {
        }

        internal PromocionMedioPagoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dal = new PromocionMedioPagoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<MedioPago> ObtenerMediosPagoPorPromocion(Promocion promocion)
        {
            MedioPagoRepositorio repositorio = new MedioPagoRepositorio(_accesoDatos);
            return _dal.ObtenerMediosPagoPorPromocion(promocion, _accesoDatos).Select(item => repositorio.ObtenerPorId(item.Id)).ToList();
        }

        public bool ReemplazarMediosPagoDePromocion(Promocion promocion, List<MedioPago> mediosPago)
        {
            return _dal.ReemplazarMediosPagoDePromocion(promocion, mediosPago ?? new List<MedioPago>(), _accesoDatos);
        }

        public int ContarPorMedioPago(MedioPago medioPago)
        {
            return _dal.ContarPorMedioPago(medioPago, _accesoDatos);
        }
    }
}
