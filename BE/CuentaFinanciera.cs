namespace BE
{
    public class CuentaFinanciera
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public EntidadFinanciera EntidadFinanciera { get; set; }

        public string Alias { get; set; }

        public string Tipo { get; set; }

        public string CredencialesCifradas { get; set; }
    }
}
