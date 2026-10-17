using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class GeminiAsistenteFinancieroServiceTests
    {
        [TestMethod]
        public void ArmarRequestBody_SinHistorial_IncluyeSoloLaPreguntaActualComoUser()
        {
            string json = GeminiAsistenteFinancieroService.ArmarRequestBody("Como ahorro mejor?", null);
            using (JsonDocument request = JsonDocument.Parse(json))
            {
                JsonElement contents = request.RootElement.GetProperty("contents");

                Assert.AreEqual(1, contents.GetArrayLength());
                Assert.AreEqual("user", contents[0].GetProperty("role").GetString());
            }
        }

        [TestMethod]
        public void ArmarRequestBody_ConHistorial_IncluyeTurnosPreviosComoUserYModel()
        {
            List<IntercambioChatbot> historial = new List<IntercambioChatbot>
            {
                new IntercambioChatbot { Pregunta = "Hola", Respuesta = "Hola, como puedo ayudarte?" }
            };

            string json = GeminiAsistenteFinancieroService.ArmarRequestBody("Y ahora?", historial);
            using (JsonDocument request = JsonDocument.Parse(json))
            {
                JsonElement contents = request.RootElement.GetProperty("contents");

                Assert.AreEqual(3, contents.GetArrayLength());
                Assert.AreEqual("user", contents[0].GetProperty("role").GetString());
                Assert.AreEqual("model", contents[1].GetProperty("role").GetString());
                Assert.AreEqual("user", contents[2].GetProperty("role").GetString());
            }
        }

        [TestMethod]
        public void ArmarRequestBody_IncluyeSystemInstruction()
        {
            string json = GeminiAsistenteFinancieroService.ArmarRequestBody("Hola", null);
            using (JsonDocument request = JsonDocument.Parse(json))
            {
                Assert.IsTrue(request.RootElement.TryGetProperty("systemInstruction", out _));
            }
        }

        [TestMethod]
        public void ArmarRequestBodyClasificacion_IncluyeLosTresAgentesPosiblesYLaPregunta()
        {
            string json = GeminiAsistenteFinancieroService.ArmarRequestBodyClasificacion("Como invierto mis ahorros?");

            using (JsonDocument request = JsonDocument.Parse(json))
            {
                JsonElement raiz = request.RootElement;
                string texto = raiz.GetProperty("contents")[0].GetProperty("parts")[0]
                    .GetProperty("text").GetString();
                JsonElement agente = raiz.GetProperty("generationConfig").GetProperty("responseFormat")
                    .GetProperty("text").GetProperty("schema").GetProperty("properties").GetProperty("agente");
                string[] valoresPosibles = agente.GetProperty("enum").EnumerateArray()
                    .Select(valor => valor.GetString()).ToArray();

                StringAssert.Contains(texto, "Como invierto mis ahorros?");
                CollectionAssert.AreEquivalent(new[] { "Ahorro", "Inversiones", "General" }, valoresPosibles);
            }
        }

        [TestMethod]
        public void ExtraerAgenteClasificado_ConAhorro_DevuelveAhorro()
        {
            string textoJsonInterno = "{\"agente\":\"Ahorro\"}";

            string resultado = GeminiAsistenteFinancieroService.ExtraerAgenteClasificado(textoJsonInterno);

            Assert.AreEqual("Ahorro", resultado);
        }

        [TestMethod]
        public void ExtraerAgenteClasificado_ConInversiones_DevuelveInversiones()
        {
            string textoJsonInterno = "{\"agente\":\"Inversiones\"}";

            string resultado = GeminiAsistenteFinancieroService.ExtraerAgenteClasificado(textoJsonInterno);

            Assert.AreEqual("Inversiones", resultado);
        }

        [TestMethod]
        public void ExtraerAgenteClasificado_ConValorDesconocido_DevuelveGeneral()
        {
            string textoJsonInterno = "{\"agente\":\"OtraCosa\"}";

            string resultado = GeminiAsistenteFinancieroService.ExtraerAgenteClasificado(textoJsonInterno);

            Assert.AreEqual("General", resultado);
        }

        [TestMethod]
        public void ExtraerAgenteClasificado_ConJsonInvalido_DevuelveGeneralSinLanzar()
        {
            string resultado = GeminiAsistenteFinancieroService.ExtraerAgenteClasificado("esto no es json");

            Assert.AreEqual("General", resultado);
        }

        [TestMethod]
        public void ExtraerAgenteClasificado_ConTextoVacio_DevuelveGeneral()
        {
            string resultado = GeminiAsistenteFinancieroService.ExtraerAgenteClasificado(string.Empty);

            Assert.AreEqual("General", resultado);
        }

        [TestMethod]
        public async Task DelegarSegunClasificacionAsync_ClasificadoComoAhorro_DelegaEnAgenteAhorro()
        {
            AgenteIAFake agenteAhorroFake = new AgenteIAFake("Ahorro", "Respuesta del agente de ahorro");
            AgenteIAFake agenteInversionesFake = new AgenteIAFake("Inversiones", "Respuesta del agente de inversiones");
            GeminiAsistenteFinancieroService servicio = new GeminiAsistenteFinancieroService(agenteAhorroFake, agenteInversionesFake);

            string respuesta = await servicio.DelegarSegunClasificacionAsync("Ahorro", "Como ahorro?", null, "api-key", "modelo");

            Assert.AreEqual("Respuesta del agente de ahorro", respuesta);
            Assert.IsTrue(agenteAhorroFake.FueLlamado);
            Assert.IsFalse(agenteInversionesFake.FueLlamado);
        }

        [TestMethod]
        public async Task DelegarSegunClasificacionAsync_ClasificadoComoInversiones_DelegaEnAgenteInversiones()
        {
            AgenteIAFake agenteAhorroFake = new AgenteIAFake("Ahorro", "Respuesta del agente de ahorro");
            AgenteIAFake agenteInversionesFake = new AgenteIAFake("Inversiones", "Respuesta del agente de inversiones");
            GeminiAsistenteFinancieroService servicio = new GeminiAsistenteFinancieroService(agenteAhorroFake, agenteInversionesFake);

            string respuesta = await servicio.DelegarSegunClasificacionAsync("Inversiones", "Como invierto?", null, "api-key", "modelo");

            Assert.AreEqual("Respuesta del agente de inversiones", respuesta);
            Assert.IsTrue(agenteInversionesFake.FueLlamado);
            Assert.IsFalse(agenteAhorroFake.FueLlamado);
        }

        // Doble de test para AgenteIA: evita pegarle a la red real de Gemini al verificar
        // la delegacion de GeminiAsistenteFinancieroService (AgenteGeneral) hacia sus
        // subclases especialistas.
        private sealed class AgenteIAFake : AgenteIA
        {
            private readonly string _respuestaFija;

            internal AgenteIAFake(string tipo, string respuestaFija)
            {
                Tipo = tipo;
                _respuestaFija = respuestaFija;
            }

            internal override string Tipo { get; }

            protected override string PromptSistema => "Fake";

            internal bool FueLlamado { get; private set; }

            internal override Task<string> ResponderAsync(string pregunta, IEnumerable<IntercambioChatbot> historial)
            {
                FueLlamado = true;
                return Task.FromResult(_respuestaFija);
            }
        }
    }
}
