using System.Globalization;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ExtractorPorcentajeInversionTests
    {
        private const string Herramienta = "recomendar_inversion";

        [TestMethod]
        public void Extraer_SinLlamadaOConOtraHerramienta_EsAusente()
        {
            Assert.AreEqual(EstadoPorcentaje.Ausente, ExtractorPorcentajeInversion.Extraer(null, Herramienta).Estado);
            Assert.AreEqual(EstadoPorcentaje.Ausente, ExtractorPorcentajeInversion.Extraer(new LlamadaHerramientaAsistente("recomendar_compras", "{\"porcentaje\":20}"), Herramienta).Estado);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("{}")]
        [DataRow("{\"porcentaje\":null}")]
        [DataRow("{\"otro\":5}")]
        public void Extraer_PorcentajeNoIndicado_EsAusenteSinValor(string argumentos)
        {
            ResultadoPorcentaje r = ExtractorPorcentajeInversion.Extraer(new LlamadaHerramientaAsistente(Herramienta, argumentos), Herramienta);
            Assert.AreEqual(EstadoPorcentaje.Ausente, r.Estado);
            Assert.IsNull(r.Valor);
        }

        [TestMethod]
        [DataRow("20", "20")]
        [DataRow("100", "100")]
        [DataRow("0.5", "0.5")]
        [DataRow("99.99", "99.99")]
        public void Extraer_PorcentajeEntreCeroExcluidoYCien_EsValidoConSuValor(string texto, string esperado)
        {
            ResultadoPorcentaje r = ExtractorPorcentajeInversion.Extraer(new LlamadaHerramientaAsistente(Herramienta, "{\"porcentaje\":" + texto + "}"), Herramienta);
            Assert.AreEqual(EstadoPorcentaje.Valido, r.Estado);
            Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), r.Valor);
        }

        [TestMethod]
        [DataRow("0")]
        [DataRow("-5")]
        [DataRow("-0.01")]
        [DataRow("100.01")]
        [DataRow("101")]
        [DataRow("150")]
        [DataRow("1e400")]
        [DataRow("\"20\"")]
        [DataRow("true")]
        public void Extraer_PorcentajeExplicitoFueraDeRangoOSinFormaNumerica_EsInvalidoSinValor(string texto)
        {
            ResultadoPorcentaje r = ExtractorPorcentajeInversion.Extraer(new LlamadaHerramientaAsistente(Herramienta, "{\"porcentaje\":" + texto + "}"), Herramienta);
            Assert.AreEqual(EstadoPorcentaje.Invalido, r.Estado);
            Assert.IsNull(r.Valor);
        }

        [TestMethod]
        [DataRow("no-json")]
        [DataRow("[]")]
        [DataRow("20")]
        public void Extraer_ArgumentosIlegibles_EsErrorTecnicoYNoSeColapsaAAusente(string argumentos)
        {
            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => ExtractorPorcentajeInversion.Extraer(new LlamadaHerramientaAsistente(Herramienta, argumentos), Herramienta));
            Assert.AreEqual("Error.Assistant.ResponseFailed", ex.Message);
        }
    }
}
