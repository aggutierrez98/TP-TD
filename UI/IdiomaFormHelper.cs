using System;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    internal static class IdiomaFormHelper
    {
        public static void Aplicar(Form form, Idioma idioma)
        {
            if (form == null || idioma == null)
            {
                return;
            }

            AplicarControl(form, idioma);
            Aplicar(form.Controls, idioma);
        }

        public static void AplicarDataGridView(DataGridView grilla, Idioma idioma)
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

        private static void Aplicar(Control.ControlCollection controls, Idioma idioma)
        {
            foreach (Control control in controls)
            {
                AplicarControl(control, idioma);

                if (control.HasChildren)
                {
                    Aplicar(control.Controls, idioma);
                }
            }
        }

        private static void AplicarControl(Control control, Idioma idioma)
        {
            if (control is DataGridView grilla)
            {
                AplicarDataGridView(grilla, idioma);
                return;
            }

            if (control.Tag is string key)
            {
                control.Text = idioma.ObtenerTraduccion(key);
            }
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

            string nombreColumna = !string.IsNullOrWhiteSpace(columna.DataPropertyName)
                ? columna.DataPropertyName.Trim()
                : columna.Name;

            return string.IsNullOrWhiteSpace(nombreColumna)
                ? null
                : string.Format("{0}.{1}", prefijo, nombreColumna);
        }
    }
}
