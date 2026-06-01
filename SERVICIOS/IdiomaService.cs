using System;
using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class IdiomaService
    {
        private const string IdiomaBaseNombre = "Espa\u00F1ol";

        private static readonly string[] AliasIdiomaBase =
        {
            "Espa\u00F1ol",
            "Espanol",
            "EspaÃ±ol",
            "EspaÃƒÂ±ol"
        };

        // TODO: levantar de un JSON el idioma base en vez de hardcode
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
            { "FormGestionUsuarios.PermissionTreeGroup", "Permisos y grupos asignados" },
            { "FormGestionUsuarios.SavePermissionsButton", "Guardar permisos del usuario" },
            { "FormGestionUsuarios.PermissionManagementButton", "Gestion de permisos" },
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
            { "FormGestionIdiomas.Grid.Value", "Traduccion" },
            { "FormGestionPermisos.Title", "Gestion de permisos" },
            { "FormGestionPermisos.TitleLabel", "Gestion de permisos" },
            { "FormGestionPermisos.TreeGroup", "Catalogo de permisos" },
            { "FormGestionPermisos.EditionGroup", "Edicion del catalogo" },
            { "FormGestionPermisos.NameLabel", "Nombre:" },
            { "FormGestionPermisos.TypeLabel", "Tipo:" },
            { "FormGestionPermisos.TypeGroup", "Grupo" },
            { "FormGestionPermisos.TypeLeaf", "Permiso" },
            { "FormGestionPermisos.RootButton", "Agregar nodo raiz" },
            { "FormGestionPermisos.ChildButton", "Agregar hijo al grupo seleccionado" },
            { "FormGestionPermisos.RenameButton", "Renombrar seleccionado" },
            { "FormGestionPermisos.DeleteButton", "Eliminar seleccionado" },
            { "FormGestionPermisos.SaveButton", "Guardar catalogo" }
        };

        public IDictionary<string, string> ObtenerTraduccionesBase()
        {
            return new Dictionary<string, string>(TraduccionesBase);
        }

        public string ObtenerIdiomaBaseNombre()
        {
            return IdiomaBaseNombre;
        }

        public bool EsNombreIdiomaBase(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return false;
            }

            return AliasIdiomaBase.Any(item =>
                string.Equals(item, nombre.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception("El nombre del idioma no puede estar vacio.");
            }

            string nombreNormalizado = nombre.Trim();
            return EsNombreIdiomaBase(nombreNormalizado)
                ? IdiomaBaseNombre
                : nombreNormalizado;
        }

        public void ValidarNombreDuplicado(IEnumerable<Idioma> idiomas, string nombre, int? idiomaId)
        {
            bool existe = idiomas.Any(item =>
                (!idiomaId.HasValue || item.Id != idiomaId.Value) &&
                SonNombresEquivalentes(item.Nombre, nombre));

            if (existe)
            {
                throw new Exception("Ya existe un idioma con ese nombre.");
            }
        }

        public bool EstaCompleto(Idioma idioma)
        {
            if (idioma == null)
            {
                return false;
            }

            return TraduccionesBase.Keys.All(idioma.TieneTraduccion);
        }

        public void ValidarIdiomaSeleccionable(Idioma idioma)
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

        public bool EsKeyBase(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && TraduccionesBase.ContainsKey(key);
        }

        private bool SonNombresEquivalentes(string nombreA, string nombreB)
        {
            bool ambosIdiomaBase = EsNombreIdiomaBase(nombreA) && EsNombreIdiomaBase(nombreB);
            if (ambosIdiomaBase)
            {
                return true;
            }

            return string.Equals(nombreA, nombreB, StringComparison.OrdinalIgnoreCase);
        }
    }
}
