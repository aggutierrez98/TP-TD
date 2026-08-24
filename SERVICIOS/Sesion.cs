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

        /// <summary>
        /// Metodo para obtener la instancia unica del servicio de sesion.
        /// Mantiene el usuario autenticado actual en memoria.
        /// </summary>
        /// <returns>La instancia unica de <see cref="Sesion" />.</returns>
        public static Sesion GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new Sesion();
            }

            return _instancia;
        }

        /// <summary>
        /// Metodo para iniciar sesion con un usuario autenticado ya construido.
        /// Reemplaza cualquier usuario previamente asociado a la sesion.
        /// </summary>
        /// <param name="usuario">El usuario autenticado que quedara como usuario actual.</param>
        public void IniciarSesion(IUsuarioAutenticado usuario)
        {
            _usuarioActual = usuario;
        }

        /// <summary>
        /// Metodo para iniciar sesion a partir del identificador y nombre de usuario.
        /// Crea una representacion liviana del usuario autenticado para la sesion.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario autenticado.</param>
        /// <param name="username">El nombre del usuario autenticado.</param>
        public void IniciarSesion(int usuarioId, string username)
        {
            _usuarioActual = new UsuarioAutenticadoSesion
            {
                Id = usuarioId,
                Username = username
            };
        }

        /// <summary>
        /// Metodo para cerrar la sesion actual.
        /// Elimina la referencia al usuario autenticado en memoria.
        /// </summary>
        public void CerrarSesion()
        {
            _usuarioActual = null;
        }

        /// <summary>
        /// Metodo para obtener el usuario autenticado actual.
        /// Devuelve <see langword="null" /> cuando no hay sesion activa.
        /// </summary>
        /// <returns>El usuario autenticado actual.</returns>
        public IUsuarioAutenticado GetUsuarioActual()
        {
            return _usuarioActual;
        }

        /// <summary>
        /// Metodo para obtener el identificador del usuario autenticado actual.
        /// Devuelve <see langword="null" /> cuando no hay sesion activa.
        /// </summary>
        /// <returns>El identificador del usuario actual.</returns>
        public int? GetUsuarioActualId()
        {
            return _usuarioActual == null ? (int?)null : _usuarioActual.Id;
        }

        /// <summary>
        /// Metodo para obtener el nombre del usuario autenticado actual.
        /// Devuelve <see langword="null" /> cuando no hay sesion activa.
        /// </summary>
        /// <returns>El nombre del usuario actual.</returns>
        public string GetUsuarioActualNombre()
        {
            return _usuarioActual == null ? null : _usuarioActual.Username;
        }

        /// <summary>
        /// Metodo para actualizar el nombre del usuario actualmente autenticado.
        /// No realiza cambios cuando no hay sesion activa.
        /// </summary>
        /// <param name="username">El nuevo nombre del usuario actual.</param>
        public void ActualizarUsuarioActualNombre(string username)
        {
            if (_usuarioActual == null)
            {
                return;
            }

            _usuarioActual.Username = username;
        }

        /// <summary>
        /// Metodo para indicar si existe una sesion activa.
        /// La sesion se considera activa cuando hay un usuario autenticado en memoria.
        /// </summary>
        /// <returns><see langword="true" /> si hay sesion activa; en caso contrario, <see langword="false" />.</returns>
        public bool HaySesionActiva()
        {
            return _usuarioActual != null;
        }
    }
}
