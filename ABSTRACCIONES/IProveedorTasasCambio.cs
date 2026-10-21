using System.Threading;
using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    // Contrato reemplazable para obtener tasas de cambio de una fuente externa con
    // una sola consulta. Ante fallos lanza ProveedorTasasException; ante cancelacion
    // del llamador lanza OperationCanceledException. No reintenta.
    public interface IProveedorTasasCambio
    {
        Task<ITasasCambioObtenidas> ObtenerTasasAsync(CancellationToken cancellationToken);
    }
}
