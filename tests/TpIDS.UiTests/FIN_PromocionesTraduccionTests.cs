using System;
using System.Data.SqlClient;
using System.Linq;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Change gestion-promociones: la pantalla Operador &gt; Promociones se traduce con las keys
    /// sembradas, y el detalle del comercio lista las promociones que le aplican por la regla de
    /// alcance (incluida una que solo coincide por rubro).
    /// </summary>
    [TestClass]
    public class FIN_PromocionesTraduccionTests : UiTestBase
    {
        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_Pantalla_MuestraSusTextosTraducidos()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Promotions"));
            Window promociones = App.EsperarVentana("FormGestionPromociones");

            string titulo = TestDatabase.ObtenerTraduccion("FormPromotionManagement.TitleLabel");
            Assert.AreNotEqual("FormPromotionManagement.TitleLabel", titulo, "La key del titulo no esta sembrada.");
            Assert.AreEqual(titulo, UiFind.Elemento(promociones, "lblTituloPromociones").Name, "El titulo de la pantalla Promociones no se tradujo.");
            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormPromotionManagement.AddButton"),
                UiFind.Elemento(promociones, "btnAgregarPromocion").Name,
                "El boton Agregar no se tradujo.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_Pantalla_SeTraduceAInglesAlCambiarElIdioma()
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

                // El idioma guardado se aplica en vivo: el menu del shell ya queda en ingles.
                UiFind.ClickMenu(principal, TraduccionIngles("FormMainShell.Menu.Operator"), TraduccionIngles("FormMainShell.Menu.Promotions"));
                Window promociones = App.EsperarVentana("FormGestionPromociones");

                string tituloIngles = TraduccionIngles("FormPromotionManagement.TitleLabel");
                Assert.AreNotEqual(
                    TestDatabase.ObtenerTraduccion("FormPromotionManagement.TitleLabel"),
                    tituloIngles,
                    "Precondicion: el titulo en ingles debe diferir del espanol.");
                Assert.AreEqual(tituloIngles, UiFind.Elemento(promociones, "lblTituloPromociones").Name, "El titulo no se tradujo al ingles.");
                Assert.AreEqual(
                    TraduccionIngles("FormPromotionManagement.AddButton"),
                    UiFind.Elemento(promociones, "btnAgregarPromocion").Name,
                    "El boton Agregar no se tradujo al ingles.");
            }
            finally
            {
                RestaurarPreferenciaIdioma(previa);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_DetalleComercio_Promociones_IncluyeLaQueSoloCoincidePorRubro()
        {
            string sufijo = DateTime.Now.ToString("HHmmssfff");
            string nombreComercio = "uicomprom" + sufijo;
            string descripcionPorRubro = "uipromrubro" + sufijo;
            string descripcionOtroRubro = "uipromotro" + sufijo;

            int rubroDelComercioId = InsertarRubro("uirubroA" + sufijo);
            int otroRubroId = InsertarRubro("uirubroB" + sufijo);
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Comercio (RubroComercioId, Nombre) VALUES (@r, @n)",
                new SqlParameter("@r", rubroDelComercioId),
                new SqlParameter("@n", nombreComercio));
            InsertarPromocionPorRubro(descripcionPorRubro, rubroDelComercioId);
            InsertarPromocionPorRubro(descripcionOtroRubro, otroRubroId);

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Merchants"));
            Window comercios = App.EsperarVentana("FormGestionComercios");
            UiFind.ClickFilaGrilla(comercios, "dgvComercios", nombreComercio);
            UiFind.Click(comercios, "btnVerDetalleComercio");
            Window detalle = App.EsperarVentana("FormDetalleComercio");

            // La grilla ya cargo cuando aparece la promocion que coincide solo por rubro.
            UiFind.SeleccionarFilaGrilla(detalle, "dgvPromociones", descripcionPorRubro);

            Assert.IsFalse(
                GrillaContieneTexto(detalle, "dgvPromociones", descripcionOtroRubro),
                "La promocion de otro rubro no debe aparecer en el detalle del comercio.");
        }

        private static bool GrillaContieneTexto(Window ventana, string automationId, string texto)
        {
            AutomationElement grilla = UiFind.Elemento(ventana, automationId);
            return grilla.FindAllDescendants().Any(elemento =>
                elemento.Name == texto
                || elemento.Patterns.Value.PatternOrDefault?.Value.ValueOrDefault == texto);
        }

        private static int InsertarRubro(string nombre)
        {
            TestDatabase.EjecutarSql("INSERT INTO dbo.RubroComercio (Nombre) VALUES (@n)", new SqlParameter("@n", nombre));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.RubroComercio WHERE Nombre = @n", new SqlParameter("@n", nombre)));
        }

        private static void InsertarPromocionPorRubro(string descripcion, int rubroId)
        {
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Promocion (Descripcion, Descuento, TopeDescuento, VigenciaDesde, VigenciaHasta) " +
                "VALUES (@d, 15, NULL, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), DATEADD(DAY, 30, CAST(GETDATE() AS DATE)))",
                new SqlParameter("@d", descripcion));
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.PromocionRubroComercio (PromocionId, RubroComercioId) " +
                "SELECT Id, @r FROM dbo.Promocion WHERE Descripcion = @d",
                new SqlParameter("@r", rubroId),
                new SqlParameter("@d", descripcion));
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
