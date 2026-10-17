using System.Collections.Generic;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var serializer = new JavaScriptSerializer();
            var request = (Dictionary<string, object>)serializer.DeserializeObject(json);
            var generationConfig = (Dictionary<string, object>)request["generationConfig"];
            var responseFormat = (Dictionary<string, object>)generationConfig["responseFormat"];
            var text = (Dictionary<string, object>)responseFormat["text"];
            var schema = (Dictionary<string, object>)text["schema"];

            Assert.AreEqual("APPLICATION_JSON", text["mimeType"]);
            Assert.AreEqual("object", schema["type"]);
        }
    }
}
