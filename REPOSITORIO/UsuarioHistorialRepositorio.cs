using System.Collections.Generic;
using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class UsuarioHistorialRepositorio : IUsuarioHistorialRepositorio
    {
        private readonly UsuarioHistorialDAL _usuarioHistorialDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public UsuarioHistorialRepositorio()
            : this(null)
        {
        }

        internal UsuarioHistorialRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _usuarioHistorialDAL = new UsuarioHistorialDAL();
            _accesoDatos = accesoDatos;
        }

        public int GuardarVersion(UsuarioHistorialSnapshot snapshot)
        {
            return _usuarioHistorialDAL.GuardarVersion(snapshot, _accesoDatos);
        }

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(int usuarioId)
        {
            return _usuarioHistorialDAL.ObtenerVersiones(usuarioId);
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            return _usuarioHistorialDAL.ObtenerVersion(historialId);
        }

    }
}
