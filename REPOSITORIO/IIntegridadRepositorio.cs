namespace REPOSITORIO
{
    public interface IIntegridadRepositorio
    {
        string ObtenerDVV(string nombreTabla, IUnidadDeTrabajo unidad = null);

        void GuardarDVV(string nombreTabla, string valor, IUnidadDeTrabajo unidad = null);
    }
}
