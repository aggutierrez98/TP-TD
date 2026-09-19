using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class RubroComercioRepositorio : IRubroComercioRepositorio
    {
        private readonly RubroComercioDAL _rubroComercioDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public RubroComercioRepositorio()
            : this(null)
        {
        }

        internal RubroComercioRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _rubroComercioDAL = new RubroComercioDAL();
            _accesoDatos = accesoDatos;
        }

        public RubroComercio ObtenerPorId(int id)
        {
            return _rubroComercioDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<RubroComercio> ObtenerTodas()
        {
            return _rubroComercioDAL.ObtenerTodas(_accesoDatos);
        }

        public int InsertarYObtenerId(RubroComercio rubro)
        {
            return _rubroComercioDAL.InsertarYObtenerId(rubro, _accesoDatos);
        }

        public bool Actualizar(RubroComercio rubro)
        {
            return _rubroComercioDAL.Actualizar(rubro, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _rubroComercioDAL.Eliminar(id, _accesoDatos);
        }
    }
}
