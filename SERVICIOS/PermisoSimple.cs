using System.Collections.Generic;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class PermisoSimple : IPermiso
    {
        public PermisoSimple()
        {
            Hijos = new List<IPermiso>();
        }

        public int Id { get; set; }

        public string Nombre { get; set; }

        public bool EsGrupo
        {
            get { return false; }
        }

        public IList<IPermiso> Hijos { get; private set; }

        public void Agregar(IPermiso permiso)
        {
        }

        public void Quitar(IPermiso permiso)
        {
        }

        public bool Contiene(int id)
        {
            return Id == id;
        }

        public IList<IPermiso> ObtenerPermisosHoja()
        {
            return new List<IPermiso> { this };
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
