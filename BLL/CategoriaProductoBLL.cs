using System;
using System.Collections.Generic;
using System.Linq;
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
                    ValidarNombreUnico(unidad, 0, nombreNormalizado);
                    CategoriaProducto nuevaCategoria = new CategoriaProducto { Nombre = nombreNormalizado };

                    int id = unidad.CategoriasProducto.InsertarYObtenerId(nuevaCategoria);
                    nuevaCategoria.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductCategoryManagement.Value.Activity.Creation", "FormProductCategoryManagement.Value.Info.Creation", "CategoriaProductoBLL", unidad);
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

                    ValidarNombreUnico(unidad, id, nombreNormalizado);
                    categoria.Nombre = nombreNormalizado;
                    bool actualizado = unidad.CategoriasProducto.Actualizar(categoria);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductCategoryManagement.Value.Activity.Update", "FormProductCategoryManagement.Value.Info.Update", "CategoriaProductoBLL", unidad);
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

                    if (unidad.Productos.ContarPorCategoria(id) > 0
                        || unidad.PromocionCategoriasProducto.ContarPorCategoriaProducto(id) > 0)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoInUse");
                    }

                    bool eliminado = unidad.CategoriasProducto.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductCategoryManagement.Value.Activity.Deletion", "FormProductCategoryManagement.Value.Info.Deletion", "CategoriaProductoBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.CategoriaProductoDeleteFailed", ex);
                }
            }
        }

        private static void ValidarNombreUnico(IUnidadDeTrabajo unidad, int id, string nombre)
        {
            if (unidad.CategoriasProducto.ObtenerTodas().Any(categoria =>
                categoria.Id != id && string.Equals(categoria.Nombre, nombre, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoDuplicateName");
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
