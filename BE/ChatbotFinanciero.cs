namespace BE
{
    // Fila unica sembrada: el chatbot conversacional de FINAZAPP no tiene alta/baja
    // desde la UI, solo se lee para asociarlo a cada ConsultaChatbot que se persiste.
    public class ChatbotFinanciero
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
    }
}
