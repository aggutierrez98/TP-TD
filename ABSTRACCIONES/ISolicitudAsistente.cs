using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Pedido al servicio de respuesta del asistente: la categoria ya admitida por el BLL
    // decide que especialista responde, el historial solo trae intercambios admitidos y el
    // contexto, los datos permitidos del usuario actual.
    public interface ISolicitudAsistente
    {
        CategoriaConsulta Categoria { get; }

        string Pregunta { get; }

        IReadOnlyList<IIntercambioChatbot> Historial { get; }

        IContextoAsistente Contexto { get; }
    }
}
