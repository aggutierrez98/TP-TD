using System.Collections.Generic;
using ABSTRACCIONES;
using BE;

namespace REPOSITORIO
{
    public interface IUsuarioPermisoRepositorio
    {
        List<IPermiso> ObtenerPermisosPorUsuario(Usuario usuario);

        bool ReemplazarPermisosUsuario(Usuario usuario, List<IPermiso> permisos);
    }
}
