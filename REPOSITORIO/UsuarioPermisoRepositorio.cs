using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class UsuarioPermisoRepositorio : IUsuarioPermisoRepositorio
    {
        private readonly UsuarioPermisoDAL _usuarioPermisoDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public UsuarioPermisoRepositorio()
            : this(null)
        {
        }

        internal UsuarioPermisoRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _usuarioPermisoDAL = new UsuarioPermisoDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerPermisoIdsPorUsuario(int usuarioId)
        {
            return _usuarioPermisoDAL.ObtenerPermisoIdsPorUsuario(usuarioId, _accesoDatos);
        }

        public bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId)
        {
            return _usuarioPermisoDAL.ReemplazarPermisosUsuario(usuarioId, permisosId, _accesoDatos);
        }
    }
}
