using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net.Http;
using System.Configuration;
using System.Text.Json;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de IAsistenteFinancieroService sobre Gemini. No clasifica ni rechaza:
    // la categoria llega ya admitida por el BLL (IClasificadorConsultas) y este servicio solo
    // rutea al especialista: Compras -> AgenteAhorro, Inversiones -> AgenteInversiones,
    // FinanzasGenerales -> AgenteFinanzasGenerales. Una categoria no admitida que llegue igual
    // (defensa en profundidad) se corta con ExcepcionTecnica sin consultar al modelo.
    public class GeminiAsistenteFinancieroService : IAsistenteFinancieroService
    {
        private readonly AgenteIA _agenteAhorro;
        private readonly AgenteIA _agenteInversiones;
        private readonly AgenteIA _agenteFinanzasGenerales;
        private readonly HttpMessageHandler _manejador;
        private readonly Func<string, string> _configuracion;

        public GeminiAsistenteFinancieroService()
            : this(new AgenteAhorro(), new AgenteInversiones(), new AgenteFinanzasGenerales())
        {
        }

        // Ctor de testabilidad: dobles de AgenteIA para verificar el ruteo sin red.
        internal GeminiAsistenteFinancieroService(AgenteIA agenteAhorro, AgenteIA agenteInversiones, AgenteIA agenteFinanzasGenerales, HttpMessageHandler manejador = null, Func<string, string> configuracion = null)
        {
            _agenteAhorro = agenteAhorro;
            _agenteInversiones = agenteInversiones;
            _agenteFinanzasGenerales = agenteFinanzasGenerales;
            _manejador = manejador;
            _configuracion = configuracion ?? (clave => ConfigurationManager.AppSettings[clave]);
        }

        public Task<string> ObtenerRespuestaAsync(ISolicitudAsistente solicitud)
        {
            if (solicitud == null || string.IsNullOrWhiteSpace(solicitud.Pregunta))
            {
                throw new ExcepcionNegocio("Error.Assistant.QuestionRequired");
            }

            switch (solicitud.Categoria)
            {
                case CategoriaConsulta.Compras:
                    return _agenteAhorro.ResponderAsync(solicitud);
                case CategoriaConsulta.Inversiones:
                    return _agenteInversiones.ResponderAsync(solicitud);
                case CategoriaConsulta.FinanzasGenerales:
                    return _agenteFinanzasGenerales.ResponderAsync(solicitud);
                default:
                    throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
            }
        }

        public async Task<ILlamadaHerramientaAsistente> ObtenerLlamadaHerramientaAsync(ISolicitudAsistente solicitud, IHerramientaAsistente herramienta, CancellationToken cancellationToken)
        {
            string apiKey = _configuracion("GeminiApiKey");
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ExcepcionTecnica("Error.Assistant.ApiKeyMissing");
            string modelo = _configuracion("GeminiModel");
            if (string.IsNullOrWhiteSpace(modelo)) modelo = GeminiChatClient.ModeloPorDefecto;
            using (JsonDocument esquema = JsonDocument.Parse(herramienta.ParametrosJson))
            {
                OpcionesGeneracionGemini opciones = new OpcionesGeneracionGemini
                {
                    Temperatura = 0,
                    Funciones = new[] { new DeclaracionFuncionGemini(herramienta.Nombre, herramienta.Descripcion, esquema.RootElement.Clone()) },
                    FuncionesPermitidas = new[] { herramienta.Nombre }
                };
                var llamadas = await GeminiChatClient.ObtenerLlamadasFuncionAsync(
                    AgenteIA.ReglasDeAlcance + " Invoca la funcion indicada. Extrae solo argumentos explicitos de la consulta; si no se nombra una lista, omiti nombreLista.",
                    solicitud.Pregunta, solicitud.Historial, opciones, apiKey, modelo, _manejador, cancellationToken).ConfigureAwait(false);
                LlamadaFuncionGemini llamada = llamadas.FirstOrDefault(llamadaFuncion => llamadaFuncion.Nombre == herramienta.Nombre);
                return llamada == null ? null : new LlamadaHerramientaAsistente(llamada.Nombre, llamada.ArgumentosJson);
            }
        }
    }
}
