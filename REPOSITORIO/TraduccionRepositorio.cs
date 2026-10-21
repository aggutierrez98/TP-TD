namespace REPOSITORIO
{
    using DAL;
    using SERVICIOS;

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

        public Palabra ObtenerPalabraPorKey(string key)
        {
            return _idiomaRepositorio.ObtenerPalabraPorKey(key);
        }

        public bool GuardarTraduccion(Traduccion traduccion)
        {
            return _idiomaRepositorio.GuardarTraduccion(traduccion);
        }

        public bool EliminarTraduccion(Traduccion traduccion)
        {
            return _idiomaRepositorio.EliminarTraduccion(traduccion);
        }
    }
}
