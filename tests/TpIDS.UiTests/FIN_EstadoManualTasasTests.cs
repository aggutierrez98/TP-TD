using System;
using System.Data.SqlClient;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Change actualizacion-automatica-tasas: estado manual en Operador &gt; Monedas. Cada test siembra su
    /// propio par de monedas y su tasa (codigos unicos por corrida, sin tocar el par USD/ARS del seed) y
    /// los borra al terminar; las entradas de bitacora que escribe la app quedan, como en el resto de la suite.
    /// </summary>
    [TestClass]
    public class FIN_EstadoManualTasasTests : UiTestBase
    {
        private const string ActividadMarcada = "FormCurrencyManagement.Value.Activity.TasaMarkedManual";
        private const string ActividadLiberada = "FormCurrencyManagement.Value.Activity.TasaReleased";

        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");
        private static readonly string TextoMenuMonedas = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Currencies");

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_EstadoManual_EditarUnaTasa_LaMarcaManualYHabilitaLiberar()
        {
            string sufijo = Sufijo();
            string codigoBase = "UA" + sufijo;
            string codigoConversion = "UC" + sufijo;

            try
            {
                int tasaId = SembrarTasa(codigoBase, codigoConversion, 10m, false);
                int marcadasPrevias = TestDatabase.ContarBitacora(ActividadMarcada, TestConfig.OperadorUsername);

                Window monedas = AbrirMonedas();
                UiFind.ClickFilaGrilla(monedas, "dgvTasas", codigoBase);

                string encabezado = TestDatabase.ObtenerTraduccion("FormCurrencyManagement.TasaGrid.EsManual");
                Assert.AreNotEqual("FormCurrencyManagement.TasaGrid.EsManual", encabezado, "La key del encabezado no esta sembrada.");
                Assert.IsTrue(
                    Retry.WhileFalse(() => GrillaContieneTexto(monedas, "dgvTasas", encabezado), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                    "La grilla de tasas deberia mostrar la columna '" + encabezado + "'.");
                Assert.IsFalse(
                    UiFind.Boton(monedas, "btnLiberarTasa").IsEnabled,
                    "Una tasa automatica no deberia permitir Liberar.");

                // Se espera a que el click cargue la tasa en el editor antes de cambiar el valor.
                Assert.IsTrue(
                    Retry.WhileFalse(() => LeerNumero(monedas, "nudCalculo") == 10m, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                    "La seleccion de la tasa no se reflejo en el editor.");

                UiFind.EstablecerNumero(monedas, "nudCalculo", 25);
                UiFind.Click(monedas, "btnActualizarTasa");
                EsperarAvisoYCerrar("FormCurrencyManagement.Success.TasaUpdated");

                Assert.AreEqual(
                    1,
                    TestDatabase.Contar(
                        "SELECT COUNT(*) FROM dbo.TasaConversion t JOIN dbo.Usuarios u ON u.Id = t.ProtegidaPorUsuarioId " +
                        "WHERE t.Id = @id AND t.EsManual = 1 AND t.CalculoConversion = 25 AND t.FechaProteccion IS NOT NULL AND u.Username = @u",
                        new SqlParameter("@id", tasaId),
                        new SqlParameter("@u", TestConfig.OperadorUsername)),
                    "Editar la tasa deberia dejarla manual, con el valor nuevo y protegida por el operador.");
                Assert.AreEqual(
                    marcadasPrevias + 1,
                    TestDatabase.ContarBitacora(ActividadMarcada, TestConfig.OperadorUsername),
                    "Marcar la tasa como manual deberia registrarse en la bitacora.");
                Assert.IsTrue(
                    Retry.WhileFalse(() => UiFind.Boton(monedas, "btnLiberarTasa").IsEnabled, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                    "Con la tasa manual seleccionada el boton Liberar deberia estar habilitado.");
            }
            finally
            {
                EliminarTasaYMonedas(codigoBase, codigoConversion);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_EstadoManual_LiberarUnaTasaManual_LaDesmarcaYDeshabilitaElBoton()
        {
            string sufijo = Sufijo();
            string codigoBase = "UA" + sufijo;
            string codigoConversion = "UC" + sufijo;

            try
            {
                int tasaId = SembrarTasa(codigoBase, codigoConversion, 10m, true);
                int liberadasPrevias = TestDatabase.ContarBitacora(ActividadLiberada, TestConfig.OperadorUsername);

                Window monedas = AbrirMonedas();
                UiFind.ClickFilaGrilla(monedas, "dgvTasas", codigoBase);

                Assert.IsTrue(
                    Retry.WhileFalse(() => UiFind.Boton(monedas, "btnLiberarTasa").IsEnabled, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                    "Con la tasa manual seleccionada el boton Liberar deberia estar habilitado.");

                UiFind.Click(monedas, "btnLiberarTasa");
                EsperarAvisoYCerrar("FormCurrencyManagement.Success.TasaReleased");

                Assert.AreEqual(
                    1,
                    TestDatabase.Contar(
                        "SELECT COUNT(*) FROM dbo.TasaConversion WHERE Id = @id AND EsManual = 0 AND ProtegidaPorUsuarioId IS NULL AND FechaProteccion IS NULL",
                        new SqlParameter("@id", tasaId)),
                    "Liberar deberia dejar la tasa automatica y sin datos de proteccion.");
                Assert.AreEqual(
                    liberadasPrevias + 1,
                    TestDatabase.ContarBitacora(ActividadLiberada, TestConfig.OperadorUsername),
                    "Liberar la tasa deberia registrarse en la bitacora.");
                Assert.IsTrue(
                    Retry.WhileFalse(() => !UiFind.Boton(monedas, "btnLiberarTasa").IsEnabled, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                    "Con la tasa ya liberada el boton Liberar deberia volver a deshabilitarse.");
            }
            finally
            {
                EliminarTasaYMonedas(codigoBase, codigoConversion);
            }
        }

        // --- Navegacion y lecturas ---

        private Window AbrirMonedas()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TextoMenuMonedas);
            return App.EsperarVentana("FormGestionMonedas");
        }

        private static bool GrillaContieneTexto(Window ventana, string automationId, string texto)
        {
            AutomationElement grilla = UiFind.Elemento(ventana, automationId);
            return grilla.FindAllDescendants().Any(elemento =>
                elemento.Name == texto
                || elemento.Patterns.Value.PatternOrDefault?.Value.ValueOrDefault == texto);
        }

        /// <summary>Lee el valor numerico del NumericUpDown (RangeValue si lo expone; si no, el texto de su caja).</summary>
        private static decimal LeerNumero(Window ventana, string automationId)
        {
            AutomationElement control = UiFind.Elemento(ventana, automationId);

            var rango = control.Patterns.RangeValue.PatternOrDefault;
            if (rango != null)
            {
                return Convert.ToDecimal(rango.Value.ValueOrDefault);
            }

            AutomationElement caja = control.FindFirstDescendant(
                condicion => condicion.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
            string texto = caja?.Patterns.Value.PatternOrDefault?.Value.ValueOrDefault ?? control.Patterns.Value.PatternOrDefault?.Value.ValueOrDefault;

            decimal valor;
            return decimal.TryParse(texto, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out valor)
                ? valor
                : decimal.MinValue;
        }

        // --- Datos previos (Moneda y TasaConversion no llevan firmas DVH/DVV) ---

        private static string Sufijo()
        {
            return DateTime.Now.ToString("mmssfff");
        }

        private static int SembrarTasa(string codigoBase, string codigoConversion, decimal calculo, bool manual)
        {
            int monedaBaseId = InsertarMoneda(codigoBase);
            int monedaConversionId = InsertarMoneda(codigoConversion);

            return Convert.ToInt32(TestDatabase.EscalarOrNull(
                "INSERT INTO dbo.TasaConversion (MonedaBaseId, MonedaConversionId, CalculoConversion, EsManual, ProtegidaPorUsuarioId, FechaProteccion) " +
                "SELECT @b, @c, @calculo, @manual, " +
                "CASE WHEN @manual = 1 THEN (SELECT Id FROM dbo.Usuarios WHERE Username = @u) END, " +
                "CASE WHEN @manual = 1 THEN GETDATE() END; " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);",
                new SqlParameter("@b", monedaBaseId),
                new SqlParameter("@c", monedaConversionId),
                new SqlParameter("@calculo", calculo),
                new SqlParameter("@manual", manual),
                new SqlParameter("@u", TestConfig.OperadorUsername)));
        }

        private static int InsertarMoneda(string codigo)
        {
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Moneda (Codigo, Nombre) VALUES (@c, @n)",
                new SqlParameter("@c", codigo),
                new SqlParameter("@n", "uimoneda " + codigo));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.Moneda WHERE Codigo = @c", new SqlParameter("@c", codigo)));
        }

        private static void EliminarTasaYMonedas(string codigoBase, string codigoConversion)
        {
            TestDatabase.EjecutarSql(
                "DELETE t FROM dbo.TasaConversion t " +
                "JOIN dbo.Moneda b ON b.Id = t.MonedaBaseId " +
                "JOIN dbo.Moneda c ON c.Id = t.MonedaConversionId " +
                "WHERE b.Codigo = @b OR c.Codigo = @c",
                new SqlParameter("@b", codigoBase),
                new SqlParameter("@c", codigoConversion));
            TestDatabase.EjecutarSql(
                "DELETE FROM dbo.Moneda WHERE Codigo IN (@b, @c)",
                new SqlParameter("@b", codigoBase),
                new SqlParameter("@c", codigoConversion));
        }
    }
}
