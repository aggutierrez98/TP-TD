using System;

namespace BE
{
    // Resultado independiente por usuario y tipo dentro de una corrida. Motivo es un codigo
    // estable (no texto libre) y es obligatorio para Omitida y Fallida.
    public class DetalleGeneracionNotificacion
    {
        public int Id { get; set; }

        public int CorridaId { get; set; }

        public int UsuarioId { get; set; }

        public TipoNotificacionAutomatica Tipo { get; set; }

        public DateTime Jornada { get; set; }

        public ResultadoGeneracionNotificacion Resultado { get; set; }

        public string Motivo { get; set; }

        public int? NotificacionId { get; set; }

        public DateTime FechaUtc { get; set; }
    }
}
