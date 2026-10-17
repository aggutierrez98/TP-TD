using System.Collections.Generic;

namespace SERVICIOS
{
    // Resultado de negocio de la tarjeta "Gastos por comercio": los comercios de mayor gasto
    // (ordenados de mayor a menor) y, aparte, lo gastado sin comercio asociado.
    public class GastoPorComercio
    {
        public List<TotalPorComercio> Comercios { get; set; } = new List<TotalPorComercio>();

        public decimal TotalSinComercio { get; set; }
    }
}
