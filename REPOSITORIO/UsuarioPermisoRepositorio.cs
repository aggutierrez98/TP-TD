using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class UsuarioPermisoRepositorio : IUsuarioPermisoRepositorio
    {
        private readonly UsuarioPermisoDAL _usuarioPermisoDAL;

        public UsuarioPermisoRepositorio()
        {
            _usuarioPermisoDAL = new UsuarioPermisoDAL();
        }

        public List<int> ObtenerPermisoIdsPorUsuario(int usuarioId, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioPermisoDAL.ObtenerPermisoIdsPorUsuario(usuarioId, ObtenerAccesoDatos(unidad));
        }

        public bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioPermisoDAL.ReemplazarPermisosUsuario(usuarioId, permisosId, ObtenerAccesoDatos(unidad));
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
