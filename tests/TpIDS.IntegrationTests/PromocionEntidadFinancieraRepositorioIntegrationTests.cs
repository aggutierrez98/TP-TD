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
    public sealed class PromocionEntidadFinancieraRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarEntidadesFinancierasDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionEntidadFinancieraRepositorio repositorio = new PromocionEntidadFinancieraRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestAsignacionEntidades");
                int entidadId1 = InsertarEntidadTest(accesoDatos, "EntidadTestPromoAsignacion1");
                int entidadId2 = InsertarEntidadTest(accesoDatos, "EntidadTestPromoAsignacion2");

                bool reemplazado = repositorio.ReemplazarEntidadesFinancierasDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = entidadId1 }, new BE.EntidadFinanciera { Id = entidadId2 } });

                Assert.IsTrue(reemplazado);
                List<int> asignadas = repositorio.ObtenerEntidadesFinancierasPorPromocion(new BE.Promocion { Id = promocionId }).ConvertAll(item => item.Id);
                Assert.AreEqual(2, asignadas.Count);
                Assert.IsTrue(asignadas.Contains(entidadId1));
                Assert.IsTrue(asignadas.Contains(entidadId2));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarEntidadesFinancierasDePromocion_NuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionEntidadFinancieraRepositorio repositorio = new PromocionEntidadFinancieraRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoEntidades");
                int entidadIdA = InsertarEntidadTest(accesoDatos, "EntidadTestPromoReemplazoA");
                int entidadIdB = InsertarEntidadTest(accesoDatos, "EntidadTestPromoReemplazoB");

                repositorio.ReemplazarEntidadesFinancierasDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = entidadIdA } });
                repositorio.ReemplazarEntidadesFinancierasDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = entidadIdB } });

                List<int> asignadas = repositorio.ObtenerEntidadesFinancierasPorPromocion(new BE.Promocion { Id = promocionId }).ConvertAll(item => item.Id);
                Assert.AreEqual(1, asignadas.Count);
                Assert.IsTrue(asignadas.Contains(entidadIdB));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarEntidadesFinancierasDePromocion_ListaVacia_QuitaTodas()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionEntidadFinancieraRepositorio repositorio = new PromocionEntidadFinancieraRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestVaciarEntidades");
                int entidadId = InsertarEntidadTest(accesoDatos, "EntidadTestPromoVaciar");
                repositorio.ReemplazarEntidadesFinancierasDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = entidadId } });

                repositorio.ReemplazarEntidadesFinancierasDePromocion(new BE.Promocion { Id = promocionId }, new List<BE.EntidadFinanciera>());

                Assert.AreEqual(0, repositorio.ObtenerEntidadesFinancierasPorPromocion(new BE.Promocion { Id = promocionId }).Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorEntidadFinanciera_CuentaLasPromocionesQueLaReferencian()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionEntidadFinancieraRepositorio repositorio = new PromocionEntidadFinancieraRepositorio(accesoDatos);
                int entidadUsadaId = InsertarEntidadTest(accesoDatos, "EntidadTestContarUsada");
                int entidadSinUsoId = InsertarEntidadTest(accesoDatos, "EntidadTestContarSinUso");
                int promocionAId = InsertarPromocionTest(accesoDatos, "PromocionTestContarEntidadA");
                int promocionBId = InsertarPromocionTest(accesoDatos, "PromocionTestContarEntidadB");
                repositorio.ReemplazarEntidadesFinancierasDePromocion(new BE.Promocion { Id = promocionAId }, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = entidadUsadaId } });
                repositorio.ReemplazarEntidadesFinancierasDePromocion(new BE.Promocion { Id = promocionBId }, new List<BE.EntidadFinanciera> { new BE.EntidadFinanciera { Id = entidadUsadaId } });

                Assert.AreEqual(2, repositorio.ContarPorEntidadFinanciera(new BE.EntidadFinanciera { Id = entidadUsadaId }));
                Assert.AreEqual(0, repositorio.ContarPorEntidadFinanciera(new BE.EntidadFinanciera { Id = entidadSinUsoId }));

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

        private static int InsertarEntidadTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = nombre });
        }
    }
}
