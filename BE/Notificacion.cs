using System;

namespace BE
{
    public class Notificacion
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public string Mensaje { get; set; }

        public DateTime Fecha { get; set; }

        public bool Leida { get; set; }

        // Metadata de automatizacion (todas nulas en avisos manuales y recordatorios individuales).
        // TipoAutomatico y FechaJornada van juntos y limitan a un aviso por usuario, tipo y jornada;
        // ClaveEjemplo identifica un ejemplo de laboratorio y nunca convive con TipoAutomatico.
        public TipoNotificacionAutomatica? TipoAutomatico { get; set; }

        public DateTime? FechaJornada { get; set; }

        public string ClaveEjemplo { get; set; }
    }
}
