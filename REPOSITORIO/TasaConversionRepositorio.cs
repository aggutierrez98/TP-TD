using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class TasaConversionRepositorio : ITasaConversionRepositorio
    {
        private readonly TasaConversionDAL _tasaConversionDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public TasaConversionRepositorio()
            : this(null)
        {
        }

        internal TasaConversionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _tasaConversionDAL = new TasaConversionDAL();
            _accesoDatos = accesoDatos;
        }

        public TasaConversion ObtenerPorId(int id)
        {
            TasaConversion tasa = _tasaConversionDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(tasa);
            return tasa;
        }

        public List<TasaConversion> ObtenerTodas()
        {
            List<TasaConversion> tasas = _tasaConversionDAL.ObtenerTodas(_accesoDatos);
            foreach (TasaConversion tasa in tasas)
            {
                Hidratar(tasa);
            }

            return tasas;
        }

        public TasaConversion ObtenerPorParDeMonedas(Moneda monedaBase, Moneda monedaConversion)
        {
            TasaConversion tasa = _tasaConversionDAL.ObtenerPorParDeMonedas(monedaBase, monedaConversion, _accesoDatos);
            Hidratar(tasa);
            return tasa;
        }

        private void Hidratar(TasaConversion tasa)
        {
            if (tasa == null)
            {
                return;
            }

            MonedaRepositorio monedaRepositorio = new MonedaRepositorio(_accesoDatos);
            tasa.MonedaBase = monedaRepositorio.ObtenerPorId(tasa.MonedaBase.Id);
            tasa.MonedaConversion = monedaRepositorio.ObtenerPorId(tasa.MonedaConversion.Id);
            if (tasa.ProtegidaPorUsuario != null)
            {
                tasa.ProtegidaPorUsuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(tasa.ProtegidaPorUsuario.Id);
            }
        }

        public int InsertarYObtenerId(TasaConversion tasa)
        {
            return _tasaConversionDAL.InsertarYObtenerId(tasa, _accesoDatos);
        }

        public bool Actualizar(TasaConversion tasa)
        {
            return _tasaConversionDAL.Actualizar(tasa, _accesoDatos);
        }

        public bool ActualizarValorAutomatico(TasaConversion tasa, decimal calculoConversion)
        {
            return _tasaConversionDAL.ActualizarValorAutomatico(tasa, calculoConversion, _accesoDatos);
        }

        public bool MarcarManual(TasaConversion tasa, Usuario usuario)
        {
            return _tasaConversionDAL.MarcarManual(tasa, usuario, _accesoDatos);
        }

        public bool Liberar(TasaConversion tasa)
        {
            return _tasaConversionDAL.Liberar(tasa, _accesoDatos);
        }

        public bool Eliminar(TasaConversion tasa)
        {
            return _tasaConversionDAL.Eliminar(tasa, _accesoDatos);
        }
    }
}
