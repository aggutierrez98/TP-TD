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
    public sealed class RubroComercioRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_RubroValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RubroComercioRepositorio repositorio = new RubroComercioRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.RubroComercio
                {
                    Nombre = "RubroTestSupermercado"
                });

                BE.RubroComercio obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual("RubroTestSupermercado", obtenido.Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeInsertado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RubroComercioRepositorio repositorio = new RubroComercioRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.RubroComercio { Nombre = "RubroTestFarmacia" });

                System.Collections.Generic.List<BE.RubroComercio> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == id && item.Nombre == "RubroTestFarmacia"));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_RubroExistente_PersisteNuevoNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RubroComercioRepositorio repositorio = new RubroComercioRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.RubroComercio { Nombre = "RubroTestOriginal" });

                bool actualizado = repositorio.Actualizar(new BE.RubroComercio { Id = id, Nombre = "RubroTestRenombrado" });

                Assert.IsTrue(actualizado);
                Assert.AreEqual("RubroTestRenombrado", repositorio.ObtenerPorId(id).Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_RubroExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RubroComercioRepositorio repositorio = new RubroComercioRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.RubroComercio { Nombre = "RubroTestAEliminar" });

                bool eliminado = repositorio.Eliminar(new BE.RubroComercio { Id = id });

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }
    }
}
