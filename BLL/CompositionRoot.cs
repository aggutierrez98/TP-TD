using ABSTRACCIONES;

namespace BLL
{
    // Composition root: unica fuente de las instancias de servicios detras de sus contratos.
    // Se dispara una vez desde Program.Main; los getters tambien auto-inicializan.
    // Reemplaza al antiguo BootstrapAplicacionBLL: aca se cablea la persistencia de las
    // utilidades transversales y se exponen los servicios como contratos para inyeccion.
    // Uso previsto: construir dependencias (ctores de BLL), no como localizador en metodos.
    public static class CompositionRoot
    {
        private static readonly object Sync = new object();
        private static bool _inicializado;

        private static ISesion _sesion;
        private static IBitacora _bitacora;
        private static IAutorizacion _autorizacion;
        private static IPasswordHasher _passwordHasher;
        private static IIntegridadService _integridadService;
        private static IObservado _servicioIdioma;

        public static void Inicializar()
        {
            if (_inicializado)
            {
                return;
            }

            lock (Sync)
            {
                if (_inicializado)
                {
                    return;
                }

                // Servicios con estado: una sola instancia (la del singleton) = fuente unica de verdad.
                _sesion = SERVICIOS.Sesion.GetInstance();
                _autorizacion = SERVICIOS.AutorizacionService.GetInstance();

                // Cablear la persistencia de las utilidades transversales antes de exponerlas.
                InicializacionBitacoraBLL.Inicializar();
                InicializacionHistorialBLL.Inicializar();
                _bitacora = SERVICIOS.BitacoraService.GetInstance();

                // Servicios sin estado.
                _passwordHasher = new SERVICIOS.PasswordHasher();
                _integridadService = new SERVICIOS.IntegridadService();
                _servicioIdioma = SERVICIOS.IdiomaService.GetInstance();

                _inicializado = true;
            }
        }

        public static ISesion Sesion
        {
            get { Inicializar(); return _sesion; }
        }

        public static IBitacora Bitacora
        {
            get { Inicializar(); return _bitacora; }
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
    }
}
