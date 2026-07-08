using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class UsuarioBLL
    {
        private const string TablaUsuarios = "Usuarios";

        private readonly UsuarioDAL _usuarioDAL;
        private readonly PasswordHasher _passwordHasher;
        private readonly IdiomaBLL _idiomaBLL;
        private readonly PermisoBLL _permisoBLL;
        private readonly UsuarioEstadoBLL _usuarioEstadoBLL;
        private readonly IntegridadBLL _integridadBLL;

        public UsuarioBLL()
        {
            BootstrapAplicacionBLL.Inicializar();
            _usuarioDAL = new UsuarioDAL();
            _passwordHasher = new PasswordHasher();
            _idiomaBLL = new IdiomaBLL();
            _permisoBLL = new PermisoBLL();
            _usuarioEstadoBLL = new UsuarioEstadoBLL();
            _integridadBLL = new IntegridadBLL();
        }

        public Usuario Login(string username, string passwordRaw)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(passwordRaw))
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.LoginEmptyFields", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.EmptyCredentials");
            }

            Usuario usuario = _usuarioDAL.ObtenerPorUsername(username.Trim());
            if (usuario == null)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.UnknownUserLogin", "UsuarioBLL");
                return null;
            }

            if (!usuario.Activo)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.InactiveUserLogin", "UsuarioBLL");
                return null;
            }

            bool esValido = _passwordHasher.VerifyPassword(passwordRaw, usuario.PasswordHash);
            if (!esValido)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.LoginInvalidCredentials", "UsuarioBLL");
                return null;
            }

            _permisoBLL.CargarPermisosEnUsuario(usuario);
            Sesion.GetInstance().IniciarSesion(usuario);
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuario.Id));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuario);
            BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.SuccessfulLogin", "FormAuditLog.Value.Info.SuccessfulLogin", "UsuarioBLL");
            return usuario;
        }

        public void CerrarSesionActual()
        {
            AutorizacionService.GetInstance().Limpiar();
            Sesion.GetInstance().CerrarSesion();
        }

        public bool Registrar(string username, string passwordRaw)
        {
            string usernameNormalizado = (username ?? string.Empty).Trim();
            string passwordNormalizada = (passwordRaw ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(usernameNormalizado) || string.IsNullOrWhiteSpace(passwordNormalizada))
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserCreation", "FormAuditLog.Value.Info.IncompleteUserCreationData", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.EmptyCredentials");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario existente = _usuarioDAL.ObtenerPorUsername(usernameNormalizado, accesoDatos);
                    if (existente != null)
                    {
                        BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserCreation", "FormAuditLog.Value.Info.UsernameAlreadyExists", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.User.UsernameExists");
                    }

                    Usuario nuevoUsuario = new Usuario(usernameNormalizado)
                    {
                        PasswordHash = _passwordHasher.HashPassword(passwordNormalizada),
                        Activo = true
                    };

                    // El Id participa del DVH, por lo que la firma recien puede
                    // calcularse despues del INSERT, cuando el Id real ya existe.
                    nuevoUsuario.Id = _usuarioDAL.InsertarYObtenerId(nuevoUsuario, accesoDatos);
                    nuevoUsuario.DVH = _integridadBLL.CalcularDVH(nuevoUsuario);
                    _usuarioDAL.ActualizarDVH(nuevoUsuario.Id, nuevoUsuario.DVH, accesoDatos);
                    _permisoBLL.AsignarPerfilBasico(nuevoUsuario.Id, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(nuevoUsuario.Id, "FormUserHistory.Value.Action.Creation", "FormUserHistory.Value.Obs.InitialCreation", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.UserCreation", "FormAuditLog.Value.Info.UserCreation", "UsuarioBLL", accesoDatos);

                    // Alta: no hay DVH anterior. Se suma el DVH del usuario
                    // nuevo al DVV persistido sin escanear Usuarios.
                    _integridadBLL.ActualizarDVV(TablaUsuarios, null, nuevoUsuario.DVH, accesoDatos);
                    accesoDatos.CommitTransactionScope();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw;
                }
                catch (Exception ex)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw new ExcepcionTecnica("Error.User.RegisterFailed", ex);
                }
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioDAL.ObtenerTodos();
        }

        public bool Actualizar(Usuario usuario, string nuevaPasswordRaw)
        {
            int id = usuario.Id;
            string usernameNormalizado = (usuario.Username ?? string.Empty).Trim();
            string passwordNormalizada = (nuevaPasswordRaw ?? string.Empty).Trim();
            bool activo = usuario.Activo;
            if (string.IsNullOrWhiteSpace(usernameNormalizado))
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.UserUpdateEmptyUsername", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.UsernameRequired");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuarioDesdeDB = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuarioDesdeDB == null)
                    {
                        throw new ExcepcionNegocio("Error.User.NotFound");
                    }

                    int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
                    if (usuarioActualId.HasValue && usuarioActualId.Value == id && !activo)
                    {
                        BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.AttemptedCurrentSessionUserDeactivation", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.User.CannotDeactivateActiveSession");
                    }

                    Usuario existente = _usuarioDAL.ObtenerPorUsername(usernameNormalizado, accesoDatos);
                    if (existente != null && existente.Id != id)
                    {
                        BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.UsernameAlreadyExists", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.User.UsernameExists");
                    }

                    // Guardar el DVH antes de modificar la entidad es obligatorio:
                    // el DVV incremental necesita restar la firma vieja y sumar
                    // la firma nueva calculada mas abajo.
                    string dvhAnterior = usuarioDesdeDB.DVH;
                    bool usuarioEstabaActivo = usuarioDesdeDB.Activo;
                    string usernameAnterior = usuarioDesdeDB.Username;
                    bool actualizaPassword = !string.IsNullOrWhiteSpace(passwordNormalizada);
                    usuarioDesdeDB.Username = usernameNormalizado;
                    if (actualizaPassword)
                    {
                        usuarioDesdeDB.PasswordHash = _passwordHasher.HashPassword(passwordNormalizada);
                    }
                    usuarioDesdeDB.Activo = activo;
                    usuarioDesdeDB.DVH = _integridadBLL.CalcularDVH(usuarioDesdeDB);
                    bool actualizado = _usuarioDAL.Actualizar(usuarioDesdeDB, accesoDatos);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.User.UpdateFailed");
                    }

                    string accionHistorial = "FormUserHistory.Value.Action.Update";
                    string observacionesHistorial = "FormUserHistory.Value.Obs.UserUpdate";
                    string actividadBitacora = "FormAuditLog.Value.Activity.UserUpdate";
                    string informacionBitacora = "FormAuditLog.Value.Info.UserUpdate";

                    if (usuarioEstabaActivo && !activo)
                    {
                        accionHistorial = "FormUserHistory.Value.Action.Deactivation";
                        observacionesHistorial = "FormUserHistory.Value.Obs.Deactivation";
                        actividadBitacora = "FormAuditLog.Value.Activity.UserDeactivation";
                        informacionBitacora = "FormAuditLog.Value.Info.UserDeactivation";
                    }
                    else if (!usuarioEstabaActivo && activo)
                    {
                        accionHistorial = "FormUserHistory.Value.Action.Reactivation";
                        observacionesHistorial = "FormUserHistory.Value.Obs.Reactivation";
                        actividadBitacora = "FormAuditLog.Value.Activity.UserReactivation";
                        informacionBitacora = "FormAuditLog.Value.Info.UserReactivation";
                    }
                    else if (actualizaPassword)
                    {
                        observacionesHistorial = "FormUserHistory.Value.Obs.PasswordUpdate";
                    }
                    else if (!string.Equals(usernameAnterior, usernameNormalizado, StringComparison.Ordinal))
                    {
                        observacionesHistorial = "FormUserHistory.Value.Obs.UsernameUpdate";
                    }

                    _usuarioEstadoBLL.GuardarVersionActual(id, accionHistorial, observacionesHistorial, accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion(actividadBitacora, informacionBitacora, "UsuarioBLL", accesoDatos);

                    // Modificacion: reemplaza la contribucion anterior por la
                    // nueva dentro de la misma transaccion de usuario/historial.
                    _integridadBLL.ActualizarDVV(TablaUsuarios, dvhAnterior, usuarioDesdeDB.DVH, accesoDatos);
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
                    throw new ExcepcionTecnica("Error.User.UpdateFailed", ex);
                }
            }

            if (Sesion.GetInstance().GetUsuarioActualId() == id)
            {
                Sesion.GetInstance().ActualizarUsuarioActualNombre(usernameNormalizado);
            }

            return true;
        }

        public bool Eliminar(Usuario usuario)
        {
            int id = usuario.Id;
            int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == id)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserHardDelete", "FormAuditLog.Value.Info.AttemptedCurrentSessionUserHardDelete", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.CannotDeleteActiveSession");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuarioDesdeDB = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuarioDesdeDB == null)
                    {
                        throw new ExcepcionNegocio("Error.User.NotFound");
                    }

                    // Baja fisica: capturar la firma antes del DELETE permite
                    // restarla del DVV luego de eliminar la fila.
                    string dvhAnterior = usuarioDesdeDB.DVH;
                    bool eliminado = _usuarioDAL.EliminarFisicamente(id, accesoDatos);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.User.DeleteFailed");
                    }

                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.UserHardDelete", "FormAuditLog.Value.Info.UserHardDelete", "UsuarioBLL", accesoDatos);

                    // Baja: el DVH nuevo es null porque la fila ya no participa
                    // del conjunto controlado por DVV.
                    _integridadBLL.ActualizarDVV(TablaUsuarios, dvhAnterior, null, accesoDatos);
                    accesoDatos.CommitTransactionScope();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw;
                }
                catch (Exception ex)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw new ExcepcionTecnica("Error.User.DeleteFailed", ex);
                }
            }
        }

        public List<UsuarioHistorialSnapshot> ObtenerHistorialEstados(int usuarioId)
        {
            return _usuarioEstadoBLL.ObtenerHistorial(usuarioId);
        }

        public UsuarioHistorialSnapshot ObtenerVersionHistorial(int historialId)
        {
            return _usuarioEstadoBLL.ObtenerVersion(historialId);
        }

        public List<ResultadoComparacionEstado> ObtenerDiferenciasConVersionAnterior(int historialId)
        {
            UsuarioHistorialSnapshot versionActual = _usuarioEstadoBLL.ObtenerVersion(historialId);
            if (versionActual == null)
            {
                return new List<ResultadoComparacionEstado>();
            }

            List<UsuarioHistorialSnapshot> historial = _usuarioEstadoBLL.ObtenerHistorial(versionActual.UsuarioId);
            int indiceActual = historial.FindIndex(item => item.HistorialId == historialId);
            UsuarioHistorialSnapshot versionAnterior = indiceActual >= 0 && indiceActual + 1 < historial.Count
                ? historial[indiceActual + 1]
                : null;

            return _usuarioEstadoBLL.CompararVersiones(versionActual, versionAnterior);
        }

        public bool RestaurarVersion(int historialId)
        {
            UsuarioHistorialSnapshot snapshot = _usuarioEstadoBLL.ObtenerVersion(historialId);
            if (snapshot == null)
            {
                throw new ExcepcionNegocio("Error.User.RestoreVersionNotFound");
            }

            int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == snapshot.UsuarioId && !snapshot.Activo)
            {
                throw new ExcepcionNegocio("Error.User.CannotRestoreInactiveCurrentSession");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(snapshot.UsuarioId, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.HistoryUserNotFound");
                    }

                    // Restauracion: el snapshot reescribe datos de usuario. Por
                    // eso se conserva la firma previa y luego se calcula la firma
                    // del estado restaurado.
                    string dvhAnterior = usuario.DVH;
                    _usuarioEstadoBLL.AplicarSnapshot(snapshot, accesoDatos);
                    Usuario usuarioRestaurado = _usuarioDAL.ObtenerPorId(snapshot.UsuarioId, accesoDatos);
                    string dvhNuevo = _integridadBLL.CalcularDVH(usuarioRestaurado);
                    _usuarioDAL.ActualizarDVH(snapshot.UsuarioId, dvhNuevo, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(snapshot.UsuarioId, "FormUserHistory.Value.Action.Restoration", "FormUserHistory.Value.Obs.Restoration", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.UserRestoration", "FormAuditLog.Value.Info.UserRestoration", "UsuarioBLL", accesoDatos);

                    // Restauracion: se reemplaza la contribucion del estado
                    // anterior por la del estado restaurado.
                    _integridadBLL.ActualizarDVV(TablaUsuarios, dvhAnterior, dvhNuevo, accesoDatos);
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
                    throw new ExcepcionTecnica("Error.User.RestoreVersionFailed", ex);
                }
            }

            RefrescarSesionSiCorresponde(snapshot.UsuarioId);
            return true;
        }

        private void RefrescarSesionSiCorresponde(int usuarioId)
        {
            int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
            if (!usuarioActualId.HasValue || usuarioActualId.Value != usuarioId)
            {
                return;
            }

            Usuario usuario = _usuarioDAL.ObtenerPorId(usuarioId);
            if (usuario == null)
            {
                return;
            }

            Sesion.GetInstance().ActualizarUsuarioActualNombre(usuario.Username);
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuarioId));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuario);
        }
    }
}
