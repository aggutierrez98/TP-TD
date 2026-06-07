using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IIdioma
    {
        int Id { get; set; }

        string Nombre { get; set; }

        bool Default { get; set; }

        IList<ITraduccion> Traducciones { get; set; }

        string ObtenerTraduccion(string key);

        bool TieneTraduccion(string key);
    }
}
