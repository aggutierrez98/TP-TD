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
        private readonly UsuarioDAL _usuarioDAL;
        private readonly PasswordHasher _passwordHasher;
        private readonly IdiomaBLL _idiomaBLL;
        private readonly PermisoBLL _permisoBLL;
        private readonly UsuarioEstadoBLL _usuarioEstadoBLL;
        private readonly IntegridadBLL _integridadBLL;

        // Tabla bajo control de integridad (debe coincidir con la usada en la verificacion).
        private const string TablaUsuarios = "Usuarios";

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
            Sesion.GetInstance().IniciarSesion(usuario.Id, usuario.Username);
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuario.Id));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuario.Id);
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

                    // El DVH se calcula antes de persistir para que la fila se guarde firmada.
                    nuevoUsuario.DVH = _integridadBLL.CalcularDVH(nuevoUsuario);
                    nuevoUsuario.Id = _usuarioDAL.InsertarYObtenerId(nuevoUsuario, accesoDatos);
                    _permisoBLL.AsignarPerfilBasico(nuevoUsuario.Id, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(nuevoUsuario.Id, "FormUserHistory.Value.Action.Creation", "FormUserHistory.Value.Obs.InitialCreation", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.UserCreation", "FormAuditLog.Value.Info.UserCreation", "UsuarioBLL", accesoDatos);
                    SincronizarDvvUsuarios(accesoDatos);
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

        public bool Actualizar(int id, string nuevoUsername, string nuevaPasswordRaw, bool activo)
        {
            string usernameNormalizado = (nuevoUsername ?? string.Empty).Trim();
            string passwordNormalizada = (nuevaPasswordRaw ?? string.Empty).Trim();
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
                    Usuario usuario = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuario == null)
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

                    bool usuarioEstabaActivo = usuario.Activo;
                    string usernameAnterior = usuario.Username;
                    bool actualizaPassword = !string.IsNullOrWhiteSpace(passwordNormalizada);
                    usuario.Username = usernameNormalizado;
                    if (actualizaPassword)
                    {
                        usuario.PasswordHash = _passwordHasher.HashPassword(passwordNormalizada);
                    }
                    usuario.Activo = activo;
                    // Se recalcula el DVH con el nuevo estado antes de persistir la fila.
                    usuario.DVH = _integridadBLL.CalcularDVH(usuario);
                    bool actualizado = _usuarioDAL.Actualizar(usuario, accesoDatos);
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
                    SincronizarDvvUsuarios(accesoDatos);
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

        public bool Eliminar(int id)
        {
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
                    Usuario usuario = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.NotFound");
                    }

                    bool eliminado = _usuarioDAL.EliminarFisicamente(id, accesoDatos);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.User.DeleteFailed");
                    }

                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.UserHardDelete", "FormAuditLog.Value.Info.UserHardDelete", "UsuarioBLL", accesoDatos);
                    // Cambio la cantidad de filas: hay que recalcular el DVV de la tabla.
                    SincronizarDvvUsuarios(accesoDatos);
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

                    _usuarioEstadoBLL.AplicarSnapshot(snapshot, accesoDatos);
                    // El estado restaurado cambia los atributos firmados: recalculo el DVH de la fila.
                    Usuario usuarioRestaurado = _usuarioDAL.ObtenerPorId(snapshot.UsuarioId, accesoDatos);
                    _usuarioDAL.ActualizarDVH(snapshot.UsuarioId, _integridadBLL.CalcularDVH(usuarioRestaurado), accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(snapshot.UsuarioId, "FormUserHistory.Value.Action.Restoration", "FormUserHistory.Value.Obs.Restoration", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormAuditLog.Value.Activity.UserRestoration", "FormAuditLog.Value.Info.UserRestoration", "UsuarioBLL", accesoDatos);
                    SincronizarDvvUsuarios(accesoDatos);
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
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuarioId);
        }

        // Recalcula y persiste el DVV de la tabla de usuarios tras una operacion de escritura.
        // Lee las filas ordenadas por Id (sobrecarga transaccional) para que el DVV resulte
        // consistente con el que valida el arranque.
        private void SincronizarDvvUsuarios(AccesoDatos accesoDatos)
        {
            List<Usuario> filas = _usuarioDAL.ObtenerTodos(accesoDatos);
            _integridadBLL.RecalcularDVV(TablaUsuarios, filas.Select(usuario => usuario.DVH), accesoDatos);
        }
    }
}
