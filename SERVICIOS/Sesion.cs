using ABSTRACCIONES;
using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class Sesion : ISesion
    {
        private static Sesion _instancia;

        private readonly List<System.Action<Idioma>> _observadoresRegistrados;
        private readonly BitacoraSesionService _bitacoraSesionService;
        private readonly PermisoService _permisoService;
        private IUsuarioAutenticado _usuarioActual;
        private Idioma _idiomaActual;

        private Sesion()
        {
            _observadoresRegistrados = new List<System.Action<Idioma>>();
            _bitacoraSesionService = new BitacoraSesionService();
            _permisoService = new PermisoService();
        }

        public static Sesion GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new Sesion();
            }
            return _instancia;
        }

        public Idioma IdiomaActual
        {
            get { return _idiomaActual; }
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
            _idiomaActual = null;
            _observadoresRegistrados.Clear();
        }

        public IUsuarioAutenticado GetUsuarioActual()
        {
            return _usuarioActual;
        }

        public bool HaySesionActiva()
        {
            return _usuarioActual != null;
        }

        public void ConfigurarBitacora(
            System.Func<EventoBitacora, object, int> registrador,
            System.Func<FiltroBitacora, IList<EventoBitacora>> consultor)
        {
            _bitacoraSesionService.Configurar(registrador, consultor);
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

        private void AplicarPermisosUsuarioActual(IList<IPermiso> permisos)
        {
            if (_usuarioActual == null)
            {
                return;
            }

            _usuarioActual.Permisos = permisos ?? new List<IPermiso>();
        }

        public void ActualizarPermisosUsuarioActual(object permisos)
        {
            if (_usuarioActual == null)
            {
                return;
            }

            IList<IPermiso> permisosConvertidos = permisos as IList<IPermiso>;
            AplicarPermisosUsuarioActual(permisosConvertidos ?? new List<IPermiso>());
        }

        public void RegistrarObservador(System.Action<Idioma> observador)
        {
            if (observador == null || _observadoresRegistrados.Contains(observador))
            {
                return;
            }

            _observadoresRegistrados.Add(observador);
        }

        public void DesregistrarObservador(System.Action<Idioma> observador)
        {
            if (observador == null)
            {
                return;
            }

            _observadoresRegistrados.Remove(observador);
        }

        public void ActualizarObservadores()
        {
            foreach (System.Action<Idioma> observador in _observadoresRegistrados.ToList())
            {
                observador(_idiomaActual);
            }
        }

        public void CambiarIdioma(Idioma idioma)
        {
            _idiomaActual = idioma;

            if (_idiomaActual != null)
            {
                ActualizarObservadores();
            }
        }

        public bool TienePermiso(string nombrePermiso)
        {
            if (string.IsNullOrWhiteSpace(nombrePermiso) || _usuarioActual == null || _usuarioActual.Permisos == null)
            {
                return false;
            }

            return _permisoService.ObtenerPermisosReconocidos(_usuarioActual.Permisos)
                .Any(item => string.Equals(item.Nombre, nombrePermiso, System.StringComparison.OrdinalIgnoreCase));
        }

        public IList<IPermiso> ObtenerPermisosEfectivos()
        {
            if (_usuarioActual == null || _usuarioActual.Permisos == null)
            {
                return new List<IPermiso>();
            }

            return _permisoService.ObtenerPermisosEfectivos(_usuarioActual.Permisos);
        }

        public void RegistrarOperacion(string actividad, string informacion, string origen, object contextoPersistencia = null)
        {
            RegistrarEventoBitacora(actividad, informacion, "Informacion", origen, false, contextoPersistencia);
        }

        public void RegistrarFalloNegocio(string actividad, string informacion, string origen, object contextoPersistencia = null)
        {
            RegistrarEventoBitacora(actividad, informacion, "Advertencia", origen, false, contextoPersistencia);
        }

        public void RegistrarExcepcionTecnica(string actividad, System.Exception exception, string origen, object contextoPersistencia = null)
        {
            string informacion = exception == null
                ? "Sin detalle."
                : string.Format("{0}: {1}", exception.GetType().Name, exception.Message);

            RegistrarEventoBitacora(actividad, informacion, "Error", origen, true, contextoPersistencia);
        }

        public IList<EventoBitacora> ConsultarBitacora(FiltroBitacora filtro)
        {
            return _bitacoraSesionService.Consultar(filtro);
        }

        private void RegistrarEventoBitacora(
            string actividad,
            string informacion,
            string nivel,
            string origen,
            bool esTecnica,
            object contextoPersistencia)
        {
            EventoBitacora evento = new EventoBitacora
            {
                FechaHora = System.DateTime.Now,
                UsuarioId = GetUsuarioActualId(),
                UsernameSnapshot = GetUsuarioActualNombre(),
                Actividad = actividad,
                Informacion = informacion,
                Nivel = nivel,
                Origen = origen,
                EsTecnica = esTecnica
            };

            _bitacoraSesionService.Registrar(evento, contextoPersistencia);
        }
    }
}
