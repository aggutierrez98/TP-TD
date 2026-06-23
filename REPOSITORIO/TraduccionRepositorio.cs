namespace REPOSITORIO
{
    public class TraduccionRepositorio : ITraduccionRepositorio
    {
        private readonly IdiomaRepositorio _idiomaRepositorio;

        public TraduccionRepositorio()
        {
            _idiomaRepositorio = new IdiomaRepositorio();
        }

        public int ObtenerPalabraIdPorKey(string key, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaRepositorio.ObtenerPalabraIdPorKey(key, unidad);
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaRepositorio.GuardarTraduccion(idiomaId, palabraId, valor, unidad);
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaRepositorio.EliminarTraduccion(idiomaId, palabraId, unidad);
        }
    }
}
