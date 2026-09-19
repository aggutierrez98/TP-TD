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
    public sealed class MonedaRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_MonedaValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MonedaRepositorio repositorio = new MonedaRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Moneda
                {
                    Codigo = "TS1",
                    Nombre = "MonedaTestPesoUno"
                });

                BE.Moneda obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual("TS1", obtenida.Codigo);
                Assert.AreEqual("MonedaTestPesoUno", obtenida.Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeInsertada()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MonedaRepositorio repositorio = new MonedaRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Moneda { Codigo = "TS2", Nombre = "MonedaTestPesoDos" });

                System.Collections.Generic.List<BE.Moneda> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == id && item.Nombre == "MonedaTestPesoDos"));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_MonedaExistente_PersisteNuevoNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MonedaRepositorio repositorio = new MonedaRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Moneda { Codigo = "TS3", Nombre = "MonedaTestOriginal" });

                bool actualizado = repositorio.Actualizar(new BE.Moneda { Id = id, Codigo = "TS3", Nombre = "MonedaTestRenombrada" });

                Assert.IsTrue(actualizado);
                Assert.AreEqual("MonedaTestRenombrada", repositorio.ObtenerPorId(id).Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_MonedaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MonedaRepositorio repositorio = new MonedaRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Moneda { Codigo = "TS4", Nombre = "MonedaTestAEliminar" });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }
    }
}
