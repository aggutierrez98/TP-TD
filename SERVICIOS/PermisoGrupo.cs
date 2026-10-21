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

        /// <summary>
        /// Metodo para agregar un permiso hijo al grupo.
        /// Ignora valores nulos, referencias repetidas y duplicados por identificador o nombre.
        /// </summary>
        /// <param name="permiso">El permiso que se quiere agregar como hijo.</param>
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

        /// <summary>
        /// Metodo para quitar un permiso hijo del grupo.
        /// Busca por referencia o por identificador del permiso recibido.
        /// </summary>
        /// <param name="permiso">El permiso que se quiere quitar.</param>
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

        /// <summary>
        /// Metodo para indicar si el grupo contiene un permiso por identificador.
        /// Recorre recursivamente todos los hijos del grupo.
        /// </summary>
        /// <param name="id">El identificador del permiso buscado.</param>
        /// <returns><see langword="true" /> si el permiso esta contenido; en caso contrario, <see langword="false" />.</returns>
        public bool Contiene(IPermiso permiso)
        {
            if (permiso == null) return false;
            if (Id == permiso.Id)
            {
                return true;
            }

            return Hijos.Any(item => item.Contiene(permiso));
        }

        /// <summary>
        /// Metodo para obtener todos los permisos hoja contenidos por el grupo.
        /// Expande recursivamente los grupos hijos y evita duplicados por identificador.
        /// </summary>
        /// <returns>La lista de permisos hoja contenidos por el grupo.</returns>
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

        /// <summary>
        /// Metodo para representar el permiso con su nombre.
        /// Usado principalmente por controles visuales y depuracion.
        /// </summary>
        /// <returns>El nombre del permiso.</returns>
        public override string ToString()
        {
            return Nombre;
        }
    }
}
