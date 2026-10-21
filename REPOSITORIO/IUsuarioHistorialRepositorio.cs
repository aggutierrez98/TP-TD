using System.Collections.Generic;
using SERVICIOS;
using ABSTRACCIONES;

namespace REPOSITORIO
{
    public interface IUsuarioHistorialRepositorio
    {
        int GuardarVersion(UsuarioHistorialSnapshot snapshot);

        List<UsuarioHistorialSnapshot> ObtenerVersiones(IUsuarioAutenticado usuario);

        UsuarioHistorialSnapshot ObtenerPorId(int id);
    }
}
