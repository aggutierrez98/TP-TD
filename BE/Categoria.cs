namespace BE
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public Usuario Usuario { get; set; } // Usuario == null => Categoria Global

        public bool EsGlobal
        {
            get { return Usuario == null; }
        }
    }
}
