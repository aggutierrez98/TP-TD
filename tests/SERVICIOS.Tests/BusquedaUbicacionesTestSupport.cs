using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    // Reloj controlado por el test: el tiempo solo avanza con Avanzar y las
    // esperas quedan pendientes hasta que se alcanza su vencimiento. Asi los
    // tests del limitador, el cooldown, el cache y el timeout no duermen.
    internal sealed class RelojFalso : IRelojBusqueda
    {
        private readonly object _candado = new object();
        private readonly List<Espera> _pendientes = new List<Espera>();
        private readonly List<Observador> _observadores = new List<Observador>();
        private readonly DateTimeOffset _inicioUtc;
        private TimeSpan _ahora = TimeSpan.FromHours(1);

        public RelojFalso(DateTimeOffset? inicioUtc = null)
        {
            _inicioUtc = inicioUtc ?? new DateTimeOffset(2026, 10, 14, 12, 0, 0, TimeSpan.Zero);
        }

        public TimeSpan Monotonico
        {
            get { lock (_candado) { return _ahora; } }
        }

        public DateTimeOffset AhoraUtc
        {
            get { lock (_candado) { return _inicioUtc + _ahora; } }
        }

        public Task EsperarAsync(TimeSpan duracion, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            Espera espera = new Espera
            {
                Duracion = duracion,
                Vencimiento = Monotonico + duracion,
                Origen = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)
            };

            if (duracion <= TimeSpan.Zero)
            {
                espera.Origen.SetResult(true);
                return espera.Origen.Task;
            }

            List<Observador> avisar = new List<Observador>();
            lock (_candado)
            {
                _pendientes.Add(espera);
                foreach (Observador o in _observadores.ToArray())
                {
                    if (o.Duracion == duracion)
                    {
                        avisar.Add(o);
                        _observadores.Remove(o);
                    }
                }
            }

            cancellationToken.Register(() =>
            {
                lock (_candado)
                {
                    _pendientes.Remove(espera);
                }

                espera.Origen.TrySetCanceled(cancellationToken);
            });

            foreach (Observador o in avisar)
            {
                o.Origen.TrySetResult(true);
            }

            return espera.Origen.Task;
        }

        public void Avanzar(TimeSpan duracion)
        {
            List<Espera> vencidas = new List<Espera>();
            lock (_candado)
            {
                _ahora += duracion;
                foreach (Espera e in _pendientes.ToArray())
                {
                    if (e.Vencimiento <= _ahora)
                    {
                        vencidas.Add(e);
                        _pendientes.Remove(e);
                    }
                }
            }

            foreach (Espera e in vencidas)
            {
                e.Origen.TrySetResult(true);
            }
        }

        // Completa cuando hay (o aparece) una espera pendiente con esa duracion.
        public Task EsperaPendienteAsync(TimeSpan duracion)
        {
            lock (_candado)
            {
                foreach (Espera e in _pendientes)
                {
                    if (e.Duracion == duracion)
                    {
                        return Task.FromResult(true);
                    }
                }

                Observador observador = new Observador
                {
                    Duracion = duracion,
                    Origen = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)
                };
                _observadores.Add(observador);
                return observador.Origen.Task;
            }
        }

        public int CantidadEsperasPendientes(TimeSpan duracion)
        {
            lock (_candado)
            {
                int n = 0;
                foreach (Espera e in _pendientes)
                {
                    if (e.Duracion == duracion)
                    {
                        n++;
                    }
                }

                return n;
            }
        }

        private sealed class Espera
        {
            public TimeSpan Duracion;
            public TimeSpan Vencimiento;
            public TaskCompletionSource<bool> Origen;
        }

        private sealed class Observador
        {
            public TimeSpan Duracion;
            public TaskCompletionSource<bool> Origen;
        }
    }

    internal sealed class PeticionCapturada
    {
        public Uri Uri;
        public HttpMethod Metodo;
        public string UserAgent;
        public TimeSpan Instante;
    }

    // HttpMessageHandler falso: registra cada peticion al momento de enviarla
    // (HttpClient descarta el request al terminar) y responde con el delegado.
    internal sealed class ManejadorHttpFalso : HttpMessageHandler
    {
        private readonly Func<PeticionCapturada, CancellationToken, Task<HttpResponseMessage>> _responder;
        private readonly RelojFalso _reloj;
        private readonly object _candado = new object();
        private readonly List<PeticionCapturada> _peticiones = new List<PeticionCapturada>();

        public ManejadorHttpFalso(Func<PeticionCapturada, CancellationToken, Task<HttpResponseMessage>> responder, RelojFalso reloj = null)
        {
            _responder = responder;
            _reloj = reloj;
        }

        public ManejadorHttpFalso(Func<HttpResponseMessage> responder, RelojFalso reloj = null)
            : this((p, ct) => Task.FromResult(responder()), reloj)
        {
        }

        public IReadOnlyList<PeticionCapturada> Peticiones
        {
            get { lock (_candado) { return _peticiones.ToArray(); } }
        }

        public int CantidadPeticiones
        {
            get { lock (_candado) { return _peticiones.Count; } }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            PeticionCapturada captura = new PeticionCapturada
            {
                Uri = request.RequestUri,
                Metodo = request.Method,
                UserAgent = request.Headers.UserAgent.ToString(),
                Instante = _reloj == null ? TimeSpan.Zero : _reloj.Monotonico
            };

            lock (_candado)
            {
                _peticiones.Add(captura);
            }

            return _responder(captura, cancellationToken);
        }
    }

    // Contenido sin longitud conocida: fuerza a leer el flujo para acotar el tamano.
    internal sealed class ContenidoSinLongitud : HttpContent
    {
        private readonly byte[] _bytes;

        public ContenidoSinLongitud(byte[] bytes)
        {
            _bytes = bytes;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return stream.WriteAsync(_bytes, 0, _bytes.Length);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }

    internal static class BusquedaUbicacionesPruebas
    {
        public const string EndpointPorDefecto = "https://nominatim.example.org/search";
        public const string XmlVacio = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><searchresults timestamp=\"t\" querystring=\"q\"></searchresults>";

        public static NominatimBusquedaUbicacionesService Crear(ManejadorHttpFalso manejador, RelojFalso reloj = null, string endpoint = EndpointPorDefecto)
        {
            return new NominatimBusquedaUbicacionesService(manejador, () => endpoint, reloj ?? new RelojFalso());
        }

        public static HttpResponseMessage RespuestaXml(string xml, HttpStatusCode codigo = HttpStatusCode.OK)
        {
            return new HttpResponseMessage(codigo)
            {
                Content = new StringContent(xml, Encoding.UTF8, "text/xml")
            };
        }

        public static string Place(string nombre, string lat, string lon, string boundingBox = null, string tipo = "cafe", string clase = "amenity")
        {
            string bbox = boundingBox == null ? string.Empty : " boundingbox=\"" + boundingBox + "\"";
            return "<place place_id=\"1\" osm_type=\"node\" osm_id=\"2\"" + bbox + " lat=\"" + lat + "\" lon=\"" + lon + "\" display_name=\"" +
                System.Security.SecurityElement.Escape(nombre) + "\" class=\"" + clase + "\" type=\"" + tipo + "\"/>";
        }

        public static string Resultados(params string[] places)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><searchresults timestamp=\"t\" querystring=\"q\">" + string.Concat(places) + "</searchresults>";
        }

        public static Dictionary<string, string> ParsearConsulta(Uri uri)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();
            foreach (string par in uri.Query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int igual = par.IndexOf('=');
                string clave = Uri.UnescapeDataString(par.Substring(0, igual));
                string valor = Uri.UnescapeDataString(par.Substring(igual + 1));
                resultado.Add(clave, valor);
            }

            return resultado;
        }
    }
}
