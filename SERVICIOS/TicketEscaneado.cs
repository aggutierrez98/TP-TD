using System;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de ITicketEscaneado.
    public class TicketEscaneado : ITicketEscaneado
    {
        public decimal? Monto { get; set; }

        public DateTime? Fecha { get; set; }

        public string Descripcion { get; set; }

        public string TextoCrudo { get; set; }
    }
}
