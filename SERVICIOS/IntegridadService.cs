using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Motor generico de calculo de digitos verificadores.
    // Usa reflexion para descubrir las propiedades marcadas con DigitoVerificadorAttribute,
    // por lo que es aplicable a cualquier entidad sin modificar el motor.
    public class IntegridadService : IIntegridadService
    {
        // Separador entre atributos dentro de la cadena canonica.
        private const string SeparadorAtributo = "//";

        public string CalcularDVH<T>(T entidad) where T : class
        {
            if (entidad == null)
            {
                throw new ArgumentNullException(nameof(entidad));
            }

            // Propiedades marcadas, ordenadas por el Orden declarado en el atributo.
            List<PropiedadVerificable> propiedades = ObtenerPropiedadesVerificables(entidad.GetType());

            StringBuilder canonica = new StringBuilder();

            foreach (PropiedadVerificable propiedad in propiedades)
            {
                int orden = propiedad.Orden;
                object valor = propiedad.Info.GetValue(entidad);
                string texto = ConvertirAStringCanonico(valor);

                // La posicion del atributo participa estructuralmente en la firma.
                canonica.Append(orden).Append('#');

                for (int indice = 0; indice < texto.Length; indice++)
                {
                    int posicionCaracter = indice + 1; // posiciones 1-based
                    int valorCaracter = texto[indice];

                    // Contenido del caracter * posicion del caracter * posicion del atributo.
                    long termino = (long)valorCaracter * posicionCaracter * orden;

                    canonica.Append(posicionCaracter)
                        .Append(':')
                        .Append(termino.ToString(CultureInfo.InvariantCulture))
                        .Append(';');
                }

                canonica.Append(SeparadorAtributo);
            }

            return CalcularHash(canonica.ToString());
        }

        public string CalcularDVV(IEnumerable<string> listaDVH)
        {
            if (listaDVH == null)
            {
                throw new ArgumentNullException(nameof(listaDVH));
            }

            // Combinacion de todos los DVH en el orden recibido.
            string concatenado = string.Join("|", listaDVH.Select(dvh => dvh ?? string.Empty));
            return CalcularHash(concatenado);
        }

        private static List<PropiedadVerificable> ObtenerPropiedadesVerificables(Type tipo)
        {
            return tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(info => new
                {
                    Info = info,
                    Atributo = info.GetCustomAttribute<DigitoVerificadorAttribute>()
                })
                .Where(item => item.Atributo != null)
                .OrderBy(item => item.Atributo.Orden)
                .Select(item => new PropiedadVerificable(item.Info, item.Atributo.Orden))
                .ToList();
        }

        private static string ConvertirAStringCanonico(object valor)
        {
            if (valor == null)
            {
                return string.Empty;
            }

            // InvariantCulture asegura una representacion estable e independiente del entorno.
            return Convert.ToString(valor, CultureInfo.InvariantCulture);
        }

        private static string CalcularHash(string contenido)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contenido);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Transporta una propiedad verificable junto con su orden ya resuelto.
        private sealed class PropiedadVerificable
        {
            public PropiedadVerificable(PropertyInfo info, int orden)
            {
                Info = info;
                Orden = orden;
            }

            public PropertyInfo Info { get; }

            public int Orden { get; }
        }
    }
}
