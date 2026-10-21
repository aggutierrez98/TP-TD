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

        // Devuelve la misma instancia recibida, con el nombre normalizado y el Id cargado.
        public CategoriaProducto Crear(CategoriaProducto categoria)
        {
            string nombreNormalizado = NormalizarNombre(categoria?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ValidarNombreUnico(unidad, null, nombreNormalizado);

                    categoria.Nombre = nombreNormalizado;
                    categoria.Id = unidad.CategoriasProducto.InsertarYObtenerId(categoria);

                    _bitacoraBLL.RegistrarOperacionInterno("FormProductCategoryManagement.Value.Activity.Creation", "FormProductCategoryManagement.Value.Info.Creation", "CategoriaProductoBLL", unidad);
                    unidad.Save();
                    return categoria;
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

        // Recibe la categoria con el nombre nuevo y lo copia sobre la persistida.
        public bool Actualizar(CategoriaProducto categoria)
        {
            string nombreNormalizado = NormalizarNombre(categoria?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CategoriaProducto persistida = unidad.CategoriasProducto.ObtenerPorId(categoria.Id);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
                    }

                    ValidarNombreUnico(unidad, persistida, nombreNormalizado);
                    persistida.Nombre = nombreNormalizado;
                    bool actualizado = unidad.CategoriasProducto.Actualizar(persistida);
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

        public bool Eliminar(CategoriaProducto categoria)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CategoriaProducto persistida = categoria == null ? null : unidad.CategoriasProducto.ObtenerPorId(categoria.Id);
                    if (persistida == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoNotFound");
                    }

                    if (unidad.Productos.ContarPorCategoria(persistida) > 0
                        || unidad.PromocionCategoriasProducto.ContarPorCategoriaProducto(persistida) > 0)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.CategoriaProductoInUse");
                    }

                    bool eliminado = unidad.CategoriasProducto.Eliminar(persistida);
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

        // excluida null = alta (sin exclusion).
        private static void ValidarNombreUnico(IUnidadDeTrabajo unidad, CategoriaProducto excluida, string nombre)
        {
            int idExcluido = excluida?.Id ?? 0;
            if (unidad.CategoriasProducto.ObtenerTodas().Any(categoria =>
                categoria.Id != idExcluido && string.Equals(categoria.Nombre, nombre, StringComparison.OrdinalIgnoreCase)))
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
