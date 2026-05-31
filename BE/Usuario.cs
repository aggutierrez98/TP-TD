using System.Collections.Generic;
using ABSTRACCIONES;

namespace BE
{
    public class Usuario : IUsuarioAutenticado
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public IList<IPermiso> Permisos { get; set; }

        public Usuario()
        {
            Permisos = new List<IPermiso>();
        }

        public Usuario(string username)
            : this()
        {
            Username = username;
        }
    }
}
