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

        public void ActualizarPermisosUsuarioActual(object permisos)
        {
            IList<IPermiso> permisosConvertidos = permisos as IList<IPermiso>;
            _permisosUsuarioActual = permisosConvertidos ?? new List<IPermiso>();
        }

        public void Limpiar()
        {
            _permisosUsuarioActual = new List<IPermiso>();
        }

        public bool TienePermiso(string nombrePermiso)
        {
            if (string.IsNullOrWhiteSpace(nombrePermiso) || _permisosUsuarioActual == null || _permisosUsuarioActual.Count == 0)
            {
                return false;
            }

            return _permisoService.ObtenerPermisosReconocidos(_permisosUsuarioActual)
                .Any(item => string.Equals(item.Nombre, nombrePermiso, System.StringComparison.OrdinalIgnoreCase));
        }

        public object ObtenerPermisosEfectivos()
        {
            if (_permisosUsuarioActual == null || _permisosUsuarioActual.Count == 0)
            {
                return new List<IPermiso>();
            }

            return _permisoService.ObtenerPermisosEfectivos(_permisosUsuarioActual);
        }
    }
}
