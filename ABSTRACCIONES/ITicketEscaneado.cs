using System;

namespace ABSTRACCIONES
{
    // Salida de IEscaneoTicketService: campos ya parseados a partir del
    // texto detectado en la imagen, mas el texto crudo por si el usuario necesita
    // completar a mano lo que no se pudo reconocer automaticamente.
    public interface ITicketEscaneado
    {
        decimal? Monto { get; }

        DateTime? Fecha { get; }

        string Descripcion { get; }

        string TextoCrudo { get; }
    }
}
