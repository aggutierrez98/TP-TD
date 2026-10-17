namespace SERVICIOS
{
    // DTO de agregacion: total de movimientos de un tipo agrupados por comercio.
    // ComercioNombre es null para los movimientos sin comercio asociado.
    public class TotalPorComercio
    {
        public string ComercioNombre { get; set; }

        public decimal Total { get; set; }
    }
}
