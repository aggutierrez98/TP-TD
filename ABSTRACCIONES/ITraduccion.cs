namespace ABSTRACCIONES
{
    public interface ITraduccion
    {
        int PalabraId { get; set; }

        int IdiomaId { get; set; }

        string Valor { get; set; }

        IPalabra Palabra { get; set; }
    }
}
