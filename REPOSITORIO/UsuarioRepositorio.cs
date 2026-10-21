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

        public bool ActualizarDVH(Usuario usuario, string dvh)
        {
            return _usuarioDAL.ActualizarDVH(usuario, dvh, _accesoDatos);
        }

        public bool ActualizarEstadoBloqueo(Usuario usuario, int intentosFallidos, bool bloqueado, DateTime? bloqueadoHasta, string dvh)
        {
            return _usuarioDAL.ActualizarEstadoBloqueo(usuario, intentosFallidos, bloqueado, bloqueadoHasta, dvh, _accesoDatos);
        }

        public bool GuardarEstadoCompleto(Usuario usuario)
        {
            return _usuarioDAL.GuardarEstadoCompleto(usuario, _accesoDatos);
        }

        public bool EliminarFisicamente(Usuario usuario)
        {
            return _usuarioDAL.EliminarFisicamente(usuario, _accesoDatos);
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioDAL.ObtenerTodos(_accesoDatos);
        }

        public List<Usuario> ObtenerActivosDespuesDe(Usuario ultimo, int cantidad)
        {
            return _usuarioDAL.ObtenerActivosDespuesDe(ultimo, cantidad, _accesoDatos);
        }
    }
}
