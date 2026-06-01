using System;
using System.Collections.Generic;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class UsuarioHistorialSnapshot : IUsuarioSnapshot
    {
        public UsuarioHistorialSnapshot()
        {
            PermisosIds = new List<int>();
        }

        public int HistorialId { get; set; }

        public int UsuarioId { get; set; }

        public DateTime FechaHora { get; set; }

        public int? ActorUsuarioId { get; set; }

        public string ActorUsernameSnapshot { get; set; }

        public string Accion { get; set; }

        public string Observaciones { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public bool Activo { get; set; }

        public int? IdiomaId { get; set; }

        public IList<int> PermisosIds { get; set; }
    }
}
