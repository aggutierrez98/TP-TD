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

        // Calcula el digito verificador vertical (DVV) como agregado de los DVH
        // de una tabla. Se usa para inicializar firmas y para validar toda la tabla.
        string CalcularDVV(IEnumerable<string> listaDVH);

        // Calcula un nuevo DVV a partir del valor almacenado y el DVH que cambia.
        // Permite mantener el DVV en O(1): resta el DVH anterior y suma el nuevo.
        string CalcularDVVIncremental(string dvvActual, string dvhAnterior, string dvhNuevo);
    }
}
