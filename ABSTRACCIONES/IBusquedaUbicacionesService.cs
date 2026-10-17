using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    // Contrato reemplazable para buscar direcciones, comercios o lugares por
    // texto. Devuelve hasta cinco coincidencias (lista vacia si no hay). Ante
    // fallos lanza BusquedaUbicacionException; ante cancelacion del llamador
    // lanza OperationCanceledException.
    public interface IBusquedaUbicacionesService
    {
        Task<IReadOnlyList<ResultadoBusquedaUbicacion>> BuscarAsync(ConsultaUbicacion consulta, CancellationToken cancellationToken);
    }
}
