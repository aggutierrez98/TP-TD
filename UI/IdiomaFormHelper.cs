using System;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using Krypton.Toolkit;

namespace TpIDS
{
    internal static class IdiomaFormHelper
    {

        // Aplica traducciones sobre formulario y sus controles
        public static void Aplicar(Form form, IIdioma idioma)
        {
            if (form == null || idioma == null)
            {
                return;
            }


            foreach (Control item in form.Controls)
            {
                    
                Aplicar(item, idioma);
            }
        }

        // Aplica traducciones sobre un control y sus hijos
        public static void Aplicar(Control control, IIdioma idioma)
        {
            if (control == null || idioma == null)
            {
                return;
            }

            if (control is DataGridView grilla)
            {
                // Encabezados: se traducen aca. Celdas: se traducen solas al repintar (CellFormatting).
                AplicarDataGridView(grilla, idioma);
                grilla.Invalidate();
                return;
            }

            if (control is ComboBox combo && TieneOpcionesTraducibles(combo))
            {
                AplicarComboBox(combo, idioma);
                return;
            }

            // KryptonComboBox no hereda de ComboBox (lo envuelve): se trata aparte.
            if (control is KryptonComboBox comboKrypton && TieneOpcionesTraducibles(comboKrypton))
            {
                AplicarComboBox(comboKrypton, idioma);
                return;
            }

            // Los items de un MenuStrip/ToolStrip no estan en Controls: se recorren aparte.
            if (control is ToolStrip barra)
            {
                AplicarItems(barra.Items, idioma);
            }

            if (control.Tag is string key)
            {
                control.Text = idioma.ObtenerTraduccion(key);
            }

            if (control.HasChildren)
            {

                foreach (Control item in control.Controls)
                {
                    Aplicar(item, idioma);
                }
            }
        }

        private static void AplicarItems(ToolStripItemCollection items, IIdioma idioma)
        {
            foreach (ToolStripItem item in items)
            {
                if (item.Tag is string key)
                {
                    item.Text = idioma.ObtenerTraduccion(key);
                }

                if (item is ToolStripDropDownItem desplegable && desplegable.HasDropDownItems)
                {
                    AplicarItems(desplegable.DropDownItems, idioma);
                }
            }
        }

        public static void AplicarDataGridView(DataGridView grilla, IIdioma idioma)
        {
            if (grilla == null || idioma == null || grilla.Columns.Count == 0)
            {
                return;
            }

            string prefijo = ObtenerPrefijoDataGrid(grilla);
            if (string.IsNullOrWhiteSpace(prefijo))
            {
                return;
            }

            foreach (DataGridViewColumn columna in grilla.Columns)
            {
                string key = ObtenerKeyColumna(prefijo, columna);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    string traduccion = idioma.ObtenerTraduccion(key);
                    if (!string.Equals(traduccion, key, StringComparison.Ordinal))
                    {
                        columna.HeaderText = traduccion;
                    }
                }
            }
        }

        private static bool TieneOpcionesTraducibles(ComboBox combo)
        {
            return combo.Items.Count > 0 && combo.Items[0] is OpcionTraducible;
        }

        private static bool TieneOpcionesTraducibles(KryptonComboBox combo)
        {
            return combo.Items.Count > 0 && combo.Items[0] is OpcionTraducible;
        }

        private static void AplicarComboBox(ComboBox combo, IIdioma idioma)
        {
            int seleccion = combo.SelectedIndex;
            object[] items = combo.Items.Cast<object>().ToArray();

            foreach (OpcionTraducible opcion in items.OfType<OpcionTraducible>())
            {
                opcion.Texto = idioma.ObtenerTraduccion(opcion.Clave);
            }

            // Re-agregar los items fuerza a que el combo recalcule el texto mostrado.
            combo.BeginUpdate();
            combo.Items.Clear();
            combo.Items.AddRange(items);
            if (seleccion >= 0 && seleccion < combo.Items.Count)
            {
                combo.SelectedIndex = seleccion;
            }

            combo.EndUpdate();
        }

        private static void AplicarComboBox(KryptonComboBox combo, IIdioma idioma)
        {
            int seleccion = combo.SelectedIndex;
            object[] items = combo.Items.Cast<object>().ToArray();

            foreach (OpcionTraducible opcion in items.OfType<OpcionTraducible>())
            {
                opcion.Texto = idioma.ObtenerTraduccion(opcion.Clave);
            }

            // Re-agregar los items fuerza a que el combo recalcule el texto mostrado.
            combo.BeginUpdate();
            combo.Items.Clear();
            combo.Items.AddRange(items);
            if (seleccion >= 0 && seleccion < combo.Items.Count)
            {
                combo.SelectedIndex = seleccion;
            }

            combo.EndUpdate();
        }

        private static string ObtenerPrefijoDataGrid(DataGridView grilla)
        {
            if (grilla.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
            {
                return tag.Trim();
            }

            Form form = grilla.FindForm();
            if (!(form?.Tag is string formKey) || string.IsNullOrWhiteSpace(formKey))
            {
                return null;
            }

            return formKey.EndsWith(".Title")
                ? formKey.Substring(0, formKey.Length - ".Title".Length) + ".Grid"
                : formKey + ".Grid";
        }

        private static string ObtenerKeyColumna(string prefijo, DataGridViewColumn columna)
        {
            if (columna == null)
            {
                return null;
            }

            if (columna.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
            {
                return tag.Trim();
            }

            // La key sale del Name de la columna (sufijo) + el prefijo de la grilla. El Name se
            // edita y conserva siempre desde el diseñador; DataPropertyName solo es respaldo.
            string nombreColumna = !string.IsNullOrWhiteSpace(columna.Name)
                ? columna.Name.Trim()
                : columna.DataPropertyName?.Trim();

            return string.IsNullOrWhiteSpace(nombreColumna)
                ? null
                : string.Format("{0}.{1}", prefijo, nombreColumna);
        }
    }
}
