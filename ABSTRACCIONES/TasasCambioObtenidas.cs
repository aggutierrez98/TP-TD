using System;
using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Tasas informadas por el proveedor en una consulta: moneda base (USD en el plan
    // gratuito), instante en que la fuente las publico (nulo si no lo informa) y el
    // valor de una unidad de la base expresado en cada moneda (codigo ISO -> valor).
    public sealed class TasasCambioObtenidas
    {
        public TasasCambioObtenidas(string monedaBase, DateTime? fechaFuenteUtc, IReadOnlyDictionary<string, decimal> tasas)
        {
            MonedaBase = monedaBase;
            FechaFuenteUtc = fechaFuenteUtc;
            Tasas = tasas;
        }

        public string MonedaBase { get; }

        public DateTime? FechaFuenteUtc { get; }

        public IReadOnlyDictionary<string, decimal> Tasas { get; }
    }
}
