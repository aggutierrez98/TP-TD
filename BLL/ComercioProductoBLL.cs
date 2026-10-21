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

        public List<ComercioProducto> ObtenerPorComercio(Comercio comercio)
        {
            return _comercioProductoRepositorio.ObtenerPorComercio(comercio);
        }

        // La UI no puede referenciar REPOSITORIO directamente (limite de capas); expone
        // el catalogo de Moneda a traves del BLL.
        public List<Moneda> ObtenerMonedas()
        {
            return _monedaRepositorio.ObtenerTodas();
        }

        // Recibe la asignacion armada por la UI (Comercio, Producto, Moneda y Precio). Relee las
        // tres entidades y devuelve la misma instancia con las persistidas.
        public ComercioProducto Asignar(ComercioProducto asignacion)
        {
            ValidarPrecio(asignacion);

            Moneda moneda = asignacion.Moneda == null ? null : _monedaRepositorio.ObtenerPorId(asignacion.Moneda.Id);
            if (moneda == null)
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.MonedaNotFound");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Comercio comercio = asignacion.Comercio == null ? null : unidad.Comercios.ObtenerPorId(asignacion.Comercio.Id);
                    if (comercio == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioNotFound");
                    }

                    Producto producto = asignacion.Producto == null ? null : unidad.Productos.ObtenerPorId(asignacion.Producto.Id);
                    if (producto == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoNotFound");
                    }

                    if (unidad.ComercioProductos.ObtenerPorComercioYProducto(comercio, producto) != null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoAlreadyAssigned");
                    }

                    asignacion.Comercio = comercio;
                    asignacion.Producto = producto;
                    asignacion.Moneda = moneda;

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

        // Recibe la asignacion con el precio nuevo y solo copia el Precio sobre la persistida:
        // una vez asignado el producto al comercio, la moneda no se edita.
        public bool ActualizarPrecio(ComercioProducto asignacion)
        {
            ValidarPrecio(asignacion);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ComercioProducto persistida = ObtenerPersistida(unidad, asignacion);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoAssignmentNotFound");
                    }

                    persistida.Precio = asignacion.Precio;
                    bool actualizado = unidad.ComercioProductos.ActualizarPrecio(persistida);
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

        public bool Quitar(ComercioProducto asignacion)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ComercioProducto persistida = ObtenerPersistida(unidad, asignacion);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ProductoAssignmentNotFound");
                    }

                    bool eliminado = unidad.ComercioProductos.Eliminar(persistida);
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

        // null si la asignacion recibida no identifica una fila (sin Comercio o sin Producto).
        private static ComercioProducto ObtenerPersistida(IUnidadDeTrabajo unidad, ComercioProducto asignacion)
        {
            if (asignacion?.Comercio == null || asignacion.Producto == null)
            {
                return null;
            }

            return unidad.ComercioProductos.ObtenerPorComercioYProducto(asignacion.Comercio, asignacion.Producto);
        }

        // Sin asignacion no hay precio: mismo error que un precio invalido.
        private static void ValidarPrecio(ComercioProducto asignacion)
        {
            if (asignacion == null || asignacion.Precio <= 0)
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.ProductoPriceInvalid");
            }
        }
    }
}
