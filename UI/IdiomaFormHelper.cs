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

            if (form.Tag is string formKey)
            {
                form.Text = idioma.ObtenerTexto(formKey);
            }

            Aplicar(form.Controls, idioma);
        }

        private static void Aplicar(Control.ControlCollection controls, Idioma idioma)
        {
            foreach (Control control in controls)
            {
                if (control.Tag is string key)
                {
                    control.Text = idioma.ObtenerTexto(key);
                }

                if (control.HasChildren)
                {
                    Aplicar(control.Controls, idioma);
                }
            }
        }
    }
}
