using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using System.Text.Json;

namespace SERVICIOS
{
    // Cliente HTTP compartido para hablar con la API de chat de Gemini (contents con
    // roles user/model + systemInstruction). Centraliza el armado del request, el POST
    // y el parseo de texto de respuesta para no triplicar esa logica entre
    // GeminiAsistenteFinancieroService (AgenteGeneral) y las subclases de AgenteIA
    // (AgenteAhorro, AgenteInversiones): todas comparten el mismo mecanismo de llamada,
    // solo cambia el promptSistema de cada una.
    internal static class GeminiChatClient
    {
        internal const string EndpointBase = "https://generativelanguage.googleapis.com/v1beta/models/";
        internal const string ModeloPorDefecto = "gemini-3.1-flash-lite";

        internal static async Task<string> ObtenerRespuestaAsync(string promptSistema, string pregunta, IEnumerable<IntercambioChatbot> historial, string apiKey, string modelo)
        {
            try
            {
                string requestBody = ArmarRequestBody(promptSistema, pregunta, historial);
                string cuerpoRespuesta = await PostearYLeerAsync(requestBody, apiKey, modelo).ConfigureAwait(false);
                return ExtraerTexto(cuerpoRespuesta);
            }
            catch (ExcepcionNegocio)
            {
                throw;
            }
            catch (ExcepcionTecnica)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
            }
        }

        // POST crudo al endpoint de generateContent. Devuelve el cuerpo de la respuesta tal
        // cual (todavia sin extraer el texto), para que llamadas con otro responseFormat
        // (ej. la clasificacion de agente en GeminiAsistenteFinancieroService) puedan
        // reutilizar el mismo mecanismo de POST sin pasar por ExtraerTexto.
        internal static Task<string> PostearYLeerAsync(string requestBody, string apiKey, string modelo)
        {
            return PostearYLeerAsync(requestBody, apiKey, modelo, null, CancellationToken.None);
        }

        // El manejador opcional permite a los tests reemplazar la red por un HttpMessageHandler
        // falso; null usa el manejador por defecto de HttpClient.
        internal static async Task<string> PostearYLeerAsync(string requestBody, string apiKey, string modelo, HttpMessageHandler manejador, CancellationToken cancellationToken)
        {
            // La key va como header (no en la URL) para que no quede expuesta en logs de
            // HttpClient, proxies o telemetria que capturen la URL completa del request.
            string endpoint = EndpointBase + modelo + ":generateContent";

            using (HttpClient cliente = manejador == null ? new HttpClient() : new HttpClient(manejador, false))
            using (StringContent contenido = new StringContent(requestBody, Encoding.UTF8, "application/json"))
            {
                cliente.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

                HttpResponseMessage respuesta = await cliente
                    .PostAsync(endpoint, contenido, cancellationToken)
                    .ConfigureAwait(false);

                string cuerpoRespuesta = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!respuesta.IsSuccessStatusCode)
                {
                    throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
                }

                return cuerpoRespuesta;
            }
        }

        // Historial como contents con roles user/model (coherencia multi-turno), seguido
        // de la pregunta actual.
        internal static string ArmarRequestBody(string promptSistema, string pregunta, IEnumerable<IntercambioChatbot> historial)
        {
            return ArmarRequestBody(promptSistema, pregunta, historial, null);
        }

        // Con opciones agrega generationConfig.temperature y, si hay funciones declaradas,
        // tools[].functionDeclarations y toolConfig.functionCallingConfig (mode ANY +
        // allowedFunctionNames). Nombres de campos segun la referencia REST de generateContent.
        internal static string ArmarRequestBody(string promptSistema, string pregunta, IEnumerable<IntercambioChatbot> historial, OpcionesGeneracionGemini opciones)
        {
            List<object> contents = new List<object>();

            if (historial != null)
            {
                foreach (IntercambioChatbot intercambio in historial)
                {
                    if (!string.IsNullOrWhiteSpace(intercambio.Pregunta))
                    {
                        contents.Add(new { role = "user", parts = new object[] { new { text = intercambio.Pregunta } } });
                    }

                    if (!string.IsNullOrWhiteSpace(intercambio.Respuesta))
                    {
                        contents.Add(new { role = "model", parts = new object[] { new { text = intercambio.Respuesta } } });
                    }
                }
            }

            contents.Add(new { role = "user", parts = new object[] { new { text = pregunta } } });

            Dictionary<string, object> request = new Dictionary<string, object>
            {
                { "systemInstruction", new { parts = new object[] { new { text = promptSistema } } } },
                { "contents", contents.ToArray() }
            };

            if (opciones != null)
            {
                if (opciones.Temperatura.HasValue)
                {
                    request.Add("generationConfig", new { temperature = opciones.Temperatura.Value });
                }

                if (opciones.Funciones != null && opciones.Funciones.Count > 0)
                {
                    AgregarFunciones(request, opciones);
                }
            }

            return JsonSerializer.Serialize(request);
        }

        private static void AgregarFunciones(Dictionary<string, object> request, OpcionesGeneracionGemini opciones)
        {
            List<string> declaradas = opciones.Funciones.Select(funcion => funcion.Nombre).ToList();
            List<string> permitidas = opciones.FuncionesPermitidas == null || opciones.FuncionesPermitidas.Count == 0
                ? declaradas
                : opciones.FuncionesPermitidas.ToList();

            foreach (string nombre in permitidas)
            {
                if (!declaradas.Contains(nombre))
                {
                    throw new ArgumentException("La funcion permitida no esta declarada: " + nombre);
                }
            }

            List<object> declaraciones = new List<object>();
            foreach (DeclaracionFuncionGemini funcion in opciones.Funciones)
            {
                Dictionary<string, object> declaracion = new Dictionary<string, object>
                {
                    { "name", funcion.Nombre },
                    { "description", funcion.Descripcion ?? string.Empty }
                };

                if (funcion.Parametros != null)
                {
                    declaracion.Add("parameters", funcion.Parametros);
                }

                declaraciones.Add(declaracion);
            }

            request.Add("tools", new object[] { new { functionDeclarations = declaraciones.ToArray() } });
            request.Add("toolConfig", new
            {
                functionCallingConfig = new
                {
                    mode = "ANY",
                    allowedFunctionNames = permitidas.ToArray()
                }
            });
        }

        // Pedido con funciones: devuelve las partes functionCall de la respuesta, sin texto.
        internal static async Task<IReadOnlyList<LlamadaFuncionGemini>> ObtenerLlamadasFuncionAsync(
            string promptSistema,
            string pregunta,
            IEnumerable<IntercambioChatbot> historial,
            OpcionesGeneracionGemini opciones,
            string apiKey,
            string modelo,
            HttpMessageHandler manejador,
            CancellationToken cancellationToken)
        {
            try
            {
                string requestBody = ArmarRequestBody(promptSistema, pregunta, historial, opciones);
                string cuerpoRespuesta = await PostearYLeerAsync(requestBody, apiKey, modelo, manejador, cancellationToken).ConfigureAwait(false);
                return ExtraerLlamadasFuncion(cuerpoRespuesta);
            }
            catch (ExcepcionNegocio)
            {
                throw;
            }
            catch (ExcepcionTecnica)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
            }
        }

        // Recorre todas las partes del primer candidato y devuelve las que son functionCall.
        internal static IReadOnlyList<LlamadaFuncionGemini> ExtraerLlamadasFuncion(string jsonRespuesta)
        {
            List<LlamadaFuncionGemini> llamadas = new List<LlamadaFuncionGemini>();

            using (JsonDocument respuesta = JsonDocument.Parse(jsonRespuesta))
            {
                JsonElement parts;
                if (!IntentarObtenerPartes(respuesta.RootElement, out parts))
                {
                    return llamadas;
                }

                foreach (JsonElement parte in parts.EnumerateArray())
                {
                    if (parte.ValueKind != JsonValueKind.Object ||
                        !parte.TryGetProperty("functionCall", out JsonElement llamada) ||
                        llamada.ValueKind != JsonValueKind.Object ||
                        !llamada.TryGetProperty("name", out JsonElement nombre) ||
                        nombre.ValueKind != JsonValueKind.String)
                    {
                        continue;
                    }

                    string id = llamada.TryGetProperty("id", out JsonElement idElemento) && idElemento.ValueKind == JsonValueKind.String
                        ? idElemento.GetString()
                        : null;
                    string argumentos = llamada.TryGetProperty("args", out JsonElement args) && args.ValueKind == JsonValueKind.Object
                        ? args.GetRawText()
                        : "{}";

                    llamadas.Add(new LlamadaFuncionGemini(id, nombre.GetString(), argumentos));
                }
            }

            return llamadas;
        }

        private static bool IntentarObtenerPartes(JsonElement raiz, out JsonElement parts)
        {
            parts = default(JsonElement);
            if (raiz.ValueKind != JsonValueKind.Object ||
                !raiz.TryGetProperty("candidates", out JsonElement candidates) ||
                candidates.ValueKind != JsonValueKind.Array || candidates.GetArrayLength() == 0)
            {
                return false;
            }

            JsonElement primero = candidates[0];
            if (primero.ValueKind != JsonValueKind.Object ||
                !primero.TryGetProperty("content", out JsonElement content) ||
                content.ValueKind != JsonValueKind.Object ||
                !content.TryGetProperty("parts", out parts) ||
                parts.ValueKind != JsonValueKind.Array || parts.GetArrayLength() == 0)
            {
                return false;
            }

            return true;
        }

        internal static string ExtraerTexto(string jsonRespuesta)
        {
            string texto = ExtraerTextoCrudo(jsonRespuesta);

            if (string.IsNullOrWhiteSpace(texto))
            {
                // Sin candidates (ej. bloqueado por filtros de seguridad) o respuesta vacia.
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
            }

            return texto;
        }

        // Variante que no lanza si no hay texto: usada por llamadas que necesitan un
        // fallback seguro (ej. la clasificacion de agente) en vez de una excepcion.
        internal static string ExtraerTextoCrudo(string jsonRespuesta)
        {
            using (JsonDocument respuesta = JsonDocument.Parse(jsonRespuesta))
            {
                JsonElement raiz = respuesta.RootElement;
                if (raiz.ValueKind != JsonValueKind.Object ||
                    !raiz.TryGetProperty("candidates", out JsonElement candidates) ||
                    candidates.ValueKind != JsonValueKind.Array || candidates.GetArrayLength() == 0)
                {
                    return null;
                }

                JsonElement primero = candidates[0];
                if (primero.ValueKind != JsonValueKind.Object ||
                    !primero.TryGetProperty("content", out JsonElement content) ||
                    content.ValueKind != JsonValueKind.Object ||
                    !content.TryGetProperty("parts", out JsonElement parts) ||
                    parts.ValueKind != JsonValueKind.Array || parts.GetArrayLength() == 0)
                {
                    return null;
                }

                JsonElement parte = parts[0];
                return parte.ValueKind == JsonValueKind.Object &&
                       parte.TryGetProperty("text", out JsonElement text) &&
                       text.ValueKind == JsonValueKind.String
                    ? text.GetString()
                    : null;
            }
        }
    }
}
