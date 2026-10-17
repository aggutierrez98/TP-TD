using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class GeminiChatClientTests
    {
        [TestMethod]
        public void ExtraerTexto_RespuestaValida_DevuelveElTextoDelPrimerPart()
        {
            string texto = GeminiChatClient.ExtraerTexto(ArmarRespuestaGemini("Hola, ¿cómo ahorrar más? Probá con el 50/30/20 — \"regla\" útil."));

            Assert.AreEqual("Hola, ¿cómo ahorrar más? Probá con el 50/30/20 — \"regla\" útil.", texto);
        }

        [TestMethod]
        public void ExtraerTexto_SinCandidatesOPartsVacios_LanzaExcepcionTecnica()
        {
            foreach (string json in new[]
            {
                "{}",
                "{\"candidates\": []}",
                "{\"candidates\": [{\"content\": {}}]}",
                "{\"candidates\": [{\"content\": {\"parts\": []}}]}",
                ArmarRespuestaGemini("  ")
            })
            {
                ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => GeminiChatClient.ExtraerTexto(json));
                Assert.AreEqual("Error.Assistant.ResponseFailed", ex.Message);
            }
        }

        [TestMethod]
        public void ExtraerTextoCrudo_SinCandidatesOPartsVacios_DevuelveNullSinLanzar()
        {
            Assert.IsNull(GeminiChatClient.ExtraerTextoCrudo("{}"));
            Assert.IsNull(GeminiChatClient.ExtraerTextoCrudo("{\"candidates\": []}"));
            Assert.IsNull(GeminiChatClient.ExtraerTextoCrudo("{\"candidates\": [{\"content\": {\"parts\": []}}]}"));
        }

        [TestMethod]
        public void ExtraerTextoCrudo_RespuestaDeClasificacion_DevuelveElJsonInternoComoString()
        {
            string crudo = GeminiChatClient.ExtraerTextoCrudo(ArmarRespuestaGemini("{\"agente\": \"Ahorro\"}"));

            Assert.AreEqual("{\"agente\": \"Ahorro\"}", crudo);
        }

        private static string ArmarRespuestaGemini(string texto)
        {
            return JsonSerializer.Serialize(new
            {
                candidates = new[]
                {
                    new
                    {
                        content = new
                        {
                            parts = new[] { new { text = texto } },
                            role = "model"
                        },
                        finishReason = "STOP"
                    }
                }
            });
        }
    }
}
