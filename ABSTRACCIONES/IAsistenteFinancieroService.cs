using System.Threading.Tasks;
using System.Threading;

namespace ABSTRACCIONES
{
    // Contrato reemplazable del servicio que redacta la respuesta del asistente financiero:
    // cualquier proveedor (Gemini, otro LLM, etc.) se integra implementando esta interfaz en
    // SERVICIOS. No clasifica ni rechaza: la solicitud llega con la categoria ya admitida
    // por el BLL (IClasificadorConsultas). Extrae argumentos por function calling y rutea
    // la redaccion al especialista, sin ejecutar herramientas ni consultar datos de negocio.
    public interface IAsistenteFinancieroService
    {
        Task<string> ObtenerRespuestaAsync(SolicitudAsistente solicitud);
        Task<LlamadaHerramientaAsistente> ObtenerLlamadaHerramientaAsync(SolicitudAsistente solicitud, HerramientaAsistente herramienta, CancellationToken cancellationToken);
    }
}
