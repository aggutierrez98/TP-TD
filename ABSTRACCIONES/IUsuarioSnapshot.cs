using System;
using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IUsuarioSnapshot
    {
        int HistorialId { get; set; }

        IUsuarioAutenticado Usuario { get; set; }

        DateTime FechaHora { get; set; }

        IUsuarioAutenticado ActorUsuario { get; set; }

        string ActorUsernameSnapshot { get; set; }

        string Accion { get; set; }

        string Observaciones { get; set; }

        string Username { get; set; }

        string PasswordHash { get; set; }

        bool Activo { get; set; }

        IIdioma Idioma { get; set; }

        IList<IPermiso> Permisos { get; set; }
    }
}
