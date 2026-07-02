using ABSTRACCIONES;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class AutorizacionService : IAutorizacion
    {
        private static AutorizacionService _instancia;

        private readonly PermisoService _permisoService;
        private IList<IPermiso> _permisosUsuarioActual;

        private AutorizacionService()
        {
            _permisoService = new PermisoService();
            _permisosUsuarioActual = new List<IPermiso>();
        }

        /// <summary>
        /// Metodo para obtener la instancia unica del servicio de autorizacion.
        /// Mantiene en memoria los permisos del usuario actualmente autenticado.
        /// </summary>
        /// <returns>La instancia unica de <see cref="AutorizacionService" />.</returns>
        public static AutorizacionService GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new AutorizacionService();
            }

            return _instancia;
        }

        private void ActualizarPermisosUsuarioActual(IList<IPermiso> permisos)
        {
            _permisosUsuarioActual = permisos == null
                ? new List<IPermiso>()
                : permisos.ToList();
        }

        /// <summary>
        /// Metodo para reemplazar los permisos asociados al usuario actual.
        /// Usado despues del login o cuando cambian los permisos disponibles en sesion.
        /// </summary>
        /// <param name="permisos">La coleccion de permisos tipados como <see cref="IPermiso" />.</param>
        public void ActualizarPermisosUsuarioActual(IEnumerable permisos)
        {
            _permisosUsuarioActual = permisos == null
                ? new List<IPermiso>()
                : permisos.Cast<object>().OfType<IPermiso>().ToList();
        }

        /// <summary>
        /// Metodo para limpiar los permisos cargados del usuario actual.
        /// Usado al cerrar sesion o reiniciar el contexto de autorizacion.
        /// </summary>
        public void Limpiar()
        {
            _permisosUsuarioActual = new List<IPermiso>();
        }

        /// <summary>
        /// Metodo para verificar si el usuario actual tiene un permiso determinado.
        /// Compara contra las patentes efectivas resultantes del arbol de permisos.
        /// </summary>
        /// <param name="rutaPermiso">La ruta o nombre normalizado del permiso requerido.</param>
        /// <returns><see langword="true" /> si el permiso esta disponible; en caso contrario, <see langword="false" />.</returns>
        public bool TienePermiso(string rutaPermiso)
        {
            if (string.IsNullOrWhiteSpace(rutaPermiso) || _permisosUsuarioActual == null || _permisosUsuarioActual.Count == 0)
            {
                return false;
            }

            string permisoBuscado = _permisoService.NormalizarNombre(rutaPermiso);

            return _permisoService.ObtenerPermisosEfectivos(_permisosUsuarioActual)
                .Any(item => _permisoService.NormalizarNombre(item.Nombre) == permisoBuscado);
        }
    }
}
