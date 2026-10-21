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
    public sealed class PromocionSegmentoRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarSegmentosDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionSegmentoRepositorio repositorio = new PromocionSegmentoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestAsignacionSegmentos");

                bool reemplazado = repositorio.ReemplazarSegmentosDePromocion(new BE.Promocion { Id = promocionId }, new List<string> { "Gold", "Black" });

                Assert.IsTrue(reemplazado);
                List<string> asignados = repositorio.ObtenerSegmentosPorPromocion(new BE.Promocion { Id = promocionId });
                Assert.AreEqual(2, asignados.Count);
                Assert.IsTrue(asignados.Contains("Gold"));
                Assert.IsTrue(asignados.Contains("Black"));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarSegmentosDePromocion_NuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionSegmentoRepositorio repositorio = new PromocionSegmentoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoSegmentos");

                repositorio.ReemplazarSegmentosDePromocion(new BE.Promocion { Id = promocionId }, new List<string> { "Gold" });
                repositorio.ReemplazarSegmentosDePromocion(new BE.Promocion { Id = promocionId }, new List<string> { "Platinum" });

                List<string> asignados = repositorio.ObtenerSegmentosPorPromocion(new BE.Promocion { Id = promocionId });
                Assert.AreEqual(1, asignados.Count);
                Assert.AreEqual("Platinum", asignados[0]);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarSegmentosDePromocion_ListaVacia_QuitaTodos()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionSegmentoRepositorio repositorio = new PromocionSegmentoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestVaciarSegmentos");
                repositorio.ReemplazarSegmentosDePromocion(new BE.Promocion { Id = promocionId }, new List<string> { "Gold" });

                repositorio.ReemplazarSegmentosDePromocion(new BE.Promocion { Id = promocionId }, new List<string>());

                Assert.AreEqual(0, repositorio.ObtenerSegmentosPorPromocion(new BE.Promocion { Id = promocionId }).Count);

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
    }
}
