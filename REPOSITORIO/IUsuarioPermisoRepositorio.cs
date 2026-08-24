using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IUsuarioPermisoRepositorio
    {
        List<int> ObtenerPermisoIdsPorUsuario(int usuarioId);

        bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId);
    }
}
