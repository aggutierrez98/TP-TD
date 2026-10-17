namespace SERVICIOS
{
    // DTO de agregacion: total de movimientos de un tipo agrupados por categoria,
    // usado por el Dashboard/Balance General. Clase simple (no ValueTuple), mismo
    // criterio que el resto de DTOs de datos del proyecto.
    public class TotalPorCategoria
    {
        public string CategoriaNombre { get; set; }

        public decimal Total { get; set; }
    }
}
