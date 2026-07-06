using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Ola 5 - T07 Digitos verificadores (tests destructivos).
    /// Trazabilidad: diagramas/T07-digitos-verificadores.
    /// Corrompen firmas por SQL directo (unica excepcion permitida a la regla de
    /// "escrituras solo por UI") y las RESTAURAN al terminar, dejando la BD consistente.
    /// El caso feliz de DVV incremental ya se cubre en T02_CU02 (alta) y T04 (borrado).
    /// </summary>
    [TestClass]
    public class T07_IntegridadTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void T07_CorrupcionHorizontal_BloqueaArranqueConContingencia()
        {
            InicializarFirmasConUnArranque();

            string dvhOriginal = TestDatabase.EscalarOrNull(
                "SELECT DVH FROM dbo.Usuarios WHERE Username = @u",
                new SqlParameter("@u", TestConfig.BasicUsername)) as string;
            Assert.IsFalse(string.IsNullOrEmpty(dvhOriginal), "El DVH deberia estar inicializado antes de corromperlo.");

            try
            {
                TestDatabase.EjecutarSql(
                    "UPDATE dbo.Usuarios SET DVH = 'UITEST-CORRUPTO' WHERE Username = @u",
                    new SqlParameter("@u", TestConfig.BasicUsername));

                Window contingencia = App.EsperarVentana("FormContingenciaIntegridad", TimeSpan.FromSeconds(30));
                Assert.AreEqual(
                    "Usuarios",
                    UiFind.Elemento(contingencia, "lblTablaValor").Name,
                    "La contingencia deberia informar la tabla afectada.");
            }
            finally
            {
                TestDatabase.EjecutarSql(
                    "UPDATE dbo.Usuarios SET DVH = @dvh WHERE Username = @u",
                    new SqlParameter("@dvh", dvhOriginal ?? (object)DBNull.Value),
                    new SqlParameter("@u", TestConfig.BasicUsername));
            }

            // Con la firma restaurada, el sistema debe volver a arrancar normalmente.
            using (UiAppSession postRestauracion = new UiAppSession())
            {
                Assert.IsNotNull(
                    postRestauracion.EsperarVentana("FormLogin", TimeSpan.FromSeconds(30)),
                    "Restaurada la firma, el arranque deberia llegar al login.");
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T07_CorrupcionVertical_BloqueaArranqueConContingencia()
        {
            InicializarFirmasConUnArranque();

            string dvvOriginal = TestDatabase.ObtenerDvv("Usuarios");
            Assert.IsFalse(string.IsNullOrEmpty(dvvOriginal), "El DVV deberia estar inicializado antes de corromperlo.");

            try
            {
                TestDatabase.EjecutarSql(
                    "UPDATE dbo.DigitoVerificadorVertical SET Valor = 'UITEST-CORRUPTO' WHERE NombreTabla = 'Usuarios'");

                Window contingencia = App.EsperarVentana("FormContingenciaIntegridad", TimeSpan.FromSeconds(30));
                Assert.AreEqual(
                    "Usuarios",
                    UiFind.Elemento(contingencia, "lblTablaValor").Name,
                    "La contingencia deberia informar la tabla afectada.");
            }
            finally
            {
                TestDatabase.EjecutarSql(
                    "UPDATE dbo.DigitoVerificadorVertical SET Valor = @dvv WHERE NombreTabla = 'Usuarios'",
                    new SqlParameter("@dvv", dvvOriginal ?? (object)DBNull.Value));
            }

            using (UiAppSession postRestauracion = new UiAppSession())
            {
                Assert.IsNotNull(
                    postRestauracion.EsperarVentana("FormLogin", TimeSpan.FromSeconds(30)),
                    "Restaurada la firma, el arranque deberia llegar al login.");
            }
        }

        /// <summary>
        /// Garantiza que las firmas DVH/DVV existan: sobre BD virgen se inicializan
        /// en el primer arranque de la app (ValidarIntegridadGlobal).
        /// </summary>
        private static void InicializarFirmasConUnArranque()
        {
            if (TestDatabase.ObtenerDvv("Usuarios") != null)
            {
                return;
            }

            using (UiAppSession arranqueInicial = new UiAppSession())
            {
                arranqueInicial.EsperarVentana("FormLogin", TimeSpan.FromSeconds(30));
            }
        }
    }
}
