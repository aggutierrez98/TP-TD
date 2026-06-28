using System.Collections.Generic;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IIdiomaRepositorio
    {
        List<Idioma> ObtenerIdiomas();

        Idioma ObtenerIdiomaCompleto(int idiomaId);

        Idioma ObtenerIdiomaDefault();

        int InsertarIdioma(Idioma idioma);

        bool ActualizarIdioma(Idioma idioma);

        bool EliminarIdioma(int idiomaId);

        bool ConsolidarIdioma(int idiomaOrigenId, int idiomaDestinoId);

        int ObtenerPalabraIdPorKey(string key);

        bool GuardarTraduccion(int idiomaId, int palabraId, string valor);

        bool EliminarTraduccion(int idiomaId, int palabraId);

        bool SwitchearIdiomaDefault(int idiomaId);
    }
}
