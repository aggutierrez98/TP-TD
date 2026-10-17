using System;

namespace BE
{
    // Una fila = un intercambio completo (pregunta + respuesta ya resuelta por el
    // asistente); se inserta una unica vez cuando ya se tiene la respuesta de Gemini,
    // nunca en dos pasos. Usa FKs planas (ChatbotFinancieroId/UsuarioId) en lugar de
    // navegacion a BE anidado: a diferencia de ReglaCategorizacion, no hace falta
    // hidratar Usuario/ChatbotFinanciero completos para el uso actual (solo chat).
    public class ConsultaChatbot
    {
        public int Id { get; set; }

        public int ChatbotFinancieroId { get; set; }

        public int UsuarioId { get; set; }

        public string Pregunta { get; set; }

        public string Respuesta { get; set; }

        public DateTime Fecha { get; set; }

        // Reservado a futuro: entradas proactivas del bot sin pregunta de usuario
        // (ej. alertas o sugerencias espontaneas). Por ahora siempre true, porque
        // todo intercambio actual nace de una pregunta del usuario.
        public bool EsPregunta { get; set; }
    }
}
