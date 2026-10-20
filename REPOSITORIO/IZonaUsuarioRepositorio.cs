using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IZonaUsuarioRepositorio
    {
        ZonaUsuario ObtenerPorId(int id);

        List<ZonaUsuario> ObtenerPorUsuario(int usuarioId);

        int InsertarYObtenerId(ZonaUsuario zona);

        bool Eliminar(int id);
    }
}
