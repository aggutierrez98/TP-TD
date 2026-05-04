namespace ABSTRACCIONES
{
    public interface IObservado
    {
        IIdioma IdiomaActual { get; }

        void RegistrarObservador(IObservador observador);

        void DesregistrarObservador(IObservador observador);

        void ActualizarObservadores();

        void CambiarIdioma(IIdioma idioma);
    }
}
