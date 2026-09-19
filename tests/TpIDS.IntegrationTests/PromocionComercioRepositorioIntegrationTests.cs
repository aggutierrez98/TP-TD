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
    public sealed class PromocionComercioRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarComerciosDePromocion_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionComercioRepositorio repositorio = new PromocionComercioRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestAsignacionComercios");
                int comercioId1 = InsertarComercioTest(accesoDatos, "ComercioTestAsignacion1");
                int comercioId2 = InsertarComercioTest(accesoDatos, "ComercioTestAsignacion2");

                bool reemplazado = repositorio.ReemplazarComerciosDePromocion(promocionId, new List<int> { comercioId1, comercioId2 });

                Assert.IsTrue(reemplazado);
                List<int> asignados = repositorio.ObtenerComercioIdsPorPromocion(promocionId);
                Assert.AreEqual(2, asignados.Count);
                Assert.IsTrue(asignados.Contains(comercioId1));
                Assert.IsTrue(asignados.Contains(comercioId2));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarComerciosDePromocion_LlamadaConNuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionComercioRepositorio repositorio = new PromocionComercioRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoComercios");
                int comercioIdA = InsertarComercioTest(accesoDatos, "ComercioTestReemplazoA");
                int comercioIdB = InsertarComercioTest(accesoDatos, "ComercioTestReemplazoB");

                repositorio.ReemplazarComerciosDePromocion(promocionId, new List<int> { comercioIdA });
                repositorio.ReemplazarComerciosDePromocion(promocionId, new List<int> { comercioIdB });

                List<int> asignados = repositorio.ObtenerComercioIdsPorPromocion(promocionId);
                Assert.AreEqual(1, asignados.Count);
                Assert.IsTrue(asignados.Contains(comercioIdB));
                Assert.IsFalse(asignados.Contains(comercioIdA));

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

        private static int InsertarComercioTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            int rubroComercioId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = "Rubro-" + nombre });
            return new ComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.Comercio { RubroComercio = new BE.RubroComercio { Id = rubroComercioId }, Nombre = nombre });
        }
    }
}
