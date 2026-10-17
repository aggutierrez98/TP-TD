using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    // Contrato reemplazable del asistente financiero conversacional: cualquier
    // proveedor (Gemini, otro LLM, etc.) se integra implementando esta unica
    // interfaz en SERVICIOS, sin tocar BLL ni UI. Por ahora solo chat libre: sin
    // inyeccion de datos de negocio (Movimientos, Categorias, etc.); eso queda
    // reservado a una iteracion futura con recomendaciones financieras reales
    // (AgenteIA / SugerenciaFinanciera del modelo de dominio, fuera de alcance).
    public interface IAsistenteFinancieroService
    {
        Task<string> ObtenerRespuestaAsync(string pregunta, IEnumerable<IntercambioChatbot> historial);
    }
}
