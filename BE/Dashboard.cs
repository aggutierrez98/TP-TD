using System.Collections.Generic;

namespace BE
{
    public class Dashboard
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public List<TarjetaDashboard> Tarjetas { get; set; } = new List<TarjetaDashboard>();
    }
}
