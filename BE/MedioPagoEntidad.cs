namespace BE
{
    // Producto real que ofrece una EntidadFinanciera (ej. Galicia + Visa Credito + Eminent).
    public class MedioPagoEntidad
    {
        public int Id { get; set; }

        public EntidadFinanciera EntidadFinanciera { get; set; }

        public MedioPago MedioPago { get; set; }

        public string Segmento { get; set; }
    }
}
