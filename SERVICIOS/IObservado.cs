namespace SERVICIOS
{
    public interface IObservado
    {
        Idioma IdiomaActual { get; }

        void RegistrarObservador(IObservador observador);

        void DesregistrarObservador(IObservador observador);

        void Notificar();

        void CambiarIdioma(Idioma idioma);
    }
}
