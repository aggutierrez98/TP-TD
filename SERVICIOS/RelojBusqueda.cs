using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SERVICIOS
{
    // Reloj y espera sustituibles para NominatimBusquedaUbicacionesService: el
    // limitador, el cooldown, el cache y el timeout dependen solo de este
    // contrato, asi los tests los gobiernan sin esperas reales. Monotonico mide
    // intervalos (inmune a cambios del reloj del sistema); AhoraUtc solo se usa
    // para convertir un Retry-After con fecha absoluta en una duracion.
    internal interface IRelojBusqueda
    {
        TimeSpan Monotonico { get; }

        DateTimeOffset AhoraUtc { get; }

        Task EsperarAsync(TimeSpan duracion, CancellationToken cancellationToken);
    }

    internal sealed class RelojSistemaBusqueda : IRelojBusqueda
    {
        private static readonly Stopwatch Cronometro = Stopwatch.StartNew();

        public TimeSpan Monotonico
        {
            get { return Cronometro.Elapsed; }
        }

        public DateTimeOffset AhoraUtc
        {
            get { return DateTimeOffset.UtcNow; }
        }

        public Task EsperarAsync(TimeSpan duracion, CancellationToken cancellationToken)
        {
            return Task.Delay(duracion, cancellationToken);
        }
    }
}
