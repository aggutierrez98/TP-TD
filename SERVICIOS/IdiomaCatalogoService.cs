using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class IdiomaCatalogoService
    {
        public string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ExcepcionNegocio("Error.Idioma.NameRequired");
            }

            return nombre.Trim();
        }

        public void ValidarNombreDuplicado(IEnumerable<Idioma> idiomas, string nombre, int? idiomaId)
        {
            bool existe = idiomas.Any(item =>
                (!idiomaId.HasValue || item.Id != idiomaId.Value) &&
                string.Equals(item.Nombre, nombre, System.StringComparison.OrdinalIgnoreCase));

            if (existe)
            {
                throw new ExcepcionNegocio("Error.Idioma.NameAlreadyExists");
            }
        }

        public bool EstaCompleto(Idioma idioma)
        {
            if (idioma == null)
            {
                return false;
            }

            List<Traduccion> traduccionesBase = idioma.Traducciones
                .Where(item => item.Palabra != null && item.Palabra.EsBase)
                .ToList();

            return traduccionesBase.Count > 0 &&
                   traduccionesBase.All(item => !string.IsNullOrWhiteSpace(item.Valor));
        }

        public void ValidarIdiomaSeleccionable(Idioma idioma)
        {
            if (idioma == null)
            {
                throw new ExcepcionNegocio("Error.Idioma.NotFound");
            }

            if (!EstaCompleto(idioma))
            {
                throw new ExcepcionNegocio("Error.Idioma.IncompleteSelection");
            }
        }
    }
}
