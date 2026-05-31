using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IPermiso
    {
        int Id { get; set; }

        string Nombre { get; set; }

        bool EsGrupo { get; }

        IList<IPermiso> Hijos { get; }

        void Agregar(IPermiso permiso);

        void Quitar(IPermiso permiso);

        bool Contiene(int id);

        IList<IPermiso> ObtenerPermisosHoja();
    }
}
