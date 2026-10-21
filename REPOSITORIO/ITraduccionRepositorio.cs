namespace REPOSITORIO
{
    using SERVICIOS;
    public interface ITraduccionRepositorio
    {
        Palabra ObtenerPalabraPorKey(string key);

        bool GuardarTraduccion(Traduccion traduccion);

        bool EliminarTraduccion(Traduccion traduccion);
    }
}
