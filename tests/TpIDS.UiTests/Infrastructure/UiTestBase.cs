using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TpIDS.UiTests.Infrastructure
{
    /// <summary>
    /// Base de los tests UI: maneja el ciclo de vida de la app bajo test,
    /// captura pantalla ante fallo y ofrece los flujos comunes (login).
    /// </summary>
    public abstract class UiTestBase
    {
        private UiAppSession _app;

        public TestContext TestContext { get; set; }

        internal UiAppSession App
        {
            get
            {
                if (_app == null)
                {
                    _app = new UiAppSession();
                }

                return _app;
            }
        }

        [TestCleanup]
        public void LimpiarAppBajoTest()
        {
            if (_app == null)
            {
                return;
            }

            try
            {
                if (TestContext != null && TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
                {
                    string captura = _app.CapturarPantalla(TestContext.TestName + "-" + DateTime.Now.ToString("HHmmss"));
                    TestContext.WriteLine("Captura de pantalla del fallo: " + captura);
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("No se pudo capturar pantalla: " + ex.Message);
            }
            finally
            {
                _app.Dispose();
                _app = null;
            }
        }

        internal Window EsperarLogin()
        {
            return App.EsperarVentana("FormLogin", TimeSpan.FromSeconds(30));
        }

        internal void CompletarLogin(Window login, string usuario, string password)
        {
            UiFind.EscribirTexto(login, "txtbNickname", usuario);
            UiFind.EscribirPassword(login, "txtbPassword", password);
            UiFind.Click(login, "btnIngreso");
        }

        /// <summary>Login completo: arranca la app si hace falta y deja abierto el shell FormPrincipal.</summary>
        internal Window LoginComo(string usuario, string password)
        {
            Window login = EsperarLogin();
            CompletarLogin(login, usuario, password);
            return App.EsperarVentana("FormPrincipal");
        }

        private static readonly string TextoMenuSesion = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Session");
        private static readonly string TextoMenuAdministracion = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Administration");

        /// <summary>Navega Administracion &gt; Usuarios desde el shell y espera el hijo MDI.</summary>
        internal Window AbrirGestionUsuarios(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuAdministracion, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Users"));
            return App.EsperarVentana("FormGestionUsuarios");
        }

        /// <summary>Navega Administracion &gt; Permisos desde el shell y espera el hijo MDI.</summary>
        internal Window AbrirGestionPermisos(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuAdministracion, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Permissions"));
            return App.EsperarVentana("FormGestionPermisos");
        }

        /// <summary>Navega Administracion &gt; Idiomas desde el shell y espera el hijo MDI.</summary>
        internal Window AbrirGestionIdiomas(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuAdministracion, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Languages"));
            return App.EsperarVentana("FormGestionIdiomas");
        }

        /// <summary>Navega Administracion &gt; Bitacora desde el shell y espera el hijo MDI.</summary>
        internal Window AbrirBitacora(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuAdministracion, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.AuditLog"));
            return App.EsperarVentana("FormBitacora");
        }

        /// <summary>Navega Administracion &gt; Backup desde el shell y espera el hijo MDI.</summary>
        internal Window AbrirBackup(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuAdministracion, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Backup"));
            return App.EsperarVentana("FormBackup");
        }

        /// <summary>Navega Sesion &gt; Mi configuracion desde el shell y espera el dialogo modal.</summary>
        internal Window AbrirConfiguracion(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuSesion, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.MyConfiguration"));
            return App.EsperarVentana("FormConfiguracionUsuario");
        }

        /// <summary>Navega Sesion &gt; Cerrar sesion desde el shell (no espera el MessageBox de confirmacion).</summary>
        internal void CerrarSesionDesdeMenu(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuSesion, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Logout"));
        }

        /// <summary>
        /// Selecciona un usuario en la grilla de gestion y espera a que el formulario
        /// refleje la seleccion en txtbNombreUsuario. Cierra la carrera con el rebind
        /// que la app hace tras cada dialogo de exito (re-selecciona la fila 0 = admin):
        /// si la edicion corre antes de verificar esto, puede caer sobre el usuario equivocado.
        /// </summary>
        internal void SeleccionarUsuarioEnGestion(Window gestion, string username)
        {
            // Camino rapido: seleccion por SelectionItemPattern (funciona con filas visibles).
            bool requiereFallbackTeclado = false;
            try
            {
                UiFind.SeleccionarFilaGrilla(gestion, "dgvUsuarios", username);
            }
            catch (FlaUI.Core.Exceptions.NoClickablePointException)
            {
                requiereFallbackTeclado = true;
            }

            if (!requiereFallbackTeclado && SeleccionReflejada(gestion, username, TimeSpan.FromSeconds(2)))
            {
                return;
            }

            // Fallback para filas fuera de la vista (Select y ScrollIntoView fallan en
            // silencio en el bridge UIA de WinForms): click en la primera fila, siempre
            // visible, para dar foco a la grilla, y flecha abajo hasta que el formulario
            // refleje el usuario buscado. El teclado scrollea y dispara SelectionChanged.
            UiFind.ClickFilaGrilla(gestion, "dgvUsuarios", TestConfig.AdminUsername);

            int limite = UiFind.ContarFilasGrilla(gestion, "dgvUsuarios") + 2;
            for (int paso = 0; paso < limite; paso++)
            {
                if (UiFind.LeerTexto(gestion, "txtbNombreUsuario") == username)
                {
                    return;
                }

                FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.DOWN);
                System.Threading.Thread.Sleep(150);
            }

            if (SeleccionReflejada(gestion, username, TimeSpan.FromSeconds(2)))
            {
                return;
            }

            throw new InvalidOperationException(
                "La seleccion de '" + username + "' no se reflejo en el formulario de gestion.");
        }

        private static bool SeleccionReflejada(Window gestion, string username, TimeSpan timeout)
        {
            RetryResult<bool> reflejada = Retry.WhileFalse(
                () => UiFind.LeerTexto(gestion, "txtbNombreUsuario") == username,
                timeout,
                TimeSpan.FromMilliseconds(200));

            return reflejada.Success;
        }

        /// <summary>
        /// Espera un MessageBox, verifica que muestre el mensaje traducido de la clave
        /// dada (idioma default de la BD de test) y lo cierra con OK.
        /// </summary>
        internal void EsperarAvisoYCerrar(string messageKey)
        {
            MessageBoxWindow aviso = App.EsperarMessageBox();
            Assert.IsTrue(
                aviso.Texto.Contains(TestDatabase.ObtenerTraduccion(messageKey)),
                "Se esperaba el aviso '" + messageKey + "' pero se mostro: " + aviso.Texto);
            aviso.CerrarConOk();
        }
    }
}
