using System;
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
    public sealed class TasaConversionRepositorioIntegrationTests
    {
        [TestMethod]
        public void ScriptSeed_IncluyeARSUSDYTasaUSDHaciaARS()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                MonedaRepositorio monedaRepositorio = new MonedaRepositorio(accesoDatos);
                TasaConversionRepositorio tasaRepositorio = new TasaConversionRepositorio(accesoDatos);

                BE.Moneda pesoArgentino = monedaRepositorio.ObtenerTodas().Single(item => item.Codigo == "ARS");
                BE.Moneda dolarEstadounidense = monedaRepositorio.ObtenerTodas().Single(item => item.Codigo == "USD");
                BE.TasaConversion tasa = tasaRepositorio.ObtenerPorParDeMonedas(dolarEstadounidense.Id, pesoArgentino.Id);

                Assert.AreEqual("Peso Argentino", pesoArgentino.Nombre);
                Assert.AreEqual("Dólar Estadounidense", dolarEstadounidense.Nombre);
                Assert.IsNotNull(tasa);
                Assert.AreEqual(1511.2481m, tasa.CalculoConversion);
                Assert.AreEqual(new DateTime(2026, 9, 20), tasa.FechaActualizacion);
            }
        }

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

        [TestMethod]
        public void InsertarYObtenerId_TasaNueva_NoEsManualYNoTieneProteccion()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int id = InsertarTasaTest(accesoDatos, repositorio, "TD1", "TD2", 10m);

                BE.TasaConversion obtenida = repositorio.ObtenerPorId(id);

                Assert.IsFalse(obtenida.EsManual);
                Assert.IsNull(obtenida.ProtegidaPorUsuarioId);
                Assert.IsNull(obtenida.FechaProteccion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void MarcarManual_TasaExistente_PersisteMarcaUsuarioYFecha()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int id = InsertarTasaTest(accesoDatos, repositorio, "TD3", "TD4", 10m);
                int usuarioId = ObtenerUsuarioIdTest(accesoDatos);
                DateTime antes = DateTime.Now.AddMinutes(-1);

                bool marcada = repositorio.MarcarManual(id, usuarioId);

                BE.TasaConversion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsTrue(marcada);
                Assert.IsTrue(obtenida.EsManual);
                Assert.AreEqual(usuarioId, obtenida.ProtegidaPorUsuarioId);
                Assert.IsNotNull(obtenida.FechaProteccion);
                Assert.IsTrue(obtenida.FechaProteccion.Value >= antes);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void MarcarManual_TasaInexistente_DevuelveFalse()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);

                Assert.IsFalse(repositorio.MarcarManual(int.MaxValue, ObtenerUsuarioIdTest(accesoDatos)));
                Assert.IsFalse(repositorio.Liberar(int.MaxValue));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Liberar_TasaManual_LimpiaMarcaUsuarioYFecha()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int id = InsertarTasaTest(accesoDatos, repositorio, "TD5", "TD6", 10m);
                repositorio.MarcarManual(id, ObtenerUsuarioIdTest(accesoDatos));

                bool liberada = repositorio.Liberar(id);

                BE.TasaConversion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsTrue(liberada);
                Assert.IsFalse(obtenida.EsManual);
                Assert.IsNull(obtenida.ProtegidaPorUsuarioId);
                Assert.IsNull(obtenida.FechaProteccion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ActualizarValorAutomatico_TasaNoManual_ActualizaValorYFechaSinMarcarla()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int id = InsertarTasaTest(accesoDatos, repositorio, "TD7", "TD8", 10m);
                DateTime antes = DateTime.Now.AddMinutes(-1);

                bool actualizada = repositorio.ActualizarValorAutomatico(id, 1234.567891m);

                BE.TasaConversion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsTrue(actualizada);
                Assert.AreEqual(1234.567891m, obtenida.CalculoConversion);
                Assert.IsTrue(obtenida.FechaActualizacion >= antes);
                Assert.IsFalse(obtenida.EsManual);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ActualizarValorAutomatico_TasaManual_NoCambiaElValorNiLaMarca()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int id = InsertarTasaTest(accesoDatos, repositorio, "TD9", "TE1", 10m);
                int usuarioId = ObtenerUsuarioIdTest(accesoDatos);
                repositorio.MarcarManual(id, usuarioId);

                bool actualizada = repositorio.ActualizarValorAutomatico(id, 999m);

                BE.TasaConversion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsFalse(actualizada);
                Assert.AreEqual(10m, obtenida.CalculoConversion);
                Assert.IsTrue(obtenida.EsManual);
                Assert.AreEqual(usuarioId, obtenida.ProtegidaPorUsuarioId);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeElEstadoManualDeCadaTasa()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                TasaConversionRepositorio repositorio = new TasaConversionRepositorio(accesoDatos);
                int idManual = InsertarTasaTest(accesoDatos, repositorio, "TE2", "TE3", 10m);
                int idAutomatica = InsertarTasaTest(accesoDatos, repositorio, "TE4", "TE5", 20m);
                repositorio.MarcarManual(idManual, ObtenerUsuarioIdTest(accesoDatos));

                var tasas = repositorio.ObtenerTodas();

                Assert.IsTrue(tasas.Single(item => item.Id == idManual).EsManual);
                Assert.IsFalse(tasas.Single(item => item.Id == idAutomatica).EsManual);

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarTasaTest(
            AccesoDatosSqlServer accesoDatos,
            TasaConversionRepositorio repositorio,
            string codigoBase,
            string codigoConversion,
            decimal calculo)
        {
            int monedaBaseId = InsertarMonedaTest(accesoDatos, codigoBase, "MonedaTestBase" + codigoBase);
            int monedaConversionId = InsertarMonedaTest(accesoDatos, codigoConversion, "MonedaTestConversion" + codigoConversion);
            return repositorio.InsertarYObtenerId(new BE.TasaConversion
            {
                MonedaBase = new BE.Moneda { Id = monedaBaseId },
                MonedaConversion = new BE.Moneda { Id = monedaConversionId },
                CalculoConversion = calculo
            });
        }

        private static int ObtenerUsuarioIdTest(AccesoDatosSqlServer accesoDatos)
        {
            return (int)accesoDatos.Read("SELECT TOP 1 Id FROM Usuarios ORDER BY Id").Rows[0]["Id"];
        }

        private static int InsertarMonedaTest(AccesoDatosSqlServer accesoDatos, string codigo, string nombre)
        {
            return new MonedaRepositorio(accesoDatos).InsertarYObtenerId(new BE.Moneda { Codigo = codigo, Nombre = nombre });
        }
    }
}
