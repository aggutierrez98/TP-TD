using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Ola 3 - T05 Gestion de Multiples Idiomas.
    /// Trazabilidad: diagramas/T05-multidioma (CU01 Agregar idioma, CU02 Actualizar idioma,
    /// CU03 Eliminar idioma, CU06 Editar traduccion, CU07 Configurar idioma preferido).
    /// CU04/CU05 (import/export JSON) usan dialogos nativos de archivo y estan cubiertos
    /// por BLL.Tests (ImportarJson/ExportarJson); no se automatizan por UI.
    /// </summary>
    [TestClass]
    public class T05_MultidiomaTests : UiTestBase
    {
        private const string IdiomaDefault = "Español";

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU01_CU02_CrearYRenombrarIdioma_PersisteYAudita()
        {
            string nombre = NombreUnico("uitest_idioma");
            string renombrado = nombre + "_ren";
            int altasPrevias = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.LanguageCreation", null);

            Window idiomas = AbrirGestionIdiomas();

            UiFind.EscribirTexto(idiomas, "txtbNombreIdioma", nombre);
            UiFind.Click(idiomas, "btnAgregarIdioma");
            MessageBoxWindow alta = App.EsperarMessageBox();
            StringAssert.Contains(
                alta.Texto,
                TestDatabase.ObtenerTraduccion("FormLanguageManagement.Success.LanguageCreated"),
                "Deberia confirmarse el alta del idioma.");
            alta.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n AND [Default] = 0",
                    new SqlParameter("@n", nombre)),
                "El idioma nuevo deberia persistirse como no predeterminado.");
            Assert.AreEqual(
                altasPrevias + 1,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.LanguageCreation", null),
                "El alta de idioma no quedo en la bitacora.");

            // CU02: el idioma recien creado queda seleccionado; se renombra y actualiza.
            UiFind.EscribirTexto(idiomas, "txtbNombreIdioma", renombrado);
            UiFind.Click(idiomas, "btnActualizarIdioma");
            MessageBoxWindow actualizacion = App.EsperarMessageBox();
            StringAssert.Contains(
                actualizacion.Texto,
                TestDatabase.ObtenerTraduccion("FormLanguageManagement.Success.LanguageUpdated"),
                "Deberia confirmarse la actualizacion del idioma.");
            actualizacion.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n",
                    new SqlParameter("@n", renombrado)),
                "El idioma deberia quedar con el nombre actualizado.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU01_IdiomaDuplicado_InformaError()
        {
            Window idiomas = AbrirGestionIdiomas();

            UiFind.EscribirTexto(idiomas, "txtbNombreIdioma", IdiomaDefault);
            UiFind.Click(idiomas, "btnAgregarIdioma");

            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("Error.Language.NameAlreadyExists"),
                "Deberia informarse que el idioma ya existe.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n",
                    new SqlParameter("@n", IdiomaDefault)),
                "No debe duplicarse el idioma default.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU03_EliminarIdioma_YProtegerDefault()
        {
            string nombre = NombreUnico("uitest_borrar");

            Window idiomas = AbrirGestionIdiomas();
            CrearIdiomaDesdeGestion(idiomas, nombre);

            // El idioma recien creado queda seleccionado: se elimina con confirmacion.
            UiFind.Click(idiomas, "btnEliminarIdioma");
            App.EsperarMessageBox().ConfirmarSi();
            MessageBoxWindow baja = App.EsperarMessageBox();
            StringAssert.Contains(
                baja.Texto,
                TestDatabase.ObtenerTraduccion("FormLanguageManagement.Success.LanguageDeleted"),
                "Deberia confirmarse la baja del idioma.");
            baja.CerrarConOk();

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n",
                    new SqlParameter("@n", nombre)),
                "El idioma eliminado no deberia seguir en la tabla.");

            // Flujo alternativo 5a: el idioma default no puede eliminarse.
            UiFind.SeleccionarItemCombo(idiomas, "cmbIdiomas", IdiomaDefault);
            UiFind.Click(idiomas, "btnEliminarIdioma");
            App.EsperarMessageBox().ConfirmarSi();
            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("Error.Language.CannotDeleteDefaultLanguage"),
                "Deberia informarse que el idioma default no puede eliminarse.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n",
                    new SqlParameter("@n", IdiomaDefault)),
                "El idioma default debe seguir existiendo.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU06_GuardarTraduccion_PersisteValorYAudita()
        {
            int traduccionesPrevias = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.TranslationUpdate", null);

            // txtbKeyManual es ReadOnly: la key se elige desde la grilla. Al abrir la
            // gestion con el idioma default, la primera traduccion queda cargada en los
            // campos manuales; se edita su valor y despues se restaura el original.
            Window idiomas = AbrirGestionIdiomas();

            string key = UiFind.LeerTexto(idiomas, "txtbKeyManual");
            string original = UiFind.LeerTexto(idiomas, "txtbValorManual");
            Assert.IsFalse(string.IsNullOrWhiteSpace(key), "La grilla deberia precargar una key en los campos manuales.");

            string editado = original + " uitest";
            UiFind.EscribirTexto(idiomas, "txtbValorManual", editado);
            UiFind.Click(idiomas, "btnGuardarTraduccion");

            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            StringAssert.Contains(
                confirmacion.Texto,
                TestDatabase.ObtenerTraduccion("FormLanguageManagement.Success.TranslationSaved"),
                "Deberia confirmarse el guardado de la traduccion.");
            confirmacion.CerrarConOk();

            Assert.AreEqual(
                editado,
                LeerTraduccionDeDb(key),
                "La traduccion editada no quedo persistida.");

            // Se restaura el valor original para no alterar el resto de la suite.
            UiFind.EscribirTexto(idiomas, "txtbValorManual", original);
            UiFind.Click(idiomas, "btnGuardarTraduccion");
            App.EsperarMessageBox().CerrarConOk();

            Assert.AreEqual(
                original,
                LeerTraduccionDeDb(key),
                "La traduccion deberia volver a su valor original.");

            Assert.AreEqual(
                traduccionesPrevias + 2,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.TranslationUpdate", null),
                "Cada guardado de traduccion deberia quedar en la bitacora.");
        }

        private string LeerTraduccionDeDb(string key)
        {
            return TestDatabase.EscalarOrNull(
                "SELECT t.Valor FROM dbo.Traduccion t " +
                "JOIN dbo.Palabra p ON p.Id = t.PalabraId " +
                "JOIN dbo.Idioma i ON i.Id = t.IdiomaId " +
                "WHERE i.Nombre = @n AND p.[Key] = @k",
                new SqlParameter("@n", IdiomaDefault),
                new SqlParameter("@k", key)) as string;
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU07_IdiomaIncompleto_NoEsSeleccionable()
        {
            string nombre = NombreUnico("uitest_inc");

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnGestionIdiomas");
            Window idiomas = App.EsperarVentana("FormGestionIdiomas");
            CrearIdiomaDesdeGestion(idiomas, nombre);
            idiomas.Close();

            UiFind.Click(gestion, "btnConfiguracion");
            Window configuracion = App.EsperarVentana("FormConfiguracionUsuario");
            UiFind.SeleccionarItemCombo(configuracion, "cmbIdiomas", nombre);
            UiFind.Click(configuracion, "btnGuardarConfiguracion");

            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("Error.Language.IncompleteSelection"),
                "Un idioma sin traducciones completas no debe ser seleccionable como preferencia.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.UsuarioPreferencias up " +
                    "JOIN dbo.Usuarios u ON u.Id = up.UsuarioId " +
                    "JOIN dbo.Idioma i ON i.Id = up.IdiomaId " +
                    "WHERE u.Username = @u AND i.Nombre = @n",
                    new SqlParameter("@u", TestConfig.AdminUsername),
                    new SqlParameter("@n", nombre)),
                "No debe guardarse la preferencia hacia un idioma incompleto.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU07_GuardarPreferencia_PersisteHistorialYBitacora()
        {
            int preferenciasPrevias = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.LanguagePreferenceUpdate", null);

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnConfiguracion");
            Window configuracion = App.EsperarVentana("FormConfiguracionUsuario");

            UiFind.SeleccionarItemCombo(configuracion, "cmbIdiomas", IdiomaDefault);
            UiFind.Click(configuracion, "btnGuardarConfiguracion");

            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            StringAssert.Contains(
                confirmacion.Texto,
                TestDatabase.ObtenerTraduccion("FormUserSettings.Success.SettingsSaved"),
                "Deberia confirmarse el guardado de la configuracion.");
            confirmacion.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.UsuarioPreferencias up " +
                    "JOIN dbo.Usuarios u ON u.Id = up.UsuarioId " +
                    "JOIN dbo.Idioma i ON i.Id = up.IdiomaId " +
                    "WHERE u.Username = @u AND i.Nombre = @n",
                    new SqlParameter("@u", TestConfig.AdminUsername),
                    new SqlParameter("@n", IdiomaDefault)),
                "La preferencia de idioma del usuario no quedo persistida.");

            Assert.AreEqual(
                preferenciasPrevias + 1,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.LanguagePreferenceUpdate", null),
                "El cambio de preferencia no quedo en la bitacora.");

            Assert.IsTrue(
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.UsuarioHistorial h " +
                    "JOIN dbo.Usuarios u ON u.Id = h.UsuarioId WHERE u.Username = @u",
                    new SqlParameter("@u", TestConfig.AdminUsername)) >= 1,
                "El cambio de preferencia deberia registrar una version en el historial.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU02_RenombrarAIdiomaExistente_InformaError()
        {
            string nombre = NombreUnico("uitest_ren");

            Window idiomas = AbrirGestionIdiomas();
            CrearIdiomaDesdeGestion(idiomas, nombre);

            // CU02 4a: el nombre nuevo colisiona con otro idioma existente.
            UiFind.EscribirTexto(idiomas, "txtbNombreIdioma", IdiomaDefault);
            UiFind.Click(idiomas, "btnActualizarIdioma");
            EsperarAvisoYCerrar("Error.Language.NameAlreadyExists");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n",
                    new SqlParameter("@n", nombre)),
                "El idioma debe conservar su nombre original si el rename colisiona.");
            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n",
                    new SqlParameter("@n", IdiomaDefault)),
                "No debe duplicarse el idioma default.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU03_EliminarNoConfirmado_MantieneIdioma()
        {
            Window idiomas = AbrirGestionIdiomas();

            // CU03 4a: el administrador no confirma la eliminacion (sobre el default).
            UiFind.SeleccionarItemCombo(idiomas, "cmbIdiomas", IdiomaDefault);
            UiFind.Click(idiomas, "btnEliminarIdioma");
            App.EsperarMessageBox().RechazarNo();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Idioma WHERE Nombre = @n",
                    new SqlParameter("@n", IdiomaDefault)),
                "El idioma debe seguir existiendo si no se confirma la eliminacion.");
            Assert.IsTrue(
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Traduccion t JOIN dbo.Idioma i ON i.Id = t.IdiomaId WHERE i.Nombre = @n",
                    new SqlParameter("@n", IdiomaDefault)) > 0,
                "Las traducciones del idioma deben seguir intactas.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T05_CU06_TraduccionVacia_InformaError()
        {
            Window idiomas = AbrirGestionIdiomas();

            string key = UiFind.LeerTexto(idiomas, "txtbKeyManual");
            string original = UiFind.LeerTexto(idiomas, "txtbValorManual");
            Assert.IsFalse(string.IsNullOrWhiteSpace(key), "La grilla deberia precargar una key.");

            // CU06 3a: valor vacio.
            UiFind.EscribirTexto(idiomas, "txtbValorManual", string.Empty);
            UiFind.Click(idiomas, "btnGuardarTraduccion");
            EsperarAvisoYCerrar("Error.Language.TranslationRequired");

            Assert.AreEqual(
                original,
                LeerTraduccionDeDb(key),
                "La traduccion no debe modificarse si el valor esta vacio.");
        }

        private Window AbrirGestionIdiomas()
        {
            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnGestionIdiomas");
            return App.EsperarVentana("FormGestionIdiomas");
        }

        private void CrearIdiomaDesdeGestion(Window idiomas, string nombre)
        {
            UiFind.EscribirTexto(idiomas, "txtbNombreIdioma", nombre);
            UiFind.Click(idiomas, "btnAgregarIdioma");
            App.EsperarMessageBox().CerrarConOk();
        }

        private static string NombreUnico(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
