using System.Collections;
using System.Collections.Generic;
using ABSTRACCIONES;

namespace BE
{
    public class Usuario : IUsuarioAutenticado, IIntegrable
    {
        [DigitoVerificador(1)]
        public int Id { get; set; }

        [DigitoVerificador(2)]
        public string Username { get; set; }

        [DigitoVerificador(3)]
        public string PasswordHash { get; set; }

        [DigitoVerificador(4)]
        public bool Activo { get; set; }

        // Digito verificador horizontal (T07): firma de la fila persistida.
        public string DVH { get; set; }

        public IList<IPermiso> Permisos { get; set; }
        public IList PermisosNoTipados
        {
            get { return Permisos as IList; }
        }

        public Usuario()
        {
            Activo = true;
            Permisos = new List<IPermiso>();
        }

        public Usuario(string username)
            : this()
        {
            Username = username;
            Activo = true;
        }
    }
}
