using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    // Lecturas de la recomendacion de compras. Delega las consultas con JOIN en
    // RecomendacionComprasDAL e hidrata los filtros de cada promocion con los DAL de sus
    // tablas intermedias, igual que PromocionRepositorio.
    public class RecomendacionComprasRepositorio : IRecomendacionComprasRepositorio
    {
        private readonly RecomendacionComprasDAL _recomendacionComprasDAL;
        private readonly PromocionDiaSemanaDAL _promocionDiaSemanaDAL;
        private readonly PromocionComercioDAL _promocionComercioDAL;
        private readonly PromocionRubroComercioDAL _promocionRubroComercioDAL;
        private readonly PromocionEntidadFinancieraDAL _promocionEntidadFinancieraDAL;
        private readonly PromocionMedioPagoDAL _promocionMedioPagoDAL;
        private readonly PromocionSegmentoDAL _promocionSegmentoDAL;
        private readonly PromocionProductoDAL _promocionProductoDAL;
        private readonly PromocionCategoriaProductoDAL _promocionCategoriaProductoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public RecomendacionComprasRepositorio()
            : this(null)
        {
        }

        internal RecomendacionComprasRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _recomendacionComprasDAL = new RecomendacionComprasDAL();
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

        public List<ItemLista> ObtenerItemsPendientesPorUsuario(int usuarioId)
        {
            return _recomendacionComprasDAL.ObtenerItemsPendientesPorUsuario(usuarioId, _accesoDatos);
        }

        public List<Producto> ObtenerProductosConCategoria()
        {
            return _recomendacionComprasDAL.ObtenerProductosConCategoria(_accesoDatos);
        }

        public List<ComercioProducto> ObtenerOfertas()
        {
            return _recomendacionComprasDAL.ObtenerOfertas(_accesoDatos);
        }

        public List<Promocion> ObtenerPromocionesVigentesEntre(DateTime desde, DateTime hasta)
        {
            List<Promocion> promociones = _recomendacionComprasDAL.ObtenerPromocionesVigentesEntre(desde, hasta, _accesoDatos);
            promociones.ForEach(HidratarFiltros);
            return promociones;
        }

        public List<CuentaFinanciera> ObtenerCuentasConMediosPorUsuario(int usuarioId)
        {
            return _recomendacionComprasDAL.ObtenerCuentasConMediosPorUsuario(usuarioId, _accesoDatos);
        }

        private void HidratarFiltros(Promocion promocion)
        {
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
    }
}
