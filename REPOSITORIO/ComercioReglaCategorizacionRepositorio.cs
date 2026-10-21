using System.Collections.Generic;
using System.Linq;
using BE;
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

        public List<Comercio> ObtenerComerciosPorReglaCategorizacion(ReglaCategorizacion reglaCategorizacion)
        {
            List<Comercio> comercios = _comercioReglaCategorizacionDAL.ObtenerComerciosPorReglaCategorizacion(reglaCategorizacion, _accesoDatos);
            ComercioRepositorio comercioRepositorio = new ComercioRepositorio(_accesoDatos);
            return comercios.Select(item => comercioRepositorio.ObtenerPorId(item.Id)).ToList();
        }

        public bool ReemplazarComerciosDeReglaCategorizacion(ReglaCategorizacion reglaCategorizacion, IEnumerable<Comercio> comercios)
        {
            return _comercioReglaCategorizacionDAL.ReemplazarComerciosDeReglaCategorizacion(reglaCategorizacion, comercios, _accesoDatos);
        }

        public int ContarPorComercio(Comercio comercio)
        {
            return _comercioReglaCategorizacionDAL.ContarPorComercio(comercio, _accesoDatos);
        }
    }
}
