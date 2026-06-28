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

        public Idioma ObtenerIdiomaCompleto(int idiomaId)
        {
            return _idiomaDAL.ObtenerIdiomaCompleto(idiomaId, _accesoDatos);
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

        public bool EliminarIdioma(int idiomaId)
        {
            return _idiomaDAL.EliminarIdioma(idiomaId, _accesoDatos);
        }

        public bool ConsolidarIdioma(int idiomaOrigenId, int idiomaDestinoId)
        {
            return _idiomaDAL.ConsolidarIdioma(idiomaOrigenId, idiomaDestinoId, _accesoDatos);
        }

        public int ObtenerPalabraIdPorKey(string key)
        {
            return _idiomaDAL.ObtenerPalabraIdPorKey(key, _accesoDatos);
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor)
        {
            return _idiomaDAL.GuardarTraduccion(idiomaId, palabraId, valor, _accesoDatos);
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            return _idiomaDAL.EliminarTraduccion(idiomaId, palabraId, _accesoDatos);
        }

        public bool SwitchearIdiomaDefault(int idiomaId)
        {
            return _idiomaDAL.SwitchearIdiomaDefault(idiomaId, _accesoDatos);
        }
    }
}
