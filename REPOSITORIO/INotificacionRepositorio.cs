using System;
using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface INotificacionRepositorio
    {
        PaginaNotificaciones ObtenerPaginaPersonal(Usuario usuario, FiltroNotificacion filtro, int pagina);

        int ContarNoLeidas(Usuario usuario);

        Notificacion ObtenerPersonal(Notificacion notificacion, Usuario usuario);

        bool MarcarPersonalLeida(Notificacion notificacion, Usuario usuario);

        Notificacion ObtenerPorId(int id);

        List<Notificacion> ObtenerPorUsuario(Usuario usuario);

        List<Notificacion> ObtenerTodas();

        int InsertarYObtenerId(Notificacion notificacion);

        bool Actualizar(Notificacion notificacion);

        bool Eliminar(Notificacion notificacion);

        int ContarPorPromocion(Promocion promocion);

        // Fecha (UTC) de la ultima notificacion automatica de inversiones del usuario; null si no tiene
        // (nunca recibio una o la elimino). Base del intervalo de siete dias: no excluye por lectura
        // y no cuenta ejemplos de laboratorio ni recordatorios manuales.
        DateTime? ObtenerFechaUltimaAutomaticaInversionUtc(Usuario usuario);

        // Punto estable de serializacion por usuario: bloquea la fila del usuario hasta el fin de la
        // transaccion de la unidad de trabajo, para ordenar entregas concurrentes del proceso
        // automatico. Devuelve false si el usuario no existe.
        bool BloquearUsuarioParaEntrega(Usuario usuario);

        // Indica si el usuario ya tiene la notificacion automatica de ese tipo para la jornada (fecha local,
        // sin hora). Respalda la deduplicacion previa; la garantia final es el indice unico de la tabla.
        bool ExisteAutomatica(Usuario usuario, TipoNotificacionAutomatica tipo, DateTime jornada);
    }
}
