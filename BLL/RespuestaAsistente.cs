namespace BLL
{
    // Resultado de un envio al asistente: la respuesta del modelo o un rechazo. El rechazo
    // no trae texto generado: trae la key de un texto fijo del catalogo de traducciones
    // (Assistant.Rejection.*), que la UI muestra en el idioma activo.
    public sealed class RespuestaAsistente
    {
        private RespuestaAsistente(string texto, string claveRechazo)
        {
            Texto = texto;
            ClaveRechazo = claveRechazo;
        }

        public string Texto { get; private set; }

        public string ClaveRechazo { get; private set; }

        public bool EsRechazo
        {
            get { return ClaveRechazo != null; }
        }

        internal static RespuestaAsistente Respondida(string texto)
        {
            return new RespuestaAsistente(texto, null);
        }

        internal static RespuestaAsistente Rechazada(string claveRechazo)
        {
            return new RespuestaAsistente(null, claveRechazo);
        }
    }
}
