namespace ABSTRACCIONES
{
    // Intercambio pregunta/respuesta ya persistido, usado como historial
    // de entrada de IAsistenteFinancieroService para dar coherencia conversacional
    // multi-turno sin inyectar datos de negocio (Movimientos, Categorias, etc.).
    public interface IIntercambioChatbot
    {
        string Pregunta { get; }

        string Respuesta { get; }
    }
}
