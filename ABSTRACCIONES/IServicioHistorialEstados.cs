using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IServicioHistorialEstados
    {
        IList<IUsuarioSnapshot> ObtenerVersionesUsuario(int usuarioId);

        IUsuarioSnapshot ObtenerVersionUsuario(int historialId);
    }
}
