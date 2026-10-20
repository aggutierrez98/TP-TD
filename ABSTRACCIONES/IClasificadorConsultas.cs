using System.Threading;
using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    // Clasifica una pregunta del asistente personal en una categoria cerrada. Es
    // reemplazable: otro proveedor solo necesita otra implementacion en SERVICIOS y su
    // alta en CompositionRoot. Quien consume trata cualquier excepcion como rechazo
    // (fail-closed), salvo la falta de configuracion, que se informa como error.
    public interface IClasificadorConsultas
    {
        Task<CategoriaConsulta> ClasificarAsync(string pregunta, CancellationToken cancellationToken);
    }
}
