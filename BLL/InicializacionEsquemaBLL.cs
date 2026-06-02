namespace BLL
{
    public static class InicializacionEsquemaBLL
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

                InicializadorEsquemaBLL.AsegurarEsquema();
                _inicializado = true;
            }
        }
    }
}
