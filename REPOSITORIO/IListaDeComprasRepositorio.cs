using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IListaDeComprasRepositorio
    {
        ListaDeCompras ObtenerPorId(int id);

        List<ListaDeCompras> ObtenerPorUsuario(int usuarioId);

        int InsertarYObtenerId(ListaDeCompras listaDeCompras);

        bool Actualizar(ListaDeCompras listaDeCompras);

        bool Eliminar(int id);
    }
}
