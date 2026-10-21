using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SERVICIOS
{
    public class IdiomaCatalogoService
    {
        private static readonly JsonSerializerOptions OpcionesExportacion = new JsonSerializerOptions
        {
            // Los catalogos se escriben en disco, conservando acentos literales.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

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
        public void ValidarNombreDuplicado(IEnumerable<Idioma> idiomas, string nombre, Idioma excluido)
        {
            bool existe = idiomas.Any(item =>
                (excluido == null || item.Id != excluido.Id) &&
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

        /// <summary>
        /// Metodo para leer y validar un archivo JSON de traducciones (clave/valor).
        /// </summary>
        /// <param name="rutaArchivo">La ruta del archivo JSON a leer.</param>
        /// <returns>El diccionario de traducciones leido, nunca vacio.</returns>
        /// <exception cref="ExcepcionNegocio">No hay ruta, el archivo no existe o no contiene traducciones.</exception>
        /// <exception cref="ExcepcionTecnica">El contenido no tiene un formato JSON valido.</exception>
        public Dictionary<string, string> LeerCatalogoJson(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                throw new ExcepcionNegocio("Error.Language.JsonFileNotFound");
            }

            string contenido = File.ReadAllText(rutaArchivo);
            Dictionary<string, string> traducciones;

            try
            {
                traducciones = JsonSerializer.Deserialize<Dictionary<string, string>>(contenido);
            }
            catch (JsonException ex)
            {
                throw new ExcepcionTecnica("Error.Language.JsonInvalidFormat", ex);
            }

            if (traducciones == null || traducciones.Count == 0)
            {
                throw new ExcepcionNegocio("Error.Language.JsonEmpty");
            }

            return traducciones;
        }

        /// <summary>
        /// Metodo para validar que se haya indicado una ruta de exportacion.
        /// </summary>
        /// <param name="rutaArchivo">La ruta de destino ingresada.</param>
        /// <exception cref="ExcepcionNegocio">La ruta esta vacia.</exception>
        public void ValidarRutaExportacion(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ExcepcionNegocio("Error.Language.ExportPathRequired");
            }
        }

        /// <summary>
        /// Metodo para armar el catalogo exportable de un idioma, ordenado por key de forma ordinal.
        /// Omite traducciones sin palabra o con key vacia y reemplaza valores nulos por cadena vacia.
        /// </summary>
        /// <param name="idioma">El idioma completo con sus traducciones.</param>
        /// <returns>El diccionario key/valor ordenado.</returns>
        public Dictionary<string, string> ConstruirCatalogoExportable(Idioma idioma)
        {
            return idioma.Traducciones
                .Where(item => item.Palabra != null && !string.IsNullOrWhiteSpace(item.Palabra.Key))
                .OrderBy(item => item.Palabra.Key)
                .ToDictionary(
                    item => item.Palabra.Key,
                    item => item.Valor ?? string.Empty,
                    System.StringComparer.Ordinal);
        }

        /// <summary>
        /// Metodo para serializar un catalogo de traducciones a JSON y escribirlo en disco como UTF-8 sin BOM.
        /// </summary>
        /// <param name="traducciones">El catalogo key/valor a escribir.</param>
        /// <param name="rutaArchivo">La ruta de destino.</param>
        /// <exception cref="ExcepcionNegocio">La ruta esta vacia.</exception>
        public void EscribirCatalogoJson(Dictionary<string, string> traducciones, string rutaArchivo)
        {
            ValidarRutaExportacion(rutaArchivo);

            string contenido = JsonSerializer.Serialize(traducciones, OpcionesExportacion);
            File.WriteAllText(rutaArchivo, contenido, new UTF8Encoding(false));
        }
    }
}
