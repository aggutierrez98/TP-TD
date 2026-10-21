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
        public List<Comercio> Comercios { get; set; } = new List<Comercio>();

        public List<RubroComercio> RubrosComercio { get; set; } = new List<RubroComercio>();

        public List<EntidadFinanciera> EntidadesFinancieras { get; set; } = new List<EntidadFinanciera>();

        public List<MedioPago> MediosPago { get; set; } = new List<MedioPago>();

        public List<string> Segmentos { get; set; } = new List<string>();

        // Incluidos: el producto (o su categoria) debe coincidir con alguno. Excluidos: si
        // coincide con alguno, la promocion no aplica (la exclusion gana).
        public List<Producto> ProductosIncluidos { get; set; } = new List<Producto>();

        public List<Producto> ProductosExcluidos { get; set; } = new List<Producto>();

        public List<CategoriaProducto> CategoriasIncluidas { get; set; } = new List<CategoriaProducto>();

        public List<CategoriaProducto> CategoriasExcluidas { get; set; } = new List<CategoriaProducto>();
    }
}
