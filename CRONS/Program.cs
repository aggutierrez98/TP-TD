using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRONS.Tareas;

namespace CRONS
{
    internal static class Program
    {
        // Una invocacion ejecuta una sola tarea: CRONS.exe tasas | notificaciones.
        // Para agregar una tarea nueva: una clase en Tareas/ y una entrada en esta tabla.
        private static int Main(string[] args)
        {
            Despachador despachador = new Despachador(
                new Dictionary<string, Func<CancellationToken, Task<int>>>
                {
                    { "tasas", ActualizarTasas.EjecutarAsync },
                    { "notificaciones", GenerarNotificaciones.EjecutarAsync }
                },
                Console.Error);

            using (CancellationTokenSource cancelacion = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cancelacion.Cancel();
                };

                return despachador.EjecutarAsync(args, cancelacion.Token).GetAwaiter().GetResult();
            }
        }
    }
}
