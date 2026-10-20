using System;
using System.Linq;
using BE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS;

namespace BLL.Tests
{
    // Lectura de notificaciones con contenido completo (notificaciones-automaticas-diarias 3.4):
    // la grilla muestra un resumen de una linea, el detalle el texto completo con sus saltos y el
    // tipo no depende de item ni vencimiento.
    [TestClass]
    public sealed class VistaPreviaNotificacionTests
    {
        [TestMethod]
        public void Resumir_MensajeCorto_SeConserva()
        {
            Assert.AreEqual("Hola usuario", VistaPreviaNotificacion.Resumir("Hola usuario"));
        }

        [TestMethod]
        public void Resumir_NuloOVacio_DevuelveVacio()
        {
            Assert.AreEqual(string.Empty, VistaPreviaNotificacion.Resumir(null));
            Assert.AreEqual(string.Empty, VistaPreviaNotificacion.Resumir("   \r\n  "));
        }

        [TestMethod]
        public void Resumir_SaltosDeLineaYEspacios_QuedaEnUnaSolaLinea()
        {
            string resumen = VistaPreviaNotificacion.Resumir("Linea 1\r\n\r\nLinea 2\n\tLinea 3   fin");

            Assert.AreEqual("Linea 1 Linea 2 Linea 3 fin", resumen);
        }

        [TestMethod]
        public void Resumir_MensajeExtenso_SeTruncaAlLimiteConPuntosSuspensivos()
        {
            string largo = string.Join("\r\n", Enumerable.Range(1, 400).Select(i => "Linea " + i));

            string resumen = VistaPreviaNotificacion.Resumir(largo);

            Assert.AreEqual(VistaPreviaNotificacion.MaximoCaracteres, resumen.Length);
            Assert.EndsWith("…", resumen);
            Assert.StartsWith("Linea 1 Linea 2", resumen);
            Assert.DoesNotContain("\n", resumen);
        }

        [TestMethod]
        public void Resumir_ExactamenteElLimite_NoSeTrunca()
        {
            string exacto = new string('a', VistaPreviaNotificacion.MaximoCaracteres);

            Assert.AreEqual(exacto, VistaPreviaNotificacion.Resumir(exacto));
        }

        [TestMethod]
        public void Resumir_CorteEnMedioDeUnParSubrogado_NoDejaMitadDeCaracter()
        {
            string texto = new string('a', VistaPreviaNotificacion.MaximoCaracteres - 2) + "\U0001F4B0\U0001F4B0\U0001F4B0";

            string resumen = VistaPreviaNotificacion.Resumir(texto);

            Assert.IsFalse(char.IsHighSurrogate(resumen[resumen.Length - 2]));
            Assert.IsTrue(resumen.Length <= VistaPreviaNotificacion.MaximoCaracteres);
            Assert.EndsWith("…", resumen);
        }

        [TestMethod]
        public void ParaCuadroDeTexto_SaltosSueltos_SeNormalizanASaltosDeWindows()
        {
            Assert.AreEqual("a\r\nb\r\nc\r\nd", VistaPreviaNotificacion.ParaCuadroDeTexto("a\nb\r\nc\rd"));
            Assert.AreEqual(string.Empty, VistaPreviaNotificacion.ParaCuadroDeTexto(null));
        }

        [TestMethod]
        public void ParaCuadroDeTexto_MensajeExtenso_NoSeTrunca()
        {
            string largo = string.Join("\n", Enumerable.Range(1, 2000).Select(i => "Linea " + i));

            string texto = VistaPreviaNotificacion.ParaCuadroDeTexto(largo);

            Assert.AreEqual(largo.Replace("\n", "\r\n"), texto);
        }

        [TestMethod]
        public void ClaveTipo_ComprasAgregadaSinItemNiVencimiento_EsRecordatorioDeCompra()
        {
            var agregada = new RecordatorioCompra { TipoAutomatico = TipoNotificacionAutomatica.Compras, FechaJornada = new DateTime(2026, 10, 20) };

            Assert.AreEqual("FormMyNotifications.Value.PurchaseReminder", VistaPreviaNotificacion.ClaveTipo(agregada));
        }

        [TestMethod]
        public void ClaveTipo_InversionYAvisoBase_ConservanSuEtiqueta()
        {
            Assert.AreEqual("FormMyNotifications.Value.InvestmentReminder", VistaPreviaNotificacion.ClaveTipo(new RecordatorioInversion()));
            Assert.AreEqual("FormMyNotifications.Value.ManualNotification", VistaPreviaNotificacion.ClaveTipo(new Notificacion()));
            Assert.AreEqual("FormMyNotifications.Value.ManualNotification", VistaPreviaNotificacion.ClaveTipo(new Notificacion { TipoAutomatico = TipoNotificacionAutomatica.Movimientos }));
        }
    }
}
