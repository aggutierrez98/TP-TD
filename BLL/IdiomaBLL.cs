using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using REPOSITORIO;
using SERVICIOS;
using BE;

namespace BLL
{
    public class IdiomaBLL : IIdiomaBLL
    {
        private readonly IIdiomaRepositorio _idiomaRepositorio;
        private readonly IUsuarioPreferenciaRepositorio _usuarioPreferenciaRepositorio;
        private readonly ITraduccionRepositorio _traduccionRepositorio;
        private readonly IdiomaCatalogoService _idiomaCatalogoService;
        private readonly IUsuarioEstadoBLL _usuarioEstadoBLL;
        private readonly IBitacoraBLL _bitacoraBLL;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IObservado _servicioIdioma;
        private readonly ISesion _sesion;

        internal IdiomaBLL(
            IIdiomaRepositorio idiomaRepositorio,
            IUsuarioPreferenciaRepositorio usuarioPreferenciaRepositorio,
            ITraduccionRepositorio traduccionRepositorio,
            IdiomaCatalogoService idiomaCatalogoService,
            IUsuarioEstadoBLL usuarioEstadoBLL,
            IBitacoraBLL bitacoraBLL,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IObservado servicioIdioma,
            ISesion sesion)
        {
            _idiomaRepositorio = idiomaRepositorio;
            _usuarioPreferenciaRepositorio = usuarioPreferenciaRepositorio;
            _traduccionRepositorio = traduccionRepositorio;
            _idiomaCatalogoService = idiomaCatalogoService;
            _usuarioEstadoBLL = usuarioEstadoBLL;
            _bitacoraBLL = bitacoraBLL;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _servicioIdioma = servicioIdioma;
            _sesion = sesion;
        }

        public void InicializarAplicacion()
        {
            Idioma idiomaDefault = ObtenerIdiomaDefault();
            if (idiomaDefault != null)
            {
                _servicioIdioma.CambiarIdioma(idiomaDefault);
            }
        }

        public List<Idioma> ObtenerIdiomas()
        {
            return _idiomaRepositorio.ObtenerIdiomas();
        }

        public Idioma ObtenerIdioma(Idioma idioma)
        {
            return idioma == null ? null : _idiomaRepositorio.ObtenerPorId(idioma.Id);
        }

        public Idioma ObtenerIdiomaDefault()
        {
            Idioma idiomaDefault = _idiomaRepositorio.ObtenerIdiomaDefault();
            if (idiomaDefault == null)
            {
                idiomaDefault = _idiomaRepositorio.ObtenerIdiomaDefault();
            }

            return idiomaDefault;
        }

        public void AplicarIdiomaSesionUsuario(Usuario usuario)
        {
            Idioma idioma = ObtenerIdiomaParaUsuario(usuario);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Language.DefaultNotFound");
            }

            _servicioIdioma.CambiarIdioma(idioma);
        }

        public bool GuardarPreferencia(Idioma recibida)
        {
            Usuario usuario = _sesion.ObtenerUsuarioActual();
            Idioma idioma = recibida == null ? null : _idiomaRepositorio.ObtenerPorId(recibida.Id);
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    bool guardado = unidad.UsuarioPreferencias.Guardar(usuario, idioma);
                    if (!guardado)
                    {
                        throw new ExcepcionTecnica("Error.Language.PreferenceSaveFailed");
                    }

                    _usuarioEstadoBLL.GuardarVersionActualInterno(usuario, "FormUserHistory.Value.Action.PreferredLanguageUpdate", "FormUserHistory.Value.Obs.LanguagePreferenceUpdate", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguagePreferenceUpdate", "FormAuditLog.Value.Info.LanguagePreferenceUpdate", "IdiomaBLL", unidad);
                    unidad.Save();
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Language.UserPreferenceSaveFailed", ex);
                }
            }

            AplicarIdiomaSesion(idioma);
            return true;
        }

        public Idioma CrearIdioma(Idioma idioma)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(idioma?.Nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaRepositorio.ObtenerIdiomas(), nombreNormalizado, null);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                idioma.Nombre = nombreNormalizado;
                idioma.Default = false;
                idioma.Id = unidad.Idiomas.InsertarIdioma(idioma);

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguageCreation", "FormAuditLog.Value.Info.LanguageCreation", "IdiomaBLL", unidad);
                unidad.Save();
                return idioma;
            }
        }

        public bool HacerIdiomaDefault(Idioma recibida)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Idioma idiomaDefault = unidad.Idiomas.ObtenerIdiomaDefault();
                    if (idiomaDefault != null)
                    {
                        unidad.Idiomas.SwitchearIdiomaDefault(idiomaDefault);
                    }

                    bool resultado = unidad.Idiomas.SwitchearIdiomaDefault(recibida);
                    unidad.Save();
                    return resultado;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public bool ActualizarIdioma(Idioma recibida)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(recibida?.Nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaRepositorio.ObtenerIdiomas(), nombreNormalizado, recibida);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                Idioma idioma = recibida == null ? null : unidad.Idiomas.ObtenerPorId(recibida.Id);
                if (idioma == null) return false;
                idioma.Nombre = nombreNormalizado;
                bool actualizado = unidad.Idiomas.ActualizarIdioma(idioma);

                if (!actualizado)
                {
                    return false;
                }

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguageUpdate", "FormAuditLog.Value.Info.LanguageUpdate", "IdiomaBLL", unidad);
                unidad.Save();
                return true;
            }
        }

        public bool EliminarIdioma(Idioma recibida)
        {
            List<Idioma> idiomas = _idiomaRepositorio.ObtenerIdiomas();
            Idioma idioma = idiomas.FirstOrDefault(item => recibida != null && item.Id == recibida.Id);
            if (idioma == null)
            {
                return false;
            }

            if (idioma.Default)
            {
                throw new ExcepcionNegocio("Error.Language.CannotDeleteDefaultLanguage");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                bool eliminado = unidad.Idiomas.EliminarIdioma(idioma);
                if (!eliminado)
                {
                    return false;
                }

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguageDeletion", "FormAuditLog.Value.Info.LanguageDeletion", "IdiomaBLL", unidad);
                unidad.Save();
                return true;
            }
        }

        public void GuardarTraduccion(Traduccion traduccion)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                GuardarTraduccionInterno(traduccion.Idioma, traduccion.Palabra?.Key, traduccion.Valor, unidad);
                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationUpdate", "FormAuditLog.Value.Info.TranslationUpdate", "IdiomaBLL", unidad);
                unidad.Save();
            }
        }

        public bool EliminarTraduccion(Traduccion recibida)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                Idioma idioma = recibida?.Idioma == null ? null : unidad.Idiomas.ObtenerPorId(recibida.Idioma.Id);
                if (idioma == null)
                {
                    throw new ExcepcionNegocio("Error.Language.NotFound");
                }

                Traduccion traduccion = idioma.Traducciones
                    .OfType<Traduccion>()
                    .FirstOrDefault(item => item.Palabra?.Id == recibida.Palabra?.Id);

                if (traduccion == null || traduccion.Palabra == null)
                {
                    unidad.Save();
                    return false;
                }

                bool eliminado = unidad.Traducciones.EliminarTraduccion(traduccion);
                if (eliminado)
                {
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationDeletion", "FormAuditLog.Value.Info.TranslationDeletion", "IdiomaBLL", unidad);
                }

                unidad.Save();
                return eliminado;
            }
        }

        public void ImportarJson(Idioma idioma, string rutaArchivo)
        {
            Dictionary<string, string> traducciones = _idiomaCatalogoService.LeerCatalogoJson(rutaArchivo);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                foreach (KeyValuePair<string, string> item in traducciones)
                {
                    GuardarTraduccionInterno(idioma, item.Key, item.Value, unidad);
                }

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationsImport", "FormAuditLog.Value.Info.TranslationsImport", "IdiomaBLL", unidad);
                unidad.Save();
            }
        }

        public void ExportarJson(Idioma recibida, string rutaArchivo)
        {
            _idiomaCatalogoService.ValidarRutaExportacion(rutaArchivo);

            Idioma idioma = recibida == null ? null : _idiomaRepositorio.ObtenerPorId(recibida.Id);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Language.NotFound");
            }

            Dictionary<string, string> traducciones = _idiomaCatalogoService.ConstruirCatalogoExportable(idioma);
            _idiomaCatalogoService.EscribirCatalogoJson(traducciones, rutaArchivo);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationsExport", "FormAuditLog.Value.Info.TranslationsExport", "IdiomaBLL", unidad);
                unidad.Save();
            }
        }

        private void AplicarIdiomaSesion(Idioma idioma)
        {
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);
            _servicioIdioma.CambiarIdioma(idioma);
        }

        // Idioma del destinatario (preferencia completa o default) sin tocar el idioma global de la aplicacion.
        public Idioma ObtenerIdiomaParaUsuario(Usuario usuario)
        {
            UsuarioPreferencia preferencia = _usuarioPreferenciaRepositorio.ObtenerPorUsuario(usuario);
            if (preferencia != null)
            {
                Idioma idiomaPreferido = preferencia.Idioma as Idioma;
                if (idiomaPreferido != null && _idiomaCatalogoService.EstaCompleto(idiomaPreferido))
                {
                    return idiomaPreferido;
                }
            }

            return ObtenerIdiomaDefault();
        }

        private void GuardarTraduccionInterno(IIdioma idioma, string key, string valor, IUnidadDeTrabajo unidad)
        {
            if (unidad == null)
            {
                throw new ArgumentNullException("unidad");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ExcepcionNegocio("Error.Language.KeyRequired");
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ExcepcionNegocio("Error.Language.TranslationRequired");
            }

            Palabra palabra = unidad.Traducciones.ObtenerPalabraPorKey(key.Trim());
            if (palabra == null)
            {
                throw new ExcepcionNegocio("Error.Language.KeyNotFound");
            }

            unidad.Traducciones.GuardarTraduccion(new Traduccion { Idioma = idioma, Palabra = palabra, Valor = valor.Trim() });
        }
    }
}
