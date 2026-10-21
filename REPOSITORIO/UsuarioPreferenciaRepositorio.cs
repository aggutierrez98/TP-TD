using DAL;
using SERVICIOS;
using ABSTRACCIONES;

namespace REPOSITORIO
{
    public class UsuarioPreferenciaRepositorio : IUsuarioPreferenciaRepositorio
    {
        private readonly UsuarioPreferenciaDAL _usuarioPreferenciaDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public UsuarioPreferenciaRepositorio()
            : this(null)
        {
        }

        internal UsuarioPreferenciaRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _usuarioPreferenciaDAL = new UsuarioPreferenciaDAL();
            _accesoDatos = accesoDatos;
        }

        public UsuarioPreferencia ObtenerPorUsuario(IUsuarioAutenticado usuario)
        {
            return _usuarioPreferenciaDAL.ObtenerPorUsuario(usuario, _accesoDatos);
        }

        public bool Guardar(UsuarioPreferencia preferencia)
        {
            return _usuarioPreferenciaDAL.Guardar(preferencia, _accesoDatos);
        }

        public bool Guardar(IUsuarioAutenticado usuario, IIdioma idioma)
        {
            return _usuarioPreferenciaDAL.Guardar(usuario, idioma, _accesoDatos);
        }
    }
}
