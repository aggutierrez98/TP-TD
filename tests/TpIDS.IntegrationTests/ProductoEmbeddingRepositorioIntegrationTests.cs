using System;
using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Patron de aislamiento: cada test abre su propia transaccion y hace
    // RollbackTransactionScope() al final.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class ProductoEmbeddingRepositorioIntegrationTests
    {
        [TestMethod]
        public void Guardar_VectorFloat32_SeLeeIgualConSusMetadatos()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoEmbeddingRepositorio repositorio = new ProductoEmbeddingRepositorio(accesoDatos);
                int productoId = InsertarProducto(accesoDatos, "ProductoTestEmbedding");
                float[] vector = { 0.125f, -1.5f, 3.4028235E+38f, float.Epsilon, 0f, -0.333333f };
                DateTime fecha = new DateTime(2026, 10, 19, 12, 30, 0);

                repositorio.Guardar(Crear(productoId, "modelo-test", "ProductoTestEmbedding Cat", vector, fecha));

                BE.ProductoEmbedding leido = repositorio.ObtenerTodos().Single(item => item.ProductoId == productoId);
                Assert.AreEqual("modelo-test", leido.Modelo);
                Assert.AreEqual("ProductoTestEmbedding Cat", leido.TextoFuente);
                Assert.AreEqual(fecha, leido.FechaCalculo);
                CollectionAssert.AreEqual(vector, leido.Vector);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Guardar_ProductoConFilaPrevia_ReemplazaLaFila()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoEmbeddingRepositorio repositorio = new ProductoEmbeddingRepositorio(accesoDatos);
                int productoId = InsertarProducto(accesoDatos, "ProductoTestEmbeddingReemplazo");
                repositorio.Guardar(Crear(productoId, "modelo-viejo", "texto viejo", new[] { 1f, 2f, 3f }, new DateTime(2026, 1, 1)));

                repositorio.Guardar(Crear(productoId, "modelo-nuevo", "texto nuevo", new[] { 4f, 5f }, new DateTime(2026, 2, 1)));

                BE.ProductoEmbedding leido = repositorio.ObtenerTodos().Single(item => item.ProductoId == productoId);
                Assert.AreEqual("modelo-nuevo", leido.Modelo);
                Assert.AreEqual("texto nuevo", leido.TextoFuente);
                Assert.AreEqual(new DateTime(2026, 2, 1), leido.FechaCalculo);
                CollectionAssert.AreEqual(new[] { 4f, 5f }, leido.Vector);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void EliminarProducto_ConEmbedding_BorraTambienElEmbedding()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoEmbeddingRepositorio repositorio = new ProductoEmbeddingRepositorio(accesoDatos);
                int productoId = InsertarProducto(accesoDatos, "ProductoTestEmbeddingBaja");
                repositorio.Guardar(Crear(productoId, "modelo-test", "texto", new[] { 1f }, new DateTime(2026, 1, 1)));

                Assert.IsTrue(new ProductoRepositorio(accesoDatos).Eliminar(productoId));

                Assert.IsFalse(repositorio.ObtenerTodos().Any(item => item.ProductoId == productoId));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Guardar_ProductoInexistente_ViolaLaClaveForanea()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoEmbeddingRepositorio repositorio = new ProductoEmbeddingRepositorio(accesoDatos);

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(
                    () => repositorio.Guardar(Crear(int.MaxValue, "modelo-test", "texto", new[] { 1f }, new DateTime(2026, 1, 1))));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static BE.ProductoEmbedding Crear(int productoId, string modelo, string textoFuente, float[] vector, DateTime fecha)
        {
            return new BE.ProductoEmbedding
            {
                ProductoId = productoId,
                Modelo = modelo,
                TextoFuente = textoFuente,
                Vector = vector,
                FechaCalculo = fecha
            };
        }

        private static int InsertarProducto(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            int categoriaId = new CategoriaProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.CategoriaProducto { Nombre = "Cat-" + nombre });
            return new ProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.Producto
            {
                CategoriaProducto = new BE.CategoriaProducto { Id = categoriaId },
                Nombre = nombre
            });
        }
    }
}
