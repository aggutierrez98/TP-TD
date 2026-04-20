using ABSTRACCIONES;
using BE;

namespace SERVICIOS
{
    public class Sesion : ISesion
    {
        private static Sesion _instancia;

        private Usuario _usuarioActual;

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

        public void IniciarSesion(object usuario)
        {

            _usuarioActual = usuario as Usuario;
        }

        public void CerrarSesion()
        {
            _usuarioActual = null;
        }

        public object GetUsuarioActual()
        {
            return _usuarioActual;
        }

        public bool HaySesionActiva()
        {
            return _usuarioActual != null;
        }

        public void IniciarSesion(Usuario usuario)
        {
            _usuarioActual = usuario;
        }

        public Usuario GetUsuarioActualUsuario()
        {
            return _usuarioActual;
        }
    }
}
