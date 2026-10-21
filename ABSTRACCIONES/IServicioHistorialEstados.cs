using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IServicioHistorialEstados
    {
        IList<IUsuarioSnapshot> ObtenerVersionesUsuario(IUsuarioAutenticado usuario);

        IUsuarioSnapshot ObtenerVersionUsuario(IUsuarioSnapshot snapshot);
    }
}
