using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class MonedaRepositorio : IMonedaRepositorio
    {
        private readonly MonedaDAL _monedaDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public MonedaRepositorio()
            : this(null)
        {
        }

        internal MonedaRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _monedaDAL = new MonedaDAL();
            _accesoDatos = accesoDatos;
        }

        public Moneda ObtenerPorId(int id)
        {
            return _monedaDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<Moneda> ObtenerTodas()
        {
            return _monedaDAL.ObtenerTodas(_accesoDatos);
        }

        public int InsertarYObtenerId(Moneda moneda)
        {
            return _monedaDAL.InsertarYObtenerId(moneda, _accesoDatos);
        }

        public bool Actualizar(Moneda moneda)
        {
            return _monedaDAL.Actualizar(moneda, _accesoDatos);
        }

        public bool Eliminar(Moneda moneda)
        {
            return _monedaDAL.Eliminar(moneda, _accesoDatos);
        }
    }
}
