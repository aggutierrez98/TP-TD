using System;
using System.Collections.Generic;
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
    public sealed class PromocionMedioDePagoRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarMediosDePagoDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioDePagoRepositorio repositorio = new PromocionMedioDePagoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestAsignacionMedios");
                int medioDePagoId1 = InsertarMedioDePagoTest(accesoDatos, "EntidadTestAsignacionMedios1");
                int medioDePagoId2 = InsertarMedioDePagoTest(accesoDatos, "EntidadTestAsignacionMedios2");

                bool reemplazado = repositorio.ReemplazarMediosDePagoDePromocion(promocionId, new List<int> { medioDePagoId1, medioDePagoId2 });

                Assert.IsTrue(reemplazado);
                List<int> asignados = repositorio.ObtenerMedioDePagoIdsPorPromocion(promocionId);
                Assert.AreEqual(2, asignados.Count);
                Assert.IsTrue(asignados.Contains(medioDePagoId1));
                Assert.IsTrue(asignados.Contains(medioDePagoId2));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarMediosDePagoDePromocion_LlamadaConNuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioDePagoRepositorio repositorio = new PromocionMedioDePagoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoMedios");
                int medioDePagoIdA = InsertarMedioDePagoTest(accesoDatos, "EntidadTestReemplazoMediosA");
                int medioDePagoIdB = InsertarMedioDePagoTest(accesoDatos, "EntidadTestReemplazoMediosB");

                repositorio.ReemplazarMediosDePagoDePromocion(promocionId, new List<int> { medioDePagoIdA });
                repositorio.ReemplazarMediosDePagoDePromocion(promocionId, new List<int> { medioDePagoIdB });

                List<int> asignados = repositorio.ObtenerMedioDePagoIdsPorPromocion(promocionId);
                Assert.AreEqual(1, asignados.Count);
                Assert.IsTrue(asignados.Contains(medioDePagoIdB));
                Assert.IsFalse(asignados.Contains(medioDePagoIdA));

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

        private static int InsertarMedioDePagoTest(AccesoDatosSqlServer accesoDatos, string nombreEntidad)
        {
            int entidadFinancieraId = new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = nombreEntidad });
            return new MedioDePagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioDePago
            {
                EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                Tipo = BE.TipoMedioDePago.TarjetaCredito,
                Marca = "Visa",
                Segmento = "Gold"
            });
        }
    }
}
