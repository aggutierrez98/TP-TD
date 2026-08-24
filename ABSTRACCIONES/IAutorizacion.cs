using System.Collections.Generic;

namespace ABSTRACCIONES
{
    // Contrato transversal del servicio de autorizacion: mantiene los permisos del
    // usuario actual y responde si tiene un permiso dado. Los permisos se reciben
    // tipados como IPermiso, que es otro contrato de esta misma capa: el contrato
    // queda fuertemente tipado sin acoplarse a ninguna implementacion concreta.
    public interface IAutorizacion
    {
        void ActualizarPermisosUsuarioActual(IEnumerable<IPermiso> permisos);

        void Limpiar();

        bool TienePermiso(string rutaPermiso);
    }
}
