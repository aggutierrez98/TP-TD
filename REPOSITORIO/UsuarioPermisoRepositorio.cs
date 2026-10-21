using System.Collections.Generic;
using DAL;
using ABSTRACCIONES;
using BE;

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

        public List<IPermiso> ObtenerPermisosPorUsuario(Usuario usuario)
        {
            return _usuarioPermisoDAL.ObtenerPermisosPorUsuario(usuario, _accesoDatos);
        }

        public bool ReemplazarPermisosUsuario(Usuario usuario, List<IPermiso> permisos)
        {
            return _usuarioPermisoDAL.ReemplazarPermisosUsuario(usuario, permisos, _accesoDatos);
        }
    }
}
