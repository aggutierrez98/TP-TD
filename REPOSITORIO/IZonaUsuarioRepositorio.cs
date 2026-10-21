using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IZonaUsuarioRepositorio
    {
        ZonaUsuario ObtenerPorId(int id);

        List<ZonaUsuario> ObtenerPorUsuario(Usuario usuario);

        int InsertarYObtenerId(ZonaUsuario zona);

        bool Eliminar(ZonaUsuario zona);
    }
}
