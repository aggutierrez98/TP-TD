using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class IdiomaService
    {
        private const string IdiomaBaseNombre = "Español";

        private static readonly Dictionary<string, string> TraduccionesBase = new Dictionary<string, string>
        {
            { "FormLogin.Title", "Menu de ingreso" },
            { "FormLogin.NicknameLabel", "Ingrese su nickname:" },
            { "FormLogin.PasswordLabel", "Ingrese su contrasena:" },
            { "FormLogin.LoginButton", "Ingresar" },
            { "FormLogin.RegisterPrompt", "No tenes usuario?" },
            { "FormLogin.RegisterButton", "Registrarme" },
            { "FormRegistro.Title", "Registro" },
            { "FormRegistro.NicknameLabel", "Ingrese su nickname:" },
            { "FormRegistro.PasswordLabel", "Ingrese su contrasena:" },
            { "FormRegistro.RegisterButton", "Registrarse" },
            { "FormGestionUsuarios.Title", "Gestion de Usuarios" },
            { "FormGestionUsuarios.TitleLabel", "Gestion de Usuarios" },
            { "FormGestionUsuarios.SessionLabelPrefix", "Sesion activa:" },
            { "FormGestionUsuarios.UserDataGroup", "Datos del usuario" },
            { "FormGestionUsuarios.UsernameLabel", "Nombre de usuario:" },
            { "FormGestionUsuarios.PasswordLabel", "Contrasena (solo para agregar usuario):" },
            { "FormGestionUsuarios.AddButton", "Agregar usuario" },
            { "FormGestionUsuarios.EditButton", "Editar usuario" },
            { "FormGestionUsuarios.DeleteButton", "Eliminar usuario" },
            { "FormGestionUsuarios.ConfigurationButton", "Configuracion" },
            { "FormGestionUsuarios.LanguageManagementButton", "Gestion de idiomas" },
            { "FormGestionUsuarios.Grid.Id", "ID" },
            { "FormGestionUsuarios.Grid.Username", "Nombre de usuario" },
            { "FormConfiguracionUsuario.Title", "Configuracion del usuario" },
            { "FormConfiguracionUsuario.TitleLabel", "Configuracion del usuario" },
            { "FormConfiguracionUsuario.CurrentUserPrefix", "Usuario actual:" },
            { "FormConfiguracionUsuario.LanguageLabel", "Idioma para la sesion:" },
            { "FormConfiguracionUsuario.SaveButton", "Guardar" },
            { "FormGestionIdiomas.Title", "Gestion de idiomas" },
            { "FormGestionIdiomas.TitleLabel", "Gestion de idiomas" },
            { "FormGestionIdiomas.LanguageGroup", "Datos del idioma" },
            { "FormGestionIdiomas.TranslationsGroup", "Traducciones" },
            { "FormGestionIdiomas.NameLabel", "Nombre del idioma:" },
            { "FormGestionIdiomas.DefaultCheck", "Idioma default" },
            { "FormGestionIdiomas.AddButton", "Agregar idioma" },
            { "FormGestionIdiomas.UpdateButton", "Actualizar idioma" },
            { "FormGestionIdiomas.DeleteButton", "Eliminar idioma" },
            { "FormGestionIdiomas.SelectLanguageLabel", "Idioma seleccionado:" },
            { "FormGestionIdiomas.ImportButton", "Importar JSON" },
            { "FormGestionIdiomas.ManualKeyLabel", "Key:" },
            { "FormGestionIdiomas.ManualValueLabel", "Traduccion:" },
            { "FormGestionIdiomas.SaveTranslationButton", "Guardar traduccion" },
            { "FormGestionIdiomas.DeleteTranslationButton", "Eliminar traduccion" },
            { "FormGestionIdiomas.Grid.Key", "Key" },
            { "FormGestionIdiomas.Grid.Value", "Traduccion" }
        };

        private readonly IIdiomaRepository idiomaRepository;
        private readonly IUsuarioPreferenciaRepository usuarioPreferenciaRepository;

        public IdiomaService(
            IIdiomaRepository idiomaRepository,
            IUsuarioPreferenciaRepository usuarioPreferenciaRepository)
        {
            this.idiomaRepository = idiomaRepository;
            this.usuarioPreferenciaRepository = usuarioPreferenciaRepository;
        }

        public void InicializarAplicacion()
        {
            GenerarDiccionarios();

            Idioma idiomaDefault = ObtenerIdiomaDefault();
            if (idiomaDefault != null)
            {
                Sesion.GetInstance().CambiarIdioma(idiomaDefault);
            }
        }

        public IList<Idioma> ObtenerIdiomas()
        {
            GenerarDiccionarios();
            return idiomaRepository.ObtenerIdiomas().Select(ConvertirIdioma).ToList();
        }

        public Idioma ObtenerIdioma(int idiomaId)
        {
            return ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaId));
        }

        public Idioma ObtenerIdiomaDefault()
        {
            Idioma idiomaDefault = ConvertirIdioma(idiomaRepository.ObtenerIdiomaDefault());
            if (idiomaDefault == null)
            {
                GenerarDiccionarios();
                idiomaDefault = ConvertirIdioma(idiomaRepository.ObtenerIdiomaDefault());
            }

            return idiomaDefault;
        }

        public Idioma ObtenerIdiomaParaUsuario(int usuarioId)
        {
            GenerarDiccionarios();

            IUsuarioPreferencia preferencia = usuarioPreferenciaRepository.ObtenerPorUsuarioId(usuarioId);
            if (preferencia != null)
            {
                Idioma idiomaPreferido = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(preferencia.IdiomaId));
                if (idiomaPreferido != null && EstaCompleto(idiomaPreferido))
                {
                    return idiomaPreferido;
                }
            }

            return ObtenerIdiomaDefault();
        }

        public void AplicarIdiomaSesionUsuario(int usuarioId)
        {
            Idioma idioma = ObtenerIdiomaParaUsuario(usuarioId);
            if (idioma == null)
            {
                throw new Exception("No se encontro un idioma default configurado.");
            }

            Sesion.GetInstance().CambiarIdioma(idioma);
        }

        public bool GuardarPreferencia(int usuarioId, int idiomaId)
        {
            Idioma idioma = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaId));
            ValidarIdiomaSeleccionable(idioma);

            return usuarioPreferenciaRepository.Guardar(new UsuarioPreferencia
            {
                UsuarioId = usuarioId,
                IdiomaId = idiomaId
            });
        }

        public int CrearIdioma(string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = NormalizarNombre(nombre);
            ValidarNombreDuplicado(nombreNormalizado, null);

            int idiomaId = idiomaRepository.InsertarIdioma(new Idioma
            {
                Nombre = nombreNormalizado,
                Default = false
            });

            if (idiomaDefault)
            {
                idiomaRepository.EstablecerIdiomaDefault(idiomaId);
            }

            AsegurarUnicoIdiomaDefault();
            return idiomaId;
        }

        public bool ActualizarIdioma(int idiomaId, string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = NormalizarNombre(nombre);
            ValidarNombreDuplicado(nombreNormalizado, idiomaId);

            bool actualizado = idiomaRepository.ActualizarIdioma(new Idioma
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
                idiomaRepository.EstablecerIdiomaDefault(idiomaId);
            }
            else
            {
                Idioma idiomaActual = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaId));
                if (idiomaActual != null && idiomaActual.Default)
                {
                    throw new Exception("Debe existir exactamente un idioma default.");
                }
            }

            AsegurarUnicoIdiomaDefault();
            return true;
        }

        public bool EliminarIdioma(int idiomaId)
        {
            List<Idioma> idiomas = idiomaRepository.ObtenerIdiomas().Select(ConvertirIdioma).ToList();
            Idioma idioma = idiomas.FirstOrDefault(item => item.Id == idiomaId);
            if (idioma == null)
            {
                return false;
            }

            if (idiomas.Count <= 1)
            {
                throw new Exception("No se puede eliminar el unico idioma existente.");
            }

            bool eliminado = idiomaRepository.EliminarIdioma(idiomaId);
            if (!eliminado)
            {
                return false;
            }

            if (idioma.Default)
            {
                Idioma reemplazo = idiomaRepository.ObtenerIdiomas()
                    .OrderBy(item => item.Nombre)
                    .Select(ConvertirIdioma)
                    .FirstOrDefault();

                if (reemplazo != null)
                {
                    idiomaRepository.EstablecerIdiomaDefault(reemplazo.Id);
                }
            }

            AsegurarUnicoIdiomaDefault();
            return true;
        }

        public void GuardarTraduccion(int idiomaId, string key, string valor)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("La key no puede estar vacia.");
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new Exception("La traduccion no puede estar vacia.");
            }

            int palabraId = idiomaRepository.ObtenerOCrearPalabra(key.Trim());
            idiomaRepository.GuardarTraduccion(idiomaId, palabraId, valor.Trim());
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            Idioma idioma = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaId));
            if (idioma == null)
            {
                throw new Exception("No se encontro el idioma seleccionado.");
            }

            Traduccion traduccion = idioma.Traducciones
                .OfType<Traduccion>()
                .FirstOrDefault(item => item.PalabraId == palabraId);

            if (traduccion == null || traduccion.Palabra == null)
            {
                return false;
            }

            if (EsKeyBase(traduccion.Palabra.Key))
            {
                throw new Exception("No se pueden eliminar traducciones de keys base del sistema.");
            }

            return idiomaRepository.EliminarTraduccion(idiomaId, palabraId);
        }

        public void ImportarJson(int idiomaId, string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                throw new Exception("No se encontro el archivo JSON seleccionado.");
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
                throw new Exception("El archivo JSON no tiene un formato valido.", ex);
            }

            if (traducciones == null || traducciones.Count == 0)
            {
                throw new Exception("El archivo JSON no contiene traducciones.");
            }

            foreach (KeyValuePair<string, string> item in traducciones)
            {
                GuardarTraduccion(idiomaId, item.Key, item.Value);
            }
        }

        public bool EstaCompleto(int idiomaId)
        {
            Idioma idioma = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaId));
            return EstaCompleto(idioma);
        }

        public void ValidarIdiomaSeleccionable(int idiomaId)
        {
            Idioma idioma = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaId));
            ValidarIdiomaSeleccionable(idioma);
        }

        public void GenerarDiccionarios()
        {
            Idioma idiomaBase = AsegurarIdiomaBase();

            foreach (KeyValuePair<string, string> item in TraduccionesBase)
            {
                if (idiomaBase.TieneTraduccion(item.Key))
                {
                    continue;
                }

                int palabraId = idiomaRepository.ObtenerOCrearPalabra(item.Key);
                idiomaRepository.GuardarTraduccion(idiomaBase.Id, palabraId, item.Value);
            }

            idiomaBase = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaBase.Id));

            if (!EstaCompleto(idiomaBase))
            {
                throw new Exception("No se pudo completar el diccionario base del sistema.");
            }
        }

        private Idioma AsegurarIdiomaBase()
        {
            List<Idioma> idiomas = idiomaRepository.ObtenerIdiomas().Select(ConvertirIdioma).ToList();
            Idioma idiomaBase = idiomas.FirstOrDefault(item =>
                string.Equals(item.Nombre, IdiomaBaseNombre, StringComparison.OrdinalIgnoreCase));

            if (idiomaBase == null)
            {
                int idiomaId = idiomaRepository.InsertarIdioma(new Idioma
                {
                    Nombre = IdiomaBaseNombre,
                    Default = true
                });

                idiomaRepository.EstablecerIdiomaDefault(idiomaId);
                idiomaBase = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaId));
            }
            else
            {
                AsegurarUnicoIdiomaDefault(idiomaBase.Id);
                idiomaBase = ConvertirIdioma(idiomaRepository.ObtenerIdiomaCompleto(idiomaBase.Id));
            }

            return idiomaBase;
        }

        private void AsegurarUnicoIdiomaDefault()
        {
            List<Idioma> idiomas = idiomaRepository.ObtenerIdiomas().Select(ConvertirIdioma).ToList();
            int? idiomaDefaultId = idiomas.Where(item => item.Default)
                .Select(item => (int?)item.Id)
                .FirstOrDefault();

            if (!idiomaDefaultId.HasValue && idiomas.Count > 0)
            {
                idiomaDefaultId = idiomas
                    .OrderBy(item => item.Nombre)
                    .Select(item => (int?)item.Id)
                    .FirstOrDefault();
            }

            if (idiomaDefaultId.HasValue)
            {
                idiomaRepository.EstablecerIdiomaDefault(idiomaDefaultId.Value);
            }
        }

        private void AsegurarUnicoIdiomaDefault(int idiomaDefaultId)
        {
            idiomaRepository.EstablecerIdiomaDefault(idiomaDefaultId);
        }

        private static string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception("El nombre del idioma no puede estar vacio.");
            }

            return nombre.Trim();
        }

        private void ValidarNombreDuplicado(string nombre, int? idiomaId)
        {
            bool existe = idiomaRepository.ObtenerIdiomas().Any(item =>
                string.Equals(item.Nombre, nombre, StringComparison.OrdinalIgnoreCase) &&
                (!idiomaId.HasValue || item.Id != idiomaId.Value));

            if (existe)
            {
                throw new Exception("Ya existe un idioma con ese nombre.");
            }
        }

        private bool EstaCompleto(Idioma idioma)
        {
            if (idioma == null)
            {
                return false;
            }

            return TraduccionesBase.Keys.All(idioma.TieneTraduccion);
        }

        private void ValidarIdiomaSeleccionable(Idioma idioma)
        {
            if (idioma == null)
            {
                throw new Exception("No se encontro el idioma seleccionado.");
            }

            if (!EstaCompleto(idioma))
            {
                throw new Exception("El idioma seleccionado no esta completo y no puede usarse en la sesion.");
            }
        }

        private static Idioma ConvertirIdioma(IIdioma idioma)
        {
            return idioma as Idioma;
        }

        private static bool EsKeyBase(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && TraduccionesBase.ContainsKey(key);
        }
    }
}
