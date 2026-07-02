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

        /// <summary>
        /// Metodo sin efecto para mantener el contrato Composite en una patente.
        /// Las patentes no admiten permisos hijos.
        /// </summary>
        /// <param name="permiso">El permiso recibido por compatibilidad con el contrato.</param>
        public void Agregar(IPermiso permiso)
        {
        }

        /// <summary>
        /// Metodo sin efecto para mantener el contrato Composite en una patente.
        /// Las patentes no tienen permisos hijos que quitar.
        /// </summary>
        /// <param name="permiso">El permiso recibido por compatibilidad con el contrato.</param>
        public void Quitar(IPermiso permiso)
        {
        }

        /// <summary>
        /// Metodo para indicar si la patente coincide con un identificador.
        /// </summary>
        /// <param name="id">El identificador del permiso buscado.</param>
        /// <returns><see langword="true" /> si el identificador coincide; en caso contrario, <see langword="false" />.</returns>
        public bool Contiene(int id)
        {
            return Id == id;
        }

        /// <summary>
        /// Metodo para obtener los permisos hoja de una patente.
        /// Una patente se devuelve a si misma porque ya es un permiso hoja.
        /// </summary>
        /// <returns>Una lista con la patente actual.</returns>
        public IList<IPermiso> ObtenerPermisosHoja()
        {
            return new List<IPermiso> { this };
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
