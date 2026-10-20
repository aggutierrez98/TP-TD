using System;
using System.Collections.Generic;
using System.Globalization;

namespace BLL
{
    // Limite de preguntas por minuto de cada usuario, para cuidar la cuota gratuita de
    // Gemini (cada pregunta admitida consume al menos dos llamadas: clasificar y responder).
    // Ventana deslizante en memoria del proceso: se reinicia al cerrar la app, protege
    // rafagas, no es facturacion. Seguro para hilos porque el envio corre fuera del hilo de
    // UI. Solo las preguntas admitidas por el limite ocupan lugar en la ventana.
    internal sealed class LimiteUsoAsistente
    {
        internal const int MaximoPorDefecto = 5;
        internal const string ClaveConfiguracion = "AsistenteMaxConsultasPorMinuto";

        private static readonly TimeSpan Ventana = TimeSpan.FromMinutes(1);

        private readonly int _maximoPorMinuto;
        private readonly Func<DateTime> _reloj;
        private readonly Dictionary<int, Queue<DateTime>> _consultasPorUsuario = new Dictionary<int, Queue<DateTime>>();
        private readonly object _bloqueo = new object();

        internal LimiteUsoAsistente(int maximoPorMinuto, Func<DateTime> reloj)
        {
            if (maximoPorMinuto <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximoPorMinuto));
            }

            _maximoPorMinuto = maximoPorMinuto;
            _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
        }

        internal bool IntentarConsumir(int usuarioId)
        {
            lock (_bloqueo)
            {
                DateTime ahora = _reloj();
                Queue<DateTime> consultas;
                if (!_consultasPorUsuario.TryGetValue(usuarioId, out consultas))
                {
                    consultas = new Queue<DateTime>();
                    _consultasPorUsuario.Add(usuarioId, consultas);
                }

                while (consultas.Count > 0 && ahora - consultas.Peek() >= Ventana)
                {
                    consultas.Dequeue();
                }

                if (consultas.Count >= _maximoPorMinuto)
                {
                    return false;
                }

                consultas.Enqueue(ahora);
                return true;
            }
        }

        // Valor de App.config (AsistenteMaxConsultasPorMinuto); ausente o invalido usa 5.
        internal static int InterpretarMaximoConfigurado(string valor)
        {
            int maximo;
            if (int.TryParse((valor ?? string.Empty).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out maximo) && maximo > 0)
            {
                return maximo;
            }

            return MaximoPorDefecto;
        }
    }
}
