namespace SERVICIOS
{
    // DTO de agregacion: resumen total (sin agrupar) de ingresos/egresos de un
    // usuario en un rango de fechas, usado por el Balance General.
    public class ResumenMovimientos
    {
        public decimal TotalIngresos { get; set; }

        public decimal TotalEgresos { get; set; }
    }
}
