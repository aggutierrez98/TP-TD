using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Limite de preguntas por minuto de cada usuario, para cuidar la cuota gratuita de
    // Gemini (cada pregunta admitida consume al menos dos llamadas: clasificar y responder).
    // Ventana deslizante en memoria del proceso: se reinicia al cerrar la app, protege
    // rafagas, no es facturacion. Seguro para hilos porque el envio corre fuera del hilo de
    // UI. Solo las preguntas admitidas por el limite ocupan lugar en la ventana.
    public sealed class LimiteUsoAsistente : ILimiteUsoAsistente
    {
        internal const int MaximoPorDefecto = 5;
        internal const string ClaveConfiguracion = "AsistenteMaxConsultasPorMinuto";

        private static readonly TimeSpan Ventana = TimeSpan.FromMinutes(1);

        private readonly int _maximoPorMinuto;
        private readonly Func<DateTime> _reloj;
        private readonly Dictionary<int, Queue<DateTime>> _consultasPorUsuario = new Dictionary<int, Queue<DateTime>>();
        private readonly object _bloqueo = new object();

        // Lee AsistenteMaxConsultasPorMinuto de App.config y usa la hora local.
        public LimiteUsoAsistente()
            : this(InterpretarMaximoConfigurado(ConfigurationManager.AppSettings[ClaveConfiguracion]), () => DateTime.Now)
        {
        }

        // Ctor de testabilidad: maximo y reloj controlados.
        internal LimiteUsoAsistente(int maximoPorMinuto, Func<DateTime> reloj)
        {
            if (maximoPorMinuto <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximoPorMinuto));
            }

            _maximoPorMinuto = maximoPorMinuto;
            _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
        }

        public bool IntentarConsumir(IUsuarioAutenticado usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            lock (_bloqueo)
            {
                DateTime ahora = _reloj();
                Queue<DateTime> consultas;
                if (!_consultasPorUsuario.TryGetValue(usuario.Id, out consultas))
                {
                    consultas = new Queue<DateTime>();
                    _consultasPorUsuario.Add(usuario.Id, consultas);
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
