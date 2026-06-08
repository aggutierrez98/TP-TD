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

        public UsuarioBLL()
        {
            BootstrapAplicacionBLL.Inicializar();
            _usuarioDAL = new UsuarioDAL();
            _passwordHasher = new PasswordHasher();
            _idiomaBLL = new IdiomaBLL();
            _permisoBLL = new PermisoBLL();
            _usuarioEstadoBLL = new UsuarioEstadoBLL();
        }

        public Usuario Login(string username, string passwordRaw)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(passwordRaw))
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.LoginFallido", "FormBitacora.Value.Info.LoginCamposVacios", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.Usuario.EmptyCredentials");
            }

            Usuario usuario = _usuarioDAL.ObtenerPorUsername(username.Trim());
            if (usuario == null)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.LoginFallido", "FormBitacora.Value.Info.LoginUsuarioInexistente", "UsuarioBLL");
                return null;
            }

            if (!usuario.Activo)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.LoginFallido", "FormBitacora.Value.Info.LoginUsuarioInactivo", "UsuarioBLL");
                return null;
            }

            bool esValido = _passwordHasher.VerifyPassword(passwordRaw, usuario.PasswordHash);
            if (!esValido)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.LoginFallido", "FormBitacora.Value.Info.LoginCredencialesInvalidas", "UsuarioBLL");
                return null;
            }

            _permisoBLL.CargarPermisosEnUsuario(usuario);
            Sesion.GetInstance().IniciarSesion(usuario.Id, usuario.Username);
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuario.Id));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuario.Id);
            BitacoraService.GetInstance().RegistrarOperacion("FormBitacora.Value.Activity.LoginExitoso", "FormBitacora.Value.Info.LoginExitoso", "UsuarioBLL");
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
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.AltaUsuarioRechazada", "FormBitacora.Value.Info.AltaUsuarioDatosIncompletos", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.Usuario.EmptyCredentials");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario existente = _usuarioDAL.ObtenerPorUsername(usernameNormalizado, accesoDatos);
                    if (existente != null)
                    {
                        BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.AltaUsuarioRechazada", "FormBitacora.Value.Info.NombreUsuarioYaExiste", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.Usuario.UsernameExists");
                    }

                    Usuario nuevoUsuario = new Usuario(usernameNormalizado)
                    {
                        PasswordHash = _passwordHasher.HashPassword(passwordNormalizada),
                        Activo = true
                    };

                    nuevoUsuario.Id = _usuarioDAL.InsertarYObtenerId(nuevoUsuario, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(nuevoUsuario.Id, "FormHistorialUsuario.Value.Action.Alta", "FormHistorialUsuario.Value.Obs.AltaInicial", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormBitacora.Value.Activity.AltaUsuario", "FormBitacora.Value.Info.AltaUsuario", "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("Error.Usuario.RegisterFailed", ex);
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
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.EdicionUsuarioRechazada", "FormBitacora.Value.Info.EdicionUsuarioNombreVacio", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.Usuario.UsernameRequired");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Usuario.NotFound");
                    }

                    int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
                    if (usuarioActualId.HasValue && usuarioActualId.Value == id && !activo)
                    {
                        BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.EdicionUsuarioRechazada", "FormBitacora.Value.Info.EdicionDesactivarSesion", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.Usuario.CannotDeactivateActiveSession");
                    }

                    Usuario existente = _usuarioDAL.ObtenerPorUsername(usernameNormalizado, accesoDatos);
                    if (existente != null && existente.Id != id)
                    {
                        BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.EdicionUsuarioRechazada", "FormBitacora.Value.Info.NombreUsuarioYaExiste", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.Usuario.UsernameExists");
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
                    bool actualizado = _usuarioDAL.Actualizar(usuario, accesoDatos);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.Usuario.UpdateFailed");
                    }

                    string accionHistorial = "FormHistorialUsuario.Value.Action.Edicion";
                    string observacionesHistorial = "FormHistorialUsuario.Value.Obs.ActualizacionUsuario";
                    string actividadBitacora = "FormBitacora.Value.Activity.EdicionUsuario";
                    string informacionBitacora = "FormBitacora.Value.Info.EdicionUsuario";

                    if (usuarioEstabaActivo && !activo)
                    {
                        accionHistorial = "FormHistorialUsuario.Value.Action.BajaLogica";
                        observacionesHistorial = "FormHistorialUsuario.Value.Obs.Desactivacion";
                        actividadBitacora = "FormBitacora.Value.Activity.BajaLogicaUsuario";
                        informacionBitacora = "FormBitacora.Value.Info.BajaLogicaUsuario";
                    }
                    else if (!usuarioEstabaActivo && activo)
                    {
                        accionHistorial = "FormHistorialUsuario.Value.Action.Reactivacion";
                        observacionesHistorial = "FormHistorialUsuario.Value.Obs.Reactivacion";
                        actividadBitacora = "FormBitacora.Value.Activity.ReactivacionUsuario";
                        informacionBitacora = "FormBitacora.Value.Info.ReactivacionUsuario";
                    }
                    else if (actualizaPassword)
                    {
                        observacionesHistorial = "FormHistorialUsuario.Value.Obs.ActualizacionPassword";
                    }
                    else if (!string.Equals(usernameAnterior, usernameNormalizado, StringComparison.Ordinal))
                    {
                        observacionesHistorial = "FormHistorialUsuario.Value.Obs.ActualizacionUsername";
                    }

                    _usuarioEstadoBLL.GuardarVersionActual(id, accionHistorial, observacionesHistorial, accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion(actividadBitacora, informacionBitacora, "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("Error.Usuario.UpdateFailed", ex);
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
                BitacoraService.GetInstance().RegistrarFalloNegocio("FormBitacora.Value.Activity.BorradoFisicoUsuarioRechazado", "FormBitacora.Value.Info.BorradoFisicoSesion", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.Usuario.CannotDeleteActiveSession");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Usuario.NotFound");
                    }

                    bool eliminado = _usuarioDAL.EliminarFisicamente(id, accesoDatos);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.Usuario.DeleteFailed");
                    }

                    BitacoraService.GetInstance().RegistrarOperacion("FormBitacora.Value.Activity.BorradoFisicoUsuario", "FormBitacora.Value.Info.BorradoFisicoUsuario", "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("Error.Usuario.DeleteFailed", ex);
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
                throw new ExcepcionNegocio("Error.Usuario.RestoreVersionNotFound");
            }

            int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == snapshot.UsuarioId && !snapshot.Activo)
            {
                throw new ExcepcionNegocio("Error.Usuario.CannotRestoreInactiveCurrentSession");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(snapshot.UsuarioId, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("Error.Usuario.HistoryUserNotFound");
                    }

                    _usuarioEstadoBLL.AplicarSnapshot(snapshot, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(snapshot.UsuarioId, "FormHistorialUsuario.Value.Action.Restauracion", "FormHistorialUsuario.Value.Obs.Restauracion", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("FormBitacora.Value.Activity.RestauracionUsuario", "FormBitacora.Value.Info.RestauracionUsuario", "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("Error.Usuario.RestoreVersionFailed", ex);
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
    }
}
