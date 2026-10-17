using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Conditions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

namespace TpIDS.UiTests.Infrastructure
{
    /// <summary>
    /// Sesion de la app bajo test: lanza el TpIDS.exe staged (apuntado a la BD de test)
    /// y expone busqueda de ventanas/dialogos por AutomationId (Name de WinForms).
    /// </summary>
    internal sealed class UiAppSession : IDisposable
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(20);

        private readonly Application _app;
        private readonly UIA3Automation _automation;

        public UiAppSession()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(TestConfig.StagedExePath)
            {
                WorkingDirectory = TestConfig.StagingDirectory
            };

            _app = Application.Launch(startInfo);
            _automation = new UIA3Automation();
        }

        public Window EsperarVentana(string automationId)
        {
            return EsperarVentana(automationId, DefaultTimeout);
        }

        public Window EsperarVentana(string automationId, TimeSpan timeout)
        {
            RetryResult<Window> resultado = Retry.WhileNull(
                () => BuscarVentana(automationId),
                timeout,
                TimeSpan.FromMilliseconds(250));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No aparecio la ventana '" + automationId + "' en " + timeout.TotalSeconds + "s.");
            }

            return resultado.Result;
        }

        public Window BuscarVentana(string automationId)
        {
            if (_app.HasExited)
            {
                throw new InvalidOperationException("La app bajo test termino inesperadamente.");
            }

            return VentanasDelProceso()
                .FirstOrDefault(ventana => CoincideAutomationId(ventana, automationId));
        }

        public Window EsperarVentanaPorTitulo(string titulo)
        {
            var resultado = Retry.WhileNull(
                () => VentanasDelProceso().FirstOrDefault(ventana => ventana.Name == titulo),
                DefaultTimeout, TimeSpan.FromMilliseconds(250));
            if (resultado.Result == null)
                throw new InvalidOperationException("No aparecio la ventana '" + titulo + "'.");
            return resultado.Result;
        }

        // Recorrer todo el arbol (no solo ControlType.Window) para llegar a los hijos MDI
        // expone tambien elementos Window "internos" (p. ej. del propio MdiClient) que no
        // soportan AutomationId via UIA: se descartan en vez de cortar la busqueda.
        private static bool CoincideAutomationId(Window ventana, string automationId)
        {
            try
            {
                return ventana.AutomationId == automationId;
            }
            catch (FlaUI.Core.Exceptions.PropertyNotSupportedException)
            {
                return false;
            }
        }

        public bool VentanaCerrada(string automationId, TimeSpan timeout)
        {
            RetryResult<bool> resultado = Retry.WhileFalse(
                () => _app.HasExited || BuscarVentana(automationId) == null,
                timeout,
                TimeSpan.FromMilliseconds(250));

            return resultado.Success;
        }

        public MessageBoxWindow EsperarMessageBox()
        {
            return EsperarMessageBox(DefaultTimeout);
        }

        public MessageBoxWindow EsperarMessageBox(TimeSpan timeout)
        {
            RetryResult<Window> resultado = Retry.WhileNull(
                () => VentanasDelProceso().FirstOrDefault(EsMessageBox),
                timeout,
                TimeSpan.FromMilliseconds(250));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException("No aparecio ningun MessageBox en " + timeout.TotalSeconds + "s.");
            }

            return new MessageBoxWindow(resultado.Result);
        }

        public bool SinMessageBoxAbierto()
        {
            return !VentanasDelProceso().Any(EsMessageBox);
        }

        // UIA puede negar ClassName de una ventana que se esta creando o cerrando justo al enumerar:
        // esa ventana todavia no cuenta como MessageBox y se vuelve a mirar en el proximo intento.
        private static bool EsMessageBox(Window ventana)
        {
            try
            {
                return ventana.ClassName == "#32770";
            }
            catch (FlaUI.Core.Exceptions.PropertyNotSupportedException)
            {
                return false;
            }
        }

        public string CapturarPantalla(string nombreArchivo)
        {
            Directory.CreateDirectory(TestConfig.ScreenshotDirectory);
            string destino = Path.Combine(TestConfig.ScreenshotDirectory, nombreArchivo + ".png");

            using (CaptureImage captura = Capture.Screen())
            {
                captura.ToFile(destino);
            }

            return destino;
        }

        public void Dispose()
        {
            try
            {
                if (!_app.HasExited)
                {
                    // Close intenta el cierre ordenado y mata el proceso si no responde.
                    _app.Close(true);
                }
            }
            catch
            {
                // El proceso pudo terminar entre el chequeo y el cierre; no ensuciar el resultado del test.
            }
            finally
            {
                _automation.Dispose();
                _app.Dispose();
            }
        }

        private Window[] VentanasDelProceso()
        {
            ConditionFactory condiciones = _automation.ConditionFactory;
            AutomationElement[] topLevel = _automation.GetDesktop()
                .FindAllChildren(condiciones.ByProcessId(_app.ProcessId));

            var ventanas = new System.Collections.Generic.List<Window>();
            foreach (AutomationElement elemento in topLevel)
            {
                AgregarVentanas(elemento, condiciones, ventanas, 3);
            }

            return ventanas.ToArray();
        }

        // En UIA3 los modales (MessageBox, ShowDialog) cuelgan de la ventana dueña,
        // no del desktop, y pueden anidarse (FormLogin > FormRegistro > MessageBox):
        // se recorre en profundidad acotada la cadena de ventanas hijas. No se filtra
        // por ControlType.Window en la recursion (solo al decidir si agregar un
        // elemento): un hijo MDI (FormGestionUsuarios, etc.) cuelga de un MdiClient
        // nativo que no es el mismo ControlType.Window, y filtrar ahi cortaria la
        // busqueda antes de llegar al hijo MDI real.
        private static void AgregarVentanas(
            AutomationElement elemento,
            ConditionFactory condiciones,
            System.Collections.Generic.List<Window> ventanas,
            int profundidadRestante)
        {
            Window ventana = elemento.AsWindow();
            if (ventana != null)
            {
                ventanas.Add(ventana);
            }

            if (profundidadRestante <= 0)
            {
                return;
            }

            foreach (AutomationElement hijo in elemento.FindAllChildren())
            {
                AgregarVentanas(hijo, condiciones, ventanas, profundidadRestante - 1);
            }
        }
    }
}
