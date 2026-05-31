using System.Collections.Generic;

namespace ABSTRACCIONES
{
    public interface IIdiomaRepository
    {
        IList<IIdioma> ObtenerIdiomas();

        IList<IPalabra> ObtenerPalabras();

        IIdioma ObtenerIdiomaCompleto(int idiomaId);

        IIdioma ObtenerIdiomaDefault();

        int InsertarIdioma(IIdioma idioma);

        bool ActualizarIdioma(IIdioma idioma);

        bool EliminarIdioma(int idiomaId);

        int ObtenerOCrearPalabra(string key);

        bool GuardarTraduccion(int idiomaId, int palabraId, string valor);

        bool EliminarTraduccion(int idiomaId, int palabraId);

        bool EstablecerIdiomaDefault(int idiomaId);
    }
}
