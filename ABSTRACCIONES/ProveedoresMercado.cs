using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    public interface IProveedorMercadoData912
    {
        Task<IReadOnlyList<IDatoMercado>> ObtenerCotizacionesAsync(bool cedears, CancellationToken cancellationToken);
    }

    public interface IProveedorArgentinaDatos
    {
        Task<IReadOnlyList<IDatoMercado>> ObtenerFondosAsync(string tipo, DateTime? fecha, CancellationToken cancellationToken);
        Task<IReadOnlyList<IDatoMercado>> ObtenerPlazosFijosAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<IDatoMercado>> ObtenerCuentasRemuneradasAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<IDatoMercado>> ObtenerInflacionAsync(CancellationToken cancellationToken);
    }

    public interface IProveedorCriptoYa
    {
        Task<IReadOnlyList<IDatoMercado>> ObtenerPreciosAsync(string moneda, CancellationToken cancellationToken);
    }
}
