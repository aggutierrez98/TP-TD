using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Pedido al servicio de respuesta del asistente: la categoria ya admitida por el BLL
    // decide que especialista responde, el historial solo trae intercambios admitidos y el
    // contexto, los datos permitidos del usuario actual.
    public class SolicitudAsistente
    {
        public SolicitudAsistente(
            CategoriaConsulta categoria,
            string pregunta,
            IReadOnlyList<IntercambioChatbot> historial,
            ContextoAsistente contexto)
        {
            Categoria = categoria;
            Pregunta = pregunta;
            Historial = historial ?? new IntercambioChatbot[0];
            Contexto = contexto ?? ContextoAsistente.Vacio;
        }

        public CategoriaConsulta Categoria { get; private set; }

        public string Pregunta { get; private set; }

        public IReadOnlyList<IntercambioChatbot> Historial { get; private set; }

        public ContextoAsistente Contexto { get; private set; }
    }
}
