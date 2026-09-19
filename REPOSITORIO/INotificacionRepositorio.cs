using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface INotificacionRepositorio
    {
        Notificacion ObtenerPorId(int id);

        List<Notificacion> ObtenerPorUsuario(int usuarioId);

        List<Notificacion> ObtenerTodas();

        int InsertarYObtenerId(Notificacion notificacion);

        bool Actualizar(Notificacion notificacion);

        bool Eliminar(int id);
    }
}
