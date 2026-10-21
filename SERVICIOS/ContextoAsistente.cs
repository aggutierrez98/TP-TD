using System.Collections.Generic;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Implementacion de IContextoAsistente.
    public class ContextoAsistente : IContextoAsistente
    {
        public static readonly ContextoAsistente Vacio = new ContextoAsistente(null, new ReferenciaAsistente[0]);

        public ContextoAsistente(string datosJson, IReadOnlyList<IReferenciaAsistente> referenciasPermitidas)
        {
            DatosJson = datosJson;
            ReferenciasPermitidas = referenciasPermitidas ?? new IReferenciaAsistente[0];
        }

        public string DatosJson { get; private set; }

        public IReadOnlyList<IReferenciaAsistente> ReferenciasPermitidas { get; private set; }
    }
}
