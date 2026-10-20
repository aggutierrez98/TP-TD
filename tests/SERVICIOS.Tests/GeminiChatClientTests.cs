using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void ArmarRequestBody_SinOpciones_NoEnviaGenerationConfigNiTools()
        {
            string json = GeminiChatClient.ArmarRequestBody("prompt", "pregunta", null, null);

            using (JsonDocument request = JsonDocument.Parse(json))
            {
                Assert.IsFalse(request.RootElement.TryGetProperty("generationConfig", out _));
                Assert.IsFalse(request.RootElement.TryGetProperty("tools", out _));
                Assert.IsFalse(request.RootElement.TryGetProperty("toolConfig", out _));
            }
        }

        [TestMethod]
        public void ArmarRequestBody_ConTemperatura_LaEnviaEnGenerationConfig()
        {
            string json = GeminiChatClient.ArmarRequestBody("prompt", "pregunta", null, new OpcionesGeneracionGemini { Temperatura = 0.2 });

            using (JsonDocument request = JsonDocument.Parse(json))
            {
                Assert.AreEqual(0.2, request.RootElement.GetProperty("generationConfig").GetProperty("temperature").GetDouble(), 0.0001);
            }
        }

        [TestMethod]
        public void ArmarRequestBody_ConFunciones_EnviaDeclaracionesYModoAnyConNombresPermitidos()
        {
            OpcionesGeneracionGemini opciones = new OpcionesGeneracionGemini
            {
                Funciones = new[] { CrearDeclaracionLista(), new DeclaracionFuncionGemini("otra_funcion", "Otra.", null) },
                FuncionesPermitidas = new[] { "recomendar_compras" }
            };

            string json = GeminiChatClient.ArmarRequestBody("prompt", "pregunta", null, opciones);

            using (JsonDocument request = JsonDocument.Parse(json))
            {
                JsonElement declaraciones = request.RootElement.GetProperty("tools")[0].GetProperty("functionDeclarations");
                Assert.AreEqual(2, declaraciones.GetArrayLength());
                Assert.AreEqual("recomendar_compras", declaraciones[0].GetProperty("name").GetString());
                Assert.AreEqual("Recomienda compras.", declaraciones[0].GetProperty("description").GetString());
                Assert.AreEqual("string", declaraciones[0].GetProperty("parameters").GetProperty("properties")
                    .GetProperty("nombreLista").GetProperty("type").GetString());
                Assert.IsFalse(declaraciones[1].TryGetProperty("parameters", out _));

                JsonElement configuracion = request.RootElement.GetProperty("toolConfig").GetProperty("functionCallingConfig");
                Assert.AreEqual("ANY", configuracion.GetProperty("mode").GetString());
                CollectionAssert.AreEqual(
                    new[] { "recomendar_compras" },
                    configuracion.GetProperty("allowedFunctionNames").EnumerateArray().Select(nombre => nombre.GetString()).ToArray());
            }
        }

        [TestMethod]
        public void ArmarRequestBody_ConFuncionesSinNombresPermitidos_PermiteTodasLasDeclaradas()
        {
            OpcionesGeneracionGemini opciones = new OpcionesGeneracionGemini { Funciones = new[] { CrearDeclaracionLista() } };

            string json = GeminiChatClient.ArmarRequestBody("prompt", "pregunta", null, opciones);

            using (JsonDocument request = JsonDocument.Parse(json))
            {
                JsonElement permitidas = request.RootElement.GetProperty("toolConfig").GetProperty("functionCallingConfig")
                    .GetProperty("allowedFunctionNames");
                Assert.AreEqual("recomendar_compras", permitidas.EnumerateArray().Single().GetString());
            }
        }

        [TestMethod]
        public void ArmarRequestBody_NombrePermitidoNoDeclarado_LanzaArgumentException()
        {
            OpcionesGeneracionGemini opciones = new OpcionesGeneracionGemini
            {
                Funciones = new[] { CrearDeclaracionLista() },
                FuncionesPermitidas = new[] { "borrar_todo" }
            };

            Assert.ThrowsExactly<ArgumentException>(() => GeminiChatClient.ArmarRequestBody("prompt", "pregunta", null, opciones));
        }

        [TestMethod]
        public void DeclaracionFuncionGemini_ParametroConIdentificadorDeUsuario_LanzaArgumentException()
        {
            foreach (string nombreParametro in new[] { "usuarioId", "UserId", "user_id", "idUsuario" })
            {
                Dictionary<string, object> propiedades = new Dictionary<string, object>
                {
                    { "filtros", new { type = "object", properties = new Dictionary<string, object> { { nombreParametro, new { type = "integer" } } } } }
                };

                Assert.ThrowsExactly<ArgumentException>(
                    () => new DeclaracionFuncionGemini("consultar", "Consulta.", new { type = "object", properties = propiedades }),
                    nombreParametro);
            }
        }

        [TestMethod]
        public void ExtraerLlamadasFuncion_RespuestaConFunctionCall_DevuelveNombreArgumentosEId()
        {
            string json = "{\"candidates\":[{\"content\":{\"role\":\"model\",\"parts\":[" +
                "{\"functionCall\":{\"id\":\"c1\",\"name\":\"recomendar_compras\",\"args\":{\"nombreLista\":\"Super\"}}}," +
                "{\"functionCall\":{\"name\":\"otra_funcion\"}}]}}]}";

            IReadOnlyList<LlamadaFuncionGemini> llamadas = GeminiChatClient.ExtraerLlamadasFuncion(json);

            Assert.AreEqual(2, llamadas.Count);
            Assert.AreEqual("c1", llamadas[0].Id);
            Assert.AreEqual("recomendar_compras", llamadas[0].Nombre);
            using (JsonDocument argumentos = JsonDocument.Parse(llamadas[0].ArgumentosJson))
            {
                Assert.AreEqual("Super", argumentos.RootElement.GetProperty("nombreLista").GetString());
            }

            Assert.IsNull(llamadas[1].Id);
            Assert.AreEqual("otra_funcion", llamadas[1].Nombre);
            Assert.AreEqual("{}", llamadas[1].ArgumentosJson);
        }

        [TestMethod]
        public void ExtraerLlamadasFuncion_RespuestaSoloTextoOSinCandidates_DevuelveListaVacia()
        {
            Assert.AreEqual(0, GeminiChatClient.ExtraerLlamadasFuncion(ArmarRespuestaGemini("hola")).Count);
            Assert.AreEqual(0, GeminiChatClient.ExtraerLlamadasFuncion("{\"candidates\": []}").Count);
        }

        [TestMethod]
        public async Task ObtenerLlamadasFuncionAsync_PosteaElRequestYDevuelveLasLlamadas()
        {
            string respuesta = "{\"candidates\":[{\"content\":{\"parts\":[{\"functionCall\":{\"name\":\"recomendar_compras\",\"args\":{}}}]}}]}";
            ManejadorGeminiFalso manejador = new ManejadorGeminiFalso(System.Net.HttpStatusCode.OK, respuesta);
            OpcionesGeneracionGemini opciones = new OpcionesGeneracionGemini { Funciones = new[] { CrearDeclaracionLista() } };

            IReadOnlyList<LlamadaFuncionGemini> llamadas = await GeminiChatClient.ObtenerLlamadasFuncionAsync(
                "prompt", "pregunta", null, opciones, "clave", "modelo", manejador, CancellationToken.None);

            Assert.AreEqual("recomendar_compras", llamadas.Single().Nombre);
            using (JsonDocument enviado = JsonDocument.Parse(manejador.Cuerpos.Single()))
            {
                Assert.AreEqual("ANY", enviado.RootElement.GetProperty("toolConfig").GetProperty("functionCallingConfig").GetProperty("mode").GetString());
            }
        }

        private static DeclaracionFuncionGemini CrearDeclaracionLista()
        {
            return new DeclaracionFuncionGemini(
                "recomendar_compras",
                "Recomienda compras.",
                new
                {
                    type = "object",
                    properties = new { nombreLista = new { type = "string", description = "Nombre de la lista." } }
                });
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
