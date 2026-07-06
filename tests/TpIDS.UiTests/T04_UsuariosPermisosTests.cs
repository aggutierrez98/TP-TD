using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Ola 2 - T04 Gestion de Perfiles de Usuario.
    /// Trazabilidad: diagramas/T04-perfiles-usuario (CU01 Gestionar permisos,
    /// CU02 Asignar perfil(es) a usuario, CU03 Controlar acceso de usuario).
    /// Incluye ademas alta/baja logica/borrado fisico desde FormGestionUsuarios.
    /// </summary>
    [TestClass]
    public class T04_UsuariosPermisosTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU01_CrearFamiliaConPatente_PersisteCatalogoYBitacora()
        {
            string familia = NombreUnico("uitestfam");
            int catalogosPrevios = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.PermissionCatalogUpdate", null);

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnGestionPermisos");
            Window permisos = App.EsperarVentana("FormGestionPermisos");

            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", familia);
            UiFind.Click(permisos, "btnCrearFamilia");
            UiFind.SeleccionarItemLista(permisos, "lstbPatentesDisponibles", "verbitacora");
            UiFind.Click(permisos, "btnAgregarPatenteHija");
            UiFind.Click(permisos, "btnGuardarFamilia");

            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            StringAssert.Contains(
                confirmacion.Texto,
                TestDatabase.ObtenerTraduccion("FormPermissionManagement.Success.CatalogSaved"),
                "Deberia confirmarse el guardado del catalogo.");
            confirmacion.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Permiso WHERE Nombre = @n AND EsGrupo = 1",
                    new SqlParameter("@n", familia)),
                "La familia nueva no quedo persistida como grupo.");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.PermisoCompuesto pc " +
                    "JOIN dbo.Permiso padre ON padre.Id = pc.PermisoPadreId " +
                    "JOIN dbo.Permiso hijo ON hijo.Id = pc.PermisoHijoId " +
                    "WHERE padre.Nombre = @n AND hijo.Nombre = 'verbitacora'",
                    new SqlParameter("@n", familia)),
                "No quedo la relacion familia -> patente en el catalogo.");

            Assert.AreEqual(
                catalogosPrevios + 1,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.PermissionCatalogUpdate", null),
                "La actualizacion del catalogo no quedo en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU01_FamiliaVacia_InformaErrorYNoPersiste()
        {
            string familia = NombreUnico("uitestvacia");

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnGestionPermisos");
            Window permisos = App.EsperarVentana("FormGestionPermisos");

            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", familia);
            UiFind.Click(permisos, "btnCrearFamilia");
            UiFind.Click(permisos, "btnGuardarFamilia");

            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("Error.Permission.EmptyGroupsNotAllowed"),
                "Deberia informarse que no se permiten grupos vacios.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Permiso WHERE Nombre = @n",
                    new SqlParameter("@n", familia)),
                "La familia vacia no debe persistirse.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU02_AsignarPermisoAUsuario_PersisteHistorialYBitacora()
        {
            string username = NombreUnico("uitest_perm");
            int asignacionesPrevias = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserPermissionsUpdate", null);

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            CrearUsuarioDesdeGestion(gestion, username);

            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.MarcarNodoArbol(gestion, "tvwPermisosUsuario", "verbitacora");
            UiFind.Click(gestion, "btnGuardarPermisosUsuario");

            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            StringAssert.Contains(
                confirmacion.Texto,
                TestDatabase.ObtenerTraduccion("FormUserManagement.Success.UserPermissionsSaved"),
                "Deberia confirmarse el guardado de permisos del usuario.");
            confirmacion.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.UsuarioPermiso up " +
                    "JOIN dbo.Usuarios u ON u.Id = up.UsuarioId " +
                    "JOIN dbo.Permiso p ON p.Id = up.PermisoId " +
                    "WHERE u.Username = @u AND p.Nombre = 'verbitacora'",
                    new SqlParameter("@u", username)),
                "El permiso verbitacora no quedo asignado al usuario.");

            Assert.IsTrue(
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.UsuarioHistorial h " +
                    "JOIN dbo.Usuarios u ON u.Id = h.UsuarioId WHERE u.Username = @u",
                    new SqlParameter("@u", username)) >= 2,
                "La asignacion de permisos deberia sumar una version al historial (ademas de la del alta).");

            Assert.AreEqual(
                asignacionesPrevias + 1,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserPermissionsUpdate", null),
                "La asignacion de permisos no quedo en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU03_UsuarioBasico_SoloVeAccionesPermitidas()
        {
            Window gestion = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);

            Assert.IsTrue(UiFind.Boton(gestion, "btnConfiguracion").IsEnabled, "basico tiene configuracionusuario.");
            Assert.IsFalse(UiFind.Boton(gestion, "btnAgregarUsuario").IsEnabled, "basico no gestiona usuarios.");
            Assert.IsFalse(UiFind.Boton(gestion, "btnEliminarUsuario").IsEnabled, "basico no gestiona usuarios.");
            Assert.IsFalse(UiFind.Boton(gestion, "btnGestionPermisos").IsEnabled, "basico no gestiona permisos.");
            Assert.IsFalse(UiFind.Boton(gestion, "btnBitacora").IsEnabled, "basico no ve bitacora.");
            Assert.IsFalse(UiFind.Boton(gestion, "btnHistorialUsuario").IsEnabled, "basico no ve historial.");
            Assert.IsFalse(UiFind.Boton(gestion, "btnGestionIdiomas").IsEnabled, "basico no gestiona idiomas.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU03_Admin_VeTodasLasAcciones()
        {
            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);

            Assert.IsTrue(UiFind.Boton(gestion, "btnAgregarUsuario").IsEnabled);
            Assert.IsTrue(UiFind.Boton(gestion, "btnEliminarUsuario").IsEnabled);
            Assert.IsTrue(UiFind.Boton(gestion, "btnGestionPermisos").IsEnabled);
            Assert.IsTrue(UiFind.Boton(gestion, "btnBitacora").IsEnabled);
            Assert.IsTrue(UiFind.Boton(gestion, "btnHistorialUsuario").IsEnabled);
            Assert.IsTrue(UiFind.Boton(gestion, "btnGestionIdiomas").IsEnabled);
            Assert.IsTrue(UiFind.Boton(gestion, "btnConfiguracion").IsEnabled);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_DesactivarYReactivarUsuario_ActualizaEstadoYBitacora()
        {
            string username = NombreUnico("uitest_baja");

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            CrearUsuarioDesdeGestion(gestion, username);

            // Baja logica: desmarcar activo + editar + confirmar.
            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.MarcarCheckBox(gestion, "chkbUsuarioActivo", false);
            UiFind.Click(gestion, "btnEditarUsuario");
            App.EsperarMessageBox().ConfirmarSi();
            MessageBoxWindow avisoBaja = App.EsperarMessageBox();
            StringAssert.Contains(
                avisoBaja.Texto,
                TestDatabase.ObtenerTraduccion("FormUserManagement.Success.UserDeactivated"),
                "Deberia confirmarse la desactivacion.");
            avisoBaja.CerrarConOk();

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u AND Activo = 1",
                    new SqlParameter("@u", username)),
                "El usuario deberia quedar inactivo tras la baja logica.");

            // Reactivacion: marcar activo + editar (sin confirmacion).
            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.MarcarCheckBox(gestion, "chkbUsuarioActivo", true);
            UiFind.Click(gestion, "btnEditarUsuario");
            MessageBoxWindow avisoAlta = App.EsperarMessageBox();
            StringAssert.Contains(
                avisoAlta.Texto,
                TestDatabase.ObtenerTraduccion("FormUserManagement.Success.UserActivated"),
                "Deberia confirmarse la reactivacion.");
            avisoAlta.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u AND Activo = 1",
                    new SqlParameter("@u", username)),
                "El usuario deberia quedar activo tras la reactivacion.");

            Assert.AreEqual(
                1,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserDeactivation", TestConfig.AdminUsername),
                "La desactivacion no quedo en la bitacora.");
            Assert.AreEqual(
                1,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserReactivation", TestConfig.AdminUsername),
                "La reactivacion no quedo en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_EliminarUsuarioFisico_QuitaFilaYActualizaDvv()
        {
            string username = NombreUnico("uitest_del");

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            CrearUsuarioDesdeGestion(gestion, username);

            string dvvAntes = TestDatabase.ObtenerDvv("Usuarios");

            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.Click(gestion, "btnEliminarUsuario");
            App.EsperarMessageBox().ConfirmarSi();
            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("FormUserManagement.Success.UserDeleted"),
                "Deberia confirmarse el borrado fisico.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u",
                    new SqlParameter("@u", username)),
                "El usuario deberia desaparecer de la tabla tras el borrado fisico.");

            Assert.AreNotEqual(
                dvvAntes,
                TestDatabase.ObtenerDvv("Usuarios"),
                "El DVV de Usuarios deberia actualizarse tras el borrado fisico.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU02_SinUsuarioSeleccionado_AdvierteEnLasAcciones()
        {
            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);

            UiFind.DeseleccionarFilaGrilla(gestion, "dgvUsuarios", TestConfig.AdminUsername);

            UiFind.Click(gestion, "btnGuardarPermisosUsuario");
            EsperarAvisoYCerrar("FormUserManagement.Warning.SelectUserPermissions");

            UiFind.Click(gestion, "btnEditarUsuario");
            EsperarAvisoYCerrar("FormUserManagement.Warning.NoUserSelectedEdit");

            UiFind.Click(gestion, "btnEliminarUsuario");
            EsperarAvisoYCerrar("FormUserManagement.Warning.NoUserSelectedDelete");

            UiFind.Click(gestion, "btnHistorialUsuario");
            EsperarAvisoYCerrar("FormUserManagement.Warning.SelectUserHistory");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CancelarEliminacionYDesactivacion_NoModificaUsuario()
        {
            string username = NombreUnico("uitest_cancel");

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            CrearUsuarioDesdeGestion(gestion, username);

            int hardDeletesPrevios = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserHardDelete", null);
            int desactivacionesPrevias = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserDeactivation", null);

            // No confirmar el borrado fisico.
            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.Click(gestion, "btnEliminarUsuario");
            App.EsperarMessageBox().RechazarNo();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u AND Activo = 1",
                    new SqlParameter("@u", username)),
                "El usuario debe seguir existiendo si no se confirma el borrado.");

            // No confirmar la desactivacion.
            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.MarcarCheckBox(gestion, "chkbUsuarioActivo", false);
            UiFind.Click(gestion, "btnEditarUsuario");
            App.EsperarMessageBox().RechazarNo();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u AND Activo = 1",
                    new SqlParameter("@u", username)),
                "El usuario debe seguir activo si no se confirma la desactivacion.");

            Assert.AreEqual(
                hardDeletesPrevios,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserHardDelete", null),
                "Un borrado cancelado no debe dejar bitacora de borrado.");
            Assert.AreEqual(
                desactivacionesPrevias,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserDeactivation", null),
                "Una desactivacion cancelada no debe dejar bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_AltaUsuarioSinPassword_AdvierteYNoInserta()
        {
            string username = NombreUnico("uitest_nopass");

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.EscribirTexto(gestion, "txtbNombreUsuario", username);
            UiFind.EscribirTexto(gestion, "txtbPasswordUsuario", string.Empty);
            UiFind.Click(gestion, "btnAgregarUsuario");

            EsperarAvisoYCerrar("FormUserManagement.Warning.CreateIncompleteData");

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u",
                    new SqlParameter("@u", username)),
                "El alta sin password no debe insertar el usuario.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU01_ValidacionesDeNombreYSeleccion_Advierte()
        {
            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnGestionPermisos");
            Window permisos = App.EsperarVentana("FormGestionPermisos");

            // CU01 7a: nombre vacio.
            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", string.Empty);
            UiFind.Click(permisos, "btnCrearFamilia");
            EsperarAvisoYCerrar("FormPermissionManagement.Warning.NameRequired");

            // CU01 7a: nombre con punto.
            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", "uitest.punto");
            UiFind.Click(permisos, "btnCrearFamilia");
            EsperarAvisoYCerrar("FormPermissionManagement.Warning.NameContainsDot");

            // Agregar patente sin item elegido en la lista.
            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", NombreUnico("uitestsel"));
            UiFind.Click(permisos, "btnCrearFamilia");
            UiFind.Click(permisos, "btnAgregarPatenteHija");
            EsperarAvisoYCerrar("FormPermissionManagement.Warning.SelectPatentFromList");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU01_FamiliaDuplicadaEnRaiz_InformaError()
        {
            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnGestionPermisos");
            Window permisos = App.EsperarVentana("FormGestionPermisos");

            // CU01 7b: ya existe la familia 'admin' en la raiz del catalogo seed.
            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", "admin");
            UiFind.Click(permisos, "btnCrearFamilia");
            UiFind.SeleccionarItemLista(permisos, "lstbPatentesDisponibles", "verbitacora");
            UiFind.Click(permisos, "btnAgregarPatenteHija");
            UiFind.Click(permisos, "btnGuardarFamilia");

            EsperarAvisoYCerrar("Error.Permission.DuplicateNames");

            Assert.AreEqual(
                1,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Permiso WHERE Nombre = 'admin'"),
                "No debe persistirse una familia duplicada en el mismo nivel.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T04_CU01_CicloEntreFamilias_InformaError()
        {
            string familiaA = NombreUnico("uitestcicloa");
            string familiaB = NombreUnico("uitestciclob");

            Window gestion = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            UiFind.Click(gestion, "btnGestionPermisos");
            Window permisos = App.EsperarVentana("FormGestionPermisos");

            // Familia A en la raiz del catalogo.
            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", familiaA);
            UiFind.Click(permisos, "btnCrearFamilia");

            // Volver al modo "nueva familia" (nodo Raiz) y crear B, tambien en la raiz.
            UiFind.SeleccionarNodoArbol(permisos, "tvwCatalogoPermisos", "Raiz");
            UiFind.EscribirTexto(permisos, "txtbNombreFamilia", familiaB);
            UiFind.Click(permisos, "btnCrearFamilia");

            // A pasa a ser hija de B.
            UiFind.SeleccionarItemLista(permisos, "lstbFamiliasDisponibles", familiaA);
            UiFind.Click(permisos, "btnAgregarFamiliaHija");

            // CU01 6a: agregar B como hija de A cerraria el ciclo. Debe informarse el
            // error de negocio, no una excepcion tecnica no controlada.
            UiFind.SeleccionarNodoArbol(permisos, "tvwCatalogoPermisos", familiaA);
            UiFind.SeleccionarItemLista(permisos, "lstbFamiliasDisponibles", familiaB);
            UiFind.Click(permisos, "btnAgregarFamiliaHija");

            EsperarAvisoYCerrar("Error.Permission.CycleDetected");

            // El rechazo no debe dejar el formulario roto: el arbol sigue navegable.
            UiFind.SeleccionarNodoArbol(permisos, "tvwCatalogoPermisos", familiaB);

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Permiso WHERE Nombre IN (@a, @b)",
                    new SqlParameter("@a", familiaA),
                    new SqlParameter("@b", familiaB)),
                "El intento de ciclo no debe persistir permisos.");
        }

        private void CrearUsuarioDesdeGestion(Window gestion, string username)
        {
            UiFind.EscribirTexto(gestion, "txtbNombreUsuario", username);
            UiFind.EscribirTexto(gestion, "txtbPasswordUsuario", "UiTest.Gestion.1234");
            UiFind.Click(gestion, "btnAgregarUsuario");

            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            StringAssert.Contains(
                confirmacion.Texto,
                TestDatabase.ObtenerTraduccion("FormUserManagement.Success.UserCreated"),
                "Deberia confirmarse el alta del usuario desde gestion.");
            confirmacion.CerrarConOk();
        }

        private static string NombreUnico(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
