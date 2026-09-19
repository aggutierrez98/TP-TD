namespace BE
{
    public class Producto
    {
        public int Id { get; set; }

        public Comercio Comercio { get; set; }

        public CategoriaProducto CategoriaProducto { get; set; }

        public Moneda Moneda { get; set; }

        public string Nombre { get; set; }

        public decimal Precio { get; set; }
    }
}
