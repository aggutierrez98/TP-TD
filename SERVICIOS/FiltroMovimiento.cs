using System;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class FiltroMovimiento
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public int? CategoriaId { get; set; }

        public TipoMovimiento? Tipo { get; set; }

        public string Texto { get; set; }
    }
}
