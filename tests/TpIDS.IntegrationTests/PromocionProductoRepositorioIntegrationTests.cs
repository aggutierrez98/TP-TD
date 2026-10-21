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

                bool reemplazado = repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = promocionId }, Productos(incluidoId), Productos(excluidoId));

                Assert.IsTrue(reemplazado);
                List<int> incluidos = repositorio.ObtenerProductosPorPromocion(new BE.Promocion { Id = promocionId }, false).ConvertAll(item => item.Id);
                List<int> excluidos = repositorio.ObtenerProductosPorPromocion(new BE.Promocion { Id = promocionId }, true).ConvertAll(item => item.Id);
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
                repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = promocionId }, Productos(idA), Productos());

                repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = promocionId }, Productos(), Productos(idB));

                Assert.AreEqual(0, repositorio.ObtenerProductosPorPromocion(new BE.Promocion { Id = promocionId }, false).Count);
                List<int> excluidos = repositorio.ObtenerProductosPorPromocion(new BE.Promocion { Id = promocionId }, true).ConvertAll(item => item.Id);
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
                repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = promocionId }, Productos(incluidoId), Productos(excluidoId));

                repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = promocionId }, Productos(), Productos());

                Assert.AreEqual(0, repositorio.ObtenerProductosPorPromocion(new BE.Promocion { Id = promocionId }, false).Count);
                Assert.AreEqual(0, repositorio.ObtenerProductosPorPromocion(new BE.Promocion { Id = promocionId }, true).Count);

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
                    repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = promocionId }, Productos(elementoId), Productos(elementoId)));

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
                repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = incluyeId }, Productos(productoUsadoId), Productos());
                repositorio.ReemplazarProductosDePromocion(new BE.Promocion { Id = excluyeId }, Productos(), Productos(productoUsadoId));

                Assert.AreEqual(2, repositorio.ContarPorProducto(new BE.Producto { Id = productoUsadoId }));
                Assert.AreEqual(0, repositorio.ContarPorProducto(new BE.Producto { Id = productoSinUsoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static List<BE.Producto> Productos(params int[] ids)
        {
            return new List<int>(ids).ConvertAll(id => new BE.Producto { Id = id });
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
