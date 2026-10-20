using System.Text;
using System.Text.RegularExpressions;
using BE;

namespace TpIDS
{
    // Presentacion de notificaciones con contenido completo (las automaticas superan los 300
    // caracteres y tienen saltos de linea): la grilla muestra un resumen de una linea, el detalle
    // el texto entero. No cambia el dato guardado.
    internal static class VistaPreviaNotificacion
    {
        internal const int MaximoCaracteres = 120;
        private const string PuntosSuspensivos = "…";

        // Una sola linea: las corridas de espacios y saltos se reducen a un espacio y, si supera el
        // maximo, se corta (sin partir un par subrogado) y termina en puntos suspensivos.
        internal static string Resumir(string mensaje, int maximo = MaximoCaracteres)
        {
            string linea = Regex.Replace(mensaje ?? string.Empty, @"\s+", " ").Trim();
            if (linea.Length <= maximo)
            {
                return linea;
            }

            int corte = maximo - PuntosSuspensivos.Length;
            if (corte > 0 && char.IsHighSurrogate(linea[corte - 1]))
            {
                corte--;
            }

            return linea.Substring(0, corte).TrimEnd() + PuntosSuspensivos;
        }

        // El cuadro de texto multilinea de WinForms solo reconoce CRLF.
        internal static string ParaCuadroDeTexto(string mensaje)
        {
            return Regex.Replace(mensaje ?? string.Empty, @"\r\n|\r|\n", "\r\n");
        }

        // Etiqueta traducible del tipo. Una compra agregada sigue siendo recordatorio de compra
        // aunque no tenga item ni vencimiento; los demas avisos conservan su etiqueta.
        internal static string ClaveTipo(Notificacion notificacion)
        {
            if (notificacion is RecordatorioCompra)
            {
                return "FormMyNotifications.Value.PurchaseReminder";
            }

            if (notificacion is RecordatorioInversion)
            {
                return "FormMyNotifications.Value.InvestmentReminder";
            }

            return "FormMyNotifications.Value.ManualNotification";
        }
    }
}
