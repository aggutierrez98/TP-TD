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
    public sealed class PromocionCategoriaProductoRepositorioIntegrationTests
    {
        [TestMethod]
        public void ReemplazarCategoriasProductoDePromocion_IncluidosYExcluidos_SeObtienenPorSeparado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionCategoriaProductoRepositorio repositorio = new PromocionCategoriaProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestIncExcCategoria");
                int incluidoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestIncluido");
                int excluidoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestExcluido");

                bool reemplazado = repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = promocionId }, Categorias(incluidoId), Categorias(excluidoId));

                Assert.IsTrue(reemplazado);
                List<int> incluidos = repositorio.ObtenerCategoriasProductoPorPromocion(new BE.Promocion { Id = promocionId }, false).ConvertAll(item => item.Id);
                List<int> excluidos = repositorio.ObtenerCategoriasProductoPorPromocion(new BE.Promocion { Id = promocionId }, true).ConvertAll(item => item.Id);
                Assert.AreEqual(1, incluidos.Count);
                Assert.AreEqual(incluidoId, incluidos[0]);
                Assert.AreEqual(1, excluidos.Count);
                Assert.AreEqual(excluidoId, excluidos[0]);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarCategoriasProductoDePromocion_NuevasListas_ReemplazanLasAnteriores()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionCategoriaProductoRepositorio repositorio = new PromocionCategoriaProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestReemplazoCategoria");
                int idA = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestReemplazoA");
                int idB = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestReemplazoB");
                repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = promocionId }, Categorias(idA), Categorias());

                repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = promocionId }, Categorias(), Categorias(idB));

                Assert.AreEqual(0, repositorio.ObtenerCategoriasProductoPorPromocion(new BE.Promocion { Id = promocionId }, false).Count);
                List<int> excluidos = repositorio.ObtenerCategoriasProductoPorPromocion(new BE.Promocion { Id = promocionId }, true).ConvertAll(item => item.Id);
                Assert.AreEqual(1, excluidos.Count);
                Assert.AreEqual(idB, excluidos[0]);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarCategoriasProductoDePromocion_ListasVacias_QuitanTodo()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionCategoriaProductoRepositorio repositorio = new PromocionCategoriaProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestVaciarCategoria");
                int incluidoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestVaciarInc");
                int excluidoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestVaciarExc");
                repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = promocionId }, Categorias(incluidoId), Categorias(excluidoId));

                repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = promocionId }, Categorias(), Categorias());

                Assert.AreEqual(0, repositorio.ObtenerCategoriasProductoPorPromocion(new BE.Promocion { Id = promocionId }, false).Count);
                Assert.AreEqual(0, repositorio.ObtenerCategoriasProductoPorPromocion(new BE.Promocion { Id = promocionId }, true).Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        // La PK compuesta (PromocionId, elemento) impide incluir y excluir el mismo elemento a la vez.
        [TestMethod]
        public void ReemplazarCategoriasProductoDePromocion_MismoElementoIncluidoYExcluido_Falla()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionCategoriaProductoRepositorio repositorio = new PromocionCategoriaProductoRepositorio(accesoDatos);
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestContradiccionCategoria");
                int elementoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestContradiccion");

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() =>
                    repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = promocionId }, Categorias(elementoId), Categorias(elementoId)));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorCategoriaProducto_CuentaLasPromocionesQueLaIncluyenOExcluyen()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                PromocionCategoriaProductoRepositorio repositorio = new PromocionCategoriaProductoRepositorio(accesoDatos);
                int categoriaUsadaId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestContarUsada");
                int categoriaSinUsoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestContarSinUso");
                int incluyeId = InsertarPromocionTest(accesoDatos, "PromocionTestContarCategoriaInc");
                int excluyeId = InsertarPromocionTest(accesoDatos, "PromocionTestContarCategoriaExc");
                repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = incluyeId }, Categorias(categoriaUsadaId), Categorias());
                repositorio.ReemplazarCategoriasProductoDePromocion(new BE.Promocion { Id = excluyeId }, Categorias(), Categorias(categoriaUsadaId));

                Assert.AreEqual(2, repositorio.ContarPorCategoriaProducto(new BE.CategoriaProducto { Id = categoriaUsadaId }));
                Assert.AreEqual(0, repositorio.ContarPorCategoriaProducto(new BE.CategoriaProducto { Id = categoriaSinUsoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static List<BE.CategoriaProducto> Categorias(params int[] ids)
        {
            return new List<int>(ids).ConvertAll(id => new BE.CategoriaProducto { Id = id });
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

        private static int InsertarCategoriaProductoTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new CategoriaProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.CategoriaProducto { Nombre = nombre });
        }
    }
}
