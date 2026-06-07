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
                BitacoraService.GetInstance().RegistrarFalloNegocio("Login fallido", "Intento de login con campos vacios.", "UsuarioBLL");
                throw new ExcepcionNegocio("Error.Usuario.EmptyCredentials");
            }

            Usuario usuario = _usuarioDAL.ObtenerPorUsername(username.Trim());
            if (usuario == null)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("Login fallido", "Intento de login con usuario inexistente.", "UsuarioBLL");
                return null;
            }

            if (!usuario.Activo)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("Login fallido", "Intento de acceso sobre usuario inactivo.", "UsuarioBLL");
                return null;
            }

            bool esValido = _passwordHasher.VerifyPassword(passwordRaw, usuario.PasswordHash);
            if (!esValido)
            {
                BitacoraService.GetInstance().RegistrarFalloNegocio("Login fallido", "Credenciales invalidas.", "UsuarioBLL");
                return null;
            }

            _permisoBLL.CargarPermisosEnUsuario(usuario);
            Sesion.GetInstance().IniciarSesion(usuario.Id, usuario.Username);
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuario.Id));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuario.Id);
            BitacoraService.GetInstance().RegistrarOperacion("Login exitoso", "Ingreso de usuario.", "UsuarioBLL");
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
                BitacoraService.GetInstance().RegistrarFalloNegocio("Alta de usuario rechazada", "Se intento registrar un usuario con datos incompletos.", "UsuarioBLL");
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
                        BitacoraService.GetInstance().RegistrarFalloNegocio("Alta de usuario rechazada", "El nombre de usuario ya existe.", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.Usuario.UsernameExists");
                    }

                    Usuario nuevoUsuario = new Usuario(usernameNormalizado)
                    {
                        PasswordHash = _passwordHasher.HashPassword(passwordNormalizada),
                        Activo = true
                    };

                    nuevoUsuario.Id = _usuarioDAL.InsertarYObtenerId(nuevoUsuario, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(nuevoUsuario.Id, "Alta", "Alta inicial del usuario.", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("Alta de usuario", "Se creo el usuario.", "UsuarioBLL", accesoDatos);
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
                BitacoraService.GetInstance().RegistrarFalloNegocio("Edicion de usuario rechazada", "Se intento guardar un nombre vacio.", "UsuarioBLL");
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
                        BitacoraService.GetInstance().RegistrarFalloNegocio("Edicion de usuario rechazada", "Se intento desactivar al usuario en sesion.", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("Error.Usuario.CannotDeactivateActiveSession");
                    }

                    Usuario existente = _usuarioDAL.ObtenerPorUsername(usernameNormalizado, accesoDatos);
                    if (existente != null && existente.Id != id)
                    {
                        BitacoraService.GetInstance().RegistrarFalloNegocio("Edicion de usuario rechazada", "El nombre de usuario ya existe.", "UsuarioBLL", accesoDatos);
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

                    string accionHistorial = "Edicion";
                    string observacionesHistorial = "Actualizacion del usuario.";
                    string actividadBitacora = "Edicion de usuario";
                    string informacionBitacora = "Se actualizo el usuario.";

                    if (usuarioEstabaActivo && !activo)
                    {
                        accionHistorial = "BajaLogica";
                        observacionesHistorial = "Desactivacion del usuario desde gestion.";
                        actividadBitacora = "Baja logica de usuario";
                        informacionBitacora = "Se desactivo el usuario.";
                    }
                    else if (!usuarioEstabaActivo && activo)
                    {
                        accionHistorial = "Reactivacion";
                        observacionesHistorial = "Reactivacion del usuario desde gestion.";
                        actividadBitacora = "Reactivacion de usuario";
                        informacionBitacora = "Se reactivo el usuario.";
                    }
                    else if (actualizaPassword)
                    {
                        observacionesHistorial = "Actualizacion del password del usuario.";
                    }
                    else if (!string.Equals(usernameAnterior, usernameNormalizado, StringComparison.Ordinal))
                    {
                        observacionesHistorial = "Actualizacion del nombre de usuario.";
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
                BitacoraService.GetInstance().RegistrarFalloNegocio("Borrado fisico de usuario rechazado", "Se intento eliminar al usuario en sesion.", "UsuarioBLL");
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

                    BitacoraService.GetInstance().RegistrarOperacion("Borrado fisico de usuario", "Se elimino definitivamente el usuario y su historial asociado.", "UsuarioBLL", accesoDatos);
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
                    _usuarioEstadoBLL.GuardarVersionActual(snapshot.UsuarioId, "Restauracion", "Restauracion desde el historial.", accesoDatos);
                    BitacoraService.GetInstance().RegistrarOperacion("Restauracion de usuario", "Se restauro el usuario desde el historial.", "UsuarioBLL", accesoDatos);
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
