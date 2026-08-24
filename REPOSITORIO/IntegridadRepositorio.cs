using DAL;

namespace REPOSITORIO
{
    public class IntegridadRepositorio : IIntegridadRepositorio
    {
        private readonly IntegridadDAL _integridadDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public IntegridadRepositorio()
            : this(null)
        {
        }

        internal IntegridadRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _integridadDAL = new IntegridadDAL();
            _accesoDatos = accesoDatos;
        }

        public string ObtenerDVV(string nombreTabla)
        {
            return _integridadDAL.ObtenerDVV(nombreTabla, _accesoDatos);
        }

        public void GuardarDVV(string nombreTabla, string valor)
        {
            _integridadDAL.GuardarDVV(nombreTabla, valor, _accesoDatos);
        }
    }
}
