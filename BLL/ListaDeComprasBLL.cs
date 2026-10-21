using System;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class ListaDeComprasBLL
    {
        private readonly IListaDeComprasRepositorio _listaDeComprasRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal ListaDeComprasBLL(
            IListaDeComprasRepositorio listaDeComprasRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _listaDeComprasRepositorio = listaDeComprasRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
        }

        // Cada usuario tiene una unica lista "definitiva": se busca la existente y,
        // si todavia no tiene, se crea automaticamente. No hay alta/gestion de multiples listas.
        public ListaDeCompras ObtenerOCrearParaUsuarioActual()
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            ListaDeCompras listaExistente = _listaDeComprasRepositorio.ObtenerPorUsuario(usuarioActual).FirstOrDefault();
            if (listaExistente != null)
            {
                return listaExistente;
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ListaDeCompras nuevaLista = new ListaDeCompras
                    {
                        Usuario = usuarioActual,
                        Nombre = "Mi lista de compras",
                        FechaCreacion = DateTime.Now
                    };

                    int id = unidad.ListasDeCompras.InsertarYObtenerId(nuevaLista);
                    nuevaLista.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormShoppingList.Value.Activity.ListCreation", "FormShoppingList.Value.Info.ListCreation", "ListaDeComprasBLL", unidad);
                    unidad.Save();
                    return nuevaLista;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.ShoppingList.CreateFailed", ex);
                }
            }
        }

        public ItemLista AgregarItem(ItemLista item)
        {
            string descripcionNormalizada = (item?.Descripcion ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(descripcionNormalizada))
            {
                throw new ExcepcionNegocio("Error.ShoppingList.DescriptionRequired");
            }

            ValidarCantidad(item.Cantidad);

            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ListaDeCompras lista = item.ListaDeCompras == null ? null : unidad.ListasDeCompras.ObtenerPorId(item.ListaDeCompras.Id);
                    ValidarPropiedadLista(lista, usuarioActual);
                    item.ListaDeCompras = lista;
                    item.Descripcion = descripcionNormalizada;
                    item.Comprado = false;
                    item.Id = unidad.ItemsLista.InsertarYObtenerId(item);

                    _bitacoraBLL.RegistrarOperacionInterno("FormShoppingList.Value.Activity.ItemCreation", "FormShoppingList.Value.Info.ItemCreation", "ListaDeComprasBLL", unidad);
                    unidad.Save();
                    return item;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.ShoppingList.AddItemFailed", ex);
                }
            }
        }

        public bool ActualizarCantidad(ItemLista recibido, int cantidad)
        {
            ValidarCantidad(cantidad);

            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ItemLista item = recibido == null ? null : unidad.ItemsLista.ObtenerPorId(recibido.Id);
                    ValidarPropiedadItem(item, usuarioActual);

                    item.Cantidad = cantidad;
                    bool actualizado = unidad.ItemsLista.Actualizar(item);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.ShoppingList.UpdateItemFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormShoppingList.Value.Activity.ItemUpdate", "FormShoppingList.Value.Info.ItemUpdate", "ListaDeComprasBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.ShoppingList.UpdateItemFailed", ex);
                }
            }
        }

        public bool MarcarComprado(ItemLista recibido, bool comprado)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ItemLista item = recibido == null ? null : unidad.ItemsLista.ObtenerPorId(recibido.Id);
                    ValidarPropiedadItem(item, usuarioActual);

                    item.Comprado = comprado;
                    bool actualizado = unidad.ItemsLista.Actualizar(item);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.ShoppingList.UpdateItemFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormShoppingList.Value.Activity.ItemUpdate", "FormShoppingList.Value.Info.ItemUpdate", "ListaDeComprasBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.ShoppingList.UpdateItemFailed", ex);
                }
            }
        }

        public bool EliminarItem(ItemLista recibido)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ItemLista item = recibido == null ? null : unidad.ItemsLista.ObtenerPorId(recibido.Id);
                    ValidarPropiedadItem(item, usuarioActual);

                    bool eliminado = unidad.ItemsLista.Eliminar(item);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.ShoppingList.DeleteItemFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormShoppingList.Value.Activity.ItemDeletion", "FormShoppingList.Value.Info.ItemDeletion", "ListaDeComprasBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.ShoppingList.DeleteItemFailed", ex);
                }
            }
        }

        private static void ValidarCantidad(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.QuantityMustBePositive");
            }
        }

        // La lista solo puede recibir items de su dueno.
        private static void ValidarPropiedadLista(ListaDeCompras lista, Usuario usuarioActual)
        {
            if (lista == null)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.NotFound");
            }

            if (lista.Usuario == null || lista.Usuario.Id != usuarioActual.Id)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.NotOwned");
            }
        }

        // Un item solo puede ser editado/eliminado por el dueno de su lista.
        private static void ValidarPropiedadItem(ItemLista item, Usuario usuarioActual)
        {
            if (item == null)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.ItemNotFound");
            }

            if (item.ListaDeCompras == null || item.ListaDeCompras.Usuario == null || item.ListaDeCompras.Usuario.Id != usuarioActual.Id)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.ItemNotOwned");
            }
        }
    }
}
