using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Detalle del comercio (back-office): un comercio vende productos del catalogo con su propio
    // precio y moneda. La moneda no se vuelve a editar una vez asignado el producto; quitar la
    // asignacion no elimina el producto del catalogo. Moneda no forma parte de la Unidad de
    // Trabajo (no tiene BLL/CRUD propio en esta tanda), por eso se valida con un repositorio
    // inyectado aparte.
    public class ComercioProductoBLL
    {
        private readonly IComercioProductoRepositorio _comercioProductoRepositorio;
        private readonly IMonedaRepositorio _monedaRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal ComercioProductoBLL(
            IComercioProductoRepositorio comercioProductoRepositorio,
            IMonedaRepositorio monedaRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _comercioProductoRepositorio = comercioProductoRepositorio;
            _monedaRepositorio = monedaRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<ComercioProducto> ObtenerPorComercio(int comercioId)
        {
            return _comercioProductoRepositorio.ObtenerPorComercio(comercioId);
        }

        // La UI no puede referenciar REPOSITORIO directamente (limite de capas); expone
        // el catalogo de Moneda a traves del BLL.
        public List<Moneda> ObtenerMonedas()
        {
            return _monedaRepositorio.ObtenerTodas();
        }

        public ComercioProducto Asignar(int comercioId, int productoId, int monedaId, decimal precio)
        {
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

                    Producto producto = unidad.Productos.ObtenerPorId(productoId);
                    if (producto == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoNotFound");
                    }

                    if (unidad.ComercioProductos.ObtenerPorId(comercioId, productoId) != null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoAlreadyAssigned");
                    }

                    ComercioProducto asignacion = new ComercioProducto
                    {
                        Comercio = comercio,
                        Producto = producto,
                        Moneda = moneda,
                        Precio = precio
                    };

                    bool insertado = unidad.ComercioProductos.Insertar(asignacion);
                    if (!insertado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ProductoAssignFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.ProductoAssignment", "FormMerchantDetail.Value.Info.ProductoAssignment", "ComercioProductoBLL", unidad);
                    unidad.Save();
                    return asignacion;
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.ProductoAssignFailed", ex);
                }
            }
        }

        // No acepta monedaId: una vez asignado el producto al comercio, la moneda no se edita.
        public bool ActualizarPrecio(int comercioId, int productoId, decimal precio)
        {
            ValidarPrecio(precio);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.ComercioProductos.ObtenerPorId(comercioId, productoId) == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoAssignmentNotFound");
                    }

                    bool actualizado = unidad.ComercioProductos.ActualizarPrecio(comercioId, productoId, precio);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ProductoPriceUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.ProductoPriceUpdate", "FormMerchantDetail.Value.Info.ProductoPriceUpdate", "ComercioProductoBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.ProductoPriceUpdateFailed", ex);
                }
            }
        }

        public bool Quitar(int comercioId, int productoId)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.ComercioProductos.ObtenerPorId(comercioId, productoId) == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoAssignmentNotFound");
                    }

                    bool eliminado = unidad.ComercioProductos.Eliminar(comercioId, productoId);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ProductoRemoveFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.ProductoRemoval", "FormMerchantDetail.Value.Info.ProductoRemoval", "ComercioProductoBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.ProductoRemoveFailed", ex);
                }
            }
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
