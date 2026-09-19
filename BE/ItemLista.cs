namespace BE
{
    public class ItemLista
    {
        public int Id { get; set; }

        public ListaDeCompras ListaDeCompras { get; set; }

        public string Descripcion { get; set; }

        public int Cantidad { get; set; }

        public bool Comprado { get; set; }
    }
}
