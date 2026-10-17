using System;

namespace BE
{
    public class TasaConversion
    {
        public int Id { get; set; }

        public Moneda MonedaBase { get; set; }

        public Moneda MonedaConversion { get; set; }

        public decimal CalculoConversion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public bool EsManual { get; set; }

        public int? ProtegidaPorUsuarioId { get; set; }

        public DateTime? FechaProteccion { get; set; }
    }
}
