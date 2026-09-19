namespace BE
{
    public class MedioDePago
    {
        public int Id { get; set; }

        public EntidadFinanciera EntidadFinanciera { get; set; }

        public TipoMedioDePago Tipo { get; set; }

        public string Marca { get; set; }

        public string Segmento { get; set; }
    }
}
