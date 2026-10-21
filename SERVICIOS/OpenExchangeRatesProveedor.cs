using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Cliente de Open Exchange Rates. Endpoint: GET https://openexchangerates.org/api/latest.json?app_id=<clave>
    // (el plan gratuito solo admite base USD). Respuesta: { "timestamp": <unix segundos>, "base": "USD",
    // "rates": { "ARS": 1513.93, ... } }. Error: { "error": true, "status": <http>, "message": "<codigo>",
    // "description": "..." } con status 401 (clave faltante o invalida), 403, 404, 429 o 400.
    // Una consulta trae todas las monedas; no reintenta. La clave viaja en la URL, asi que ningun mensaje
    // de error la incluye.
    public sealed class OpenExchangeRatesProveedor : IProveedorTasasCambio
    {
        internal const string Endpoint = "https://openexchangerates.org/api/latest.json";
        internal const string ClaveAppSettings = "OpenExchangeRatesApiKey";
        internal const string MonedaBaseEsperada = "USD";
        internal static readonly TimeSpan TiempoMaximoPeticion = TimeSpan.FromSeconds(15);

        private readonly HttpClient _cliente;
        private readonly Func<string> _proveedorClave;
        private readonly TimeSpan _tiempoMaximo;

        public OpenExchangeRatesProveedor()
            : this(new HttpClientHandler(), LeerClaveConfigurada, TiempoMaximoPeticion)
        {
        }

        internal OpenExchangeRatesProveedor(HttpMessageHandler manejador, Func<string> proveedorClave, TimeSpan tiempoMaximo)
        {
            // El .NET Framework 4.7.2 no habilita TLS 1.2 por defecto en todos los equipos.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            _cliente = new HttpClient(manejador) { Timeout = System.Threading.Timeout.InfiniteTimeSpan };
            _proveedorClave = proveedorClave;
            _tiempoMaximo = tiempoMaximo;
        }

        public async Task<ITasasCambioObtenidas> ObtenerTasasAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string clave = _proveedorClave();
            if (string.IsNullOrWhiteSpace(clave))
            {
                throw new ProveedorTasasException(EstadoProveedorTasas.ClaveFaltante);
            }

            Uri uri = new Uri(Endpoint + "?app_id=" + Uri.EscapeDataString(clave.Trim()));
            RespuestaHttp respuesta = await EnviarAsync(uri, cancellationToken).ConfigureAwait(false);

            if (!respuesta.Exitosa)
            {
                throw CrearErrorHttp(respuesta);
            }

            return Interpretar(respuesta.Cuerpo);
        }

        private static string LeerClaveConfigurada()
        {
            return ConfigurationManager.AppSettings[ClaveAppSettings];
        }

        // Un vencimiento del tiempo maximo es un fallo de conexion; la cancelacion pedida
        // por el llamador se propaga tal cual.
        private async Task<RespuestaHttp> EnviarAsync(Uri uri, CancellationToken cancellationToken)
        {
            using (CancellationTokenSource limite = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                limite.CancelAfter(_tiempoMaximo);

                try
                {
                    using (HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Get, uri))
                    using (HttpResponseMessage respuesta = await _cliente.SendAsync(peticion, limite.Token).ConfigureAwait(false))
                    {
                        string cuerpo = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return new RespuestaHttp((int)respuesta.StatusCode, respuesta.IsSuccessStatusCode, cuerpo);
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException ex)
                {
                    throw new ProveedorTasasException(EstadoProveedorTasas.ConexionFallida, ex);
                }
                catch (HttpRequestException ex)
                {
                    throw new ProveedorTasasException(EstadoProveedorTasas.ConexionFallida, ex);
                }
                catch (IOException ex)
                {
                    throw new ProveedorTasasException(EstadoProveedorTasas.ConexionFallida, ex);
                }
            }
        }

        private static ProveedorTasasException CrearErrorHttp(RespuestaHttp respuesta)
        {
            EstadoProveedorTasas estado;
            if (respuesta.Codigo == 401 || respuesta.Codigo == 403)
            {
                estado = EstadoProveedorTasas.ClaveInvalida;
            }
            else if (respuesta.Codigo == 429)
            {
                estado = EstadoProveedorTasas.LimiteExcedido;
            }
            else
            {
                estado = EstadoProveedorTasas.Rechazado;
            }

            return new ProveedorTasasException(estado, respuesta.Codigo, ExtraerCodigoDeError(respuesta.Cuerpo));
        }

        // El cuerpo de un error puede no ser JSON (por ejemplo una pagina HTML de un proxy).
        private static string ExtraerCodigoDeError(string cuerpo)
        {
            try
            {
                using (JsonDocument documento = JsonDocument.Parse(cuerpo))
                {
                    JsonElement raiz = documento.RootElement;
                    JsonElement mensaje;
                    if (raiz.ValueKind == JsonValueKind.Object &&
                        raiz.TryGetProperty("message", out mensaje) &&
                        mensaje.ValueKind == JsonValueKind.String)
                    {
                        return mensaje.GetString();
                    }
                }
            }
            catch (JsonException)
            {
            }

            return null;
        }

        private static TasasCambioObtenidas Interpretar(string cuerpo)
        {
            try
            {
                using (JsonDocument documento = JsonDocument.Parse(cuerpo))
                {
                    JsonElement raiz = documento.RootElement;
                    if (raiz.ValueKind != JsonValueKind.Object || EsRespuestaDeError(raiz))
                    {
                        throw new ProveedorTasasException(EstadoProveedorTasas.RespuestaInvalida);
                    }

                    JsonElement baseElemento;
                    if (!raiz.TryGetProperty("base", out baseElemento) ||
                        baseElemento.ValueKind != JsonValueKind.String ||
                        !string.Equals(baseElemento.GetString(), MonedaBaseEsperada, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ProveedorTasasException(EstadoProveedorTasas.RespuestaInvalida);
                    }

                    return new TasasCambioObtenidas(MonedaBaseEsperada, LeerFechaFuente(raiz), LeerTasas(raiz));
                }
            }
            catch (JsonException ex)
            {
                throw new ProveedorTasasException(EstadoProveedorTasas.RespuestaInvalida, ex);
            }
        }

        private static bool EsRespuestaDeError(JsonElement raiz)
        {
            JsonElement error;
            return raiz.TryGetProperty("error", out error) && error.ValueKind == JsonValueKind.True;
        }

        private static IReadOnlyDictionary<string, decimal> LeerTasas(JsonElement raiz)
        {
            JsonElement tasas;
            if (!raiz.TryGetProperty("rates", out tasas) || tasas.ValueKind != JsonValueKind.Object)
            {
                throw new ProveedorTasasException(EstadoProveedorTasas.RespuestaInvalida);
            }

            Dictionary<string, decimal> resultado = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (JsonProperty tasa in tasas.EnumerateObject())
            {
                decimal valor;
                if (tasa.Value.ValueKind != JsonValueKind.Number || !tasa.Value.TryGetDecimal(out valor))
                {
                    throw new ProveedorTasasException(EstadoProveedorTasas.RespuestaInvalida);
                }

                resultado[tasa.Name] = valor;
            }

            if (resultado.Count == 0)
            {
                throw new ProveedorTasasException(EstadoProveedorTasas.RespuestaInvalida);
            }

            return resultado;
        }

        // El instante de la fuente es informativo: un timestamp ausente o ilegible no invalida las tasas.
        private static DateTime? LeerFechaFuente(JsonElement raiz)
        {
            JsonElement timestamp;
            long segundos;
            if (!raiz.TryGetProperty("timestamp", out timestamp) ||
                timestamp.ValueKind != JsonValueKind.Number ||
                !timestamp.TryGetInt64(out segundos))
            {
                return null;
            }

            try
            {
                return DateTimeOffset.FromUnixTimeSeconds(segundos).UtcDateTime;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private sealed class RespuestaHttp
        {
            public RespuestaHttp(int codigo, bool exitosa, string cuerpo)
            {
                Codigo = codigo;
                Exitosa = exitosa;
                Cuerpo = cuerpo;
            }

            public int Codigo { get; }

            public bool Exitosa { get; }

            public string Cuerpo { get; }
        }
    }
}
