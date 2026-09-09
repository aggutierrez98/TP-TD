using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// T08 Backup y restauracion de base de datos (smoke tests).
    /// Trazabilidad: diagramas/T08-backup (pantalla FormBackup dentro de Administracion).
    ///
    /// No se automatiza aqui ni la creacion ni la restauracion real de un backup:
    /// - Crear requiere manejar el SaveFileDialog nativo de Windows (fuera del arbol UIA de
    ///   la app), lo que agrega fragilidad sin cubrir logica adicional a la ya probada en
    ///   BackupDALIntegrationTests (CrearBackup_GeneraUnArchivoDeBackupValido).
    /// - Restaurar exige poner la base compartida de esta suite en SINGLE_USER WITH ROLLBACK
    ///   IMMEDIATE, lo que tira cualquier otra conexion activa (incluida la app bajo test y
    ///   cualquier otro test corriendo sobre la misma BD) y dejaria la suite completa
    ///   inconsistente. Mismo motivo que documenta
    ///   TpIDS.IntegrationTests/BackupDALIntegrationTests.cs para no automatizar
    ///   RestaurarBackup ahi tampoco.
    /// </summary>
    [TestClass]
    public class T08_BackupTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void T08_Admin_AbreBackupYVeLosControlesEsperados()
        {
            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window backup = AbrirBackup(principal);

            Assert.IsNotNull(UiFind.Elemento(backup, "txtbRutaCrearBackup"));
            Assert.IsFalse(UiFind.Boton(backup, "btnCrearBackup").IsEnabled, "Sin ruta seleccionada, crear backup debe estar deshabilitado.");

            Assert.IsNotNull(UiFind.Elemento(backup, "txtbRutaRestaurarBackup"));
            Assert.IsFalse(UiFind.Boton(backup, "btnRestaurarBackup").IsEnabled, "Sin archivo seleccionado, restaurar backup debe estar deshabilitado.");
        }

        // El item "Backup" cuelga de Administracion junto con Usuarios/Permisos/Idiomas/
        // Bitacora, y los cuatro se habilitan/deshabilitan con el mismo criterio en
        // FormPrincipal.AplicarPermisosMenu (OR de sus permisos individuales sobre el menu
        // padre). Igual que T04_CU03_UsuarioBasico_SoloVeAccionesPermitidas, verificar que el
        // menu padre este deshabilitado para basico alcanza: no tiene gestionusuarios,
        // gestionpermisos, gestionidiomas, verbitacora ni gestionbackup.
        [TestMethod]
        [TestCategory("UI")]
        public void T08_UsuarioBasico_MenuAdministracionDeshabilitado()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);

            Assert.IsFalse(
                UiFind.MenuTopHabilitado(principal, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Administration")),
                "basico no tiene gestionbackup (ni el resto de permisos de Administracion): el menu debe estar deshabilitado.");
        }
    }
}
