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
    /// Change gestion-promociones: ABM de promociones (Operador &gt; Promociones). Los datos previos
    /// se arman por SQL con textos unicos por corrida, para que la busqueda y la grilla no choquen
    /// con promociones de otros tests.
    /// </summary>
    [TestClass]
    public class FIN_PromocionesTests : UiTestBase
    {
        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_Alta_PersistePromocionConSusFiltros()
        {
            string sufijo = Sufijo();
            string nombreComercio = "uicomalta" + sufijo;
            string descripcion = "uipromalta" + sufijo;
            int comercioId = InsertarComercio(nombreComercio, InsertarRubro("uirubalta" + sufijo));

            Window promociones = AbrirGestionPromociones();

            UiFind.EscribirTexto(promociones, "txtbDescripcionPromocion", descripcion);
            UiFind.EscribirTexto(promociones, "txtbDescuentoPromocion", "15");
            UiFind.MarcarCheckBox(promociones, "chkbDiaLunes", true);
            UiFind.MarcarItemListaChequeable(promociones, "clbComercios", nombreComercio);
            UiFind.Click(promociones, "btnAgregarPromocion");
            EsperarAvisoYCerrar("FormPromotionManagement.Success.PromocionCreated");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Promocion p " +
                    "JOIN dbo.PromocionComercio pc ON pc.PromocionId = p.Id " +
                    "WHERE p.Descripcion = @d AND p.Descuento = 15 AND pc.ComercioId = @c",
                    new SqlParameter("@d", descripcion),
                    new SqlParameter("@c", comercioId)),
                "La promocion no quedo persistida con su comercio.");
            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.PromocionDiaSemana d " +
                    "JOIN dbo.Promocion p ON p.Id = d.PromocionId " +
                    "WHERE p.Descripcion = @d AND d.DiaSemana = 'Lunes'",
                    new SqlParameter("@d", descripcion)),
                "La promocion no quedo persistida con el dia marcado.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_BuscarPorTexto_MuestraSoloLaQueCoincide()
        {
            string sufijo = Sufijo();
            string buscada = "uitextoalfa" + sufijo;
            string otra = "uitextobeta" + sufijo;
            InsertarPromocion(buscada);
            InsertarPromocion(otra);

            Window promociones = AbrirGestionPromociones();

            // Precondicion: sin filtro ambas estan en la grilla, asi la ausencia posterior es del filtro.
            Assert.IsTrue(
                Retry.WhileFalse(() => GrillaContieneTexto(promociones, otra), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(300)).Success,
                "Precondicion: la promocion '" + otra + "' deberia estar en la grilla antes de buscar.");

            // Sin distinguir mayusculas: se busca en mayusculas una descripcion en minusculas.
            UiFind.EscribirTexto(promociones, "txtbBuscarTextoPromocion", buscada.ToUpperInvariant());
            UiFind.Click(promociones, "btnBuscarPromociones");

            EsperarQueDesaparezca(promociones, otra);
            Assert.IsTrue(
                GrillaContieneTexto(promociones, buscada),
                "La promocion que coincide con el texto deberia seguir en la grilla.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_BuscarPorComercio_IncluyeSinRestriccionYPorRubro()
        {
            string sufijo = Sufijo();
            string token = "uicombus" + sufijo;
            int rubroId = InsertarRubro("uirubbus" + sufijo);
            int otroRubroId = InsertarRubro("uirubotro" + sufijo);
            string nombreComercio = "uicombusA" + sufijo;
            int comercioId = InsertarComercio(nombreComercio, rubroId);
            int otroComercioId = InsertarComercio("uicombusB" + sufijo, otroRubroId);

            string sinRestriccion = token + " sinrestriccion";
            string porComercio = token + " porcomercio";
            string porRubro = token + " porrubro";
            string deOtroComercio = token + " deotrocomercio";
            InsertarPromocion(sinRestriccion);
            InsertarPromocion(porComercio);
            InsertarPromocion(porRubro);
            InsertarPromocion(deOtroComercio);
            EjecutarFiltro("INSERT INTO dbo.PromocionComercio (PromocionId, ComercioId) SELECT Id, @x FROM dbo.Promocion WHERE Descripcion = @d", porComercio, comercioId);
            EjecutarFiltro("INSERT INTO dbo.PromocionRubroComercio (PromocionId, RubroComercioId) SELECT Id, @x FROM dbo.Promocion WHERE Descripcion = @d", porRubro, rubroId);
            EjecutarFiltro("INSERT INTO dbo.PromocionComercio (PromocionId, ComercioId) SELECT Id, @x FROM dbo.Promocion WHERE Descripcion = @d", deOtroComercio, otroComercioId);

            Window promociones = AbrirGestionPromociones();

            UiFind.EscribirTexto(promociones, "txtbBuscarTextoPromocion", token);
            UiFind.SeleccionarItemCombo(promociones, "cmbComercioBusquedaPromocion", nombreComercio);
            UiFind.Click(promociones, "btnBuscarPromociones");

            EsperarQueDesaparezca(promociones, deOtroComercio);
            Assert.IsTrue(GrillaContieneTexto(promociones, sinRestriccion), "La promocion sin restriccion de comercio le aplica a todos.");
            Assert.IsTrue(GrillaContieneTexto(promociones, porComercio), "La promocion cargada para el comercio debe aparecer.");
            Assert.IsTrue(GrillaContieneTexto(promociones, porRubro), "La promocion que coincide por rubro debe aparecer.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_Eliminar_PromocionSinUso_QuitaPromocionYFiltros()
        {
            string sufijo = Sufijo();
            string descripcion = "uipromdel" + sufijo;
            int comercioId = InsertarComercio("uicomdel" + sufijo, InsertarRubro("uirubdel" + sufijo));
            InsertarPromocion(descripcion);
            EjecutarFiltro("INSERT INTO dbo.PromocionComercio (PromocionId, ComercioId) SELECT Id, @x FROM dbo.Promocion WHERE Descripcion = @d", descripcion, comercioId);
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.PromocionDiaSemana (PromocionId, DiaSemana) SELECT Id, 'Martes' FROM dbo.Promocion WHERE Descripcion = @d",
                new SqlParameter("@d", descripcion));

            Window promociones = AbrirGestionPromociones();
            SeleccionarPromocion(promociones, descripcion);

            UiFind.Click(promociones, "btnEliminarPromocion");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormPromotionManagement.Success.PromocionDeleted");

            Assert.AreEqual(0, ContarPromociones(descripcion), "La promocion eliminada no deberia seguir en la base.");
            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.PromocionComercio WHERE ComercioId = @c",
                    new SqlParameter("@c", comercioId)),
                "La baja debe quitar tambien los filtros de la promocion.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_Eliminar_PromocionReferenciadaPorNotificacion_InformaEnUsoYNoElimina()
        {
            string descripcion = "uipromuso" + Sufijo();
            InsertarPromocion(descripcion);
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Notificacion (UsuarioId, TipoNotificacion, Mensaje, PromocionId) " +
                "SELECT u.Id, 'Notificacion', 'uinotifprom', p.Id FROM dbo.Usuarios u, dbo.Promocion p " +
                "WHERE u.Username = @u AND p.Descripcion = @d",
                new SqlParameter("@u", TestConfig.BasicUsername),
                new SqlParameter("@d", descripcion));

            Window promociones = AbrirGestionPromociones();
            SeleccionarPromocion(promociones, descripcion);

            UiFind.Click(promociones, "btnEliminarPromocion");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("Error.PromotionManagement.PromocionInUse");

            Assert.AreEqual(1, ContarPromociones(descripcion), "Una promocion referenciada por una notificacion no se puede eliminar.");
        }

        // --- Navegacion ---

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Promociones_ListasConOpciones_NoMuestranLaAyudaDeListaVacia()
        {
            // Las listas se arman con datos propios del test, asi la ausencia de la ayuda no depende
            // del resto de la base: la ayuda de "lista vacia" solo puede verse si la lista no tiene items.
            string sufijo = Sufijo();
            InsertarComercio("uicomayuda" + sufijo, InsertarRubro("uirubayuda" + sufijo));
            InsertarProductoConCategoria("uiprodayuda" + sufijo, "uicatayuda" + sufijo);

            Window promociones = AbrirGestionPromociones();
            UiFind.Elemento(promociones, "clbComercios");

            string[] ayudas =
            {
                "lblVacioComercios", "lblVacioRubrosComercio",
                "lblVacioProductosIncluidos", "lblVacioProductosExcluidos",
                "lblVacioCategoriasIncluidas", "lblVacioCategoriasExcluidas"
            };
            foreach (string ayuda in ayudas)
            {
                // Un control con Visible = false no figura en el arbol de UIA.
                Assert.IsNull(
                    promociones.FindFirstDescendant(condicion => condicion.ByAutomationId(ayuda)),
                    "La ayuda '" + ayuda + "' no deberia verse cuando la lista tiene opciones.");
            }
        }

        private Window AbrirGestionPromociones()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Promotions"));
            return App.EsperarVentana("FormGestionPromociones");
        }

        /// <summary>
        /// Filtra por la descripcion unica, hace click real sobre la fila y espera a que el formulario
        /// la cargue en el editor (evita operar antes de que la seleccion se refleje).
        /// </summary>
        private static void SeleccionarPromocion(Window promociones, string descripcion)
        {
            UiFind.EscribirTexto(promociones, "txtbBuscarTextoPromocion", descripcion);
            UiFind.Click(promociones, "btnBuscarPromociones");
            UiFind.ClickFilaGrilla(promociones, "dgvPromociones", descripcion);

            RetryResult<bool> reflejada = Retry.WhileFalse(
                () => UiFind.LeerTexto(promociones, "txtbDescripcionPromocion") == descripcion,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(200));

            Assert.IsTrue(reflejada.Success, "La seleccion de '" + descripcion + "' no se reflejo en el editor.");
        }

        private static bool GrillaContieneTexto(Window ventana, string texto)
        {
            AutomationElement grilla = UiFind.Elemento(ventana, "dgvPromociones");
            return grilla.FindAllDescendants().Any(elemento =>
                elemento.Name == texto
                || elemento.Patterns.Value.PatternOrDefault?.Value.ValueOrDefault == texto);
        }

        /// <summary>Espera a que la grilla se refresque con la busqueda: la fila excluida deja de estar.</summary>
        private static void EsperarQueDesaparezca(Window ventana, string texto)
        {
            RetryResult<bool> resultado = Retry.WhileFalse(
                () => !GrillaContieneTexto(ventana, texto),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(300));

            Assert.IsTrue(resultado.Success, "La promocion '" + texto + "' no deberia aparecer con ese filtro.");
        }

        // --- Datos previos ---

        private static string Sufijo()
        {
            return DateTime.Now.ToString("HHmmssfff");
        }

        private static int InsertarRubro(string nombre)
        {
            TestDatabase.EjecutarSql("INSERT INTO dbo.RubroComercio (Nombre) VALUES (@n)", new SqlParameter("@n", nombre));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.RubroComercio WHERE Nombre = @n", new SqlParameter("@n", nombre)));
        }

        private static int InsertarComercio(string nombre, int rubroId)
        {
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Comercio (RubroComercioId, Nombre) VALUES (@r, @n)",
                new SqlParameter("@r", rubroId),
                new SqlParameter("@n", nombre));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.Comercio WHERE Nombre = @n", new SqlParameter("@n", nombre)));
        }

        private static void InsertarProductoConCategoria(string producto, string categoria)
        {
            TestDatabase.EjecutarSql("INSERT INTO dbo.CategoriaProducto (Nombre) VALUES (@c)", new SqlParameter("@c", categoria));
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Producto (CategoriaProductoId, Nombre) SELECT Id, @n FROM dbo.CategoriaProducto WHERE Nombre = @c",
                new SqlParameter("@n", producto),
                new SqlParameter("@c", categoria));
        }

        private static void InsertarPromocion(string descripcion)
        {
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Promocion (Descripcion, Descuento, TopeDescuento, VigenciaDesde, VigenciaHasta) " +
                "VALUES (@d, 15, NULL, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), DATEADD(DAY, 30, CAST(GETDATE() AS DATE)))",
                new SqlParameter("@d", descripcion));
        }

        private static void EjecutarFiltro(string sql, string descripcion, int id)
        {
            int filas = TestDatabase.EjecutarSql(sql, new SqlParameter("@d", descripcion), new SqlParameter("@x", id));
            Assert.AreEqual(1, filas, "No se pudo armar el dato previo: filtro de la promocion '" + descripcion + "'.");
        }

        private static int ContarPromociones(string descripcion)
        {
            return TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Promocion WHERE Descripcion = @d", new SqlParameter("@d", descripcion));
        }
    }
}
