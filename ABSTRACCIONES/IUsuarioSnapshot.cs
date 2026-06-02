using System;
using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IUsuarioSnapshot
    {
        int HistorialId { get; set; }

        int UsuarioId { get; set; }

        DateTime FechaHora { get; set; }

        int? ActorUsuarioId { get; set; }

        string ActorUsernameSnapshot { get; set; }

        string Accion { get; set; }

        string Observaciones { get; set; }

        string Username { get; set; }

        string PasswordHash { get; set; }

        bool Activo { get; set; }

        int? IdiomaId { get; set; }

        IList<int> PermisosIds { get; set; }
    }
}
