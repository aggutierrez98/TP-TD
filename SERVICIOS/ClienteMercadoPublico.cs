using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Transporte comun a las fuentes publicas. El timeout incluye recibir el cuerpo.
    internal sealed class ClienteMercadoPublico
    {
        private readonly HttpClient _cliente;
        private readonly TimeSpan _timeout;
        private readonly Func<DateTime> _ahora;
        private readonly TimeSpan _vigencia;
        private readonly object _candado = new object();
        private readonly Dictionary<string, Tuple<DateTime, IReadOnlyList<IDatoMercado>>> _cache = new Dictionary<string, Tuple<DateTime, IReadOnlyList<IDatoMercado>>>();

        internal ClienteMercadoPublico(HttpMessageHandler handler, TimeSpan timeout, Func<DateTime> ahora, TimeSpan vigencia)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            _cliente = new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };
            _cliente.DefaultRequestHeaders.UserAgent.ParseAdd("FINAZAPP/1.0");
            _timeout = timeout;
            _ahora = ahora;
            _vigencia = vigencia;
        }

        internal async Task<IReadOnlyList<IDatoMercado>> ObtenerAsync(string url, Func<JsonElement, List<DatoMercado>> parsear, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            lock (_candado)
            {
                if (_cache.TryGetValue(url, out var entrada) && _ahora() >= entrada.Item1 && _ahora() - entrada.Item1 < _vigencia) return entrada.Item2;
            }
            using (CancellationTokenSource tiempo = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                tiempo.CancelAfter(_timeout);
                try
                {
                    using (HttpResponseMessage respuesta = await _cliente.GetAsync(url, tiempo.Token).ConfigureAwait(false))
                    {
                        respuesta.EnsureSuccessStatusCode();
                        using (JsonDocument json = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false)))
                        {
                            List<DatoMercado> datos = parsear(json.RootElement);
                            if (datos.Count == 0) throw new FormatException("La fuente no contiene datos utilizables.");
                            IReadOnlyList<IDatoMercado> resultado = datos.AsReadOnly();
                            ct.ThrowIfCancellationRequested();
                            lock (_candado) _cache[url] = Tuple.Create(_ahora(), resultado);
                            return resultado;
                        }
                    }
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
                catch (Exception ex) when (ex is HttpRequestException || ex is OperationCanceledException || ex is JsonException || ex is FormatException || ex is InvalidOperationException || ex is OverflowException)
                {
                    throw new ExcepcionTecnica("Error.Assistant.MarketUnavailable", ex);
                }
            }
        }

        internal static string Texto(JsonElement e, string nombre) => e.TryGetProperty(nombre, out JsonElement v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
        internal static decimal? Numero(JsonElement e, string nombre) => e.TryGetProperty(nombre, out JsonElement v) && v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out decimal n) ? n : (decimal?)null;
        internal static DateTime? Fecha(JsonElement e, string nombre)
        {
            return DateTime.TryParseExact(Texto(e, nombre), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha) ? fecha : (DateTime?)null;
        }
    }
}
