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
            GenerarDiccionarios();

            Idioma idiomaDefault = ObtenerIdiomaDefault();
            if (idiomaDefault != null)
            {
                IdiomaService.GetInstance().CambiarIdioma(idiomaDefault);
            }
        }

        public List<Idioma> ObtenerIdiomas()
        {
            GenerarDiccionarios();
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
                GenerarDiccionarios();
                idiomaDefault = _idiomaDAL.ObtenerIdiomaDefault();
            }

            return idiomaDefault;
        }

        public void AplicarIdiomaSesionUsuario(int usuarioId)
        {
            Idioma idioma = ObtenerIdiomaParaUsuario(usuarioId);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Idioma.DefaultNotFound");
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
                        throw new ExcepcionTecnica("Error.Idioma.PreferenceSaveFailed");
                    }

                    _usuarioEstadoBLL.GuardarVersionActual(usuarioId, "ActualizacionIdiomaPreferido", "Actualizacion de la preferencia de idioma del usuario.", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("Actualizacion preferencia idioma", "Se actualizo el idioma preferido del usuario.", "IdiomaBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("Error.Idioma.UserPreferenceSaveFailed", ex);
                }
            }

            AplicarIdiomaSesion(idiomaId);
            return true;
        }

        public int CrearIdioma(string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = _idiomaCatalogoService.NormalizarNombre(nombre);
            _idiomaCatalogoService.ValidarNombreDuplicado(_idiomaDAL.ObtenerIdiomas(), nombreNormalizado, null);

            int idiomaId = _idiomaDAL.InsertarIdioma(new Idioma
            {
                Nombre = nombreNormalizado,
                Default = false
            });

            if (idiomaDefault)
            {
                _idiomaDAL.EstablecerIdiomaDefault(idiomaId);
            }

            AsegurarUnicoIdiomaDefault();
            BitacoraService.GetInstance().RegistrarOperacion("Alta de idioma", "Se creo el idioma.", "IdiomaBLL");
            return idiomaId;
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

            if (idiomaDefault)
            {
                _idiomaDAL.EstablecerIdiomaDefault(idiomaId);
            }
            else
            {
                Idioma idiomaActual = _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
                if (idiomaActual != null && idiomaActual.Default)
                {
                    throw new ExcepcionNegocio("Error.Idioma.ExactlyOneDefaultRequired");
                }
            }

            AsegurarUnicoIdiomaDefault();
            BitacoraService.GetInstance().RegistrarOperacion("Edicion de idioma", "Se actualizo el idioma.", "IdiomaBLL");
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

            if (idiomas.Count <= 1)
            {
                throw new ExcepcionNegocio("Error.Idioma.CannotDeleteLastLanguage");
            }

            bool eliminado = _idiomaDAL.EliminarIdioma(idiomaId);
            if (!eliminado)
            {
                return false;
            }

            if (idioma.Default)
            {
                Idioma reemplazo = _idiomaDAL.ObtenerIdiomas()
                    .OrderBy(item => item.Nombre)
                    .ThenBy(item => item.Id)
                    .FirstOrDefault();

                if (reemplazo != null)
                {
                    _idiomaDAL.EstablecerIdiomaDefault(reemplazo.Id);
                }
            }

            AsegurarUnicoIdiomaDefault();
            BitacoraService.GetInstance().RegistrarOperacion("Baja de idioma", "Se elimino el idioma.", "IdiomaBLL");
            return true;
        }

        public void GuardarTraduccion(int idiomaId, string key, string valor)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ExcepcionNegocio("Error.Idioma.KeyRequired");
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ExcepcionNegocio("Error.Idioma.TranslationRequired");
            }

            int palabraId = _idiomaDAL.ObtenerOCrearPalabra(key.Trim(), false);
            _idiomaDAL.GuardarTraduccion(idiomaId, palabraId, valor.Trim());
            BitacoraService.GetInstance().RegistrarOperacion("Actualizacion traduccion", "Se guardo la traduccion.", "IdiomaBLL");
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            Idioma idioma = _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Idioma.NotFound");
            }

            Traduccion traduccion = idioma.Traducciones
                .OfType<Traduccion>()
                .FirstOrDefault(item => item.PalabraId == palabraId);

            if (traduccion == null || traduccion.Palabra == null)
            {
                return false;
            }

            if (traduccion.Palabra.EsBase)
            {
                throw new ExcepcionNegocio("Error.Idioma.CannotDeleteBaseTranslation");
            }

            bool eliminado = _idiomaDAL.EliminarTraduccion(idiomaId, palabraId);
            if (eliminado)
            {
                BitacoraService.GetInstance().RegistrarOperacion("Baja de traduccion", "Se elimino la traduccion.", "IdiomaBLL");
            }

            return eliminado;
        }

        public void ImportarJson(int idiomaId, string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                throw new ExcepcionNegocio("Error.Idioma.JsonFileNotFound");
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
                throw new ExcepcionTecnica("Error.Idioma.JsonInvalidFormat", ex);
            }

            if (traducciones == null || traducciones.Count == 0)
            {
                throw new ExcepcionNegocio("Error.Idioma.JsonEmpty");
            }

            foreach (KeyValuePair<string, string> item in traducciones)
            {
                GuardarTraduccion(idiomaId, item.Key, item.Value);
            }

            BitacoraService.GetInstance().RegistrarOperacion("Importacion de traducciones", "Se importaron traducciones JSON.", "IdiomaBLL");
        }

        public void ExportarJson(int idiomaId, string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ExcepcionNegocio("Error.Idioma.ExportPathRequired");
            }

            Idioma idioma = _idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Idioma.NotFound");
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

            BitacoraService.GetInstance().RegistrarOperacion("Exportacion de traducciones", "Se exportaron traducciones JSON.", "IdiomaBLL");
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

        public void GenerarDiccionarios()
        {
            AsegurarUnicoIdiomaDefault();
        }

        private Idioma ObtenerIdiomaParaUsuario(int usuarioId)
        {
            GenerarDiccionarios();

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

        private void AsegurarUnicoIdiomaDefault()
        {
            List<Idioma> idiomas = _idiomaDAL.ObtenerIdiomas();
            int? idiomaDefaultId = idiomas.Where(item => item.Default)
                .Select(item => (int?)item.Id)
                .FirstOrDefault();

            if (!idiomaDefaultId.HasValue && idiomas.Count > 0)
            {
                idiomaDefaultId = idiomas
                    .OrderBy(item => item.Nombre)
                    .ThenBy(item => item.Id)
                    .Select(item => (int?)item.Id)
                    .FirstOrDefault();
            }

            if (idiomaDefaultId.HasValue)
            {
                _idiomaDAL.EstablecerIdiomaDefault(idiomaDefaultId.Value);
            }
        }
    }
}
