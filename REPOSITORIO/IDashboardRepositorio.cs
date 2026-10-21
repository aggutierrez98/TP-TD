using BE;

namespace REPOSITORIO
{
    public interface IDashboardRepositorio
    {
        Dashboard ObtenerPorId(int id);

        Dashboard ObtenerPorUsuario(Usuario usuario);

        int InsertarYObtenerId(Dashboard dashboard);

        bool Eliminar(Dashboard dashboard);
    }
}
