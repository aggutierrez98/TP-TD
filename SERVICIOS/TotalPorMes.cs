namespace SERVICIOS
{
    // DTO de agregacion: totales de ingresos/egresos de un usuario agrupados por
    // anio/mes, usado por la tarjeta ComparativoMensual/EvolucionBalance del Dashboard.
    public class TotalPorMes
    {
        public int Anio { get; set; }

        public int Mes { get; set; }

        public decimal TotalIngresos { get; set; }

        public decimal TotalEgresos { get; set; }
    }
}
