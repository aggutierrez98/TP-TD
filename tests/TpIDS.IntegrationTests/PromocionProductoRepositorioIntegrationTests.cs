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
    public sealed class PromocionProductoRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarProductosDePromocion_IncluidosYExcluidos_SeObtienenPorSeparado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionProductoRepositorio repositorio = new PromocionProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestIncExcProducto");
                int incluidoId = InsertarProductoTest(accesoDatos, "ProductoTestIncluido");
                int excluidoId = InsertarProductoTest(accesoDatos, "ProductoTestExcluido");

                bool reemplazado = repositorio.ReemplazarProductosDePromocion(promocionId, new List<int> { incluidoId }, new List<int> { excluidoId });

                Assert.IsTrue(reemplazado);
                List<int> incluidos = repositorio.ObtenerProductoIdsPorPromocion(promocionId, false);
                List<int> excluidos = repositorio.ObtenerProductoIdsPorPromocion(promocionId, true);
                Assert.AreEqual(1, incluidos.Count);
                Assert.AreEqual(incluidoId, incluidos[0]);
                Assert.AreEqual(1, excluidos.Count);
                Assert.AreEqual(excluidoId, excluidos[0]);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarProductosDePromocion_NuevasListas_ReemplazanLasAnteriores()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionProductoRepositorio repositorio = new PromocionProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoProducto");
                int idA = InsertarProductoTest(accesoDatos, "ProductoTestReemplazoA");
                int idB = InsertarProductoTest(accesoDatos, "ProductoTestReemplazoB");
                repositorio.ReemplazarProductosDePromocion(promocionId, new List<int> { idA }, new List<int>());

                repositorio.ReemplazarProductosDePromocion(promocionId, new List<int>(), new List<int> { idB });

                Assert.AreEqual(0, repositorio.ObtenerProductoIdsPorPromocion(promocionId, false).Count);
                List<int> excluidos = repositorio.ObtenerProductoIdsPorPromocion(promocionId, true);
                Assert.AreEqual(1, excluidos.Count);
                Assert.AreEqual(idB, excluidos[0]);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarProductosDePromocion_ListasVacias_QuitanTodo()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionProductoRepositorio repositorio = new PromocionProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestVaciarProducto");
                int incluidoId = InsertarProductoTest(accesoDatos, "ProductoTestVaciarInc");
                int excluidoId = InsertarProductoTest(accesoDatos, "ProductoTestVaciarExc");
                repositorio.ReemplazarProductosDePromocion(promocionId, new List<int> { incluidoId }, new List<int> { excluidoId });

                repositorio.ReemplazarProductosDePromocion(promocionId, new List<int>(), new List<int>());

                Assert.AreEqual(0, repositorio.ObtenerProductoIdsPorPromocion(promocionId, false).Count);
                Assert.AreEqual(0, repositorio.ObtenerProductoIdsPorPromocion(promocionId, true).Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        // La PK compuesta (PromocionId, elemento) impide incluir y excluir el mismo elemento a la vez.
        [TestMethod]
        public void ReemplazarProductosDePromocion_MismoElementoIncluidoYExcluido_Falla()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionProductoRepositorio repositorio = new PromocionProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestContradiccionProducto");
                int elementoId = InsertarProductoTest(accesoDatos, "ProductoTestContradiccion");

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() =>
                    repositorio.ReemplazarProductosDePromocion(promocionId, new List<int> { elementoId }, new List<int> { elementoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorProducto_CuentaLasPromocionesQueLoIncluyenOExcluyen()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionProductoRepositorio repositorio = new PromocionProductoRepositorio(accesoDatos);
                int productoUsadoId = InsertarProductoTest(accesoDatos, "ProductoTestContarUsado");
                int productoSinUsoId = InsertarProductoTest(accesoDatos, "ProductoTestContarSinUso");
                int incluyeId = InsertarPromocionTest(accesoDatos, "PromocionTestContarProductoInc");
                int excluyeId = InsertarPromocionTest(accesoDatos, "PromocionTestContarProductoExc");
                repositorio.ReemplazarProductosDePromocion(incluyeId, new List<int> { productoUsadoId }, new List<int>());
                repositorio.ReemplazarProductosDePromocion(excluyeId, new List<int>(), new List<int> { productoUsadoId });

                Assert.AreEqual(2, repositorio.ContarPorProducto(productoUsadoId));
                Assert.AreEqual(0, repositorio.ContarPorProducto(productoSinUsoId));

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

        private static int InsertarProductoTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            int categoriaProductoId = new CategoriaProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.CategoriaProducto { Nombre = "Cat-" + nombre });
            return new ProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.Producto
            {
                CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                Nombre = nombre
            });
        }
    }
}
