using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class PermisoService
    {
        public List<PermisoNodo> ConstruirArbol(IList<PermisoNodo> nodos, IList<PermisoRelacion> relaciones)
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

                indice[relacion.GrupoId].Agregar(indice[relacion.PermisoId]);
            }

            HashSet<int> hijos = new HashSet<int>(relaciones.Select(item => item.PermisoId));
            return nodos
                .Where(item => !hijos.Contains(item.Id))
                .OrderBy(item => item.Nombre, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public List<IPermiso> ObtenerPermisosEfectivos(IEnumerable<IPermiso> permisosAsignados)
        {
            List<IPermiso> permisos = new List<IPermiso>();
            AgregarPermisosHoja(permisosAsignados, permisos);
            return permisos;
        }

        public string NormalizarNombre(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre)
                ? string.Empty
                : nombre.Trim().ToLowerInvariant();
        }

        public void ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ExcepcionNegocio("Error.Permission.NameRequired");
            }

            if (nombre.Contains("."))
            {
                throw new ExcepcionNegocio("Error.Permission.NameContainsDot");
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
                throw new ExcepcionNegocio("Error.Permission.EmptyGroupsNotAllowed");
            }
        }

        public void ValidarSinCiclos(IEnumerable<PermisoNodo> raices)
        {
            foreach (PermisoNodo raiz in raices)
            {
                ValidarSinCiclos(raiz, new HashSet<int>());
            }
        }

        public List<PermisoNodo> Aplanar(IEnumerable<PermisoNodo> raices)
        {
            List<PermisoNodo> resultado = new List<PermisoNodo>();
            foreach (PermisoNodo raiz in raices)
            {
                Aplanar(raiz, resultado);
            }

            return resultado;
        }

        public List<PermisoRelacion> ObtenerRelaciones(IEnumerable<PermisoNodo> raices)
        {
            List<PermisoRelacion> relaciones = new List<PermisoRelacion>();
            foreach (PermisoNodo raiz in raices)
            {
                ObtenerRelaciones(raiz, relaciones);
            }

            return relaciones;
        }

        private void ValidarSinCiclos(PermisoNodo nodo, ISet<int> visitados)
        {
            if (nodo == null)
            {
                return;
            }

            if (nodo.Id > 0 && visitados.Contains(nodo.Id))
            {
                throw new ExcepcionNegocio("Error.Permission.CycleDetected");
            }

            HashSet<int> ramaActual = new HashSet<int>(visitados);
            if (nodo.Id > 0)
            {
                ramaActual.Add(nodo.Id);
            }

            foreach (PermisoNodo hijo in nodo.HijosNodos)
            {
                ValidarSinCiclos(hijo, ramaActual);
            }
        }

        private void Aplanar(PermisoNodo nodo, ICollection<PermisoNodo> resultado)
        {
            if (nodo == null)
            {
                return;
            }

            bool existe = nodo.Id > 0
                ? resultado.Any(item => item.Id == nodo.Id)
                : resultado.Any(item => object.ReferenceEquals(item, nodo));

            if (!existe)
            {
                resultado.Add(nodo);
            }

            foreach (PermisoNodo hijo in nodo.HijosNodos)
            {
                Aplanar(hijo, resultado);
            }
        }

        private void ObtenerRelaciones(PermisoNodo nodo, ICollection<PermisoRelacion> relaciones)
        {
            foreach (PermisoNodo hijo in nodo.HijosNodos)
            {
                bool existeRelacion = relaciones.Any(item => item.GrupoId == nodo.Id && item.PermisoId == hijo.Id);
                if (!existeRelacion)
                {
                    relaciones.Add(new PermisoRelacion
                    {
                        GrupoId = nodo.Id,
                        PermisoId = hijo.Id
                    });
                }

                ObtenerRelaciones(hijo, relaciones);
            }
        }

        private void ValidarNombresUnicosPorNivel(IEnumerable<PermisoNodo> hermanos)
        {
            List<PermisoNodo> lista = hermanos == null
                ? new List<PermisoNodo>()
                : hermanos.ToList();

            List<string> duplicados = lista
                .GroupBy(item => NormalizarNombre(item.Nombre), StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            if (duplicados.Count > 0)
            {
                throw new ExcepcionNegocio("Error.Permission.DuplicateNames");
            }

            foreach (PermisoNodo nodo in lista.Where(item => item.EsGrupo))
            {
                ValidarNombresUnicosPorNivel(nodo.HijosNodos);
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
                    bool existePorId = permisoHoja.Id > 0 && destino.Any(item => item.Id == permisoHoja.Id);
                    bool existePorNombre = destino
                        .OfType<PermisoNodo>()
                        .Any(item => string.Equals(NormalizarNombre(item.Nombre), NormalizarNombre(((PermisoNodo)permisoHoja).Nombre), StringComparison.Ordinal));

                    if (!existePorId && !existePorNombre)
                    {
                        destino.Add(permisoHoja);
                    }
                }
            }
        }

    }
}
