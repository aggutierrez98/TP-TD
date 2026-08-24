namespace ABSTRACCIONES
{
    // Sujeto observable del patron Observer aplicado al idioma actual (i18n).
    // Tipado contra IIdioma para no acoplar el contrato a una implementacion concreta.
    public interface IObservado
    {
        IIdioma IdiomaActual { get; }

        void RegistrarObservador(IObservador observador);

        void DesregistrarObservador(IObservador observador);

        void Notificar();

        void CambiarIdioma(IIdioma idioma);
    }
}
