namespace ABSTRACCIONES
{
    // Limite de preguntas por minuto de cada usuario del asistente. Devuelve false si
    // el usuario ya agoto su cupo en la ventana; solo las preguntas admitidas lo consumen.
    public interface ILimiteUsoAsistente
    {
        bool IntentarConsumir(IUsuarioAutenticado usuario);
    }
}
