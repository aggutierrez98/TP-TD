using System;
using System.Collections.Generic;
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
    public sealed class PromocionRubroComercioRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarRubrosComercioDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRubroComercioRepositorio repositorio = new PromocionRubroComercioRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestAsignacionRubros");
                int rubroComercioId1 = InsertarRubroComercioTest(accesoDatos, "RubroTestAsignacion1");
                int rubroComercioId2 = InsertarRubroComercioTest(accesoDatos, "RubroTestAsignacion2");

                bool reemplazado = repositorio.ReemplazarRubrosComercioDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.RubroComercio> { new BE.RubroComercio { Id = rubroComercioId1 }, new BE.RubroComercio { Id = rubroComercioId2 } });

                Assert.IsTrue(reemplazado);
                List<int> asignados = repositorio.ObtenerRubrosComercioPorPromocion(new BE.Promocion { Id = promocionId }).ConvertAll(item => item.Id);
                Assert.AreEqual(2, asignados.Count);
                Assert.IsTrue(asignados.Contains(rubroComercioId1));
                Assert.IsTrue(asignados.Contains(rubroComercioId2));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarRubrosComercioDePromocion_LlamadaConNuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRubroComercioRepositorio repositorio = new PromocionRubroComercioRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoRubros");
                int rubroComercioIdA = InsertarRubroComercioTest(accesoDatos, "RubroTestReemplazoA");
                int rubroComercioIdB = InsertarRubroComercioTest(accesoDatos, "RubroTestReemplazoB");

                repositorio.ReemplazarRubrosComercioDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.RubroComercio> { new BE.RubroComercio { Id = rubroComercioIdA } });
                repositorio.ReemplazarRubrosComercioDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.RubroComercio> { new BE.RubroComercio { Id = rubroComercioIdB } });

                List<int> asignados = repositorio.ObtenerRubrosComercioPorPromocion(new BE.Promocion { Id = promocionId }).ConvertAll(item => item.Id);
                Assert.AreEqual(1, asignados.Count);
                Assert.IsTrue(asignados.Contains(rubroComercioIdB));
                Assert.IsFalse(asignados.Contains(rubroComercioIdA));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorRubroComercio_CuentaLasPromocionesQueLoReferencian()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRubroComercioRepositorio repositorio = new PromocionRubroComercioRepositorio(accesoDatos);
                int rubroUsadoId = InsertarRubroComercioTest(accesoDatos, "RubroTestContarUsado");
                int rubroSinUsoId = InsertarRubroComercioTest(accesoDatos, "RubroTestContarSinUso");
                int promocionAId = InsertarPromocionTest(accesoDatos, "PromocionTestContarRubroA");
                int promocionBId = InsertarPromocionTest(accesoDatos, "PromocionTestContarRubroB");
                repositorio.ReemplazarRubrosComercioDePromocion(new BE.Promocion { Id = promocionAId }, new List<BE.RubroComercio> { new BE.RubroComercio { Id = rubroUsadoId } });
                repositorio.ReemplazarRubrosComercioDePromocion(new BE.Promocion { Id = promocionBId }, new List<BE.RubroComercio> { new BE.RubroComercio { Id = rubroUsadoId } });

                Assert.AreEqual(2, repositorio.ContarPorRubroComercio(new BE.RubroComercio { Id = rubroUsadoId }));
                Assert.AreEqual(0, repositorio.ContarPorRubroComercio(new BE.RubroComercio { Id = rubroSinUsoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarPromocionTest(AccesoDatosSqlServer accesoDatos, string descripcion)
        {
            return new PromocionRepositorio(accesoDatos).InsertarYObtenerId(new BE.Promocion
            {
                Descripcion = descripcion,
                Descuento = 10m,
                VigenciaDesde = new DateTime(2026, 1, 1),
                VigenciaHasta = new DateTime(2026, 1, 31)
            });
        }

        private static int InsertarRubroComercioTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = nombre });
        }
    }
}
