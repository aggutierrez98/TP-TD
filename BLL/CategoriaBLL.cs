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

        public List<Categoria> ObtenerVisiblesParaUsuarioActual()
        {
            return _categoriaRepositorio.ObtenerVisiblesParaUsuario(_sesion.ObtenerUsuarioActual());
        }

        // Devuelve la misma instancia recibida, con Id y Usuario (el de sesion) cargados.
        public Categoria Crear(Categoria categoria)
        {
            string nombreNormalizado = NormalizarNombre(categoria);

            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    if (unidad.Categorias.ExisteNombreEnScope(nombreNormalizado, usuarioActual, null))
                    {
                        throw new ExcepcionNegocio("Error.Category.NameExists");
                    }

                    categoria.Nombre = nombreNormalizado;
                    categoria.Usuario = usuarioActual;

                    categoria.Id = unidad.Categorias.InsertarYObtenerId(categoria);
                    _bitacoraBLL.RegistrarOperacionInterno("FormCategories.Value.Activity.CategoryCreation", "FormCategories.Value.Info.CategoryCreation", "CategoriaBLL", unidad);
                    unidad.Save();
                    return categoria;
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

        // Recibe la categoria con el nombre nuevo en Nombre. Valida la propiedad sobre
        // la persistida (no confia en el Usuario recibido) y solo copia el Nombre.
        public bool Actualizar(Categoria categoria)
        {
            string nombreNormalizado = NormalizarNombre(categoria);

            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Categoria persistida = unidad.Categorias.ObtenerPorId(categoria.Id);
                    ValidarPropiedad(persistida, usuarioActual);

                    if (unidad.Categorias.ExisteNombreEnScope(nombreNormalizado, usuarioActual, persistida))
                    {
                        throw new ExcepcionNegocio("Error.Category.NameExists");
                    }

                    persistida.Nombre = nombreNormalizado;
                    bool actualizado = unidad.Categorias.Actualizar(persistida);
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

        public bool Eliminar(Categoria categoria)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Categoria persistida = categoria == null ? null : unidad.Categorias.ObtenerPorId(categoria.Id);
                    ValidarPropiedad(persistida, usuarioActual);

                    bool eliminado = unidad.Categorias.Eliminar(persistida);
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

        private static string NormalizarNombre(Categoria categoria)
        {
            string nombreNormalizado = (categoria?.Nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.Category.NameRequired");
            }

            return nombreNormalizado;
        }

        // Una categoria solo puede ser editada/eliminada por su dueno. Las
        // globales (Usuario null) no se pueden tocar desde esta pantalla.
        private static void ValidarPropiedad(Categoria categoria, Usuario usuarioActual)
        {
            if (categoria == null)
            {
                throw new ExcepcionNegocio("Error.Category.NotFound");
            }

            if (categoria.Usuario == null || categoria.Usuario.Id != usuarioActual.Id)
            {
                throw new ExcepcionNegocio("Error.Category.NotOwned");
            }
        }
    }
}
