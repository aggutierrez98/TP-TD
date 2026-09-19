using System;
using System.Collections.Generic;

namespace BE
{
    public class ListaDeCompras
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public string Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }

        public List<ItemLista> ItemsLista { get; set; } = new List<ItemLista>();
    }
}
