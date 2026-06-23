using System.Collections.Generic;
using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class IdiomaRepositorio : IIdiomaRepositorio
    {
        private readonly IdiomaDAL _idiomaDAL;

        public IdiomaRepositorio()
        {
            _idiomaDAL = new IdiomaDAL();
        }

        public List<Idioma> ObtenerIdiomas(IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.ObtenerIdiomas(ObtenerAccesoDatos(unidad));
        }

        public Idioma ObtenerIdiomaCompleto(int idiomaId, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.ObtenerIdiomaCompleto(idiomaId, ObtenerAccesoDatos(unidad));
        }

        public Idioma ObtenerIdiomaDefault(IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.ObtenerIdiomaDefault(ObtenerAccesoDatos(unidad));
        }

        public int InsertarIdioma(Idioma idioma, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.InsertarIdioma(idioma, ObtenerAccesoDatos(unidad));
        }

        public bool ActualizarIdioma(Idioma idioma, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.ActualizarIdioma(idioma, ObtenerAccesoDatos(unidad));
        }

        public bool EliminarIdioma(int idiomaId, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.EliminarIdioma(idiomaId, ObtenerAccesoDatos(unidad));
        }

        public bool ConsolidarIdioma(int idiomaOrigenId, int idiomaDestinoId, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.ConsolidarIdioma(idiomaOrigenId, idiomaDestinoId, ObtenerAccesoDatos(unidad));
        }

        public int ObtenerPalabraIdPorKey(string key, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.ObtenerPalabraIdPorKey(key, ObtenerAccesoDatos(unidad));
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.GuardarTraduccion(idiomaId, palabraId, valor, ObtenerAccesoDatos(unidad));
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.EliminarTraduccion(idiomaId, palabraId, ObtenerAccesoDatos(unidad));
        }

        public bool SwitchearIdiomaDefault(int idiomaId, IUnidadDeTrabajo unidad = null)
        {
            return _idiomaDAL.SwitchearIdiomaDefault(idiomaId, ObtenerAccesoDatos(unidad));
        }

        private static AccesoDatosSqlServer ObtenerAccesoDatos(IUnidadDeTrabajo unidad)
        {
            return unidad == null ? null : ((UnidadDeTrabajoSqlServer)unidad).AccesoDatosSqlServer;
        }
    }
}
