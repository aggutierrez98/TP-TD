using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public abstract class PermisoNodo : IPermiso
    {
        protected PermisoNodo()
        {
            Hijos = new List<IPermiso>();
        }

        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Ruta { get; set; }

        public abstract bool EsGrupo { get; }

        public IList<IPermiso> Hijos { get; private set; }


        public IList<PermisoNodo> HijosNodos
        {
            get { return Hijos.OfType<PermisoNodo>().ToList(); }
        }

        public virtual void Agregar(IPermiso permiso)
        {
            if (!EsGrupo)
            {
                throw new InvalidOperationException("Un permiso hoja no puede contener hijos.");
            }

            if (permiso == null || Hijos.Any(item => item.Id == permiso.Id && permiso.Id > 0))
            {
                return;
            }

            Hijos.Add(permiso);
        }

        public virtual void Quitar(IPermiso permiso)
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
            Hijos.Clear();
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

            if (!EsGrupo)
            {
                resultado.Add(this);
                return resultado;
            }

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

        public string ObtenerClave()
        {
            return Id > 0
                ? "ID:" + Id
                : "REF:" + RuntimeHelpers.GetHashCode(this);
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
