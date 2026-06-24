using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class UsuarioBLL
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordHashService _passwordHasher;
        private readonly IIdiomaBLL _idiomaBLL;
        private readonly IPermisoBLL _permisoBLL;
        private readonly IUsuarioEstadoBLL _usuarioEstadoBLL;
        private readonly IIntegridadBLL _integridadBLL;
        private readonly IBitacoraBLL _bitacoraBLL;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IAutorizacion _autorizacion;

        private const string TablaUsuarios = "Usuarios";

        public UsuarioBLL()
            : this(
                new UsuarioRepositorio(),
                new PasswordHashService(),
                new IdiomaBLL(),
                new PermisoBLL(),
                new UsuarioEstadoBLL(),
                new IntegridadBLL(),
                new BitacoraBLL(),
                new UnidadDeTrabajoFactory(),
                CompositionRoot.Sesion,
                CompositionRoot.Autorizacion)
        {
        }

        internal UsuarioBLL(
            IUsuarioRepositorio usuarioRepositorio,
            IPasswordHashService passwordHasher,
            IIdiomaBLL idiomaBLL,
            IPermisoBLL permisoBLL,
            IUsuarioEstadoBLL usuarioEstadoBLL,
            IIntegridadBLL integridadBLL,
            IBitacoraBLL bitacoraBLL,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IAutorizacion autorizacion)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _passwordHasher = passwordHasher;
            _idiomaBLL = idiomaBLL;
            _permisoBLL = permisoBLL;
            _usuarioEstadoBLL = usuarioEstadoBLL;
            _integridadBLL = integridadBLL;
            _bitacoraBLL = bitacoraBLL;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _autorizacion = autorizacion;
        }

        public Usuario Login(BE.Usuario usuarioEntidad, string passwordRaw)
        {
            Usuario usuarioBD = _usuarioRepositorio.ObtenerPorUsername(usuarioEntidad.Username.Trim());
            if (usuarioBD == null)
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.UnknownUserLogin", "UsuarioBLL");
                return null;
            }

            if (!usuarioBD.Activo)
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.InactiveUserLogin", "UsuarioBLL");
                return null;
            }

            bool esValido = _passwordHasher.VerifyPassword(passwordRaw, usuarioBD.PasswordHash);
            if (!esValido)
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.LoginInvalidCredentials", "UsuarioBLL");
                return null;
            }

            _sesion.IniciarSesion(usuarioBD);
            _autorizacion.ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuarioBD.Id));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuarioBD.Id);
            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.SuccessfulLogin", "FormAuditLog.Value.Info.SuccessfulLogin", "UsuarioBLL");
            return usuarioBD;
        }

        public void CerrarSesionActual()
        {
            _autorizacion.Limpiar();
            _sesion.CerrarSesion();
        }

        public bool Registrar(Usuario usuario, string passwordRaw)
        {
            string usernameNormalizado = (usuario.Username ?? string.Empty).Trim();
            string passwordNormalizada = (passwordRaw ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(usernameNormalizado) || string.IsNullOrWhiteSpace(passwordNormalizada))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserCreation", "FormAuditLog.Value.Info.IncompleteUserCreationData", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.EmptyCredentials");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    Usuario existente = _usuarioRepositorio.ObtenerPorUsername(usernameNormalizado, unidad);
                    if (existente != null)
                    {
                        _bitacoraBLL.RegistrarFalloNegocioInterno("FormAuditLog.Value.Activity.RejectedUserCreation", "FormAuditLog.Value.Info.UsernameAlreadyExists", "UsuarioBLL", unidad);
                        throw new ExcepcionNegocio("Error.User.UsernameExists");
                    }

                    Usuario nuevoUsuario = new Usuario(usernameNormalizado)
                    {
                        PasswordHash = _passwordHasher.HashPassword(passwordNormalizada),
                        Activo = true
                    };

                    nuevoUsuario.DVH = _integridadBLL.CalcularDVH(nuevoUsuario);
                    nuevoUsuario.Id = _usuarioRepositorio.InsertarYObtenerId(nuevoUsuario, unidad);
                    _permisoBLL.AsignarPerfilBasicoInterno(nuevoUsuario.Id, unidad);
                    _usuarioEstadoBLL.GuardarVersionActualInterno(nuevoUsuario.Id, "FormUserHistory.Value.Action.Creation", "FormUserHistory.Value.Obs.InitialCreation", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserCreation", "FormAuditLog.Value.Info.UserCreation", "UsuarioBLL", unidad);
                    SincronizarDvvUsuarios(unidad);
                    unidad.Commit();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    unidad.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    unidad.Rollback();
                    throw new ExcepcionTecnica("Error.User.RegisterFailed", ex);
                }
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioRepositorio.ObtenerTodos();
        }

        public bool Actualizar(int id, string nuevoUsername, string nuevaPasswordRaw, bool activo)
        {
            string usernameNormalizado = (nuevoUsername ?? string.Empty).Trim();
            string passwordNormalizada = (nuevaPasswordRaw ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(usernameNormalizado))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.UserUpdateEmptyUsername", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.UsernameRequired");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    Usuario usuario = _usuarioRepositorio.ObtenerPorId(id, unidad);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.NotFound");
                    }

                    int? usuarioActualId = _sesion.GetUsuarioActualId();
                    if (usuarioActualId.HasValue && usuarioActualId.Value == id && !activo)
                    {
                        _bitacoraBLL.RegistrarFalloNegocioInterno("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.AttemptedCurrentSessionUserDeactivation", "UsuarioBLL", unidad);
                        throw new ExcepcionNegocio("Error.User.CannotDeactivateActiveSession");
                    }

                    Usuario existente = _usuarioRepositorio.ObtenerPorUsername(usernameNormalizado, unidad);
                    if (existente != null && existente.Id != id)
                    {
                        _bitacoraBLL.RegistrarFalloNegocioInterno("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.UsernameAlreadyExists", "UsuarioBLL", unidad);
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
                    usuario.DVH = _integridadBLL.CalcularDVH(usuario);
                    bool actualizado = _usuarioRepositorio.Actualizar(usuario, unidad);
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

                    _usuarioEstadoBLL.GuardarVersionActualInterno(id, accionHistorial, observacionesHistorial, unidad);
                    _bitacoraBLL.RegistrarOperacionInterno(actividadBitacora, informacionBitacora, "UsuarioBLL", unidad);
                    SincronizarDvvUsuarios(unidad);
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
                    throw new ExcepcionTecnica("Error.User.UpdateFailed", ex);
                }
            }

            if (_sesion.GetUsuarioActualId() == id)
            {
                _sesion.ActualizarUsuarioActualNombre(usernameNormalizado);
            }

            return true;
        }

        public bool Eliminar(int id)
        {
            int? usuarioActualId = _sesion.GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == id)
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserHardDelete", "FormAuditLog.Value.Info.AttemptedCurrentSessionUserHardDelete", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.CannotDeleteActiveSession");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    Usuario usuario = _usuarioRepositorio.ObtenerPorId(id, unidad);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.NotFound");
                    }

                    bool eliminado = _usuarioRepositorio.EliminarFisicamente(id, unidad);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.User.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserHardDelete", "FormAuditLog.Value.Info.UserHardDelete", "UsuarioBLL", unidad);
                    SincronizarDvvUsuarios(unidad);
                    unidad.Commit();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    unidad.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    unidad.Rollback();
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

            int? usuarioActualId = _sesion.GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == snapshot.UsuarioId && !snapshot.Activo)
            {
                throw new ExcepcionNegocio("Error.User.CannotRestoreInactiveCurrentSession");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    Usuario usuario = _usuarioRepositorio.ObtenerPorId(snapshot.UsuarioId, unidad);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.HistoryUserNotFound");
                    }

                    _usuarioEstadoBLL.AplicarSnapshotInterno(snapshot, unidad);
                    Usuario usuarioRestaurado = _usuarioRepositorio.ObtenerPorId(snapshot.UsuarioId, unidad);
                    _usuarioRepositorio.ActualizarDVH(snapshot.UsuarioId, _integridadBLL.CalcularDVH(usuarioRestaurado), unidad);
                    _usuarioEstadoBLL.GuardarVersionActualInterno(snapshot.UsuarioId, "FormUserHistory.Value.Action.Restoration", "FormUserHistory.Value.Obs.Restoration", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserRestoration", "FormAuditLog.Value.Info.UserRestoration", "UsuarioBLL", unidad);
                    SincronizarDvvUsuarios(unidad);
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
                    throw new ExcepcionTecnica("Error.User.RestoreVersionFailed", ex);
                }
            }

            RefrescarSesionSiCorresponde(snapshot.UsuarioId);
            return true;
        }

        private void RefrescarSesionSiCorresponde(int usuarioId)
        {
            int? usuarioActualId = _sesion.GetUsuarioActualId();
            if (!usuarioActualId.HasValue || usuarioActualId.Value != usuarioId)
            {
                return;
            }

            Usuario usuario = _usuarioRepositorio.ObtenerPorId(usuarioId);
            if (usuario == null)
            {
                return;
            }

            _sesion.ActualizarUsuarioActualNombre(usuario.Username);
            _autorizacion.ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuarioId));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuarioId);
        }

        private void SincronizarDvvUsuarios(IUnidadDeTrabajo unidad)
        {
            List<Usuario> filas = _usuarioRepositorio.ObtenerTodos(unidad);
            _integridadBLL.RecalcularDVVInterno(TablaUsuarios, filas.Select(usuario => usuario.DVH), unidad);
        }
    }
}
