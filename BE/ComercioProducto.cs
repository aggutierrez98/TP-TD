namespace BE
{
    // Un comercio vende un producto del catalogo con su propio precio y moneda.
    public class ComercioProducto
    {
        public Comercio Comercio { get; set; }

        public Producto Producto { get; set; }

        public Moneda Moneda { get; set; }

        public decimal Precio { get; set; }
    }
}
