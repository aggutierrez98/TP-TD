using System.Collections.Generic;
using DAL;
using SERVICIOS;

namespace REPOSITORIO
{
    public class IdiomaRepositorio : IIdiomaRepositorio
    {
        private readonly IdiomaDAL _idiomaDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public IdiomaRepositorio()
            : this(null)
        {
        }

        internal IdiomaRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _idiomaDAL = new IdiomaDAL();
            _accesoDatos = accesoDatos;
        }

        public List<Idioma> ObtenerIdiomas()
        {
            return _idiomaDAL.ObtenerIdiomas(_accesoDatos);
        }

        public Idioma ObtenerPorId(int id)
        {
            return _idiomaDAL.ObtenerPorId(id, _accesoDatos);
        }

        public Idioma ObtenerIdiomaDefault()
        {
            return _idiomaDAL.ObtenerIdiomaDefault(_accesoDatos);
        }

        public int InsertarIdioma(Idioma idioma)
        {
            return _idiomaDAL.InsertarIdioma(idioma, _accesoDatos);
        }

        public bool ActualizarIdioma(Idioma idioma)
        {
            return _idiomaDAL.ActualizarIdioma(idioma, _accesoDatos);
        }

        public bool EliminarIdioma(Idioma idioma)
        {
            return _idiomaDAL.EliminarIdioma(idioma, _accesoDatos);
        }

        public bool ConsolidarIdioma(Idioma origen, Idioma destino)
        {
            return _idiomaDAL.ConsolidarIdioma(origen, destino, _accesoDatos);
        }

        public Palabra ObtenerPalabraPorKey(string key)
        {
            return _idiomaDAL.ObtenerPalabraPorKey(key, _accesoDatos);
        }

        public bool GuardarTraduccion(Traduccion traduccion)
        {
            return _idiomaDAL.GuardarTraduccion(traduccion, _accesoDatos);
        }

        public bool EliminarTraduccion(Traduccion traduccion)
        {
            return _idiomaDAL.EliminarTraduccion(traduccion, _accesoDatos);
        }

        public bool SwitchearIdiomaDefault(Idioma idioma)
        {
            return _idiomaDAL.SwitchearIdiomaDefault(idioma, _accesoDatos);
        }
    }
}
