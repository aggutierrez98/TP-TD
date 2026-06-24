using System.Collections;

namespace ABSTRACCIONES
{
    // Contrato transversal del servicio de autorizacion: mantiene los permisos del
    // usuario actual y responde si tiene un permiso dado. Recibe los permisos como
    // IEnumerable para no acoplar el contrato al tipo concreto de permiso.
    public interface IAutorizacion
    {
        void ActualizarPermisosUsuarioActual(IEnumerable permisos);

        void Limpiar();

        bool TienePermiso(string rutaPermiso);
    }
}
