using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ItemListaRepositorio : IItemListaRepositorio
    {
        private readonly ItemListaDAL _itemListaDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ItemListaRepositorio()
            : this(null)
        {
        }

        internal ItemListaRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _itemListaDAL = new ItemListaDAL();
            _accesoDatos = accesoDatos;
        }

        public ItemLista ObtenerPorId(int id)
        {
            ItemLista itemLista = _itemListaDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(itemLista);
            return itemLista;
        }

        public List<ItemLista> ObtenerPorListaDeCompras(int listaDeComprasId)
        {
            List<ItemLista> items = _itemListaDAL.ObtenerPorListaDeCompras(listaDeComprasId, _accesoDatos);
            items.ForEach(Hidratar);
            return items;
        }

        private void Hidratar(ItemLista itemLista)
        {
            if (itemLista == null)
            {
                return;
            }

            itemLista.ListaDeCompras = new ListaDeComprasRepositorio(_accesoDatos).ObtenerPorId(itemLista.ListaDeCompras.Id);
        }

        public int InsertarYObtenerId(ItemLista itemLista)
        {
            return _itemListaDAL.InsertarYObtenerId(itemLista, _accesoDatos);
        }

        public bool Actualizar(ItemLista itemLista)
        {
            return _itemListaDAL.Actualizar(itemLista, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _itemListaDAL.Eliminar(id, _accesoDatos);
        }
    }
}
