using System;
using System.Collections.Generic;
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

            string passwordHash = _passwordHasher.HashPassword(passwordNormalizada);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Usuario existente = unidad.Usuarios.ObtenerPorUsername(usernameNormalizado);
                    if (existente != null)
                    {
                        _bitacoraBLL.RegistrarFalloNegocioInterno("FormAuditLog.Value.Activity.RejectedUserCreation", "FormAuditLog.Value.Info.UsernameAlreadyExists", "UsuarioBLL", unidad);
                        throw new ExcepcionNegocio("Error.User.UsernameExists");
                    }

                    Usuario nuevoUsuario = new Usuario(usernameNormalizado)
                    {
                        PasswordHash = passwordHash,
                        Activo = true
                    };

                    nuevoUsuario.DVH = _integridadBLL.CalcularDVH(nuevoUsuario);
                    nuevoUsuario.Id = unidad.Usuarios.InsertarYObtenerId(nuevoUsuario);
                    _permisoBLL.AsignarPerfilBasicoInterno(nuevoUsuario.Id, unidad);
                    _usuarioEstadoBLL.GuardarVersionActualInterno(nuevoUsuario.Id, "FormUserHistory.Value.Action.Creation", "FormUserHistory.Value.Obs.InitialCreation", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserCreation", "FormAuditLog.Value.Info.UserCreation", "UsuarioBLL", unidad);

                    // Alta: no hay DVH anterior. Se suma el DVH del usuario
                    // nuevo al DVV persistido sin escanear Usuarios.
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, null, nuevoUsuario.DVH, unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
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
            string passwordHash = string.IsNullOrWhiteSpace(passwordNormalizada) ? null : _passwordHasher.HashPassword(passwordNormalizada);
            if (string.IsNullOrWhiteSpace(usernameNormalizado))
            {
                _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.UserUpdateEmptyUsername", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.User.UsernameRequired");
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Usuario usuario = unidad.Usuarios.ObtenerPorId(id);
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

                    Usuario existente = unidad.Usuarios.ObtenerPorUsername(usernameNormalizado);
                    if (existente != null && existente.Id != id)
                    {
                        _bitacoraBLL.RegistrarFalloNegocioInterno("FormAuditLog.Value.Activity.RejectedUserUpdate", "FormAuditLog.Value.Info.UsernameAlreadyExists", "UsuarioBLL", unidad);
                        throw new ExcepcionNegocio("Error.User.UsernameExists");
                    }

                    // Guardar el DVH antes de modificar la entidad es obligatorio:
                    // el DVV incremental necesita restar la firma vieja y sumar
                    // la firma nueva calculada mas abajo.
                    string dvhAnterior = usuario.DVH;
                    bool usuarioEstabaActivo = usuario.Activo;
                    string usernameAnterior = usuario.Username;
                    bool actualizaPassword = !string.IsNullOrWhiteSpace(passwordNormalizada);
                    usuario.Username = usernameNormalizado;
                    if (actualizaPassword)
                    {
                        usuario.PasswordHash = passwordHash;
                    }

                    usuario.Activo = activo;
                    usuario.DVH = _integridadBLL.CalcularDVH(usuario);
                    bool actualizado = unidad.Usuarios.Actualizar(usuario);
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

                    // Modificacion: reemplaza la contribucion anterior por la
                    // nueva dentro de la misma transaccion de usuario/historial.
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, dvhAnterior, usuario.DVH, unidad);
                    unidad.Save();
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
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
                try
                {
                    Usuario usuario = unidad.Usuarios.ObtenerPorId(id);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.NotFound");
                    }

                    // Baja fisica: capturar la firma antes del DELETE permite
                    // restarla del DVV luego de eliminar la fila.
                    string dvhAnterior = usuario.DVH;
                    bool eliminado = unidad.Usuarios.EliminarFisicamente(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.User.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserHardDelete", "FormAuditLog.Value.Info.UserHardDelete", "UsuarioBLL", unidad);

                    // Baja: el DVH nuevo es null porque la fila ya no participa
                    // del conjunto controlado por DVV.
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, dvhAnterior, null, unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
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
                try
                {
                    Usuario usuario = unidad.Usuarios.ObtenerPorId(snapshot.UsuarioId);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.HistoryUserNotFound");
                    }

                    // Restauracion: el snapshot reescribe datos de usuario. Por
                    // eso se conserva la firma previa y luego se calcula la firma
                    // del estado restaurado.
                    string dvhAnterior = usuario.DVH;
                    _usuarioEstadoBLL.AplicarSnapshotInterno(snapshot, unidad);
                    Usuario usuarioRestaurado = unidad.Usuarios.ObtenerPorId(snapshot.UsuarioId);
                    string dvhNuevo = _integridadBLL.CalcularDVH(usuarioRestaurado);
                    unidad.Usuarios.ActualizarDVH(snapshot.UsuarioId, dvhNuevo);
                    _usuarioEstadoBLL.GuardarVersionActualInterno(snapshot.UsuarioId, "FormUserHistory.Value.Action.Restoration", "FormUserHistory.Value.Obs.Restoration", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserRestoration", "FormAuditLog.Value.Info.UserRestoration", "UsuarioBLL", unidad);

                    // Restauracion: se reemplaza la contribucion del estado
                    // anterior por la del estado restaurado.
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, dvhAnterior, dvhNuevo, unidad);
                    unidad.Save();
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
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

    }
}
