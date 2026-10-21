namespace ABSTRACCIONES
{
    public interface ITraduccion
    {
        IIdioma Idioma { get; set; }

        string Valor { get; set; }

        IPalabra Palabra { get; set; }
    }
}
