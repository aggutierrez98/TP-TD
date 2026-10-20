using System;
using System.Threading;
using System.Threading.Tasks;
using BLL;

namespace CRONS.Tareas
{
    // Tarea "notificaciones": generacion diaria de notificaciones automaticas, ejecutada a las 07:00 y al
    // iniciar sesion por el Programador de tareas. Sin Mutex: la base es la autoridad (indice unico +
    // bloqueo por usuario), asi que dos corridas superpuestas no duplican avisos.
    internal static class GenerarNotificaciones
    {
        internal static Task<int> EjecutarAsync(CancellationToken cancellationToken)
        {
            // CompositionRoot valida la configuracion al construirse; un fallo ahi cae en EjecucionGenerador como fatal.
            return EjecucionGenerador.EjecutarAsync(
                ct => CompositionRoot.GeneracionNotificaciones.EjecutarAsync(ct),
                Console.Out,
                Console.Error,
                cancellationToken);
        }
    }
}
