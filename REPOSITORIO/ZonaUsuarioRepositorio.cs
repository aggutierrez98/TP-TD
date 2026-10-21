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

        public List<ZonaUsuario> ObtenerPorUsuario(Usuario usuario)
        {
            return _zonaUsuarioDAL.ObtenerPorUsuario(usuario, _accesoDatos);
        }

        public int InsertarYObtenerId(ZonaUsuario zona)
        {
            return _zonaUsuarioDAL.InsertarYObtenerId(zona, _accesoDatos);
        }

        public bool Eliminar(ZonaUsuario zona)
        {
            return _zonaUsuarioDAL.Eliminar(zona, _accesoDatos);
        }
    }
}
