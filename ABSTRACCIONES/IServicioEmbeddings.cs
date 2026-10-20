using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    // Calcula la representacion semantica (vector) de textos, en lote. Es reemplazable: otro
    // proveedor solo necesita otra implementacion en SERVICIOS y su alta en CompositionRoot.
    // Modelo identifica el espacio de los vectores: vectores de modelos distintos no son
    // comparables. Ante cualquier fallo lanza una excepcion; quien consume decide como degradar.
    public interface IServicioEmbeddings
    {
        string Modelo { get; }

        // Un vector por texto, en el mismo orden.
        Task<IReadOnlyList<float[]>> CalcularAsync(IReadOnlyList<string> textos, CancellationToken cancellationToken);
    }
}
