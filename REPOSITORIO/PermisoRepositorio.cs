using System;
using System.Collections.Generic;
using ABSTRACCIONES;
using DAL;

namespace REPOSITORIO
{
    public class PermisoRepositorio : IPermisoRepositorio
    {
        private readonly PermisoDAL _permisoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PermisoRepositorio()
            : this(null)
        {
        }

        internal PermisoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _permisoDAL = new PermisoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<IPermiso> ObtenerNodos()
        {
            return _permisoDAL.ObtenerNodos(_accesoDatos);
        }

        public List<Tuple<int, int>> ObtenerRelaciones()
        {
            return _permisoDAL.ObtenerRelaciones(_accesoDatos);
        }

        public int Guardar(IPermiso permiso)
        {
            return _permisoDAL.Guardar(permiso, _accesoDatos);
        }

        public bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones)
        {
            return _permisoDAL.ReemplazarRelaciones(relaciones, _accesoDatos);
        }

        public bool EliminarPermisos(List<int> idsPermiso)
        {
            return _permisoDAL.EliminarPermisos(idsPermiso, _accesoDatos);
        }

        public int ObtenerIdPorNombre(string nombre)
        {
            return _permisoDAL.ObtenerIdPorNombre(nombre, _accesoDatos);
        }
    }
}
