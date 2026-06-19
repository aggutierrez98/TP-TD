using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class IdiomaBLL
    {
        private readonly IdiomaDAL _idiomaDAL;
        private readonly UsuarioPreferenciaDAL _usuarioPreferenciaDAL;
        private readonly IdiomaCatalogoService _idiomaCatalogoService;
        private readonly UsuarioEstadoBLL _usuarioEstadoBLL;

        public IdiomaBLL()
        {
            BootstrapAplicacionBLL.Inicializar();
            _idiomaDAL = new IdiomaDAL();
            _usuarioPreferenciaDAL = new UsuarioPreferenciaDAL();
            _idiomaCatalogoService = new IdiomaCatalogoService();
            _usuarioEstadoBLL = new UsuarioEstadoBLL();
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
            return _idiomaDAL.ObtenerIdiomas();
        }

        public Idioma ObtenerIdioma(int idiomaId)
        {
            return _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
        }

        public Idioma ObtenerIdiomaDefault()
        {
            Idioma idiomaDefault = _idiomaDAL.ObtenerIdiomaDefault();
            if (idiomaDefault == null)
            {
                idiomaDefault = _idiomaDAL.ObtenerIdiomaDefault();
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
            Idioma idioma = _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    bool guardado = _usuarioPreferenciaDAL.Guardar(usuarioId, idiomaId, accesoDatos);
                    if (!guardado)
                    {
                        throw new ExcepcionTecnica("Error.Language.PreferenceSaveFailed");
                    }

                    _usuarioEstadoBLL.GuardarVersionActual(usuarioId, "FormUserHistory.Value.Action.PreferredLanguageUpdate", "FormUserHistory.Value.Obs.LanguagePreferenceUpdate", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.LanguagePreferenceUpdate", "FormAuditLog.Value.Info.LanguagePreferenceUpdate", "IdiomaBLL", accesoDatos);
                    accesoDatos.CommitTransactionScope();
                }
                catch (ExcepcionNegocio)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw;
                }
                catch (Exception ex)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw new ExcepcionTecnica("Error.Language.UserPreferenceSaveFailed", ex);
                }
            }

            AplicarIdiomaSesion(idiomaId);
            return true;
        }

        public int CrearIdioma(string nombre)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaDAL.ObtenerIdiomas(), nombreNormalizado, null);

            int idiomaId = _idiomaDAL.InsertarIdioma(new Idioma
            {
                Nombre = nombreNormalizado,
                Default = false
            });

            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.LanguageCreation", "FormAuditLog.Value.Info.LanguageCreation", "IdiomaBLL");
            return idiomaId;
        }

        public bool HacerIdiomaDefault(int idiomaId)
        {
            try {
                Idioma idiomaDefault = _idiomaDAL.ObtenerIdiomaDefault();
                _idiomaDAL.SwitchearIdiomaDefault(idiomaDefault.Id);
                _idiomaDAL.SwitchearIdiomaDefault(idiomaId);
            }
            catch (InvalidOperationException ex)
            {
                // TODO: Tirar error
                return false;
            }

            return true;
        }
             
        public bool ActualizarIdioma(int idiomaId, string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaDAL.ObtenerIdiomas(), nombreNormalizado, idiomaId);

            bool actualizado = _idiomaDAL.ActualizarIdioma(new Idioma
            {
                Id = idiomaId,
                Nombre = nombreNormalizado
            });

            if (!actualizado)
            {
                return false;
            }

            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.LanguageUpdate", "FormAuditLog.Value.Info.LanguageUpdate", "IdiomaBLL");
            return true;
        }

        public bool EliminarIdioma(int idiomaId)
        {
            List<Idioma> idiomas = _idiomaDAL.ObtenerIdiomas();
            Idioma idioma = idiomas.FirstOrDefault(item => item.Id == idiomaId);
            if (idioma == null)
            {
                return false;
            }

            if (idioma.Default)
            {
                throw new ExcepcionNegocio("Error.Language.CannotDeleteDefaultLanguage");
            }

            bool eliminado = _idiomaDAL.EliminarIdioma(idiomaId);
            if (!eliminado)
            {
                return false;
            }

            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.LanguageDeletion", "FormAuditLog.Value.Info.LanguageDeletion", "IdiomaBLL");
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

            int palabraId = _idiomaDAL.ObtenerPalabraIdPorKey(key.Trim());
            if (palabraId == 0)
            {
                throw new ExcepcionNegocio("Error.Language.KeyNotFound");
            }

            _idiomaDAL.GuardarTraduccion(idiomaId, palabraId, valor.Trim());
            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.TranslationUpdate", "FormAuditLog.Value.Info.TranslationUpdate", "IdiomaBLL");
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            Idioma idioma = _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
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

            // Las keys base tambien se pueden borrar: al hacerlo, su traduccion queda vacia
            bool eliminado = _idiomaDAL.EliminarTraduccion(idiomaId, palabraId);
            if (eliminado)
            {
                BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.TranslationDeletion", "FormAuditLog.Value.Info.TranslationDeletion", "IdiomaBLL");
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

            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.TranslationsImport", "FormAuditLog.Value.Info.TranslationsImport", "IdiomaBLL");
        }

        public void ExportarJson(int idiomaId, string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ExcepcionNegocio("Error.Language.ExportPathRequired");
            }

            Idioma idioma = _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
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

            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.TranslationsExport", "FormAuditLog.Value.Info.TranslationsExport", "IdiomaBLL");
        }

        public bool EstaCompleto(int idiomaId)
        {
            return _idiomaCatalogoService.EstaCompleto(_idiomaDAL.ObtenerIdiomaCompleto(idiomaId));
        }

        public void AplicarIdiomaSesion(int idiomaId)
        {
            Idioma idioma = _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            _idiomaCatalogoService.ValidarIdiomaSeleccionable(idioma);
            IdiomaService.GetInstance().CambiarIdioma(idioma);
        }

        private Idioma ObtenerIdiomaParaUsuario(int usuarioId)
        {
            UsuarioPreferencia preferencia = _usuarioPreferenciaDAL.ObtenerPorUsuarioId(usuarioId);
            if (preferencia != null)
            {
                Idioma idiomaPreferido = _idiomaDAL.ObtenerIdiomaCompleto(preferencia.IdiomaId);
                if (idiomaPreferido != null && _idiomaCatalogoService.EstaCompleto(idiomaPreferido))
                {
                    return idiomaPreferido;
                }
            }

            return ObtenerIdiomaDefault();
        }

    }
}
