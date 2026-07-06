using System;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;

namespace TpIDS.UiTests.Infrastructure
{
    /// <summary>
    /// Busqueda de controles WinForms por AutomationId (= propiedad Name del control),
    /// con reintentos porque los formularios cargan datos en el evento Load.
    /// </summary>
    internal static class UiFind
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        public static AutomationElement Elemento(AutomationElement raiz, string automationId)
        {
            return Elemento(raiz, automationId, DefaultTimeout);
        }

        public static AutomationElement Elemento(AutomationElement raiz, string automationId, TimeSpan timeout)
        {
            RetryResult<AutomationElement> resultado = Retry.WhileNull(
                () => raiz.FindFirstDescendant(condicion => condicion.ByAutomationId(automationId)),
                timeout,
                TimeSpan.FromMilliseconds(200));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el control '" + automationId + "' dentro de '" + raiz.AutomationId + "'.");
            }

            return resultado.Result;
        }

        public static Button Boton(AutomationElement raiz, string automationId)
        {
            return Elemento(raiz, automationId).AsButton();
        }

        /// <summary>
        /// Click con mouse real (no InvokePattern): Invoke espera a que el handler
        /// termine y agota el timeout COM cuando el handler abre un MessageBox modal.
        /// </summary>
        public static void Click(Window ventana, string automationId)
        {
            Button boton = Boton(ventana, automationId);

            try
            {
                ventana.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta el click igual.
            }

            boton.Click();
        }

        public static TextBox CajaTexto(AutomationElement raiz, string automationId)
        {
            return Elemento(raiz, automationId).AsTextBox();
        }

        /// <summary>
        /// PasswordTextBox es un UserControl propio: el Edit real es su hijo txtbInput.
        /// </summary>
        public static TextBox CajaPassword(AutomationElement raiz, string automationId)
        {
            AutomationElement contenedor = Elemento(raiz, automationId);
            return Elemento(contenedor, "txtbInput").AsTextBox();
        }

        /// <summary>
        /// Escribe texto inyectando el valor por ValuePattern (no simula teclado),
        /// para que el resultado no dependa del foco si el usuario toca otra ventana.
        /// </summary>
        public static void EscribirTexto(AutomationElement raiz, string automationId, string valor)
        {
            EstablecerValor(Elemento(raiz, automationId), valor);
        }

        public static void EscribirPassword(AutomationElement raiz, string automationId, string valor)
        {
            AutomationElement contenedor = Elemento(raiz, automationId);
            EstablecerValor(Elemento(contenedor, "txtbInput"), valor);
        }

        /// <summary>Marca o desmarca un CheckBox por TogglePattern (sin teclado).</summary>
        public static void MarcarCheckBox(AutomationElement raiz, string automationId, bool marcado)
        {
            CheckBox checkBox = Elemento(raiz, automationId).AsCheckBox();
            if (checkBox.IsChecked != marcado)
            {
                checkBox.IsChecked = marcado;
            }
        }

        /// <summary>
        /// Selecciona la fila de un DataGridView cuya celda coincide con el texto dado.
        /// </summary>
        public static void SeleccionarFilaGrilla(AutomationElement raiz, string automationId, string texto)
        {
            AutomationElement grilla = Elemento(raiz, automationId);

            RetryResult<AutomationElement> resultado = Retry.WhileNull(
                () => BuscarFilaPorTexto(grilla, texto),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromMilliseconds(300));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro una fila con '" + texto + "' en la grilla '" + automationId + "'.");
            }

            // Select() sobre una fila fuera de la vista falla en silencio: scrollear primero.
            ScrollFilaALaVista(resultado.Result);

            var seleccion = resultado.Result.Patterns.SelectionItem.PatternOrDefault;
            if (seleccion != null)
            {
                seleccion.Select();
                return;
            }

            resultado.Result.Click();
        }

        /// <summary>Click de mouse real sobre una fila (da foco de teclado a la grilla).</summary>
        public static void ClickFilaGrilla(Window ventana, string automationId, string texto)
        {
            AutomationElement grilla = Elemento(ventana, automationId);

            RetryResult<AutomationElement> resultado = Retry.WhileNull(
                () => BuscarFilaPorTexto(grilla, texto),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(300));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro una fila con '" + texto + "' en la grilla '" + automationId + "'.");
            }

            ScrollFilaALaVista(resultado.Result);

            try
            {
                ventana.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta el click igual.
            }

            resultado.Result.Click();
        }

        private static void ScrollFilaALaVista(AutomationElement fila)
        {
            var scroll = fila.Patterns.ScrollItem.PatternOrDefault;
            if (scroll == null)
            {
                return;
            }

            try
            {
                scroll.ScrollIntoView();
            }
            catch
            {
                // Si el scroll falla, la seleccion/click posterior lo delata.
            }
        }

        private static AutomationElement BuscarFilaPorTexto(AutomationElement grilla, string texto)
        {
            foreach (AutomationElement fila in grilla.FindAllChildren(
                condicion => condicion.ByControlType(FlaUI.Core.Definitions.ControlType.Custom)
                    .Or(condicion.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem))))
            {
                foreach (AutomationElement celda in fila.FindAllChildren())
                {
                    string valor = null;
                    var pattern = celda.Patterns.Value.PatternOrDefault;
                    if (pattern != null)
                    {
                        valor = pattern.Value.ValueOrDefault;
                    }

                    if (valor == texto || celda.Name == texto)
                    {
                        return fila;
                    }
                }
            }

            return null;
        }

        /// <summary>Selecciona un item de ListBox por su texto visible.</summary>
        public static void SeleccionarItemLista(AutomationElement raiz, string automationId, string texto)
        {
            ListBox lista = Elemento(raiz, automationId).AsListBox();

            RetryResult<ListBoxItem> resultado = Retry.WhileNull(
                () => lista.Items.FirstOrDefault(item => item.Text == texto || item.Name == texto),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(300));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el item '" + texto + "' en la lista '" + automationId + "'.");
            }

            resultado.Result.Select();
        }

        /// <summary>
        /// Deja el DataGridView sin filas seleccionadas: selecciona la fila indicada
        /// (via SelectionItemPattern, que si dispara SelectionChanged) y la des-selecciona
        /// con Ctrl+click, que con MultiSelect=false togglea la fila. Ni SelectionPattern
        /// en la grilla ni IsSelected por fila son confiables en el bridge UIA de WinForms.
        /// </summary>
        public static void DeseleccionarFilaGrilla(Window ventana, string automationId, string textoFila)
        {
            SeleccionarFilaGrilla(ventana, automationId, textoFila);

            AutomationElement grilla = Elemento(ventana, automationId);
            AutomationElement fila = BuscarFilaPorTexto(grilla, textoFila);
            if (fila == null)
            {
                throw new InvalidOperationException(
                    "No se encontro la fila '" + textoFila + "' para des-seleccionar en '" + automationId + "'.");
            }

            ScrollFilaALaVista(fila);

            try
            {
                ventana.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta el click igual.
            }

            using (FlaUI.Core.Input.Keyboard.Pressing(FlaUI.Core.WindowsAPI.VirtualKeyShort.CONTROL))
            {
                fila.Click();
            }
        }

        /// <summary>Selecciona un nodo de TreeView por nombre, sin tocar su checkbox.</summary>
        public static void SeleccionarNodoArbol(Window ventana, string automationId, string nombreNodo)
        {
            AutomationElement arbol = Elemento(ventana, automationId);

            RetryResult<AutomationElement> resultado = Retry.WhileNull(
                () => arbol.FindFirstDescendant(condicion => condicion.ByName(nombreNodo)),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(300));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el nodo '" + nombreNodo + "' en el arbol '" + automationId + "'.");
            }

            var seleccion = resultado.Result.Patterns.SelectionItem.PatternOrDefault;
            if (seleccion != null)
            {
                seleccion.Select();
                return;
            }

            try
            {
                ventana.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta el click igual.
            }

            resultado.Result.Click();
        }

        /// <summary>
        /// Espera a que la grilla termine de bindear al menos la cantidad de filas dada
        /// (los formularios cargan datos en Load, despues de que la ventana ya es visible).
        /// </summary>
        public static void EsperarFilasGrilla(AutomationElement raiz, string automationId, int minimoFilas)
        {
            RetryResult<bool> resultado = Retry.WhileFalse(
                () => ContarFilasGrilla(raiz, automationId) >= minimoFilas,
                TimeSpan.FromSeconds(15),
                TimeSpan.FromMilliseconds(300));

            if (!resultado.Success)
            {
                throw new InvalidOperationException(
                    "La grilla '" + automationId + "' no llego a " + minimoFilas + " filas. Filas actuales: " +
                    ContarFilasGrilla(raiz, automationId) + ".");
            }
        }

        /// <summary>Cuenta las filas de datos de un DataGridView via GridPattern.</summary>
        public static int ContarFilasGrilla(AutomationElement raiz, string automationId)
        {
            AutomationElement grilla = Elemento(raiz, automationId);
            var grid = grilla.Patterns.Grid.PatternOrDefault;
            if (grid != null)
            {
                return grid.RowCount.ValueOrDefault;
            }

            return grilla.FindAllChildren(
                condicion => condicion.ByControlType(FlaUI.Core.Definitions.ControlType.Custom)
                    .Or(condicion.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem))).Length;
        }

        /// <summary>Lee el texto actual de un control con ValuePattern.</summary>
        public static string LeerTexto(AutomationElement raiz, string automationId)
        {
            var pattern = Elemento(raiz, automationId).Patterns.Value.PatternOrDefault;
            if (pattern == null)
            {
                throw new InvalidOperationException(
                    "El control '" + automationId + "' no soporta ValuePattern para leer texto.");
            }

            return pattern.Value.ValueOrDefault ?? string.Empty;
        }

        /// <summary>Selecciona un item de ComboBox por su texto visible.</summary>
        public static void SeleccionarItemCombo(AutomationElement raiz, string automationId, string texto)
        {
            ComboBox combo = Elemento(raiz, automationId).AsComboBox();
            combo.Expand();

            RetryResult<ComboBoxItem> resultado = Retry.WhileNull(
                () => combo.Items.FirstOrDefault(item => item.Text == texto || item.Name == texto),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(200));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el item '" + texto + "' en el combo '" + automationId + "'.");
            }

            resultado.Result.Select();

            try
            {
                combo.Collapse();
            }
            catch
            {
                // Algunos combos se colapsan solos al seleccionar.
            }
        }

        /// <summary>
        /// Marca el checkbox de un nodo de TreeView (CheckBoxes=true). Intenta TogglePattern
        /// y cae a seleccionar el nodo + tecla ESPACIO, que es como WinForms togglea el check.
        /// </summary>
        public static void MarcarNodoArbol(Window ventana, string automationId, string nombreNodo)
        {
            AutomationElement arbol = Elemento(ventana, automationId);

            RetryResult<AutomationElement> resultado = Retry.WhileNull(
                () => arbol.FindFirstDescendant(condicion => condicion.ByName(nombreNodo)),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(300));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el nodo '" + nombreNodo + "' en el arbol '" + automationId + "'.");
            }

            AutomationElement nodo = resultado.Result;
            var toggle = nodo.Patterns.Toggle.PatternOrDefault;
            if (toggle != null)
            {
                if (toggle.ToggleState.ValueOrDefault != FlaUI.Core.Definitions.ToggleState.On)
                {
                    toggle.Toggle();
                }

                return;
            }

            try
            {
                ventana.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta igual.
            }

            var seleccion = nodo.Patterns.SelectionItem.PatternOrDefault;
            if (seleccion != null)
            {
                seleccion.Select();
            }
            else
            {
                nodo.Click();
            }

            FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.SPACE);
        }

        private static void EstablecerValor(AutomationElement elemento, string valor)
        {
            var pattern = elemento.Patterns.Value.PatternOrDefault;
            if (pattern == null)
            {
                throw new InvalidOperationException(
                    "El control '" + elemento.AutomationId + "' no soporta ValuePattern para escribir texto.");
            }

            pattern.SetValue(valor ?? string.Empty);

            // Los Edit de password no exponen su valor por UIA: si la lectura viene
            // vacia no se puede verificar y se confia en el SetValue. Solo es error
            // una lectura no vacia distinta del valor pedido.
            RetryResult<bool> verificacion = Retry.WhileFalse(
                () =>
                {
                    string leido = pattern.Value.ValueOrDefault;
                    return string.IsNullOrEmpty(leido) || leido == (valor ?? string.Empty);
                },
                TimeSpan.FromSeconds(3),
                TimeSpan.FromMilliseconds(100));

            if (!verificacion.Success)
            {
                throw new InvalidOperationException(
                    "El control '" + elemento.AutomationId + "' no tomo el valor esperado. Valor leido: '" +
                    pattern.Value.ValueOrDefault + "'.");
            }
        }
    }
}
