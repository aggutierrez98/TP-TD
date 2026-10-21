using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de IConsultaUbicacion.
    public sealed class ConsultaUbicacion : IConsultaUbicacion
    {
        public ConsultaUbicacion(string texto, string idioma, ILimitesGeograficos zonaPreferida)
        {
            Texto = texto;
            Idioma = idioma;
            ZonaPreferida = zonaPreferida;
        }

        public string Texto { get; }

        public string Idioma { get; }

        public ILimitesGeograficos ZonaPreferida { get; }
    }
}
