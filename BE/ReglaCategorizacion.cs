namespace BE
{
    public class ReglaCategorizacion
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public Categoria Categoria { get; set; }

        public string Nombre { get; set; }

        public string PalabrasClave { get; set; }
    }
}
