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
    public sealed class PromocionRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_PromocionValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestPropia",
                    Descuento = 15m,
                    VigenciaDesde = new DateTime(2026, 1, 1),
                    VigenciaHasta = new DateTime(2026, 1, 31)
                });

                BE.Promocion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual("PromocionTestPropia", obtenida.Descripcion);
                Assert.AreEqual(15m, obtenida.Descuento);
                Assert.AreEqual(new DateTime(2026, 1, 1), obtenida.VigenciaDesde);
                Assert.AreEqual(new DateTime(2026, 1, 31), obtenida.VigenciaHasta);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeInsertada()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestListado",
                    Descuento = 20m,
                    VigenciaDesde = new DateTime(2026, 2, 1),
                    VigenciaHasta = new DateTime(2026, 2, 28)
                });

                System.Collections.Generic.List<BE.Promocion> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_PromocionExistente_PersisteNuevoDescuento()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestOriginal",
                    Descuento = 10m,
                    VigenciaDesde = new DateTime(2026, 3, 1),
                    VigenciaHasta = new DateTime(2026, 3, 31)
                });

                bool actualizada = repositorio.Actualizar(new BE.Promocion
                {
                    Id = id,
                    Descripcion = "PromocionTestOriginal",
                    Descuento = 30m,
                    VigenciaDesde = new DateTime(2026, 3, 1),
                    VigenciaHasta = new DateTime(2026, 3, 31)
                });

                Assert.IsTrue(actualizada);
                Assert.AreEqual(30m, repositorio.ObtenerPorId(id).Descuento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_PromocionExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionRepositorio repositorio = new PromocionRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Promocion
                {
                    Descripcion = "PromocionTestAEliminar",
                    Descuento = 25m,
                    VigenciaDesde = new DateTime(2026, 4, 1),
                    VigenciaHasta = new DateTime(2026, 4, 30)
                });

                bool eliminada = repositorio.Eliminar(id);

                Assert.IsTrue(eliminada);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }
    }
}
