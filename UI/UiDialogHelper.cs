using System;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    internal static class UiDialogHelper
    {
        public static void MostrarInfo(string messageKey, params object[] args)
        {
            Mostrar(messageKey, "Dialog.Title.Success", MessageBoxButtons.OK, MessageBoxIcon.Information, args);
        }

        public static void MostrarInfoConTitulo(string messageKey, string titleKey, params object[] args)
        {
            Mostrar(messageKey, titleKey, MessageBoxButtons.OK, MessageBoxIcon.Information, args);
        }

        public static void MostrarWarning(string messageKey, params object[] args)
        {
            Mostrar(messageKey, "Dialog.Title.Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, args);
        }

        public static void MostrarWarningConTitulo(string messageKey, string titleKey, params object[] args)
        {
            Mostrar(messageKey, titleKey, MessageBoxButtons.OK, MessageBoxIcon.Warning, args);
        }

        public static void MostrarError(string messageKey, params object[] args)
        {
            Mostrar(messageKey, "Dialog.Title.Error", MessageBoxButtons.OK, MessageBoxIcon.Error, args);
        }

        public static void MostrarErrorConTitulo(string messageKey, string titleKey, params object[] args)
        {
            Mostrar(messageKey, titleKey, MessageBoxButtons.OK, MessageBoxIcon.Error, args);
        }

        public static DialogResult Confirmar(string messageKey, params object[] args)
        {
            return Confirmar(messageKey, "Dialog.Title.Confirmation", MessageBoxIcon.Question, args);
        }

        public static DialogResult Confirmar(string messageKey, string titleKey, MessageBoxIcon icon, params object[] args)
        {
            return MessageBox.Show(
                Traducir(messageKey, args),
                Traducir(titleKey),
                MessageBoxButtons.YesNo,
                icon);
        }

        public static string Traducir(string key, params object[] args)
        {
            string texto = ResolverTexto(key);

            if (args == null || args.Length == 0)
            {
                return texto;
            }

            return string.Format(texto, args);
        }

        public static string TraducirSiExiste(string keyOrText, params object[] args)
        {
            string texto = ResolverTexto(keyOrText);

            if (args == null || args.Length == 0)
            {
                return texto;
            }

            return string.Format(texto, args);
        }

        private static void Mostrar(string messageKey, string titleKey, MessageBoxButtons buttons, MessageBoxIcon icon, params object[] args)
        {
            MessageBox.Show(
                Traducir(messageKey, args),
                Traducir(titleKey),
                buttons,
                icon);
        }

        private static string ResolverTexto(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            Idioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma != null && idioma.TieneTraduccion(key))
            {
                string traduccion = idioma.ObtenerTraduccion(key);
                if (!string.IsNullOrWhiteSpace(traduccion))
                {
                    return traduccion;
                }
            }

            return key;
        }
    }
}
