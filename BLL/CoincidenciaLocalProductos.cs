using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BE;

namespace BLL
{
    // Coincidencia local, determinista y sin servicios externos, entre un item de texto libre de
    // una lista de compras y los productos del catalogo. Normaliza (minusculas, sin acentos ni
    // signos, plural simple), descarta palabras vacias y compara contra el nombre del producto
    // y contra nombre + categoria. El puntaje es el mayor entre:
    // - cobertura: promedio, por palabra del item, de la mejor similitud Jaro-Winkler contra las
    //   palabras del producto (una similitud menor a SimilitudMinimaPalabra cuenta como 0, asi
    //   una palabra ausente pesa de verdad);
    // - Jaccard de trigramas de los textos completos.
    // Acepta desde UmbralLocal y conserva hasta MaximoProductosPorItem productos.
    internal static class CoincidenciaLocalProductos
    {
        internal const double UmbralLocal = 0.85;
        internal const int MaximoProductosPorItem = 3;
        internal const double SimilitudMinimaPalabra = 0.80;

        private static readonly HashSet<string> PalabrasVacias = new HashSet<string>
        {
            "de", "del", "la", "las", "el", "los", "con", "sin", "para", "por", "y", "o", "a", "en", "un", "una", "x"
        };

        internal static List<CoincidenciaProducto> Buscar(string descripcionItem, IEnumerable<Producto> productos)
        {
            List<string> palabrasItem = Tokenizar(descripcionItem);
            if (palabrasItem.Count == 0 || productos == null)
            {
                return new List<CoincidenciaProducto>();
            }

            return productos
                .Where(producto => producto != null && !string.IsNullOrWhiteSpace(producto.Nombre))
                .Select(producto => new CoincidenciaProducto(producto, PuntajeContraProducto(palabrasItem, producto), false))
                .Where(coincidencia => coincidencia.Puntaje >= UmbralLocal)
                .OrderByDescending(coincidencia => coincidencia.Puntaje)
                .ThenBy(coincidencia => coincidencia.Producto.Id)
                .Take(MaximoProductosPorItem)
                .ToList();
        }

        internal static double Similitud(string descripcionItem, string textoProducto)
        {
            return Puntaje(Tokenizar(descripcionItem), textoProducto);
        }

        internal static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            string descompuesto = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder resultado = new StringBuilder(descompuesto.Length);
            bool anteriorEspacio = true;
            foreach (char caracter in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caracter) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(caracter))
                {
                    resultado.Append(caracter);
                    anteriorEspacio = false;
                }
                else if (!anteriorEspacio)
                {
                    resultado.Append(' ');
                    anteriorEspacio = true;
                }
            }

            return resultado.ToString().Trim().Normalize(NormalizationForm.FormC);
        }

        private static double PuntajeContraProducto(List<string> palabrasItem, Producto producto)
        {
            double contraNombre = Puntaje(palabrasItem, producto.Nombre);
            string categoria = producto.CategoriaProducto == null ? null : producto.CategoriaProducto.Nombre;
            if (string.IsNullOrWhiteSpace(categoria))
            {
                return contraNombre;
            }

            return Math.Max(contraNombre, Puntaje(palabrasItem, producto.Nombre + " " + categoria));
        }

        private static double Puntaje(List<string> palabrasItem, string textoProducto)
        {
            List<string> palabrasProducto = Tokenizar(textoProducto);
            if (palabrasItem.Count == 0 || palabrasProducto.Count == 0)
            {
                return 0;
            }

            double suma = 0;
            foreach (string palabra in palabrasItem)
            {
                double mejor = palabrasProducto.Max(candidata => JaroWinkler(palabra, candidata));
                suma += mejor >= SimilitudMinimaPalabra ? mejor : 0;
            }

            double cobertura = suma / palabrasItem.Count;
            double jaccard = JaccardTrigramas(string.Join(" ", palabrasItem), string.Join(" ", palabrasProducto));
            return Math.Max(cobertura, jaccard);
        }

        private static List<string> Tokenizar(string texto)
        {
            return Normalizar(texto)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(palabra => !PalabrasVacias.Contains(palabra))
                .Select(Singular)
                .ToList();
        }

        // Plural simple del espanol: "limones" -> "limon", "galletitas" -> "galletita".
        private static string Singular(string palabra)
        {
            if (palabra.Length > 4 && palabra.EndsWith("es", StringComparison.Ordinal) && !EsVocal(palabra[palabra.Length - 3]))
            {
                return palabra.Substring(0, palabra.Length - 2);
            }

            if (palabra.Length > 3 && palabra.EndsWith("s", StringComparison.Ordinal) && !char.IsDigit(palabra[palabra.Length - 2]))
            {
                return palabra.Substring(0, palabra.Length - 1);
            }

            return palabra;
        }

        private static bool EsVocal(char caracter)
        {
            return "aeiou".IndexOf(caracter) >= 0;
        }

        internal static double JaroWinkler(string primera, string segunda)
        {
            if (primera == segunda)
            {
                return 1;
            }

            if (primera.Length == 0 || segunda.Length == 0)
            {
                return 0;
            }

            int ventana = Math.Max(0, Math.Max(primera.Length, segunda.Length) / 2 - 1);
            bool[] coincidePrimera = new bool[primera.Length];
            bool[] coincideSegunda = new bool[segunda.Length];
            int coincidencias = 0;

            for (int i = 0; i < primera.Length; i++)
            {
                int desde = Math.Max(0, i - ventana);
                int hasta = Math.Min(segunda.Length - 1, i + ventana);
                for (int j = desde; j <= hasta; j++)
                {
                    if (!coincideSegunda[j] && primera[i] == segunda[j])
                    {
                        coincidePrimera[i] = true;
                        coincideSegunda[j] = true;
                        coincidencias++;
                        break;
                    }
                }
            }

            if (coincidencias == 0)
            {
                return 0;
            }

            int transposiciones = 0;
            int k = 0;
            for (int i = 0; i < primera.Length; i++)
            {
                if (!coincidePrimera[i])
                {
                    continue;
                }

                while (!coincideSegunda[k])
                {
                    k++;
                }

                if (primera[i] != segunda[k])
                {
                    transposiciones++;
                }

                k++;
            }

            double m = coincidencias;
            double jaro = (m / primera.Length + m / segunda.Length + (m - transposiciones / 2.0) / m) / 3.0;

            int prefijo = 0;
            int maximoPrefijo = Math.Min(4, Math.Min(primera.Length, segunda.Length));
            while (prefijo < maximoPrefijo && primera[prefijo] == segunda[prefijo])
            {
                prefijo++;
            }

            return jaro + prefijo * 0.1 * (1 - jaro);
        }

        private static double JaccardTrigramas(string primero, string segundo)
        {
            HashSet<string> trigramasPrimero = Trigramas(primero);
            HashSet<string> trigramasSegundo = Trigramas(segundo);
            if (trigramasPrimero.Count == 0 || trigramasSegundo.Count == 0)
            {
                return 0;
            }

            int interseccion = trigramasPrimero.Count(trigrama => trigramasSegundo.Contains(trigrama));
            int union = trigramasPrimero.Count + trigramasSegundo.Count - interseccion;
            return (double)interseccion / union;
        }

        private static HashSet<string> Trigramas(string texto)
        {
            HashSet<string> trigramas = new HashSet<string>();
            string relleno = "  " + texto + " ";
            for (int i = 0; i + 3 <= relleno.Length; i++)
            {
                trigramas.Add(relleno.Substring(i, 3));
            }

            return trigramas;
        }
    }
}
