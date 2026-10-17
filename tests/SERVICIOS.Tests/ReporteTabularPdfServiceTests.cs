using System;
using System.IO;
using System.Linq;
using BE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;
using PdfReader = UglyToad.PdfPig.PdfDocument;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class ReporteTabularPdfServiceTests
    {
        [TestMethod]
        public void Generar_SubconjuntoConAcentos_ConservaColumnasFiltrosYValores()
        {
            byte[] bytes = new ReporteTabularPdfService().Generar(CrearReporte(new[] {
                new[] { "14/10/2026", "Egreso", "Alimentación", "Café", "$ 20,00", "Compra de café" },
                new[] { "13/10/2026", "Ingreso", "Otros", "", "$ 10,00", "Reintegro" }
            }));
            using (var pdf = PdfReader.Open(bytes))
            {
                var page = pdf.GetPage(1);
                string texto = ContentOrderTextExtractor.GetText(page);
                foreach (string esperado in new[] { "Fecha", "Tipo", "Categoría", "Comercio", "Monto", "Descripción",
                    "Listado de movimientos", "Descripción: café", "Alimentación", "Café", "Reintegro", "Cantidad: 2" })
                    Assert.Contains(esperado, texto);
                Assert.IsTrue(texto.IndexOf("Compra de café", StringComparison.Ordinal) < texto.IndexOf("Reintegro", StringComparison.Ordinal));
                Assert.IsTrue(page.Width > page.Height);
                Assert.AreEqual(1, pdf.NumberOfPages);
            }
            GuardarMuestra("movimientos-simple.pdf", bytes);
        }

        [TestMethod]
        public void Generar_MuchasFilas_RepiteEncabezadosYNumeracionEnCadaPagina()
        {
            var filas = Enumerable.Range(1, 150).Select(i => new[] {
                "14/10/2026", "Egreso", "Servicios", "Internet", "$ 10,00", "Movimiento " + i
            }).ToArray();
            byte[] bytes = new ReporteTabularPdfService().Generar(CrearReporte(filas));
            using (var pdf = PdfReader.Open(bytes))
            {
                Assert.IsTrue(pdf.NumberOfPages > 1);
                for (int i = 1; i <= pdf.NumberOfPages; i++)
                {
                    string texto = ContentOrderTextExtractor.GetText(pdf.GetPage(i));
                    Assert.Contains("Descripción", texto);
                    Assert.Contains("Página " + i + " de " + pdf.NumberOfPages, texto);
                }
                Assert.Contains("Movimiento 150", ContentOrderTextExtractor.GetText(pdf.GetPage(pdf.NumberOfPages)));
            }
            GuardarMuestra("movimientos-multipagina.pdf", bytes);
        }

        [TestMethod]
        public void Generar_DescripcionMayorQueUnaPagina_ConservaFinalYCuentaUnMovimiento()
        {
            string larga = string.Join(" ", Enumerable.Repeat("Descripción extensa con acentos y detalles.", 450)) + " FIN-DESCRIPCION";
            byte[] bytes = new ReporteTabularPdfService().Generar(CrearReporte(new[] {
                new[] { "14/10/2026", "Egreso", "Servicios", "", "$ 10,00", larga }
            }));
            using (var pdf = PdfReader.Open(bytes))
            {
                Assert.IsTrue(pdf.NumberOfPages > 1);
                string texto = string.Join(" ", pdf.GetPages().Select(p => ContentOrderTextExtractor.GetText(p)));
                Assert.Contains("FIN-DESCRIPCION", texto);
                Assert.Contains("Continuación", texto);
                Assert.Contains("Cantidad: 1", texto);
            }
            GuardarMuestra("movimientos-descripcion-larga.pdf", bytes);
        }

        private static ReporteMovimientosPdf CrearReporte(string[][] filas)
        {
            string root = EncontrarRaiz();
            string logo = Path.Combine(root, "UI", "Resources", "logo_reporte_finanzapp.png");
            return new ReporteMovimientosPdf(7, "Listado de movimientos",
                new[] { "Fecha", "Tipo", "Categoría", "Comercio", "Monto", "Descripción" },
                filas, new[] { "Descripción: café", "Tipo: Todos", "Fecha desde: Sin límite" },
                File.Exists(logo) ? File.ReadAllBytes(logo) : null, new DateTime(2026, 10, 14, 10, 30, 0),
                "Generado", "Cantidad", "Página {0} de {1}", "Continuación");
        }

        private static void GuardarMuestra(string nombre, byte[] bytes)
        {
            string carpeta = Path.Combine(EncontrarRaiz(), "TestResults", "pdf-samples");
            Directory.CreateDirectory(carpeta);
            File.WriteAllBytes(Path.Combine(carpeta, nombre), bytes);
        }

        private static string EncontrarRaiz()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(typeof(ReporteTabularPdfServiceTests).Assembly.Location));
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "TpIDS.sln"))) dir = dir.Parent;
            return dir == null ? throw new InvalidOperationException("No se encontro la raiz del repo.") : dir.FullName;
        }
    }
}
