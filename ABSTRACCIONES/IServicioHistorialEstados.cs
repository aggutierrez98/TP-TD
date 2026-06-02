using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IServicioHistorialEstados
    {
        int GuardarVersionUsuario(IUsuarioSnapshot snapshot, object contextoPersistencia = null);

        IList<IUsuarioSnapshot> ObtenerVersionesUsuario(int usuarioId);

        IUsuarioSnapshot ObtenerVersionUsuario(int historialId);
    }
}
