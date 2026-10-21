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
        public void ObtenerTodos_DevuelveElCatalogoSinImportarComercio()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            fixture.ProductoRepo.Productos.Add(2, new Producto { Id = 2, Nombre = "Arroz", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            ProductoBLL sut = fixture.CreateSut();

            System.Collections.Generic.List<Producto> productos = sut.ObtenerTodos();

            Assert.AreEqual(2, productos.Count);
        }

        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new Producto { Nombre = "   ", CategoriaProducto = new CategoriaProducto { Id = 1 } }));

            Assert.AreEqual("Error.MerchantManagement.ProductoNameRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new Producto { Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 99 } }));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoNotFound", ex.Message);
        }

        [TestMethod]
        public void Crear_Valido_InsertaSinComercioAuditaYGuarda()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            ProductoBLL sut = fixture.CreateSut();

            Producto creado = sut.Crear(new Producto { Nombre = "  Leche  ", CategoriaProducto = new CategoriaProducto { Id = 1 } });

            Producto insertado = fixture.Unidad.ProductoRepo.Productos[creado.Id];
            Assert.AreEqual("Leche", insertado.Nombre);
            Assert.AreEqual(1, insertado.CategoriaProducto.Id);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_ProductoInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new Producto { Id = 99, Nombre = "Nuevo", CategoriaProducto = new CategoriaProducto { Id = 1 } }));

            Assert.AreEqual("Error.MerchantManagement.ProductoNotFound", ex.Message);
        }

        [TestMethod]
        public void Actualizar_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 99 } }));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoNotFound", ex.Message);
        }

        [TestMethod]
        public void Actualizar_Valido_CambiaNombreYCategoriaAuditaYGuarda()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(2, new CategoriaProducto { Id = 2, Nombre = "Bebidas" });
            ProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(new Producto { Id = 1, Nombre = "  Leche Entera  ", CategoriaProducto = new CategoriaProducto { Id = 2 } });

            Assert.IsTrue(resultado);
            Producto actualizado = fixture.Unidad.ProductoRepo.Productos[1];
            Assert.AreEqual("Leche Entera", actualizado.Nombre);
            Assert.AreEqual(2, actualizado.CategoriaProducto.Id);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ProductoInexistente_ThrowsExcepcionNegocio()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Producto { Id = 99 }));

            Assert.AreEqual("Error.MerchantManagement.ProductoNotFound", ex.Message);
        }

        [TestMethod]
        public void Eliminar_ProductoVendidoPorUnComercio_ThrowsProductoInUseYNoElimina()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            fixture.Unidad.ComercioProductoRepo.Asignaciones.Add(new ComercioProducto
            {
                Comercio = new Comercio { Id = 1 },
                Producto = new Producto { Id = 1 },
                Moneda = new Moneda { Id = 1 },
                Precio = 100m
            });
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Producto { Id = 1 }));

            Assert.AreEqual("Error.MerchantManagement.ProductoInUse", ex.Message);
            Assert.IsTrue(fixture.Unidad.ProductoRepo.Productos.ContainsKey(1));
            Assert.IsNull(fixture.Unidad.ProductoRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ProductoSinUso_EliminaAuditaYGuarda()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            ProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(new Producto { Id = 1 });

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.ProductoRepo.Productos.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_IncluidoPorUnaPromocion_ThrowsProductoInUseYNoElimina()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            fixture.Unidad.PromocionProductoRepo.IncluidosPorPromocion[7] = new System.Collections.Generic.List<int> { 1 };
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Producto { Id = 1 }));

            Assert.AreEqual("Error.MerchantManagement.ProductoInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.ProductoRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ExcluidoPorUnaPromocion_ThrowsProductoInUseYNoElimina()
        {
            ProductoFixture fixture = new ProductoFixture();
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            fixture.Unidad.PromocionProductoRepo.ExcluidosPorPromocion[7] = new System.Collections.Generic.List<int> { 1 };
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Producto { Id = 1 }));

            Assert.AreEqual("Error.MerchantManagement.ProductoInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.ProductoRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_Valido_DevuelveLaMismaInstanciaConIdYCategoriaPersistida()
        {
            ProductoFixture fixture = new ProductoFixture();
            CategoriaProducto categoriaPersistida = new CategoriaProducto { Id = 1, Nombre = "Lacteos" };
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, categoriaPersistida);
            ProductoBLL sut = fixture.CreateSut();
            Producto producto = new Producto { Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } };

            Producto creado = sut.Crear(producto);

            Assert.AreSame(producto, creado);
            Assert.IsTrue(creado.Id > 0);
            Assert.AreSame(categoriaPersistida, creado.CategoriaProducto);
        }

        [TestMethod]
        public void Crear_ProductoNulo_ThrowsExcepcionNegocioNameRequired()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(null));

            Assert.AreEqual("Error.MerchantManagement.ProductoNameRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_Valido_CopiaLosCamposSobreElPersistidoConLaCategoriaPersistida()
        {
            ProductoFixture fixture = new ProductoFixture();
            CategoriaProducto categoriaPersistida = new CategoriaProducto { Id = 2, Nombre = "Almacen" };
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(2, categoriaPersistida);
            Producto persistido = new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } };
            fixture.ProductoRepo.Productos.Add(1, persistido);
            ProductoBLL sut = fixture.CreateSut();

            sut.Actualizar(new Producto { Id = 1, Nombre = "Arroz", CategoriaProducto = new CategoriaProducto { Id = 2 } });

            Assert.AreSame(persistido, fixture.ProductoRepo.LastUpdated);
            Assert.AreEqual("Arroz", persistido.Nombre);
            Assert.AreSame(categoriaPersistida, persistido.CategoriaProducto);
        }

        [TestMethod]
        public void Eliminar_ProductoNulo_ThrowsExcepcionNegocioNotFound()
        {
            ProductoFixture fixture = new ProductoFixture();
            ProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(null));

            Assert.AreEqual("Error.MerchantManagement.ProductoNotFound", ex.Message);
            Assert.IsNull(fixture.ProductoRepo.LastDeleted);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaElPersistido()
        {
            ProductoFixture fixture = new ProductoFixture();
            Producto persistido = new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } };
            fixture.ProductoRepo.Productos.Add(1, persistido);
            ProductoBLL sut = fixture.CreateSut();

            sut.Eliminar(new Producto { Id = 1 });

            Assert.AreSame(persistido, fixture.ProductoRepo.LastDeleted);
        }

        private sealed class ProductoFixture
        {
            public readonly FakeProductoRepositorio ProductoRepo = new FakeProductoRepositorio();
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
                return new ProductoBLL(ProductoRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
