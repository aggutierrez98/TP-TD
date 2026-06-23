using System;
using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class PermisoRepositorio : IPermisoRepositorio
    {
        private readonly PermisoDAL _permisoDAL;

        public PermisoRepositorio()
        {
            _permisoDAL = new PermisoDAL();
        }

        public List<object> ObtenerNodos(IUnidadDeTrabajo unidad = null)
        {
            return _permisoDAL.ObtenerNodos(ObtenerAccesoDatos(unidad));
        }

        public List<Tuple<int, int>> ObtenerRelaciones(IUnidadDeTrabajo unidad = null)
        {
            return _permisoDAL.ObtenerRelaciones(ObtenerAccesoDatos(unidad));
        }

        public int Guardar(object permiso, IUnidadDeTrabajo unidad = null)
        {
            return _permisoDAL.Guardar(permiso, ObtenerAccesoDatos(unidad));
        }

        public bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones, IUnidadDeTrabajo unidad = null)
        {
            return _permisoDAL.ReemplazarRelaciones(relaciones, ObtenerAccesoDatos(unidad));
        }

        public bool EliminarPermisos(List<int> idsPermiso, IUnidadDeTrabajo unidad = null)
        {
            return _permisoDAL.EliminarPermisos(idsPermiso, ObtenerAccesoDatos(unidad));
        }

        public int ObtenerIdPorNombre(string nombre, IUnidadDeTrabajo unidad = null)
        {
            return _permisoDAL.ObtenerIdPorNombre(nombre, ObtenerAccesoDatos(unidad));
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
