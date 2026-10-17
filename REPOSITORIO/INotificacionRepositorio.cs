using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface INotificacionRepositorio
    {
        PaginaNotificaciones ObtenerPaginaPersonal(int usuarioId, FiltroNotificacion filtro, int pagina);

        int ContarNoLeidas(int usuarioId);

        Notificacion ObtenerPersonal(int id, int usuarioId);

        bool MarcarPersonalLeida(int id, int usuarioId);

        Notificacion ObtenerPorId(int id);

        List<Notificacion> ObtenerPorUsuario(int usuarioId);

        List<Notificacion> ObtenerTodas();

        int InsertarYObtenerId(Notificacion notificacion);

        bool Actualizar(Notificacion notificacion);

        bool Eliminar(int id);

        int ContarPorPromocion(int promocionId);
    }
}
