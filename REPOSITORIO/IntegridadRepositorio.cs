using DAL;

namespace REPOSITORIO
{
    public class IntegridadRepositorio : IIntegridadRepositorio
    {
        private readonly IntegridadDAL _integridadDAL;

        public IntegridadRepositorio()
        {
            _integridadDAL = new IntegridadDAL();
        }

        public string ObtenerDVV(string nombreTabla, IUnidadDeTrabajo unidad = null)
        {
            return _integridadDAL.ObtenerDVV(nombreTabla, ObtenerAccesoDatos(unidad));
        }

        public void GuardarDVV(string nombreTabla, string valor, IUnidadDeTrabajo unidad = null)
        {
            _integridadDAL.GuardarDVV(nombreTabla, valor, ObtenerAccesoDatos(unidad));
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
