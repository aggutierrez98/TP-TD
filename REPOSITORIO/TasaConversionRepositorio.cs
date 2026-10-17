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

        public TasaConversion ObtenerPorParDeMonedas(int monedaBaseId, int monedaConversionId)
        {
            TasaConversion tasa = _tasaConversionDAL.ObtenerPorParDeMonedas(monedaBaseId, monedaConversionId, _accesoDatos);
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
        }

        public int InsertarYObtenerId(TasaConversion tasa)
        {
            return _tasaConversionDAL.InsertarYObtenerId(tasa, _accesoDatos);
        }

        public bool Actualizar(TasaConversion tasa)
        {
            return _tasaConversionDAL.Actualizar(tasa, _accesoDatos);
        }

        public bool ActualizarValorAutomatico(int id, decimal calculoConversion)
        {
            return _tasaConversionDAL.ActualizarValorAutomatico(id, calculoConversion, _accesoDatos);
        }

        public bool MarcarManual(int id, int usuarioId)
        {
            return _tasaConversionDAL.MarcarManual(id, usuarioId, _accesoDatos);
        }

        public bool Liberar(int id)
        {
            return _tasaConversionDAL.Liberar(id, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _tasaConversionDAL.Eliminar(id, _accesoDatos);
        }
    }
}
