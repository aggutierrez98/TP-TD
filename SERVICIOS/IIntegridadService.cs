using System.Collections.Generic;

namespace SERVICIOS
{
    // Contrato del motor matematico de digitos verificadores (T07).
    // Vive en SERVICIOS (no en ABSTRACCIONES) para que la capa BLL pueda consumirlo
    // de forma fuertemente tipada, sin recurrir a object/dynamic.
    public interface IIntegridadService
    {
        // Calcula el digito verificador horizontal (DVH) de una entidad.
        // El motor descubre por reflexion las propiedades marcadas con
        // DigitoVerificadorAttribute, por lo que es aplicable a cualquier entidad.
        string CalcularDVH<T>(T entidad) where T : class;

        // Calcula el digito verificador vertical (DVV) a partir de los DVH de una
        // tabla, en el orden recibido.
        string CalcularDVV(IEnumerable<string> listaDVH);
    }
}
