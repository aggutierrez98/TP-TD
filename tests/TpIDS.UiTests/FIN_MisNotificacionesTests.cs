using System;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    [TestClass]
    public sealed class FIN_MisNotificacionesTests : UiTestBase
    {
        private static string MenuTexto => TestDatabase.ObtenerTraduccion("FormMainShell.Menu.MyNotifications");

        private static MenuItem MenuPersonal(Window principal)
        {
            return UiFind.Elemento(principal, "menuStripPrincipal").AsMenu().Items.First(i => i.Name == MenuTexto);
        }

        private static string DescripcionContador(Window principal)
        {
            return MenuPersonal(principal).Patterns.LegacyIAccessible.Pattern.Description.Value;
        }

        private Window AbrirBandeja(Window principal)
        {
            MenuPersonal(principal).Click();
            return App.EsperarVentana("FormMisNotificaciones");
        }

        private static void Insertar(string mensaje, string usuario = null)
        {
            TestDatabase.EjecutarSql(@"INSERT INTO Notificacion (UsuarioId, TipoNotificacion, Mensaje, Fecha, Leida)
                SELECT Id, 'Notificacion', @Mensaje, GETDATE(), 0 FROM Usuarios WHERE Username = @Usuario",
                new SqlParameter("@Mensaje", mensaje), new SqlParameter("@Usuario", usuario ?? TestConfig.BasicUsername));
        }

        private static void Limpiar(string prefijo)
        {
            TestDatabase.EjecutarSql("DELETE FROM Notificacion WHERE Mensaje LIKE @Prefijo", new SqlParameter("@Prefijo", prefijo + "%"));
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisNotificaciones_Textos_SeMuestranTraducidos()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Assert.AreNotEqual("FormMainShell.Menu.MyNotifications", MenuTexto);
            Window bandeja = AbrirBandeja(principal);
            foreach (var par in new[] {
                new[] { "lblTituloNotificacionesPersonales", "FormMyNotifications.Title" },
                new[] { "btnBuscarNotificaciones", "FormMyNotifications.Search" },
                new[] { "btnDetalleNotificacion", "FormMyNotifications.Details" },
                new[] { "btnMarcarNotificacionLeida", "FormMyNotifications.MarkRead" },
                new[] { "btnAnteriorNotificaciones", "FormMyNotifications.Previous" },
                new[] { "btnSiguienteNotificaciones", "FormMyNotifications.Next" } })
            {
                string texto = TestDatabase.ObtenerTraduccion(par[1]);
                Assert.AreNotEqual(par[1], texto);
                Assert.AreEqual(texto, UiFind.Elemento(bandeja, par[0]).Name);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisNotificaciones_UsuarioBasico_AbreDetalleYMarcaSoloPropia()
        {
            string prefijo = "uinotif" + Guid.NewGuid().ToString("N").Substring(0, 8);
            Insertar(prefijo + " propia");
            Insertar(prefijo + " ajena", TestConfig.AdminUsername);
            try
            {
                Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
                Assert.IsTrue(MenuPersonal(principal).IsEnabled);
                Window bandeja = AbrirBandeja(principal);
                UiFind.SeleccionarFilaGrilla(bandeja, "dgvNotificacionesPersonales", prefijo + " propia");
                UiFind.Click(bandeja, "btnDetalleNotificacion");
                Window detalle = App.EsperarVentana("FormDetalleNotificacion");
                Assert.AreEqual(prefijo + " propia", UiFind.CajaTexto(detalle, "txtbMensajeNotificacion").Text);
                Assert.AreEqual(1, TestDatabase.Contar("SELECT COUNT(*) FROM Notificacion WHERE Mensaje = @m AND Leida = 1", new SqlParameter("@m", prefijo + " propia")));
                Assert.AreEqual(0, TestDatabase.Contar("SELECT COUNT(*) FROM Notificacion WHERE Mensaje = @m AND Leida = 1", new SqlParameter("@m", prefijo + " ajena")));
                UiFind.Click(detalle, "btnCerrarDetalle");
                Assert.IsFalse(UiFind.Boton(bandeja, "btnMarcarNotificacionLeida").IsEnabled);
                int restantes = TestDatabase.Contar("SELECT COUNT(*) FROM Notificacion n JOIN Usuarios u ON u.Id = n.UsuarioId WHERE u.Username = @u AND n.Leida = 0", new SqlParameter("@u", TestConfig.BasicUsername));
                string esperado = string.Format(TestDatabase.ObtenerTraduccion("FormMainShell.MyNotifications.UnreadCount"), restantes);
                Assert.IsTrue(Retry.WhileFalse(() => DescripcionContador(principal) == esperado, TimeSpan.FromSeconds(10)).Success);
            }
            finally { Limpiar(prefijo); }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisNotificaciones_EjemplosDemo_SeLeenCompletosDesdeLaBandeja()
        {
            string[] claves = { "Demo.Compras", "Demo.Inversiones", "Demo.Movimientos" };
            TestDatabase.EjecutarSql(BloqueEjemplos(), new SqlParameter("@u", TestConfig.BasicUsername));
            try
            {
                Window bandeja = AbrirBandeja(LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword));
                for (int i = 0; i < claves.Length; i++)
                {
                    string completo = (string)TestDatabase.EscalarOrNull(
                        "SELECT n.Mensaje FROM Notificacion n JOIN Usuarios u ON u.Id = n.UsuarioId WHERE u.Username = @u AND n.ClaveEjemplo = @c",
                        new SqlParameter("@u", TestConfig.BasicUsername), new SqlParameter("@c", claves[i]));
                    Assert.IsTrue(completo.Contains("\r\n"), "El ejemplo debe tener saltos de linea.");
                    UiFind.SeleccionarFilaGrilla(bandeja, "dgvNotificacionesPersonales", ResumenGrilla(completo));
                    UiFind.Click(bandeja, "btnDetalleNotificacion");
                    Window detalle = App.EsperarVentana("FormDetalleNotificacion");
                    Assert.AreEqual(completo, UiFind.CajaTexto(detalle, "txtbMensajeNotificacion").Text);
                    Assert.AreEqual(1, TestDatabase.Contar("SELECT COUNT(*) FROM Notificacion WHERE ClaveEjemplo = @c AND Leida = 1", new SqlParameter("@c", claves[i])));
                    UiFind.Click(detalle, "btnCerrarDetalle");
                }
            }
            finally
            {
                TestDatabase.EjecutarSql("DELETE n FROM Notificacion n JOIN Usuarios u ON u.Id = n.UsuarioId WHERE u.Username = @u AND n.ClaveEjemplo LIKE 'Demo.%'",
                    new SqlParameter("@u", TestConfig.BasicUsername));
            }
        }

        // Texto de la celda Mensaje: una linea, espacios colapsados, maximo 120 caracteres con puntos suspensivos
        // (misma regla que UI/VistaPreviaNotificacion.Resumir).
        private static string ResumenGrilla(string mensaje)
        {
            string linea = System.Text.RegularExpressions.Regex.Replace(mensaje, @"\s+", " ").Trim();
            return linea.Length <= 120 ? linea : linea.Substring(0, 119).TrimEnd() + "\u2026";
        }

        // Bloque de ejemplos de script_datos_prueba.sql (entre sus marcadores), ligado al usuario basico.
        private static string BloqueEjemplos()
        {
            string texto = File.ReadAllText(Path.Combine(TestConfig.RepoRoot, "script_datos_prueba.sql"));
            int desde = texto.IndexOf("-- INICIO ejemplos de notificaciones", StringComparison.Ordinal);
            int hasta = texto.IndexOf("-- FIN ejemplos de notificaciones", StringComparison.Ordinal);
            Assert.IsTrue(desde >= 0 && hasta > desde, "Faltan los marcadores del bloque de ejemplos en script_datos_prueba.sql.");
            return "DECLARE @uid INT = (SELECT Id FROM Usuarios WHERE Username = @u);" + Environment.NewLine + texto.Substring(desde, hasta - desde);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisNotificaciones_Historial_PaginaDeDiezYMarcadoExplicito()
        {
            string prefijo = "uipagina" + Guid.NewGuid().ToString("N").Substring(0, 8);
            for (int i = 0; i < 12; i++) Insertar(prefijo + i.ToString("D2"));
            try
            {
                Window bandeja = AbrirBandeja(LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword));
                UiFind.EsperarFilasGrilla(bandeja, "dgvNotificacionesPersonales", 10);
                Assert.AreEqual(10, UiFind.ContarFilasGrilla(bandeja, "dgvNotificacionesPersonales"));
                UiFind.SeleccionarFilaGrilla(bandeja, "dgvNotificacionesPersonales", prefijo + "11");
                UiFind.Click(bandeja, "btnMarcarNotificacionLeida");
                Assert.IsTrue(Retry.WhileFalse(() => TestDatabase.Contar("SELECT COUNT(*) FROM Notificacion WHERE Mensaje = @m AND Leida = 1", new SqlParameter("@m", prefijo + "11")) == 1, TimeSpan.FromSeconds(10)).Success);
                Assert.IsTrue(Retry.WhileFalse(() => UiFind.Boton(bandeja, "btnSiguienteNotificaciones").IsEnabled, TimeSpan.FromSeconds(10)).Success);
                UiFind.Click(bandeja, "btnSiguienteNotificaciones");
                Assert.IsTrue(Retry.WhileFalse(() => UiFind.Boton(bandeja, "btnAnteriorNotificaciones").IsEnabled, TimeSpan.FromSeconds(10)).Success);
            }
            finally { Limpiar(prefijo); }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisNotificaciones_NuevaDuranteSesion_ActualizaContadorSinAbrirBandeja()
        {
            string prefijo = "uipoll" + Guid.NewGuid().ToString("N").Substring(0, 8);
            try
            {
                Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
                int cantidad = TestDatabase.Contar("SELECT COUNT(*) FROM Notificacion n JOIN Usuarios u ON u.Id = n.UsuarioId WHERE u.Username = @u AND n.Leida = 0", new SqlParameter("@u", TestConfig.BasicUsername));
                string esperado = string.Format(TestDatabase.ObtenerTraduccion("FormMainShell.MyNotifications.UnreadCount"), cantidad);
                Assert.IsTrue(Retry.WhileFalse(() => DescripcionContador(principal) == esperado, TimeSpan.FromSeconds(15)).Success);
                Insertar(prefijo);
                string nuevo = string.Format(TestDatabase.ObtenerTraduccion("FormMainShell.MyNotifications.UnreadCount"), cantidad + 1);
                Assert.IsTrue(Retry.WhileFalse(() => DescripcionContador(principal) == nuevo, TimeSpan.FromSeconds(40)).Success);
                Assert.IsNull(App.BuscarVentana("FormMisNotificaciones"));
            }
            finally { Limpiar(prefijo); }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisNotificaciones_Fechas_FiltraDiaCompletoSinAlterarContador()
        {
            string prefijo = "uifecha" + Guid.NewGuid().ToString("N").Substring(0, 8);
            Insertar(prefijo + " dentro");
            Insertar(prefijo + " fuera");
            TestDatabase.EjecutarSql("UPDATE Notificacion SET Fecha = @f WHERE Mensaje = @m", new SqlParameter("@f", new DateTime(2020, 2, 10, 23, 59, 0)), new SqlParameter("@m", prefijo + " dentro"));
            try
            {
                Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
                Window bandeja = AbrirBandeja(principal);
                UiFind.MarcarCheckBox(bandeja, "chkbDesdeNotificaciones", true);
                UiFind.MarcarCheckBox(bandeja, "chkbHastaNotificaciones", true);
                UiFind.EstablecerFecha(bandeja, "dtpDesdeNotificaciones", new DateTime(2020, 2, 10));
                UiFind.EstablecerFecha(bandeja, "dtpHastaNotificaciones", new DateTime(2020, 2, 10));
                UiFind.Click(bandeja, "btnBuscarNotificaciones");
                Assert.IsTrue(Retry.WhileFalse(() => UiFind.ContarFilasGrilla(bandeja, "dgvNotificacionesPersonales") == 1, TimeSpan.FromSeconds(10)).Success);
                UiFind.SeleccionarFilaGrilla(bandeja, "dgvNotificacionesPersonales", prefijo + " dentro");
                int total = TestDatabase.Contar("SELECT COUNT(*) FROM Notificacion n JOIN Usuarios u ON u.Id = n.UsuarioId WHERE u.Username = @u AND n.Leida = 0", new SqlParameter("@u", TestConfig.BasicUsername));
                string esperado = string.Format(TestDatabase.ObtenerTraduccion("FormMainShell.MyNotifications.UnreadCount"), total);
                Assert.IsTrue(Retry.WhileFalse(() => DescripcionContador(principal) == esperado, TimeSpan.FromSeconds(10)).Success);
            }
            finally { Limpiar(prefijo); }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MisNotificaciones_IdiomaEnVivo_TraduceBandejaYConservaContador()
        {
            object previa = TestDatabase.EscalarOrNull("SELECT p.IdiomaId FROM UsuarioPreferencias p JOIN Usuarios u ON u.Id = p.UsuarioId WHERE u.Username = @u", new SqlParameter("@u", TestConfig.BasicUsername));
            try
            {
                Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
                Window bandeja = AbrirBandeja(principal);
                Window configuracion = AbrirConfiguracion(principal);
                UiFind.SeleccionarItemCombo(configuracion, "cmbIdiomas", "English");
                UiFind.Click(configuracion, "btnGuardarConfiguracion");
                App.EsperarMessageBox().CerrarConOk();
                configuracion.Close();
                Assert.AreEqual("My notifications", UiFind.Elemento(bandeja, "lblTituloNotificacionesPersonales").Name);
                Assert.AreEqual("Mark as read", UiFind.Elemento(bandeja, "btnMarcarNotificacionLeida").Name);
                var menu = UiFind.Elemento(principal, "menuStripPrincipal").AsMenu().Items.First(i => i.Name == "My notifications");
                Assert.IsTrue(menu.Patterns.LegacyIAccessible.Pattern.Description.Value.Contains("unread notifications"));
            }
            finally
            {
                TestDatabase.EjecutarSql("DELETE p FROM UsuarioPreferencias p JOIN Usuarios u ON u.Id = p.UsuarioId WHERE u.Username = @u", new SqlParameter("@u", TestConfig.BasicUsername));
                if (previa != null && previa != DBNull.Value)
                    TestDatabase.EjecutarSql("INSERT INTO UsuarioPreferencias (UsuarioId, IdiomaId) SELECT Id, @i FROM Usuarios WHERE Username = @u", new SqlParameter("@i", previa), new SqlParameter("@u", TestConfig.BasicUsername));
            }
        }
    }
}
