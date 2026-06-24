namespace ABSTRACCIONES
{
    // Interfaz de marcado para entidades de negocio sujetas a control de integridad
    // mediante digito verificador horizontal (DVH). La entidad expone el atributo
    // donde se persiste su firma de fila.
    public interface IIntegrable
    {
        // Digito verificador horizontal calculado para la fila.
        string DVH { get; set; }
    }
}
