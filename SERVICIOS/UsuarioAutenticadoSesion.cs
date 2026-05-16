using System.Collections.Generic;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class UsuarioAutenticadoSesion : IUsuarioAutenticado
    {
        public UsuarioAutenticadoSesion()
        {
            Permisos = new List<IPermiso>();
        }

        public int Id { get; set; }

        public string Username { get; set; }

        public IList<IPermiso> Permisos { get; set; }
    }
}
