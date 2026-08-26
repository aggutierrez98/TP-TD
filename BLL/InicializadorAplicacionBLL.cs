namespace BLL
{
    public static class InicializadorAplicacionBLL
    {
        private static bool _inicializado;

        /** Punto unico de arranque: compone los servicios y BLL via CompositionRoot,
         * carga el idioma default y luego valida la integridad global de los datos.
         * Debe invocarse una unica vez antes de mostrar cualquier pantalla.
        **/
        public static void Inicializar()
        {
            if (_inicializado)
            {
                return;
            }

            // Se carga el idioma ANTES de validar la integridad, para que si hay
            // ExcepcionIntegridad la pantalla de contingencia se muestre ya traducida.
            CompositionRoot.Idioma.InicializarAplicacion();
            CompositionRoot.Integridad.ValidarIntegridadGlobal();

            _inicializado = true;
        }
    }
}
