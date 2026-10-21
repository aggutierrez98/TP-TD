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
    public sealed class PromocionMedioPagoRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarMediosPagoDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioPagoRepositorio repositorio = new PromocionMedioPagoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestAsignacionMediosPago");
                int medioPagoId1 = InsertarMedioPagoTest(accesoDatos, "MedioTestPromoAsignacion1");
                int medioPagoId2 = InsertarMedioPagoTest(accesoDatos, "MedioTestPromoAsignacion2");

                bool reemplazado = repositorio.ReemplazarMediosPagoDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.MedioPago> { new BE.MedioPago { Id = medioPagoId1 }, new BE.MedioPago { Id = medioPagoId2 } });

                Assert.IsTrue(reemplazado);
                List<int> asignados = repositorio.ObtenerMediosPagoPorPromocion(new BE.Promocion { Id = promocionId }).ConvertAll(item => item.Id);
                Assert.AreEqual(2, asignados.Count);
                Assert.IsTrue(asignados.Contains(medioPagoId1));
                Assert.IsTrue(asignados.Contains(medioPagoId2));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarMediosPagoDePromocion_NuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioPagoRepositorio repositorio = new PromocionMedioPagoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoMediosPago");
                int medioPagoIdA = InsertarMedioPagoTest(accesoDatos, "MedioTestPromoReemplazoA");
                int medioPagoIdB = InsertarMedioPagoTest(accesoDatos, "MedioTestPromoReemplazoB");

                repositorio.ReemplazarMediosPagoDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.MedioPago> { new BE.MedioPago { Id = medioPagoIdA } });
                repositorio.ReemplazarMediosPagoDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.MedioPago> { new BE.MedioPago { Id = medioPagoIdB } });

                List<int> asignados = repositorio.ObtenerMediosPagoPorPromocion(new BE.Promocion { Id = promocionId }).ConvertAll(item => item.Id);
                Assert.AreEqual(1, asignados.Count);
                Assert.IsTrue(asignados.Contains(medioPagoIdB));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarMediosPagoDePromocion_ListaVacia_QuitaTodas()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioPagoRepositorio repositorio = new PromocionMedioPagoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestVaciarMediosPago");
                int medioPagoId = InsertarMedioPagoTest(accesoDatos, "MedioTestPromoVaciar");
                repositorio.ReemplazarMediosPagoDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.MedioPago> { new BE.MedioPago { Id = medioPagoId } });

                repositorio.ReemplazarMediosPagoDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.MedioPago>());

                Assert.AreEqual(0, repositorio.ObtenerMediosPagoPorPromocion(new BE.Promocion { Id = promocionId }).Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorMedioPago_CuentaLasPromocionesQueLoReferencian()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionMedioPagoRepositorio repositorio = new PromocionMedioPagoRepositorio(accesoDatos);
                int medioUsadoId = InsertarMedioPagoTest(accesoDatos, "MarcaTestContarUsado");
                int medioSinUsoId = InsertarMedioPagoTest(accesoDatos, "MarcaTestContarSinUso");
                int promocionAId = InsertarPromocionTest(accesoDatos, "PromocionTestContarMedioA");
                int promocionBId = InsertarPromocionTest(accesoDatos, "PromocionTestContarMedioB");
                repositorio.ReemplazarMediosPagoDePromocion(new BE.Promocion { Id = promocionAId }, new List<BE.MedioPago> { new BE.MedioPago { Id = medioUsadoId } });
                repositorio.ReemplazarMediosPagoDePromocion(new BE.Promocion { Id = promocionBId }, new List<BE.MedioPago> { new BE.MedioPago { Id = medioUsadoId } });

                Assert.AreEqual(2, repositorio.ContarPorMedioPago(new BE.MedioPago { Id = medioUsadoId }));
                Assert.AreEqual(0, repositorio.ContarPorMedioPago(new BE.MedioPago { Id = medioSinUsoId }));

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

        // UQ_MedioPago_Tipo_Marca: cada medio de pago de test lleva una marca propia.
        private static int InsertarMedioPagoTest(AccesoDatosSqlServer accesoDatos, string marca)
        {
            return new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago
            {
                Tipo = BE.TipoMedioPago.TarjetaCredito,
                Marca = marca
            });
        }
    }
}
