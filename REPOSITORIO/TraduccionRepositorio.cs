namespace REPOSITORIO
{
    using DAL;

    public class TraduccionRepositorio : ITraduccionRepositorio
    {
        private readonly IdiomaRepositorio _idiomaRepositorio;

        public TraduccionRepositorio()
            : this(null)
        {
        }

        internal TraduccionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _idiomaRepositorio = new IdiomaRepositorio(accesoDatos);
        }

        public int ObtenerPalabraIdPorKey(string key)
        {
            return _idiomaRepositorio.ObtenerPalabraIdPorKey(key);
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor)
        {
            return _idiomaRepositorio.GuardarTraduccion(idiomaId, palabraId, valor);
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            return _idiomaRepositorio.EliminarTraduccion(idiomaId, palabraId);
        }
    }
}
