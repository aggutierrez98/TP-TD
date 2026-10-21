
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
    // Usa reflexion para descubrir las propiedades marcadas con un atributo IDigitoVerificador,
    // por lo que es aplicable a cualquier entidad sin modificar el motor.
    public class IntegridadService : IIntegridadService
    {
        // Separador entre atributos dentro de la cadena canonica.
        private const string SeparadorAtributo = "//";
        private const int LongitudHashBytes = 32;

        /// <summary>
        /// Metodo para calcular el DVH de una entidad en base a sus propiedades verificables.
        /// Usa las propiedades marcadas con IDigitoVerificador y una representacion canonica estable.
        /// </summary>
        /// <param name="entidad">La entidad sobre la que se calcula el digito verificador horizontal.</param>
        /// <returns>El DVH calculado.</returns>
        /// <exception cref="ArgumentNullException">La entidad es <see langword="null" />.</exception>
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

        /// <summary>
        /// Metodo para calcular el DVV en base a los DVH de una entidad.
        /// Usado solo para el primer calculo del DVV.
        /// </summary>
        /// <param name="listaDVH">La coleccion de DVH que componen el DVV de la tabla.</param>
        /// <returns>El DVV calculado.</returns>
        /// <exception cref="ArgumentNullException">La lista de DVH es <see langword="null" />.</exception>
        public string CalcularDVV(IEnumerable<string> listaDVH)
        {
            if (listaDVH == null)
            {
                throw new ArgumentNullException(nameof(listaDVH));
            }

            byte[] acumulador = CrearAcumuladorVacio();
            foreach (string dvh in listaDVH)
            {
                Sumar(acumulador, ObtenerContribucionDVH(dvh));
            }

            return Convert.ToBase64String(acumulador);
        }

        /// <summary>
        /// Metodo para calcular el DVV en base a el ultimo DVV de una entidad y un DVH correspondiente a cambios sobre una entidad.
        /// Usado cuando un DVV ya fue calculado y se quiere actualizar con un nuevo DVH.
        /// </summary>
        /// <param name="dvvActual">El DVV persistido antes del cambio.</param>
        /// <param name="dvhAnterior">El DVH que debe descontarse del DVV, si corresponde.</param>
        /// <param name="dvhNuevo">El DVH que debe incorporarse al DVV, si corresponde.</param>
        /// <returns>El DVV calculado.</returns>
        /// <exception cref="ArgumentException">El DVV o alguno de los DVH no representa un hash SHA-256 en Base64.</exception>
        public string CalcularDVVIncremental(string dvvActual, string dvhAnterior, string dvhNuevo)
        {
            byte[] acumulador = ObtenerAcumulador(dvvActual);

            Restar(acumulador, ObtenerContribucionDVH(dvhAnterior));
            Sumar(acumulador, ObtenerContribucionDVH(dvhNuevo));

            return Convert.ToBase64String(acumulador);
        }

        private static List<PropiedadVerificable> ObtenerPropiedadesVerificables(Type tipo)
        {
            return tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(info => new
                {
                    Info = info,
                    Atributo = Attribute.GetCustomAttributes(info, true).OfType<IDigitoVerificador>().FirstOrDefault()
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

        private static byte[] CrearAcumuladorVacio()
        {
            return new byte[LongitudHashBytes];
        }

        private static byte[] ObtenerAcumulador(string dvv)
        {
            if (string.IsNullOrWhiteSpace(dvv))
            {
                // Si la tabla de DVV todavia no tiene fila, el neutro de la
                // suma modular permite iniciar el acumulador sin caso especial.
                return CrearAcumuladorVacio();
            }

            return DecodificarHashBase64(dvv, nameof(dvv));
        }

        private static byte[] ObtenerContribucionDVH(string dvh)
        {
            if (string.IsNullOrWhiteSpace(dvh))
            {
                // Un DVH inexistente no aporta nada al DVV. Esto modela altas y
                // bajas sin bifurcar la formula incremental.
                return CrearAcumuladorVacio();
            }

            return DecodificarHashBase64(dvh, nameof(dvh));
        }

        private static byte[] DecodificarHashBase64(string valor, string nombreParametro)
        {
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(valor);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("El valor debe ser un hash SHA-256 en Base64.", nombreParametro, ex);
            }

            if (bytes.Length != LongitudHashBytes)
            {
                throw new ArgumentException("El valor debe representar exactamente 32 bytes.", nombreParametro);
            }

            return bytes;
        }

        private static void Sumar(byte[] acumulador, byte[] contribucion)
        {
            // Suma big-endian modulo 2^256. El acarreo final se descarta a
            // proposito: eso cierra el espacio de 32 bytes y mantiene el valor
            // serializable como Base64 de SHA-256.
            int acarreo = 0;
            for (int indice = acumulador.Length - 1; indice >= 0; indice--)
            {
                int suma = acumulador[indice] + contribucion[indice] + acarreo;
                acumulador[indice] = (byte)(suma & 0xFF);
                acarreo = suma >> 8;
            }
        }

        private static void Restar(byte[] acumulador, byte[] contribucion)
        {
            // Resta big-endian modulo 2^256. Si hay prestamo al salir del byte
            // mas significativo, queda implicito por el comportamiento modular.
            int prestamo = 0;
            for (int indice = acumulador.Length - 1; indice >= 0; indice--)
            {
                int resta = acumulador[indice] - contribucion[indice] - prestamo;
                if (resta < 0)
                {
                    resta += 256;
                    prestamo = 1;
                }
                else
                {
                    prestamo = 0;
                }

                acumulador[indice] = (byte)resta;
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
