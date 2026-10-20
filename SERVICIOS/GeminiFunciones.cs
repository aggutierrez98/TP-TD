using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SERVICIOS
{
    // Declaracion de una funcion que Gemini puede invocar (tools[].functionDeclarations).
    // Parametros es el esquema OpenAPI de los argumentos; null = funcion sin argumentos.
    // Aislamiento de datos: ninguna funcion puede recibir un identificador de usuario. El
    // usuario sale siempre de la sesion en el BLL, nunca de un argumento elegido por el modelo.
    internal sealed class DeclaracionFuncionGemini
    {
        private static readonly string[] FragmentosIdentificadorUsuario = { "usuario", "userid", "user_id" };

        internal DeclaracionFuncionGemini(string nombre, string descripcion, object parametros)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("La funcion necesita un nombre.", nameof(nombre));
            }

            ValidarSinIdentificadorDeUsuario(parametros);
            Nombre = nombre;
            Descripcion = descripcion;
            Parametros = parametros;
        }

        internal string Nombre { get; private set; }

        internal string Descripcion { get; private set; }

        internal object Parametros { get; private set; }

        private static void ValidarSinIdentificadorDeUsuario(object parametros)
        {
            if (parametros == null)
            {
                return;
            }

            using (JsonDocument esquema = JsonDocument.Parse(JsonSerializer.Serialize(parametros)))
            {
                RevisarNodo(esquema.RootElement);
            }
        }

        private static void RevisarNodo(JsonElement nodo)
        {
            if (nodo.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement elemento in nodo.EnumerateArray())
                {
                    RevisarNodo(elemento);
                }

                return;
            }

            if (nodo.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            foreach (JsonProperty propiedad in nodo.EnumerateObject())
            {
                if (propiedad.Name == "properties" && propiedad.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty parametro in propiedad.Value.EnumerateObject())
                    {
                        string nombre = parametro.Name.ToLowerInvariant();
                        foreach (string fragmento in FragmentosIdentificadorUsuario)
                        {
                            if (nombre.Contains(fragmento))
                            {
                                throw new ArgumentException("Una funcion del asistente no puede recibir un identificador de usuario: " + parametro.Name);
                            }
                        }
                    }
                }

                RevisarNodo(propiedad.Value);
            }
        }
    }

    // Opciones de generacion de un pedido a Gemini: temperatura y, si hay funciones,
    // tools + toolConfig con mode ANY (el modelo solo puede responder con una llamada a
    // funcion) restringido a FuncionesPermitidas (todas las declaradas si queda vacio).
    internal sealed class OpcionesGeneracionGemini
    {
        internal double? Temperatura { get; set; }

        internal IReadOnlyList<DeclaracionFuncionGemini> Funciones { get; set; }

        internal IReadOnlyList<string> FuncionesPermitidas { get; set; }
    }

    // Parte functionCall de la respuesta: id opcional, nombre y args como JSON crudo ("{}"
    // si el modelo no envio argumentos). Quien la consume valida los argumentos.
    internal sealed class LlamadaFuncionGemini
    {
        internal LlamadaFuncionGemini(string id, string nombre, string argumentosJson)
        {
            Id = id;
            Nombre = nombre;
            ArgumentosJson = argumentosJson;
        }

        internal string Id { get; private set; }

        internal string Nombre { get; private set; }

        internal string ArgumentosJson { get; private set; }
    }
}
