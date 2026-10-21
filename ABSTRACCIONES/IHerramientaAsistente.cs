namespace ABSTRACCIONES
{
    // Contrato neutro: el esquema se transporta como JSON, sin tipos del proveedor.
    public interface IHerramientaAsistente
    {
        string Nombre { get; }

        string Descripcion { get; }

        string ParametrosJson { get; }
    }
}
