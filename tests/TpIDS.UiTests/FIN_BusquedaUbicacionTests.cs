using System.Drawing;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Busqueda textual en el selector de ubicacion (change busqueda-textual-ubicacion-comercio). Solo cubre lo que
    /// no emite ninguna peticion de red: el exe real no admite un servicio falso. La logica con respuestas controladas
    /// (obsoletas, doble envio, errores, cierre tardio) vive en BLL.Tests/CoordinadorBusquedaUbicacionTests y la
    /// busqueda contra el proveedor real se comprueba a mano (tarea 4.1).
    /// </summary>
    [TestClass]
    public class FIN_BusquedaUbicacionTests : UiTestBase
    {
        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");
        private const string IdDialogo = "FormSeleccionarUbicacionComercio";
        private const int Tolerancia = 2;

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_BusquedaUbicacion_BuscarConTextoVacio_InformaQueFaltaTextoSinCerrarNiMarcar()
        {
            Window dialogo = AbrirDialogoUbicacion();

            UiFind.Click(dialogo, "btnBuscarUbicacion");

            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormMapLocationPicker.SearchTextRequired"),
                UiFind.Elemento(dialogo, "lblEstadoBusquedaUbicacion").Name,
                "Buscar sin texto debe mostrar el mensaje traducido de texto requerido.");
            Assert.IsFalse(UiFind.Elemento(dialogo, "cmbResultadosUbicacion").IsEnabled, "Sin consulta no debe haber coincidencias elegibles.");
            Assert.IsFalse(UiFind.Elemento(dialogo, "btnAceptarUbicacion").IsEnabled, "Buscar sin texto no debe marcar una ubicacion.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_BusquedaUbicacion_EnterEnElCampoVacio_NoAceptaNiCierraElDialogo()
        {
            Window dialogo = AbrirDialogoUbicacion();
            UiFind.Elemento(dialogo, "txtbBusquedaUbicacion").Click();

            Keyboard.Type(VirtualKeyShort.ENTER);

            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormMapLocationPicker.SearchTextRequired"),
                UiFind.Elemento(dialogo, "lblEstadoBusquedaUbicacion").Name,
                "Enter en el campo debe ejecutar Buscar (texto requerido).");
            Assert.IsFalse(App.VentanaCerrada(IdDialogo, System.TimeSpan.FromSeconds(1)), "Enter en el campo no debe cerrar ni aceptar el dialogo.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_BusquedaUbicacion_AlAbrir_MuestraAyudaDeCoberturaYAtribucionLibre()
        {
            Window dialogo = AbrirDialogoUbicacion();

            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormMapLocationPicker.SearchCoverageHelp"),
                UiFind.Elemento(dialogo, "lblEstadoBusquedaUbicacion").Name);
            Assert.AreEqual(
                TestDatabase.ObtenerTraduccion("FormMapLocationPicker.SearchAttribution"),
                UiFind.Elemento(dialogo, "lblAtribucionOsmUbicacion").Name);
            AssertDistribucion(dialogo);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_BusquedaUbicacion_AgrandarYRestaurar_ConservaDistribucionYAtribucionVisible()
        {
            Window dialogo = AbrirDialogoUbicacion();
            Rectangle ventana = dialogo.BoundingRectangle;
            Rectangle mapa = UiFind.Elemento(dialogo, "mapUbicacionComercio").BoundingRectangle;

            dialogo.Patterns.Transform.Pattern.Resize(ventana.Width + 240, ventana.Height + 160);
            System.Threading.Thread.Sleep(500);
            Rectangle mapaAgrandado = UiFind.Elemento(dialogo, "mapUbicacionComercio").BoundingRectangle;
            Assert.AreEqual(mapa.Width + 240, mapaAgrandado.Width, Tolerancia, "El mapa no acompaño el ancho.");
            Assert.AreEqual(mapa.Height + 160, mapaAgrandado.Height, Tolerancia, "El mapa no acompaño el alto.");
            AssertDistribucion(dialogo);

            dialogo.Patterns.Transform.Pattern.Resize(ventana.Width, ventana.Height);
            System.Threading.Thread.Sleep(500);
            Assert.AreEqual(mapa.Size, UiFind.Elemento(dialogo, "mapUbicacionComercio").BoundingRectangle.Size, "Restaurar no devolvio el mapa a su tamaño inicial.");
            AssertDistribucion(dialogo);
        }

        // Orden vertical sin solapes (campo, coincidencias, estado, mapa, atribucion/botones) y atribucion dentro de la ventana.
        private static void AssertDistribucion(Window dialogo)
        {
            Rectangle ventana = dialogo.BoundingRectangle;
            Rectangle campo = UiFind.Elemento(dialogo, "txtbBusquedaUbicacion").BoundingRectangle;
            Rectangle buscar = UiFind.Elemento(dialogo, "btnBuscarUbicacion").BoundingRectangle;
            Rectangle combo = UiFind.Elemento(dialogo, "cmbResultadosUbicacion").BoundingRectangle;
            Rectangle estado = UiFind.Elemento(dialogo, "lblEstadoBusquedaUbicacion").BoundingRectangle;
            Rectangle mapa = UiFind.Elemento(dialogo, "mapUbicacionComercio").BoundingRectangle;
            Rectangle atribucion = UiFind.Elemento(dialogo, "lblAtribucionOsmUbicacion").BoundingRectangle;
            Rectangle aceptar = UiFind.Elemento(dialogo, "btnAceptarUbicacion").BoundingRectangle;

            Assert.IsTrue(campo.Right <= buscar.Left, "Campo y Buscar no deben solaparse.");
            Assert.IsTrue(buscar.Bottom <= combo.Top + Tolerancia && combo.Bottom <= estado.Top + Tolerancia, "Campo, coincidencias y estado deben apilarse sin solapes.");
            Assert.IsTrue(estado.Bottom <= mapa.Top + Tolerancia, "El estado no debe invadir el mapa.");
            Assert.IsTrue(mapa.Bottom <= atribucion.Top + Tolerancia, "La atribucion debe quedar libre, fuera del mapa.");
            Assert.IsTrue(atribucion.Right <= aceptar.Left, "La atribucion no debe solaparse con Aceptar.");
            Assert.IsTrue(ventana.Contains(atribucion), "La atribucion debe estar dentro de la ventana.");
            Assert.IsTrue(mapa.Height > 150, "El mapa debe conservar un area util para navegar.");
        }

        private Window AbrirDialogoUbicacion()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Merchants"));
            Window comercios = App.EsperarVentana("FormGestionComercios");
            UiFind.Click(comercios, "btnUbicarComercioEnMapa");
            return App.EsperarVentana(IdDialogo);
        }
    }
}
