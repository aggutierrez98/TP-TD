using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class PermisoGrupo : IPermiso
    {
        public PermisoGrupo()
        {
            Hijos = new List<IPermiso>();
        }

        public int Id { get; set; }

        public string Nombre { get; set; }

        public bool EsGrupo
        {
            get { return true; }
        }

        public IList<IPermiso> Hijos { get; private set; }

        public void Agregar(IPermiso permiso)
        {
            if (permiso == null)
            {
                return;
            }

            if (Hijos.Any(item => ReferenceEquals(item, permiso)))
            {
                return;
            }

            string nombreNuevo = string.IsNullOrWhiteSpace(permiso.Nombre)
                ? string.Empty
                : permiso.Nombre.Trim().ToLowerInvariant();

            if (Hijos.Any(item =>
                (item.Id > 0 && item.Id == permiso.Id) ||
                string.Equals(
                    string.IsNullOrWhiteSpace(item.Nombre) ? string.Empty : item.Nombre.Trim().ToLowerInvariant(),
                    nombreNuevo,
                    System.StringComparison.Ordinal)))
            {
                return;
            }

            Hijos.Add(permiso);
        }

        public void Quitar(IPermiso permiso)
        {
            if (permiso == null)
            {
                return;
            }

            IPermiso existente = Hijos.FirstOrDefault(item => ReferenceEquals(item, permiso) || item.Id == permiso.Id);
            if (existente != null)
            {
                Hijos.Remove(existente);
            }
        }

        public bool Contiene(int id)
        {
            if (Id == id)
            {
                return true;
            }

            return Hijos.Any(item => item.Contiene(id));
        }

        public IList<IPermiso> ObtenerPermisosHoja()
        {
            List<IPermiso> resultado = new List<IPermiso>();

            foreach (IPermiso hijo in Hijos)
            {
                foreach (IPermiso permiso in hijo.ObtenerPermisosHoja())
                {
                    if (resultado.All(item => item.Id != permiso.Id || permiso.Id == 0))
                    {
                        resultado.Add(permiso);
                    }
                }
            }

            return resultado;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
