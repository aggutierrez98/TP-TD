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
    }
}
