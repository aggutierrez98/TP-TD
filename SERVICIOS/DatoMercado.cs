using System;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de IDatoMercado.
    public sealed class DatoMercado : IDatoMercado
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
        public int? MesesInmovilizacion { get; set; }
        public int? DiasHabilesDisponibilidad { get; set; }
        public bool? RequierePreaviso { get; set; }
        public bool? RequierePermanencia { get; set; }
        public string FuenteCondiciones { get; set; }
        public DateTime? FechaDisponibilidadMaxima { get; set; }
    }
}
