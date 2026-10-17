namespace ABSTRACCIONES
{
    // DTO de un intercambio pregunta/respuesta ya persistido, usado como historial
    // de entrada de IAsistenteFinancieroService para dar coherencia conversacional
    // multi-turno sin inyectar datos de negocio (Movimientos, Categorias, etc.).
    public class IntercambioChatbot
    {
        public string Pregunta { get; set; }

        public string Respuesta { get; set; }
    }
}
