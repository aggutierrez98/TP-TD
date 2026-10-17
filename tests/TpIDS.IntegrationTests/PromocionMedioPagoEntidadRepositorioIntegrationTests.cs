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
    public sealed class PromocionMedioPagoEntidadRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarMediosPagoEntidadDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioPagoEntidadRepositorio repositorio = new PromocionMedioPagoEntidadRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestAsignacionMedios");
                int medioPagoEntidadId1 = InsertarMedioPagoEntidadTest(accesoDatos, "EntidadTestAsignacionMedios1");
                int medioPagoEntidadId2 = InsertarMedioPagoEntidadTest(accesoDatos, "EntidadTestAsignacionMedios2");

                bool reemplazado = repositorio.ReemplazarMediosPagoEntidadDePromocion(promocionId, new List<int> { medioPagoEntidadId1, medioPagoEntidadId2 });

                Assert.IsTrue(reemplazado);
                List<int> asignados = repositorio.ObtenerMedioPagoEntidadIdsPorPromocion(promocionId);
                Assert.AreEqual(2, asignados.Count);
                Assert.IsTrue(asignados.Contains(medioPagoEntidadId1));
                Assert.IsTrue(asignados.Contains(medioPagoEntidadId2));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarMediosPagoEntidadDePromocion_LlamadaConNuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioPagoEntidadRepositorio repositorio = new PromocionMedioPagoEntidadRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoMedios");
                int medioPagoEntidadIdA = InsertarMedioPagoEntidadTest(accesoDatos, "EntidadTestReemplazoMediosA");
                int medioPagoEntidadIdB = InsertarMedioPagoEntidadTest(accesoDatos, "EntidadTestReemplazoMediosB");

                repositorio.ReemplazarMediosPagoEntidadDePromocion(promocionId, new List<int> { medioPagoEntidadIdA });
                repositorio.ReemplazarMediosPagoEntidadDePromocion(promocionId, new List<int> { medioPagoEntidadIdB });

                List<int> asignados = repositorio.ObtenerMedioPagoEntidadIdsPorPromocion(promocionId);
                Assert.AreEqual(1, asignados.Count);
                Assert.IsTrue(asignados.Contains(medioPagoEntidadIdB));
                Assert.IsFalse(asignados.Contains(medioPagoEntidadIdA));

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

        // Cada llamada crea su propia entidad y su propio MedioPago (marca derivada del
        // nombre de la entidad) para no chocar con UQ_MedioPago_Tipo_Marca.
        private static int InsertarMedioPagoEntidadTest(AccesoDatosSqlServer accesoDatos, string nombreEntidad)
        {
            int entidadFinancieraId = new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = nombreEntidad });
            int medioPagoId = new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago
            {
                Tipo = BE.TipoMedioPago.TarjetaCredito,
                Marca = "Marca" + nombreEntidad
            });

            return new MedioPagoEntidadRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPagoEntidad
            {
                EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                MedioPago = new BE.MedioPago { Id = medioPagoId },
                Segmento = "Gold"
            });
        }
    }
}
