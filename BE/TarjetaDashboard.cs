namespace BE
{
    public class TarjetaDashboard
    {
        public int Id { get; set; }

        public Dashboard Dashboard { get; set; }

        public TipoTarjetaDashboard Tipo { get; set; }

        public int Orden { get; set; }
    }
}
