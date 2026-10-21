using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Datos ya filtrados del usuario actual que se entregan al modelo (JSON chico) y el
    // conjunto de referencias que la respuesta tiene permitido nombrar. Lo arma el BLL a
    // partir de la sesion; el modelo nunca recibe ni genera un identificador de usuario.
    public interface IContextoAsistente
    {
        string DatosJson { get; }

        IReadOnlyList<IReferenciaAsistente> ReferenciasPermitidas { get; }
    }
}
