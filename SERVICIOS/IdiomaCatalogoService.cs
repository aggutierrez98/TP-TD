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
                throw new ExcepcionNegocio("Error.Language.NameRequired");
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
                throw new ExcepcionNegocio("Error.Language.NameAlreadyExists");
            }
        }

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
