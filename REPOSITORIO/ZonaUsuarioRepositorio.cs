using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    // Zonas de un usuario. Usuario queda liviano (solo Id): las zonas siempre se piden
    // para el usuario de la sesion y nadie necesita el resto de sus datos.
    public class ZonaUsuarioRepositorio : IZonaUsuarioRepositorio
    {
        private readonly ZonaUsuarioDAL _zonaUsuarioDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ZonaUsuarioRepositorio()
            : this(null)
        {
        }

        internal ZonaUsuarioRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _zonaUsuarioDAL = new ZonaUsuarioDAL();
            _accesoDatos = accesoDatos;
        }

        public ZonaUsuario ObtenerPorId(int id)
        {
            return _zonaUsuarioDAL.ObtenerPorId(id, _accesoDatos);
        }

        public List<ZonaUsuario> ObtenerPorUsuario(int usuarioId)
        {
            return _zonaUsuarioDAL.ObtenerPorUsuario(usuarioId, _accesoDatos);
        }

        public int InsertarYObtenerId(ZonaUsuario zona)
        {
            return _zonaUsuarioDAL.InsertarYObtenerId(zona, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _zonaUsuarioDAL.Eliminar(id, _accesoDatos);
        }
    }
}
