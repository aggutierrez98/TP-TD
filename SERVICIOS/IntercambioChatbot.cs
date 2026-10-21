using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de IIntercambioChatbot.
    public class IntercambioChatbot : IIntercambioChatbot
    {
        public string Pregunta { get; set; }

        public string Respuesta { get; set; }
    }
}
