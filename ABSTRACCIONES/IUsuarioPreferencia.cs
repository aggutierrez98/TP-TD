namespace ABSTRACCIONES
{
    public interface IUsuarioPreferencia
    {
        IUsuarioAutenticado Usuario { get; set; }

        IIdioma Idioma { get; set; }
    }
}
