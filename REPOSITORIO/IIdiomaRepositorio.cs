using System.Collections.Generic;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IIdiomaRepositorio
    {
        List<Idioma> ObtenerIdiomas();

        Idioma ObtenerPorId(int id);

        Idioma ObtenerIdiomaDefault();

        int InsertarIdioma(Idioma idioma);

        bool ActualizarIdioma(Idioma idioma);

        bool EliminarIdioma(Idioma idioma);

        bool ConsolidarIdioma(Idioma origen, Idioma destino);

        Palabra ObtenerPalabraPorKey(string key);

        bool GuardarTraduccion(Traduccion traduccion);

        bool EliminarTraduccion(Traduccion traduccion);

        bool SwitchearIdiomaDefault(Idioma idioma);
    }
}
