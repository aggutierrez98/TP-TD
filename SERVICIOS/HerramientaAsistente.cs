using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementaciones de IHerramientaAsistente e ILlamadaHerramientaAsistente.
    public sealed class HerramientaAsistente : IHerramientaAsistente
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

    public sealed class LlamadaHerramientaAsistente : ILlamadaHerramientaAsistente
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
