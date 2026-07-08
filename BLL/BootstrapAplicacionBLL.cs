namespace BLL
{
    public static class BootstrapAplicacionBLL
    {
        private static readonly object Sync = new object();
        private static bool _inicializado;

        /** Punto unico de arranque: configura los servicios de infraestructura (bitacora, historial),
         * carga el idioma default y luego valida la integridad global de los datos.
         * Debe invocarse una unica vez antes de mostrar cualquier pantalla.
        **/
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

                InicializacionBitacoraBLL.Inicializar();
                InicializacionHistorialBLL.Inicializar();

                // El idioma default se carga antes de validar integridad para que
                // la pantalla de contingencia pueda mostrarse traducida.
                new IdiomaBLL().InicializarAplicacion();
                new IntegridadBLL().ValidarIntegridadGlobal();

                _inicializado = true;
            }
        }
    }
}
