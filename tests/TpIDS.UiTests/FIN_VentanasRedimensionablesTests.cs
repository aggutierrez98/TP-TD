using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Patterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Ventanas redimensionables (change formularios-redimensionables): el contenido acompaña el tamaño de la
    /// ventana, el minimo es el tamaño de apertura y maximizar una pantalla MDI ocupa el area de trabajo sin
    /// fusionar su barra de titulo con la ventana principal.
    /// </summary>
    [TestClass]
    public class FIN_VentanasRedimensionablesTests : UiTestBase
    {
        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");

        // Tolerancia en pixeles para comparar limites (redondeos del layout).
        private const int Tolerancia = 2;
        private const int ToleranciaMdi = 2;

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Redimension_GestionComercios_AgrandarHaceCrecerLaGrilla()
        {
            Window comercios = AbrirGestionComercios();
            Rectangle ventana = comercios.BoundingRectangle;
            Rectangle grilla = UiFind.Elemento(comercios, "dgvComercios").BoundingRectangle;

            Redimensionar(comercios, ventana.Width + 300, ventana.Height + 200);

            Rectangle grillaAgrandada = UiFind.Elemento(comercios, "dgvComercios").BoundingRectangle;
            Assert.AreEqual(grilla.Width + 300, grillaAgrandada.Width, Tolerancia, "La grilla no acompaño el ancho de la ventana.");
            Assert.IsTrue(grillaAgrandada.Height > grilla.Height, "La grilla no crecio en alto al agrandar la ventana.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Redimension_GestionComercios_NoSeAchicaPorDebajoDelTamanioDeApertura()
        {
            Window comercios = AbrirGestionComercios();
            Rectangle apertura = comercios.BoundingRectangle;

            Redimensionar(comercios, apertura.Width / 2, apertura.Height / 2);

            Rectangle actual = comercios.BoundingRectangle;
            Assert.AreEqual(apertura.Width, actual.Width, "La ventana se achico por debajo de su ancho de apertura.");
            Assert.AreEqual(apertura.Height, actual.Height, "La ventana se achico por debajo de su alto de apertura.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Redimension_GestionComercios_MaximizarOcupaElAreaDeTrabajoYRestaurarVuelveAlTamanioPrevio()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            principal.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
            Window comercios = AbrirGestionComercios(principal);
            Rectangle antes = comercios.BoundingRectangle;
            Rectangle areaDeTrabajo = AreaMdi(principal);

            DobleClicEnBarraDeTitulo(comercios);

            Rectangle maximizada = comercios.BoundingRectangle;
            // El MdiClient tiene un borde hundido de 2 px: la pantalla ocupa su area cliente.
            Assert.AreEqual(areaDeTrabajo.Left, maximizada.Left, ToleranciaMdi, "La pantalla maximizada no arranca en el borde izquierdo del area de trabajo.");
            Assert.AreEqual(areaDeTrabajo.Top, maximizada.Top, ToleranciaMdi, "La pantalla maximizada no arranca en el borde superior del area de trabajo.");
            Assert.AreEqual(areaDeTrabajo.Width, maximizada.Width, 2 * ToleranciaMdi, "La pantalla maximizada no ocupa el ancho del area de trabajo.");
            Assert.AreEqual(areaDeTrabajo.Height, maximizada.Height, 2 * ToleranciaMdi, "La pantalla maximizada no ocupa el alto del area de trabajo.");
            Assert.AreEqual(
                WindowVisualState.Normal,
                comercios.Patterns.Window.Pattern.WindowVisualState.Value,
                "El maximizado nativo de un hijo MDI fusiona la barra de titulo con la ventana principal.");

            DobleClicEnBarraDeTitulo(comercios);

            Assert.AreEqual(antes, comercios.BoundingRectangle, "Restaurar no devolvio la pantalla a su tamaño y posicion previos.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Redimension_DialogoUbicacion_BotonesQuedanAbajoALaDerecha()
        {
            Window comercios = AbrirGestionComercios();
            UiFind.Click(comercios, "btnUbicarComercioEnMapa");
            Window dialogo = App.EsperarVentana("FormSeleccionarUbicacionComercio");
            Rectangle ventana = dialogo.BoundingRectangle;
            Rectangle boton = UiFind.Elemento(dialogo, "btnCancelarUbicacion").BoundingRectangle;

            Redimensionar(dialogo, ventana.Width + 200, ventana.Height + 150);

            Rectangle ventanaAgrandada = dialogo.BoundingRectangle;
            Rectangle botonAgrandado = UiFind.Elemento(dialogo, "btnCancelarUbicacion").BoundingRectangle;
            Assert.AreEqual(boton.Size, botonAgrandado.Size, "El boton cambio de tamaño al agrandar el dialogo.");
            Assert.AreEqual(ventana.Right - boton.Right, ventanaAgrandada.Right - botonAgrandado.Right, Tolerancia, "El boton no acompaño el borde derecho.");
            Assert.AreEqual(ventana.Bottom - boton.Bottom, ventanaAgrandada.Bottom - botonAgrandado.Bottom, Tolerancia, "El boton no acompaño el borde inferior.");
        }

        private Window AbrirGestionComercios()
        {
            return AbrirGestionComercios(LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword));
        }

        private Window AbrirGestionComercios(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Merchants"));
            return App.EsperarVentana("FormGestionComercios");
        }

        private static void Redimensionar(Window ventana, int ancho, int alto)
        {
            ITransformPattern transform = ventana.Patterns.Transform.Pattern;
            Assert.IsTrue(transform.CanResize.Value, "La ventana '" + ventana.AutomationId + "' no es redimensionable.");
            transform.Resize(ancho, alto);
            Thread.Sleep(500);
        }

        // El boton maximizar de Krypton no se expone por UIA; el doble clic en la barra llega por el mismo
        // WM_SYSCOMMAND. WindowPattern.SetWindowVisualState no sirve: usa ShowWindow y maximiza en forma nativa.
        private static void DobleClicEnBarraDeTitulo(Window ventana)
        {
            Rectangle limites = ventana.BoundingRectangle;
            Mouse.DoubleClick(new Point(limites.Left + limites.Width / 2, limites.Top + 12));
            Thread.Sleep(700);
        }

        private static Rectangle AreaMdi(Window principal)
        {
            AutomationElement mdiClient = principal.FindAllChildren()
                .FirstOrDefault(hijo => (hijo.Properties.ClassName.ValueOrDefault ?? string.Empty).Contains("MDICLIENT"));
            Assert.IsNotNull(mdiClient, "No se encontro el area MDI de la ventana principal.");
            return mdiClient.BoundingRectangle;
        }
    }
}
