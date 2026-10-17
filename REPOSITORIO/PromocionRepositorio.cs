using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionRepositorio : IPromocionRepositorio
    {
        private readonly PromocionDAL _promocionDAL;
        private readonly PromocionDiaSemanaDAL _promocionDiaSemanaDAL;
        private readonly PromocionComercioDAL _promocionComercioDAL;
        private readonly PromocionRubroComercioDAL _promocionRubroComercioDAL;
        private readonly PromocionEntidadFinancieraDAL _promocionEntidadFinancieraDAL;
        private readonly PromocionMedioPagoDAL _promocionMedioPagoDAL;
        private readonly PromocionSegmentoDAL _promocionSegmentoDAL;
        private readonly PromocionProductoDAL _promocionProductoDAL;
        private readonly PromocionCategoriaProductoDAL _promocionCategoriaProductoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionRepositorio()
            : this(null)
        {
        }

        internal PromocionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionDAL = new PromocionDAL();
            _promocionDiaSemanaDAL = new PromocionDiaSemanaDAL();
            _promocionComercioDAL = new PromocionComercioDAL();
            _promocionRubroComercioDAL = new PromocionRubroComercioDAL();
            _promocionEntidadFinancieraDAL = new PromocionEntidadFinancieraDAL();
            _promocionMedioPagoDAL = new PromocionMedioPagoDAL();
            _promocionSegmentoDAL = new PromocionSegmentoDAL();
            _promocionProductoDAL = new PromocionProductoDAL();
            _promocionCategoriaProductoDAL = new PromocionCategoriaProductoDAL();
            _accesoDatos = accesoDatos;
        }

        public Promocion ObtenerPorId(int id)
        {
            Promocion promocion = _promocionDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(promocion);
            return promocion;
        }

        public List<Promocion> ObtenerTodas()
        {
            List<Promocion> promociones = _promocionDAL.ObtenerTodas(_accesoDatos);
            promociones.ForEach(Hidratar);
            return promociones;
        }

        private void Hidratar(Promocion promocion)
        {
            if (promocion == null)
            {
                return;
            }

            promocion.DiasSemana = _promocionDiaSemanaDAL.ObtenerDiasPorPromocion(promocion.Id, _accesoDatos);
            promocion.ComercioIds = _promocionComercioDAL.ObtenerComercioIdsPorPromocion(promocion.Id, _accesoDatos);
            promocion.RubroComercioIds = _promocionRubroComercioDAL.ObtenerRubroComercioIdsPorPromocion(promocion.Id, _accesoDatos);
            promocion.EntidadFinancieraIds = _promocionEntidadFinancieraDAL.ObtenerEntidadFinancieraIdsPorPromocion(promocion.Id, _accesoDatos);
            promocion.MedioPagoIds = _promocionMedioPagoDAL.ObtenerMedioPagoIdsPorPromocion(promocion.Id, _accesoDatos);
            promocion.Segmentos = _promocionSegmentoDAL.ObtenerSegmentosPorPromocion(promocion.Id, _accesoDatos);
            promocion.ProductoIncluidoIds = _promocionProductoDAL.ObtenerProductoIdsPorPromocion(promocion.Id, false, _accesoDatos);
            promocion.ProductoExcluidoIds = _promocionProductoDAL.ObtenerProductoIdsPorPromocion(promocion.Id, true, _accesoDatos);
            promocion.CategoriaIncluidaIds = _promocionCategoriaProductoDAL.ObtenerCategoriaProductoIdsPorPromocion(promocion.Id, false, _accesoDatos);
            promocion.CategoriaExcluidaIds = _promocionCategoriaProductoDAL.ObtenerCategoriaProductoIdsPorPromocion(promocion.Id, true, _accesoDatos);
        }

        public int InsertarYObtenerId(Promocion promocion)
        {
            return _promocionDAL.InsertarYObtenerId(promocion, _accesoDatos);
        }

        public bool Actualizar(Promocion promocion)
        {
            return _promocionDAL.Actualizar(promocion, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _promocionDAL.Eliminar(id, _accesoDatos);
        }

        public bool ReemplazarDiasDePromocion(int promocionId, List<DiaSemana> dias)
        {
            return _promocionDiaSemanaDAL.ReemplazarDiasDePromocion(promocionId, dias ?? new List<DiaSemana>(), _accesoDatos);
        }
    }
}
