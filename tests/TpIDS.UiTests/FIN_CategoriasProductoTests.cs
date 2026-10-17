using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    [TestClass]
    public sealed class FIN_CategoriasProductoTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_Alta_PersisteYListaCategoria()
        {
            string nombre = NombreUnico();
            Window categorias = AbrirCategorias();
            UiFind.EscribirTexto(categorias, "txtbNombreCategoriaProducto", nombre);
            UiFind.Click(categorias, "btnAgregarCategoriaProducto");
            EsperarAvisoYCerrar("FormProductCategoryManagement.Success.CategoriaCreated");
            Assert.AreEqual(1, Contar(nombre));
            Seleccionar(categorias, nombre);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_Editar_PersisteNombreNuevo()
        {
            string nombre = NombreUnico();
            Insertar(nombre);
            Window categorias = AbrirCategorias();
            Seleccionar(categorias, nombre);
            UiFind.EscribirTexto(categorias, "txtbNombreCategoriaProducto", nombre + "editado");
            UiFind.Click(categorias, "btnActualizarCategoriaProducto");
            EsperarAvisoYCerrar("FormProductCategoryManagement.Success.CategoriaUpdated");
            Assert.AreEqual(0, Contar(nombre));
            Assert.AreEqual(1, Contar(nombre + "editado"));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_AltaDuplicada_InformaYNoInserta()
        {
            string nombre = NombreUnico();
            Insertar(nombre);
            Window categorias = AbrirCategorias();
            UiFind.EscribirTexto(categorias, "txtbNombreCategoriaProducto", "  " + nombre.ToUpperInvariant() + "  ");
            UiFind.Click(categorias, "btnAgregarCategoriaProducto");
            EsperarAvisoYCerrar("Error.MerchantManagement.CategoriaProductoDuplicateName");
            Assert.AreEqual(1, Contar(nombre));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_NombreVacio_InformaObligatorio()
        {
            Window categorias = AbrirCategorias();
            UiFind.EscribirTexto(categorias, "txtbNombreCategoriaProducto", "   ");
            UiFind.Click(categorias, "btnAgregarCategoriaProducto");
            EsperarAvisoYCerrar("Error.MerchantManagement.CategoriaProductoNameRequired");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_EliminarSinUso_QuitaCategoria()
        {
            string nombre = NombreUnico();
            Insertar(nombre);
            Window categorias = AbrirCategorias();
            Seleccionar(categorias, nombre);
            UiFind.Click(categorias, "btnEliminarCategoriaProducto");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormProductCategoryManagement.Success.CategoriaDeleted");
            Assert.AreEqual(0, Contar(nombre));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_EliminarConProducto_InformaEnUso()
        {
            string nombre = NombreUnico();
            int id = Insertar(nombre);
            TestDatabase.EjecutarSql("INSERT INTO dbo.Producto (Nombre, CategoriaProductoId) VALUES (@n, @c)",
                new SqlParameter("@n", nombre + "producto"), new SqlParameter("@c", id));
            Window categorias = AbrirCategorias();
            Seleccionar(categorias, nombre);
            UiFind.Click(categorias, "btnEliminarCategoriaProducto");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("Error.MerchantManagement.CategoriaProductoInUse");
            Assert.AreEqual(1, Contar(nombre));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_UsuarioBasico_MenuDeshabilitado()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Assert.IsFalse(UiFind.MenuTopHabilitado(principal, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator")));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_Textos_TraducidosEnEspanol()
        {
            Window categorias = AbrirCategorias();
            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormProductCategoryManagement.TitleLabel"),
                UiFind.Elemento(categorias, "lblTituloCategoriasProducto").Name);
            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormProductCategoryManagement.AddButton"),
                UiFind.Elemento(categorias, "btnAgregarCategoriaProducto").Name);
            TestContext.AddResultFile(App.CapturarPantalla("categorias-producto"));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CategoriasProducto_CambiarIdioma_TraducePantallaAbierta()
        {
            object previa = TestDatabase.EscalarOrNull("SELECT up.IdiomaId FROM dbo.UsuarioPreferencias up JOIN dbo.Usuarios u ON u.Id = up.UsuarioId WHERE u.Username = @u",
                new SqlParameter("@u", TestConfig.AdminUsername));
            try
            {
                Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
                Window categorias = AbrirCategorias(principal);
                Window configuracion = AbrirConfiguracion(principal);
                UiFind.SeleccionarItemCombo(configuracion, "cmbIdiomas", "English");
                UiFind.Click(configuracion, "btnGuardarConfiguracion");
                App.EsperarMessageBox().CerrarConOk();
                configuracion.Close();
                Assert.AreEqual(Ingles("FormProductCategoryManagement.TitleLabel"), UiFind.Elemento(categorias, "lblTituloCategoriasProducto").Name);
                Assert.AreEqual(Ingles("FormProductCategoryManagement.DeleteButton"), UiFind.Elemento(categorias, "btnEliminarCategoriaProducto").Name);
            }
            finally
            {
                if (previa == null)
                {
                    TestDatabase.EjecutarSql("DELETE up FROM dbo.UsuarioPreferencias up JOIN dbo.Usuarios u ON u.Id = up.UsuarioId WHERE u.Username = @u",
                        new SqlParameter("@u", TestConfig.AdminUsername));
                }
                else
                {
                    TestDatabase.EjecutarSql("UPDATE up SET IdiomaId = @i FROM dbo.UsuarioPreferencias up JOIN dbo.Usuarios u ON u.Id = up.UsuarioId WHERE u.Username = @u",
                        new SqlParameter("@i", previa), new SqlParameter("@u", TestConfig.AdminUsername));
                }
            }
        }

        private Window AbrirCategorias()
        {
            return AbrirCategorias(LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword));
        }

        private Window AbrirCategorias(Window principal)
        {
            UiFind.ClickMenu(principal, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator"),
                TestDatabase.ObtenerTraduccion("FormMainShell.Menu.ProductCategories"));
            return App.EsperarVentana("FormGestionCategoriasProducto");
        }

        private static void Seleccionar(Window categorias, string nombre)
        {
            UiFind.ClickFilaGrilla(categorias, "dgvCategoriasProducto", nombre);
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.LeerTexto(categorias, "txtbNombreCategoriaProducto") == nombre,
                TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(200)).Success);
        }

        private static int Insertar(string nombre)
        {
            return Convert.ToInt32(TestDatabase.EscalarOrNull("INSERT INTO dbo.CategoriaProducto (Nombre) VALUES (@n); SELECT CAST(SCOPE_IDENTITY() AS INT)", new SqlParameter("@n", nombre)));
        }

        private static int Contar(string nombre)
        {
            return TestDatabase.Contar("SELECT COUNT(*) FROM dbo.CategoriaProducto WHERE Nombre = @n", new SqlParameter("@n", nombre));
        }

        private static string Ingles(string key)
        {
            return Convert.ToString(TestDatabase.EscalarOrNull("SELECT t.Valor FROM dbo.Traduccion t JOIN dbo.Palabra p ON p.Id = t.PalabraId JOIN dbo.Idioma i ON i.Id = t.IdiomaId WHERE p.[Key] = @k AND i.Nombre = 'English'", new SqlParameter("@k", key)));
        }

        private static string NombreUnico()
        {
            return "uicat" + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
