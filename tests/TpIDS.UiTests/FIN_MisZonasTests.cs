using System.Data.SqlClient;
using System.Drawing;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// FIN - Mis zonas (change zonas-de-usuario): menu Mi usuario &gt; Mis zonas, alta por clic en
    /// el mapa, nombre repetido, falta de punto, baja confirmada, zonas de otro usuario y permiso.
    /// La busqueda de direccion no se cubre aca porque consulta un servicio real (ver
    /// FIN_BusquedaUbicacionTests); su logica vive en BLL.Tests/CoordinadorBusquedaUbicacionTests.
    /// ZonaUsuario no tiene firmas DVH/DVV: los datos previos se arman por SQL directo y cada test
    /// arranca sin zonas del usuario basico ni de admin (el tope es de 10 por usuario).
    /// </summary>
    [TestClass]
    public class FIN_MisZonasTests : UiTestBase
    {
        private const string IdFormulario = "FormMisZonas";
        private static readonly string TextoMenuMiUsuario = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.MyUser");

        [TestInitialize]
        public void BorrarZonasDePrueba()
        {
            TestDatabase.EjecutarSql(
                "DELETE z FROM dbo.ZonaUsuario z JOIN dbo.Usuarios u ON u.Id = z.UsuarioId WHERE u.Username IN (@basico, @admin)",
                new SqlParameter("@basico", TestConfig.BasicUsername),
                new SqlParameter("@admin", TestConfig.AdminUsername));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisZonas_AltaPorClicEnElMapa_PersisteLaZonaConSuRadioYLaMuestraEnLaGrilla()
        {
            Window zonas = AbrirMisZonasComoBasico();

            UiFind.EscribirTexto(zonas, "txtbNombreZona", "Trabajo");
            UiFind.EstablecerNumero(zonas, "nudRadioZona", 600);
            ClickEnElCentroDelMapa(zonas);
            Assert.AreNotEqual(
                TestDatabase.ObtenerTraduccion("FormMyZones.Value.NoPoint"),
                UiFind.Elemento(zonas, "lblCoordenadasZona").Name,
                "El clic en el mapa debe fijar el punto provisorio.");

            UiFind.Click(zonas, "btnAgregarZona");
            EsperarAvisoYCerrar("FormMyZones.Success.Created");

            Assert.AreEqual(
                1,
                ContarZonas(TestConfig.BasicUsername, "Trabajo", 600),
                "La zona no quedo persistida para el usuario de la sesion con el radio elegido.");
            UiFind.EsperarFilasGrilla(zonas, "dgvZonasUsuario", 1);
            Assert.AreEqual(1, UiFind.ContarFilasGrilla(zonas, "dgvZonasUsuario"));
            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormMyZones.Value.NoPoint"),
                UiFind.Elemento(zonas, "lblCoordenadasZona").Name,
                "Tras el alta el punto provisorio debe limpiarse.");
            TestContext.WriteLine("Captura del alta: " + App.CapturarPantalla("FIN_MisZonas_Alta"));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisZonas_AgregarSinPunto_PideElegirUnPuntoYNoGuarda()
        {
            Window zonas = AbrirMisZonasComoBasico();

            UiFind.EscribirTexto(zonas, "txtbNombreZona", "Casa");
            UiFind.Click(zonas, "btnAgregarZona");
            EsperarAvisoYCerrar("Error.MyZones.PointRequired");

            Assert.AreEqual(0, ContarZonas(TestConfig.BasicUsername, "Casa", null), "Sin punto no debe guardarse nada.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisZonas_NombreRepetidoSinDistinguirMayusculas_RechazaElAlta()
        {
            InsertarZona(TestConfig.BasicUsername, "Casa", -34.6m, -58.4m, 1000);
            Window zonas = AbrirMisZonasComoBasico();

            UiFind.EscribirTexto(zonas, "txtbNombreZona", "CASA");
            ClickEnElCentroDelMapa(zonas);
            UiFind.Click(zonas, "btnAgregarZona");
            EsperarAvisoYCerrar("Error.MyZones.DuplicateName");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.ZonaUsuario z JOIN dbo.Usuarios u ON u.Id = z.UsuarioId WHERE u.Username = @u",
                    new SqlParameter("@u", TestConfig.BasicUsername)),
                "El nombre repetido no debe agregar otra zona.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisZonas_QuitarSeleccionadaYConfirmar_EliminaLaZona()
        {
            InsertarZona(TestConfig.BasicUsername, "Gimnasio", -34.6m, -58.4m, 800);
            Window zonas = AbrirMisZonasComoBasico();
            UiFind.EsperarFilasGrilla(zonas, "dgvZonasUsuario", 1);

            UiFind.SeleccionarFilaGrilla(zonas, "dgvZonasUsuario", "Gimnasio");
            UiFind.Click(zonas, "btnQuitarZona");
            MessageBoxWindow confirmacion = App.EsperarMessageBox();
            Assert.IsTrue(confirmacion.Texto.Contains("Gimnasio"), "La confirmacion debe nombrar la zona a quitar.");
            confirmacion.ConfirmarSi();
            EsperarAvisoYCerrar("FormMyZones.Success.Deleted");

            Assert.AreEqual(0, ContarZonas(TestConfig.BasicUsername, "Gimnasio", null), "La zona confirmada debe eliminarse.");
            Assert.AreEqual(0, UiFind.ContarFilasGrilla(zonas, "dgvZonasUsuario"), "La zona debe desaparecer de la grilla.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisZonas_ZonasDeOtroUsuario_NoSeMuestran()
        {
            InsertarZona(TestConfig.AdminUsername, "ZonaDelAdmin", -34.6m, -58.4m, 1000);
            InsertarZona(TestConfig.BasicUsername, "ZonaPropia", -34.61m, -58.41m, 1000);

            Window zonas = AbrirMisZonasComoBasico();
            UiFind.EsperarFilasGrilla(zonas, "dgvZonasUsuario", 1);

            Assert.AreEqual(1, UiFind.ContarFilasGrilla(zonas, "dgvZonasUsuario"), "Solo deben verse las zonas del usuario de la sesion.");
            UiFind.SeleccionarFilaGrilla(zonas, "dgvZonasUsuario", "ZonaPropia");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisZonas_UsuarioSinPermiso_VeMisZonasDeshabilitadoYCerrarSesionHabilitado()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);

            Assert.IsTrue(UiFind.MenuTopHabilitado(principal, TextoMenuMiUsuario), "El menu Mi usuario debe estar disponible.");
            Assert.IsFalse(
                UiFind.MenuItemHabilitado(principal, TextoMenuMiUsuario, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.MyZones")),
                "Sin gestionzonasusuario, Mis zonas debe estar deshabilitado.");
            Assert.IsTrue(
                UiFind.MenuItemHabilitado(principal, TextoMenuMiUsuario, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Logout")),
                "Cerrar sesion debe conservar su comportamiento.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisZonas_AlAbrir_MuestraTextosTraducidosYSinPuntoElegido()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Assert.IsTrue(
                UiFind.MenuItemHabilitado(principal, TextoMenuMiUsuario, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.MyZones")),
                "El usuario basico debe ver Mis zonas habilitado.");
            Window zonas = AbrirMisZonas(principal);

            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormMyZones.TitleLabel"), UiFind.Elemento(zonas, "lblTituloMisZonas").Name);
            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormMyZones.AddButton"), UiFind.Elemento(zonas, "btnAgregarZona").Name);
            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormMyZones.RemoveButton"), UiFind.Elemento(zonas, "btnQuitarZona").Name);
            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormMyZones.Value.NoPoint"), UiFind.Elemento(zonas, "lblCoordenadasZona").Name);
            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormMapLocationPicker.SearchCoverageHelp"),
                UiFind.Elemento(zonas, "lblEstadoBusquedaZona").Name);
            TestContext.WriteLine("Captura al abrir: " + App.CapturarPantalla("FIN_MisZonas_AlAbrir"));
        }

        private Window AbrirMisZonasComoBasico()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            return AbrirMisZonas(principal);
        }

        private static void ClickEnElCentroDelMapa(Window zonas)
        {
            Rectangle mapa = UiFind.Elemento(zonas, "mapZonasUsuario").BoundingRectangle;
            Mouse.Click(new Point(mapa.Left + (mapa.Width / 2), mapa.Top + (mapa.Height / 2)));
            System.Threading.Thread.Sleep(300);
        }

        private static int ContarZonas(string username, string nombre, int? radioMetros)
        {
            return TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.ZonaUsuario z JOIN dbo.Usuarios u ON u.Id = z.UsuarioId " +
                "WHERE u.Username = @u AND z.Nombre = @n AND (@r IS NULL OR z.RadioMetros = @r)",
                new SqlParameter("@u", username),
                new SqlParameter("@n", nombre),
                new SqlParameter("@r", System.Data.SqlDbType.Int) { Value = (object)radioMetros ?? System.DBNull.Value });
        }

        private static void InsertarZona(string username, string nombre, decimal latitud, decimal longitud, int radioMetros)
        {
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.ZonaUsuario (UsuarioId, Nombre, Latitud, Longitud, RadioMetros) " +
                "SELECT u.Id, @n, @lat, @lng, @r FROM dbo.Usuarios u WHERE u.Username = @u",
                new SqlParameter("@u", username),
                new SqlParameter("@n", nombre),
                new SqlParameter("@lat", latitud),
                new SqlParameter("@lng", longitud),
                new SqlParameter("@r", radioMetros));
        }
    }
}
