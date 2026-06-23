namespace REPOSITORIO
{
    public interface ITraduccionRepositorio
    {
        int ObtenerPalabraIdPorKey(string key, IUnidadDeTrabajo unidad = null);

        bool GuardarTraduccion(int idiomaId, int palabraId, string valor, IUnidadDeTrabajo unidad = null);

        bool EliminarTraduccion(int idiomaId, int palabraId, IUnidadDeTrabajo unidad = null);
    }
}
