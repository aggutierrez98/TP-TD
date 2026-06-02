using System;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class EventoBitacora : IEventoBitacora
    {
        public int Id { get; set; }

        public DateTime FechaHora { get; set; }

        public int? UsuarioId { get; set; }

        public string UsernameSnapshot { get; set; }

        public string Actividad { get; set; }

        public string Informacion { get; set; }

        public string Nivel { get; set; }

        public string Origen { get; set; }

        public bool EsTecnica { get; set; }
    }
}
