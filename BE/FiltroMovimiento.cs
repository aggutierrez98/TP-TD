using System;
using ABSTRACCIONES;

namespace BE
{
    public class FiltroMovimiento
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public Categoria Categoria { get; set; }

        public TipoMovimiento? Tipo { get; set; }

        public string Texto { get; set; }
    }
}
