using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class UsuarioPreferenciaRepositorio : IUsuarioPreferenciaRepositorio
    {
        private readonly UsuarioPreferenciaDAL _usuarioPreferenciaDAL;

        public UsuarioPreferenciaRepositorio()
        {
            _usuarioPreferenciaDAL = new UsuarioPreferenciaDAL();
        }

        public UsuarioPreferencia ObtenerPorUsuarioId(int usuarioId, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioPreferenciaDAL.ObtenerPorUsuarioId(usuarioId, ObtenerAccesoDatos(unidad));
        }

        public bool Guardar(UsuarioPreferencia preferencia, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioPreferenciaDAL.Guardar(preferencia, ObtenerAccesoDatos(unidad));
        }

        public bool Guardar(int usuarioId, int? idiomaId, IUnidadDeTrabajo unidad = null)
        {
            return _usuarioPreferenciaDAL.Guardar(usuarioId, idiomaId, ObtenerAccesoDatos(unidad));
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
