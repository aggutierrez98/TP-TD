using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Contrato del motor matematico de digitos verificadores (T07).
    // El motor descubre por reflexion las propiedades marcadas con
    // DigitoVerificadorAttribute, por lo que es aplicable a cualquier entidad.
    public interface IIntegridadService
    {
        // Calcula el digito verificador horizontal (DVH) de una entidad.
        string CalcularDVH<T>(T entidad) where T : class;

        // Calcula el digito verificador vertical (DVV) a partir de los DVH de una
        // tabla, en el orden recibido.
        string CalcularDVV(IEnumerable<string> listaDVH);
    }
}
