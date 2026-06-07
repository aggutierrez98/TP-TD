using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    internal static class UiDialogHelper
    {
        private static readonly IDictionary<string, string> TextosBase =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Dialog.Title.Error", "Error" },
                { "Dialog.Title.Warning", "Atencion" },
                { "Dialog.Title.Success", "Exito" },
                { "Dialog.Title.Confirmation", "Confirmacion" },

                { "Error.Tecnico.Unexpected", "Ocurrio un error tecnico inesperado." },
                { "Error.Tecnico.SchemaInitializationFailed", "No se pudo inicializar el esquema de base de datos." },

                { "Error.Usuario.EmptyCredentials", "El usuario y la contrasena no pueden estar vacios." },
                { "Error.Usuario.UsernameExists", "El nombre de usuario ya existe. Por favor elegi otro." },
                { "Error.Usuario.RegisterFailed", "No se pudo registrar el usuario." },
                { "Error.Usuario.UsernameRequired", "El nombre de usuario no puede estar vacio." },
                { "Error.Usuario.NotFound", "No se encontro el usuario seleccionado." },
                { "Error.Usuario.CannotDeactivateActiveSession", "No se puede desactivar al usuario que tiene la sesion activa." },
                { "Error.Usuario.UpdateFailed", "No se pudo actualizar el usuario." },
                { "Error.Usuario.CannotDeleteActiveSession", "No se puede eliminar al usuario que tiene la sesion activa." },
                { "Error.Usuario.DeleteFailed", "No se pudo eliminar el usuario." },
                { "Error.Usuario.RestoreVersionNotFound", "No se encontro la version seleccionada." },
                { "Error.Usuario.CannotRestoreInactiveCurrentSession", "No se puede restaurar al estado inactivo del usuario con sesion actual." },
                { "Error.Usuario.HistoryUserNotFound", "No se encontro el usuario asociado al historial." },
                { "Error.Usuario.RestoreVersionFailed", "No se pudo restaurar la version seleccionada." },

                { "Error.Idioma.NameRequired", "El nombre del idioma no puede estar vacio." },
                { "Error.Idioma.NameAlreadyExists", "Ya existe un idioma con ese nombre." },
                { "Error.Idioma.NotFound", "No se encontro el idioma seleccionado." },
                { "Error.Idioma.IncompleteSelection", "El idioma seleccionado no esta completo y no puede usarse en la sesion." },
                { "Error.Idioma.DefaultNotFound", "No se encontro un idioma default configurado." },
                { "Error.Idioma.PreferenceSaveFailed", "No se pudo guardar la preferencia de idioma." },
                { "Error.Idioma.UserPreferenceSaveFailed", "No se pudo guardar la preferencia del usuario." },
                { "Error.Idioma.ExactlyOneDefaultRequired", "Debe existir exactamente un idioma default." },
                { "Error.Idioma.CannotDeleteLastLanguage", "No se puede eliminar el unico idioma existente." },
                { "Error.Idioma.KeyRequired", "La key no puede estar vacia." },
                { "Error.Idioma.TranslationRequired", "La traduccion no puede estar vacia." },
                { "Error.Idioma.CannotDeleteBaseTranslation", "No se pueden eliminar traducciones de keys base del sistema." },
                { "Error.Idioma.JsonFileNotFound", "No se encontro el archivo JSON seleccionado." },
                { "Error.Idioma.JsonInvalidFormat", "El archivo JSON no tiene un formato valido." },
                { "Error.Idioma.JsonEmpty", "El archivo JSON no contiene traducciones." },
                { "Error.Idioma.ExportPathRequired", "Debe seleccionar una ruta de destino para exportar el JSON." },

                { "Error.Permiso.CatalogEmpty", "Debe existir al menos un permiso o grupo." },
                { "Error.Permiso.UserNotFound", "No se encontro el usuario seleccionado." },
                { "Error.Permiso.SaveUserPermissionsFailed", "No se pudieron guardar los permisos del usuario." },
                { "Error.Permiso.NameRequired", "El nombre del permiso no puede estar vacio." },
                { "Error.Permiso.DuplicateNames", "No puede haber permisos o grupos con el mismo nombre." },
                { "Error.Permiso.EmptyGroupsNotAllowed", "No se pueden guardar grupos vacios." },
                { "Error.Permiso.CycleDetected", "No se pueden generar ciclos en el arbol de permisos." },

                { "FormLogin.Warning.CompleteFields", "Debe completar todos los campos." },
                { "FormLogin.Error.InvalidCredentials", "Credenciales incorrectas." },

                { "FormRegistro.Warning.CompleteFields", "Debe completar todos los campos." },
                { "FormRegistro.Success.UserRegisteredMessage", "Usuario registrado con exito." },
                { "FormRegistro.Success.UserRegisteredTitle", "Ahora usa las credenciales para ingresar..." },

                { "FormConfiguracionUsuario.Error.NoActiveSession", "No hay una sesion activa." },
                { "FormConfiguracionUsuario.Warning.SelectLanguage", "Debe seleccionar un idioma." },
                { "FormConfiguracionUsuario.Success.SettingsSaved", "La configuracion se guardo correctamente." },

                { "FormGestionIdiomas.Success.LanguageCreated", "Idioma creado correctamente." },
                { "FormGestionIdiomas.Warning.SelectLanguage", "Debe seleccionar un idioma." },
                { "FormGestionIdiomas.Success.LanguageUpdated", "Idioma actualizado correctamente." },
                { "FormGestionIdiomas.Confirm.DeleteLanguageMessage", "Estas seguro de que queres eliminar el idioma '{0}'?" },
                { "FormGestionIdiomas.Confirm.DeleteLanguageTitle", "Confirmar eliminacion" },
                { "FormGestionIdiomas.Success.LanguageDeleted", "Idioma eliminado correctamente." },
                { "FormGestionIdiomas.Success.TranslationsImported", "Traducciones importadas correctamente." },
                { "FormGestionIdiomas.Success.TranslationSaved", "Traduccion guardada correctamente." },
                { "FormGestionIdiomas.Warning.SelectTranslation", "Debe seleccionar una traduccion." },
                { "FormGestionIdiomas.Warning.DeleteBaseTranslation", "La traduccion seleccionada corresponde a una key base del sistema y no puede eliminarse." },
                { "FormGestionIdiomas.Warning.DeleteBaseTranslationTitle", "Operacion no permitida" },

                { "FormGestionPermisos.Warning.SelectParentGroup", "Debe seleccionar un grupo padre." },
                { "FormGestionPermisos.Warning.OnlyGroupsCanContainChildren", "Solo se pueden agregar hijos dentro de un grupo." },
                { "FormGestionPermisos.Warning.SelectPermission", "Debe seleccionar un permiso o grupo." },
                { "FormGestionPermisos.Warning.NameRequired", "Debe indicar un nombre." },
                { "FormGestionPermisos.Warning.NodeNameRequired", "Debe indicar un nombre para el permiso o grupo." },
                { "FormGestionPermisos.Confirm.DeleteNodeMessage", "Se eliminara el nodo seleccionado y todos sus hijos. Tambien se eliminaran sus asignaciones a usuarios al guardar. Desea continuar?" },
                { "FormGestionPermisos.Confirm.DeleteNodeTitle", "Confirmar eliminacion" },
                { "FormGestionPermisos.Success.CatalogSaved", "Catalogo de permisos guardado correctamente." },

                { "FormGestionUsuarios.Warning.CreateIncompleteData", "Para agregar un usuario debe completar el Nombre de usuario y la Contrasena." },
                { "FormGestionUsuarios.Warning.CreateIncompleteDataTitle", "Datos incompletos" },
                { "FormGestionUsuarios.Success.UserCreated", "Usuario creado correctamente." },
                { "FormGestionUsuarios.Warning.NoUserSelectedEdit", "No hay ningun usuario seleccionado. Selecciona uno de la lista para editarlo." },
                { "FormGestionUsuarios.Warning.NoUserSelectedTitle", "Sin seleccion" },
                { "FormGestionUsuarios.Warning.UsernameRequired", "El nombre de usuario no puede estar vacio." },
                { "FormGestionUsuarios.Warning.UsernameRequiredTitle", "Datos incompletos" },
                { "FormGestionUsuarios.Confirm.DeactivateUserMessage", "Esta accion desactivara al usuario seleccionado, por lo que no podra utilizarse hasta que vuelva a activarse. Desea continuar?" },
                { "FormGestionUsuarios.Confirm.DeactivateUserTitle", "Confirmar desactivacion" },
                { "FormGestionUsuarios.Success.UserActivated", "Usuario actualizado y activado correctamente." },
                { "FormGestionUsuarios.Success.UserDeactivated", "Usuario actualizado y desactivado correctamente." },
                { "FormGestionUsuarios.Success.UserUpdated", "Usuario actualizado correctamente." },
                { "FormGestionUsuarios.Warning.NoUserSelectedDelete", "No hay ningun usuario seleccionado. Selecciona uno de la lista para eliminarlo." },
                { "FormGestionUsuarios.Confirm.PhysicalDeleteMessage", "Esta es una accion critica. Se eliminara definitivamente al usuario seleccionado del sistema y todo su historial asociado. Esta operacion no podra deshacerse. Desea continuar?" },
                { "FormGestionUsuarios.Confirm.PhysicalDeleteTitle", "Confirmar borrado fisico" },
                { "FormGestionUsuarios.Success.UserDeleted", "Usuario eliminado definitivamente." },
                { "FormGestionUsuarios.Warning.SelectUserPermissions", "Debe seleccionar un usuario para guardar sus permisos." },
                { "FormGestionUsuarios.Success.UserPermissionsSaved", "Permisos guardados correctamente." },
                { "FormGestionUsuarios.Warning.SelectUserHistory", "Debe seleccionar un usuario para ver su historial." },

                { "FormHistorialUsuario.Warning.SelectVersion", "Debe seleccionar una version." },
                { "FormHistorialUsuario.Confirm.RestoreVersionMessage", "Se restaurara la version seleccionada del usuario. Desea continuar?" },
                { "FormHistorialUsuario.Confirm.RestoreVersionTitle", "Confirmar restauracion" },
                { "FormHistorialUsuario.Success.VersionRestored", "La version se restauro correctamente." }
            };

        public static void MostrarInfo(string messageKey, params object[] args)
        {
            Mostrar(messageKey, "Dialog.Title.Success", MessageBoxButtons.OK, MessageBoxIcon.Information, args);
        }

        public static void MostrarInfoConTitulo(string messageKey, string titleKey, params object[] args)
        {
            Mostrar(messageKey, titleKey, MessageBoxButtons.OK, MessageBoxIcon.Information, args);
        }

        public static void MostrarWarning(string messageKey, params object[] args)
        {
            Mostrar(messageKey, "Dialog.Title.Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, args);
        }

        public static void MostrarWarningConTitulo(string messageKey, string titleKey, params object[] args)
        {
            Mostrar(messageKey, titleKey, MessageBoxButtons.OK, MessageBoxIcon.Warning, args);
        }

        public static void MostrarError(string messageKey, params object[] args)
        {
            Mostrar(messageKey, "Dialog.Title.Error", MessageBoxButtons.OK, MessageBoxIcon.Error, args);
        }

        public static void MostrarErrorConTitulo(string messageKey, string titleKey, params object[] args)
        {
            Mostrar(messageKey, titleKey, MessageBoxButtons.OK, MessageBoxIcon.Error, args);
        }

        public static DialogResult Confirmar(string messageKey, params object[] args)
        {
            return Confirmar(messageKey, "Dialog.Title.Confirmation", MessageBoxIcon.Question, args);
        }

        public static DialogResult Confirmar(string messageKey, string titleKey, MessageBoxIcon icon, params object[] args)
        {
            return MessageBox.Show(
                Traducir(messageKey, args),
                Traducir(titleKey),
                MessageBoxButtons.YesNo,
                icon);
        }

        public static string Traducir(string key, params object[] args)
        {
            string texto = ResolverTexto(key);

            if (args == null || args.Length == 0)
            {
                return texto;
            }

            return string.Format(texto, args);
        }

        private static void Mostrar(string messageKey, string titleKey, MessageBoxButtons buttons, MessageBoxIcon icon, params object[] args)
        {
            MessageBox.Show(
                Traducir(messageKey, args),
                Traducir(titleKey),
                buttons,
                icon);
        }

        private static string ResolverTexto(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            Idioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma != null && idioma.TieneTraduccion(key))
            {
                string traduccion = idioma.ObtenerTraduccion(key);
                if (!string.IsNullOrWhiteSpace(traduccion) &&
                    !string.Equals(traduccion, key, StringComparison.OrdinalIgnoreCase))
                {
                    return traduccion;
                }
            }

            return TextosBase.TryGetValue(key, out string textoBase)
                ? textoBase
                : key;
        }
    }
}
