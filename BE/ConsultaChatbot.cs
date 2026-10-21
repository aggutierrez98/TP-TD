using System;

namespace BE
{
    // Una fila = un intercambio completo (pregunta + respuesta ya resuelta por el
    // asistente); se inserta una unica vez cuando ya se tiene la respuesta de Gemini,
    // nunca en dos pasos.
    public class ConsultaChatbot
    {
        public int Id { get; set; }

        public ChatbotFinanciero ChatbotFinanciero { get; set; }

        public Usuario Usuario { get; set; }

        public string Pregunta { get; set; }

        public string Respuesta { get; set; }

        public DateTime Fecha { get; set; }

        // Reservado a futuro: entradas proactivas del bot sin pregunta de usuario
        // (ej. alertas o sugerencias espontaneas). Por ahora siempre true, porque
        // todo intercambio actual nace de una pregunta del usuario.
        public bool EsPregunta { get; set; }
    }
}
