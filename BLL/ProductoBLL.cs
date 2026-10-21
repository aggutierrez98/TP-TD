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

        // Devuelve la misma instancia recibida, con el nombre normalizado, la categoria
        // persistida y el Id cargado.
        public Producto Crear(Producto producto)
        {
            string nombreNormalizado = NormalizarNombre(producto?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CategoriaProducto categoria = ObtenerCategoriaPersistida(unidad, producto.CategoriaProducto);

                    producto.CategoriaProducto = categoria;
                    producto.Nombre = nombreNormalizado;

                    producto.Id = unidad.Productos.InsertarYObtenerId(producto);

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductManagement.Value.Activity.ProductoCreation", "FormProductManagement.Value.Info.ProductoCreation", "ProductoBLL", unidad);
                    unidad.Save();
                    return producto;
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

        // Recibe el producto con nombre y categoria nuevos y los copia sobre el persistido.
        public bool Actualizar(Producto producto)
        {
            string nombreNormalizado = NormalizarNombre(producto?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Producto persistido = unidad.Productos.ObtenerPorId(producto.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoNotFound");
                    }

                    CategoriaProducto categoria = ObtenerCategoriaPersistida(unidad, producto.CategoriaProducto);

                    persistido.Nombre = nombreNormalizado;
                    persistido.CategoriaProducto = categoria;

                    bool actualizado = unidad.Productos.Actualizar(persistido);
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

        public bool Eliminar(Producto producto)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Producto persistido = producto == null ? null : unidad.Productos.ObtenerPorId(producto.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoNotFound");
                    }

                    // Un producto que algun comercio vende, o que una promocion incluye o
                    // excluye, no se puede dar de baja: hay que quitarlo antes de esos usos.
                    if (unidad.ComercioProductos.ContarPorProducto(persistido) > 0
                        || unidad.PromocionProductos.ContarPorProducto(persistido) > 0)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoInUse");
                    }

                    bool eliminado = unidad.Productos.Eliminar(persistido);
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

        // Relee la categoria recibida: el producto queda con la entidad persistida, no con la recibida.
        private static CategoriaProducto ObtenerCategoriaPersistida(IUnidadDeTrabajo unidad, CategoriaProducto categoria)
        {
            CategoriaProducto persistida = categoria == null ? null : unidad.CategoriasProducto.ObtenerPorId(categoria.Id);
            if (persistida == null)
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
            }

            return persistida;
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
