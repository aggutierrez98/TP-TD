namespace REPOSITORIO
{
    public interface IIntegridadRepositorio
    {
        string ObtenerDVV(string nombreTabla);

        void GuardarDVV(string nombreTabla, string valor);
    }
}
