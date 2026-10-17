using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public sealed class NominatimBusquedaUbicacionesService : IBusquedaUbicacionesService
    {
        internal const string UserAgent = "FinazAppTP-UAI/1.0";
        internal const int MaximoResultados = 5;
        internal const int MaximoEntradasCache = 100;
        internal const int MaximoBytesRespuesta = 1024 * 1024;
        internal static readonly TimeSpan TiempoMaximoPeticion = TimeSpan.FromSeconds(15);
        internal static readonly TimeSpan SeparacionMinima = TimeSpan.FromSeconds(1);
        internal static readonly TimeSpan VigenciaCache = TimeSpan.FromHours(24);
        internal static readonly TimeSpan EsperaPorDefectoLimite = TimeSpan.FromSeconds(60);
        internal static readonly TimeSpan EsperaMaximaLimite = TimeSpan.FromHours(1);

        private readonly HttpClient _cliente;
        private readonly Func<string> _proveedorEndpoint;
        private readonly IRelojBusqueda _reloj;

        // Serializa las peticiones externas de esta instancia. Estado protegido por
        // la compuerta: inicio (monotonico) de la ultima peticion y fin del cooldown
        // impuesto por el proveedor. El alcance es esta instancia, es decir, un
        // proceso: no coordina varias instalaciones.
        // Cache en memoria compartido por la instancia (exitos y listas vacias).
        private readonly object _candadoCache = new object();
        private readonly Dictionary<string, EntradaCache> _cache = new Dictionary<string, EntradaCache>();

        private readonly SemaphoreSlim _compuerta = new SemaphoreSlim(1, 1);
        private TimeSpan? _ultimoInicio;
        private TimeSpan? _cooldownHasta;

        public NominatimBusquedaUbicacionesService()
            : this(CrearManejadorPorDefecto(), LeerEndpointConfigurado, new RelojSistemaBusqueda())
        {
        }

        internal NominatimBusquedaUbicacionesService(HttpMessageHandler manejador, Func<string> proveedorEndpoint, IRelojBusqueda reloj)
        {
            _cliente = new HttpClient(manejador) { Timeout = System.Threading.Timeout.InfiniteTimeSpan };
            _proveedorEndpoint = proveedorEndpoint;
            _reloj = reloj;
        }

        public async Task<IReadOnlyList<ResultadoBusquedaUbicacion>> BuscarAsync(ConsultaUbicacion consulta, CancellationToken cancellationToken)
        {
            if (consulta == null)
            {
                throw new ArgumentNullException(nameof(consulta));
            }

            if (string.IsNullOrWhiteSpace(consulta.Texto))
            {
                throw new ArgumentException("El texto de busqueda es obligatorio.", nameof(consulta));
            }

            cancellationToken.ThrowIfCancellationRequested();

            Uri endpoint = ResolverEndpoint();
            string texto = NormalizarTexto(consulta.Texto);
            string idioma = NormalizarIdioma(consulta.Idioma);
            string viewbox = FormatearViewbox(consulta.ZonaPreferida);
            string clave = string.Join("|", endpoint.AbsoluteUri, texto, idioma, viewbox ?? string.Empty);

            IReadOnlyList<ResultadoBusquedaUbicacion> enCache;
            if (TryObtenerDeCache(clave, out enCache))
            {
                return enCache;
            }

            await _compuerta.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Otra consulta igual pudo completarse mientras esta esperaba su turno.
                if (TryObtenerDeCache(clave, out enCache))
                {
                    return enCache;
                }

                VerificarCooldown();
                await EsperarSeparacionMinimaAsync(cancellationToken).ConfigureAwait(false);
                _ultimoInicio = _reloj.Monotonico;
                IReadOnlyList<ResultadoBusquedaUbicacion> resultados = await EnviarAsync(ConstruirUri(endpoint, texto, idioma, viewbox), cancellationToken).ConfigureAwait(false);
                GuardarEnCache(clave, resultados);
                return resultados;
            }
            finally
            {
                _compuerta.Release();
            }
        }

        private bool TryObtenerDeCache(string clave, out IReadOnlyList<ResultadoBusquedaUbicacion> resultados)
        {
            lock (_candadoCache)
            {
                EntradaCache entrada;
                if (_cache.TryGetValue(clave, out entrada))
                {
                    if (_reloj.Monotonico - entrada.Instante < VigenciaCache)
                    {
                        resultados = entrada.Resultados;
                        return true;
                    }

                    _cache.Remove(clave);
                }
            }

            resultados = null;
            return false;
        }

        // Descarta vencidas y, si se alcanzo el maximo, desaloja por antiguedad.
        private void GuardarEnCache(string clave, IReadOnlyList<ResultadoBusquedaUbicacion> resultados)
        {
            lock (_candadoCache)
            {
                TimeSpan ahora = _reloj.Monotonico;
                foreach (string vencida in _cache.Where(x => ahora - x.Value.Instante >= VigenciaCache).Select(x => x.Key).ToList())
                {
                    _cache.Remove(vencida);
                }

                _cache.Remove(clave);
                while (_cache.Count >= MaximoEntradasCache)
                {
                    string masAntigua = _cache.OrderBy(x => x.Value.Instante).First().Key;
                    _cache.Remove(masAntigua);
                }

                _cache[clave] = new EntradaCache(resultados, ahora);
            }
        }

        private sealed class EntradaCache
        {
            public EntradaCache(IReadOnlyList<ResultadoBusquedaUbicacion> resultados, TimeSpan instante)
            {
                Resultados = resultados;
                Instante = instante;
            }

            public IReadOnlyList<ResultadoBusquedaUbicacion> Resultados { get; }

            public TimeSpan Instante { get; }
        }

        private void VerificarCooldown()
        {
            if (!_cooldownHasta.HasValue)
            {
                return;
            }

            TimeSpan restante = _cooldownHasta.Value - _reloj.Monotonico;
            if (restante > TimeSpan.Zero)
            {
                throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.LimiteExcedido, null, restante);
            }

            _cooldownHasta = null;
        }

        private async Task EsperarSeparacionMinimaAsync(CancellationToken cancellationToken)
        {
            if (!_ultimoInicio.HasValue)
            {
                return;
            }

            TimeSpan falta = _ultimoInicio.Value + SeparacionMinima - _reloj.Monotonico;
            if (falta > TimeSpan.Zero)
            {
                await _reloj.EsperarAsync(falta, cancellationToken).ConfigureAwait(false);
            }
        }

        // Envia una unica peticion (sin reintentos) acotada por TiempoMaximoPeticion.
        // El temporizador usa el reloj inyectado; si vence cancela la peticion y se
        // informa TiempoAgotado, mientras que la cancelacion del llamador se
        // propaga como OperationCanceledException.
        private async Task<IReadOnlyList<ResultadoBusquedaUbicacion>> EnviarAsync(Uri uri, CancellationToken cancellationToken)
        {
            using (CancellationTokenSource cancelacion = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            using (CancellationTokenSource cancelacionTemporizador = CancellationTokenSource.CreateLinkedTokenSource(cancelacion.Token))
            {
                Task temporizador = _reloj.EsperarAsync(TiempoMaximoPeticion, cancelacionTemporizador.Token)
                    .ContinueWith(
                        espera =>
                        {
                            if (espera.Status == TaskStatus.RanToCompletion)
                            {
                                try
                                {
                                    cancelacion.Cancel();
                                }
                                catch (ObjectDisposedException)
                                {
                                }
                            }
                        },
                        TaskScheduler.Default);

                try
                {
                    using (HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Get, uri))
                    {
                        peticion.Headers.UserAgent.ParseAdd(UserAgent);
                        using (HttpResponseMessage respuesta = await _cliente.SendAsync(peticion, HttpCompletionOption.ResponseHeadersRead, cancelacion.Token).ConfigureAwait(false))
                        {
                            VerificarEstadoHttp(respuesta);
                            byte[] cuerpo = await LeerAcotadoAsync(respuesta.Content, cancelacion.Token).ConfigureAwait(false);
                            return InterpretarRespuesta(cuerpo);
                        }
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException ex)
                {
                    throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.TiempoAgotado, ex);
                }
                catch (HttpRequestException ex)
                {
                    throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.ConexionFallida, ex);
                }
                catch (IOException ex)
                {
                    throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.ConexionFallida, ex);
                }
                finally
                {
                    cancelacionTemporizador.Cancel();
                }
            }
        }

        // Un rechazo temporal (403/429/503) registra el cooldown compartido segun
        // Retry-After (segundos o fecha); sin cabecera, 429 usa 60 segundos.
        private void VerificarEstadoHttp(HttpResponseMessage respuesta)
        {
            if (respuesta.IsSuccessStatusCode)
            {
                return;
            }

            int codigo = (int)respuesta.StatusCode;
            if (codigo != 403 && codigo != 429 && codigo != 503)
            {
                throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.ConexionFallida, codigo, null);
            }

            TimeSpan? espera = null;
            RetryConditionHeaderValue retryAfter = respuesta.Headers.RetryAfter;
            if (retryAfter != null && retryAfter.Delta.HasValue)
            {
                espera = retryAfter.Delta.Value;
            }
            else if (retryAfter != null && retryAfter.Date.HasValue)
            {
                espera = retryAfter.Date.Value - _reloj.AhoraUtc;
            }
            else if (codigo == 429)
            {
                espera = EsperaPorDefectoLimite;
            }

            if (espera.HasValue)
            {
                espera = espera.Value < TimeSpan.Zero ? TimeSpan.Zero : (espera.Value > EsperaMaximaLimite ? EsperaMaximaLimite : espera.Value);
                if (espera.Value > TimeSpan.Zero)
                {
                    _cooldownHasta = _reloj.Monotonico + espera.Value;
                }
            }

            throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.LimiteExcedido, codigo, espera);
        }

        // Lee el cuerpo sin superar MaximoBytesRespuesta, aun si el servidor no
        // declara Content-Length o miente sobre el.
        private static async Task<byte[]> LeerAcotadoAsync(HttpContent contenido, CancellationToken cancellationToken)
        {
            long? declarada = contenido.Headers.ContentLength;
            if (declarada.HasValue && declarada.Value > MaximoBytesRespuesta)
            {
                throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.RespuestaInvalida);
            }

            using (Stream flujo = await contenido.ReadAsStreamAsync().ConfigureAwait(false))
            using (MemoryStream acumulado = new MemoryStream())
            {
                byte[] buffer = new byte[8192];
                int leidos;
                while ((leidos = await flujo.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    if (acumulado.Length + leidos > MaximoBytesRespuesta)
                    {
                        throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.RespuestaInvalida);
                    }

                    acumulado.Write(buffer, 0, leidos);
                }

                return acumulado.ToArray();
            }
        }

        // XML con DTD prohibida y sin resolucion de recursos externos. Una raiz
        // valida sin elementos es una busqueda sin coincidencias; elementos sin
        // ninguno utilizable, o XML malformado, es una respuesta invalida.
        internal static IReadOnlyList<ResultadoBusquedaUbicacion> InterpretarRespuesta(byte[] cuerpo)
        {
            XElement raiz;
            try
            {
                XmlReaderSettings configuracion = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    MaxCharactersFromEntities = 0,
                    MaxCharactersInDocument = MaximoBytesRespuesta
                };

                using (MemoryStream flujo = new MemoryStream(cuerpo, false))
                using (XmlReader lector = XmlReader.Create(flujo, configuracion))
                {
                    raiz = XDocument.Load(lector).Root;
                }
            }
            catch (XmlException ex)
            {
                throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.RespuestaInvalida, ex);
            }

            if (raiz == null || raiz.Name.LocalName != "searchresults")
            {
                throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.RespuestaInvalida);
            }

            List<ResultadoBusquedaUbicacion> resultados = new List<ResultadoBusquedaUbicacion>();
            int elementos = 0;
            foreach (XElement lugar in raiz.Elements("place"))
            {
                elementos++;
                ResultadoBusquedaUbicacion resultado = InterpretarLugar(lugar);
                if (resultado != null)
                {
                    resultados.Add(resultado);
                    if (resultados.Count == MaximoResultados)
                    {
                        break;
                    }
                }
            }

            if (elementos > 0 && resultados.Count == 0)
            {
                throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.RespuestaInvalida);
            }

            return resultados.AsReadOnly();
        }

        private static ResultadoBusquedaUbicacion InterpretarLugar(XElement lugar)
        {
            string descripcion = ((string)lugar.Attribute("display_name") ?? string.Empty).Trim();
            double latitud;
            double longitud;
            if (descripcion.Length == 0
                || !TryParseCoordenada((string)lugar.Attribute("lat"), -90, 90, out latitud)
                || !TryParseCoordenada((string)lugar.Attribute("lon"), -180, 180, out longitud))
            {
                return null;
            }

            string tipo = ((string)lugar.Attribute("type") ?? (string)lugar.Attribute("class") ?? string.Empty).Trim();
            return new ResultadoBusquedaUbicacion(
                descripcion,
                latitud,
                longitud,
                tipo.Length == 0 ? null : tipo,
                InterpretarLimites((string)lugar.Attribute("boundingbox")));
        }

        // boundingbox de Nominatim: sur,norte,oeste,este. Cualquier valor
        // invalido o degenerado se omite sin invalidar la coordenada.
        private static LimitesGeograficos InterpretarLimites(string boundingBox)
        {
            if (string.IsNullOrWhiteSpace(boundingBox))
            {
                return null;
            }

            string[] partes = boundingBox.Split(',');
            double sur;
            double norte;
            double oeste;
            double este;
            if (partes.Length != 4
                || !TryParseCoordenada(partes[0], -90, 90, out sur)
                || !TryParseCoordenada(partes[1], -90, 90, out norte)
                || !TryParseCoordenada(partes[2], -180, 180, out oeste)
                || !TryParseCoordenada(partes[3], -180, 180, out este)
                || sur >= norte
                || oeste >= este)
            {
                return null;
            }

            return new LimitesGeograficos(sur, oeste, norte, este);
        }

        private static bool TryParseCoordenada(string texto, double minimo, double maximo, out double valor)
        {
            return double.TryParse(texto, NumberStyles.Float, CultureInfo.InvariantCulture, out valor)
                && EsFinito(valor)
                && valor >= minimo
                && valor <= maximo;
        }

        private Uri ResolverEndpoint()
        {
            string configurado = _proveedorEndpoint();
            Uri endpoint;
            if (string.IsNullOrWhiteSpace(configurado)
                || !Uri.TryCreate(configurado.Trim(), UriKind.Absolute, out endpoint)
                || endpoint.Scheme != Uri.UriSchemeHttps
                || string.IsNullOrEmpty(endpoint.Host)
                || !string.IsNullOrEmpty(endpoint.UserInfo)
                || !string.IsNullOrEmpty(endpoint.Query)
                || !string.IsNullOrEmpty(endpoint.Fragment))
            {
                throw new BusquedaUbicacionException(EstadoBusquedaUbicacion.ConfiguracionInvalida);
            }

            return endpoint;
        }

        private static Uri ConstruirUri(Uri endpoint, string texto, string idioma, string viewbox)
        {
            List<string> parametros = new List<string>
            {
                Par("q", texto),
                Par("format", "xml"),
                Par("limit", MaximoResultados.ToString(CultureInfo.InvariantCulture)),
                Par("accept-language", idioma),
                Par("layer", "address,poi")
            };

            if (viewbox != null)
            {
                parametros.Add(Par("viewbox", viewbox));
                parametros.Add(Par("bounded", "0"));
            }

            UriBuilder constructor = new UriBuilder(endpoint) { Query = string.Join("&", parametros) };
            return constructor.Uri;
        }

        private static string Par(string clave, string valor)
        {
            return clave + "=" + Uri.EscapeDataString(valor);
        }

        // Recorta y colapsa espacios consecutivos; conserva signos, acentos y mayusculas.
        private static string NormalizarTexto(string texto)
        {
            return Regex.Replace(texto.Trim(), @"\s+", " ");
        }

        private static string NormalizarIdioma(string idioma)
        {
            return string.Equals(idioma, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "es";
        }

        // viewbox = oeste,norte,este,sur (lon,lat de dos esquinas). Devuelve null
        // si la zona no es un rectangulo valido (no finita, fuera de rango,
        // invertida/degenerada o que cruza el antimeridiano).
        private static string FormatearViewbox(LimitesGeograficos zona)
        {
            if (zona == null
                || !EsFinito(zona.Sur) || !EsFinito(zona.Norte) || !EsFinito(zona.Oeste) || !EsFinito(zona.Este)
                || zona.Sur < -90 || zona.Norte > 90 || zona.Oeste < -180 || zona.Este > 180
                || zona.Sur >= zona.Norte || zona.Oeste >= zona.Este)
            {
                return null;
            }

            return string.Join(",", FormatearCoordenada(zona.Oeste), FormatearCoordenada(zona.Norte), FormatearCoordenada(zona.Este), FormatearCoordenada(zona.Sur));
        }

        private static bool EsFinito(double valor)
        {
            return !double.IsNaN(valor) && !double.IsInfinity(valor);
        }

        private static string FormatearCoordenada(double valor)
        {
            double redondeado = Math.Round(valor, 4);
            if (redondeado == 0)
            {
                redondeado = 0; // evita "-0"
            }

            return redondeado.ToString("0.####", CultureInfo.InvariantCulture);
        }

        private static HttpMessageHandler CrearManejadorPorDefecto()
        {
            return new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
        }

        private static string LeerEndpointConfigurado()
        {
            return ConfigurationManager.AppSettings["GeocodingSearchEndpoint"];
        }
    }
}
