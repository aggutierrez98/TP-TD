using System;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;

namespace TpIDS.UiTests.Infrastructure
{
    /// <summary>
    /// Wrapper de un MessageBox nativo (clase #32770) mostrado por UiDialogHelper.
    /// Los botones nativos usan AutomationIds numericos: 1=OK, 2=Cancelar/OK-solo, 6=Si, 7=No.
    /// </summary>
    internal sealed class MessageBoxWindow
    {
        private readonly Window _window;

        public MessageBoxWindow(Window window)
        {
            _window = window;
        }

        public string Titulo
        {
            get { return _window.Title; }
        }

        public string Texto
        {
            get
            {
                AutomationElement[] textos = _window.FindAllDescendants(
                    condicion => condicion.ByControlType(ControlType.Text));

                return string.Join(
                    Environment.NewLine,
                    textos.Select(texto => texto.Name).Where(nombre => !string.IsNullOrWhiteSpace(nombre)));
            }
        }

        public void CerrarConOk()
        {
            ClickBoton("2", "1");
        }

        public void ConfirmarSi()
        {
            ClickBoton("6");
        }

        public void RechazarNo()
        {
            ClickBoton("7");
        }

        public void EsperarCierre(TimeSpan timeout)
        {
            Retry.WhileFalse(
                () =>
                {
                    try
                    {
                        return _window.IsOffscreen;
                    }
                    catch
                    {
                        // El elemento ya no existe: el dialogo se cerro.
                        return true;
                    }
                },
                timeout,
                TimeSpan.FromMilliseconds(100));
        }

        private void ClickBoton(params string[] automationIds)
        {
            Button[] botones = _window.FindAllDescendants(
                    condicion => condicion.ByControlType(ControlType.Button))
                .Select(elemento => elemento.AsButton())
                .Where(boton => boton != null)
                .ToArray();

            if (botones.Length == 0)
            {
                throw new InvalidOperationException("El MessageBox '" + Titulo + "' no tiene botones visibles.");
            }

            try
            {
                _window.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta el click igual.
            }

            foreach (string automationId in automationIds)
            {
                Button boton = botones.FirstOrDefault(candidato => candidato.AutomationId == automationId);
                if (boton != null)
                {
                    // Click con mouse real: Invoke (InvokePattern) puede colgarse si al
                    // cerrar este dialogo el handler pendiente abre otro modal.
                    boton.Click();
                    return;
                }
            }

            botones[0].Click();
        }
    }
}
