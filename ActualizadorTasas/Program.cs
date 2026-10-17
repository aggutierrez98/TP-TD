using System;
using System.Linq;
using System.Threading;
using BE;
using BLL;

namespace ActualizadorTasas
{
    internal static class Program
    {
        // Exit codes read by the Task Scheduler (Last Run Result).
        internal const int SalidaCorrecta = 0;
        internal const int SalidaFallidaRegistrada = 1;
        internal const int SalidaErrorInesperado = 2;

        // No Mutex: the cap is re-checked inside the locked transaction, so two overlapping runs
        // cannot both query the API.
        private static int Main(string[] args)
        {
            try
            {
                ResultadoEjecucionActualizacionTasas resultado = CompositionRoot.ActualizacionTasas
                    .EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();

                Informar(resultado);
                return CodigoDeSalida(resultado);
            }
            catch (OperationCanceledException)
            {
                Console.Error.WriteLine("Rate update cancelled.");
                return SalidaErrorInesperado;
            }
            catch (Exception ex)
            {
                // Only the message (a key or short text) goes to the console, never the stack trace.
                Console.Error.WriteLine("Rate update failed: " + ex.Message);
                return SalidaErrorInesperado;
            }
        }

        internal static int CodigoDeSalida(ResultadoEjecucionActualizacionTasas resultado)
        {
            if (resultado == null || resultado.Ejecucion == null)
            {
                return SalidaErrorInesperado;
            }

            return resultado.Ejecucion.Resultado == ResultadoActualizacionTasas.Fallida
                ? SalidaFallidaRegistrada
                : SalidaCorrecta;
        }

        private static void Informar(ResultadoEjecucionActualizacionTasas resultado)
        {
            EjecucionActualizacionTasas ejecucion = resultado.Ejecucion;
            if (ejecucion == null)
            {
                Console.Error.WriteLine("Rate update returned no execution.");
                return;
            }

            int actualizados = ejecucion.Detalles.Count(d => d.Estado == EstadoActualizacionTasa.Actualizado);
            int manuales = ejecucion.Detalles.Count(d => d.Estado == EstadoActualizacionTasa.SaltadoManual);
            int sinDato = ejecucion.Detalles.Count(d => d.Estado == EstadoActualizacionTasa.SaltadoSinDato);

            Console.WriteLine(string.Format(
                "Rate update {0}: {1} updated, {2} skipped (manual), {3} skipped (no data), request consumed: {4}.",
                ejecucion.Resultado,
                actualizados,
                manuales,
                sinDato,
                ejecucion.ConsumioRequest ? "yes" : "no"));

            if (!string.IsNullOrEmpty(ejecucion.Mensaje))
            {
                Console.WriteLine("Detail: " + ejecucion.Mensaje);
            }

            if (resultado.ProximaConsultaPermitida.HasValue)
            {
                Console.WriteLine("Next query allowed at: " + resultado.ProximaConsultaPermitida.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}
