namespace ABSTRACCIONES
{
    // Solicitud de busqueda textual de una direccion, comercio o lugar.
    // Idioma es el codigo de dos letras de la interfaz ("es" o "en") y orienta
    // el idioma de las descripciones. ZonaPreferida es opcional: favorece los
    // resultados de esa zona sin excluir los externos ni fijar un pais.
    public interface IConsultaUbicacion
    {
        string Texto { get; }

        string Idioma { get; }

        ILimitesGeograficos ZonaPreferida { get; }
    }
}
