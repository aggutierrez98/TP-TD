using System;
using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Tasas informadas por el proveedor en una consulta: moneda base (USD en el plan
    // gratuito), instante en que la fuente las publico (nulo si no lo informa) y el
    // valor de una unidad de la base expresado en cada moneda (codigo ISO -> valor).
    public interface ITasasCambioObtenidas
    {
        string MonedaBase { get; }

        DateTime? FechaFuenteUtc { get; }

        IReadOnlyDictionary<string, decimal> Tasas { get; }
    }
}
