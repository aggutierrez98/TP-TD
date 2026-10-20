using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL
{
    internal enum ResultadoPrefiltro
    {
        Admitida,
        DemasiadoLarga,
        Inyeccion,
        TemaExcluido
    }

    // Primera capa del asistente, local y determinista: rechaza sin llamar a ningun servicio
    // externo las preguntas demasiado largas y las que contienen patrones obvios de inyeccion
    // o de temas excluidos. Compara sobre el texto en minusculas, sin acentos y con espacios
    // colapsados; cada patron debe empezar en un limite de palabra. Las listas son revisables:
    // se evita "codigo" suelto para no rechazar "codigo de descuento" y "programar" suelto
    // para no rechazar "programar una transferencia".
    internal static class PrefiltroConsultasAsistente
    {
        internal const int LargoMaximo = 500;

        internal static readonly string[] PatronesInyeccion =
        {
            "ignora las instrucciones",
            "ignora tus instrucciones",
            "ignora todas las instrucciones",
            "ignore previous",
            "ignore all previous",
            "ignore the previous",
            "system prompt",
            "prompt del sistema",
            "actua como",
            "olvida lo anterior",
            "olvida las instrucciones",
            "olvida tus instrucciones"
        };

        internal static readonly string[] PatronesTemaExcluido =
        {
            "receta",
            "programar en",
            "programa en",
            "codigo fuente",
            "script",
            "sql"
        };

        private static readonly Regex EspaciosMultiples = new Regex(@"\s+", RegexOptions.Compiled);

        internal static ResultadoPrefiltro Evaluar(string pregunta)
        {
            string texto = pregunta ?? string.Empty;
            if (texto.Length > LargoMaximo)
            {
                return ResultadoPrefiltro.DemasiadoLarga;
            }

            string normalizado = Normalizar(texto);
            if (ContieneAlguno(normalizado, PatronesInyeccion))
            {
                return ResultadoPrefiltro.Inyeccion;
            }

            if (ContieneAlguno(normalizado, PatronesTemaExcluido))
            {
                return ResultadoPrefiltro.TemaExcluido;
            }

            return ResultadoPrefiltro.Admitida;
        }

        // Minusculas, sin marcas diacriticas y con espacios colapsados. Compartido con el
        // validador de respuestas para comparar nombres con el mismo criterio.
        internal static string Normalizar(string texto)
        {
            string descompuesto = (texto ?? string.Empty).ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder resultado = new StringBuilder(descompuesto.Length);
            foreach (char caracter in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(caracter);
                }
            }

            return EspaciosMultiples.Replace(resultado.ToString().Normalize(NormalizationForm.FormC), " ").Trim();
        }

        private static bool ContieneAlguno(string normalizado, string[] patrones)
        {
            foreach (string patron in patrones)
            {
                if (Regex.IsMatch(normalizado, @"\b" + Regex.Escape(patron)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
