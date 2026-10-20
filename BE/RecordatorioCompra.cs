using System;

namespace BE
{
    public class RecordatorioCompra : Notificacion
    {
        public ItemLista ItemLista { get; set; }

        public Promocion Promocion { get; set; }

        // Nulo solo en la recomendacion de compras agregada (automatica o ejemplo): un resumen
        // diario no tiene vencimiento y no se le inventa uno.
        public DateTime? FechaVencimiento { get; set; }
    }
}
