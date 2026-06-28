using System.Collections.Generic;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IUsuarioHistorialRepositorio
    {
        int GuardarVersion(UsuarioHistorialSnapshot snapshot);

        List<UsuarioHistorialSnapshot> ObtenerVersiones(int usuarioId);

        UsuarioHistorialSnapshot ObtenerVersion(int historialId);
    }
}
