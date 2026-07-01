using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using ABSTRACCIONES;
using REPOSITORIO;
using SERVICIOS;

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

        internal IdiomaBLL(
            IIdiomaRepositorio idiomaRepositorio,
            IUsuarioPreferenciaRepositorio usuarioPreferenciaRepositorio,
            ITraduccionRepositorio traduccionRepositorio,
            IdiomaCatalogoService idiomaCatalogoService,
            IUsuarioEstadoBLL usuarioEstadoBLL,
            IBitacoraBLL bitacoraBLL,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IObservado servicioIdioma)
        {
            _idiomaRepositorio = idiomaRepositorio;
            _usuarioPreferenciaRepositorio = usuarioPreferenciaRepositorio;
            _traduccionRepositorio = traduccionRepositorio;
            _idiomaCatalogoService = idiomaCatalogoService;
            _usuarioEstadoBLL = usuarioEstadoBLL;
            _bitacoraBLL = bitacoraBLL;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _servicioIdioma = servicioIdioma;
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

        public Idioma ObtenerIdioma(int idiomaId)
        {
            return _idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId);
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

        public void AplicarIdiomaSesionUsuario(int usuarioId)
        {
            Idioma idioma = ObtenerIdiomaParaUsuario(usuarioId);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Language.DefaultNotFound");
            }

            _servicioIdioma.CambiarIdioma(idioma);
        }

        public bool GuardarPreferencia(int usuarioId, int idiomaId)
        {
            Idioma idioma = _idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId);
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    bool guardado = unidad.UsuarioPreferencias.Guardar(usuarioId, idiomaId);
                    if (!guardado)
                    {
                        throw new ExcepcionTecnica("Error.Language.PreferenceSaveFailed");
                    }

                    _usuarioEstadoBLL.GuardarVersionActualInterno(usuarioId, "FormUserHistory.Value.Action.PreferredLanguageUpdate", "FormUserHistory.Value.Obs.LanguagePreferenceUpdate", unidad);
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

            AplicarIdiomaSesion(idiomaId);
            return true;
        }

        public int CrearIdioma(string nombre)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaRepositorio.ObtenerIdiomas(), nombreNormalizado, null);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                int idiomaId = unidad.Idiomas.InsertarIdioma(new Idioma
                {
                    Nombre = nombreNormalizado,
                    Default = false
                });

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguageCreation", "FormAuditLog.Value.Info.LanguageCreation", "IdiomaBLL", unidad);
                unidad.Save();
                return idiomaId;
            }
        }

        public bool HacerIdiomaDefault(int idiomaId)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Idioma idiomaDefault = unidad.Idiomas.ObtenerIdiomaDefault();
                    if (idiomaDefault != null)
                    {
                        unidad.Idiomas.SwitchearIdiomaDefault(idiomaDefault.Id);
                    }

                    bool resultado = unidad.Idiomas.SwitchearIdiomaDefault(idiomaId);
                    unidad.Save();
                    return resultado;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public bool ActualizarIdioma(int idiomaId, string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaRepositorio.ObtenerIdiomas(), nombreNormalizado, idiomaId);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                bool actualizado = unidad.Idiomas.ActualizarIdioma(new Idioma
                {
                    Id = idiomaId,
                    Nombre = nombreNormalizado
                });

                if (!actualizado)
                {
                    return false;
                }

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguageUpdate", "FormAuditLog.Value.Info.LanguageUpdate", "IdiomaBLL", unidad);
                unidad.Save();
                return true;
            }
        }

        public bool EliminarIdioma(int idiomaId)
        {
            List<Idioma> idiomas = _idiomaRepositorio.ObtenerIdiomas();
            Idioma idioma = idiomas.FirstOrDefault(item => item.Id == idiomaId);
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
                bool eliminado = unidad.Idiomas.EliminarIdioma(idiomaId);
                if (!eliminado)
                {
                    return false;
                }

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguageDeletion", "FormAuditLog.Value.Info.LanguageDeletion", "IdiomaBLL", unidad);
                unidad.Save();
                return true;
            }
        }

        public void GuardarTraduccion(int idiomaId, string key, string valor)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                GuardarTraduccionInterno(idiomaId, key, valor, unidad);
                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationUpdate", "FormAuditLog.Value.Info.TranslationUpdate", "IdiomaBLL", unidad);
                unidad.Save();
            }
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                Idioma idioma = unidad.Idiomas.ObtenerIdiomaCompleto(idiomaId);
                if (idioma == null)
                {
                    throw new ExcepcionNegocio("Error.Language.NotFound");
                }

                Traduccion traduccion = idioma.Traducciones
                    .OfType<Traduccion>()
                    .FirstOrDefault(item => item.PalabraId == palabraId);

                if (traduccion == null || traduccion.Palabra == null)
                {
                    unidad.Save();
                    return false;
                }

                bool eliminado = unidad.Traducciones.EliminarTraduccion(idiomaId, palabraId);
                if (eliminado)
                {
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationDeletion", "FormAuditLog.Value.Info.TranslationDeletion", "IdiomaBLL", unidad);
                }

                unidad.Save();
                return eliminado;
            }
        }

        public void ImportarJson(int idiomaId, string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                throw new ExcepcionNegocio("Error.Language.JsonFileNotFound");
            }

            string contenido = File.ReadAllText(rutaArchivo);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, string> traducciones;

            try
            {
                traducciones = serializer.Deserialize<Dictionary<string, string>>(contenido);
            }
            catch (InvalidOperationException ex)
            {
                throw new ExcepcionTecnica("Error.Language.JsonInvalidFormat", ex);
            }

            if (traducciones == null || traducciones.Count == 0)
            {
                throw new ExcepcionNegocio("Error.Language.JsonEmpty");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                foreach (KeyValuePair<string, string> item in traducciones)
                {
                    GuardarTraduccionInterno(idiomaId, item.Key, item.Value, unidad);
                }

                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationsImport", "FormAuditLog.Value.Info.TranslationsImport", "IdiomaBLL", unidad);
                unidad.Save();
            }
        }

        public void ExportarJson(int idiomaId, string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ExcepcionNegocio("Error.Language.ExportPathRequired");
            }

            Idioma idioma = _idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Language.NotFound");
            }

            Dictionary<string, string> traducciones = idioma.Traducciones
                .Where(item => item.Palabra != null && !string.IsNullOrWhiteSpace(item.Palabra.Key))
                .OrderBy(item => item.Palabra.Key)
                .ToDictionary(
                    item => item.Palabra.Key,
                    item => item.Valor ?? string.Empty,
                    StringComparer.Ordinal);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string contenido = serializer.Serialize(traducciones);
            File.WriteAllText(rutaArchivo, contenido, new UTF8Encoding(false));

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.TranslationsExport", "FormAuditLog.Value.Info.TranslationsExport", "IdiomaBLL", unidad);
                unidad.Save();
            }
        }

        public bool EstaCompleto(int idiomaId)
        {
            return _idiomaCatalogoService.EstaCompleto(_idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId));
        }

        public void AplicarIdiomaSesion(int idiomaId)
        {
            Idioma idioma = _idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId);
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);
            _servicioIdioma.CambiarIdioma(idioma);
        }

        private Idioma ObtenerIdiomaParaUsuario(int usuarioId)
        {
            UsuarioPreferencia preferencia = _usuarioPreferenciaRepositorio.ObtenerPorUsuarioId(usuarioId);
            if (preferencia != null)
            {
                Idioma idiomaPreferido = _idiomaRepositorio.ObtenerIdiomaCompleto(preferencia.IdiomaId);
                if (idiomaPreferido != null && _idiomaCatalogoService.EstaCompleto(idiomaPreferido))
                {
                    return idiomaPreferido;
                }
            }

            return ObtenerIdiomaDefault();
        }

        private void GuardarTraduccionInterno(int idiomaId, string key, string valor, IUnidadDeTrabajo unidad)
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

            int palabraId = unidad.Traducciones.ObtenerPalabraIdPorKey(key.Trim());
            if (palabraId == 0)
            {
                throw new ExcepcionNegocio("Error.Language.KeyNotFound");
            }

            unidad.Traducciones.GuardarTraduccion(idiomaId, palabraId, valor.Trim());
        }
    }
}
