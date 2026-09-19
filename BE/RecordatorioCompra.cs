using System;

namespace BE
{
    public class RecordatorioCompra : Notificacion
    {
        public ItemLista ItemLista { get; set; }

        public Promocion Promocion { get; set; }

        public DateTime FechaVencimiento { get; set; }
    }
}
