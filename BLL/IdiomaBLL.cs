using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
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

        public IdiomaBLL()
            : this(
                new IdiomaRepositorio(),
                new UsuarioPreferenciaRepositorio(),
                new TraduccionRepositorio(),
                new IdiomaCatalogoService(),
                new UsuarioEstadoBLL(),
                new BitacoraBLL(),
                new UnidadDeTrabajoFactory())
        {
        }

        internal IdiomaBLL(
            IIdiomaRepositorio idiomaRepositorio,
            IUsuarioPreferenciaRepositorio usuarioPreferenciaRepositorio,
            ITraduccionRepositorio traduccionRepositorio,
            IdiomaCatalogoService idiomaCatalogoService,
            IUsuarioEstadoBLL usuarioEstadoBLL,
            IBitacoraBLL bitacoraBLL,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory)
        {
            BootstrapAplicacionBLL.Inicializar();
            _idiomaRepositorio = idiomaRepositorio;
            _usuarioPreferenciaRepositorio = usuarioPreferenciaRepositorio;
            _traduccionRepositorio = traduccionRepositorio;
            _idiomaCatalogoService = idiomaCatalogoService;
            _usuarioEstadoBLL = usuarioEstadoBLL;
            _bitacoraBLL = bitacoraBLL;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
        }

        public void InicializarAplicacion()
        {
            Idioma idiomaDefault = ObtenerIdiomaDefault();
            if (idiomaDefault != null)
            {
                IdiomaService.GetInstance().CambiarIdioma(idiomaDefault);
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

            IdiomaService.GetInstance().CambiarIdioma(idioma);
        }

        public bool GuardarPreferencia(int usuarioId, int idiomaId)
        {
            Idioma idioma = _idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId);
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    bool guardado = _usuarioPreferenciaRepositorio.Guardar(usuarioId, idiomaId, unidad);
                    if (!guardado)
                    {
                        throw new ExcepcionTecnica("Error.Language.PreferenceSaveFailed");
                    }

                    _usuarioEstadoBLL.GuardarVersionActualInterno(usuarioId, "FormUserHistory.Value.Action.PreferredLanguageUpdate", "FormUserHistory.Value.Obs.LanguagePreferenceUpdate", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.LanguagePreferenceUpdate", "FormAuditLog.Value.Info.LanguagePreferenceUpdate", "IdiomaBLL", unidad);
                    unidad.Commit();
                }
                catch (ExcepcionNegocio)
                {
                    unidad.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    unidad.Rollback();
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

            int idiomaId = _idiomaRepositorio.InsertarIdioma(new Idioma
            {
                Nombre = nombreNormalizado,
                Default = false
            });

            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.LanguageCreation", "FormAuditLog.Value.Info.LanguageCreation", "IdiomaBLL");
            return idiomaId;
        }

        public bool HacerIdiomaDefault(int idiomaId)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    Idioma idiomaDefault = _idiomaRepositorio.ObtenerIdiomaDefault(unidad);
                    if (idiomaDefault != null)
                    {
                        _idiomaRepositorio.SwitchearIdiomaDefault(idiomaDefault.Id, unidad);
                    }

                    bool resultado = _idiomaRepositorio.SwitchearIdiomaDefault(idiomaId, unidad);
                    unidad.Commit();
                    return resultado;
                }
                catch (InvalidOperationException)
                {
                    unidad.Rollback();
                    return false;
                }
                catch
                {
                    unidad.Rollback();
                    throw;
                }
            }
        }

        public bool ActualizarIdioma(int idiomaId, string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaRepositorio.ObtenerIdiomas(), nombreNormalizado, idiomaId);

            bool actualizado = _idiomaRepositorio.ActualizarIdioma(new Idioma
            {
                Id = idiomaId,
                Nombre = nombreNormalizado
            });

            if (!actualizado)
            {
                return false;
            }

            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.LanguageUpdate", "FormAuditLog.Value.Info.LanguageUpdate", "IdiomaBLL");
            return true;
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

            bool eliminado = _idiomaRepositorio.EliminarIdioma(idiomaId);
            if (!eliminado)
            {
                return false;
            }

            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.LanguageDeletion", "FormAuditLog.Value.Info.LanguageDeletion", "IdiomaBLL");
            return true;
        }

        public void GuardarTraduccion(int idiomaId, string key, string valor)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ExcepcionNegocio("Error.Language.KeyRequired");
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ExcepcionNegocio("Error.Language.TranslationRequired");
            }

            int palabraId = _traduccionRepositorio.ObtenerPalabraIdPorKey(key.Trim());
            if (palabraId == 0)
            {
                throw new ExcepcionNegocio("Error.Language.KeyNotFound");
            }

            _traduccionRepositorio.GuardarTraduccion(idiomaId, palabraId, valor.Trim());
            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.TranslationUpdate", "FormAuditLog.Value.Info.TranslationUpdate", "IdiomaBLL");
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            Idioma idioma = _idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Language.NotFound");
            }

            Traduccion traduccion = idioma.Traducciones
                .OfType<Traduccion>()
                .FirstOrDefault(item => item.PalabraId == palabraId);

            if (traduccion == null || traduccion.Palabra == null)
            {
                return false;
            }

            bool eliminado = _traduccionRepositorio.EliminarTraduccion(idiomaId, palabraId);
            if (eliminado)
            {
                _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.TranslationDeletion", "FormAuditLog.Value.Info.TranslationDeletion", "IdiomaBLL");
            }

            return eliminado;
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

            foreach (KeyValuePair<string, string> item in traducciones)
            {
                GuardarTraduccion(idiomaId, item.Key, item.Value);
            }

            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.TranslationsImport", "FormAuditLog.Value.Info.TranslationsImport", "IdiomaBLL");
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

            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.TranslationsExport", "FormAuditLog.Value.Info.TranslationsExport", "IdiomaBLL");
        }

        public bool EstaCompleto(int idiomaId)
        {
            return _idiomaCatalogoService.EstaCompleto(_idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId));
        }

        public void AplicarIdiomaSesion(int idiomaId)
        {
            Idioma idioma = _idiomaRepositorio.ObtenerIdiomaCompleto(idiomaId);
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);
            IdiomaService.GetInstance().CambiarIdioma(idioma);
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
    }
}
