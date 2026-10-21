using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class ListaDeComprasRepositorio : IListaDeComprasRepositorio
    {
        private readonly ListaDeComprasDAL _listaDeComprasDAL;
        private readonly ItemListaDAL _itemListaDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ListaDeComprasRepositorio()
            : this(null)
        {
        }

        internal ListaDeComprasRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _listaDeComprasDAL = new ListaDeComprasDAL();
            _itemListaDAL = new ItemListaDAL();
            _accesoDatos = accesoDatos;
        }

        public ListaDeCompras ObtenerPorId(int id)
        {
            ListaDeCompras listaDeCompras = _listaDeComprasDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(listaDeCompras);
            return listaDeCompras;
        }

        public List<ListaDeCompras> ObtenerPorUsuario(Usuario usuario)
        {
            List<ListaDeCompras> listas = _listaDeComprasDAL.ObtenerPorUsuario(usuario, _accesoDatos);
            listas.ForEach(Hidratar);
            return listas;
        }

        private void Hidratar(ListaDeCompras listaDeCompras)
        {
            if (listaDeCompras == null)
            {
                return;
            }

            listaDeCompras.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(listaDeCompras.Usuario.Id);

            // Se leen los items directo del DAL (no via ItemListaRepositorio) para no
            // reingresar en su Hidratar y volver a pedir esta misma ListaDeCompras al infinito.
            listaDeCompras.ItemsLista = _itemListaDAL.ObtenerPorListaDeCompras(listaDeCompras, _accesoDatos);
            foreach (ItemLista item in listaDeCompras.ItemsLista)
            {
                item.ListaDeCompras = listaDeCompras;
            }
        }

        public int InsertarYObtenerId(ListaDeCompras listaDeCompras)
        {
            return _listaDeComprasDAL.InsertarYObtenerId(listaDeCompras, _accesoDatos);
        }

        public bool Actualizar(ListaDeCompras listaDeCompras)
        {
            return _listaDeComprasDAL.Actualizar(listaDeCompras, _accesoDatos);
        }

        public bool Eliminar(ListaDeCompras lista)
        {
            return _listaDeComprasDAL.Eliminar(lista, _accesoDatos);
        }
    }
}
