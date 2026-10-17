using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// NEG02 - Gestion de categorias (Finanzas &gt; Categorias). El usuario basico administra sus
    /// categorias propias; las globales (UsuarioId NULL) se ven pero no se editan ni se eliminan.
    /// La tabla Categoria no tiene firmas DVH/DVV, por eso los datos previos se arman por SQL directo.
    /// </summary>
    [TestClass]
    public class NEG02_CategoriasTests : UiTestBase
    {
        private static readonly string TextoMenuFinanzas = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Finance");

        [TestMethod]
        [TestCategory("UI")]
        public void NEG02_Categorias_Crear_PersisteCategoriaPropia()
        {
            string nombre = NombreUnico("uicat");

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window categorias = AbrirGestionCategorias(principal);

            UiFind.EscribirTexto(categorias, "txtbNombreCategoria", nombre);
            UiFind.Click(categorias, "btnAgregarCategoria");
            EsperarAvisoYCerrar("FormCategories.Success.CategoryCreated");

            Assert.AreEqual(
                1,
                ContarCategoriasPropias(TestConfig.BasicUsername, nombre),
                "La categoria nueva no quedo persistida como propia del usuario.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void NEG02_Categorias_NombreVacio_InformaErrorYNoInserta()
        {
            int categoriasPrevias = TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Categoria");

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window categorias = AbrirGestionCategorias(principal);

            UiFind.EscribirTexto(categorias, "txtbNombreCategoria", string.Empty);
            UiFind.Click(categorias, "btnAgregarCategoria");
            EsperarAvisoYCerrar("Error.Category.NameRequired");

            Assert.AreEqual(
                categoriasPrevias,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Categoria"),
                "Un nombre vacio no debe insertar una categoria.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void NEG02_Categorias_NombreDuplicado_InformaErrorYNoInserta()
        {
            string nombre = NombreUnico("uidup");
            InsertarCategoriaPropia(TestConfig.BasicUsername, nombre);

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window categorias = AbrirGestionCategorias(principal);

            UiFind.EscribirTexto(categorias, "txtbNombreCategoria", nombre);
            UiFind.Click(categorias, "btnAgregarCategoria");
            EsperarAvisoYCerrar("Error.Category.NameExists");

            Assert.AreEqual(
                1,
                ContarCategoriasPropias(TestConfig.BasicUsername, nombre),
                "No debe insertarse una segunda categoria propia con el mismo nombre.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void NEG02_Categorias_Actualizar_PersisteNuevoNombre()
        {
            string nombre = NombreUnico("uiold");
            string nombreNuevo = NombreUnico("uinew");
            InsertarCategoriaPropia(TestConfig.BasicUsername, nombre);

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window categorias = AbrirGestionCategorias(principal);

            SeleccionarFilaYEsperarNombre(categorias, nombre);
            UiFind.EscribirTexto(categorias, "txtbNombreCategoria", nombreNuevo);
            UiFind.Click(categorias, "btnActualizarCategoria");
            EsperarAvisoYCerrar("FormCategories.Success.CategoryUpdated");

            Assert.AreEqual(
                1,
                ContarCategoriasPropias(TestConfig.BasicUsername, nombreNuevo),
                "La categoria no quedo con el nombre nuevo.");
            Assert.AreEqual(
                0,
                ContarCategoriasPropias(TestConfig.BasicUsername, nombre),
                "El nombre anterior no deberia seguir existiendo.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void NEG02_Categorias_Eliminar_QuitaCategoria()
        {
            string nombre = NombreUnico("uidel");
            InsertarCategoriaPropia(TestConfig.BasicUsername, nombre);

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window categorias = AbrirGestionCategorias(principal);

            SeleccionarFilaYEsperarNombre(categorias, nombre);
            UiFind.Click(categorias, "btnEliminarCategoria");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormCategories.Success.CategoryDeleted");

            Assert.AreEqual(
                0,
                ContarCategoriasPropias(TestConfig.BasicUsername, nombre),
                "La categoria eliminada no deberia seguir en la tabla.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void NEG02_Categorias_EliminarGlobal_InformaAvisoYNoElimina()
        {
            string nombreGlobal = Convert.ToString(TestDatabase.EscalarOrNull(
                "SELECT TOP 1 Nombre FROM dbo.Categoria WHERE UsuarioId IS NULL ORDER BY Id"));
            Assert.IsFalse(string.IsNullOrEmpty(nombreGlobal), "La BD de test no tiene categorias globales sembradas.");
            int globalesPrevias = TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Categoria WHERE UsuarioId IS NULL");

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window categorias = AbrirGestionCategorias(principal);

            UiFind.ClickFilaGrilla(categorias, "dgvCategorias", nombreGlobal);
            UiFind.Click(categorias, "btnEliminarCategoria");
            EsperarAvisoYCerrar("FormCategories.Warning.CannotEditGlobal");

            Assert.AreEqual(
                globalesPrevias,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Categoria WHERE UsuarioId IS NULL"),
                "Una categoria global no debe poder eliminarse.");
        }

        // --- Navegacion ---

        private Window AbrirGestionCategorias(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuFinanzas, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Categories"));
            return App.EsperarVentana("FormGestionCategorias");
        }

        /// <summary>
        /// Click de mouse real sobre la fila y espera a que el formulario copie el nombre al
        /// textbox (CellClick): evita editar antes de que la seleccion se refleje.
        /// </summary>
        private static void SeleccionarFilaYEsperarNombre(Window categorias, string nombre)
        {
            UiFind.ClickFilaGrilla(categorias, "dgvCategorias", nombre);

            RetryResult<bool> reflejada = Retry.WhileFalse(
                () => UiFind.LeerTexto(categorias, "txtbNombreCategoria") == nombre,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(200));

            Assert.IsTrue(reflejada.Success, "La seleccion de '" + nombre + "' no se reflejo en el formulario.");
        }

        // --- Datos previos y asserts de BD ---

        private static void InsertarCategoriaPropia(string username, string nombre)
        {
            int filas = TestDatabase.EjecutarSql(
                "INSERT INTO dbo.Categoria (Nombre, UsuarioId) SELECT @n, u.Id FROM dbo.Usuarios u WHERE u.Username = @u",
                new SqlParameter("@n", nombre),
                new SqlParameter("@u", username));

            Assert.AreEqual(1, filas, "No se pudo armar el dato previo: categoria propia de '" + username + "'.");
        }

        private static int ContarCategoriasPropias(string username, string nombre)
        {
            return TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.Categoria c JOIN dbo.Usuarios u ON u.Id = c.UsuarioId " +
                "WHERE u.Username = @u AND c.Nombre = @n",
                new SqlParameter("@u", username),
                new SqlParameter("@n", nombre));
        }

        private static string NombreUnico(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
