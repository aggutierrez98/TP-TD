namespace BLL
{
    public static class BootstrapAplicacionBLL
    {
        private static readonly object Sync = new object();
        private static bool _inicializado;

        /** Punto unico de arranque: configura los servicios de infraestructura (bitacora, historial) 
         * y luego valida la integridad global de los datos. Debe invocarse una unica vez antes de mostrar cualquier pantalla 
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
                _inicializado = true;
            }

            new IntegridadBLL().ValidarIntegridadGlobal();
        }
    }
}
