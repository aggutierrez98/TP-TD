using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionRepositorio : IPromocionRepositorio
    {
        private readonly PromocionDAL _promocionDAL;
        private readonly PromocionDiaSemanaDAL _promocionDiaSemanaDAL;
        private readonly PromocionSegmentoDAL _promocionSegmentoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionRepositorio()
            : this(null)
        {
        }

        internal PromocionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionDAL = new PromocionDAL();
            _promocionDiaSemanaDAL = new PromocionDiaSemanaDAL();
            _promocionSegmentoDAL = new PromocionSegmentoDAL();
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

        internal void Hidratar(Promocion promocion)
        {
            if (promocion == null)
            {
                return;
            }

            promocion.DiasSemana = _promocionDiaSemanaDAL.ObtenerDiasPorPromocion(promocion, _accesoDatos);
            promocion.Comercios = new PromocionComercioRepositorio(_accesoDatos).ObtenerComerciosPorPromocion(promocion);
            promocion.RubrosComercio = new PromocionRubroComercioRepositorio(_accesoDatos).ObtenerRubrosComercioPorPromocion(promocion);
            promocion.EntidadesFinancieras = new PromocionEntidadFinancieraRepositorio(_accesoDatos).ObtenerEntidadesFinancierasPorPromocion(promocion);
            promocion.MediosPago = new PromocionMedioPagoRepositorio(_accesoDatos).ObtenerMediosPagoPorPromocion(promocion);
            promocion.Segmentos = _promocionSegmentoDAL.ObtenerSegmentosPorPromocion(promocion, _accesoDatos);
            PromocionProductoRepositorio productos = new PromocionProductoRepositorio(_accesoDatos);
            promocion.ProductosIncluidos = productos.ObtenerProductosPorPromocion(promocion, false);
            promocion.ProductosExcluidos = productos.ObtenerProductosPorPromocion(promocion, true);
            PromocionCategoriaProductoRepositorio categorias = new PromocionCategoriaProductoRepositorio(_accesoDatos);
            promocion.CategoriasIncluidas = categorias.ObtenerCategoriasProductoPorPromocion(promocion, false);
            promocion.CategoriasExcluidas = categorias.ObtenerCategoriasProductoPorPromocion(promocion, true);
        }

        public int InsertarYObtenerId(Promocion promocion)
        {
            return _promocionDAL.InsertarYObtenerId(promocion, _accesoDatos);
        }

        public bool Actualizar(Promocion promocion)
        {
            return _promocionDAL.Actualizar(promocion, _accesoDatos);
        }

        public bool Eliminar(Promocion promocion)
        {
            return _promocionDAL.Eliminar(promocion, _accesoDatos);
        }

        public bool ReemplazarDiasDePromocion(Promocion promocion, List<DiaSemana> dias)
        {
            return _promocionDiaSemanaDAL.ReemplazarDiasDePromocion(promocion, dias ?? new List<DiaSemana>(), _accesoDatos);
        }
    }
}
