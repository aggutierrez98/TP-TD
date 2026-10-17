using System.Collections.Generic;

namespace BE
{
    public class EntidadFinanciera
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        // Catalogo completo de medios de pago que ofrece esta entidad.
        public List<MedioPagoEntidad> MediosPagoEntidad { get; set; } = new List<MedioPagoEntidad>();
    }
}
