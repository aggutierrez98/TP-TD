using System;
using System.Collections.Generic;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de ITasasCambioObtenidas.
    public sealed class TasasCambioObtenidas : ITasasCambioObtenidas
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
