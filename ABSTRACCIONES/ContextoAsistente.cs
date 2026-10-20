using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Datos ya filtrados del usuario actual que se entregan al modelo (JSON chico) y el
    // conjunto de referencias que la respuesta tiene permitido nombrar. Lo arma el BLL a
    // partir de la sesion; el modelo nunca recibe ni genera un identificador de usuario.
    public class ContextoAsistente
    {
        public static readonly ContextoAsistente Vacio = new ContextoAsistente(null, new ReferenciaAsistente[0]);

        public ContextoAsistente(string datosJson, IReadOnlyList<ReferenciaAsistente> referenciasPermitidas)
        {
            DatosJson = datosJson;
            ReferenciasPermitidas = referenciasPermitidas ?? new ReferenciaAsistente[0];
        }

        public string DatosJson { get; private set; }

        public IReadOnlyList<ReferenciaAsistente> ReferenciasPermitidas { get; private set; }
    }
}
