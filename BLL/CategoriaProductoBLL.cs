using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Pantalla de operador (back-office): sin validacion de "dueno", CategoriaProducto
    // no tiene Usuario asociado. Mismo patron que RubroComercioBLL.
    public class CategoriaProductoBLL
    {
        private readonly ICategoriaProductoRepositorio _categoriaProductoRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal CategoriaProductoBLL(
            ICategoriaProductoRepositorio categoriaProductoRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _categoriaProductoRepositorio = categoriaProductoRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<CategoriaProducto> ObtenerTodas()
        {
            return _categoriaProductoRepositorio.ObtenerTodas();
        }

        public CategoriaProducto Crear(string nombre)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CategoriaProducto nuevaCategoria = new CategoriaProducto { Nombre = nombreNormalizado };

                    int id = unidad.CategoriasProducto.InsertarYObtenerId(nuevaCategoria);
                    nuevaCategoria.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.CategoriaProductoCreation", "FormMerchantDetail.Value.Info.CategoriaProductoCreation", "CategoriaProductoBLL", unidad);
                    unidad.Save();
                    return nuevaCategoria;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoCreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, string nombre)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CategoriaProducto categoria = unidad.CategoriasProducto.ObtenerPorId(id);
                    if (categoria == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
                    }

                    categoria.Nombre = nombreNormalizado;
                    bool actualizado = unidad.CategoriasProducto.Actualizar(categoria);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.CategoriaProductoUpdate", "FormMerchantDetail.Value.Info.CategoriaProductoUpdate", "CategoriaProductoBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoUpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CategoriaProducto categoria = unidad.CategoriasProducto.ObtenerPorId(id);
                    if (categoria == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
                    }

                    bool eliminado = unidad.CategoriasProducto.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantDetail.Value.Activity.CategoriaProductoDeletion", "FormMerchantDetail.Value.Info.CategoriaProductoDeletion", "CategoriaProductoBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // Producto (que referencia a CategoriaProducto via FK) si forma parte de esta
                    // tanda, pero no exponemos ObtenerPorCategoriaProducto: al igual que Comercio
                    // con Producto en la tanda anterior, se revisa el SqlException ya envuelto
                    // como ExcepcionTecnica desde AccesoDatosSqlServer.ExecuteInTransaction.
                    if (ViolacionIntegridadReferencialHelper.EsViolacionRestriccionForanea(ex))
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoInUse");
                    }

                    if (ex is ExcepcionTecnica)
                    {
                        throw;
                    }

                    throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoDeleteFailed", ex);
                }
            }
        }

        private static string NormalizarNombre(string nombre)
        {
            string nombreNormalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNameRequired");
            }

            return nombreNormalizado;
        }
    }
}
