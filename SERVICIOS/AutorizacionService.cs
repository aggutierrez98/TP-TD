using ABSTRACCIONES;
using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class AutorizacionService
    {
        private static AutorizacionService _instancia;

        private readonly PermisoService _permisoService;
        private IList<IPermiso> _permisosUsuarioActual;

        private AutorizacionService()
        {
            _permisoService = new PermisoService();
            _permisosUsuarioActual = new List<IPermiso>();
        }

        public static AutorizacionService GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new AutorizacionService();
            }

            return _instancia;
        }

        public void ActualizarPermisosUsuarioActual(IEnumerable<IPermiso> permisos)
        {
            _permisosUsuarioActual = permisos == null
                ? new List<IPermiso>()
                : permisos.ToList();
        }

        public void Limpiar()
        {
            _permisosUsuarioActual = new List<IPermiso>();
        }

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
