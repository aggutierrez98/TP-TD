using System.Collections.Generic;

namespace BE
{
    public class CuentaFinanciera
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public EntidadFinanciera EntidadFinanciera { get; set; }

        public string Alias { get; set; }

        public TipoCuentaFinanciera Tipo { get; set; }

        public string CredencialesCifradas { get; set; }

        // Subconjunto del catalogo de la EntidadFinanciera que el usuario contrato (N:M).
        public List<MedioPagoEntidad> MediosPagoEntidad { get; set; } = new List<MedioPagoEntidad>();
    }
}
