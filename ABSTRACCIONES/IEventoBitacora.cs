using System;

namespace ABSTRACCIONES
{
    public interface IEventoBitacora
    {
        int Id { get; set; }

        DateTime FechaHora { get; set; }

        int? UsuarioId { get; set; }

        string UsernameSnapshot { get; set; }

        string Actividad { get; set; }

        string Informacion { get; set; }

        string Nivel { get; set; }

        string Origen { get; set; }

        bool EsTecnica { get; set; }
    }
}
