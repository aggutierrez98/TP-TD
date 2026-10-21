using ABSTRACCIONES;

namespace SERVICIOS
{
    public class Traduccion : ITraduccion
    {
        public IIdioma Idioma { get; set; }

        public string Valor { get; set; }

        public Palabra Palabra { get; set; }

        IPalabra ITraduccion.Palabra
        {
            get { return Palabra; }
            set { Palabra = value as Palabra; }
        }
    }
}
