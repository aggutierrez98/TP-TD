using System.Collections.Generic;

namespace SERVICIOS
{
    // Resultado de negocio de la tarjeta "Gasto acumulado del mes": acumulado de egresos por dia
    // del mes en curso (hasta hoy) y del mes anterior (completo), y su comparacion al mismo dia.
    public class GastoAcumuladoMensual
    {
        public List<decimal> MesActual { get; set; } = new List<decimal>();

        public List<decimal> MesAnterior { get; set; } = new List<decimal>();

        public decimal AcumuladoActual { get; set; }

        public decimal AcumuladoAnteriorAlMismoDia { get; set; }

        // Null cuando el mes anterior no tiene egresos hasta el mismo dia (no se puede comparar).
        public decimal? VariacionPorcentual { get; set; }
    }
}
