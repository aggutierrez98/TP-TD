using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Change desacoplar-producto-de-comercio: el catalogo de productos (Operador &gt; Productos) es
    /// independiente de los comercios, y el detalle del comercio asigna productos del catalogo con
    /// precio y moneda propios. Los datos previos se arman por SQL directo.
    /// </summary>
    [TestClass]
    public class FIN_ProductosTests : UiTestBase
    {
        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_Crear_PersisteProductoEnElCatalogoSinComercio()
        {
            string categoria = NombreUnico("uicat");
            int categoriaId = InsertarCategoriaProducto(categoria);
            string nombre = NombreUnico("uiprod");

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window productos = AbrirGestionProductos(principal);

            UiFind.EscribirTexto(productos, "txtbNombreProducto", nombre);
            UiFind.SeleccionarItemCombo(productos, "cmbCategoriaProducto", categoria);
            UiFind.Click(productos, "btnAgregarProducto");
            EsperarAvisoYCerrar("FormProductManagement.Success.ProductoCreated");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Producto WHERE Nombre = @n AND CategoriaProductoId = @c",
                    new SqlParameter("@n", nombre),
                    new SqlParameter("@c", categoriaId)),
                "El producto nuevo no quedo persistido en el catalogo.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_Pantalla_MuestraSusTextosTraducidos()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window productos = AbrirGestionProductos(principal);

            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormProductManagement.TitleLabel"),
                UiFind.Elemento(productos, "lblTituloProductos").Name,
                "El titulo de la pantalla Productos no se tradujo con la key del catalogo.");
            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormProductManagement.AddButton"),
                UiFind.Elemento(productos, "btnAgregarProducto").Name,
                "El boton Agregar no se tradujo con la key del catalogo.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_Pantalla_SeTraduceAInglesAlCambiarElIdioma()
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
                UiFind.ClickMenu(principal, TraduccionIngles("FormMainShell.Menu.Operator"), TraduccionIngles("FormMainShell.Menu.Products"));
                Window productos = App.EsperarVentana("FormGestionProductos");

                string tituloIngles = TraduccionIngles("FormProductManagement.TitleLabel");
                Assert.AreNotEqual(
                    TestDatabase.ObtenerTraduccion("FormProductManagement.TitleLabel"),
                    tituloIngles,
                    "Precondicion: el titulo en ingles debe diferir del espanol.");
                Assert.AreEqual(tituloIngles, UiFind.Elemento(productos, "lblTituloProductos").Name, "El titulo no se tradujo al ingles.");
                Assert.AreEqual(
                    TraduccionIngles("FormProductManagement.AddButton"),
                    UiFind.Elemento(productos, "btnAgregarProducto").Name,
                    "El boton Agregar no se tradujo al ingles.");
            }
            finally
            {
                RestaurarPreferenciaIdioma(previa);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_Eliminar_ProductoSinUso_QuitaProducto()
        {
            string nombre = NombreUnico("uidel");
            InsertarProducto(nombre);

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window productos = AbrirGestionProductos(principal);

            SeleccionarFilaYEsperarNombre(productos, nombre);
            UiFind.Click(productos, "btnEliminarProducto");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormProductManagement.Success.ProductoDeleted");

            Assert.AreEqual(0, ContarProductos(nombre), "El producto eliminado no deberia seguir en el catalogo.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_Eliminar_ProductoVendidoPorUnComercio_InformaEnUsoYNoElimina()
        {
            string nombre = NombreUnico("uiuso");
            int productoId = InsertarProducto(nombre);
            AsignarProductoAComercio(InsertarComercio(NombreUnico("uicom")), productoId, 10m);

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window productos = AbrirGestionProductos(principal);

            SeleccionarFilaYEsperarNombre(productos, nombre);
            UiFind.Click(productos, "btnEliminarProducto");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("Error.MerchantManagement.ProductoInUse");

            Assert.AreEqual(1, ContarProductos(nombre), "Un producto que algun comercio vende no se puede eliminar.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_UsuarioBasico_MenuOperadorDeshabilitado()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);

            Assert.IsFalse(
                UiFind.MenuTopHabilitado(principal, TextoMenuOperador),
                "El menu Operador no deberia estar habilitado para un usuario sin permisos de operador.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_DetalleComercio_AsignarProductoDelCatalogo_PersisteConPrecioYMoneda()
        {
            string nombreProducto = NombreUnico("uiasig");
            int productoId = InsertarProducto(nombreProducto);
            string nombreComercio = NombreUnico("uicom");
            int comercioId = InsertarComercio(nombreComercio);
            string moneda = Convert.ToString(TestDatabase.EscalarOrNull("SELECT TOP 1 Nombre FROM dbo.Moneda WHERE Codigo = 'ARS'"));

            Window detalle = AbrirDetalleComercio(nombreComercio);

            UiFind.SeleccionarItemCombo(detalle, "cmbProductoCatalogo", nombreProducto);
            UiFind.EscribirTexto(detalle, "txtbPrecioProducto", "150.5");
            UiFind.SeleccionarItemCombo(detalle, "cmbMonedaProducto", moneda);
            UiFind.Click(detalle, "btnAgregarProducto");
            EsperarAvisoYCerrar("FormMerchantDetail.Success.ProductoAssigned");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.ComercioProducto cp JOIN dbo.Moneda m ON m.Id = cp.MonedaId " +
                    "WHERE cp.ComercioId = @c AND cp.ProductoId = @p AND cp.Precio = 150.50 AND m.Codigo = 'ARS'",
                    new SqlParameter("@c", comercioId),
                    new SqlParameter("@p", productoId)),
                "El producto no quedo asignado al comercio con el precio y la moneda elegidos.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_DetalleComercio_QuitarProducto_ConservaElProductoEnElCatalogo()
        {
            string nombreProducto = NombreUnico("uiquit");
            int productoId = InsertarProducto(nombreProducto);
            string nombreComercio = NombreUnico("uicom");
            int comercioId = InsertarComercio(nombreComercio);
            AsignarProductoAComercio(comercioId, productoId, 20m);

            Window detalle = AbrirDetalleComercio(nombreComercio);

            UiFind.ClickFilaGrilla(detalle, "dgvProductos", nombreProducto);
            UiFind.Click(detalle, "btnQuitarProducto");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormMerchantDetail.Success.ProductoRemoved");

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.ComercioProducto WHERE ComercioId = @c AND ProductoId = @p",
                    new SqlParameter("@c", comercioId),
                    new SqlParameter("@p", productoId)),
                "La asignacion no deberia seguir existiendo.");
            Assert.AreEqual(1, ContarProductos(nombreProducto), "Quitar el producto del comercio no debe eliminarlo del catalogo.");
        }

        // --- Navegacion ---
        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_ConCategoria_OcultaOrientacion()
        {
            InsertarCategoriaProducto(NombreUnico("uihint"));
            Window productos = AbrirGestionProductos(LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword));
            AutomationElement ayuda = productos.FindFirstDescendant(cf => cf.ByAutomationId("lblSinCategoriasProducto"));
            Assert.IsTrue(ayuda == null || ayuda.IsOffscreen, "La ayuda debe ocultarse si existen categorias.");
            Assert.IsTrue(UiFind.Elemento(productos, "btnAgregarProducto").IsEnabled);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_DetalleComercio_SinABMCategorias_MuestraProductosYPromociones()
        {
            string nombre = NombreUnico("uidet");
            InsertarComercio(nombre);
            Window detalle = AbrirDetalleComercio(nombre);
            Assert.IsNull(detalle.FindFirstDescendant(cf => cf.ByAutomationId("dgvCategoriasProducto")));
            Assert.IsNull(detalle.FindFirstDescendant(cf => cf.ByAutomationId("btnAgregarCategoriaProducto")));
            Assert.IsNotNull(UiFind.Elemento(detalle, "dgvProductos"));
            Assert.IsNotNull(UiFind.Elemento(detalle, "dgvPromociones"));
            TestContext.AddResultFile(App.CapturarPantalla("detalle-comercio-sin-categorias"));
        }


        private Window AbrirGestionProductos(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Products"));
            return App.EsperarVentana("FormGestionProductos");
        }

        private Window AbrirDetalleComercio(string nombreComercio)
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Merchants"));
            Window comercios = App.EsperarVentana("FormGestionComercios");

            UiFind.ClickFilaGrilla(comercios, "dgvComercios", nombreComercio);
            UiFind.Click(comercios, "btnVerDetalleComercio");
            return App.EsperarVentana("FormDetalleComercio");
        }

        /// <summary>
        /// Click de mouse real sobre la fila y espera a que el formulario copie el nombre al
        /// textbox (CellClick): evita operar antes de que la seleccion se refleje.
        /// </summary>
        private static void SeleccionarFilaYEsperarNombre(Window productos, string nombre)
        {
            UiFind.ClickFilaGrilla(productos, "dgvProductos", nombre);

            RetryResult<bool> reflejada = Retry.WhileFalse(
                () => UiFind.LeerTexto(productos, "txtbNombreProducto") == nombre,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(200));

            Assert.IsTrue(reflejada.Success, "La seleccion de '" + nombre + "' no se reflejo en el formulario.");
        }

        // --- Datos previos ---

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

        private static int InsertarCategoriaProducto(string nombre)
        {
            TestDatabase.EjecutarSql("INSERT INTO dbo.CategoriaProducto (Nombre) VALUES (@n)", new SqlParameter("@n", nombre));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.CategoriaProducto WHERE Nombre = @n", new SqlParameter("@n", nombre)));
        }

        private static int InsertarProducto(string nombre)
        {
            int categoriaId = InsertarCategoriaProducto("cat-" + nombre);
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Producto (CategoriaProductoId, Nombre) VALUES (@c, @n)",
                new SqlParameter("@c", categoriaId),
                new SqlParameter("@n", nombre));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.Producto WHERE Nombre = @n", new SqlParameter("@n", nombre)));
        }

        private static int InsertarComercio(string nombre)
        {
            TestDatabase.EjecutarSql("INSERT INTO dbo.RubroComercio (Nombre) VALUES (@r)", new SqlParameter("@r", "rubro-" + nombre));
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Comercio (RubroComercioId, Nombre) SELECT Id, @n FROM dbo.RubroComercio WHERE Nombre = @r",
                new SqlParameter("@n", nombre),
                new SqlParameter("@r", "rubro-" + nombre));
            return Convert.ToInt32(TestDatabase.EscalarOrNull("SELECT Id FROM dbo.Comercio WHERE Nombre = @n", new SqlParameter("@n", nombre)));
        }

        private static void AsignarProductoAComercio(int comercioId, int productoId, decimal precio)
        {
            int filas = TestDatabase.EjecutarSql(
                "INSERT INTO dbo.ComercioProducto (ComercioId, ProductoId, MonedaId, Precio) " +
                "SELECT @c, @p, Id, @precio FROM dbo.Moneda WHERE Codigo = 'ARS'",
                new SqlParameter("@c", comercioId),
                new SqlParameter("@p", productoId),
                new SqlParameter("@precio", precio));

            Assert.AreEqual(1, filas, "No se pudo armar el dato previo: producto asignado al comercio.");
        }

        private static int ContarProductos(string nombre)
        {
            return TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Producto WHERE Nombre = @n", new SqlParameter("@n", nombre));
        }

        private static string NombreUnico(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
