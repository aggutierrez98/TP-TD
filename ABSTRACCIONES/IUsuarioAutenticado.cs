using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IUsuarioAutenticado
    {
        int Id { get; set; }

        string Username { get; set; }

        IList<IPermiso> Permisos { get; set; }
    }
}
