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

        // Filtros de alcance: cada lista vacia = sin restriccion en esa dimension.
        public List<int> ComercioIds { get; set; } = new List<int>();

        public List<int> RubroComercioIds { get; set; } = new List<int>();

        public List<int> EntidadFinancieraIds { get; set; } = new List<int>();

        public List<int> MedioPagoIds { get; set; } = new List<int>();

        public List<string> Segmentos { get; set; } = new List<string>();

        // Incluidos: el producto (o su categoria) debe coincidir con alguno. Excluidos: si
        // coincide con alguno, la promocion no aplica (la exclusion gana).
        public List<int> ProductoIncluidoIds { get; set; } = new List<int>();

        public List<int> ProductoExcluidoIds { get; set; } = new List<int>();

        public List<int> CategoriaIncluidaIds { get; set; } = new List<int>();

        public List<int> CategoriaExcluidaIds { get; set; } = new List<int>();
    }
}
