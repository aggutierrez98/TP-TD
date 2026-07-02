using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class IdiomaCatalogoService
    {
        /// <summary>
        /// Metodo para normalizar el nombre de un idioma antes de validarlo o persistirlo.
        /// Elimina espacios sobrantes y rechaza nombres vacios.
        /// </summary>
        /// <param name="nombre">El nombre ingresado para el idioma.</param>
        /// <returns>El nombre normalizado.</returns>
        /// <exception cref="ExcepcionNegocio">El nombre esta vacio o contiene solo espacios.</exception>
        public string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ExcepcionNegocio("Error.Language.NameRequired");
            }

            return nombre.Trim();
        }

        /// <summary>
        /// Metodo para validar que no exista otro idioma con el mismo nombre.
        /// Permite excluir el idioma actual durante una edicion.
        /// </summary>
        /// <param name="idiomas">La coleccion de idiomas existentes.</param>
        /// <param name="nombre">El nombre normalizado que se quiere validar.</param>
        /// <param name="idiomaId">El identificador del idioma en edicion, si corresponde.</param>
        /// <exception cref="ExcepcionNegocio">Ya existe otro idioma con el mismo nombre.</exception>
        public void ValidarNombreDuplicado(IEnumerable<Idioma> idiomas, string nombre, int? idiomaId)
        {
            bool existe = idiomas.Any(item =>
                (!idiomaId.HasValue || item.Id != idiomaId.Value) &&
                string.Equals(item.Nombre, nombre, System.StringComparison.OrdinalIgnoreCase));

            if (existe)
            {
                throw new ExcepcionNegocio("Error.Language.NameAlreadyExists");
            }
        }

        /// <summary>
        /// Metodo para determinar si un idioma tiene todas sus traducciones cargadas.
        /// Usado para evitar seleccionar idiomas incompletos en la interfaz.
        /// </summary>
        /// <param name="idioma">El idioma a evaluar.</param>
        /// <returns><see langword="true" /> si todas las traducciones tienen valor; en caso contrario, <see langword="false" />.</returns>
        public bool EstaCompleto(Idioma idioma)
        {
            if (idioma == null)
            {
                return false;
            }

            // Un idioma esta completo si tiene valor en TODAS las keys del sistema
            List<Traduccion> traducciones = idioma.Traducciones
                .Where(item => item.Palabra != null)
                .ToList();

            return traducciones.Count > 0 &&
                   traducciones.All(item => !string.IsNullOrWhiteSpace(item.Valor));
        }

        /// <summary>
        /// Metodo para validar que un idioma pueda ser seleccionado como idioma activo.
        /// Requiere que el idioma exista y que su catalogo de traducciones este completo.
        /// </summary>
        /// <param name="idioma">El idioma candidato a seleccionar.</param>
        /// <exception cref="ExcepcionNegocio">El idioma no existe o esta incompleto.</exception>
        public void ValidarIdiomaSeleccionable(Idioma idioma)
        {
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Language.NotFound");
            }

            if (!EstaCompleto(idioma))
            {
                throw new ExcepcionNegocio("Error.Language.IncompleteSelection");
            }
        }
    }
}
