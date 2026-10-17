using System;

namespace ABSTRACCIONES
{
    // DTO de salida de IEscaneoTicketService: campos ya parseados a partir del
    // texto detectado en la imagen, mas el texto crudo por si el usuario necesita
    // completar a mano lo que no se pudo reconocer automaticamente.
    public class TicketEscaneado
    {
        public decimal? Monto { get; set; }

        public DateTime? Fecha { get; set; }

        public string Descripcion { get; set; }

        public string TextoCrudo { get; set; }
    }
}
