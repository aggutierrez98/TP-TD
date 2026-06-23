using System.Collections.Generic;

namespace REPOSITORIO
{
    public interface IUsuarioPermisoRepositorio
    {
        List<int> ObtenerPermisoIdsPorUsuario(int usuarioId, IUnidadDeTrabajo unidad = null);

        bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId, IUnidadDeTrabajo unidad = null);
    }
}
