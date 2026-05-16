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

        public string ObtenerTexto(string key)
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
