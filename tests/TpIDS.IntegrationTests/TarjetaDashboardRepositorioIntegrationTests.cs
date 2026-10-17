using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Patron de aislamiento: cada test abre su propia transaccion sobre
    // AccesoDatosSqlServer y hace RollbackTransactionScope() al final,
    // asi que ningun test deja datos permanentes en TpCatedraIDS_Test.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class TarjetaDashboardRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_TarjetaValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TarjetaDashboardRepositorio repositorio = new TarjetaDashboardRepositorio(accesoDatos);
                int dashboardId = InsertarDashboardTest(accesoDatos, "user");

                int id = repositorio.InsertarYObtenerId(new BE.TarjetaDashboard
                {
                    Dashboard = new BE.Dashboard { Id = dashboardId },
                    Tipo = BE.TipoTarjetaDashboard.IngresosPorCategoria,
                    Orden = 1
                });

                BE.TarjetaDashboard obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual(dashboardId, obtenida.Dashboard.Id);
                Assert.AreEqual(BE.TipoTarjetaDashboard.IngresosPorCategoria, obtenida.Tipo);
                Assert.AreEqual(1, obtenida.Orden);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorDashboard_ExcluyeTarjetasDeOtroDashboard()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TarjetaDashboardRepositorio repositorio = new TarjetaDashboardRepositorio(accesoDatos);
                int dashboardId = InsertarDashboardTest(accesoDatos, "user");
                int otroDashboardId = InsertarDashboardTest(accesoDatos, "admin");

                int idPropia = repositorio.InsertarYObtenerId(new BE.TarjetaDashboard
                {
                    Dashboard = new BE.Dashboard { Id = dashboardId },
                    Tipo = BE.TipoTarjetaDashboard.IngresosPorCategoria,
                    Orden = 1
                });
                int idAjena = repositorio.InsertarYObtenerId(new BE.TarjetaDashboard
                {
                    Dashboard = new BE.Dashboard { Id = otroDashboardId },
                    Tipo = BE.TipoTarjetaDashboard.GastosPorCategoria,
                    Orden = 1
                });

                System.Collections.Generic.List<BE.TarjetaDashboard> propias = repositorio.ObtenerPorDashboard(dashboardId);

                Assert.IsTrue(propias.Any(item => item.Id == idPropia));
                Assert.IsFalse(propias.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_TarjetaExistente_PersisteNuevoOrden()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TarjetaDashboardRepositorio repositorio = new TarjetaDashboardRepositorio(accesoDatos);
                int dashboardId = InsertarDashboardTest(accesoDatos, "user");
                int id = repositorio.InsertarYObtenerId(new BE.TarjetaDashboard
                {
                    Dashboard = new BE.Dashboard { Id = dashboardId },
                    Tipo = BE.TipoTarjetaDashboard.IngresosPorCategoria,
                    Orden = 1
                });

                bool actualizada = repositorio.Actualizar(new BE.TarjetaDashboard
                {
                    Id = id,
                    Dashboard = new BE.Dashboard { Id = dashboardId },
                    Tipo = BE.TipoTarjetaDashboard.ComparativoMensual,
                    Orden = 3
                });

                Assert.IsTrue(actualizada);
                BE.TarjetaDashboard obtenida = repositorio.ObtenerPorId(id);
                Assert.AreEqual(BE.TipoTarjetaDashboard.ComparativoMensual, obtenida.Tipo);
                Assert.AreEqual(3, obtenida.Orden);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_TarjetaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TarjetaDashboardRepositorio repositorio = new TarjetaDashboardRepositorio(accesoDatos);
                int dashboardId = InsertarDashboardTest(accesoDatos, "user");
                int id = repositorio.InsertarYObtenerId(new BE.TarjetaDashboard
                {
                    Dashboard = new BE.Dashboard { Id = dashboardId },
                    Tipo = BE.TipoTarjetaDashboard.EvolucionBalance,
                    Orden = 1
                });

                bool eliminada = repositorio.Eliminar(id);

                Assert.IsTrue(eliminada);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarDashboardTest(AccesoDatosSqlServer accesoDatos, string username)
        {
            int usuarioId = new UsuarioDAL().ObtenerPorUsername(username, accesoDatos).Id;
            return new DashboardRepositorio(accesoDatos).InsertarYObtenerId(new BE.Dashboard { Usuario = new BE.Usuario { Id = usuarioId } });
        }
    }
}
