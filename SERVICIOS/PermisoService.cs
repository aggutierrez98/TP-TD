using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class PermisoService
    {

        // Metodo para obtener arbol mostrable en el treeView
        public IList<PermisoNodo> ConstruirArbol(IList<PermisoNodo> nodos, IList<PermisoRelacion> relaciones)
        {
            Dictionary<int, PermisoNodo> indice = nodos.ToDictionary(item => item.Id);

            foreach (PermisoNodo nodo in nodos)
            {
                nodo.VaciarHijos();
            }

            foreach (PermisoRelacion relacion in relaciones)
            {
                if (!indice.ContainsKey(relacion.GrupoId) || !indice.ContainsKey(relacion.PermisoId))
                {
                    continue;
                }

                // Reconstruye relaciones padre-hijo entre los nodos
                indice[relacion.GrupoId].Agregar(indice[relacion.PermisoId]);
            }

            // Determinar cuales son nodos raiz (que no son hijos de nadie)
            HashSet<int> hijos = new HashSet<int>(relaciones.Select(item => item.PermisoId));
            IList<PermisoNodo> raices = nodos.Where(item => !hijos.Contains(item.Id)).ToList();

            AsignarRutas(raices, string.Empty);
            return raices;
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
                throw new ExcepcionNegocio("Error.Permiso.NameRequired");
            }

            if (nombre.Contains("."))
            {
                throw new ExcepcionNegocio("Error.Permiso.NameContainsDot");
            }
        }

        public void ValidarNombresUnicos(IEnumerable<PermisoNodo> raices)
        {
            ValidarNombresUnicosPorNivel(raices);
        }

        public void ValidarGruposNoVacios(IEnumerable<PermisoNodo> nodos)
        {
            if (nodos.Any(item => item.EsGrupo && item.Hijos.Count == 0))
            {
                throw new ExcepcionNegocio("Error.Permiso.EmptyGroupsNotAllowed");
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

        // Convierte nodos a la interfaz base para asignacion cross-layer via reflexion
        public object NodosComoPermisos(IList<PermisoNodo> nodos)
        {
            return nodos == null
                ? new List<IPermiso>()
                : (object)nodos.Cast<IPermiso>().ToList();
        }

        private void AsignarRutas(IEnumerable<PermisoNodo> nodos, string prefijo)
        {
            foreach (PermisoNodo nodo in nodos)
            {
                nodo.Ruta = string.IsNullOrEmpty(prefijo) ? nodo.Nombre : prefijo + "." + nodo.Nombre;
                AsignarRutas(nodo.HijosNodos, nodo.Ruta);
            }
        }

        private void ValidarNombresUnicosPorNivel(IEnumerable<PermisoNodo> hermanos)
        {
            List<PermisoNodo> lista = hermanos.ToList();

            List<string> duplicados = lista
                .GroupBy(item => item.Nombre, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicados.Count > 0)
            {
                throw new ExcepcionNegocio("Error.Permiso.DuplicateNames");
            }

            foreach (PermisoNodo nodo in lista.Where(item => item.EsGrupo))
            {
                ValidarNombresUnicosPorNivel(nodo.HijosNodos);
            }
        }

        private void ValidarSinCiclos(PermisoNodo nodo, HashSet<string> visitados)
        {
            string clave = nodo.ObtenerClave();
            if (visitados.Contains(clave))
            {
                throw new ExcepcionNegocio("Error.Permiso.CycleDetected");
            }

            HashSet<string> ramaActual = new HashSet<string>(visitados) { clave };
            foreach (PermisoNodo hijo in nodo.Hijos.OfType<PermisoNodo>())
            {
                ValidarSinCiclos(hijo, ramaActual);
            }
        }

        private void Aplanar(PermisoNodo nodo, ICollection<PermisoNodo> resultado)
        {
            if (resultado.All(item => item.ObtenerClave() != nodo.ObtenerClave()))
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

        // Devolver solo las hojas, es decir, los permisos que no son grupos
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

        // Devolver todos los permisos reconocidos, incluyendo grupos y hojas
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
    }
}
