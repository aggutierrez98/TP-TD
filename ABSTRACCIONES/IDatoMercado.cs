using System;

namespace ABSTRACCIONES
{
    // Contrato de transporte acotado: no contiene entidades ni estado del usuario.
    public interface IDatoMercado
    {
        string Nombre { get; }
        string Tipo { get; }
        string Fuente { get; }
        DateTime Fecha { get; }
        decimal? Precio { get; }
        decimal? Volumen { get; }
        decimal? Vcp { get; }
        decimal? Tasa { get; }
        string TipoTasa { get; }
        string Moneda { get; }
        // Solo completar con condiciones verificadas por el proveedor o un catalogo versionado.
        // null significa desconocido, nunca disponibilidad inmediata.
        int? MesesInmovilizacion { get; }
        int? DiasHabilesDisponibilidad { get; }
        bool? RequierePreaviso { get; }
        bool? RequierePermanencia { get; }
        string FuenteCondiciones { get; }
        DateTime? FechaDisponibilidadMaxima { get; }
    }
}
