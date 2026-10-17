namespace BE
{
    // Catalogo global de medios de pago (Tipo + Marca, ej. "Visa Credito"), independiente
    // de la entidad que lo emite. El producto concreto de una entidad es MedioPagoEntidad.
    public class MedioPago
    {
        public int Id { get; set; }

        public TipoMedioPago Tipo { get; set; }

        public string Marca { get; set; }
    }
}
