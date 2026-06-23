using System.Collections.Generic;
using SERVICIOS;

namespace REPOSITORIO
{
    public interface IIdiomaRepositorio
    {
        List<Idioma> ObtenerIdiomas(IUnidadDeTrabajo unidad = null);

        Idioma ObtenerIdiomaCompleto(int idiomaId, IUnidadDeTrabajo unidad = null);

        Idioma ObtenerIdiomaDefault(IUnidadDeTrabajo unidad = null);

        int InsertarIdioma(Idioma idioma, IUnidadDeTrabajo unidad = null);

        bool ActualizarIdioma(Idioma idioma, IUnidadDeTrabajo unidad = null);

        bool EliminarIdioma(int idiomaId, IUnidadDeTrabajo unidad = null);

        bool ConsolidarIdioma(int idiomaOrigenId, int idiomaDestinoId, IUnidadDeTrabajo unidad = null);

        int ObtenerPalabraIdPorKey(string key, IUnidadDeTrabajo unidad = null);

        bool GuardarTraduccion(int idiomaId, int palabraId, string valor, IUnidadDeTrabajo unidad = null);

        bool EliminarTraduccion(int idiomaId, int palabraId, IUnidadDeTrabajo unidad = null);

        bool SwitchearIdiomaDefault(int idiomaId, IUnidadDeTrabajo unidad = null);
    }
}
