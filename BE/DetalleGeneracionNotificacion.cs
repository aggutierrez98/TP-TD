using System;

namespace BE
{
    // Resultado independiente por usuario y tipo dentro de una corrida. Motivo es un codigo
    // estable (no texto libre) y es obligatorio para Omitida y Fallida.
    public class DetalleGeneracionNotificacion
    {
        public int Id { get; set; }

        public CorridaGeneracionNotificaciones Corrida { get; set; }

        public Usuario Usuario { get; set; }

        public TipoNotificacionAutomatica Tipo { get; set; }

        public DateTime Jornada { get; set; }

        public ResultadoGeneracionNotificacion Resultado { get; set; }

        public string Motivo { get; set; }

        public Notificacion Notificacion { get; set; }

        public DateTime FechaUtc { get; set; }
    }
}
