using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ProductoBLLTests
    {
        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, "   ", 1, 1, 10m));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_PrecioInvalido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, "Leche", 1, 1, 0m));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_MonedaInexistente_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, "Leche", 1, 99, 10m));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_ComercioInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(99, "Leche", 1, 1, 10m));
        }

        [TestMethod]
        public void Crear_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, "Leche", 99, 1, 10m));
        }

        [TestMethod]
        public void Crear_Valido_InsertaAuditaYGuarda()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            ProductoBLL sut = fixture.CreateSut();

            Producto creado = sut.Crear(1, "  Leche  ", 1, 1, 150.5m);

            Producto insertado = fixture.Unidad.ProductoRepo.Productos[creado.Id];
            Assert.AreEqual("Leche", insertado.Nombre);
            Assert.AreEqual(1, insertado.Comercio.Id);
            Assert.AreEqual(1, insertado.CategoriaProducto.Id);
            Assert.AreEqual(1, insertado.Moneda.Id);
            Assert.AreEqual(150.5m, insertado.Precio);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_ProductoInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(99, "Nuevo", 1, 10m));
        }

        [TestMethod]
        public void Actualizar_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            Moneda moneda = new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" };
            Comercio comercio = new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } };
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", Comercio = comercio, Moneda = moneda, CategoriaProducto = new CategoriaProducto { Id = 1 }, Precio = 100m });
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(1, "Leche", 99, 100m));
        }

        [TestMethod]
        public void Actualizar_Valido_NoCambiaComercioNiMonedaActualizaAuditaYGuarda()
        {
            ProductoFixture fixture = new ProductoFixture();
            Moneda moneda = new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" };
            Comercio comercio = new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } };
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", Comercio = comercio, Moneda = moneda, CategoriaProducto = new CategoriaProducto { Id = 1 }, Precio = 100m });
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(2, new CategoriaProducto { Id = 2, Nombre = "Bebidas" });
            ProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(1, "  Leche Entera  ", 2, 200m);

            Assert.IsTrue(resultado);
            Producto actualizado = fixture.Unidad.ProductoRepo.Productos[1];
            Assert.AreEqual("Leche Entera", actualizado.Nombre);
            Assert.AreEqual(2, actualizado.CategoriaProducto.Id);
            Assert.AreEqual(200m, actualizado.Precio);
            Assert.AreEqual(1, actualizado.Comercio.Id);
            Assert.AreEqual(1, actualizado.Moneda.Id);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ProductoInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(99));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", Comercio = new Comercio { Id = 1 }, Moneda = new Moneda { Id = 1 }, CategoriaProducto = new CategoriaProducto { Id = 1 }, Precio = 100m });
            ProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(1);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.ProductoRepo.Productos.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        private sealed class ProductoFixture
        {
            public readonly FakeProductoRepositorio ProductoRepo = new FakeProductoRepositorio();
            public readonly FakeMonedaRepositorio MonedaRepo = new FakeMonedaRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public ProductoFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { ProductoRepo = ProductoRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public ProductoBLL CreateSut()
            {
                return new ProductoBLL(ProductoRepo, MonedaRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
