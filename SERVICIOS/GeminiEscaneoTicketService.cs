using System;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ABSTRACCIONES;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SERVICIOS
{
    // Implementacion concreta de IEscaneoTicketService sobre la API de Gemini
    // (multimodal): a diferencia de un OCR crudo, el modelo devuelve los campos
    // de negocio ya estructurados via responseSchema, sin necesitar un parser
    // de texto propio. Es una implementacion reemplazable: cualquier otro
    // proveedor solo necesita otra clase que implemente IEscaneoTicketService
    // y darla de alta en CompositionRoot.
    public class GeminiEscaneoTicketService : IEscaneoTicketService
    {
        private const string EndpointBase = "https://generativelanguage.googleapis.com/v1beta/models/";
        private const string ModeloPorDefecto = "gemini-3.1-flash-lite";

        private const string Prompt =
            "Analiza la imagen de un ticket o factura de compra. Extrae el monto total pagado, " +
            "la fecha de la compra, el nombre del comercio y todo el texto legible del ticket. " +
            "Si no podes determinar con confianza el monto o la fecha, devolve null en ese campo " +
            "en vez de inventar un valor.";

        public async Task<ITicketEscaneado> EscanearTicketAsync(byte[] imagenBytes)
        {
            if (imagenBytes == null || imagenBytes.Length == 0)
            {
                throw new ExcepcionNegocio("Error.Ticket.ImageRequired");
            }

            string apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ExcepcionTecnica("Error.Ticket.ApiKeyMissing");
            }

            string modelo = ConfigurationManager.AppSettings["GeminiModel"];
            if (string.IsNullOrWhiteSpace(modelo))
            {
                modelo = ModeloPorDefecto;
            }

            return await LlamarApiYArmarResultadoAsync(imagenBytes, apiKey, modelo).ConfigureAwait(false);
        }

        private static async Task<TicketEscaneado> LlamarApiYArmarResultadoAsync(byte[] imagenBytes, string apiKey, string modelo)
        {
            try
            {
                string requestBody = ArmarRequestBody(imagenBytes);
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
                        throw new ExcepcionTecnica("Error.Ticket.ScanFailed");
                    }

                    return ExtraerTicket(cuerpoRespuesta);
                }
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
                throw new ExcepcionTecnica("Error.Ticket.ScanFailed", ex);
            }
        }

        internal static string ArmarRequestBody(byte[] imagenBytes)
        {
            var request = new
            {
                contents = new object[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { inlineData = new { mimeType = ObtenerMimeType(imagenBytes), data = Convert.ToBase64String(imagenBytes) } },
                            new { text = Prompt }
                        }
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
                                    monto = new { type = new[] { "number", "null" }, description = "Monto total final pagado; null si no es determinable con confianza." },
                                    fecha = new { type = new[] { "string", "null" }, format = "date", description = "Fecha de compra en formato ISO 8601 YYYY-MM-DD; null si no es determinable con confianza." },
                                    descripcion = new { type = "string", description = "Nombre del comercio o descripcion breve del gasto." },
                                    textoDetectado = new { type = "string", description = "Todo el texto legible extraido del ticket." }
                                }
                            }
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(request);
        }

        private static string ObtenerMimeType(byte[] imagenBytes)
        {
            return imagenBytes.Length >= 8
                && imagenBytes[0] == 137
                && imagenBytes[1] == 80
                && imagenBytes[2] == 78
                && imagenBytes[3] == 71
                && imagenBytes[4] == 13
                && imagenBytes[5] == 10
                && imagenBytes[6] == 26
                && imagenBytes[7] == 10
                ? "image/png"
                : "image/jpeg";
        }

        internal static TicketEscaneado ExtraerTicket(string jsonRespuesta)
        {
            string textoJsonInterno = GeminiChatClient.ExtraerTextoCrudo(jsonRespuesta);

            if (string.IsNullOrWhiteSpace(textoJsonInterno))
            {
                // Sin candidates (ej. bloqueado por filtros de seguridad) o respuesta vacia.
                throw new ExcepcionTecnica("Error.Ticket.ScanFailed");
            }

            TicketExtraido extraido;
            try
            {
                extraido = JsonSerializer.Deserialize<TicketExtraido>(textoJsonInterno);
            }
            catch (Exception ex)
            {
                throw new ExcepcionTecnica("Error.Ticket.ScanFailed", ex);
            }

            return new TicketEscaneado
            {
                Monto = extraido?.Monto,
                Fecha = ParsearFecha(extraido?.Fecha),
                Descripcion = extraido?.Descripcion,
                TextoCrudo = extraido?.TextoDetectado ?? string.Empty
            };
        }

        private static DateTime? ParsearFecha(string fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
            {
                return null;
            }

            return DateTime.TryParse(fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultado)
                ? resultado
                : (DateTime?)null;
        }

        private sealed class TicketExtraido
        {
            public TicketExtraido()
            {
            }

            [JsonPropertyName("monto")]
            public decimal? Monto { get; set; }

            [JsonPropertyName("fecha")]
            public string Fecha { get; set; }

            [JsonPropertyName("descripcion")]
            public string Descripcion { get; set; }

            [JsonPropertyName("textoDetectado")]
            public string TextoDetectado { get; set; }
        }
    }
}
