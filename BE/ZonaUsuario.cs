namespace BE
{
    // Lugar habitual del usuario (casa, trabajo) con su propio radio de cercania en metros.
    public class ZonaUsuario
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public string Nombre { get; set; }

        public decimal Latitud { get; set; }

        public decimal Longitud { get; set; }

        public int RadioMetros { get; set; }
    }
}
