namespace BE
{
    // Catalogo de productos independiente de los comercios: quien lo vende y a que precio
    // vive en ComercioProducto.
    public class Producto
    {
        public int Id { get; set; }

        public CategoriaProducto CategoriaProducto { get; set; }

        public string Nombre { get; set; }
    }
}
