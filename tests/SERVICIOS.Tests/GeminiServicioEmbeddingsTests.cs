using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SERVICIOS.Tests
{
    // Cliente de embeddings de Gemini (batchEmbedContents). Todas las pruebas usan un
    // manejador HTTP falso: nunca se llama a la API real.
    [TestClass]
    public sealed class GeminiServicioEmbeddingsTests
    {
        private const string Clave = "clave-embeddings-de-prueba";

        [TestMethod]
        public async Task CalcularAsync_VariosTextos_EnviaUnSoloLoteConModeloPrefijoYDimension()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta(Vectores(2)));
            GeminiServicioEmbeddings servicio = Crear(manejador);

            await servicio.CalcularAsync(new[] { "birra", "Cerveza rubia Quilmes" }, CancellationToken.None);

            Assert.AreEqual(1, manejador.Peticiones.Count);
            PeticionEmbeddings peticion = manejador.Peticiones[0];
            Assert.AreEqual(HttpMethod.Post, peticion.Metodo);
            Assert.AreEqual("https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-2:batchEmbedContents", peticion.Uri.ToString());
            Assert.AreEqual(Clave, peticion.ClaveApi);
            Assert.IsFalse(peticion.Uri.ToString().Contains(Clave));

            using (JsonDocument documento = JsonDocument.Parse(peticion.Cuerpo))
            {
                JsonElement[] pedidos = documento.RootElement.GetProperty("requests").EnumerateArray().ToArray();
                Assert.AreEqual(2, pedidos.Length);
                Assert.AreEqual("models/gemini-embedding-2", pedidos[0].GetProperty("model").GetString());
                Assert.AreEqual(768, pedidos[0].GetProperty("outputDimensionality").GetInt32());
                Assert.AreEqual("task: sentence similarity | query: birra", pedidos[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString());
                Assert.AreEqual("task: sentence similarity | query: Cerveza rubia Quilmes", pedidos[1].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString());
                Assert.IsFalse(pedidos[0].TryGetProperty("taskType", out JsonElement _));
            }
        }

        [TestMethod]
        public async Task CalcularAsync_RespuestaValida_DevuelveUnVectorPorTextoEnOrden()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta(Vectores(2)));
            GeminiServicioEmbeddings servicio = Crear(manejador);

            IReadOnlyList<float[]> vectores = await servicio.CalcularAsync(new[] { "uno", "dos" }, CancellationToken.None);

            Assert.AreEqual(2, vectores.Count);
            Assert.AreEqual(768, vectores[0].Length);
            Assert.AreEqual(0.5f, vectores[0][0]);
            Assert.AreEqual(1.5f, vectores[1][0]);
            Assert.AreEqual("gemini-embedding-2", servicio.Modelo);
        }

        [TestMethod]
        public async Task CalcularAsync_MasTextosQueElLote_LosPartaEnVariosPedidos()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo =>
            {
                using (JsonDocument documento = JsonDocument.Parse(cuerpo))
                {
                    return Respuesta(Vectores(documento.RootElement.GetProperty("requests").GetArrayLength()));
                }
            });
            GeminiServicioEmbeddings servicio = Crear(manejador);
            string[] textos = Enumerable.Range(0, GeminiServicioEmbeddings.TextosPorLote + 5).Select(i => "texto " + i).ToArray();

            IReadOnlyList<float[]> vectores = await servicio.CalcularAsync(textos, CancellationToken.None);

            Assert.AreEqual(textos.Length, vectores.Count);
            Assert.AreEqual(2, manejador.Peticiones.Count);
        }

        [TestMethod]
        public async Task CalcularAsync_ListaVacia_NoHacePedidos()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta(Vectores(0)));
            GeminiServicioEmbeddings servicio = Crear(manejador);

            IReadOnlyList<float[]> vectores = await servicio.CalcularAsync(new string[0], CancellationToken.None);

            Assert.AreEqual(0, vectores.Count);
            Assert.AreEqual(0, manejador.Peticiones.Count);
        }

        [TestMethod]
        public async Task CalcularAsync_ErrorHttp_LanzaExcepcionTecnica()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta("{\"error\":{\"code\":429}}", (HttpStatusCode)429));
            GeminiServicioEmbeddings servicio = Crear(manejador);

            ExcepcionTecnica ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(
                () => servicio.CalcularAsync(new[] { "uno" }, CancellationToken.None));

            Assert.AreEqual("Error.Assistant.ResponseFailed", ex.Message);
        }

        [TestMethod]
        public async Task CalcularAsync_CantidadDeVectoresDistinta_LanzaExcepcionTecnica()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta(Vectores(1)));
            GeminiServicioEmbeddings servicio = Crear(manejador);

            await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(
                () => servicio.CalcularAsync(new[] { "uno", "dos" }, CancellationToken.None));
        }

        [TestMethod]
        public async Task CalcularAsync_DimensionInesperada_LanzaExcepcionTecnica()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta("{\"embeddings\":[{\"values\":[0.1,0.2]}]}"));
            GeminiServicioEmbeddings servicio = Crear(manejador);

            await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(
                () => servicio.CalcularAsync(new[] { "uno" }, CancellationToken.None));
        }

        [TestMethod]
        public async Task CalcularAsync_CuerpoNoJson_LanzaExcepcionTecnica()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta("<html>proxy</html>"));
            GeminiServicioEmbeddings servicio = Crear(manejador);

            await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(
                () => servicio.CalcularAsync(new[] { "uno" }, CancellationToken.None));
        }

        [TestMethod]
        public async Task CalcularAsync_TiempoAgotado_LanzaExcepcionTecnica()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(async (cuerpo, ct) =>
            {
                await Task.Delay(Timeout.Infinite, ct).ConfigureAwait(false);
                return Respuesta(Vectores(1));
            });
            GeminiServicioEmbeddings servicio = Crear(manejador, tiempoMaximo: TimeSpan.FromMilliseconds(50));

            ExcepcionTecnica ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(
                () => servicio.CalcularAsync(new[] { "uno" }, CancellationToken.None));

            Assert.IsInstanceOfType(ex.InnerException, typeof(OperationCanceledException));
        }

        [TestMethod]
        public async Task CalcularAsync_CanceladoPorElLlamador_PropagaLaCancelacion()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(async (cuerpo, ct) =>
            {
                await Task.Delay(Timeout.Infinite, ct).ConfigureAwait(false);
                return Respuesta(Vectores(1));
            });
            GeminiServicioEmbeddings servicio = Crear(manejador, tiempoMaximo: TimeSpan.FromSeconds(30));

            using (CancellationTokenSource cancelacion = new CancellationTokenSource(TimeSpan.FromMilliseconds(50)))
            {
                try
                {
                    await servicio.CalcularAsync(new[] { "uno" }, cancelacion.Token);
                    Assert.Fail("Se esperaba la cancelacion.");
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        [TestMethod]
        public async Task CalcularAsync_SinClave_LanzaApiKeyMissingSinPedir()
        {
            ManejadorEmbeddingsFalso manejador = new ManejadorEmbeddingsFalso(cuerpo => Respuesta(Vectores(1)));
            GeminiServicioEmbeddings servicio = Crear(manejador, clave: " ");

            ExcepcionTecnica ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(
                () => servicio.CalcularAsync(new[] { "uno" }, CancellationToken.None));

            Assert.AreEqual("Error.Assistant.ApiKeyMissing", ex.Message);
            Assert.AreEqual(0, manejador.Peticiones.Count);
        }

        private static GeminiServicioEmbeddings Crear(ManejadorEmbeddingsFalso manejador, string clave = Clave, TimeSpan? tiempoMaximo = null)
        {
            return new GeminiServicioEmbeddings(manejador, () => clave, tiempoMaximo ?? TimeSpan.FromSeconds(5));
        }

        private static string Vectores(int cantidad)
        {
            StringBuilder json = new StringBuilder("{\"embeddings\":[");
            for (int i = 0; i < cantidad; i++)
            {
                if (i > 0)
                {
                    json.Append(',');
                }

                float primero = 0.5f + i;
                json.Append("{\"values\":[");
                json.Append(primero.ToString(System.Globalization.CultureInfo.InvariantCulture));
                json.Append(string.Concat(Enumerable.Repeat(",0.01", 767)));
                json.Append("]}");
            }

            json.Append("],\"usageMetadata\":{\"promptTokenCount\":3}}");
            return json.ToString();
        }

        private static HttpResponseMessage Respuesta(string cuerpo, HttpStatusCode codigo = HttpStatusCode.OK)
        {
            return new HttpResponseMessage(codigo) { Content = new StringContent(cuerpo, Encoding.UTF8, "application/json") };
        }

        private sealed class PeticionEmbeddings
        {
            public Uri Uri;
            public HttpMethod Metodo;
            public string ClaveApi;
            public string Cuerpo;
        }

        // Captura metodo, URL, clave y cuerpo de cada pedido antes de responder.
        private sealed class ManejadorEmbeddingsFalso : HttpMessageHandler
        {
            private readonly Func<string, CancellationToken, Task<HttpResponseMessage>> _responder;

            public ManejadorEmbeddingsFalso(Func<string, HttpResponseMessage> responder)
                : this((cuerpo, ct) => Task.FromResult(responder(cuerpo)))
            {
            }

            public ManejadorEmbeddingsFalso(Func<string, CancellationToken, Task<HttpResponseMessage>> responder)
            {
                _responder = responder;
            }

            public List<PeticionEmbeddings> Peticiones { get; } = new List<PeticionEmbeddings>();

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                string cuerpo = request.Content == null ? null : await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                IEnumerable<string> claves;
                Peticiones.Add(new PeticionEmbeddings
                {
                    Uri = request.RequestUri,
                    Metodo = request.Method,
                    ClaveApi = request.Headers.TryGetValues("x-goog-api-key", out claves) ? claves.FirstOrDefault() : null,
                    Cuerpo = cuerpo
                });

                return await _responder(cuerpo, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
