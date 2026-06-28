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
        private const int LongitudHashBytes = 32;

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

            // El DVV se define como suma modular de contribuciones de DVH.
            // Antes era un hash de la concatenacion completa, lo que obligaba a
            // leer toda la tabla en cada escritura. Con esta forma, la
            // recomputacion completa y la actualizacion incremental producen el
            // mismo valor.
            byte[] acumulador = CrearAcumuladorVacio();
            foreach (string dvh in listaDVH)
            {
                Sumar(acumulador, ObtenerContribucionDVH(dvh));
            }

            return Convert.ToBase64String(acumulador);
        }

        public string CalcularDVVIncremental(string dvvActual, string dvhAnterior, string dvhNuevo)
        {
            byte[] acumulador = ObtenerAcumulador(dvvActual);

            // Paso incremental:
            // 1. Partir del DVV persistido.
            // 2. Sacar la contribucion del DVH viejo, si existia.
            // 3. Agregar la contribucion del DVH nuevo, si existe.
            // Alta: anterior null, nuevo con valor.
            // Baja: anterior con valor, nuevo null.
            // Modificacion: ambos con valor.
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
