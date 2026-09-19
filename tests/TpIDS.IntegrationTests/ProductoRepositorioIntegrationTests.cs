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
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestProductoPropio");
                int categoriaProductoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoPropio");
                int monedaId = InsertarMonedaTest(accesoDatos, "TP1", "MonedaTestProductoPropio");

                int id = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    Comercio = new BE.Comercio { Id = comercioId },
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Moneda = new BE.Moneda { Id = monedaId },
                    Nombre = "ProductoTestPropio",
                    Precio = 100.50m
                });

                BE.Producto obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.IsNotNull(obtenido.Comercio);
                Assert.AreEqual(comercioId, obtenido.Comercio.Id);
                Assert.AreEqual("ComercioTestProductoPropio", obtenido.Comercio.Nombre);
                Assert.IsNotNull(obtenido.CategoriaProducto);
                Assert.AreEqual(categoriaProductoId, obtenido.CategoriaProducto.Id);
                Assert.IsNotNull(obtenido.Moneda);
                Assert.AreEqual(monedaId, obtenido.Moneda.Id);
                Assert.AreEqual("ProductoTestPropio", obtenido.Nombre);
                Assert.AreEqual(100.50m, obtenido.Precio);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorComercio_ExcluyeProductosDeOtroComercio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoRepositorio repositorio = new ProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestProductoFiltroA");
                int otroComercioId = InsertarComercioTest(accesoDatos, "ComercioTestProductoFiltroB");
                int categoriaProductoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoFiltro");
                int monedaId = InsertarMonedaTest(accesoDatos, "TP2", "MonedaTestProductoFiltro");

                int idPropio = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    Comercio = new BE.Comercio { Id = comercioId },
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Moneda = new BE.Moneda { Id = monedaId },
                    Nombre = "ProductoTestComercioA",
                    Precio = 10m
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    Comercio = new BE.Comercio { Id = otroComercioId },
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Moneda = new BE.Moneda { Id = monedaId },
                    Nombre = "ProductoTestComercioB",
                    Precio = 20m
                });

                System.Collections.Generic.List<BE.Producto> propios = repositorio.ObtenerPorComercio(comercioId);

                Assert.IsTrue(propios.Any(item => item.Id == idPropio));
                Assert.IsFalse(propios.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_ProductoExistente_PersisteNuevoPrecio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ProductoRepositorio repositorio = new ProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestProductoOriginal");
                int categoriaProductoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoOriginal");
                int monedaId = InsertarMonedaTest(accesoDatos, "TP3", "MonedaTestProductoOriginal");
                int id = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    Comercio = new BE.Comercio { Id = comercioId },
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Moneda = new BE.Moneda { Id = monedaId },
                    Nombre = "ProductoTestOriginal",
                    Precio = 50m
                });

                bool actualizado = repositorio.Actualizar(new BE.Producto
                {
                    Id = id,
                    Comercio = new BE.Comercio { Id = comercioId },
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Moneda = new BE.Moneda { Id = monedaId },
                    Nombre = "ProductoTestOriginal",
                    Precio = 75.25m
                });

                Assert.IsTrue(actualizado);
                Assert.AreEqual(75.25m, repositorio.ObtenerPorId(id).Precio);

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
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestProductoAEliminar");
                int categoriaProductoId = InsertarCategoriaProductoTest(accesoDatos, "CategoriaTestProductoAEliminar");
                int monedaId = InsertarMonedaTest(accesoDatos, "TP4", "MonedaTestProductoAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.Producto
                {
                    Comercio = new BE.Comercio { Id = comercioId },
                    CategoriaProducto = new BE.CategoriaProducto { Id = categoriaProductoId },
                    Moneda = new BE.Moneda { Id = monedaId },
                    Nombre = "ProductoTestAEliminar",
                    Precio = 30m
                });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarComercioTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            int rubroComercioId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = "Rubro-" + nombre });
            return new ComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.Comercio { RubroComercio = new BE.RubroComercio { Id = rubroComercioId }, Nombre = nombre });
        }

        private static int InsertarCategoriaProductoTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new CategoriaProductoRepositorio(accesoDatos).InsertarYObtenerId(new BE.CategoriaProducto { Nombre = nombre });
        }

        private static int InsertarMonedaTest(AccesoDatosSqlServer accesoDatos, string codigo, string nombre)
        {
            return new MonedaRepositorio(accesoDatos).InsertarYObtenerId(new BE.Moneda { Codigo = codigo, Nombre = nombre });
        }
    }
}
