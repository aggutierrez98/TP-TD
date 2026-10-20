using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ABSTRACCIONES
{
    public interface IProveedorMercadoData912
    {
        Task<IReadOnlyList<DatoMercado>> ObtenerCotizacionesAsync(bool cedears, CancellationToken cancellationToken);
    }

    public interface IProveedorArgentinaDatos
    {
        Task<IReadOnlyList<DatoMercado>> ObtenerFondosAsync(string tipo, DateTime? fecha, CancellationToken cancellationToken);
        Task<IReadOnlyList<DatoMercado>> ObtenerPlazosFijosAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<DatoMercado>> ObtenerCuentasRemuneradasAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<DatoMercado>> ObtenerInflacionAsync(CancellationToken cancellationToken);
    }

    public interface IProveedorCriptoYa
    {
        Task<IReadOnlyList<DatoMercado>> ObtenerPreciosAsync(string moneda, CancellationToken cancellationToken);
    }

    // Contrato de transporte acotado: no contiene entidades ni estado del usuario.
    public sealed class DatoMercado
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Fuente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? Vcp { get; set; }
        public decimal? Tasa { get; set; }
        public string TipoTasa { get; set; }
        public string Moneda { get; set; }
        // Solo completar con condiciones verificadas por el proveedor o un catalogo versionado.
        // null significa desconocido, nunca disponibilidad inmediata.
        public int? MesesInmovilizacion { get; set; }
        public int? DiasHabilesDisponibilidad { get; set; }
        public bool? RequierePreaviso { get; set; }
        public bool? RequierePermanencia { get; set; }
        public string FuenteCondiciones { get; set; }
        public DateTime? FechaDisponibilidadMaxima { get; set; }
    }
}
