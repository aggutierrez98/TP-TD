using System;
using DAL;

namespace REPOSITORIO
{
    internal sealed class UnidadDeTrabajoSqlServer : IUnidadDeTrabajo
    {
        private AccesoDatosSqlServer _accesoDatos;
        private bool _guardado;
        private bool _disposeInvocado;

        private IBitacoraRepositorio _bitacora;
        private IIdiomaRepositorio _idiomas;
        private IIntegridadRepositorio _integridad;
        private IPermisoRepositorio _permisos;
        private ITraduccionRepositorio _traducciones;
        private IUsuarioHistorialRepositorio _usuarioHistorial;
        private IUsuarioPermisoRepositorio _usuarioPermisos;
        private IUsuarioPreferenciaRepositorio _usuarioPreferencias;
        private IUsuarioRepositorio _usuarios;

        public UnidadDeTrabajoSqlServer()
        {
        }

        public IBitacoraRepositorio Bitacora
        {
            get { return _bitacora ?? (_bitacora = new BitacoraRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IIdiomaRepositorio Idiomas
        {
            get { return _idiomas ?? (_idiomas = new IdiomaRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IIntegridadRepositorio Integridad
        {
            get { return _integridad ?? (_integridad = new IntegridadRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IPermisoRepositorio Permisos
        {
            get { return _permisos ?? (_permisos = new PermisoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public ITraduccionRepositorio Traducciones
        {
            get { return _traducciones ?? (_traducciones = new TraduccionRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioHistorialRepositorio UsuarioHistorial
        {
            get { return _usuarioHistorial ?? (_usuarioHistorial = new UsuarioHistorialRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioPermisoRepositorio UsuarioPermisos
        {
            get { return _usuarioPermisos ?? (_usuarioPermisos = new UsuarioPermisoRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioPreferenciaRepositorio UsuarioPreferencias
        {
            get { return _usuarioPreferencias ?? (_usuarioPreferencias = new UsuarioPreferenciaRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public IUsuarioRepositorio Usuarios
        {
            get { return _usuarios ?? (_usuarios = new UsuarioRepositorio(ObtenerAccesoDatosTransaccional())); }
        }

        public void Save()
        {
            if (_guardado)
            {
                return;
            }

            if (_accesoDatos != null)
            {
                _accesoDatos.CommitTransactionScope();
            }

            _guardado = true;
        }

        private AccesoDatosSqlServer ObtenerAccesoDatosTransaccional()
        {
            if (_disposeInvocado)
            {
                throw new ObjectDisposedException("UnidadDeTrabajoSqlServer");
            }

            if (_guardado)
            {
                throw new InvalidOperationException("La unidad de trabajo ya fue confirmada.");
            }

            if (_accesoDatos == null)
            {
                _accesoDatos = new AccesoDatosSqlServer();
                _accesoDatos.BeginTransactionScope();
            }

            return _accesoDatos;
        }

        public void Dispose()
        {
            if (_disposeInvocado)
            {
                return;
            }

            try
            {
                if (!_guardado && _accesoDatos != null)
                {
                    _accesoDatos.RollbackTransactionScope();
                }
            }
            catch
            {
            }
            finally
            {
                if (_accesoDatos != null)
                {
                    _accesoDatos.Dispose();
                    _accesoDatos = null;
                }

                _disposeInvocado = true;
            }
        }
    }
}
