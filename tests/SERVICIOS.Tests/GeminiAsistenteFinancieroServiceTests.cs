using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
            var serializer = new JavaScriptSerializer();
            var request = (Dictionary<string, object>)serializer.DeserializeObject(json);
            var contents = (object[])request["contents"];

            Assert.AreEqual(1, contents.Length);
            var primero = (Dictionary<string, object>)contents[0];
            Assert.AreEqual("user", primero["role"]);
        }

        [TestMethod]
        public void ArmarRequestBody_ConHistorial_IncluyeTurnosPreviosComoUserYModel()
        {
            List<IntercambioChatbot> historial = new List<IntercambioChatbot>
            {
                new IntercambioChatbot { Pregunta = "Hola", Respuesta = "Hola, como puedo ayudarte?" }
            };

            string json = GeminiAsistenteFinancieroService.ArmarRequestBody("Y ahora?", historial);
            var serializer = new JavaScriptSerializer();
            var request = (Dictionary<string, object>)serializer.DeserializeObject(json);
            var contents = (object[])request["contents"];

            Assert.AreEqual(3, contents.Length);
            var primero = (Dictionary<string, object>)contents[0];
            var segundo = (Dictionary<string, object>)contents[1];
            var tercero = (Dictionary<string, object>)contents[2];
            Assert.AreEqual("user", primero["role"]);
            Assert.AreEqual("model", segundo["role"]);
            Assert.AreEqual("user", tercero["role"]);
        }

        [TestMethod]
        public void ArmarRequestBody_IncluyeSystemInstruction()
        {
            string json = GeminiAsistenteFinancieroService.ArmarRequestBody("Hola", null);
            var serializer = new JavaScriptSerializer();
            var request = (Dictionary<string, object>)serializer.DeserializeObject(json);

            Assert.IsTrue(request.ContainsKey("systemInstruction"));
        }

        [TestMethod]
        public void ArmarRequestBodyClasificacion_IncluyeLosTresAgentesPosiblesYLaPregunta()
        {
            string json = GeminiAsistenteFinancieroService.ArmarRequestBodyClasificacion("Como invierto mis ahorros?");

            var serializer = new JavaScriptSerializer();
            var request = (Dictionary<string, object>)serializer.DeserializeObject(json);
            var contents = (object[])request["contents"];
            var primerContenido = (Dictionary<string, object>)contents[0];
            var partes = (object[])primerContenido["parts"];
            var primeraParte = (Dictionary<string, object>)partes[0];
            string texto = (string)primeraParte["text"];

            var generationConfig = (Dictionary<string, object>)request["generationConfig"];
            var responseFormat = (Dictionary<string, object>)generationConfig["responseFormat"];
            var textFormat = (Dictionary<string, object>)responseFormat["text"];
            var schema = (Dictionary<string, object>)textFormat["schema"];
            var properties = (Dictionary<string, object>)schema["properties"];
            var agente = (Dictionary<string, object>)properties["agente"];
            var valoresPosibles = (object[])agente["enum"];

            StringAssert.Contains(texto, "Como invierto mis ahorros?");
            CollectionAssert.AreEquivalent(new[] { "Ahorro", "Inversiones", "General" }, valoresPosibles);
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
