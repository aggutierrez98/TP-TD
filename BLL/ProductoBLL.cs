using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Pantalla de operador (back-office): sin validacion de "dueno". Comercio y
    // CategoriaProducto se validan dentro de la transaccion (forman parte de la Unidad de
    // Trabajo); Moneda no forma parte de la Unidad de Trabajo (no tiene BLL/CRUD propio en
    // esta tanda), por eso se valida con un repositorio inyectado aparte, mismo criterio que
    // usa MovimientoBLL con su CategoriaRepositorio para validaciones cruzadas fuera de la UoW.
    public class ProductoBLL
    {
        private readonly IProductoRepositorio _productoRepositorio;
        private readonly IMonedaRepositorio _monedaRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal ProductoBLL(
            IProductoRepositorio productoRepositorio,
            IMonedaRepositorio monedaRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _productoRepositorio = productoRepositorio;
            _monedaRepositorio = monedaRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<Producto> ObtenerPorComercio(int comercioId)
        {
            return _productoRepositorio.ObtenerPorComercio(comercioId);
        }

        // La UI no puede referenciar REPOSITORIO directamente (limite de capas); expone
        // el catalogo de Moneda a traves del BLL, mismo criterio que ObtenerPorComercio.
        public List<Moneda> ObtenerMonedas()
        {
            return _monedaRepositorio.ObtenerTodas();
        }

        public Producto Crear(int comercioId, string nombre, int categoriaProductoId, int monedaId, decimal precio)
        {
            string nombreNormalizado = NormalizarNombre(nombre);
            ValidarPrecio(precio);

            Moneda moneda = _monedaRepositorio.ObtenerPorId(monedaId);
            if (moneda == null)
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.MonedaNotFound");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Comercio comercio = unidad.Comercios.ObtenerPorId(comercioId);
                    if (comercio == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioNotFound");
                    }

                    CategoriaProducto categoria = unidad.CategoriasProducto.ObtenerPorId(categoriaProductoId);
                    if (categoria == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
                    }

                    Producto nuevoProducto = new Producto
                    {
                        Comercio = comercio,
                        CategoriaProducto = categoria,
                        Moneda = moneda,
                        Nombre = nombreNormalizado,
                        Precio = precio
                    };

                    int id = unidad.Productos.InsertarYObtenerId(nuevoProducto);
                    nuevoProducto.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.ProductoCreation", "FormMerchantDetail.Value.Info.ProductoCreation", "ProductoBLL", unidad);
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

        // No acepta comercioId ni monedaId: una vez creado el producto, ni el comercio
        // duenio ni la moneda se vuelven a editar.
        public bool Actualizar(int id, string nombre, int categoriaProductoId, decimal precio)
        {
            string nombreNormalizado = NormalizarNombre(nombre);
            ValidarPrecio(precio);

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
                    producto.Precio = precio;

                    bool actualizado = unidad.Productos.Actualizar(producto);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ProductoUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.ProductoUpdate", "FormMerchantDetail.Value.Info.ProductoUpdate", "ProductoBLL", unidad);
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

                    bool eliminado = unidad.Productos.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ProductoDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.ProductoDeletion", "FormMerchantDetail.Value.Info.ProductoDeletion", "ProductoBLL", unidad);
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

        private static void ValidarPrecio(decimal precio)
        {
            if (precio <= 0)
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.ProductoPriceInvalid");
            }
        }
    }
}
