using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IItemListaRepositorio
    {
        ItemLista ObtenerPorId(int id);

        List<ItemLista> ObtenerPorListaDeCompras(int listaDeComprasId);

        int InsertarYObtenerId(ItemLista itemLista);

        bool Actualizar(ItemLista itemLista);

        bool Eliminar(int id);
    }
}
