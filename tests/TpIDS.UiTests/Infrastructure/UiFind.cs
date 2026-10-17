using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
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

        /// <summary>
        /// Navega el MenuStrip principal (AutomationId "menuStripPrincipal"): expande el
        /// item de primer nivel y clickea el item dentro de su dropdown, matcheando por
        /// texto visible (Name) ya traducido. Usa Menu.Items / MenuItem.Items en vez de
        /// FindFirstDescendant porque los ToolStripMenuItem no se materializan como
        /// descendientes normales en el arbol de automatizacion de WinForms, y matchea
        /// por Name (no AutomationId) porque los items de primer nivel de un MenuStrip
        /// no soportan la propiedad AutomationId via UIA (PropertyNotSupportedException).
        /// </summary>
        public static void ClickMenu(Window ventana, string textoMenuTop, string textoItem)
        {
            try
            {
                ventana.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta igual.
            }

            Menu menuStrip = Elemento(ventana, "menuStripPrincipal").AsMenu();

            RetryResult<MenuItem> menuTopResultado = Retry.WhileNull(
                () => menuStrip.Items.FirstOrDefault(item => CoincideNombre(item, textoMenuTop)),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(200));

            if (!menuTopResultado.Success || menuTopResultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el menu de primer nivel '" + textoMenuTop + "'.");
            }

            MenuItem menuTop = menuTopResultado.Result;
            menuTop.Expand();

            RetryResult<MenuItem> resultado = Retry.WhileNull(
                () => menuTop.Items.FirstOrDefault(item => CoincideNombre(item, textoItem)),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(200));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el item de menu '" + textoItem + "' dentro de '" + textoMenuTop + "'.");
            }

            resultado.Result.Click();
        }

        /// <summary>Lee el estado Enabled de un item de primer nivel del MenuStrip principal, sin expandirlo.</summary>
        public static bool MenuTopHabilitado(Window ventana, string textoMenuTop)
        {
            Menu menuStrip = Elemento(ventana, "menuStripPrincipal").AsMenu();

            RetryResult<MenuItem> resultado = Retry.WhileNull(
                () => menuStrip.Items.FirstOrDefault(item => CoincideNombre(item, textoMenuTop)),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromMilliseconds(200));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el menu de primer nivel '" + textoMenuTop + "'.");
            }

            return resultado.Result.IsEnabled;
        }

        // Los separadores de un dropdown se enumeran como MenuItem igual que los items
        // reales: se descartan en vez de cortar el escaneo de Items si alguna propiedad
        // no esta soportada para ese elemento puntual.
        private static bool CoincideNombre(MenuItem item, string texto)
        {
            try
            {
                return item.Name == texto;
            }
            catch (FlaUI.Core.Exceptions.PropertyNotSupportedException)
            {
                return false;
            }
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

        /// <summary>
        /// Establece el valor de un NumericUpDown. Primero intenta RangeValuePattern (setea Value
        /// directo); si el spinner no lo expone, escribe en su caja interna con el teclado y
        /// confirma con TAB, que es cuando el control toma el texto como valor.
        /// </summary>
        public static void EstablecerNumero(Window ventana, string automationId, int valor)
        {
            AutomationElement control = Elemento(ventana, automationId);

            var rango = control.Patterns.RangeValue.PatternOrDefault;
            if (rango != null && !rango.IsReadOnly.ValueOrDefault)
            {
                rango.SetValue(valor);
                return;
            }

            AutomationElement caja = control.FindFirstDescendant(
                condicion => condicion.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
            if (caja == null)
            {
                throw new InvalidOperationException(
                    "El control '" + automationId + "' no expone RangeValuePattern ni una caja de texto interna.");
            }

            try
            {
                ventana.SetForeground();
            }
            catch
            {
                // Si no se puede traer al frente se intenta igual.
            }

            caja.Focus();
            FlaUI.Core.Input.Keyboard.TypeSimultaneously(
                FlaUI.Core.WindowsAPI.VirtualKeyShort.CONTROL,
                FlaUI.Core.WindowsAPI.VirtualKeyShort.KEY_A);
            FlaUI.Core.Input.Keyboard.Type(valor.ToString(CultureInfo.InvariantCulture));
            FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.TAB);
        }

        /// <summary>Establece la fecha de un DateTimePicker (control Win32, sin ValuePattern por UIA).</summary>
        public static void EstablecerFecha(AutomationElement raiz, string automationId, DateTime valor)
        {
            AutomationElement selector = Elemento(raiz, automationId);

            try
            {
                selector.AsDateTimePicker().SelectedDate = valor;
                return;
            }
            catch (Exception)
            {
                // KryptonDateTimePicker no expone el patron que usa FlaUI: se escribe por teclado.
            }

            EscribirFechaPorTeclado(selector, valor);
        }

        /// <summary>
        /// Escribe la fecha como un usuario: clic en el primer segmento y digitos segmento a
        /// segmento (RIGHT entre ellos), respetando el orden dia/mes/anio de la cultura actual.
        /// </summary>
        private static void EscribirFechaPorTeclado(AutomationElement selector, DateTime valor)
        {
            var rectangulo = selector.BoundingRectangle;
            FlaUI.Core.Input.Mouse.Click(new System.Drawing.Point(rectangulo.Left + 10, rectangulo.Top + rectangulo.Height / 2));
            Thread.Sleep(250);

            string patron = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            string orden = new string(patron.Where(c => c == 'd' || c == 'M' || c == 'y').Distinct().ToArray());

            for (int i = 0; i < orden.Length; i++)
            {
                string segmento = orden[i] == 'd' ? valor.Day.ToString("00")
                    : orden[i] == 'M' ? valor.Month.ToString("00")
                    : valor.Year.ToString("0000");

                FlaUI.Core.Input.Keyboard.Type(segmento);
                Thread.Sleep(200);

                if (i < orden.Length - 1)
                {
                    FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.RIGHT);
                    Thread.Sleep(150);
                }
            }
        }

        /// <summary>Lee la fecha actual de un DateTimePicker.</summary>
        public static DateTime LeerFecha(AutomationElement raiz, string automationId)
        {
            return Elemento(raiz, automationId).AsDateTimePicker().SelectedDate.Value;
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

            ClickEnFila(resultado.Result);
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

            ClickEnFila(resultado.Result);
        }

        /// <summary>
        /// Clic de mouse sobre una fila. Las filas del DataGridView nativo se clickean directo;
        /// las de KryptonDataGridView no exponen SelectionItem y el clic sobre el elemento fila
        /// no selecciona, asi que se clickea una de sus celdas.
        /// </summary>
        private static void ClickEnFila(AutomationElement fila)
        {
            if (fila.Patterns.SelectionItem.IsSupported)
            {
                fila.Click();
                return;
            }

            AutomationElement celda = fila.FindAllChildren().FirstOrDefault(hijo => hijo.Patterns.Value.IsSupported);
            (celda ?? fila).Click();
        }

        private static void ScrollFilaALaVista(AutomationElement fila)
        {
            var scroll = fila.Patterns.ScrollItem.PatternOrDefault;
            if (scroll != null)
            {
                try
                {
                    scroll.ScrollIntoView();
                }
                catch
                {
                    // Si el scroll falla, la seleccion/click posterior lo delata.
                }
            }

            ScrollConRuedaHastaQueSeaClickeable(fila);
        }

        /// <summary>
        /// ScrollIntoView del bridge UIA de WinForms no siempre trae la fila a la zona visible de una
        /// grilla con muchas filas (la fila existe en el arbol pero no tiene punto clickeable). Si la
        /// fila ya es clickeable no hace nada; si no, mueve la rueda del mouse sobre la grilla en pasos
        /// chicos, hacia la direccion donde esta la fila, hasta que quede dentro de su area visible.
        /// </summary>
        private static void ScrollConRuedaHastaQueSeaClickeable(AutomationElement fila)
        {
            const int MaximoPasos = 40;

            if (EsClickeableDentroDeGrilla(fila))
            {
                return;
            }

            AutomationElement grilla = fila.Parent;
            if (grilla == null)
            {
                return;
            }

            for (int paso = 0; paso < MaximoPasos; paso++)
            {
                Rectangle area = grilla.BoundingRectangle;
                if (area.IsEmpty)
                {
                    return;
                }

                Rectangle filaRect = fila.BoundingRectangle;
                bool filaAbajo = filaRect.IsEmpty || filaRect.Top >= area.Bottom || filaRect.Bottom > area.Bottom;

                Mouse.MoveTo(new Point(area.Left + (area.Width / 2), area.Top + (area.Height / 2)));
                Mouse.Scroll(filaAbajo ? -3 : 3);
                Thread.Sleep(120);

                if (EsClickeableDentroDeGrilla(fila))
                {
                    return;
                }
            }
        }

        private static bool EsClickeableDentroDeGrilla(AutomationElement fila)
        {
            AutomationElement grilla = fila.Parent;
            Rectangle filaRect;
            try
            {
                filaRect = fila.BoundingRectangle;
            }
            catch
            {
                return false;
            }

            if (filaRect.IsEmpty || fila.IsOffscreen)
            {
                return false;
            }

            if (grilla != null)
            {
                Rectangle area = grilla.BoundingRectangle;
                if (!area.IsEmpty)
                {
                    int centroY = filaRect.Top + (filaRect.Height / 2);
                    if (centroY <= area.Top || centroY >= area.Bottom)
                    {
                        return false;
                    }
                }
            }

            return fila.TryGetClickablePoint(out Point _);
        }

        private static AutomationElement BuscarFilaPorTexto(AutomationElement grilla, string texto)
        {
            AutomationElement[] filas = grilla.FindAllChildren(
                condicion => condicion.ByControlType(FlaUI.Core.Definitions.ControlType.Custom)
                    .Or(condicion.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem)));

            // Primero por valor de celda. KryptonDataGridView nombra las filas y sus encabezados
            // con base 0 ('Fila 1' es la segunda), asi que comparar por Name puede elegir la fila equivocada.
            foreach (AutomationElement fila in filas)
            {
                foreach (AutomationElement celda in fila.FindAllChildren())
                {
                    var pattern = celda.Patterns.Value.PatternOrDefault;
                    if (pattern != null && pattern.Value.ValueOrDefault == texto)
                    {
                        return fila;
                    }
                }
            }

            // Respaldo: por Name de la celda (comportamiento original).
            foreach (AutomationElement fila in filas)
            {
                foreach (AutomationElement celda in fila.FindAllChildren())
                {
                    if (celda.Name == texto)
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
                ClickEnFila(fila);
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
            ComboBox combo = ResolverCombo(Elemento(raiz, automationId));
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
        /// Selecciona un item de ComboBox navegando con el teclado (HOME + flecha abajo), como
        /// un usuario real. A diferencia de <see cref="SeleccionarItemCombo"/>, dispara
        /// SelectedIndexChanged: SelectionItemPattern.Select() cambia la seleccion nativa
        /// sin emitir CBN_SELCHANGE, asi que no sirve para combos que cargan datos dependientes.
        /// </summary>
        public static void SeleccionarItemComboConTeclado(AutomationElement raiz, string automationId, string texto)
        {
            AutomationElement elemento = Elemento(raiz, automationId);
            if (!elemento.Patterns.ExpandCollapse.IsSupported)
            {
                SeleccionarItemComboKryptonConTeclado(elemento, automationId, texto);
                return;
            }

            ComboBox combo = elemento.AsComboBox();
            int cantidad = combo.Items.Length;
            combo.Focus();
            FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.HOME);

            for (int i = 0; i <= cantidad; i++)
            {
                FlaUI.Core.Input.Wait.UntilInputIsProcessed();
                ComboBoxItem seleccionado = combo.SelectedItem;
                if (seleccionado != null && (seleccionado.Text == texto || seleccionado.Name == texto))
                {
                    return;
                }

                FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.DOWN);
            }

            throw new InvalidOperationException(
                "No se encontro el item '" + texto + "' en el combo '" + automationId + "'.");
        }

        /// <summary>
        /// KryptonComboBox envuelve un ComboBox nativo como hijo UIA (el wrapper solo expone Value):
        /// si el elemento no soporta ExpandCollapse se usa ese hijo, donde corre la logica nativa.
        /// </summary>
        private static ComboBox ResolverCombo(AutomationElement elemento)
        {
            if (elemento.Patterns.ExpandCollapse.IsSupported)
            {
                return elemento.AsComboBox();
            }

            AutomationElement interno = elemento.FindFirstDescendant(
                condicion => condicion.ByControlType(FlaUI.Core.Definitions.ControlType.ComboBox));

            return (interno ?? elemento).AsComboBox();
        }

        /// <summary>
        /// Variante por teclado para KryptonComboBox: HOME y flecha abajo leyendo el texto por
        /// ValuePattern hasta dar con el item; si el texto deja de cambiar, llego al final.
        /// </summary>
        private static void SeleccionarItemComboKryptonConTeclado(AutomationElement combo, string automationId, string texto)
        {
            combo.Focus();
            FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.HOME);

            string anterior = null;
            for (int paso = 0; paso < 500; paso++)
            {
                FlaUI.Core.Input.Wait.UntilInputIsProcessed();
                var valor = combo.Patterns.Value.PatternOrDefault;
                string actual = valor == null ? null : valor.Value.ValueOrDefault;

                if (actual == texto)
                {
                    return;
                }

                if (paso > 0 && actual == anterior)
                {
                    break;
                }

                anterior = actual;
                FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.DOWN);
            }

            throw new InvalidOperationException(
                "No se encontro el item '" + texto + "' en el combo '" + automationId + "'.");
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

        /// <summary>
        /// Marca el item de un CheckedListBox cuyo texto visible contiene el texto dado.
        /// Intenta TogglePattern; si no esta, usa la accion default MSAA (Check/Uncheck)
        /// segun el estado CHECKED; y como ultimo recurso selecciona el item + ESPACIO.
        /// </summary>
        public static void MarcarItemListaChequeable(Window ventana, string automationId, string textoContenido)
        {
            AutomationElement lista = Elemento(ventana, automationId);

            RetryResult<AutomationElement> resultado = Retry.WhileNull(
                () => lista.FindAllDescendants().FirstOrDefault(item => NombreContiene(item, textoContenido)),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(300));

            if (!resultado.Success || resultado.Result == null)
            {
                throw new InvalidOperationException(
                    "No se encontro un item con '" + textoContenido + "' en la lista '" + automationId + "'.");
            }

            AutomationElement itemLista = resultado.Result;

            var toggle = itemLista.Patterns.Toggle.PatternOrDefault;
            if (toggle != null)
            {
                if (toggle.ToggleState.ValueOrDefault != FlaUI.Core.Definitions.ToggleState.On)
                {
                    toggle.Toggle();
                }

                return;
            }

            var legacy = itemLista.Patterns.LegacyIAccessible.PatternOrDefault;
            if (legacy != null)
            {
                bool marcado = (legacy.State.ValueOrDefault & FlaUI.Core.WindowsAPI.AccessibilityState.STATE_SYSTEM_CHECKED) != 0;
                if (!marcado)
                {
                    legacy.DoDefaultAction();
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

            itemLista.Click();
            FlaUI.Core.Input.Keyboard.Type(FlaUI.Core.WindowsAPI.VirtualKeyShort.SPACE);
        }

        /// <summary>
        /// Estado de marca del item de un CheckedListBox cuyo texto visible contiene el texto dado:
        /// true/false segun este marcado, null si la lista no tiene ese item.
        /// </summary>
        public static bool? EstadoItemListaChequeable(AutomationElement raiz, string automationId, string textoContenido)
        {
            AutomationElement lista = Elemento(raiz, automationId);
            AutomationElement itemLista = lista.FindAllDescendants().FirstOrDefault(item => NombreContiene(item, textoContenido));
            if (itemLista == null)
            {
                return null;
            }

            var toggle = itemLista.Patterns.Toggle.PatternOrDefault;
            if (toggle != null)
            {
                return toggle.ToggleState.ValueOrDefault == FlaUI.Core.Definitions.ToggleState.On;
            }

            var legacy = itemLista.Patterns.LegacyIAccessible.PatternOrDefault;
            if (legacy != null)
            {
                return (legacy.State.ValueOrDefault & FlaUI.Core.WindowsAPI.AccessibilityState.STATE_SYSTEM_CHECKED) != 0;
            }

            throw new InvalidOperationException(
                "El item '" + textoContenido + "' de la lista '" + automationId + "' no expone su estado de marca.");
        }

        /// <summary>Texto del item seleccionado de un ComboBox (vacio si no hay seleccion).</summary>
        public static string LeerItemSeleccionadoCombo(AutomationElement raiz, string automationId)
        {
            ComboBox combo = ResolverCombo(Elemento(raiz, automationId));
            return combo.SelectedItem?.Text ?? string.Empty;
        }

        private static bool NombreContiene(AutomationElement elemento, string texto)
        {
            try
            {
                string nombre = elemento.Name;
                return !string.IsNullOrEmpty(nombre) && nombre.Contains(texto);
            }
            catch (FlaUI.Core.Exceptions.PropertyNotSupportedException)
            {
                return false;
            }
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
