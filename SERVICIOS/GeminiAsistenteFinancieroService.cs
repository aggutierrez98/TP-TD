using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using ABSTRACCIONES;
using System.Text.Json;

namespace SERVICIOS
{
    // Implementacion concreta de IAsistenteFinancieroService sobre la API de Gemini:
    // este es el "AgenteGeneral" que atiende el chat. Antes de responder, clasifica la
    // intencion de la pregunta en Ahorro, Inversiones o General (llamada HTTP liviana y
    // separada, con responseFormat/schema igual que GeminiEscaneoTicketService) y delega
    // en la subclase de AgenteIA correspondiente; si la pregunta es general, o la
    // clasificacion falla o es ambigua por cualquier motivo, responde el mismo con su
    // propio prompt general (fallback seguro, sin romper el comportamiento existente).
    // El historial se manda como contents con roles user/model para dar coherencia
    // conversacional multi-turno. Es una implementacion reemplazable: cualquier otro
    // proveedor solo necesita otra clase que implemente IAsistenteFinancieroService y
    // darla de alta en CompositionRoot.
    public class GeminiAsistenteFinancieroService : IAsistenteFinancieroService
    {
        private const string ModeloPorDefecto = "gemini-3.1-flash-lite";

        private const string PromptSistema =
            "Sos el asistente financiero personal de FINAZAPP. Respondé de forma clara y concisa " +
            "sobre finanzas personales.";

        // Prompt corto para la llamada de clasificacion (separada de la respuesta final):
        // solo le pide a Gemini que devuelva la categoria de especialista, no una respuesta.
        private const string PromptClasificacion =
            "Clasifica la siguiente pregunta de un usuario de FINAZAPP (una app de finanzas " +
            "personales) segun a que especialidad corresponde. Devolve exactamente una de estas " +
            "tres categorias: \"Ahorro\" si la pregunta es sobre gastar menos, ahorrar dinero o " +
            "reducir gastos de compras; \"Inversiones\" si la pregunta es sobre en que invertir, " +
            "tipos de inversion o instrumentos financieros; \"General\" si la pregunta es sobre " +
            "cualquier otro tema de finanzas personales o no encaja claramente en ninguna de las " +
            "dos anteriores. Ante la duda, elegi \"General\".\n\nPregunta del usuario: ";

        private readonly AgenteIA _agenteAhorro;
        private readonly AgenteIA _agenteInversiones;

        public GeminiAsistenteFinancieroService()
            : this(new AgenteAhorro(), new AgenteInversiones())
        {
        }

        // Ctor de testabilidad: permite inyectar dobles de test para las subclases de
        // AgenteIA y verificar la delegacion sin pegarle a la red real de Gemini. El ctor
        // publico sin parametros (usado por CompositionRoot) las instancia de verdad.
        internal GeminiAsistenteFinancieroService(AgenteIA agenteAhorro, AgenteIA agenteInversiones)
        {
            _agenteAhorro = agenteAhorro;
            _agenteInversiones = agenteInversiones;
        }

        public async Task<string> ObtenerRespuestaAsync(string pregunta, IEnumerable<IntercambioChatbot> historial)
        {
            if (string.IsNullOrWhiteSpace(pregunta))
            {
                throw new ExcepcionNegocio("Error.Assistant.QuestionRequired");
            }

            string apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ExcepcionTecnica("Error.Assistant.ApiKeyMissing");
            }

            string modelo = ConfigurationManager.AppSettings["GeminiModel"];
            if (string.IsNullOrWhiteSpace(modelo))
            {
                modelo = ModeloPorDefecto;
            }

            string agenteClasificado = await ClasificarAgenteAsync(pregunta, apiKey, modelo).ConfigureAwait(false);

            return await DelegarSegunClasificacionAsync(agenteClasificado, pregunta, historial, apiKey, modelo).ConfigureAwait(false);
        }

        // Dado el resultado ya clasificado ("Ahorro" | "Inversiones" | "General"), decide a
        // quien responde: separado de ObtenerRespuestaAsync para poder testear la
        // delegacion en aislamiento (sin la llamada HTTP de clasificacion) inyectando
        // dobles de test de AgenteIA via el ctor internal.
        internal Task<string> DelegarSegunClasificacionAsync(string agenteClasificado, string pregunta, IEnumerable<IntercambioChatbot> historial, string apiKey, string modelo)
        {
            if (agenteClasificado == _agenteAhorro.Tipo)
            {
                return _agenteAhorro.ResponderAsync(pregunta, historial);
            }

            if (agenteClasificado == _agenteInversiones.Tipo)
            {
                return _agenteInversiones.ResponderAsync(pregunta, historial);
            }

            return GeminiChatClient.ObtenerRespuestaAsync(PromptSistema, pregunta, historial, apiKey, modelo);
        }

        // Llamada HTTP adicional y liviana (separada de la respuesta final) que solo le
        // pide a Gemini la categoria de especialista. Cualquier fallo (red, API, JSON
        // inesperado) hace fallback seguro a "General": el chat general no debe romperse
        // porque la clasificacion no pudo resolverse.
        private static async Task<string> ClasificarAgenteAsync(string pregunta, string apiKey, string modelo)
        {
            try
            {
                string requestBody = ArmarRequestBodyClasificacion(pregunta);
                string cuerpoRespuesta = await GeminiChatClient.PostearYLeerAsync(requestBody, apiKey, modelo).ConfigureAwait(false);
                string textoJsonInterno = GeminiChatClient.ExtraerTextoCrudo(cuerpoRespuesta);

                return ExtraerAgenteClasificado(textoJsonInterno);
            }
            catch (Exception)
            {
                // Fallback seguro: un fallo en la clasificacion nunca debe romper el chat general.
                return "General";
            }
        }

        internal static string ArmarRequestBodyClasificacion(string pregunta)
        {
            var request = new
            {
                contents = new object[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[] { new { text = PromptClasificacion + pregunta } }
                    }
                },
                generationConfig = new
                {
                    responseFormat = new
                    {
                        text = new
                        {
                            mimeType = "APPLICATION_JSON",
                            schema = new
                            {
                                type = "object",
                                properties = new
                                {
                                    agente = new
                                    {
                                        type = "string",
                                        @enum = new[] { "Ahorro", "Inversiones", "General" },
                                        description = "Categoria de especialista a la que corresponde la pregunta."
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(request);
        }

        // No lanza excepciones: cualquier JSON invalido, inesperado o con un valor de
        // "agente" desconocido hace fallback seguro a "General".
        internal static string ExtraerAgenteClasificado(string textoJsonInterno)
        {
            if (string.IsNullOrWhiteSpace(textoJsonInterno))
            {
                return "General";
            }

            try
            {
                using (JsonDocument clasificacion = JsonDocument.Parse(textoJsonInterno))
                {
                    JsonElement raiz = clasificacion.RootElement;
                    if (raiz.ValueKind == JsonValueKind.Object &&
                        raiz.TryGetProperty("agente", out JsonElement agente) &&
                        agente.ValueKind == JsonValueKind.String)
                    {
                        string valor = agente.GetString();
                        if (valor == "Ahorro" || valor == "Inversiones")
                        {
                            return valor;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // JSON invalido o inesperado: fallback seguro a General.
            }

            return "General";
        }

        // Wrapper que mantiene la firma historica usada por los tests existentes; delega
        // en el cliente compartido con el prompt general de este agente.
        internal static string ArmarRequestBody(string pregunta, IEnumerable<IntercambioChatbot> historial)
        {
            return GeminiChatClient.ArmarRequestBody(PromptSistema, pregunta, historial);
        }

    }
}
