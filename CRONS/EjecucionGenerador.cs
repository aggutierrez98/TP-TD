using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BLL;

namespace CRONS
{
    // Decide el resumen y el codigo de salida que lee el Programador de tareas (Last Run Result).
    // 0: sin fallas (las omisiones no son fallas). 1: fallas parciales registradas en la corrida.
    // 2: error fatal o corrida cancelada (no se completo): nunca se informa como exito.
    internal static class EjecucionGenerador
    {
        internal const int SalidaCorrecta = 0;
        internal const int SalidaFallasRegistradas = 1;
        internal const int SalidaErrorFatal = 2;

        internal static async Task<int> EjecutarAsync(
            Func<CancellationToken, Task<ResultadoCorridaNotificaciones>> corrida,
            TextWriter salida,
            TextWriter error,
            CancellationToken cancellationToken)
        {
            try
            {
                ResultadoCorridaNotificaciones resultado = await corrida(cancellationToken).ConfigureAwait(false);
                if (resultado == null)
                {
                    error.WriteLine("Notification generation returned no result.");
                    return SalidaErrorFatal;
                }

                if (resultado.AntesDelHorario)
                {
                    salida.WriteLine("Notification generation skipped: it is before 07:00, nothing was generated.");
                    return SalidaCorrecta;
                }

                string resumen = string.Format(
                    "Run {0} for {1:yyyy-MM-dd}: {2} delivered, {3} skipped, {4} failed.",
                    resultado.CorridaId, resultado.Jornada, resultado.Entregadas, resultado.Omitidas, resultado.Fallidas);

                if (resultado.Cancelada)
                {
                    error.WriteLine("Notification generation cancelled before finishing. " + resumen);
                    return SalidaErrorFatal;
                }

                salida.WriteLine(resumen);
                return resultado.Fallidas > 0 ? SalidaFallasRegistradas : SalidaCorrecta;
            }
            catch (OperationCanceledException)
            {
                error.WriteLine("Notification generation cancelled.");
                return SalidaErrorFatal;
            }
            catch (Exception ex)
            {
                // Solo el mensaje (una clave o texto corto) va a la consola, nunca el stack trace.
                error.WriteLine("Notification generation failed: " + ex.Message);
                return SalidaErrorFatal;
            }
        }
    }
}
