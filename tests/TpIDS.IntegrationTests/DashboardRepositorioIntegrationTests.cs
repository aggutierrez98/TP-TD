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
    public sealed class DashboardRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_DashboardValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                DashboardRepositorio repositorio = new DashboardRepositorio(accesoDatos);
                int usuarioId = new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;

                int id = repositorio.InsertarYObtenerId(new BE.Dashboard
                {
                    Usuario = new BE.Usuario { Id = usuarioId }
                });

                BE.Dashboard obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(usuarioId, obtenido.Usuario.Id);
                Assert.IsNotNull(obtenido.Tarjetas);
                Assert.AreEqual(0, obtenido.Tarjetas.Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_DevuelveElDashboardDelUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                DashboardRepositorio repositorio = new DashboardRepositorio(accesoDatos);
                int usuarioId = new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;

                int id = repositorio.InsertarYObtenerId(new BE.Dashboard
                {
                    Usuario = new BE.Usuario { Id = usuarioId }
                });

                BE.Dashboard obtenido = repositorio.ObtenerPorUsuario(usuarioId);

                Assert.IsNotNull(obtenido);
                Assert.AreEqual(id, obtenido.Id);
                Assert.AreEqual(usuarioId, obtenido.Usuario.Id);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_SinDashboard_DevuelveNull()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                DashboardRepositorio repositorio = new DashboardRepositorio(accesoDatos);
                int otroUsuarioId = new UsuarioDAL().ObtenerPorUsername("admin", accesoDatos).Id;

                BE.Dashboard obtenido = repositorio.ObtenerPorUsuario(otroUsuarioId);

                Assert.IsNull(obtenido);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_DashboardExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                DashboardRepositorio repositorio = new DashboardRepositorio(accesoDatos);
                int usuarioId = new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;
                int id = repositorio.InsertarYObtenerId(new BE.Dashboard
                {
                    Usuario = new BE.Usuario { Id = usuarioId }
                });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }
    }
}
