using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Embeddings de Gemini (verificado el 2026-10-18 en https://ai.google.dev/gemini-api/docs/embeddings):
    // POST models/gemini-embedding-2:batchEmbedContents con requests[] { model, content.parts[].text,
    // outputDimensionality }, respuesta embeddings[].values en el mismo orden. gemini-embedding-2 no
    // acepta taskType: la tarea va como prefijo del texto, igual para productos e items, y con 768
    // dimensiones el vector ya llega normalizado. La clave viaja como header, nunca en la URL. El
    // nivel gratuito cuenta cada texto como un pedido (100 por minuto); quien consume acota el lote.
    public sealed class GeminiServicioEmbeddings : IServicioEmbeddings
    {
        internal const string ModeloEmbeddings = "gemini-embedding-2";
        internal const int Dimension = 768;
        internal const int TextosPorLote = 100;
        internal const string PrefijoTarea = "task: sentence similarity | query: ";
        internal static readonly TimeSpan TiempoMaximoPeticion = TimeSpan.FromSeconds(20);

        private readonly HttpClient _cliente;
        private readonly Func<string> _proveedorClave;
        private readonly TimeSpan _tiempoMaximo;

        public GeminiServicioEmbeddings()
            : this(new HttpClientHandler(), () => ConfigurationManager.AppSettings["GeminiApiKey"], TiempoMaximoPeticion)
        {
        }

        // Ctor de testabilidad: manejador HTTP falso, clave y tiempo maximo inyectados.
        internal GeminiServicioEmbeddings(HttpMessageHandler manejador, Func<string> proveedorClave, TimeSpan tiempoMaximo)
        {
            // El .NET Framework 4.7.2 no habilita TLS 1.2 por defecto en todos los equipos.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            _cliente = new HttpClient(manejador) { Timeout = Timeout.InfiniteTimeSpan };
            _proveedorClave = proveedorClave;
            _tiempoMaximo = tiempoMaximo;
        }

        public string Modelo
        {
            get { return ModeloEmbeddings; }
        }

        public async Task<IReadOnlyList<float[]>> CalcularAsync(IReadOnlyList<string> textos, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<float[]> vectores = new List<float[]>();
            if (textos == null || textos.Count == 0)
            {
                return vectores;
            }

            string clave = _proveedorClave();
            if (string.IsNullOrWhiteSpace(clave))
            {
                throw new ExcepcionTecnica("Error.Assistant.ApiKeyMissing");
            }

            for (int inicio = 0; inicio < textos.Count; inicio += TextosPorLote)
            {
                List<string> lote = textos.Skip(inicio).Take(TextosPorLote).ToList();
                string cuerpo = await PostearAsync(ArmarRequestBody(lote), clave.Trim(), cancellationToken).ConfigureAwait(false);
                vectores.AddRange(Interpretar(cuerpo, lote.Count));
            }

            return vectores;
        }

        internal static string ArmarRequestBody(IEnumerable<string> textos)
        {
            var request = new
            {
                requests = textos.Select(texto => new
                {
                    model = "models/" + ModeloEmbeddings,
                    content = new { parts = new object[] { new { text = PrefijoTarea + (texto ?? string.Empty) } } },
                    outputDimensionality = Dimension
                }).ToArray()
            };

            return JsonSerializer.Serialize(request);
        }

        // Un vencimiento del tiempo maximo es un fallo tecnico; la cancelacion pedida por el
        // llamador se propaga tal cual.
        private async Task<string> PostearAsync(string requestBody, string clave, CancellationToken cancellationToken)
        {
            using (CancellationTokenSource limite = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                limite.CancelAfter(_tiempoMaximo);

                try
                {
                    using (HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, GeminiChatClient.EndpointBase + ModeloEmbeddings + ":batchEmbedContents"))
                    {
                        peticion.Headers.Add("x-goog-api-key", clave);
                        peticion.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                        using (HttpResponseMessage respuesta = await _cliente.SendAsync(peticion, limite.Token).ConfigureAwait(false))
                        {
                            string cuerpo = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);
                            if (!respuesta.IsSuccessStatusCode)
                            {
                                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
                            }

                            return cuerpo;
                        }
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException ex)
                {
                    throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
                }
                catch (HttpRequestException ex)
                {
                    throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
                }
                catch (IOException ex)
                {
                    throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
                }
            }
        }

        private static List<float[]> Interpretar(string cuerpo, int cantidadEsperada)
        {
            try
            {
                using (JsonDocument documento = JsonDocument.Parse(cuerpo))
                {
                    JsonElement raiz = documento.RootElement;
                    JsonElement embeddings;
                    if (raiz.ValueKind != JsonValueKind.Object ||
                        !raiz.TryGetProperty("embeddings", out embeddings) ||
                        embeddings.ValueKind != JsonValueKind.Array ||
                        embeddings.GetArrayLength() != cantidadEsperada)
                    {
                        throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
                    }

                    List<float[]> vectores = new List<float[]>();
                    foreach (JsonElement embedding in embeddings.EnumerateArray())
                    {
                        JsonElement valores;
                        if (embedding.ValueKind != JsonValueKind.Object ||
                            !embedding.TryGetProperty("values", out valores) ||
                            valores.ValueKind != JsonValueKind.Array ||
                            valores.GetArrayLength() != Dimension)
                        {
                            throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
                        }

                        vectores.Add(valores.EnumerateArray().Select(valor => valor.GetSingle()).ToArray());
                    }

                    return vectores;
                }
            }
            catch (JsonException ex)
            {
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
            }
            catch (FormatException ex)
            {
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
            }
        }
    }
}
