using BE;

namespace REPOSITORIO
{
    public interface IDashboardRepositorio
    {
        Dashboard ObtenerPorId(int id);

        Dashboard ObtenerPorUsuario(int usuarioId);

        int InsertarYObtenerId(Dashboard dashboard);

        bool Eliminar(int id);
    }
}
