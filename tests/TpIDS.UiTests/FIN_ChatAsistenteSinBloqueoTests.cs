using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Chat del asistente personal sin bloqueo de la interfaz (change chat-asistente-sin-bloqueo-ui).
    /// El exe real no admite un servicio falso, asi que la dependencia lenta controlada es SQL: el test
    /// retiene dbo.ConsultaChatbot con un bloqueo exclusivo y la lectura del historial del envio queda
    /// esperando dentro de la app. La copia stageada no tiene clave de Gemini, por lo que al liberar el
    /// bloqueo el envio termina por el camino de error (sin red). El camino exitoso con respuesta real se
    /// cubre en BLL.Tests/AsistenteFinancieroBLLTests.
    /// </summary>
    [TestClass]
    public class FIN_ChatAsistenteSinBloqueoTests : UiTestBase
    {
        private static readonly string TextoMenuAsistente = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.PersonalAssistant");
        private const string IdFormularioAsistente = "FormAsistentePersonal";
        private const string Pregunta = "Pregunta retenida por la prueba de interfaz";

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ChatAsistente_LecturaDeHistorialRetenida_ShellSigueRespondiendoYRecuperaTrasError()
        {
            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window asistente = AbrirAsistente(principal);
            UiFind.EscribirTexto(asistente, "txtbMensajeAsistente", Pregunta);

            using (BloqueoConsultaChatbot bloqueo = BloqueoConsultaChatbot.Tomar())
            {
                UiFind.Click(asistente, "btnEnviarMensajeAsistente");
                Assert.IsTrue(bloqueo.EsperarLecturaRetenida(TimeSpan.FromSeconds(10)), "El envio no llego a la lectura del historial retenida.");

                Assert.IsTrue(VentanaRespondeMensajes(principal), "El shell dejo de procesar mensajes durante la lectura del historial.");
                Assert.IsFalse(UiFind.Boton(asistente, "btnEnviarMensajeAsistente").IsEnabled, "El boton debe quedar inhibido mientras hay un envio activo.");
                Assert.IsFalse(UiFind.Elemento(asistente, "txtbMensajeAsistente").IsEnabled, "El campo debe quedar inhibido, sin Enter que dispare otro envio.");

                Rectangle posicionInicial = asistente.BoundingRectangle;
                asistente.Patterns.Transform.Pattern.Move(posicionInicial.Left + 40, posicionInicial.Top + 30);
                Assert.IsTrue(
                    Retry.WhileFalse(() => asistente.BoundingRectangle.Location != posicionInicial.Location, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(200)).Success,
                    "La ventana del asistente no se pudo mover durante la espera.");

                Window bitacora = AbrirBitacora(principal);
                Assert.IsNotNull(bitacora, "El shell debe permitir navegar a otra ventana durante la espera.");
                Assert.IsTrue(bloqueo.LecturaSigueRetenida(), "La verificacion debe ocurrir con la lectura todavia retenida.");
            }

            MessageBoxWindow error = App.EsperarMessageBox(TimeSpan.FromSeconds(30));
            StringAssert.Contains(error.Texto, TestDatabase.ObtenerTraduccion("Error.Assistant.ApiKeyMissing"));
            error.CerrarConOk();

            Assert.IsFalse(
                Retry.WhileFalse(() => !App.SinMessageBoxAbierto(), TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(200)).Success,
                "Un solo envio debe producir un solo resultado.");
            Assert.IsTrue(
                Retry.WhileFalse(() => UiFind.Boton(asistente, "btnEnviarMensajeAsistente").IsEnabled, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(200)).Success,
                "El envio debe rehabilitarse tras el error.");
            Assert.AreEqual(Pregunta, UiFind.LeerTexto(asistente, "txtbMensajeAsistente"), "Ante un error se conserva la pregunta para reintentar.");
            Assert.AreEqual(0, TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.ConsultaChatbot WHERE Pregunta = @p",
                new SqlParameter("@p", Pregunta)), "Un envio fallido no debe persistir el intercambio.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ChatAsistente_CerrarConEnvioPendiente_NoMuestraDialogoTardioYElShellSigueOperativo()
        {
            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window asistente = AbrirAsistente(principal);
            UiFind.EscribirTexto(asistente, "txtbMensajeAsistente", Pregunta);

            using (BloqueoConsultaChatbot bloqueo = BloqueoConsultaChatbot.Tomar())
            {
                UiFind.Click(asistente, "btnEnviarMensajeAsistente");
                Assert.IsTrue(bloqueo.EsperarLecturaRetenida(TimeSpan.FromSeconds(10)), "El envio no llego a la lectura del historial retenida.");

                asistente.Close();
                Assert.IsTrue(App.VentanaCerrada(IdFormularioAsistente, TimeSpan.FromSeconds(5)), "El formulario debe poder cerrarse con un envio pendiente.");
                Assert.IsTrue(bloqueo.LecturaSigueRetenida(), "El cierre debe ocurrir con la lectura todavia retenida.");
            }

            // Al liberar, el envio termina con error (sin clave): el dialogo del formulario cerrado no debe aparecer.
            Assert.IsFalse(
                Retry.WhileFalse(() => !App.SinMessageBoxAbierto(), TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(250)).Success,
                "No debe mostrarse un dialogo asociado al formulario ya cerrado.");
            Assert.IsTrue(VentanaRespondeMensajes(principal), "El shell debe seguir procesando mensajes tras la respuesta tardia.");

            Window reabierto = AbrirAsistente(principal);
            Assert.IsTrue(UiFind.Boton(reabierto, "btnEnviarMensajeAsistente").IsEnabled, "Un formulario nuevo no hereda el envio del cerrado.");
        }

        // Guardrails (change guardrails-asistente): el prefiltro local rechaza la pregunta sin llamar a
        // Gemini, asi que el camino se puede recorrer con la copia stageada sin clave de API.
        [TestMethod]
        [TestCategory("UI")]
        public void FIN_ChatAsistente_PreguntaFueraDeTema_MuestraRechazoFijoSinDialogoNiPersistencia()
        {
            const string preguntaFueraDeTema = "Pasame una receta de cocina para la prueba de interfaz";
            string textoRechazo = TestDatabase.ObtenerTraduccion("Assistant.Rejection.OutOfScope");
            int rechazosPrevios = TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.Bitacora WHERE Actividad = 'FormPersonalAssistant.Value.Activity.QuestionRejected'");

            Window principal = LoginComo(TestConfig.AdminUsername, TestConfig.AdminPassword);
            Window asistente = AbrirAsistente(principal);
            UiFind.EscribirTexto(asistente, "txtbMensajeAsistente", preguntaFueraDeTema);
            UiFind.Click(asistente, "btnEnviarMensajeAsistente");

            Assert.IsTrue(
                Retry.WhileFalse(() => LeerHistorial(asistente).Contains(textoRechazo), TimeSpan.FromSeconds(15), TimeSpan.FromMilliseconds(250)).Success,
                "El rechazo debe mostrarse como mensaje del asistente con el texto fijo traducido.");
            Assert.IsTrue(App.SinMessageBoxAbierto(), "Un rechazo no es un error: no debe abrir un dialogo.");
            Assert.IsTrue(
                Retry.WhileFalse(() => UiFind.Boton(asistente, "btnEnviarMensajeAsistente").IsEnabled, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(200)).Success,
                "El envio debe rehabilitarse tras el rechazo.");
            Assert.AreEqual(0, TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.ConsultaChatbot WHERE Pregunta = @p",
                new SqlParameter("@p", preguntaFueraDeTema)), "Una pregunta rechazada no se persiste en el historial.");
            Assert.AreEqual(rechazosPrevios + 1, TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.Bitacora WHERE Actividad = 'FormPersonalAssistant.Value.Activity.QuestionRejected' " +
                "AND Informacion = 'FormPersonalAssistant.Value.Info.Rejected.Prefilter'"), "El rechazo debe quedar en Bitacora con su motivo.");
        }

        private static string LeerHistorial(Window asistente)
        {
            AutomationElement historial = UiFind.Elemento(asistente, "rtbHistorialAsistente");
            var valor = historial.Patterns.Value.PatternOrDefault;
            if (valor != null)
            {
                return valor.Value.ValueOrDefault ?? string.Empty;
            }

            var texto = historial.Patterns.Text.PatternOrDefault;
            return texto == null ? historial.Name ?? string.Empty : texto.DocumentRange.GetText(-1) ?? string.Empty;
        }

        private Window AbrirAsistente(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuAsistente, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.PersonalAssistantChat"));
            return App.EsperarVentana(IdFormularioAsistente);
        }

        // WM_NULL con SMTO_ABORTIFHUNG: solo vuelve con exito si el hilo de la ventana bombea mensajes.
        private static bool VentanaRespondeMensajes(Window ventana)
        {
            IntPtr handle = ventana.Properties.NativeWindowHandle.Value;
            UIntPtr resultado;
            return SendMessageTimeout(handle, WmNull, UIntPtr.Zero, IntPtr.Zero, SmtoAbortIfHung, 2000, out resultado) != IntPtr.Zero;
        }

        private const uint WmNull = 0x0000;
        private const uint SmtoAbortIfHung = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam, uint flags, uint timeout, out UIntPtr result);

        /// <summary>
        /// Bloqueo exclusivo sobre dbo.ConsultaChatbot dentro de una transaccion abierta (sin escribir datos),
        /// revertida al liberar. Con READ COMMITTED sin snapshot, la lectura del historial de la app espera.
        /// </summary>
        private sealed class BloqueoConsultaChatbot : IDisposable
        {
            private readonly SqlConnection _conexion;
            private readonly SqlTransaction _transaccion;
            private readonly short _sesionId;

            private BloqueoConsultaChatbot(SqlConnection conexion, SqlTransaction transaccion, short sesionId)
            {
                _conexion = conexion;
                _transaccion = transaccion;
                _sesionId = sesionId;
            }

            public static BloqueoConsultaChatbot Tomar()
            {
                SqlConnection conexion = new SqlConnection(TestConfig.ConnectionString);
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();
                using (SqlCommand comando = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.ConsultaChatbot WITH (TABLOCKX, HOLDLOCK); SELECT CAST(@@SPID AS SMALLINT);",
                    conexion,
                    transaccion))
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    lector.NextResult();
                    lector.Read();
                    short sesionId = lector.GetInt16(0);
                    return new BloqueoConsultaChatbot(conexion, transaccion, sesionId);
                }
            }

            public bool EsperarLecturaRetenida(TimeSpan timeout)
            {
                return Retry.WhileFalse(LecturaSigueRetenida, timeout, TimeSpan.FromMilliseconds(200)).Success;
            }

            public bool LecturaSigueRetenida()
            {
                return TestDatabase.Contar(
                    "SELECT COUNT(*) FROM sys.dm_exec_requests WHERE blocking_session_id = @sesion",
                    new SqlParameter("@sesion", _sesionId)) > 0;
            }

            public void Dispose()
            {
                try
                {
                    _transaccion.Rollback();
                }
                finally
                {
                    _conexion.Dispose();
                }
            }
        }
    }
}
