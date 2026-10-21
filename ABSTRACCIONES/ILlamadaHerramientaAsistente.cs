namespace ABSTRACCIONES
{
    // Contrato neutro: los argumentos se transportan como JSON, sin tipos del proveedor.
    public interface ILlamadaHerramientaAsistente
    {
        string Nombre { get; }

        string ArgumentosJson { get; }
    }
}
