using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;

namespace SERVICIOS
{
    public class Idioma : IIdioma   
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public bool Default { get; set; }   

        public List<Traduccion> Traducciones { get; set; }

        public Idioma()
        {
            Traducciones = new List<Traduccion>();
        }

        /// <summary>
        /// Metodo para obtener el texto traducido asociado a una key.
        /// Si la key no existe en el idioma, devuelve la misma key como fallback visible.
        /// </summary>
        /// <param name="key">La clave de traduccion solicitada.</param>
        /// <returns>El valor traducido o la key original cuando no hay traduccion.</returns>
        public string ObtenerTraduccion(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            ITraduccion traduccion = Traducciones.FirstOrDefault(item =>
                item.Palabra != null &&
                string.Equals(item.Palabra.Key, key, StringComparison.OrdinalIgnoreCase));

            return traduccion == null ? key : traduccion.Valor;
        }

        /// <summary>
        /// Metodo para verificar si el idioma contiene una traduccion util para una key.
        /// Solo considera valida una traduccion con valor no vacio.
        /// </summary>
        /// <param name="key">La clave de traduccion a buscar.</param>
        /// <returns><see langword="true" /> si existe una traduccion con valor; en caso contrario, <see langword="false" />.</returns>
        public bool TieneTraduccion(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            return Traducciones.Any(item =>
                item.Palabra != null &&
                string.Equals(item.Palabra.Key, key, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(item.Valor));
        }

        IList<ITraduccion> IIdioma.Traducciones
        {
            get { return Traducciones.Cast<ITraduccion>().ToList(); }
            set { Traducciones = value == null ? new List<Traduccion>() : value.OfType<Traduccion>().ToList(); }
        }
    }
}
