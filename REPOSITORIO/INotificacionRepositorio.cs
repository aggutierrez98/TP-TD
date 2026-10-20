using System;
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

        // Fecha (UTC) de la ultima notificacion automatica de inversiones del usuario; null si no tiene
        // (nunca recibio una o la elimino). Base del intervalo de siete dias: no excluye por lectura
        // y no cuenta ejemplos de laboratorio ni recordatorios manuales.
        DateTime? ObtenerFechaUltimaAutomaticaInversionUtc(int usuarioId);

        // Punto estable de serializacion por usuario: bloquea la fila del usuario hasta el fin de la
        // transaccion de la unidad de trabajo, para ordenar entregas concurrentes del proceso
        // automatico. Devuelve false si el usuario no existe.
        bool BloquearUsuarioParaEntrega(int usuarioId);

        // Indica si el usuario ya tiene la notificacion automatica de ese tipo para la jornada (fecha local,
        // sin hora). Respalda la deduplicacion previa; la garantia final es el indice unico de la tabla.
        bool ExisteAutomatica(int usuarioId, TipoNotificacionAutomatica tipo, DateTime jornada);
    }
}
