using System.Collections.Generic;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class CategoriaBLL
    {
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal CategoriaBLL(
            ICategoriaRepositorio categoriaRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _categoriaRepositorio = categoriaRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<Categoria> ObtenerVisiblesParaUsuario(int usuarioId)
        {
            return _categoriaRepositorio.ObtenerVisiblesParaUsuario(usuarioId);
        }

        public int Crear(string nombre)
        {
            string nombreNormalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.Category.NameRequired");
            }

            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.Categorias.ExisteNombreEnScope(nombreNormalizado, usuarioActualId, null))
                    {
                        throw new ExcepcionNegocio("Error.Category.NameExists");
                    }

                    Categoria nuevaCategoria = new Categoria
                    {
                        Nombre = nombreNormalizado,
                        Usuario = new Usuario { Id = usuarioActualId }
                    };

                    int id = unidad.Categorias.InsertarYObtenerId(nuevaCategoria);
                    _bitacoraBLL.RegistrarOperacionInterno("FormCategories.Value.Activity.CategoryCreation", "FormCategories.Value.Info.CategoryCreation", "CategoriaBLL", unidad);
                    unidad.Save();
                    return id;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (System.Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Category.CreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, string nuevoNombre)
        {
            string nombreNormalizado = (nuevoNombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.Category.NameRequired");
            }

            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Categoria categoria = unidad.Categorias.ObtenerPorId(id);
                    ValidarPropiedad(categoria, usuarioActualId);

                    if (unidad.Categorias.ExisteNombreEnScope(nombreNormalizado, usuarioActualId, id))
                    {
                        throw new ExcepcionNegocio("Error.Category.NameExists");
                    }

                    categoria.Nombre = nombreNormalizado;
                    bool actualizado = unidad.Categorias.Actualizar(categoria);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.Category.UpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCategories.Value.Activity.CategoryUpdate", "FormCategories.Value.Info.CategoryUpdate", "CategoriaBLL", unidad);
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
                catch (System.Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Category.UpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Categoria categoria = unidad.Categorias.ObtenerPorId(id);
                    ValidarPropiedad(categoria, usuarioActualId);

                    bool eliminado = unidad.Categorias.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.Category.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormCategories.Value.Activity.CategoryDeletion", "FormCategories.Value.Info.CategoryDeletion", "CategoriaBLL", unidad);
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
                catch (System.Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Category.DeleteFailed", ex);
                }
            }
        }

        // Una categoria solo puede ser editada/eliminada por su dueno. Las
        // globales (Usuario null) no se pueden tocar desde esta pantalla.
        private static void ValidarPropiedad(Categoria categoria, int usuarioActualId)
        {
            if (categoria == null)
            {
                throw new ExcepcionNegocio("Error.Category.NotFound");
            }

            if (categoria.Usuario == null || categoria.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.Category.NotOwned");
            }
        }
    }
}
