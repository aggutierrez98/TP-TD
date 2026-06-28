using DAL;
using SERVICIOS;

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

        public UsuarioPreferencia ObtenerPorUsuarioId(int usuarioId)
        {
            return _usuarioPreferenciaDAL.ObtenerPorUsuarioId(usuarioId, _accesoDatos);
        }

        public bool Guardar(UsuarioPreferencia preferencia)
        {
            return _usuarioPreferenciaDAL.Guardar(preferencia, _accesoDatos);
        }

        public bool Guardar(int usuarioId, int? idiomaId)
        {
            return _usuarioPreferenciaDAL.Guardar(usuarioId, idiomaId, _accesoDatos);
        }
    }
}
