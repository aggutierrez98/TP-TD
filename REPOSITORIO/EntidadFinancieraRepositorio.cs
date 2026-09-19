using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class EntidadFinancieraRepositorio : IEntidadFinancieraRepositorio
    {
        private readonly EntidadFinancieraDAL _entidadFinancieraDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public EntidadFinancieraRepositorio()
            : this(null)
        {
        }

        internal EntidadFinancieraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _entidadFinancieraDAL = new EntidadFinancieraDAL();
            _accesoDatos = accesoDatos;
        }

        public EntidadFinanciera ObtenerPorId(int id)
        {
            return _entidadFinancieraDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<EntidadFinanciera> ObtenerTodas()
        {
            return _entidadFinancieraDAL.ObtenerTodas(_accesoDatos);
        }

        public int InsertarYObtenerId(EntidadFinanciera entidad)
        {
            return _entidadFinancieraDAL.InsertarYObtenerId(entidad, _accesoDatos);
        }

        public bool Actualizar(EntidadFinanciera entidad)
        {
            return _entidadFinancieraDAL.Actualizar(entidad, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _entidadFinancieraDAL.Eliminar(id, _accesoDatos);
        }
    }
}
