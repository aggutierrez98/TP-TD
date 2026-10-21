namespace ABSTRACCIONES
{
    // Nombre visible de una entidad (comercio, promocion, cuenta o instrumento) que el
    // validador de respuestas busca en el texto generado por el modelo.
    public interface IReferenciaAsistente
    {
        TipoReferenciaAsistente Tipo { get; }

        string Nombre { get; }

        bool EsTicker { get; }
    }
}
