using System.Collections.Generic;

namespace BE
{
    public sealed class PaginaNotificaciones
    {
        public const int TamanoPagina = 10;
        public List<Notificacion> Elementos { get; set; } = new List<Notificacion>();
        public int Total { get; set; }
        public int Pagina { get; set; } = 1;
        public int TotalPaginas => (Total + TamanoPagina - 1) / TamanoPagina;
    }
}
