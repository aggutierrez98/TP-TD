using ABSTRACCIONES;

namespace SERVICIOS
{
    public class Sesion : ISesion
    {
        private static Sesion _instancia;

        private IUsuarioAutenticado _usuarioActual;

        private Sesion()
        {
        }

        public static Sesion GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new Sesion();
            }

            return _instancia;
        }

        public void IniciarSesion(IUsuarioAutenticado usuario)
        {
            _usuarioActual = usuario;
        }

        public void IniciarSesion(int usuarioId, string username)
        {
            _usuarioActual = new UsuarioAutenticadoSesion
            {
                Id = usuarioId,
                Username = username
            };
        }

        public void CerrarSesion()
        {
            _usuarioActual = null;
        }

        public IUsuarioAutenticado GetUsuarioActual()
        {
            return _usuarioActual;
        }

        public int? GetUsuarioActualId()
        {
            return _usuarioActual == null ? (int?)null : _usuarioActual.Id;
        }

        public string GetUsuarioActualNombre()
        {
            return _usuarioActual == null ? null : _usuarioActual.Username;
        }

        public void ActualizarUsuarioActualNombre(string username)
        {
            if (_usuarioActual == null)
            {
                return;
            }

            _usuarioActual.Username = username;
        }

        public bool HaySesionActiva()
        {
            return _usuarioActual != null;
        }
    }
}
