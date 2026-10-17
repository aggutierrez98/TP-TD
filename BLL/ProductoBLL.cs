using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Pantalla de operador (back-office): catalogo de productos independiente de los comercios
    // (nombre y categoria). Que comercio lo vende y a que precio lo resuelve ComercioProductoBLL.
    // La baja se bloquea con un chequeo explicito de uso (no se espera a que falle una FK).
    public class ProductoBLL
    {
        private readonly IProductoRepositorio _productoRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal ProductoBLL(
            IProductoRepositorio productoRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _productoRepositorio = productoRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<Producto> ObtenerTodos()
        {
            return _productoRepositorio.ObtenerTodos();
        }

        public Producto Crear(string nombre, int categoriaProductoId)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CategoriaProducto categoria = unidad.CategoriasProducto.ObtenerPorId(categoriaProductoId);
                    if (categoria == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
                    }

                    Producto nuevoProducto = new Producto
                    {
                        CategoriaProducto = categoria,
                        Nombre = nombreNormalizado
                    };

                    int id = unidad.Productos.InsertarYObtenerId(nuevoProducto);
                    nuevoProducto.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductManagement.Value.Activity.ProductoCreation", "FormProductManagement.Value.Info.ProductoCreation", "ProductoBLL", unidad);
                    unidad.Save();
                    return nuevoProducto;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.MerchantManagement.ProductoCreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, string nombre, int categoriaProductoId)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Producto producto = unidad.Productos.ObtenerPorId(id);
                    if (producto == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoNotFound");
                    }

                    CategoriaProducto categoria = unidad.CategoriasProducto.ObtenerPorId(categoriaProductoId);
                    if (categoria == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
                    }

                    producto.Nombre = nombreNormalizado;
                    producto.CategoriaProducto = categoria;

                    bool actualizado = unidad.Productos.Actualizar(producto);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ProductoUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductManagement.Value.Activity.ProductoUpdate", "FormProductManagement.Value.Info.ProductoUpdate", "ProductoBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.ProductoUpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Producto producto = unidad.Productos.ObtenerPorId(id);
                    if (producto == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoNotFound");
                    }

                    // Un producto que algun comercio vende, o que una promocion incluye o
                    // excluye, no se puede dar de baja: hay que quitarlo antes de esos usos.
                    if (unidad.ComercioProductos.ContarPorProducto(id) > 0
                        || unidad.PromocionProductos.ContarPorProducto(id) > 0)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoInUse");
                    }

                    bool eliminado = unidad.Productos.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ProductoDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductManagement.Value.Activity.ProductoDeletion", "FormProductManagement.Value.Info.ProductoDeletion", "ProductoBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.ProductoDeleteFailed", ex);
                }
            }
        }

        private static string NormalizarNombre(string nombre)
        {
            string nombreNormalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.ProductoNameRequired");
            }

            return nombreNormalizado;
        }
    }
}
