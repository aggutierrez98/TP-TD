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
    public sealed class ComercioProductoRepositorioIntegrationTests
    {
        [TestMethod]
        public void Insertar_AsignacionValida_QuedaDisponibleConPrecioYMoneda()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestAsignacionPropia");
                int productoId = InsertarProductoTest(accesoDatos, "ProductoTestAsignacionPropia");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA1", "MonedaTestAsignacionPropia");

                bool insertado = repositorio.Insertar(Crear(comercioId, productoId, monedaId, 100.50m));

                Assert.IsTrue(insertado);
                BE.ComercioProducto obtenido = repositorio.ObtenerPorComercioYProducto(new BE.Comercio { Id = comercioId }, new BE.Producto { Id = productoId });
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(comercioId, obtenido.Comercio.Id);
                Assert.AreEqual("ComercioTestAsignacionPropia", obtenido.Comercio.Nombre);
                Assert.AreEqual(productoId, obtenido.Producto.Id);
                Assert.AreEqual("ProductoTestAsignacionPropia", obtenido.Producto.Nombre);
                Assert.AreEqual(monedaId, obtenido.Moneda.Id);
                Assert.AreEqual(100.50m, obtenido.Precio);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorComercio_ExcluyeAsignacionesDeOtroComercio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestFiltroA");
                int otroComercioId = InsertarComercioTest(accesoDatos, "ComercioTestFiltroB");
                int productoPropioId = InsertarProductoTest(accesoDatos, "ProductoTestFiltroPropio");
                int productoAjenoId = InsertarProductoTest(accesoDatos, "ProductoTestFiltroAjeno");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA2", "MonedaTestFiltro");
                repositorio.Insertar(Crear(comercioId, productoPropioId, monedaId, 10m));
                repositorio.Insertar(Crear(otroComercioId, productoAjenoId, monedaId, 20m));

                System.Collections.Generic.List<BE.ComercioProducto> propios = repositorio.ObtenerPorComercio(new BE.Comercio { Id = comercioId });

                Assert.IsTrue(propios.Any(item => item.Producto.Id == productoPropioId));
                Assert.IsFalse(propios.Any(item => item.Producto.Id == productoAjenoId));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_MismoProductoEnDosComercios_GuardaPrecioPorComercio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioAId = InsertarComercioTest(accesoDatos, "ComercioTestPrecioA");
                int comercioBId = InsertarComercioTest(accesoDatos, "ComercioTestPrecioB");
                int productoId = InsertarProductoTest(accesoDatos, "ProductoTestPrecioPorComercio");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA3", "MonedaTestPrecio");

                repositorio.Insertar(Crear(comercioAId, productoId, monedaId, 100m));
                repositorio.Insertar(Crear(comercioBId, productoId, monedaId, 130m));

                Assert.AreEqual(100m, repositorio.ObtenerPorComercioYProducto(new BE.Comercio { Id = comercioAId }, new BE.Producto { Id = productoId }).Precio);
                Assert.AreEqual(130m, repositorio.ObtenerPorComercioYProducto(new BE.Comercio { Id = comercioBId }, new BE.Producto { Id = productoId }).Precio);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_ProductoRepetidoEnElMismoComercio_Falla()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestDuplicado");
                int productoId = InsertarProductoTest(accesoDatos, "ProductoTestDuplicado");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA4", "MonedaTestDuplicado");
                repositorio.Insertar(Crear(comercioId, productoId, monedaId, 50m));

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() => repositorio.Insertar(Crear(comercioId, productoId, monedaId, 60m)));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_PrecioNoPositivo_Falla()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestPrecioCero");
                int productoId = InsertarProductoTest(accesoDatos, "ProductoTestPrecioCero");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA5", "MonedaTestPrecioCero");

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() => repositorio.Insertar(Crear(comercioId, productoId, monedaId, 0m)));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ActualizarPrecio_AsignacionExistente_PersisteNuevoPrecioSoloEnEseComercio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioAId = InsertarComercioTest(accesoDatos, "ComercioTestActualizarA");
                int comercioBId = InsertarComercioTest(accesoDatos, "ComercioTestActualizarB");
                int productoId = InsertarProductoTest(accesoDatos, "ProductoTestActualizar");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA6", "MonedaTestActualizar");
                repositorio.Insertar(Crear(comercioAId, productoId, monedaId, 50m));
                repositorio.Insertar(Crear(comercioBId, productoId, monedaId, 80m));

                bool actualizado = repositorio.ActualizarPrecio(Crear(comercioAId, productoId, monedaId, 75.25m));

                Assert.IsTrue(actualizado);
                Assert.AreEqual(75.25m, repositorio.ObtenerPorComercioYProducto(new BE.Comercio { Id = comercioAId }, new BE.Producto { Id = productoId }).Precio);
                Assert.AreEqual(80m, repositorio.ObtenerPorComercioYProducto(new BE.Comercio { Id = comercioBId }, new BE.Producto { Id = productoId }).Precio);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_AsignacionExistente_DejaDeExistirPeroConservaElProducto()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                ProductoRepositorio productos = new ProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestQuitar");
                int productoId = InsertarProductoTest(accesoDatos, "ProductoTestQuitar");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA7", "MonedaTestQuitar");
                repositorio.Insertar(Crear(comercioId, productoId, monedaId, 30m));

                bool eliminado = repositorio.Eliminar(Crear(comercioId, productoId, monedaId, 30m));

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorComercioYProducto(new BE.Comercio { Id = comercioId }, new BE.Producto { Id = productoId }));
                Assert.IsNotNull(productos.ObtenerPorId(productoId));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorProducto_CuentaLosComerciosQueLoVenden()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioAId = InsertarComercioTest(accesoDatos, "ComercioTestContarA");
                int comercioBId = InsertarComercioTest(accesoDatos, "ComercioTestContarB");
                int productoVendidoId = InsertarProductoTest(accesoDatos, "ProductoTestContarVendido");
                int productoSinUsoId = InsertarProductoTest(accesoDatos, "ProductoTestContarSinUso");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA8", "MonedaTestContar");
                repositorio.Insertar(Crear(comercioAId, productoVendidoId, monedaId, 10m));
                repositorio.Insertar(Crear(comercioBId, productoVendidoId, monedaId, 12m));

                Assert.AreEqual(2, repositorio.ContarPorProducto(new BE.Producto { Id = productoVendidoId }));
                Assert.AreEqual(0, repositorio.ContarPorProducto(new BE.Producto { Id = productoSinUsoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        // Test de caracterizacion: la FK ComercioProducto -> Comercio es lo que hace que un
        // comercio con productos asignados no se pueda eliminar (nace verde a proposito).
        [TestMethod]
        public void EliminarComercio_ConProductoAsignado_Falla()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioProductoRepositorio repositorio = new ComercioProductoRepositorio(accesoDatos);
                int comercioId = InsertarComercioTest(accesoDatos, "ComercioTestConProducto");
                int productoId = InsertarProductoTest(accesoDatos, "ProductoTestDeComercio");
                int monedaId = InsertarMonedaTest(accesoDatos, "TA9", "MonedaTestComercioConProducto");
                repositorio.Insertar(Crear(comercioId, productoId, monedaId, 10m));

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() => new ComercioRepositorio(accesoDatos).Eliminar(new BE.Comercio { Id = comercioId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static BE.ComercioProducto Crear(int comercioId, int productoId, int monedaId, decimal precio)
        {
            return new BE.ComercioProducto
            {
                Comercio = new BE.Comercio { Id = comercioId },
                Producto = new BE.Producto { Id = productoId },
                Moneda = new BE.Moneda { Id = monedaId },
                Precio = precio
            };
        }

        private static int InsertarComercioTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            int rubroComercioId = new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = "Rubro-" + nombre });
            return new ComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.Comercio { RubroComercio = new BE.RubroComercio { Id = rubroComercioId }, Nombre = nombre });
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

        private static int InsertarMonedaTest(AccesoDatosSqlServer accesoDatos, string codigo, string nombre)
        {
            return new MonedaRepositorio(accesoDatos).InsertarYObtenerId(new BE.Moneda { Codigo = codigo, Nombre = nombre });
        }
    }
}
