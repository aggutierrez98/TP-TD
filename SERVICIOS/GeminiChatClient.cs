using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ABSTRACCIONES;

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
        internal static async Task<string> PostearYLeerAsync(string requestBody, string apiKey, string modelo)
        {
            // La key va como header (no en la URL) para que no quede expuesta en logs de
            // HttpClient, proxies o telemetria que capturen la URL completa del request.
            string endpoint = EndpointBase + modelo + ":generateContent";

            using (HttpClient cliente = new HttpClient())
            using (StringContent contenido = new StringContent(requestBody, Encoding.UTF8, "application/json"))
            {
                cliente.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

                HttpResponseMessage respuesta = await cliente
                    .PostAsync(endpoint, contenido)
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
        // de la pregunta actual. No se inyecta ningun dato de negocio (Movimientos,
        // Categorias, etc.): eso queda para una iteracion futura.
        internal static string ArmarRequestBody(string promptSistema, string pregunta, IEnumerable<IntercambioChatbot> historial)
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

            var request = new
            {
                systemInstruction = new { parts = new object[] { new { text = promptSistema } } },
                contents = contents.ToArray()
            };

            return new JavaScriptSerializer().Serialize(request);
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
            GeminiApiResponse respuesta = new JavaScriptSerializer().Deserialize<GeminiApiResponse>(jsonRespuesta);
            return respuesta?.candidates != null && respuesta.candidates.Count > 0
                ? respuesta.candidates[0]?.content?.parts != null && respuesta.candidates[0].content.parts.Count > 0
                    ? respuesta.candidates[0].content.parts[0]?.text
                    : null
                : null;
        }

        // Los nombres de propiedad de las clases de abajo reproducen exactamente
        // las claves del JSON de Gemini (camelCase): JavaScriptSerializer mapea
        // por nombre exacto, sin soporte de atributos de renombrado.
        private sealed class GeminiApiResponse
        {
            public List<GeminiCandidate> candidates { get; set; }
        }

        private sealed class GeminiCandidate
        {
            public GeminiContent content { get; set; }
        }

        private sealed class GeminiContent
        {
            public List<GeminiPart> parts { get; set; }
        }

        private sealed class GeminiPart
        {
            public string text { get; set; }
        }
    }
}
