using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    // Servicio de dominio del Composite de permisos. Trabaja siempre con IPermiso
    // (las capas consumidoras tipan fuerte, sin object/dynamic).
    public class PermisoService
    {
        /// <summary>
        /// Metodo para construir un arbol de permisos a partir de nodos y relaciones padre-hijo.
        /// Limpia los hijos de las familias y devuelve solo las raices ordenadas por nombre.
        /// </summary>
        /// <param name="nodos">La coleccion completa de permisos disponibles.</param>
        /// <param name="relaciones">La coleccion de relaciones entre permiso padre e hijo.</param>
        /// <returns>La lista de permisos raiz del arbol construido.</returns>
        public List<IPermiso> ConstruirArbol(IList<IPermiso> nodos, IList<Tuple<int, int>> relaciones)
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

        /// <summary>
        /// Metodo para obtener las patentes efectivas a partir de los permisos asignados.
        /// Expande las familias y elimina duplicados por identificador o nombre normalizado.
        /// </summary>
        /// <param name="permisosAsignados">La coleccion de permisos asignados al usuario o perfil.</param>
        /// <returns>La lista de permisos hoja efectivos.</returns>
        public List<IPermiso> ObtenerPermisosEfectivos(IEnumerable<IPermiso> permisosAsignados)
        {
            List<IPermiso> permisos = new List<IPermiso>();
            AgregarPermisosHoja(permisosAsignados, permisos);
            return permisos;
        }

        /// <summary>
        /// Metodo para filtrar solo las familias dentro de un conjunto de permisos.
        /// Aplana previamente el arbol para incluir familias en cualquier nivel.
        /// </summary>
        /// <param name="nodos">La coleccion de permisos raiz o parciales.</param>
        /// <returns>La lista de permisos que representan grupos.</returns>
        public List<IPermiso> FiltrarFamilias(IEnumerable<IPermiso> nodos)
        {
            return Aplanar(nodos).Where(item => item.EsGrupo).ToList();
        }

        /// <summary>
        /// Metodo para filtrar solo las patentes dentro de un conjunto de permisos.
        /// Aplana previamente el arbol para incluir patentes en cualquier nivel.
        /// </summary>
        /// <param name="nodos">La coleccion de permisos raiz o parciales.</param>
        /// <returns>La lista de permisos que no representan grupos.</returns>
        public List<IPermiso> FiltrarPatentes(IEnumerable<IPermiso> nodos)
        {
            return Aplanar(nodos).Where(item => !item.EsGrupo).ToList();
        }

        /// <summary>
        /// Metodo para normalizar nombres de permisos antes de compararlos.
        /// Convierte valores nulos o vacios en cadena vacia y aplica trim con minusculas invariantes.
        /// </summary>
        /// <param name="nombre">El nombre del permiso a normalizar.</param>
        /// <returns>El nombre normalizado.</returns>
        public string NormalizarNombre(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre)
                ? string.Empty
                : nombre.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Metodo para validar el nombre de un permiso.
        /// Rechaza nombres vacios y nombres que contienen puntos.
        /// </summary>
        /// <param name="nombre">El nombre de permiso a validar.</param>
        /// <exception cref="ExcepcionNegocio">El nombre esta vacio o contiene un punto.</exception>
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

        /// <summary>
        /// Metodo para validar que no existan nombres duplicados entre permisos hermanos.
        /// La validacion se aplica recursivamente por cada nivel del arbol.
        /// </summary>
        /// <param name="raices">La coleccion de permisos raiz a validar.</param>
        /// <exception cref="ExcepcionNegocio">Existen permisos hermanos con nombres duplicados.</exception>
        public void ValidarNombresUnicos(IEnumerable<IPermiso> raices)
        {
            ValidarNombresUnicosPorNivel(raices);
        }

        /// <summary>
        /// Metodo para validar que todas las familias tengan al menos un hijo.
        /// Usado para evitar perfiles o grupos vacios.
        /// </summary>
        /// <param name="nodos">La coleccion de permisos a validar.</param>
        /// <exception cref="ExcepcionNegocio">Existe una familia sin permisos hijos.</exception>
        public void ValidarGruposNoVacios(IEnumerable<IPermiso> nodos)
        {
            if (nodos.Any(item => item.EsGrupo && item.Hijos.Count == 0))
            {
                throw new ExcepcionNegocio("Error.Permission.EmptyGroupsNotAllowed");
            }
        }

        /// <summary>
        /// Metodo para validar que el arbol de permisos no contenga ciclos.
        /// Recorre cada raiz manteniendo la rama visitada.
        /// </summary>
        /// <param name="raices">La coleccion de permisos raiz a validar.</param>
        /// <exception cref="ExcepcionNegocio">Existe un ciclo dentro del arbol de permisos.</exception>
        public void ValidarSinCiclos(IEnumerable<IPermiso> raices)
        {
            foreach (IPermiso raiz in raices)
            {
                ValidarSinCiclos(raiz, new HashSet<int>());
            }
        }

        /// <summary>
        /// Metodo para aplanar un arbol de permisos.
        /// Evita duplicados por identificador cuando el permiso ya esta persistido.
        /// </summary>
        /// <param name="raices">La coleccion de permisos raiz a recorrer.</param>
        /// <returns>La lista plana de permisos encontrados.</returns>
        public List<IPermiso> Aplanar(IEnumerable<IPermiso> raices)
        {
            List<IPermiso> resultado = new List<IPermiso>();
            foreach (IPermiso raiz in raices)
            {
                Aplanar(raiz, resultado);
            }

            return resultado;
        }

        /// <summary>
        /// Metodo para obtener las relaciones padre-hijo desde un arbol de permisos.
        /// Evita duplicar la misma relacion durante el recorrido recursivo.
        /// </summary>
        /// <param name="raices">La coleccion de permisos raiz a recorrer.</param>
        /// <returns>La lista de tuplas con identificador de padre e identificador de hijo.</returns>
        public List<Tuple<int, int>> ObtenerRelaciones(IEnumerable<IPermiso> raices)
        {
            List<Tuple<int, int>> relaciones = new List<Tuple<int, int>>();
            foreach (IPermiso raiz in raices)
            {
                ObtenerRelaciones(raiz, relaciones);
            }

            return relaciones;
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
