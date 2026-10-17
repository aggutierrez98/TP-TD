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
    /// Change actualizacion-automatica-tasas: pantalla Operador &gt; Actualizacion de tasas. El historial
    /// se arma por SQL (las tablas de ejecuciones no llevan firmas DVH/DVV) y cada test deja la base como
    /// la encontro. Ningun test consulta la API real: "Actualizar ahora" se prueba dentro del tope de
    /// 60 minutos, donde el BLL corta antes de pedir cualquier cosa al proveedor.
    /// </summary>
    [TestClass]
    public class FIN_ActualizacionTasasTests : UiTestBase
    {
        private const int LimiteMensual = 1000;

        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");
        private static readonly string TextoMenuTasas = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.ExchangeRateUpdate");

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ActualizacionTasas_Operador_AccedeDesdeElMenuYVeLaPantallaTraducida()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Assert.AreNotEqual("FormMainShell.Menu.ExchangeRateUpdate", TextoMenuTasas, "La key del menu no esta sembrada.");

            Window pantalla = AbrirPantalla(principal);

            AssertTraducido(pantalla, "lblTituloActualizacionTasas", "FormExchangeRateUpdate.TitleLabel");
            AssertTraducido(pantalla, "btnActualizarAhoraTasas", "FormExchangeRateUpdate.UpdateNowButton");
            AssertTraducido(pantalla, "lblDetalleActualizacionTasas", "FormExchangeRateUpdate.DetailLabel");
            Assert.IsTrue(UiFind.Boton(pantalla, "btnActualizarAhoraTasas").IsEnabled, "El boton Actualizar ahora deberia estar habilitado.");
            UiFind.Elemento(pantalla, "dgvEjecucionesTasas");
            UiFind.Elemento(pantalla, "dgvDetalleEjecucionTasas");

            foreach (string columna in new[] { "FechaInicio", "Origen", "Resultado", "ConsumioRequest", "Mensaje" })
            {
                AssertEncabezado(pantalla, "dgvEjecucionesTasas", "FormExchangeRateUpdate.Grid." + columna);
            }

            foreach (string columna in new[] { "MonedaBase", "MonedaConversion", "ValorAnterior", "ValorNuevo", "Estado" })
            {
                AssertEncabezado(pantalla, "dgvDetalleEjecucionTasas", "FormExchangeRateUpdate.DetailGrid." + columna);
            }

            AssertContadorDelMes(pantalla);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ActualizacionTasas_Administrador_TambienAccedeDesdeElMenu()
        {
            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);

            Window pantalla = AbrirPantalla(principal);

            AssertTraducido(pantalla, "lblTituloActualizacionTasas", "FormExchangeRateUpdate.TitleLabel");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ActualizacionTasas_UsuarioBasico_NoTieneAccesoAlMenuOperador()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);

            Assert.IsFalse(
                UiFind.MenuTopHabilitado(principal, TextoMenuOperador),
                "Un usuario sin permisos de Operador no deberia poder abrir el menu que contiene Actualizacion de tasas.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ActualizacionTasas_Historial_MuestraLaEjecucionSembradaYSuDetalle()
        {
            string sufijo = Sufijo();
            string codigoBase = "UB" + sufijo;
            string codigoConversion = "UC" + sufijo;
            string mensaje = "uitasashist" + sufijo;
            int idPrevio = ObtenerIdMaximoEjecuciones();

            try
            {
                int monedaBaseId = InsertarMoneda(codigoBase);
                int monedaConversionId = InsertarMoneda(codigoConversion);
                int ejecucionId = InsertarEjecucion("Programada", "Exitosa", true, mensaje, 5);
                InsertarDetalle(ejecucionId, monedaBaseId, monedaConversionId, 100m, 110.5m, "Actualizado");
                InsertarDetalle(ejecucionId, monedaConversionId, monedaBaseId, 0.01m, null, "SaltadoManual");

                Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
                Window pantalla = AbrirPantalla(principal);

                UiFind.ClickFilaGrilla(pantalla, "dgvEjecucionesTasas", mensaje);

                Assert.IsTrue(
                    Retry.WhileFalse(
                        () => UiFind.ContarFilasGrilla(pantalla, "dgvDetalleEjecucionTasas") == 2,
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromMilliseconds(300)).Success,
                    "Al seleccionar la ejecucion el detalle deberia mostrar sus dos pares.");

                Assert.IsTrue(GrillaContieneTexto(pantalla, "dgvEjecucionesTasas", TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.Value.Origin.Programada")), "El origen no se mostro traducido.");
                Assert.IsTrue(GrillaContieneTexto(pantalla, "dgvEjecucionesTasas", TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.Value.Result.Exitosa")), "El resultado no se mostro traducido.");
                Assert.IsTrue(GrillaContieneTexto(pantalla, "dgvDetalleEjecucionTasas", codigoBase), "El detalle deberia mostrar el codigo de la moneda base.");
                Assert.IsTrue(GrillaContieneTexto(pantalla, "dgvDetalleEjecucionTasas", codigoConversion), "El detalle deberia mostrar el codigo de la moneda de conversion.");
                Assert.IsTrue(GrillaContieneTexto(pantalla, "dgvDetalleEjecucionTasas", TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.Value.PairStatus.Actualizado")), "El estado Actualizado no se mostro traducido.");
                Assert.IsTrue(GrillaContieneTexto(pantalla, "dgvDetalleEjecucionTasas", TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.Value.PairStatus.SaltadoManual")), "El estado SaltadoManual no se mostro traducido.");
                AssertContadorDelMes(pantalla);
            }
            finally
            {
                LimpiarEjecuciones(idPrevio);
                EliminarMonedas("UB" + sufijo, "UC" + sufijo);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ActualizacionTasas_ActualizarAhora_DentroDelTope_NoConsultaYRegistraLaOmision()
        {
            int idPrevio = ObtenerIdMaximoEjecuciones();

            try
            {
                InsertarEjecucion("Programada", "Exitosa", true, null, 5);
                int consumidasPrevias = ContarConsumidasDesde(idPrevio);
                Assert.AreEqual(1, consumidasPrevias, "Precondicion: solo la ejecucion sembrada deberia haber consumido request.");

                Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
                Window pantalla = AbrirPantalla(principal);

                UiFind.Click(pantalla, "btnActualizarAhoraTasas");

                MessageBoxWindow aviso = App.EsperarMessageBox();
                string prefijo = TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.Info.Skipped").Split(new[] { "{0}" }, StringSplitOptions.None)[0];
                Assert.IsTrue(
                    aviso.Texto.Contains(prefijo),
                    "Se esperaba el aviso de omision por tope pero se mostro: " + aviso.Texto);
                aviso.CerrarConOk();

                Assert.IsTrue(
                    Retry.WhileFalse(() => UiFind.Boton(pantalla, "btnActualizarAhoraTasas").IsEnabled, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                    "El boton Actualizar ahora deberia volver a habilitarse al terminar.");
                Assert.AreEqual(
                    1,
                    TestDatabase.Contar(
                        "SELECT COUNT(*) FROM dbo.EjecucionActualizacionTasas WHERE Id > @id AND Origen = 'Manual' AND Resultado = 'Omitida' AND ConsumioRequest = 0",
                        new SqlParameter("@id", idPrevio)),
                    "La omision por tope deberia quedar registrada como ejecucion Manual/Omitida sin consumo.");
                Assert.AreEqual(
                    0,
                    TestDatabase.Contar(
                        "SELECT COUNT(*) FROM dbo.EjecucionActualizacionTasas WHERE Id > @id AND Resultado = 'Fallida'",
                        new SqlParameter("@id", idPrevio)),
                    "Dentro del tope no deberia haberse intentado ninguna consulta.");
                Assert.AreEqual(consumidasPrevias, ContarConsumidasDesde(idPrevio), "La omision no deberia consumir requests.");
                Assert.IsTrue(
                    Retry.WhileFalse(
                        () => GrillaContieneTexto(pantalla, "dgvEjecucionesTasas", TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.Value.Result.Omitida")),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromMilliseconds(300)).Success,
                    "La grilla deberia refrescarse y mostrar la ejecucion Omitida.");
                AssertContadorDelMes(pantalla);
            }
            finally
            {
                LimpiarEjecuciones(idPrevio);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ActualizacionTasas_Pantalla_SeTraduceAInglesAlCambiarElIdioma()
        {
            object previa = TestDatabase.EscalarOrNull(
                "SELECT up.IdiomaId FROM dbo.UsuarioPreferencias up " +
                "JOIN dbo.Usuarios u ON u.Id = up.UsuarioId WHERE u.Username = @u",
                new SqlParameter("@u", TestConfig.AdminUsername));

            try
            {
                Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
                Window configuracion = AbrirConfiguracion(principal);
                UiFind.SeleccionarItemCombo(configuracion, "cmbIdiomas", "English");
                UiFind.Click(configuracion, "btnGuardarConfiguracion");
                App.EsperarMessageBox().CerrarConOk();
                configuracion.Close();

                UiFind.ClickMenu(principal, TraduccionIngles("FormMainShell.Menu.Operator"), TraduccionIngles("FormMainShell.Menu.ExchangeRateUpdate"));
                Window pantalla = App.EsperarVentana("FormActualizacionTasas");

                string tituloIngles = TraduccionIngles("FormExchangeRateUpdate.TitleLabel");
                Assert.AreNotEqual(
                    TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.TitleLabel"),
                    tituloIngles,
                    "Precondicion: el titulo en ingles debe diferir del espanol.");
                Assert.AreEqual(tituloIngles, UiFind.Elemento(pantalla, "lblTituloActualizacionTasas").Name, "El titulo no se tradujo al ingles.");
                Assert.AreEqual(
                    TraduccionIngles("FormExchangeRateUpdate.UpdateNowButton"),
                    UiFind.Elemento(pantalla, "btnActualizarAhoraTasas").Name,
                    "El boton Actualizar ahora no se tradujo al ingles.");

                string prefijoContador = TraduccionIngles("FormExchangeRateUpdate.RequestsCounter").Split(new[] { "{0}" }, StringSplitOptions.None)[0];
                Assert.IsTrue(
                    Retry.WhileFalse(
                        () => UiFind.Elemento(pantalla, "lblRequestsMesActualizacionTasas").Name.StartsWith(prefijoContador),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromMilliseconds(300)).Success,
                    "El contador de requests no se mostro en ingles.");
            }
            finally
            {
                RestaurarPreferenciaIdioma(previa);
            }
        }

        // --- Navegacion y asserts ---

        private Window AbrirPantalla(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuOperador, TextoMenuTasas);
            return App.EsperarVentana("FormActualizacionTasas");
        }

        private static void AssertTraducido(Window ventana, string automationId, string key)
        {
            string texto = TestDatabase.ObtenerTraduccion(key);
            Assert.AreNotEqual(key, texto, "La key '" + key + "' no esta sembrada.");
            Assert.AreEqual(texto, UiFind.Elemento(ventana, automationId).Name, "El control '" + automationId + "' no se mostro traducido.");
        }

        private static void AssertEncabezado(Window ventana, string grilla, string key)
        {
            string texto = TestDatabase.ObtenerTraduccion(key);
            Assert.AreNotEqual(key, texto, "La key '" + key + "' no esta sembrada.");
            Assert.IsTrue(
                Retry.WhileFalse(() => GrillaContieneTexto(ventana, grilla, texto), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                "La grilla '" + grilla + "' no muestra el encabezado '" + texto + "'.");
        }

        private static void AssertContadorDelMes(Window ventana)
        {
            DateTime hoy = DateTime.Now;
            DateTime inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            int usadas = TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.EjecucionActualizacionTasas WHERE ConsumioRequest = 1 AND FechaInicio >= @desde AND FechaInicio < @hasta",
                new SqlParameter("@desde", inicioMes),
                new SqlParameter("@hasta", inicioMes.AddMonths(1)));
            string esperado = string.Format(TestDatabase.ObtenerTraduccion("FormExchangeRateUpdate.RequestsCounter"), usadas, LimiteMensual);

            Assert.IsTrue(
                Retry.WhileFalse(
                    () => UiFind.Elemento(ventana, "lblRequestsMesActualizacionTasas").Name == esperado,
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromMilliseconds(300)).Success,
                "El contador de requests deberia decir '" + esperado + "'.");
        }

        private static bool GrillaContieneTexto(Window ventana, string automationId, string texto)
        {
            AutomationElement grilla = UiFind.Elemento(ventana, automationId);
            return grilla.FindAllDescendants().Any(elemento =>
                elemento.Name == texto
                || elemento.Patterns.Value.PatternOrDefault?.Value.ValueOrDefault == texto);
        }

        // --- Datos previos (tablas sin firmas DVH/DVV) ---

        private static string Sufijo()
        {
            return DateTime.Now.ToString("mmssfff");
        }

        private static int ObtenerIdMaximoEjecuciones()
        {
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT ISNULL(MAX(Id), 0) FROM dbo.EjecucionActualizacionTasas"));
        }

        private static int InsertarMoneda(string codigo)
        {
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Moneda (Codigo, Nombre) VALUES (@c, @n)",
                new SqlParameter("@c", codigo),
                new SqlParameter("@n", "uimoneda " + codigo));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.Moneda WHERE Codigo = @c", new SqlParameter("@c", codigo)));
        }

        private static int InsertarEjecucion(string origen, string resultado, bool consumioRequest, string mensaje, int minutosAtras)
        {
            return Convert.ToInt32(TestDatabase.EscalarOrNull(
                "INSERT INTO dbo.EjecucionActualizacionTasas (FechaInicio, Origen, Resultado, ConsumioRequest, Mensaje) " +
                "VALUES (DATEADD(MINUTE, -@minutos, GETDATE()), @origen, @resultado, @consumio, @mensaje); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);",
                new SqlParameter("@minutos", minutosAtras),
                new SqlParameter("@origen", origen),
                new SqlParameter("@resultado", resultado),
                new SqlParameter("@consumio", consumioRequest),
                new SqlParameter("@mensaje", (object)mensaje ?? DBNull.Value)));
        }

        private static void InsertarDetalle(int ejecucionId, int monedaBaseId, int monedaConversionId, decimal valorAnterior, decimal? valorNuevo, string estado)
        {
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.ActualizacionTasaDetalle (EjecucionId, MonedaBaseId, MonedaConversionId, ValorAnterior, ValorNuevo, Estado) " +
                "VALUES (@e, @b, @c, @anterior, @nuevo, @estado)",
                new SqlParameter("@e", ejecucionId),
                new SqlParameter("@b", monedaBaseId),
                new SqlParameter("@c", monedaConversionId),
                new SqlParameter("@anterior", valorAnterior),
                new SqlParameter("@nuevo", (object)valorNuevo ?? DBNull.Value),
                new SqlParameter("@estado", estado));
        }

        private static int ContarConsumidasDesde(int idPrevio)
        {
            return TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.EjecucionActualizacionTasas WHERE Id > @id AND ConsumioRequest = 1",
                new SqlParameter("@id", idPrevio));
        }

        /// <summary>Borra todo lo creado desde el snapshot: lo sembrado y lo que registro la propia app.</summary>
        private static void LimpiarEjecuciones(int idPrevio)
        {
            TestDatabase.EjecutarSql(
                "DELETE FROM dbo.ActualizacionTasaDetalle WHERE EjecucionId > @id",
                new SqlParameter("@id", idPrevio));
            TestDatabase.EjecutarSql(
                "DELETE FROM dbo.EjecucionActualizacionTasas WHERE Id > @id",
                new SqlParameter("@id", idPrevio));
        }

        private static void EliminarMonedas(params string[] codigos)
        {
            foreach (string codigo in codigos)
            {
                TestDatabase.EjecutarSql("DELETE FROM dbo.Moneda WHERE Codigo = @c", new SqlParameter("@c", codigo));
            }
        }

        private static string TraduccionIngles(string key)
        {
            return Convert.ToString(TestDatabase.EscalarOrNull(
                "SELECT t.Valor FROM dbo.Traduccion t " +
                "JOIN dbo.Palabra p ON p.Id = t.PalabraId " +
                "JOIN dbo.Idioma i ON i.Id = t.IdiomaId " +
                "WHERE p.[Key] = @key AND i.Nombre = 'English'",
                new SqlParameter("@key", key)));
        }

        private static void RestaurarPreferenciaIdioma(object idiomaIdPrevio)
        {
            SqlParameter usuario = new SqlParameter("@u", TestConfig.AdminUsername);
            if (idiomaIdPrevio == null)
            {
                TestDatabase.EjecutarSql(
                    "DELETE up FROM dbo.UsuarioPreferencias up JOIN dbo.Usuarios u ON u.Id = up.UsuarioId WHERE u.Username = @u",
                    usuario);
                return;
            }

            TestDatabase.EjecutarSql(
                "UPDATE up SET IdiomaId = @i FROM dbo.UsuarioPreferencias up JOIN dbo.Usuarios u ON u.Id = up.UsuarioId WHERE u.Username = @u",
                usuario,
                new SqlParameter("@i", idiomaIdPrevio));
        }
    }
}
