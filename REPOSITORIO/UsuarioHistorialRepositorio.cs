using System.Collections.Generic;
using DAL;
using SERVICIOS;
using ABSTRACCIONES;

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

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(IUsuarioAutenticado usuario)
        {
            return _usuarioHistorialDAL.ObtenerVersiones(usuario);
        }

        public UsuarioHistorialSnapshot ObtenerPorId(int id)
        {
            return _usuarioHistorialDAL.ObtenerPorId(id);
        }

    }
}
