namespace ABSTRACCIONES
{
    // Observador del patron Observer: reacciona a los cambios de idioma del IObservado.
    public interface IObservador
    {
        void Actualizar(IObservado observado);
    }
}
