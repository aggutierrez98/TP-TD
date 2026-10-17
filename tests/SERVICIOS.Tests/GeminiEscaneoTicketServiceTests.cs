using System;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class GeminiEscaneoTicketServiceTests
    {
        [TestMethod]
        public void ArmarRequestBody_ImagenDeTicket_UsaSalidaEstructuradaActualDeGemini()
        {
            string json = GeminiEscaneoTicketService.ArmarRequestBody(new byte[] { 1, 2, 3 });
            using (JsonDocument request = JsonDocument.Parse(json))
            {
                JsonElement text = request.RootElement.GetProperty("generationConfig")
                    .GetProperty("responseFormat").GetProperty("text");
                JsonElement schema = text.GetProperty("schema");

                Assert.AreEqual("APPLICATION_JSON", text.GetProperty("mimeType").GetString());
                Assert.AreEqual("object", schema.GetProperty("type").GetString());
            }
        }

        [TestMethod]
        public void ExtraerTicket_RespuestaValidaCompleta_MapeaTodosLosCampos()
        {
            string respuesta = ArmarRespuestaGemini(
                "{\"monto\": 1234.56, \"fecha\": \"2026-03-15\", \"descripcion\": \"Supermercado Dia\", \"textoDetectado\": \"TOTAL 1234,56\"}");

            TicketEscaneado ticket = GeminiEscaneoTicketService.ExtraerTicket(respuesta);

            Assert.AreEqual(1234.56m, ticket.Monto);
            Assert.AreEqual(new DateTime(2026, 3, 15), ticket.Fecha);
            Assert.AreEqual("Supermercado Dia", ticket.Descripcion);
            Assert.AreEqual("TOTAL 1234,56", ticket.TextoCrudo);
        }

        [TestMethod]
        public void ExtraerTicket_MontoNull_DejaMontoNulo()
        {
            string respuesta = ArmarRespuestaGemini(
                "{\"monto\": null, \"fecha\": \"2026-03-15\", \"descripcion\": \"Kiosco\", \"textoDetectado\": \"texto\"}");

            TicketEscaneado ticket = GeminiEscaneoTicketService.ExtraerTicket(respuesta);

            Assert.IsNull(ticket.Monto);
            Assert.AreEqual(new DateTime(2026, 3, 15), ticket.Fecha);
        }

        [TestMethod]
        public void ExtraerTicket_MontoAusente_DejaMontoNulo()
        {
            string respuesta = ArmarRespuestaGemini(
                "{\"fecha\": \"2026-03-15\", \"descripcion\": \"Kiosco\", \"textoDetectado\": \"texto\"}");

            TicketEscaneado ticket = GeminiEscaneoTicketService.ExtraerTicket(respuesta);

            Assert.IsNull(ticket.Monto);
            Assert.AreEqual("Kiosco", ticket.Descripcion);
        }

        [TestMethod]
        public void ExtraerTicket_FechaInvalida_DejaFechaNula()
        {
            string respuesta = ArmarRespuestaGemini(
                "{\"monto\": 10, \"fecha\": \"no-es-una-fecha\", \"descripcion\": \"Kiosco\", \"textoDetectado\": \"texto\"}");

            TicketEscaneado ticket = GeminiEscaneoTicketService.ExtraerTicket(respuesta);

            Assert.IsNull(ticket.Fecha);
            Assert.AreEqual(10m, ticket.Monto);
        }

        [TestMethod]
        public void ExtraerTicket_FechaNullOAusente_DejaFechaNula()
        {
            TicketEscaneado conNull = GeminiEscaneoTicketService.ExtraerTicket(ArmarRespuestaGemini(
                "{\"monto\": 10, \"fecha\": null, \"descripcion\": \"Kiosco\", \"textoDetectado\": \"texto\"}"));
            TicketEscaneado ausente = GeminiEscaneoTicketService.ExtraerTicket(ArmarRespuestaGemini(
                "{\"monto\": 10, \"descripcion\": \"Kiosco\", \"textoDetectado\": \"texto\"}"));

            Assert.IsNull(conNull.Fecha);
            Assert.IsNull(ausente.Fecha);
        }

        [TestMethod]
        public void ExtraerTicket_TextoDetectadoAusente_DevuelveTextoCrudoVacio()
        {
            TicketEscaneado ticket = GeminiEscaneoTicketService.ExtraerTicket(ArmarRespuestaGemini(
                "{\"monto\": 10, \"fecha\": \"2026-03-15\", \"descripcion\": \"Kiosco\"}"));

            Assert.AreEqual(string.Empty, ticket.TextoCrudo);
        }

        [TestMethod]
        public void ExtraerTicket_SinCandidates_LanzaExcepcionTecnica()
        {
            AssertScanFailed("{}");
            AssertScanFailed("{\"candidates\": []}");
        }

        [TestMethod]
        public void ExtraerTicket_CandidateSinPartsOPartsVacio_LanzaExcepcionTecnica()
        {
            AssertScanFailed("{\"candidates\": [{\"content\": {}}]}");
            AssertScanFailed("{\"candidates\": [{\"content\": {\"parts\": []}}]}");
            AssertScanFailed("{\"candidates\": [{\"finishReason\": \"SAFETY\"}]}");
        }

        [TestMethod]
        public void ExtraerTicket_TextoInternoVacio_LanzaExcepcionTecnica()
        {
            AssertScanFailed(ArmarRespuestaGemini("   "));
        }

        [TestMethod]
        public void ExtraerTicket_JsonInternoMalFormado_LanzaExcepcionTecnicaConCausa()
        {
            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(
                () => GeminiEscaneoTicketService.ExtraerTicket(ArmarRespuestaGemini("{\"monto\": 12,, esto no es json")));

            Assert.AreEqual("Error.Ticket.ScanFailed", ex.Message);
            Assert.IsNotNull(ex.InnerException);
        }

        [TestMethod]
        public void ExtraerTicket_CaracteresEspecialesYAcentos_SePreservan()
        {
            string descripcion = "Panadería \"La Ñata\" & Cía. — café con leche";
            string textoDetectado = "Línea 1\nLínea 2\t€ 5,50 \\ / ¿Qué tal? 日本";
            string interno = JsonSerializer.Serialize(new
            {
                monto = 5.5m,
                fecha = "2026-01-02",
                descripcion,
                textoDetectado
            });

            TicketEscaneado ticket = GeminiEscaneoTicketService.ExtraerTicket(ArmarRespuestaGemini(interno));

            Assert.AreEqual(descripcion, ticket.Descripcion);
            Assert.AreEqual(textoDetectado, ticket.TextoCrudo);
            Assert.AreEqual(5.5m, ticket.Monto);
        }

        // Reproduce la forma real de la API: candidates[0].content.parts[0].text es un
        // string que contiene el JSON estructurado (serializado como string escapado).
        private static string ArmarRespuestaGemini(string textoJsonInterno)
        {
            return JsonSerializer.Serialize(new
            {
                candidates = new[]
                {
                    new
                    {
                        content = new
                        {
                            parts = new[] { new { text = textoJsonInterno } },
                            role = "model"
                        },
                        finishReason = "STOP",
                        index = 0
                    }
                },
                usageMetadata = new { promptTokenCount = 100, totalTokenCount = 150 },
                modelVersion = "gemini-3.1-flash-lite"
            });
        }

        private static void AssertScanFailed(string jsonRespuesta)
        {
            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(
                () => GeminiEscaneoTicketService.ExtraerTicket(jsonRespuesta));

            Assert.AreEqual("Error.Ticket.ScanFailed", ex.Message);
        }
    }
}
