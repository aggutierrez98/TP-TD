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

        private const int UmbralBloqueoTemporalCorto = 3;
        private const int UmbralBloqueoTemporalLargo = 4;
        private const int UmbralBloqueoPermanente = 5;
        private static readonly TimeSpan DuracionBloqueoCorto = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan DuracionBloqueoLargo = TimeSpan.FromMinutes(5);

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

            // Soft lockout (OWASP): un bloqueo activo se rechaza sin verificar
            // password ni tocar el contador, para no permitir que probar
            // contrasenas durante la espera salte de etapa.
            if (usuarioBD.Bloqueado)
            {
                if (usuarioBD.BloqueadoHasta == null)
                {
                    _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.LoginRejectedPermanentlyLocked", "UsuarioBLL");
                    throw new ExcepcionNegocio("Error.User.LoginPermanentlyLocked");
                }

                if (usuarioBD.BloqueadoHasta.Value > DateTime.Now)
                {
                    _bitacoraBLL.RegistrarFalloNegocio("FormAuditLog.Value.Activity.FailedLogin", "FormAuditLog.Value.Info.LoginRejectedTemporarilyLocked", "UsuarioBLL");
                    throw new ExcepcionNegocio("Error.User.LoginTemporarilyLocked");
                }

                // BloqueadoHasta ya vencio: el intento sigue su curso normal y el
                // estado de bloqueo se resuelve mas abajo segun exito o fallo.
            }

            bool esValido = _passwordHasher.VerifyPassword(passwordRaw, usuarioBD.PasswordHash);
            if (!esValido)
            {
                RegistrarIntentoFallidoYAplicarBloqueoSiCorresponde(usuarioBD);
                return null;
            }

            ResetearContadorYBloqueoInterno(usuarioBD);

            _sesion.IniciarSesion(usuarioBD);
            _autorizacion.ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuarioBD.Id));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuarioBD.Id);
            _bitacoraBLL.RegistrarOperacion("FormAuditLog.Value.Activity.SuccessfulLogin", "FormAuditLog.Value.Info.SuccessfulLogin", "UsuarioBLL");
            return usuarioBD;
        }

        private void RegistrarIntentoFallidoYAplicarBloqueoSiCorresponde(Usuario usuarioBD)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    string dvhAnterior = usuarioBD.DVH;
                    int nuevosIntentos = usuarioBD.IntentosFallidos + 1;
                    bool nuevoBloqueado;
                    DateTime? nuevoBloqueadoHasta;
                    string informacionBitacora;

                    if (nuevosIntentos == UmbralBloqueoTemporalCorto)
                    {
                        nuevoBloqueado = true;
                        nuevoBloqueadoHasta = DateTime.Now.Add(DuracionBloqueoCorto);
                        informacionBitacora = "FormAuditLog.Value.Info.LoginLockedTemporaryShort";
                    }
                    else if (nuevosIntentos == UmbralBloqueoTemporalLargo)
                    {
                        nuevoBloqueado = true;
                        nuevoBloqueadoHasta = DateTime.Now.Add(DuracionBloqueoLargo);
                        informacionBitacora = "FormAuditLog.Value.Info.LoginLockedTemporaryLong";
                    }
                    else if (nuevosIntentos >= UmbralBloqueoPermanente)
                    {
                        nuevoBloqueado = true;
                        nuevoBloqueadoHasta = null;
                        informacionBitacora = "FormAuditLog.Value.Info.LoginLockedPermanent";
                    }
                    else
                    {
                        nuevoBloqueado = false;
                        nuevoBloqueadoHasta = null;
                        informacionBitacora = "FormAuditLog.Value.Info.LoginInvalidCredentials";
                    }

                    usuarioBD.IntentosFallidos = nuevosIntentos;
                    usuarioBD.Bloqueado = nuevoBloqueado;
                    usuarioBD.BloqueadoHasta = nuevoBloqueadoHasta;
                    usuarioBD.DVH = _integridadBLL.CalcularDVH(usuarioBD);

                    unidad.Usuarios.ActualizarEstadoBloqueo(usuarioBD.Id, nuevosIntentos, nuevoBloqueado, nuevoBloqueadoHasta, usuarioBD.DVH);
                    _bitacoraBLL.RegistrarFalloNegocioInterno("FormAuditLog.Value.Activity.FailedLogin", informacionBitacora, "UsuarioBLL", unidad);
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, dvhAnterior, usuarioBD.DVH, unidad);
                    unidad.Save();
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.User.LoginRegisterAttemptFailed", ex);
                }
            }
        }

        private void ResetearContadorYBloqueoInterno(Usuario usuarioBD)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    string dvhAnterior = usuarioBD.DVH;
                    usuarioBD.IntentosFallidos = 0;
                    usuarioBD.Bloqueado = false;
                    usuarioBD.BloqueadoHasta = null;
                    usuarioBD.DVH = _integridadBLL.CalcularDVH(usuarioBD);

                    unidad.Usuarios.ActualizarEstadoBloqueo(usuarioBD.Id, 0, false, null, usuarioBD.DVH);
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, dvhAnterior, usuarioBD.DVH, unidad);
                    unidad.Save();
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.User.LoginRegisterAttemptFailed", ex);
                }
            }
        }

        public bool DesbloquearUsuario(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Usuario usuario = unidad.Usuarios.ObtenerPorId(id);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.User.NotFound");
                    }

                    if (!usuario.Bloqueado)
                    {
                        throw new ExcepcionNegocio("Error.User.NotLocked");
                    }

                    string dvhAnterior = usuario.DVH;
                    usuario.IntentosFallidos = 0;
                    usuario.Bloqueado = false;
                    usuario.BloqueadoHasta = null;
                    usuario.DVH = _integridadBLL.CalcularDVH(usuario);

                    bool actualizado = unidad.Usuarios.ActualizarEstadoBloqueo(id, 0, false, null, usuario.DVH);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.User.UnlockFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserUnlock", "FormAuditLog.Value.Info.UserUnlock", "UsuarioBLL", unidad);
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, dvhAnterior, usuario.DVH, unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.User.UnlockFailed", ex);
                }
            }
        }

        public void CerrarSesionActual()
        {
            _autorizacion.Limpiar();
            _sesion.CerrarSesion();
        }

        public bool Registrar(Usuario usuario, string passwordRaw)
        {
            RegistrarInterno(usuario, passwordRaw);
            return true;
        }

        public ContinuacionPerfilInversor RegistrarParaOnboarding(Usuario usuario, string passwordRaw)
        {
            int propietario = RegistrarInterno(usuario, passwordRaw);
            return new ContinuacionPerfilInversor(propietario, DateTime.UtcNow);
        }

        private int RegistrarInterno(Usuario usuario, string passwordRaw)
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

                    // El DVH depende del Id (esta marcado con [DigitoVerificador]),
                    // por lo que hay que insertar primero para conocer el Id real y
                    // recien entonces calcular y persistir la firma. Calcularla con
                    // Id=0 dejaria un DVH desfasado respecto al recalculo global y
                    // dispararia un falso positivo de corrupcion horizontal.
                    nuevoUsuario.Id = unidad.Usuarios.InsertarYObtenerId(nuevoUsuario);
                    nuevoUsuario.DVH = _integridadBLL.CalcularDVH(nuevoUsuario);
                    unidad.Usuarios.ActualizarDVH(nuevoUsuario.Id, nuevoUsuario.DVH);
                    _permisoBLL.AsignarPerfilBasicoInterno(nuevoUsuario.Id, unidad);
                    _usuarioEstadoBLL.GuardarVersionActualInterno(nuevoUsuario.Id, "FormUserHistory.Value.Action.Creation", "FormUserHistory.Value.Obs.InitialCreation", unidad);
                    _bitacoraBLL.RegistrarOperacionInterno("FormAuditLog.Value.Activity.UserCreation", "FormAuditLog.Value.Info.UserCreation", "UsuarioBLL", unidad);

                    // Alta: no hay DVH anterior. Se suma el DVH del usuario
                    // nuevo al DVV persistido sin escanear Usuarios.
                    _integridadBLL.ActualizarDVVInterno(TablaUsuarios, null, nuevoUsuario.DVH, unidad);
                    unidad.Save();
                    return nuevoUsuario.Id;
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

                    _permisoBLL.ValidarPuedeEliminarUsuario(id, unidad);

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
