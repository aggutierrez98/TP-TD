using System;

namespace BE
{
    public class Promocion
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public decimal Descuento { get; set; }

        public DateTime VigenciaDesde { get; set; }

        public DateTime VigenciaHasta { get; set; }
    }
}
