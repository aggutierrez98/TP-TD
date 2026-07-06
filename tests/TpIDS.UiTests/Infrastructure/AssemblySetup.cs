using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TpIDS.UiTests.Infrastructure
{
    [TestClass]
    public class AssemblySetup
    {
        /// <summary>
        /// Una sola vez por corrida: stagea la app productiva con config apuntado a la BD
        /// de test y siembra credenciales conocidas mientras la BD esta virgen (sin firmas).
        /// Requiere haber corrido tests\tools\prepare-test-database.ps1 antes.
        /// </summary>
        [AssemblyInitialize]
        public static void Inicializar(TestContext context)
        {
            AppUnderTestStager.Stage();
            AppUnderTestStager.SembrarCredencialesConocidas();
        }
    }
}
