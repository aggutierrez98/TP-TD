namespace TpIDS
{
    // Item de presentacion para combos: separa el valor de dominio (Valor) de su texto
    // traducible (Clave -> Texto). IdiomaFormHelper traduce el Texto al cambiar el idioma,
    // sin que el formulario tenga que escuchar la notificacion.
    internal sealed class OpcionTraducible
    {
        public OpcionTraducible(string clave, object valor)
        {
            Clave = clave;
            Valor = valor;
        }

        public string Clave { get; }

        public object Valor { get; }

        public string Texto { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Texto) ? Clave : Texto;
        }
    }
}
