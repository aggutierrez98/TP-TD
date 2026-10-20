using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly UsuarioDAL _usuarioDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public UsuarioRepositorio()
            : this(null)
        {
        }

        internal UsuarioRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _usuarioDAL = new UsuarioDAL();
            _accesoDatos = accesoDatos;
        }

        public Usuario ObtenerPorId(int id)
        {
            return _usuarioDAL.ObtenerPorId(id, _accesoDatos);
        }

        public Usuario ObtenerPorUsername(string username)
        {
            return _usuarioDAL.ObtenerPorUsername(username, _accesoDatos);
        }

        public int InsertarYObtenerId(Usuario usuario)
        {
            return _usuarioDAL.InsertarYObtenerId(usuario, _accesoDatos);
        }

        public bool Actualizar(Usuario usuario)
        {
            return _usuarioDAL.Actualizar(usuario, _accesoDatos);
        }

        public bool ActualizarDVH(int id, string dvh)
        {
            return _usuarioDAL.ActualizarDVH(id, dvh, _accesoDatos);
        }

        public bool ActualizarEstadoBloqueo(int id, int intentosFallidos, bool bloqueado, DateTime? bloqueadoHasta, string dvh)
        {
            return _usuarioDAL.ActualizarEstadoBloqueo(id, intentosFallidos, bloqueado, bloqueadoHasta, dvh, _accesoDatos);
        }

        public bool GuardarEstadoCompleto(Usuario usuario)
        {
            return _usuarioDAL.GuardarEstadoCompleto(usuario, _accesoDatos);
        }

        public bool EliminarFisicamente(int id)
        {
            return _usuarioDAL.EliminarFisicamente(id, _accesoDatos);
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioDAL.ObtenerTodos(_accesoDatos);
        }

        public List<int> ObtenerIdsActivosDespuesDe(int ultimoId, int cantidad)
        {
            return _usuarioDAL.ObtenerIdsActivosDespuesDe(ultimoId, cantidad, _accesoDatos);
        }
    }
}
