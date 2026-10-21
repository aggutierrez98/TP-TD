using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ComercioProductoBLLTests
    {
        [TestMethod]
        public void ObtenerPorComercio_DevuelveSoloLasAsignacionesDeEseComercio()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.AgregarAsignacion(1, 1, 100m);
            fixture.AgregarAsignacion(2, 1, 130m);
            ComercioProductoBLL sut = fixture.CreateSut();

            System.Collections.Generic.List<ComercioProducto> propias = sut.ObtenerPorComercio(new Comercio { Id = 1 });

            Assert.AreEqual(1, propias.Count);
            Assert.AreEqual(100m, propias[0].Precio);
        }

        [TestMethod]
        public void ObtenerMonedas_DevuelveElCatalogo()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            ComercioProductoBLL sut = fixture.CreateSut();

            Assert.AreEqual(1, sut.ObtenerMonedas().Count);
        }

        [TestMethod]
        public void Asignar_PrecioInvalido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Asignar(Asignacion(1, 1, 1, 0m)));

            Assert.AreEqual("Error.MerchantManagement.ProductoPriceInvalid", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Asignar_MonedaInexistente_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Asignar(Asignacion(1, 1, 99, 10m)));

            Assert.AreEqual("Error.MerchantManagement.MonedaNotFound", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Asignar_ComercioInexistente_ThrowsExcepcionNegocio()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Asignar(Asignacion(99, 1, 1, 10m)));

            Assert.AreEqual("Error.MerchantManagement.ComercioNotFound", ex.Message);
        }

        [TestMethod]
        public void Asignar_ProductoInexistente_ThrowsExcepcionNegocio()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Asignar(Asignacion(1, 99, 1, 10m)));

            Assert.AreEqual("Error.MerchantManagement.ProductoNotFound", ex.Message);
        }

        [TestMethod]
        public void Asignar_ProductoRepetidoEnElMismoComercio_ThrowsYaAsignadoYNoInserta()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.AgregarAsignacion(1, 1, 100m);
            int asignacionesAntes = fixture.Unidad.ComercioProductoRepo.Asignaciones.Count;
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Asignar(Asignacion(1, 1, 1, 200m)));

            Assert.AreEqual("Error.MerchantManagement.ProductoAlreadyAssigned", ex.Message);
            Assert.AreEqual(asignacionesAntes, fixture.Unidad.ComercioProductoRepo.Asignaciones.Count);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Asignar_Valido_InsertaConPrecioYMonedaAuditaYGuarda()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            ComercioProductoBLL sut = fixture.CreateSut();

            ComercioProducto asignada = sut.Asignar(Asignacion(1, 1, 1, 150.5m));

            ComercioProducto insertada = fixture.Unidad.ComercioProductoRepo.Buscar(1, 1);
            Assert.IsNotNull(insertada);
            Assert.AreEqual(150.5m, insertada.Precio);
            Assert.AreEqual(1, insertada.Moneda.Id);
            Assert.AreEqual(1, asignada.Producto.Id);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Asignar_MismoProductoEnDosComercios_ConservaPrecioPorComercio()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            fixture.Unidad.ComercioRepo.Comercios.Add(2, new Comercio { Id = 2, Nombre = "Coto", RubroComercio = new RubroComercio { Id = 1 } });
            fixture.Unidad.ProductoRepo.Productos.Add(1, new Producto { Id = 1, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            ComercioProductoBLL sut = fixture.CreateSut();

            sut.Asignar(Asignacion(1, 1, 1, 100m));
            sut.Asignar(Asignacion(2, 1, 1, 130m));

            Assert.AreEqual(100m, fixture.Unidad.ComercioProductoRepo.Buscar(1, 1).Precio);
            Assert.AreEqual(130m, fixture.Unidad.ComercioProductoRepo.Buscar(2, 1).Precio);
        }

        [TestMethod]
        public void ActualizarPrecio_PrecioInvalido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarPrecio(Asignacion(1, 1, 1, -5m)));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void ActualizarPrecio_AsignacionInexistente_ThrowsExcepcionNegocio()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarPrecio(Asignacion(1, 1, 1, 10m)));

            Assert.AreEqual("Error.MerchantManagement.ProductoAssignmentNotFound", ex.Message);
        }

        [TestMethod]
        public void ActualizarPrecio_Valido_ActualizaSoloEseComercioAuditaYGuarda()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.AgregarAsignacion(1, 1, 100m);
            fixture.AgregarAsignacion(2, 1, 130m);
            ComercioProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.ActualizarPrecio(Asignacion(1, 1, 1, 175.25m));

            Assert.IsTrue(resultado);
            Assert.AreEqual(175.25m, fixture.Unidad.ComercioProductoRepo.Buscar(1, 1).Precio);
            Assert.AreEqual(130m, fixture.Unidad.ComercioProductoRepo.Buscar(2, 1).Precio);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Quitar_AsignacionInexistente_ThrowsExcepcionNegocio()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Quitar(Asignacion(1, 1, 1, 0m)));

            Assert.AreEqual("Error.MerchantManagement.ProductoAssignmentNotFound", ex.Message);
        }

        [TestMethod]
        public void Quitar_Valido_QuitaDelComercioPeroConservaElProductoAuditaYGuarda()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.AgregarAsignacion(1, 1, 100m);
            ComercioProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.Quitar(Asignacion(1, 1, 1, 0m));

            Assert.IsTrue(resultado);
            Assert.IsNull(fixture.Unidad.ComercioProductoRepo.Buscar(1, 1));
            Assert.IsTrue(fixture.Unidad.ProductoRepo.Productos.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Asignar_Valido_DevuelveLaMismaInstanciaConLasEntidadesPersistidas()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.AgregarAsignacion(1, 1, 100m);
            Comercio comercio = fixture.Unidad.ComercioRepo.Comercios[1];
            Producto producto = new Producto { Id = 2, Nombre = "Arroz", CategoriaProducto = new CategoriaProducto { Id = 1 } };
            fixture.Unidad.ProductoRepo.Productos.Add(2, producto);
            ComercioProductoBLL sut = fixture.CreateSut();
            ComercioProducto asignacion = Asignacion(1, 2, 1, 50m);

            ComercioProducto devuelta = sut.Asignar(asignacion);

            Assert.AreSame(asignacion, devuelta);
            Assert.AreSame(comercio, devuelta.Comercio);
            Assert.AreSame(producto, devuelta.Producto);
            Assert.AreSame(fixture.MonedaRepo.Monedas[1], devuelta.Moneda);
            Assert.AreSame(asignacion, fixture.Unidad.ComercioProductoRepo.LastInserted);
        }

        [TestMethod]
        public void Asignar_AsignacionNula_ThrowsExcepcionNegocioPriceInvalid()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Asignar(null));

            Assert.AreEqual("Error.MerchantManagement.ProductoPriceInvalid", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void ActualizarPrecio_Valido_CopiaSoloElPrecioSobreLaPersistida()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.AgregarAsignacion(1, 1, 100m);
            ComercioProducto persistida = fixture.Unidad.ComercioProductoRepo.Buscar(1, 1);
            Moneda monedaPersistida = persistida.Moneda;
            ComercioProductoBLL sut = fixture.CreateSut();

            sut.ActualizarPrecio(Asignacion(1, 1, 99, 175m));

            Assert.AreSame(persistida, fixture.Unidad.ComercioProductoRepo.LastPrecioActualizado);
            Assert.AreEqual(175m, persistida.Precio);
            Assert.AreSame(monedaPersistida, persistida.Moneda);
        }

        [TestMethod]
        public void ActualizarPrecio_AsignacionNula_ThrowsExcepcionNegocioPriceInvalid()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarPrecio(null));

            Assert.AreEqual("Error.MerchantManagement.ProductoPriceInvalid", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Quitar_AsignacionNula_ThrowsExcepcionNegocioAssignmentNotFound()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            ComercioProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Quitar(null));

            Assert.AreEqual("Error.MerchantManagement.ProductoAssignmentNotFound", ex.Message);
            Assert.IsNull(fixture.Unidad.ComercioProductoRepo.LastDeleted);
        }

        [TestMethod]
        public void Quitar_Valido_EliminaLaPersistida()
        {
            ComercioProductoFixture fixture = new ComercioProductoFixture();
            fixture.AgregarAsignacion(1, 1, 100m);
            ComercioProducto persistida = fixture.Unidad.ComercioProductoRepo.Buscar(1, 1);
            ComercioProductoBLL sut = fixture.CreateSut();

            sut.Quitar(Asignacion(1, 1, 1, 0m));

            Assert.AreSame(persistida, fixture.Unidad.ComercioProductoRepo.LastDeleted);
        }

        // La asignacion como la arma la UI: entidades con su Id y el precio ingresado.
        private static ComercioProducto Asignacion(int comercioId, int productoId, int monedaId, decimal precio)
        {
            return new ComercioProducto
            {
                Comercio = new Comercio { Id = comercioId },
                Producto = new Producto { Id = productoId },
                Moneda = new Moneda { Id = monedaId },
                Precio = precio
            };
        }

        private sealed class ComercioProductoFixture
        {
            public readonly FakeMonedaRepositorio MonedaRepo = new FakeMonedaRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public ComercioProductoFixture()
            {
                Unidad = new FakeUnidadDeTrabajo();
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            // Deja un comercio, el producto 1 y la asignacion lista para usar en un test.
            public void AgregarAsignacion(int comercioId, int productoId, decimal precio)
            {
                if (!Unidad.ComercioRepo.Comercios.ContainsKey(comercioId))
                {
                    Unidad.ComercioRepo.Comercios.Add(comercioId, new Comercio { Id = comercioId, Nombre = "Comercio" + comercioId, RubroComercio = new RubroComercio { Id = 1 } });
                }

                if (!Unidad.ProductoRepo.Productos.ContainsKey(productoId))
                {
                    Unidad.ProductoRepo.Productos.Add(productoId, new Producto { Id = productoId, Nombre = "Producto" + productoId, CategoriaProducto = new CategoriaProducto { Id = 1 } });
                }

                if (!MonedaRepo.Monedas.ContainsKey(1))
                {
                    MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso" });
                }

                Unidad.ComercioProductoRepo.Asignaciones.Add(new ComercioProducto
                {
                    Comercio = Unidad.ComercioRepo.Comercios[comercioId],
                    Producto = Unidad.ProductoRepo.Productos[productoId],
                    Moneda = MonedaRepo.Monedas[1],
                    Precio = precio
                });
            }

            public ComercioProductoBLL CreateSut()
            {
                return new ComercioProductoBLL(Unidad.ComercioProductoRepo, MonedaRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
