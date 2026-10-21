using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    // Contrato reemplazable para el escaneo de tickets: cualquier proveedor
    // (Google Vision, Document AI, Gemini, etc.) se integra implementando esta
    // unica interfaz en SERVICIOS, sin tocar BLL ni UI.
    public interface IEscaneoTicketService
    {
        Task<ITicketEscaneado> EscanearTicketAsync(byte[] imagenBytes);
    }
}
