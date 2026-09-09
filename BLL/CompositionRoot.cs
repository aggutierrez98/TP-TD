using ABSTRACCIONES;
using REPOSITORIO;

namespace BLL
{
    // Unica fuente y via de contruccion de las instancias de servicios y BLL detras de sus contratos.
    // Uso previsto: obtener instancias ya compuestas (getters), no como localizador en metodos.
    public static class CompositionRoot
    {
        private static bool _inicializado;

        private static ISesion _sesion;
        private static IAutorizacion _autorizacion;
        private static IPasswordHasher _passwordHasher;
        private static IIntegridadService _integridadService;
        private static IObservado _servicioIdioma;

        // BLL de caso de uso: una sola instancia de cada uno, compartida por referencia.
        private static BitacoraBLL _bitacoraBLL;
        private static CategoriaBLL _categoriaBLL;
        private static MovimientoBLL _movimientoBLL;
        private static UsuarioEstadoBLL _usuarioEstadoBLL;
        private static IntegridadBLL _integridadBLL;
        private static IdiomaBLL _idiomaBLL;
        private static PermisoBLL _permisoBLL;
        private static UsuarioBLL _usuarioBLL;
        private static BackupBLL _backupBLL;

        public static void Inicializar()
        {
            if (_inicializado)
            {
                return;
            }

            // Servicios con estado: una sola instancia (la del singleton) = fuente unica de verdad.
            _sesion = SERVICIOS.Sesion.GetInstance();
            _autorizacion = SERVICIOS.AutorizacionService.GetInstance();

            InicializacionHistorialBLL.Inicializar();

            // Servicios sin estado.
            _passwordHasher = new SERVICIOS.PasswordHasher();
            _integridadService = new SERVICIOS.IntegridadService();
            _servicioIdioma = SERVICIOS.IdiomaService.GetInstance();

            UnidadDeTrabajoFactory uow = new UnidadDeTrabajoFactory();

            _bitacoraBLL = new BitacoraBLL(new BitacoraRepositorio(), uow, _sesion);

            _categoriaBLL = new CategoriaBLL(new CategoriaRepositorio(), uow, _sesion, _bitacoraBLL);

            _movimientoBLL = new MovimientoBLL(new MovimientoRepositorio(), new CategoriaRepositorio(), uow, _sesion, _bitacoraBLL);

            _usuarioEstadoBLL = new UsuarioEstadoBLL(
                new UsuarioRepositorio(),
                new UsuarioPermisoRepositorio(),
                new UsuarioPreferenciaRepositorio(),
                new PermisoRepositorio(),
                uow,
                _sesion);

            _integridadBLL = new IntegridadBLL(
                _integridadService,
                new IntegridadRepositorio(),
                new UsuarioRepositorio(),
                _bitacoraBLL,
                uow);

            _idiomaBLL = new IdiomaBLL(
                new IdiomaRepositorio(),
                new UsuarioPreferenciaRepositorio(),
                new TraduccionRepositorio(),
                new SERVICIOS.IdiomaCatalogoService(),
                _usuarioEstadoBLL,
                _bitacoraBLL,
                uow,
                _servicioIdioma);

            _permisoBLL = new PermisoBLL(
                new PermisoRepositorio(),
                new UsuarioPermisoRepositorio(),
                new SERVICIOS.PermisoService(),
                new UsuarioRepositorio(),
                _usuarioEstadoBLL,
                _bitacoraBLL,
                uow,
                _sesion,
                _autorizacion);

            _usuarioBLL = new UsuarioBLL(
                new UsuarioRepositorio(),
                new PasswordHashService(),
                _idiomaBLL,
                _permisoBLL,
                _usuarioEstadoBLL,
                _integridadBLL,
                _bitacoraBLL,
                uow,
                _sesion,
                _autorizacion);

            _backupBLL = new BackupBLL(new BackupRepositorio(), _bitacoraBLL);

            _inicializado = true;
        }

        public static ISesion Sesion
        {
            get { Inicializar(); return _sesion; }
        }

        public static IBitacora Bitacora
        {
            get { Inicializar(); return _bitacoraBLL; }
        }

        public static IAutorizacion Autorizacion
        {
            get { Inicializar(); return _autorizacion; }
        }

        public static IPasswordHasher PasswordHasher
        {
            get { Inicializar(); return _passwordHasher; }
        }

        public static IIntegridadService IntegridadService
        {
            get { Inicializar(); return _integridadService; }
        }

        public static IObservado ServicioIdioma
        {
            get { Inicializar(); return _servicioIdioma; }
        }

        public static UsuarioBLL Usuario
        {
            get { Inicializar(); return _usuarioBLL; }
        }

        public static PermisoBLL Permiso
        {
            get { Inicializar(); return _permisoBLL; }
        }

        public static IdiomaBLL Idioma
        {
            get { Inicializar(); return _idiomaBLL; }
        }

        public static IntegridadBLL Integridad
        {
            get { Inicializar(); return _integridadBLL; }
        }

        public static BitacoraBLL GestorBitacora
        {
            get { Inicializar(); return _bitacoraBLL; }
        }

        public static CategoriaBLL Categoria
        {
            get { Inicializar(); return _categoriaBLL; }
        }

        public static MovimientoBLL Movimiento
        {
            get { Inicializar(); return _movimientoBLL; }
        }

        public static BackupBLL Backup
        {
            get { Inicializar(); return _backupBLL; }
        }
    }
}
