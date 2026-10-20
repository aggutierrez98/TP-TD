namespace ABSTRACCIONES
{
    // Contrato neutro: el esquema y los argumentos se transportan como JSON, sin tipos del proveedor.
    public sealed class HerramientaAsistente
    {
        public HerramientaAsistente(string nombre, string descripcion, string parametrosJson)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            ParametrosJson = parametrosJson;
        }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public string ParametrosJson { get; private set; }
    }

    public sealed class LlamadaHerramientaAsistente
    {
        public LlamadaHerramientaAsistente(string nombre, string argumentosJson)
        {
            Nombre = nombre;
            ArgumentosJson = argumentosJson;
        }
        public string Nombre { get; private set; }
        public string ArgumentosJson { get; private set; }
    }
}
