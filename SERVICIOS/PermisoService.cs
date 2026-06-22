using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class PermisoService
    {
        public List<object> ConstruirArbol(IList nodos, IList<Tuple<int, int>> relaciones)
        {
            IList<IPermiso> nodosTipados = ConvertirLista(nodos);
            return ConstruirArbol(nodosTipados, relaciones)
                .Cast<object>()
                .ToList();
        }

        public void AgregarHijo(object padre, object hijo)
        {
            Convertir(padre).Agregar(Convertir(hijo));
        }

        public void QuitarHijo(object padre, object hijo)
        {
            Convertir(padre).Quitar(Convertir(hijo));
        }

        public bool EsGrupo(object permiso)
        {
            return Convertir(permiso).EsGrupo;
        }

        public int ObtenerId(object permiso)
        {
            return Convertir(permiso).Id;
        }

        public void EstablecerId(object permiso, int id)
        {
            Convertir(permiso).Id = id;
        }

        public string ObtenerNombre(object permiso)
        {
            return Convertir(permiso).Nombre;
        }

        public void EstablecerNombre(object permiso, string nombre)
        {
            Convertir(permiso).Nombre = nombre;
        }

        public List<object> ObtenerHijos(object permiso)
        {
            return Convertir(permiso).Hijos
                .Cast<object>()
                .ToList();
        }

        public List<object> FiltrarFamilias(IEnumerable nodos)
        {
            return Aplanar(nodos)
                .Where(item => EsGrupo(item))
                .ToList();
        }

        public List<object> FiltrarPatentes(IEnumerable nodos)
        {
            return Aplanar(nodos)
                .Where(item => !EsGrupo(item))
                .ToList();
        }

        internal List<IPermiso> ConstruirArbol(IList<IPermiso> nodos, IList<Tuple<int, int>> relaciones)
        {
            Dictionary<int, IPermiso> indice = nodos.ToDictionary(item => item.Id);

            foreach (IPermiso nodo in nodos.Where(item => item.EsGrupo))
            {
                nodo.Hijos.Clear();
            }

            foreach (Tuple<int, int> relacion in relaciones)
            {
                if (!indice.ContainsKey(relacion.Item1) || !indice.ContainsKey(relacion.Item2))
                {
                    continue;
                }

                indice[relacion.Item1].Agregar(indice[relacion.Item2]);
            }

            HashSet<int> hijos = new HashSet<int>(relaciones.Select(item => item.Item2));
            return nodos
                .Where(item => !hijos.Contains(item.Id))
                .OrderBy(item => item.Nombre, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        internal List<IPermiso> ObtenerPermisosEfectivos(IEnumerable<IPermiso> permisosAsignados)
        {
            List<IPermiso> permisos = new List<IPermiso>();
            AgregarPermisosHoja(permisosAsignados, permisos);
            return permisos;
        }

        public List<object> ObtenerPermisosEfectivos(IEnumerable permisosAsignados)
        {
            IEnumerable<IPermiso> permisosTipados = ConvertirLista(permisosAsignados);
            return ObtenerPermisosEfectivos(permisosTipados)
                .Cast<object>()
                .ToList();
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

        internal void ValidarNombresUnicos(IEnumerable<IPermiso> raices)
        {
            ValidarNombresUnicosPorNivel(raices);
        }

        public void ValidarNombresUnicos(IEnumerable raices)
        {
            IEnumerable<IPermiso> permisosTipados = ConvertirLista(raices);
            ValidarNombresUnicos(permisosTipados);
        }

        internal void ValidarGruposNoVacios(IEnumerable<IPermiso> nodos)
        {
            if (nodos.Any(item => item.EsGrupo && item.Hijos.Count == 0))
            {
                throw new ExcepcionNegocio("Error.Permission.EmptyGroupsNotAllowed");
            }
        }

        public void ValidarGruposNoVacios(IEnumerable nodos)
        {
            IEnumerable<IPermiso> permisosTipados = ConvertirLista(nodos);
            ValidarGruposNoVacios(permisosTipados);
        }

        internal void ValidarSinCiclos(IEnumerable<IPermiso> raices)
        {
            foreach (IPermiso raiz in raices)
            {
                ValidarSinCiclos(raiz, new HashSet<int>());
            }
        }

        public void ValidarSinCiclos(IEnumerable raices)
        {
            IEnumerable<IPermiso> permisosTipados = ConvertirLista(raices);
            ValidarSinCiclos(permisosTipados);
        }

        internal List<IPermiso> Aplanar(IEnumerable<IPermiso> raices)
        {
            List<IPermiso> resultado = new List<IPermiso>();
            foreach (IPermiso raiz in raices)
            {
                Aplanar(raiz, resultado);
            }

            return resultado;
        }

        public List<object> Aplanar(IEnumerable raices)
        {
            IEnumerable<IPermiso> permisosTipados = ConvertirLista(raices);
            return Aplanar(permisosTipados)
                .Cast<object>()
                .ToList();
        }

        internal List<Tuple<int, int>> ObtenerRelaciones(IEnumerable<IPermiso> raices)
        {
            List<Tuple<int, int>> relaciones = new List<Tuple<int, int>>();
            foreach (IPermiso raiz in raices)
            {
                ObtenerRelaciones(raiz, relaciones);
            }

            return relaciones;
        }

        public List<Tuple<int, int>> ObtenerRelaciones(IEnumerable raices)
        {
            IEnumerable<IPermiso> permisosTipados = ConvertirLista(raices);
            return ObtenerRelaciones(permisosTipados);
        }

        private List<IPermiso> ConvertirLista(IEnumerable origen)
        {
            List<IPermiso> permisos = new List<IPermiso>();
            if (origen == null)
            {
                return permisos;
            }

            foreach (object item in origen)
            {
                permisos.Add(Convertir(item));
            }

            return permisos;
        }

        private IPermiso Convertir(object permiso)
        {
            if (permiso is IPermiso permisoTipado)
            {
                return permisoTipado;
            }

            if (permiso is PermisoGrupo grupo)
            {
                return grupo;
            }

            if (permiso is PermisoSimple simple)
            {
                return simple;
            }

            throw new InvalidOperationException("Tipo de permiso no soportado.");
        }

        private void ValidarSinCiclos(IPermiso nodo, ISet<int> visitados)
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

            foreach (IPermiso hijo in nodo.Hijos)
            {
                ValidarSinCiclos(hijo, ramaActual);
            }
        }

        private void Aplanar(IPermiso nodo, ICollection<IPermiso> resultado)
        {
            if (nodo == null)
            {
                return;
            }

            bool existe = nodo.Id > 0
                ? resultado.Any(item => item.Id == nodo.Id)
                : resultado.Any(item => ReferenceEquals(item, nodo));

            if (!existe)
            {
                resultado.Add(nodo);
            }

            foreach (IPermiso hijo in nodo.Hijos)
            {
                Aplanar(hijo, resultado);
            }
        }

        private void ObtenerRelaciones(IPermiso nodo, ICollection<Tuple<int, int>> relaciones)
        {
            foreach (IPermiso hijo in nodo.Hijos)
            {
                bool existeRelacion = relaciones.Any(item => item.Item1 == nodo.Id && item.Item2 == hijo.Id);
                if (!existeRelacion)
                {
                    relaciones.Add(Tuple.Create(nodo.Id, hijo.Id));
                }

                ObtenerRelaciones(hijo, relaciones);
            }
        }

        private void ValidarNombresUnicosPorNivel(IEnumerable<IPermiso> hermanos)
        {
            List<IPermiso> lista = hermanos == null
                ? new List<IPermiso>()
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

            foreach (IPermiso nodo in lista.Where(item => item.EsGrupo))
            {
                ValidarNombresUnicosPorNivel(nodo.Hijos);
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
                    bool existePorNombre = destino.Any(item => string.Equals(NormalizarNombre(item.Nombre), NormalizarNombre(permisoHoja.Nombre), StringComparison.Ordinal));

                    if (!existePorId && !existePorNombre)
                    {
                        destino.Add(permisoHoja);
                    }
                }
            }
        }
    }
}
