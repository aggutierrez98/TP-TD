using ABSTRACCIONES;

namespace SERVICIOS
{
    public class Traduccion : ITraduccion
    {
        public int PalabraId { get; set; }

        public int IdiomaId { get; set; }

        public string Valor { get; set; }

        public IPalabra Palabra { get; set; }
    }
}
