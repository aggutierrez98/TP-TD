namespace ABSTRACCIONES
{
    public interface IResultadoComparacionEstado
    {
        string Campo { get; set; }

        string ValorAnterior { get; set; }

        string ValorNuevo { get; set; }
    }
}
