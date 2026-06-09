namespace BLL
{
    public static class BootstrapAplicacionBLL
    {
        private static readonly object Sync = new object();
        private static bool _inicializado;

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
        }
    }
}
