using System.Collections.Generic;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de ISolicitudAsistente.
    public class SolicitudAsistente : ISolicitudAsistente
    {
        public SolicitudAsistente(
            CategoriaConsulta categoria,
            string pregunta,
            IReadOnlyList<IIntercambioChatbot> historial,
            IContextoAsistente contexto)
        {
            Categoria = categoria;
            Pregunta = pregunta;
            Historial = historial ?? new IIntercambioChatbot[0];
            Contexto = contexto ?? ContextoAsistente.Vacio;
        }

        public CategoriaConsulta Categoria { get; private set; }

        public string Pregunta { get; private set; }

        public IReadOnlyList<IIntercambioChatbot> Historial { get; private set; }

        public IContextoAsistente Contexto { get; private set; }
    }
}
