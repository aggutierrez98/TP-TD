namespace BLL
{
    public static class InicializadorAplicacionBLL
    {
        public static void Inicializar()
        {
            // Se carga el idioma ANTES de validar la integridad, para que si hay ExcepcionIntegridad se muestre ya traducida
            CompositionRoot.Idioma.InicializarAplicacion();
            CompositionRoot.Integridad.ValidarIntegridadGlobal();
        }
    }
}
