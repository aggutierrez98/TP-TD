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
    public sealed class TasaConversionRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_TasaValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int monedaBaseId = InsertarMonedaTest(accesoDatos, "TC1", "MonedaTestTasaBase");
                int monedaConversionId = InsertarMonedaTest(accesoDatos, "TC2", "MonedaTestTasaConversion");

                int id = repositorio.InsertarYObtenerId(new BE.TasaConversion
                {
                    MonedaBase = new BE.Moneda { Id = monedaBaseId },
                    MonedaConversion = new BE.Moneda { Id = monedaConversionId },
                    CalculoConversion = 350.123456m
                });

                BE.TasaConversion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.IsNotNull(obtenida.MonedaBase);
                Assert.AreEqual(monedaBaseId, obtenida.MonedaBase.Id);
                Assert.AreEqual("MonedaTestTasaBase", obtenida.MonedaBase.Nombre);
                Assert.IsNotNull(obtenida.MonedaConversion);
                Assert.AreEqual(monedaConversionId, obtenida.MonedaConversion.Id);
                Assert.AreEqual(350.123456m, obtenida.CalculoConversion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorParDeMonedas_DevuelveLaTasaDelPar()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int monedaBaseId = InsertarMonedaTest(accesoDatos, "TC3", "MonedaTestTasaParBase");
                int monedaConversionId = InsertarMonedaTest(accesoDatos, "TC4", "MonedaTestTasaParConversion");
                int otraMonedaId = InsertarMonedaTest(accesoDatos, "TC5", "MonedaTestTasaParOtra");

                int id = repositorio.InsertarYObtenerId(new BE.TasaConversion
                {
                    MonedaBase = new BE.Moneda { Id = monedaBaseId },
                    MonedaConversion = new BE.Moneda { Id = monedaConversionId },
                    CalculoConversion = 900.5m
                });

                BE.TasaConversion obtenida = repositorio.ObtenerPorParDeMonedas(monedaBaseId, monedaConversionId);
                BE.TasaConversion inexistente = repositorio.ObtenerPorParDeMonedas(monedaBaseId, otraMonedaId);

                Assert.IsNotNull(obtenida);
                Assert.AreEqual(id, obtenida.Id);
                Assert.IsNull(inexistente);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_TasaExistente_PersisteNuevoCalculo()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int monedaBaseId = InsertarMonedaTest(accesoDatos, "TC6", "MonedaTestTasaActualizarBase");
                int monedaConversionId = InsertarMonedaTest(accesoDatos, "TC7", "MonedaTestTasaActualizarConversion");
                int id = repositorio.InsertarYObtenerId(new BE.TasaConversion
                {
                    MonedaBase = new BE.Moneda { Id = monedaBaseId },
                    MonedaConversion = new BE.Moneda { Id = monedaConversionId },
                    CalculoConversion = 100m
                });

                bool actualizado = repositorio.Actualizar(new BE.TasaConversion
                {
                    Id = id,
                    MonedaBase = new BE.Moneda { Id = monedaBaseId },
                    MonedaConversion = new BE.Moneda { Id = monedaConversionId },
                    CalculoConversion = 123.456m
                });

                Assert.IsTrue(actualizado);
                Assert.AreEqual(123.456m, repositorio.ObtenerPorId(id).CalculoConversion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_TasaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int monedaBaseId = InsertarMonedaTest(accesoDatos, "TC8", "MonedaTestTasaEliminarBase");
                int monedaConversionId = InsertarMonedaTest(accesoDatos, "TC9", "MonedaTestTasaEliminarConversion");
                int id = repositorio.InsertarYObtenerId(new BE.TasaConversion
                {
                    MonedaBase = new BE.Moneda { Id = monedaBaseId },
                    MonedaConversion = new BE.Moneda { Id = monedaConversionId },
                    CalculoConversion = 50m
                });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarMonedaTest(AccesoDatosSqlServer accesoDatos, string codigo, string nombre)
        {
            return new MonedaRepositorio(accesoDatos).InsertarYObtenerId(new BE.Moneda { Codigo = codigo, Nombre = nombre });
        }
    }
}
