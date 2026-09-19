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
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            ListaDeCompras listaExistente = _listaDeComprasRepositorio.ObtenerPorUsuario(usuarioActualId).FirstOrDefault();
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
                        Usuario = new Usuario { Id = usuarioActualId },
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

        public ItemLista AgregarItem(int listaId, string descripcion, int cantidad)
        {
            string descripcionNormalizada = (descripcion ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(descripcionNormalizada))
            {
                throw new ExcepcionNegocio("Error.ShoppingList.DescriptionRequired");
            }

            ValidarCantidad(cantidad);

            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ListaDeCompras lista = unidad.ListasDeCompras.ObtenerPorId(listaId);
                    ValidarPropiedadLista(lista, usuarioActualId);

                    ItemLista nuevoItem = new ItemLista
                    {
                        ListaDeCompras = new ListaDeCompras { Id = listaId },
                        Descripcion = descripcionNormalizada,
                        Cantidad = cantidad,
                        Comprado = false
                    };

                    int id = unidad.ItemsLista.InsertarYObtenerId(nuevoItem);
                    nuevoItem.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormShoppingList.Value.Activity.ItemCreation", "FormShoppingList.Value.Info.ItemCreation", "ListaDeComprasBLL", unidad);
                    unidad.Save();
                    return nuevoItem;
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

        public bool ActualizarCantidad(int itemId, int cantidad)
        {
            ValidarCantidad(cantidad);

            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ItemLista item = unidad.ItemsLista.ObtenerPorId(itemId);
                    ValidarPropiedadItem(item, usuarioActualId);

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

        public bool MarcarComprado(int itemId, bool comprado)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ItemLista item = unidad.ItemsLista.ObtenerPorId(itemId);
                    ValidarPropiedadItem(item, usuarioActualId);

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

        public bool EliminarItem(int itemId)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ItemLista item = unidad.ItemsLista.ObtenerPorId(itemId);
                    ValidarPropiedadItem(item, usuarioActualId);

                    bool eliminado = unidad.ItemsLista.Eliminar(itemId);
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
        private static void ValidarPropiedadLista(ListaDeCompras lista, int usuarioActualId)
        {
            if (lista == null)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.NotFound");
            }

            if (lista.Usuario == null || lista.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.NotOwned");
            }
        }

        // Un item solo puede ser editado/eliminado por el dueno de su lista.
        private static void ValidarPropiedadItem(ItemLista item, int usuarioActualId)
        {
            if (item == null)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.ItemNotFound");
            }

            if (item.ListaDeCompras == null || item.ListaDeCompras.Usuario == null || item.ListaDeCompras.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.ShoppingList.ItemNotOwned");
            }
        }
    }
}
