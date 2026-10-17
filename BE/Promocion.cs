using System;
using System.Collections.Generic;

namespace BE
{
    public class Promocion
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public decimal Descuento { get; set; }

        // Tope de descuento en pesos (ARS implicito); null = sin tope.
        public decimal? TopeDescuento { get; set; }

        public DateTime VigenciaDesde { get; set; }

        public DateTime VigenciaHasta { get; set; }

        // Lista vacia = la promocion aplica todos los dias.
        public List<DiaSemana> DiasSemana { get; set; } = new List<DiaSemana>();
    }
}
