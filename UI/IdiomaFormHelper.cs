using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    internal static class IdiomaFormHelper
    {

        // Aplicar traducciones sobre todos los forms y controles que tengan un Tag con la clave de traducción
        public static void Aplicar(Form form, Idioma idioma)
        {
            if (form == null || idioma == null)
            {
                return;
            }

            if (form.Tag is string formKey)
            {
                form.Text = idioma.ObtenerTraduccion(formKey);
            }

            Aplicar(form.Controls, idioma);
        }

        private static void Aplicar(Control.ControlCollection controls, Idioma idioma)
        {
            foreach (Control control in controls)
            {
                if (control.Tag is string key)
                {
                    control.Text = idioma.ObtenerTraduccion(key);
                }

                if (control.HasChildren)
                {
                    Aplicar(control.Controls, idioma);
                }
            }
        }

        // TODO: Aplicar traducciones sobre todos los mensajes de de error.
    }
}
