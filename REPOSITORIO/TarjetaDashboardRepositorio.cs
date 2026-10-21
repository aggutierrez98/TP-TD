using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class TarjetaDashboardRepositorio : ITarjetaDashboardRepositorio
    {
        private readonly TarjetaDashboardDAL _tarjetaDashboardDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public TarjetaDashboardRepositorio()
            : this(null)
        {
        }

        internal TarjetaDashboardRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _tarjetaDashboardDAL = new TarjetaDashboardDAL();
            _accesoDatos = accesoDatos;
        }

        public TarjetaDashboard ObtenerPorId(int id)
        {
            TarjetaDashboard tarjeta = _tarjetaDashboardDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(tarjeta);
            return tarjeta;
        }

        public List<TarjetaDashboard> ObtenerPorDashboard(Dashboard dashboard)
        {
            List<TarjetaDashboard> tarjetas = _tarjetaDashboardDAL.ObtenerPorDashboard(dashboard, _accesoDatos);
            tarjetas.ForEach(Hidratar);
            return tarjetas;
        }

        private void Hidratar(TarjetaDashboard tarjeta)
        {
            if (tarjeta == null)
            {
                return;
            }

            tarjeta.Dashboard = new DashboardRepositorio(_accesoDatos).ObtenerPorId(tarjeta.Dashboard.Id);
        }

        public int InsertarYObtenerId(TarjetaDashboard tarjeta)
        {
            return _tarjetaDashboardDAL.InsertarYObtenerId(tarjeta, _accesoDatos);
        }

        public bool Actualizar(TarjetaDashboard tarjeta)
        {
            return _tarjetaDashboardDAL.Actualizar(tarjeta, _accesoDatos);
        }

        public bool Eliminar(TarjetaDashboard tarjeta)
        {
            return _tarjetaDashboardDAL.Eliminar(tarjeta, _accesoDatos);
        }
    }
}
