using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Clase base de los agentes especialistas de FINAZAPP (AgenteAhorro, AgenteInversiones).
    // Encapsula el llamado comun a Gemini via GeminiChatClient; cada subclase solo aporta
    // su propio prompt de especialista. Son expertos "por prompt": no consultan datos reales
    // del sistema (Movimientos, ListaDeCompras, Presupuesto), y si necesitan cifras del
    // usuario se las piden en la conversacion en vez de leerlas de la base.
    // TODO: usar embeddings/datos reales de Movimientos, ListaDeCompras y Presupuesto
    // cuando exista esa integracion (idem TODO de embeddings de IAsistenteFinancieroService).
    internal abstract class AgenteIA
    {
        // Identifica al agente para que AgenteGeneral (GeminiAsistenteFinancieroService)
        // pueda matchear el resultado de su clasificacion de intencion contra esta subclase.
        internal abstract string Tipo { get; }

        protected abstract string PromptSistema { get; }

        internal virtual async Task<string> ResponderAsync(string pregunta, IEnumerable<IntercambioChatbot> historial)
        {
            string apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ExcepcionTecnica("Error.Assistant.ApiKeyMissing");
            }

            string modelo = ConfigurationManager.AppSettings["GeminiModel"];
            if (string.IsNullOrWhiteSpace(modelo))
            {
                modelo = GeminiChatClient.ModeloPorDefecto;
            }

            return await GeminiChatClient.ObtenerRespuestaAsync(PromptSistema, pregunta, historial, apiKey, modelo).ConfigureAwait(false);
        }
    }
}
