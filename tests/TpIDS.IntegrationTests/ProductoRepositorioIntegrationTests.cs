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
    public sealed class ProductoRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_ProductoValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoRepositorio repositorio = new ProductoRepositorio(accesoDatos);
                int categoriaProductoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoPropio");

                int id = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Nombre = "ProductoTestPropio"
                });

                BE.Producto obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.IsNotNull(obtenido.CategoriaProducto);
                Assert.AreEqual(categoriaProductoId, obtenido.CategoriaProducto.Id);
                Assert.AreEqual("CategoriaTestProductoPropio", obtenido.CategoriaProducto.Nombre);
                Assert.AreEqual("ProductoTestPropio", obtenido.Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodos_IncluyeProductosInsertadosSinImportarComercio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoRepositorio repositorio = new ProductoRepositorio(accesoDatos);
                int categoriaProductoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoTodos");

                int idA = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Nombre = "ProductoTestTodosA"
                });
                int idB = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Nombre = "ProductoTestTodosB"
                });

                System.Collections.Generic.List<BE.Producto> todos = repositorio.ObtenerTodos();

                Assert.IsTrue(todos.Any(item => item.Id == idA));
                Assert.IsTrue(todos.Any(item => item.Id == idB));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_ProductoExistente_PersisteNuevoNombreYCategoria()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoRepositorio repositorio = new ProductoRepositorio(accesoDatos);
                int categoriaOriginalId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoOriginal");
                int categoriaNuevaId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoNueva");
                int id = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaOriginalId },
                    Nombre = "ProductoTestOriginal"
                });

                bool actualizado = repositorio.Actualizar(new BE.Producto
                {
                    Id = id,
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaNuevaId },
                    Nombre = "ProductoTestRenombrado"
                });

                Assert.IsTrue(actualizado);
                BE.Producto obtenido = repositorio.ObtenerPorId(id);
                Assert.AreEqual("ProductoTestRenombrado", obtenido.Nombre);
                Assert.AreEqual(categoriaNuevaId, obtenido.CategoriaProducto.Id);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_ProductoExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoRepositorio repositorio = new ProductoRepositorio(accesoDatos);
                int categoriaProductoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Nombre = "ProductoTestAEliminar"
                });

                bool eliminado = repositorio.Eliminar(new BE.Producto { Id = id });

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorCategoria_CuentaLosProductosDeEsaCategoria()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoRepositorio repositorio = new ProductoRepositorio(accesoDatos);
                int categoriaConProductosId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestContarConProductos");
                int categoriaSinUsoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestContarSinProductos");
                repositorio.InsertarYObtenerId(new BE.Producto { CategoriaProducto = new BE.CategoriaProducto { Id = categoriaConProductosId }, Nombre = "ProductoTestContarCategoriaA" });
                repositorio.InsertarYObtenerId(new BE.Producto { CategoriaProducto = new BE.CategoriaProducto { Id = categoriaConProductosId }, Nombre = "ProductoTestContarCategoriaB" });

                Assert.AreEqual(2, repositorio.ContarPorCategoria(new BE.CategoriaProducto { Id = categoriaConProductosId }));
                Assert.AreEqual(0, repositorio.ContarPorCategoria(new BE.CategoriaProducto { Id = categoriaSinUsoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarCategoriaProductoTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new CategoriaProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.CategoriaProducto { Nombre = nombre });
        }
    }
}
