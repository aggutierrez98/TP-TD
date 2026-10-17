namespace BE
{
    public class Comercio
    {
        public int Id { get; set; }

        public RubroComercio RubroComercio { get; set; }

        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Localidad { get; set; }

        public string Provincia { get; set; }

        public decimal? Latitud { get; set; }

        public decimal? Longitud { get; set; }
    }
}
