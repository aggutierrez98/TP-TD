using System;

namespace ABSTRACCIONES
{
    public interface IFiltroBitacora
    {
        DateTime? FechaDesde { get; set; }

        DateTime? FechaHasta { get; set; }

        string Usuario { get; set; }

        string Actividad { get; set; }

        string Nivel { get; set; }

        bool? EsTecnica { get; set; }
    }
}
