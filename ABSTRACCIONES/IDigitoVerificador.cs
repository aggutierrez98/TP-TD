namespace ABSTRACCIONES
{
    // Contrato del atributo que marca una propiedad para el digito verificador
    // horizontal. La implementacion vive en BE (DigitoVerificadorAttribute) y el
    // motor de SERVICIOS la descubre por reflexion a traves de esta interfaz.
    public interface IDigitoVerificador
    {
        // Posicion del atributo dentro de la entidad (1-based).
        int Orden { get; }
    }
}
