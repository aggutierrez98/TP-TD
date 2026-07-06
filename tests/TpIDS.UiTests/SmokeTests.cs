using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Ola 0: valida el harness completo (staging, lanzamiento, BD de test)
    /// antes de encarar los casos de uso reales.
    /// </summary>
    [TestClass]
    public class SmokeTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void T00_Smoke01_AppArrancaContraBdDeTestYMuestraLogin()
        {
            var login = EsperarLogin();

            Assert.IsNotNull(UiFind.CajaTexto(login, "txtbNickname"), "Falta txtbNickname en FormLogin.");
            Assert.IsNotNull(UiFind.CajaPassword(login, "txtbPassword"), "Falta txtbPassword en FormLogin.");
            Assert.IsNotNull(UiFind.Boton(login, "btnIngreso"), "Falta btnIngreso en FormLogin.");
            Assert.IsNotNull(UiFind.Boton(login, "btnRegistro"), "Falta btnRegistro en FormLogin.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T00_Smoke02_ArranqueInicializaFirmasDeIntegridad()
        {
            // T07: sobre BD virgen, el arranque (ValidarIntegridadGlobal) inicializa DVH y DVV.
            EsperarLogin();

            string dvv = TestDatabase.ObtenerDvv("Usuarios");
            Assert.IsFalse(string.IsNullOrWhiteSpace(dvv), "El arranque no inicializo el DVV de Usuarios.");

            int usuariosSinDvh = TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Usuarios WHERE DVH IS NULL");
            Assert.AreEqual(0, usuariosSinDvh, "Quedaron usuarios sin DVH despues del arranque.");
        }
    }
}
