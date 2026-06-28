namespace REPOSITORIO
{
    public interface ITraduccionRepositorio
    {
        int ObtenerPalabraIdPorKey(string key);

        bool GuardarTraduccion(int idiomaId, int palabraId, string valor);

        bool EliminarTraduccion(int idiomaId, int palabraId);
    }
}
