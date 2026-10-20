using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ABSTRACCIONES;

namespace BLL
{
    internal enum ResultadoValidacionRespuesta
    {
        Valida,
        RespuestaVacia,
        BloqueDeCodigo,
        ReferenciaNoPermitida
    }

    // Ultima capa del asistente, en codigo: descarta respuestas con bloques de codigo
    // (triple backtick o bloque indentado al estilo Markdown) y respuestas que nombran una
    // entidad del universo conocido (comercios, promociones, cuentas, instrumentos) que no
    // estaba entre las referencias permitidas del contexto entregado al modelo. Con contexto
    // vacio, cualquier mencion del universo invalida la respuesta. La comparacion es por
    // tipo, sin mayusculas ni acentos y por palabra completa. No detecta parafrasis: complementa
    // al contexto cerrado, no lo reemplaza.
    internal sealed class ValidadorRespuestaAsistente
    {
        private static readonly Regex InicioDeLista = new Regex(@"^([-*+]|\d+[.)])\s", RegexOptions.Compiled);

        internal ResultadoValidacionRespuesta Validar(string respuesta, IEnumerable<ReferenciaAsistente> universo, ContextoAsistente contexto)
        {
            if (string.IsNullOrWhiteSpace(respuesta))
            {
                return ResultadoValidacionRespuesta.RespuestaVacia;
            }

            if (ContieneBloqueDeCodigo(respuesta))
            {
                return ResultadoValidacionRespuesta.BloqueDeCodigo;
            }

            if (universo != null && NombraReferenciaNoPermitida(respuesta, universo, contexto ?? ContextoAsistente.Vacio))
            {
                return ResultadoValidacionRespuesta.ReferenciaNoPermitida;
            }

            return ResultadoValidacionRespuesta.Valida;
        }

        private static bool ContieneBloqueDeCodigo(string respuesta)
        {
            if (respuesta.Contains("```") || respuesta.Contains("~~~"))
            {
                return true;
            }

            string[] lineas = respuesta.Replace("\r\n", "\n").Split('\n');
            bool anteriorEnBlanco = true;
            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                {
                    anteriorEnBlanco = true;
                    continue;
                }

                bool indentada = linea.StartsWith("    ", StringComparison.Ordinal) || linea.StartsWith("\t", StringComparison.Ordinal);
                if (anteriorEnBlanco && indentada && !InicioDeLista.IsMatch(linea.Trim()))
                {
                    return true;
                }

                anteriorEnBlanco = false;
            }

            return false;
        }

        private static bool NombraReferenciaNoPermitida(string respuesta, IEnumerable<ReferenciaAsistente> universo, ContextoAsistente contexto)
        {
            string texto = PrefiltroConsultasAsistente.Normalizar(respuesta);
            HashSet<string> permitidas = new HashSet<string>(
                contexto.ReferenciasPermitidas
                    .Where(referencia => referencia != null && !string.IsNullOrWhiteSpace(referencia.Nombre))
                    .Select(referencia => Clave(referencia.Tipo, referencia.Nombre)));

            foreach (ReferenciaAsistente referencia in universo)
            {
                if (referencia == null || string.IsNullOrWhiteSpace(referencia.Nombre))
                {
                    continue;
                }

                string nombre = PrefiltroConsultasAsistente.Normalizar(referencia.Nombre);
                bool mencionada = referencia.EsTicker
                    ? NombraTicker(respuesta, referencia.Nombre)
                    : Regex.IsMatch(texto, @"(?<![\p{L}\p{N}])" + Regex.Escape(nombre) + @"(?![\p{L}\p{N}])");
                if (mencionada && !permitidas.Contains(Clave(referencia.Tipo, referencia.Nombre)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool NombraTicker(string respuesta, string ticker)
        {
            string simbolo = Regex.Escape(ticker);
            // DE/O/SE/COME tambien son prosa; A/B identifican clases de fondos.
            // $ticker y etiquetas explicitas son inequivocos aun en minusculas.
            if (Regex.IsMatch(respuesta, @"(?:\$|\b(?:ticker|accion|acciones|cedear)\s+)" + simbolo + @"(?![\p{L}\p{N}])", RegexOptions.IgnoreCase)) return true;
            return ticker.Length > 1 && Regex.IsMatch(respuesta, @"(?<![\p{L}\p{N}])" + simbolo + @"(?![\p{L}\p{N}])");
        }

        private static string Clave(TipoReferenciaAsistente tipo, string nombre)
        {
            return tipo + "|" + PrefiltroConsultasAsistente.Normalizar(nombre);
        }
    }
}
