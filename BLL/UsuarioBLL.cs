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
            InfraestructuraBLL.Inicializar();
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
                Sesion.GetInstance().RegistrarFalloNegocio("Login fallido", "Intento de login con campos vacios.", "UsuarioBLL");
                throw new ExcepcionNegocio("El usuario y la contrasena no pueden estar vacios.");
            }

            Usuario usuario = _usuarioDAL.ObtenerPorUsername(username.Trim());
            if (usuario == null)
            {
                Sesion.GetInstance().RegistrarFalloNegocio("Login fallido", "Usuario inexistente: " + username.Trim() + ".", "UsuarioBLL");
                return null;
            }

            if (!usuario.Activo)
            {
                Sesion.GetInstance().RegistrarFalloNegocio("Login fallido", "Intento de acceso sobre usuario inactivo: " + usuario.Username + ".", "UsuarioBLL");
                return null;
            }

            bool esValido = _passwordHasher.VerifyPassword(passwordRaw, usuario.PasswordHash);
            if (!esValido)
            {
                Sesion.GetInstance().RegistrarFalloNegocio("Login fallido", "Credenciales invalidas para el usuario " + usuario.Username + ".", "UsuarioBLL");
                return null;
            }

            _permisoBLL.CargarPermisosEnUsuario(usuario);
            Sesion.GetInstance().IniciarSesion(usuario.Id, usuario.Username);
            Sesion.GetInstance().ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuario.Id));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuario.Id);
            Sesion.GetInstance().RegistrarOperacion("Login exitoso", "Ingreso del usuario " + usuario.Username + ".", "UsuarioBLL");
            return usuario;
        }

        public bool Registrar(string username, string passwordRaw)
        {
            string usernameNormalizado = (username ?? string.Empty).Trim();
            string passwordNormalizada = (passwordRaw ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(usernameNormalizado) || string.IsNullOrWhiteSpace(passwordNormalizada))
            {
                Sesion.GetInstance().RegistrarFalloNegocio("Alta de usuario rechazada", "Se intento registrar un usuario con datos incompletos.", "UsuarioBLL");
                throw new ExcepcionNegocio("El usuario y la contrasena no pueden estar vacios.");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario existente = _usuarioDAL.ObtenerPorUsername(usernameNormalizado, accesoDatos);
                    if (existente != null)
                    {
                        Sesion.GetInstance().RegistrarFalloNegocio("Alta de usuario rechazada", "El nombre de usuario " + usernameNormalizado + " ya existe.", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("El nombre de usuario ya existe. Por favor elegi otro.");
                    }

                    Usuario nuevoUsuario = new Usuario(usernameNormalizado)
                    {
                        PasswordHash = _passwordHasher.HashPassword(passwordNormalizada),
                        Activo = true
                    };

                    nuevoUsuario.Id = _usuarioDAL.InsertarYObtenerId(nuevoUsuario, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(nuevoUsuario.Id, "Alta", "Alta inicial del usuario.", accesoDatos);
                    Sesion.GetInstance().RegistrarOperacion("Alta de usuario", "Se creo el usuario " + usernameNormalizado + ".", "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("No se pudo registrar el usuario.", ex);
                }
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioDAL.ObtenerTodos();
        }

        public bool Actualizar(int id, string nuevoUsername)
        {
            string usernameNormalizado = (nuevoUsername ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(usernameNormalizado))
            {
                Sesion.GetInstance().RegistrarFalloNegocio("Edicion de usuario rechazada", "Se intento guardar un nombre vacio para el usuario ID " + id + ".", "UsuarioBLL");
                throw new ExcepcionNegocio("El nombre de usuario no puede estar vacio.");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("No se encontro el usuario seleccionado.");
                    }

                    if (!usuario.Activo)
                    {
                        throw new ExcepcionNegocio("El usuario seleccionado se encuentra inactivo. Debe restaurarlo desde el historial.");
                    }

                    Usuario existente = _usuarioDAL.ObtenerPorUsername(usernameNormalizado, accesoDatos);
                    if (existente != null && existente.Id != id)
                    {
                        Sesion.GetInstance().RegistrarFalloNegocio("Edicion de usuario rechazada", "El nombre de usuario " + usernameNormalizado + " ya existe.", "UsuarioBLL", accesoDatos);
                        throw new ExcepcionNegocio("El nombre de usuario ya existe. Por favor elegi otro.");
                    }

                    usuario.Username = usernameNormalizado;
                    bool actualizado = _usuarioDAL.Actualizar(usuario, accesoDatos);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("No se pudo actualizar el usuario.");
                    }

                    _usuarioEstadoBLL.GuardarVersionActual(id, "Edicion", "Actualizacion del nombre de usuario.", accesoDatos);
                    Sesion.GetInstance().RegistrarOperacion("Edicion de usuario", "Se actualizo el usuario " + usernameNormalizado + ".", "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("No se pudo actualizar el usuario.", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == id)
            {
                Sesion.GetInstance().RegistrarFalloNegocio("Baja de usuario rechazada", "Se intento desactivar al usuario en sesion.", "UsuarioBLL");
                throw new ExcepcionNegocio("No se puede desactivar al usuario que tiene la sesion activa.");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(id, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("No se encontro el usuario seleccionado.");
                    }

                    if (!usuario.Activo)
                    {
                        throw new ExcepcionNegocio("El usuario ya se encuentra inactivo.");
                    }

                    bool desactivado = _usuarioDAL.Desactivar(id, accesoDatos);
                    if (!desactivado)
                    {
                        throw new ExcepcionTecnica("No se pudo desactivar el usuario.");
                    }

                    _usuarioEstadoBLL.GuardarVersionActual(id, "BajaLogica", "Desactivacion del usuario.", accesoDatos);
                    Sesion.GetInstance().RegistrarOperacion("Baja logica de usuario", "Se desactivo el usuario " + usuario.Username + ".", "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("No se pudo desactivar el usuario.", ex);
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
                throw new ExcepcionNegocio("No se encontro la version seleccionada.");
            }

            int? usuarioActualId = Sesion.GetInstance().GetUsuarioActualId();
            if (usuarioActualId.HasValue && usuarioActualId.Value == snapshot.UsuarioId && !snapshot.Activo)
            {
                throw new ExcepcionNegocio("No se puede restaurar al estado inactivo del usuario con sesion actual.");
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    Usuario usuario = _usuarioDAL.ObtenerPorId(snapshot.UsuarioId, accesoDatos);
                    if (usuario == null)
                    {
                        throw new ExcepcionNegocio("No se encontro el usuario asociado al historial.");
                    }

                    _usuarioEstadoBLL.AplicarSnapshot(snapshot, accesoDatos);
                    _usuarioEstadoBLL.GuardarVersionActual(snapshot.UsuarioId, "Restauracion", "Restauracion desde historial #" + historialId + ".", accesoDatos);
                    Sesion.GetInstance().RegistrarOperacion("Restauracion de usuario", "Se restauro el usuario " + snapshot.Username + " desde el historial #" + historialId + ".", "UsuarioBLL", accesoDatos);
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
                    throw new ExcepcionTecnica("No se pudo restaurar la version seleccionada.", ex);
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
            Sesion.GetInstance().ActualizarPermisosUsuarioActual(_permisoBLL.ObtenerPermisosDirectosUsuario(usuarioId));
            _idiomaBLL.AplicarIdiomaSesionUsuario(usuarioId);
        }
    }
}
