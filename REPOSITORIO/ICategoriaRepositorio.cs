using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface ICategoriaRepositorio
    {
        Categoria ObtenerPorId(int id);

        List<Categoria> ObtenerVisiblesParaUsuario(Usuario usuario);

        int InsertarYObtenerId(Categoria categoria);

        bool Actualizar(Categoria categoria);

        bool Eliminar(Categoria categoria);

        // usuario null = scope global; excluida null = sin exclusion.
        bool ExisteNombreEnScope(string nombre, Usuario usuario, Categoria excluida);
    }
}
