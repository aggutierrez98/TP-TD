using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class DashboardRepositorio : IDashboardRepositorio
    {
        private readonly DashboardDAL _dashboardDAL;
        private readonly TarjetaDashboardDAL _tarjetaDashboardDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public DashboardRepositorio()
            : this(null)
        {
        }

        internal DashboardRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _dashboardDAL = new DashboardDAL();
            _tarjetaDashboardDAL = new TarjetaDashboardDAL();
            _accesoDatos = accesoDatos;
        }

        public Dashboard ObtenerPorId(int id)
        {
            Dashboard dashboard = _dashboardDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(dashboard);
            return dashboard;
        }

        public Dashboard ObtenerPorUsuario(int usuarioId)
        {
            Dashboard dashboard = _dashboardDAL.ObtenerPorUsuario(usuarioId, _accesoDatos);
            Hidratar(dashboard);
            return dashboard;
        }

        private void Hidratar(Dashboard dashboard)
        {
            if (dashboard == null)
            {
                return;
            }

            dashboard.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(dashboard.Usuario.Id);

            // Se leen las tarjetas directo del DAL (no via TarjetaDashboardRepositorio) para no
            // reingresar en su Hidratar y volver a pedir este mismo Dashboard al infinito.
            dashboard.Tarjetas = _tarjetaDashboardDAL.ObtenerPorDashboard(dashboard.Id, _accesoDatos);
            foreach (TarjetaDashboard tarjeta in dashboard.Tarjetas)
            {
                tarjeta.Dashboard = dashboard;
            }
        }

        public int InsertarYObtenerId(Dashboard dashboard)
        {
            return _dashboardDAL.InsertarYObtenerId(dashboard, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _dashboardDAL.Eliminar(id, _accesoDatos);
        }
    }
}
