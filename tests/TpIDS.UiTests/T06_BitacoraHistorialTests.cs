using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Ola 4 - T06a Gestion de Bitacora + T06b Control de cambios de usuario.
    /// Trazabilidad: diagramas/T06-bitacora-control-cambios (CU01 Consultar bitacora,
    /// CU03/CU04 historial de versiones y restauracion).
    /// El registro en bitacora (T06a CU02) se verifica transversalmente en T02/T04/T05.
    /// </summary>
    [TestClass]
    public class T06_BitacoraHistorialTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void T06a_CU01_FiltrosUsuarioYNivel_CoincidenConSql()
        {
            // Se genera al menos una Advertencia (login fallido) antes de loguear.
            Window login = EsperarLogin();
            CompletarLogin(login, TestConfig.AdminUsername, "password-incorrecta");
            App.EsperarMessageBox().CerrarConOk();
            CompletarLogin(login, TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window principal = App.EsperarVentana("FormPrincipal");

            Window bitacora = AbrirBitacora(principal);

            // Filtro por usuario (el DAL aplica UsernameSnapshot LIKE %texto%).
            UiFind.EscribirTexto(bitacora, "txtbUsuarioBitacora", TestConfig.AdminUsername);
            UiFind.Click(bitacora, "btnBuscarBitacora");
            int filasUsuario = UiFind.ContarFilasGrilla(bitacora, "dgvBitacora");
            int sqlUsuario = TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.Bitacora WHERE UsernameSnapshot LIKE '%' + @u + '%'",
                new SqlParameter("@u", TestConfig.AdminUsername));

            Assert.IsTrue(sqlUsuario > 0, "Deberia haber eventos del usuario admin en la bitacora.");
            Assert.AreEqual(sqlUsuario, filasUsuario, "La grilla filtrada por usuario no coincide con la BD.");

            // Filtro por nivel Advertencia (el combo muestra el texto traducido del nivel).
            UiFind.EscribirTexto(bitacora, "txtbUsuarioBitacora", string.Empty);
            UiFind.SeleccionarItemCombo(
                bitacora,
                "cmbNivelBitacora",
                TestDatabase.ObtenerTraduccion("FormAuditLog.Level.Warning"));
            UiFind.Click(bitacora, "btnBuscarBitacora");
            int filasNivel = UiFind.ContarFilasGrilla(bitacora, "dgvBitacora");
            int sqlNivel = TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.Bitacora WHERE Nivel = 'Advertencia'");

            Assert.IsTrue(sqlNivel > 0, "Deberia haber al menos una advertencia (login fallido).");
            Assert.AreEqual(sqlNivel, filasNivel, "La grilla filtrada por nivel no coincide con la BD.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T06a_CU01_FiltroFecha_CoincideConSql()
        {
            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window bitacora = AbrirBitacora(principal);

            // El picker Win32 conserva su hora de reloj al setear solo la fecha via UIA:
            // se pide un dia entero de margen y se relee el valor realmente aplicado
            // para comparar contra la misma marca exacta que uso el filtro.
            UiFind.MarcarCheckBox(bitacora, "chkbFechaDesdeBitacora", true);
            UiFind.EstablecerFecha(bitacora, "dtpFechaDesdeBitacora", DateTime.Now.AddDays(-1));
            UiFind.MarcarCheckBox(bitacora, "chkbFechaHastaBitacora", true);
            UiFind.EstablecerFecha(bitacora, "dtpFechaHastaBitacora", DateTime.Now.AddDays(1));

            DateTime desdeAplicado = UiFind.LeerFecha(bitacora, "dtpFechaDesdeBitacora");
            DateTime hastaAplicado = UiFind.LeerFecha(bitacora, "dtpFechaHastaBitacora");

            UiFind.Click(bitacora, "btnBuscarBitacora");

            int filasFecha = UiFind.ContarFilasGrilla(bitacora, "dgvBitacora");
            int sqlFecha = TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.Bitacora WHERE FechaHora >= @desde AND FechaHora <= @hasta",
                new SqlParameter("@desde", desdeAplicado),
                new SqlParameter("@hasta", hastaAplicado));

            Assert.IsTrue(sqlFecha > 0, "El login deberia haber generado al menos un evento en el rango de hoy.");
            Assert.AreEqual(sqlFecha, filasFecha, "La grilla filtrada por fecha no coincide con la BD.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T06b_HistorialUsuario_ListaLasVersionesDelUsuario()
        {
            string username = NombreUnico("uitest_hist");
            string renombrado = username + "_v2";

            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window gestion = AbrirGestionUsuarios(principal);
            CrearUsuarioDesdeGestion(gestion, username);

            // Version 2: rename por edicion.
            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.EscribirTexto(gestion, "txtbNombreUsuario", renombrado);
            UiFind.Click(gestion, "btnEditarUsuario");
            App.EsperarMessageBox().CerrarConOk();

            SeleccionarUsuarioEnGestion(gestion, renombrado);
            UiFind.Click(gestion, "btnHistorialUsuario");
            Window historial = App.EsperarVentana("FormHistorialUsuario");

            int filas = UiFind.ContarFilasGrilla(historial, "dgvHistorialUsuario");
            int sqlVersiones = TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.UsuarioHistorial h " +
                "JOIN dbo.Usuarios u ON u.Id = h.UsuarioId WHERE u.Username = @u",
                new SqlParameter("@u", renombrado));

            Assert.AreEqual(2, sqlVersiones, "El alta y la edicion deberian dejar dos versiones en el historial.");
            Assert.AreEqual(sqlVersiones, filas, "La grilla de historial no coincide con las versiones en la BD.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T06b_RestaurarVersion_VuelveAlEstadoAnteriorYAudita()
        {
            string username = NombreUnico("uitest_rest");
            string renombrado = username + "_v2";
            int restauracionesPrevias = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserRestoration", null);

            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window gestion = AbrirGestionUsuarios(principal);
            CrearUsuarioDesdeGestion(gestion, username);

            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.EscribirTexto(gestion, "txtbNombreUsuario", renombrado);
            UiFind.Click(gestion, "btnEditarUsuario");
            App.EsperarMessageBox().CerrarConOk();

            SeleccionarUsuarioEnGestion(gestion, renombrado);
            UiFind.Click(gestion, "btnHistorialUsuario");
            Window historial = App.EsperarVentana("FormHistorialUsuario");

            // La version 1 es la fila cuyo snapshot conserva el username original.
            UiFind.SeleccionarFilaGrilla(historial, "dgvHistorialUsuario", username);
            UiFind.Click(historial, "btnRestaurarVersion");
            App.EsperarMessageBox().ConfirmarSi();
            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            StringAssert.Contains(
                confirmacion.Texto,
                TestDatabase.ObtenerTraduccion("FormUserHistory.Success.VersionRestored"),
                "Deberia confirmarse la restauracion de la version.");
            confirmacion.CerrarConOk();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u",
                    new SqlParameter("@u", username)),
                "El usuario deberia volver a su username original tras restaurar.");
            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u",
                    new SqlParameter("@u", renombrado)),
                "El username renombrado no deberia seguir vigente tras restaurar.");
            Assert.AreEqual(
                restauracionesPrevias + 1,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserRestoration", null),
                "La restauracion no quedo en la bitacora.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void T06b_RestaurarSinSeleccionONoConfirmado_NoModifica()
        {
            string username = NombreUnico("uitest_norest");
            string renombrado = username + "_v2";
            int restauracionesPrevias = TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserRestoration", null);

            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window gestion = AbrirGestionUsuarios(principal);
            CrearUsuarioDesdeGestion(gestion, username);

            SeleccionarUsuarioEnGestion(gestion, username);
            UiFind.EscribirTexto(gestion, "txtbNombreUsuario", renombrado);
            UiFind.Click(gestion, "btnEditarUsuario");
            App.EsperarMessageBox().CerrarConOk();

            SeleccionarUsuarioEnGestion(gestion, renombrado);
            UiFind.Click(gestion, "btnHistorialUsuario");
            Window historial = App.EsperarVentana("FormHistorialUsuario");
            UiFind.EsperarFilasGrilla(historial, "dgvHistorialUsuario", 2);

            // Sin version seleccionada: advierte y no hace nada.
            UiFind.DeseleccionarFilaGrilla(historial, "dgvHistorialUsuario", username);
            UiFind.Click(historial, "btnRestaurarVersion");
            EsperarAvisoYCerrar("FormUserHistory.Warning.SelectVersion");

            // Restauracion no confirmada: el estado no cambia.
            UiFind.SeleccionarFilaGrilla(historial, "dgvHistorialUsuario", username);
            UiFind.Click(historial, "btnRestaurarVersion");
            App.EsperarMessageBox().RechazarNo();

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u",
                    new SqlParameter("@u", renombrado)),
                "El username renombrado debe seguir vigente si no se confirma la restauracion.");
            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @u",
                    new SqlParameter("@u", username)),
                "El username original no debe volver sin confirmacion.");
            Assert.AreEqual(
                restauracionesPrevias,
                TestDatabase.ContarBitacora("FormAuditLog.Value.Activity.UserRestoration", null),
                "Una restauracion cancelada no debe dejar bitacora.");
        }

        private void CrearUsuarioDesdeGestion(Window gestion, string username)
        {
            UiFind.EscribirTexto(gestion, "txtbNombreUsuario", username);
            UiFind.EscribirTexto(gestion, "txtbPasswordUsuario", "UiTest.Gestion.1234");
            UiFind.Click(gestion, "btnAgregarUsuario");
            App.EsperarMessageBox().CerrarConOk();
        }

        private static string NombreUnico(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
