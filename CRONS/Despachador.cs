using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRONS
{
    // Resuelve la tarea pedida por el primer argumento y ejecuta solo esa. Cada tarea devuelve el
    // codigo de salida que lee el Programador de tareas (0 sin fallas, 1 fallas registradas, 2 fatal).
    internal sealed class Despachador
    {
        internal const int SalidaErrorInesperado = 2;

        private readonly IDictionary<string, Func<CancellationToken, Task<int>>> _tareas;
        private readonly TextWriter _error;

        internal Despachador(IDictionary<string, Func<CancellationToken, Task<int>>> tareas, TextWriter error)
        {
            if (tareas == null) throw new ArgumentNullException(nameof(tareas));
            if (error == null) throw new ArgumentNullException(nameof(error));

            _tareas = new Dictionary<string, Func<CancellationToken, Task<int>>>(tareas, StringComparer.OrdinalIgnoreCase);
            _error = error;
        }

        internal async Task<int> EjecutarAsync(string[] args, CancellationToken cancellationToken)
        {
            string nombre = args != null && args.Length > 0 ? args[0] : null;
            if (string.IsNullOrWhiteSpace(nombre))
            {
                _error.WriteLine("No task given. Usage: CRONS.exe <task>. Valid tasks: " + Validas() + ".");
                return SalidaErrorInesperado;
            }

            Func<CancellationToken, Task<int>> tarea;
            if (!_tareas.TryGetValue(nombre.Trim(), out tarea))
            {
                _error.WriteLine("Unknown task '" + nombre + "'. Valid tasks: " + Validas() + ".");
                return SalidaErrorInesperado;
            }

            try
            {
                return await tarea(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _error.WriteLine("Task '" + nombre + "' cancelled.");
                return SalidaErrorInesperado;
            }
            catch (Exception ex)
            {
                // Solo el mensaje (una clave o texto corto) va a la consola, nunca el stack trace.
                _error.WriteLine("Task '" + nombre + "' failed: " + ex.Message);
                return SalidaErrorInesperado;
            }
        }

        private string Validas()
        {
            return string.Join(", ", _tareas.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase));
        }
    }
}
