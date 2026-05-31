using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class PermisoService
    {
        public IList<PermisoNodo> ConstruirArbol(IList<PermisoNodo> nodos, IList<PermisoRelacion> relaciones)
        {
            Dictionary<int, PermisoNodo> indice = nodos.ToDictionary(item => item.Id);

            foreach (PermisoNodo nodo in nodos)
            {
                while (nodo.Hijos.Count > 0)
                {
                    nodo.Quitar(nodo.Hijos[0]);
                }
            }

            foreach (PermisoRelacion relacion in relaciones)
            {
                if (!indice.ContainsKey(relacion.GrupoId) || !indice.ContainsKey(relacion.PermisoId))
                {
                    continue;
                }

                indice[relacion.GrupoId].Agregar(indice[relacion.PermisoId]);
            }

            HashSet<int> hijos = new HashSet<int>(relaciones.Select(item => item.PermisoId));
            return nodos.Where(item => !hijos.Contains(item.Id)).ToList();
        }

        public IList<IPermiso> ObtenerPermisosEfectivos(IEnumerable<IPermiso> permisosAsignados)
        {
            List<IPermiso> permisos = new List<IPermiso>();
            AgregarPermisosHoja(permisosAsignados, permisos);
            return permisos;
        }

        public IList<IPermiso> ObtenerPermisosReconocidos(IEnumerable<IPermiso> permisosAsignados)
        {
            List<IPermiso> permisos = new List<IPermiso>();
            AgregarPermisosReconocidos(permisosAsignados, permisos);
            return permisos;
        }

        public void ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception("El nombre del permiso no puede estar vacio.");
            }
        }

        public void ValidarNombresUnicos(IEnumerable<PermisoNodo> nodos)
        {
            List<string> nombresDuplicados = nodos
                .GroupBy(item => item.Nombre, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (nombresDuplicados.Count > 0)
            {
                throw new Exception("No puede haber permisos o grupos con el mismo nombre.");
            }
        }

        public void ValidarGruposNoVacios(IEnumerable<PermisoNodo> nodos)
        {
            if (nodos.Any(item => item.EsGrupo && item.Hijos.Count == 0))
            {
                throw new Exception("No se pueden guardar grupos vacios.");
            }
        }

        public void ValidarSinCiclos(IEnumerable<PermisoNodo> raices)
        {
            foreach (PermisoNodo raiz in raices)
            {
                ValidarSinCiclos(raiz, new HashSet<string>());
            }
        }

        public IList<PermisoNodo> Aplanar(IEnumerable<PermisoNodo> raices)
        {
            List<PermisoNodo> resultado = new List<PermisoNodo>();
            foreach (PermisoNodo raiz in raices)
            {
                Aplanar(raiz, resultado);
            }

            return resultado;
        }

        public IList<PermisoRelacion> ObtenerRelaciones(IEnumerable<PermisoNodo> raices)
        {
            List<PermisoRelacion> relaciones = new List<PermisoRelacion>();
            foreach (PermisoNodo raiz in raices)
            {
                ObtenerRelaciones(raiz, relaciones);
            }

            return relaciones;
        }

        public IList<int> ObtenerIdsSubarbol(IPermiso raiz)
        {
            List<int> ids = new List<int>();
            if (raiz == null)
            {
                return ids;
            }

            ids.Add(raiz.Id);
            foreach (IPermiso hijo in raiz.Hijos)
            {
                ids.AddRange(ObtenerIdsSubarbol(hijo));
            }

            return ids.Distinct().ToList();
        }

        public object CrearColeccionPermisos(IEnumerable<PermisoNodo> permisos)
        {
            return permisos == null
                ? new List<IPermiso>()
                : permisos.Cast<IPermiso>().ToList();
        }

        private void ValidarSinCiclos(IPermiso permiso, HashSet<string> visitados)
        {
            if (permiso == null)
            {
                return;
            }

            string clave = ObtenerClaveNodo(permiso);
            if (visitados.Contains(clave))
            {
                throw new Exception("No se pueden generar ciclos en el arbol de permisos.");
            }

            HashSet<string> ramaActual = new HashSet<string>(visitados) { clave };
            foreach (IPermiso hijo in permiso.Hijos)
            {
                ValidarSinCiclos(hijo, ramaActual);
            }
        }

        private void Aplanar(PermisoNodo nodo, ICollection<PermisoNodo> resultado)
        {
            if (resultado.All(item => ObtenerClaveNodo(item) != ObtenerClaveNodo(nodo)))
            {
                resultado.Add(nodo);
            }

            foreach (PermisoNodo hijo in nodo.Hijos.OfType<PermisoNodo>())
            {
                Aplanar(hijo, resultado);
            }
        }

        private void ObtenerRelaciones(PermisoNodo nodo, ICollection<PermisoRelacion> relaciones)
        {
            foreach (PermisoNodo hijo in nodo.Hijos.OfType<PermisoNodo>())
            {
                relaciones.Add(new PermisoRelacion
                {
                    GrupoId = nodo.Id,
                    PermisoId = hijo.Id
                });

                ObtenerRelaciones(hijo, relaciones);
            }
        }

        private void AgregarPermisosHoja(IEnumerable<IPermiso> origen, ICollection<IPermiso> destino)
        {
            if (origen == null)
            {
                return;
            }

            foreach (IPermiso permiso in origen)
            {
                if (permiso == null)
                {
                    continue;
                }

                foreach (IPermiso permisoHoja in permiso.ObtenerPermisosHoja())
                {
                    if (destino.All(item => item.Id != permisoHoja.Id))
                    {
                        destino.Add(permisoHoja);
                    }
                }
            }
        }

        private void AgregarPermisosReconocidos(IEnumerable<IPermiso> origen, ICollection<IPermiso> destino)
        {
            if (origen == null)
            {
                return;
            }

            foreach (IPermiso permiso in origen)
            {
                if (permiso == null)
                {
                    continue;
                }

                if (destino.All(item => item.Id != permiso.Id))
                {
                    destino.Add(permiso);
                }

                AgregarPermisosReconocidos(permiso.Hijos, destino);
            }
        }

        private string ObtenerClaveNodo(IPermiso permiso)
        {
            if (permiso == null)
            {
                return string.Empty;
            }

            return permiso.Id > 0
                ? "ID:" + permiso.Id
                : "REF:" + RuntimeHelpers.GetHashCode(permiso);
        }
    }
}
