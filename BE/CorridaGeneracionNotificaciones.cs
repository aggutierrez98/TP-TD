using System;
using System.Collections.Generic;

namespace BE
{
    // Registro tecnico de una corrida del proceso automatico: inicio/fin UTC, jornada local
    // evaluada y cantidades. No guarda prompts, secretos ni respuestas privadas.
    public class CorridaGeneracionNotificaciones
    {
        public int Id { get; set; }

        public DateTime InicioUtc { get; set; }

        public DateTime? FinUtc { get; set; }

        public DateTime Jornada { get; set; }

        public int Entregadas { get; set; }

        public int Omitidas { get; set; }

        public int Fallidas { get; set; }

        public List<DetalleGeneracionNotificacion> Detalles { get; set; } = new List<DetalleGeneracionNotificacion>();
    }
}
