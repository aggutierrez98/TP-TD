using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    // Lecturas de la recomendacion de compras. Delega las consultas con JOIN en
    // RecomendacionComprasDAL e hidrata los filtros mediante PromocionRepositorio.
    public class RecomendacionComprasRepositorio : IRecomendacionComprasRepositorio
    {
        private readonly RecomendacionComprasDAL _recomendacionComprasDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public RecomendacionComprasRepositorio()
            : this(null)
        {
        }

        internal RecomendacionComprasRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _recomendacionComprasDAL = new RecomendacionComprasDAL();
            _accesoDatos = accesoDatos;
        }

        public List<ItemLista> ObtenerItemsPendientesPorUsuario(Usuario usuario)
        {
            return _recomendacionComprasDAL.ObtenerItemsPendientesPorUsuario(usuario, _accesoDatos);
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
            promociones.ForEach(new PromocionRepositorio(_accesoDatos).Hidratar);
            return promociones;
        }

        public List<CuentaFinanciera> ObtenerCuentasConMediosPorUsuario(Usuario usuario)
        {
            return _recomendacionComprasDAL.ObtenerCuentasConMediosPorUsuario(usuario, _accesoDatos);
        }

    }
}
