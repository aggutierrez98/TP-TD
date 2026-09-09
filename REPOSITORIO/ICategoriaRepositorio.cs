using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ICategoriaRepositorio
    {
        Categoria ObtenerPorId(int id);

        List<Categoria> ObtenerVisiblesParaUsuario(int usuarioId);

        int InsertarYObtenerId(Categoria categoria);

        bool Actualizar(Categoria categoria);

        bool Eliminar(int id);

        bool ExisteNombreEnScope(string nombre, int? usuarioId, int? idExcluido);
    }
}
