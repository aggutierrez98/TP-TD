using System;
using System.Collections.Generic;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class UsuarioHistorialSnapshot : IUsuarioSnapshot
    {
        public UsuarioHistorialSnapshot()
        {
            Permisos = new List<IPermiso>();
        }

        public int HistorialId { get; set; }

        public IUsuarioAutenticado Usuario { get; set; }

        public DateTime FechaHora { get; set; }

        public IUsuarioAutenticado ActorUsuario { get; set; }

        public string ActorUsernameSnapshot { get; set; }

        public string Accion { get; set; }

        public string Observaciones { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public bool Activo { get; set; }

        public IIdioma Idioma { get; set; }

        public IList<IPermiso> Permisos { get; set; }
    }
}
