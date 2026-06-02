using ABSTRACCIONES;

namespace SERVICIOS
{
    public class Palabra : IPalabra
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public bool EsBase { get; set; }
    }
}
