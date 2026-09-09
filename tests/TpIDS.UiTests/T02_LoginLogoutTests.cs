using System;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Ola 1 - T02 Gestion de Login y Logout.
    /// Trazabilidad: diagramas/T02-login-logout/T02-login-logout-casos-de-uso.json
    /// (CU01 Login, CU02 Registro, CU03 Logout).
    /// </summary>
    [TestClass]
    public class T02_LoginLogoutTests : UiTestBase
    {
        private const string ActividadLoginOk = "FormAuditLog.Value.Activity.SuccessfulLogin";
        private const string ActividadLoginFallido = "FormAuditLog.Value.Activity.FailedLogin";
        private const string ActividadAltaUsuario = "FormAuditLog.Value.Activity.UserCreation";

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU01_LoginValido_AbrePantallaPrincipalYRegistraBitacora()
        {
            int loginsPrevios = TestDatabase.ContarBitacora(ActividadLoginOk, TestConfig.AdminUsername);

            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);

            Assert.IsNotNull(principal, "No se abrio FormPrincipal tras el login.");
            Assert.AreEqual(
                loginsPrevios + 1,
                TestDatabase.ContarBitacora(ActividadLoginOk, TestConfig.AdminUsername),
                "El login exitoso no quedo registrado en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU01_PasswordIncorrecta_InformaYRegistraFallo()
        {
            int fallosPrevios = TestDatabase.ContarBitacoraPorInformacion(
                ActividadLoginFallido, "FormAuditLog.Value.Info.LoginInvalidCredentials");

            Window login = EsperarLogin();
            CompletarLogin(login, TestConfig.AdminUsername, "password-incorrecta");

            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("FormLogin.Error.InvalidCredentials"),
                "El aviso deberia mostrar el mensaje de credenciales invalidas en el idioma default.");
            aviso.CerrarConOk();

            Assert.IsNotNull(App.BuscarVentana("FormLogin"), "La pantalla de login deberia seguir visible.");
            Assert.AreEqual(
                fallosPrevios + 1,
                TestDatabase.ContarBitacoraPorInformacion(ActividadLoginFallido, "FormAuditLog.Value.Info.LoginInvalidCredentials"),
                "El fallo de password no quedo registrado en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU01_UsuarioInexistente_InformaYRegistraFallo()
        {
            int fallosPrevios = TestDatabase.ContarBitacoraPorInformacion(
                ActividadLoginFallido, "FormAuditLog.Value.Info.UnknownUserLogin");

            Window login = EsperarLogin();
            CompletarLogin(login, "uitest_no_existe_" + DateTime.Now.Ticks, "cualquiera");

            // La UI muestra el mismo mensaje generico de credenciales invalidas (no revela si el usuario existe).
            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("FormLogin.Error.InvalidCredentials"),
                "El aviso deberia mostrar el mensaje generico de credenciales invalidas.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                fallosPrevios + 1,
                TestDatabase.ContarBitacoraPorInformacion(ActividadLoginFallido, "FormAuditLog.Value.Info.UnknownUserLogin"),
                "El intento con usuario inexistente no quedo en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU01_UsuarioInactivo_InformaYRegistraFallo()
        {
            int fallosPrevios = TestDatabase.ContarBitacoraPorInformacion(
                ActividadLoginFallido, "FormAuditLog.Value.Info.InactiveUserLogin");

            Window login = EsperarLogin();
            CompletarLogin(login, TestConfig.InactiveUsername, TestConfig.InactivePassword);

            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("FormLogin.Error.InvalidCredentials"),
                "El aviso deberia mostrar el mensaje generico de credenciales invalidas.");
            aviso.CerrarConOk();

            Assert.IsNotNull(App.BuscarVentana("FormLogin"), "La pantalla de login deberia seguir visible.");
            Assert.AreEqual(
                fallosPrevios + 1,
                TestDatabase.ContarBitacoraPorInformacion(ActividadLoginFallido, "FormAuditLog.Value.Info.InactiveUserLogin"),
                "El intento con usuario inactivo no quedo en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU01_CamposVacios_AdvierteYRegistraFallo()
        {
            int fallosPrevios = TestDatabase.ContarBitacoraPorInformacion(
                ActividadLoginFallido, "FormAuditLog.Value.Info.LoginEmptyFields");

            Window login = EsperarLogin();
            UiFind.Click(login, "btnIngreso");

            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("FormLogin.Warning.CompleteFields"),
                "El aviso deberia pedir completar los campos.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                fallosPrevios + 1,
                TestDatabase.ContarBitacoraPorInformacion(ActividadLoginFallido, "FormAuditLog.Value.Info.LoginEmptyFields"),
                "El intento con campos vacios no quedo en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU02_Registro_CreaUsuarioActivoConPerfilBasicoYFirmas()
        {
            string username = "uitest_reg_" + DateTime.Now.Ticks;
            const string password = "UiTest.Registro.1234";

            string dvvAntes = TestDatabase.ObtenerDvv("Usuarios");
            int altasPrevias = TestDatabase.ContarBitacora(ActividadAltaUsuario, null);

            Window login = EsperarLogin();
            UiFind.Click(login, "btnRegistro");

            Window registro = App.EsperarVentana("FormRegistro");
            UiFind.EscribirTexto(registro, "txtbNickname", username);
            UiFind.EscribirPassword(registro, "txtbPassword", password);
            UiFind.Click(registro, "btnRegistro");

            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            StringAssert.Contains(
                confirmacion.Texto,
                TestDatabase.ObtenerTraduccion("FormRegistration.Success.UserRegisteredMessage"),
                "Deberia informarse el registro exitoso.");
            confirmacion.CerrarConOk();

            Assert.IsTrue(
                App.VentanaCerrada("FormRegistro", TimeSpan.FromSeconds(10)),
                "El formulario de registro deberia cerrarse tras el alta exitosa.");

            // Persistencia del alta (CU02 pasos 8 a 11).
            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u AND Activo = 1 AND DVH IS NOT NULL " +
                    "AND PasswordHash <> @p",
                    new System.Data.SqlClient.SqlParameter("@u", username),
                    new System.Data.SqlClient.SqlParameter("@p", password)),
                "El usuario registrado no quedo activo con hash y DVH.");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.UsuarioPermiso up " +
                    "JOIN dbo.Usuarios u ON u.Id = up.UsuarioId " +
                    "JOIN dbo.Permiso p ON p.Id = up.PermisoId " +
                    "WHERE u.Username = @u AND p.Nombre = 'basico'",
                    new System.Data.SqlClient.SqlParameter("@u", username)),
                "El usuario registrado no recibio el perfil basico.");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.UsuarioHistorial h " +
                    "JOIN dbo.Usuarios u ON u.Id = h.UsuarioId WHERE u.Username = @u",
                    new System.Data.SqlClient.SqlParameter("@u", username)),
                "El alta no dejo la version inicial en el historial.");

            Assert.AreEqual(
                altasPrevias + 1,
                TestDatabase.ContarBitacora(ActividadAltaUsuario, null),
                "El alta no quedo registrada en la bitacora.");

            Assert.AreNotEqual(
                dvvAntes,
                TestDatabase.ObtenerDvv("Usuarios"),
                "El DVV de Usuarios deberia actualizarse de forma incremental tras el alta.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU02_RegistroDuplicado_InformaYNoInserta()
        {
            int usuariosAntes = TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Usuarios");
            string dvvAntes = TestDatabase.ObtenerDvv("Usuarios");

            Window login = EsperarLogin();
            UiFind.Click(login, "btnRegistro");

            Window registro = App.EsperarVentana("FormRegistro");
            UiFind.EscribirTexto(registro, "txtbNickname", TestConfig.AdminUsername);
            UiFind.EscribirPassword(registro, "txtbPassword", "OtraPassword.1234");
            UiFind.Click(registro, "btnRegistro");

            MessageBoxWindow aviso = App.EsperarMessageBox();
            StringAssert.Contains(
                aviso.Texto,
                TestDatabase.ObtenerTraduccion("Error.User.UsernameExists"),
                "Deberia informarse que el nombre de usuario ya existe.");
            aviso.CerrarConOk();

            Assert.AreEqual(
                usuariosAntes,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Usuarios"),
                "El registro duplicado no debe insertar usuarios.");
            Assert.AreEqual(
                dvvAntes,
                TestDatabase.ObtenerDvv("Usuarios"),
                "El registro duplicado no debe modificar el DVV.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU02_RegistroCamposVacios_AdvierteYNoInserta()
        {
            int usuariosAntes = TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Usuarios");

            Window login = EsperarLogin();
            UiFind.Click(login, "btnRegistro");
            Window registro = App.EsperarVentana("FormRegistro");

            // CU02 5a: registro sin completar campos (validacion de FormRegistro).
            UiFind.Click(registro, "btnRegistro");
            EsperarAvisoYCerrar("FormRegistration.Warning.CompleteFields");

            Assert.AreEqual(
                usuariosAntes,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Usuarios"),
                "El registro con campos vacios no debe insertar usuarios.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU03_LogoutConfirmado_CierraShellYVuelveALogin()
        {
            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);

            CerrarSesionDesdeMenu(principal);
            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            confirmacion.ConfirmarSi();

            Assert.IsTrue(
                App.VentanaCerrada("FormPrincipal", TimeSpan.FromSeconds(10)),
                "FormPrincipal deberia cerrarse al confirmar el logout.");
            Assert.IsNotNull(App.BuscarVentana("FormLogin"), "La pantalla de login deberia quedar visible tras el logout.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T02_CU03_LogoutNoConfirmado_MantieneSesion()
        {
            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);

            CerrarSesionDesdeMenu(principal);
            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            confirmacion.RechazarNo();
            confirmacion.EsperarCierre(TimeSpan.FromSeconds(5));

            Assert.IsNotNull(
                App.BuscarVentana("FormPrincipal"),
                "FormPrincipal deberia seguir abierta si el usuario no confirma el logout.");
        }
    }
}
