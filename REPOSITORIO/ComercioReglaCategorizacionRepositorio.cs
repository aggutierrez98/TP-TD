using System.Collections.Generic;
using DAL;

namespace REPOSITORIO
{
    public class ComercioReglaCategorizacionRepositorio : IComercioReglaCategorizacionRepositorio
    {
        private readonly ComercioReglaCategorizacionDAL _comercioReglaCategorizacionDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public ComercioReglaCategorizacionRepositorio()
            : this(null)
        {
        }

        internal ComercioReglaCategorizacionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _comercioReglaCategorizacionDAL = new ComercioReglaCategorizacionDAL();
            _accesoDatos = accesoDatos;
        }

        public List<int> ObtenerComercioIdsPorReglaCategorizacion(int reglaCategorizacionId)
        {
            return _comercioReglaCategorizacionDAL.ObtenerComercioIdsPorReglaCategorizacion(reglaCategorizacionId, _accesoDatos);
        }

        public bool ReemplazarComerciosDeReglaCategorizacion(int reglaCategorizacionId, List<int> comercioIds)
        {
            return _comercioReglaCategorizacionDAL.ReemplazarComerciosDeReglaCategorizacion(reglaCategorizacionId, comercioIds, _accesoDatos);
        }

        public int ContarPorComercio(int comercioId)
        {
            return _comercioReglaCategorizacionDAL.ContarPorComercio(comercioId, _accesoDatos);
        }
    }
}
