using System;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class FiltroBitacora : IFiltroBitacora
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public string Usuario { get; set; }

        public string Actividad { get; set; }

        public string Nivel { get; set; }

        public bool? EsTecnica { get; set; }
    }
}
