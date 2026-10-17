using System;

namespace SERVICIOS
{
    // DTO de agregacion: total de movimientos de un tipo en una fecha (sin hora).
    public class TotalPorDia
    {
        public DateTime Fecha { get; set; }

        public decimal Total { get; set; }
    }
}
