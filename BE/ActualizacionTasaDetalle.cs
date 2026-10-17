namespace BE
{
    public class ActualizacionTasaDetalle
    {
        public int Id { get; set; }

        public int EjecucionId { get; set; }

        public Moneda MonedaBase { get; set; }

        public Moneda MonedaConversion { get; set; }

        public decimal ValorAnterior { get; set; }

        public decimal? ValorNuevo { get; set; }

        public EstadoActualizacionTasa Estado { get; set; }
    }
}
