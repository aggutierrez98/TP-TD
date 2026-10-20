using System;
using System.Data.SqlClient;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    [TestClass]
    [TestCategory("UI")]
    public sealed class FIN_PerfilInversorTests : UiTestBase
    {
        [TestMethod]
        public void FIN_PerfilInversor_RegistroOmitido_ConservaCuentaSinPerfilYVuelveAlLogin()
        {
            string usuario = "uitest_perfil_" + DateTime.Now.Ticks;
            var cuestionario = Registrar(usuario);
            Assert.AreEqual(TestDatabase.ObtenerTraduccion("FormInvestorProfile.Button.Skip"), UiFind.Elemento(cuestionario, "btnCancelar").Name);
            UiFind.Click(cuestionario, "btnCancelar");
            Assert.IsTrue(App.VentanaCerrada("FormRegistro", TimeSpan.FromSeconds(10)));
            Assert.IsNotNull(EsperarLogin());
            Assert.AreEqual(1, TestDatabase.Contar("SELECT COUNT(*) FROM Usuarios WHERE Username=@u", new SqlParameter("@u", usuario)));
            Assert.AreEqual(0, ContarPerfil(usuario));
        }

        [TestMethod]
        public void FIN_PerfilInversor_RegistroCompleto_ExigeRespuestaConservaAlVolverYGuardaResultadoAlto()
        {
            string usuario = "uitest_perfil_" + DateTime.Now.Ticks;
            var cuestionario = Registrar(usuario);
            UiFind.Click(cuestionario, "btnSiguiente");
            EsperarPregunta(cuestionario, 1);
            Assert.IsFalse(UiFind.Elemento(cuestionario, "rdbOpcionA").AsRadioButton().IsChecked);
            UiFind.Click(cuestionario, "btnSiguiente");
            EsperarPregunta(cuestionario, 1);
            // Teclado: las flechas mueven la seleccion entre opciones (Abajo/Derecha avanzan, Arriba/Izquierda retroceden, con vuelta).
            UiFind.Elemento(cuestionario, "rdbOpcionA").Focus();
            Keyboard.Type(VirtualKeyShort.DOWN);
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.Elemento(cuestionario, "rdbOpcionB").AsRadioButton().IsChecked, TimeSpan.FromSeconds(5)).Success, "Flecha abajo debe marcar B.");
            Keyboard.Type(VirtualKeyShort.UP);
            Keyboard.Type(VirtualKeyShort.UP);
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.Elemento(cuestionario, "rdbOpcionC").AsRadioButton().IsChecked, TimeSpan.FromSeconds(5)).Success, "Flecha arriba desde A debe volver a la ultima opcion (la pregunta 1 tiene A, B y C).");
            Elegir(cuestionario, 'C');
            UiFind.Click(cuestionario, "btnSiguiente");
            EsperarPregunta(cuestionario, 2);
            UiFind.Click(cuestionario, "btnAnterior");
            EsperarPregunta(cuestionario, 1);
            Assert.IsTrue(UiFind.Elemento(cuestionario, "rdbOpcionC").AsRadioButton().IsChecked);
            Completar(cuestionario, "CDCDCCDDDC");
            Assert.Contains(TestDatabase.ObtenerTraduccion("FormInvestorProfile.Level.Alto"), UiFind.Elemento(cuestionario, "lblPregunta").Name);
            Assert.AreEqual(0, ContarPerfil(usuario), "Ver el resultado no debe guardarlo.");
            UiFind.Click(cuestionario, "btnAnterior");
            EsperarPregunta(cuestionario, 1);
            Completar(cuestionario, "CDCDCCDDDC");
            UiFind.Click(cuestionario, "btnSiguiente");
            Assert.IsTrue(App.VentanaCerrada("FormPerfilInversor", TimeSpan.FromSeconds(15)));
            Assert.AreEqual(1, ContarPerfil(usuario));
            Assert.AreEqual(10, TestDatabase.Contar("SELECT COUNT(*) FROM PerfilInversorRespuesta r JOIN PerfilInversor p ON r.PerfilInversorId=p.Id JOIN Usuarios u ON p.UsuarioId=u.Id WHERE u.Username=@u", new SqlParameter("@u", usuario)));
            Assert.IsNotNull(EsperarLogin());
        }

        [TestMethod]
        public void FIN_PerfilInversor_ConfiguracionSinPerfil_CompletaYReflejaNivelYFecha()
        {
            string usuario = "uitest_perfil_" + DateTime.Now.Ticks;
            UiFind.Click(Registrar(usuario), "btnCancelar");
            var principal = LoginComo(usuario, "UiTest.Perfil.1234");
            var configuracion = AbrirConfiguracion(principal);
            EsperarTexto(configuracion, "lblPerfilInversor", "FormUserSettings.Value.NoInvestorProfile");
            UiFind.Click(configuracion, "btnCuestionarioPerfil");
            var cuestionario = App.EsperarVentana("FormPerfilInversor");
            UiFind.Click(cuestionario, "btnSiguiente");
            Completar(cuestionario, "AAAAAAAAAA");
            UiFind.Click(cuestionario, "btnSiguiente");
            Assert.IsTrue(App.VentanaCerrada("FormPerfilInversor", TimeSpan.FromSeconds(15)));
            EsperarTexto(configuracion, "lblPerfilInversor", "FormInvestorProfile.Level.Bajo");
            Assert.IsFalse(string.IsNullOrWhiteSpace(UiFind.Elemento(configuracion, "lblFechaPerfil").Name));
            Assert.AreEqual(1, ContarPerfil(usuario));
        }

        [TestMethod]
        public void FIN_PerfilInversor_RepetirYCancelar_ConservaElPerfilAltoYLaFecha()
        {
            string usuario = "uitest_perfil_" + DateTime.Now.Ticks;
            var cuestionario = Registrar(usuario);
            UiFind.Click(cuestionario, "btnSiguiente");
            Completar(cuestionario, "CDCDCCDDDC");
            UiFind.Click(cuestionario, "btnSiguiente");
            Assert.IsTrue(App.VentanaCerrada("FormRegistro", TimeSpan.FromSeconds(15)));
            var configuracion = AbrirConfiguracion(LoginComo(usuario, "UiTest.Perfil.1234"));
            EsperarTexto(configuracion, "lblPerfilInversor", "FormInvestorProfile.Level.Alto");
            string fecha = UiFind.Elemento(configuracion, "lblFechaPerfil").Name;
            UiFind.Click(configuracion, "btnCuestionarioPerfil");
            cuestionario = App.EsperarVentana("FormPerfilInversor");
            UiFind.Click(cuestionario, "btnSiguiente");
            Completar(cuestionario, "AAAAAAAAAA");
            UiFind.Click(cuestionario, "btnCancelar");
            Assert.IsTrue(App.VentanaCerrada("FormPerfilInversor", TimeSpan.FromSeconds(10)));
            Assert.AreEqual(fecha, UiFind.Elemento(configuracion, "lblFechaPerfil").Name);
            Assert.Contains(TestDatabase.ObtenerTraduccion("FormInvestorProfile.Level.Alto"), UiFind.Elemento(configuracion, "lblPerfilInversor").Name);
            Assert.AreEqual(2, (int)TestDatabase.EscalarOrNull("SELECT p.NivelRiesgo FROM PerfilInversor p JOIN Usuarios u ON p.UsuarioId=u.Id WHERE u.Username=@u", new SqlParameter("@u", usuario)));
        }

        private Window Registrar(string usuario)
        {
            UiFind.Click(EsperarLogin(), "btnRegistro");
            var registro = App.EsperarVentana("FormRegistro");
            UiFind.EscribirTexto(registro, "txtbNickname", usuario);
            UiFind.EscribirPassword(registro, "txtbPassword", "UiTest.Perfil.1234");
            UiFind.Click(registro, "btnRegistro");
            EsperarAvisoYCerrar("FormRegistration.Success.UserRegisteredMessage");
            return App.EsperarVentana("FormPerfilInversor");
        }

        private static void Completar(Window cuestionario, string respuestas)
        {
            for (int i = 0; i < respuestas.Length; i++)
            {
                EsperarPregunta(cuestionario, i + 1);
                Elegir(cuestionario, respuestas[i]);
                UiFind.Click(cuestionario, "btnSiguiente");
            }
            EsperarTexto(cuestionario, "lblPregunta", "FormInvestorProfile.Level." + (respuestas.All(c => c == 'A') ? "Bajo" : "Alto"));
        }

        private static void Elegir(Window cuestionario, char opcion) => UiFind.Elemento(cuestionario, "rdbOpcion" + opcion).AsRadioButton().Click();

        private static void EsperarPregunta(Window cuestionario, int pregunta)
        {
            string texto = string.Format(TestDatabase.ObtenerTraduccion("FormInvestorProfile.Value.Progress"), pregunta);
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.Elemento(cuestionario, "lblPaso").Name == texto, TimeSpan.FromSeconds(10)).Success);
        }

        private static void EsperarTexto(Window ventana, string control, string key) =>
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.Elemento(ventana, control).Name.Contains(TestDatabase.ObtenerTraduccion(key)), TimeSpan.FromSeconds(15)).Success);

        private static int ContarPerfil(string usuario) => TestDatabase.Contar("SELECT COUNT(*) FROM PerfilInversor p JOIN Usuarios u ON p.UsuarioId=u.Id WHERE u.Username=@u", new SqlParameter("@u", usuario));
    }
}
