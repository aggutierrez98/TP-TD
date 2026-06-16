using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public abstract class PermisoNodo : IPermiso
    {
        private readonly List<IPermiso> _hijos;

        protected PermisoNodo()
        {
            _hijos = new List<IPermiso>();
        }

        public int Id { get; set; }

        public string Nombre { get; set; }

        public abstract bool EsGrupo { get; }

        public IList<IPermiso> Hijos
        {
            get { return _hijos; }
        }

        public IList<PermisoNodo> HijosNodos
        {
            get { return _hijos.OfType<PermisoNodo>().ToList(); }
        }

        public virtual void Agregar(IPermiso permiso)
        {
            if (!EsGrupo)
            {
                return;
            }

            if (permiso == null)
            {
                return;
            }

            PermisoNodo permisoNodo = permiso as PermisoNodo;
            if (permisoNodo == null)
            {
                return;
            }

            if (_hijos.Any(item => ReferenceEquals(item, permiso)))
            {
                return;
            }

            string nombreNuevo = string.IsNullOrWhiteSpace(permisoNodo.Nombre)
                ? string.Empty
                : permisoNodo.Nombre.Trim().ToLowerInvariant();

            if (_hijos.OfType<PermisoNodo>().Any(item =>
                (item.Id > 0 && item.Id == permisoNodo.Id) ||
                string.Equals(
                    string.IsNullOrWhiteSpace(item.Nombre) ? string.Empty : item.Nombre.Trim().ToLowerInvariant(),
                    nombreNuevo,
                    System.StringComparison.Ordinal)))
            {
                return;
            }

            _hijos.Add(permiso);
        }

        public virtual void Quitar(IPermiso permiso)
        {
            if (permiso == null)
            {
                return;
            }

            IPermiso existente = _hijos.FirstOrDefault(item => ReferenceEquals(item, permiso) || item.Id == permiso.Id);
            if (existente != null)
            {
                _hijos.Remove(existente);
            }
        }

        public void AgregarHijo(PermisoNodo permiso)
        {
            Agregar(permiso);
        }

        public void QuitarHijo(PermisoNodo permiso)
        {
            Quitar(permiso);
        }

        public void VaciarHijos()
        {
            _hijos.Clear();
        }

        public bool Contiene(int id)
        {
            if (Id == id)
            {
                return true;
            }

            return _hijos.Any(item => item.Contiene(id));
        }

        public IList<IPermiso> ObtenerPermisosHoja()
        {
            List<IPermiso> resultado = new List<IPermiso>();

            if (!EsGrupo)
            {
                resultado.Add(this);
                return resultado;
            }

            foreach (IPermiso hijo in _hijos)
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
