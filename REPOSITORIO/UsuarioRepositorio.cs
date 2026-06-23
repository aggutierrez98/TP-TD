using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly UsuarioDAL _usuarioDAL;

        public UsuarioRepositorio()
        {
            _usuarioDAL = new UsuarioDAL();
        }

        public Usuario ObtenerPorId(int id, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.ObtenerPorId(id, ObtenerAccesoDatos(unidad));
        }

        public Usuario ObtenerPorUsername(string username, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.ObtenerPorUsername(username, ObtenerAccesoDatos(unidad));
        }

        public int InsertarYObtenerId(Usuario usuario, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.InsertarYObtenerId(usuario, ObtenerAccesoDatos(unidad));
        }

        public bool Actualizar(Usuario usuario, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.Actualizar(usuario, ObtenerAccesoDatos(unidad));
        }

        public bool ActualizarDVH(int id, string dvh, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.ActualizarDVH(id, dvh, ObtenerAccesoDatos(unidad));
        }

        public bool GuardarEstadoCompleto(Usuario usuario, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.GuardarEstadoCompleto(usuario, ObtenerAccesoDatos(unidad));
        }

        public bool EliminarFisicamente(int id, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.EliminarFisicamente(id, ObtenerAccesoDatos(unidad));
        }

        public List<Usuario> ObtenerTodos(IUnidadDeTrabajo unidad = null)
        {
            return _usuarioDAL.ObtenerTodos(ObtenerAccesoDatos(unidad));
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
