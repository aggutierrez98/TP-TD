using System.Collections.Generic;
using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class UsuarioHistorialRepositorio : IUsuarioHistorialRepositorio
    {
        private readonly UsuarioHistorialDAL _usuarioHistorialDAL;

        public UsuarioHistorialRepositorio()
        {
            _usuarioHistorialDAL = new UsuarioHistorialDAL();
        }

        public int GuardarVersion(UsuarioHistorialSnapshot snapshot, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioHistorialDAL.GuardarVersion(snapshot, ObtenerAccesoDatos(unidad));
        }

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(int usuarioId)
        {
            return _usuarioHistorialDAL.ObtenerVersiones(usuarioId);
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            return _usuarioHistorialDAL.ObtenerVersion(historialId);
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
