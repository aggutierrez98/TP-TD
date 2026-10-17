using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    [TestClass]
    public sealed class FIN_ProductosSinCategoriasTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Productos_SinCategorias_OrientaYDeshabilitaAlta()
        {
            // Correr esta clase sola con la base de test recien regenerada.
            if (TestDatabase.Contar("SELECT COUNT(*) FROM dbo.CategoriaProducto") != 0)
            {
                Assert.Inconclusive("Este escenario requiere la base regenerada sin categorias de producto.");
            }
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator"), TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Products"));
            Window productos = App.EsperarVentana("FormGestionProductos");
            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormProductManagement.NoCategoriesHint"), UiFind.Elemento(productos, "lblSinCategoriasProducto").Name);
            Assert.IsFalse(UiFind.Elemento(productos, "btnAgregarProducto").IsEnabled);
            TestContext.AddResultFile(App.CapturarPantalla("productos-sin-categorias"));
        }
    }
}
