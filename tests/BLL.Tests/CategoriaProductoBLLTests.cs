using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class CategoriaProductoBLLTests
    {
        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            CategoriaProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("   "));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_Valido_InsertaAuditaYGuarda()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            CategoriaProductoBLL sut = fixture.CreateSut();

            CategoriaProducto creada = sut.Crear("  Lacteos  ");

            CategoriaProducto insertada = fixture.Unidad.CategoriaProductoRepo.Categorias[creada.Id];
            Assert.AreEqual("Lacteos", insertada.Nombre);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.AreEqual("FormProductCategoryManagement.Value.Activity.Creation", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreEqual("FormProductCategoryManagement.Value.Info.Creation", fixture.Bitacora.Calls[0].Informacion);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_NombreRepetidoConOtrasMayusculas_ThrowsExcepcionNegocioYNoInserta()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lácteos" });
            CategoriaProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("  LÁCTEOS  "));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoDuplicateName", ex.Message);
            Assert.AreEqual(1, fixture.CategoriaProductoRepo.Categorias.Count);
            Assert.AreEqual(0, fixture.Bitacora.Calls.Count);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_NombreSinAcentoDistintoDeOtro_PermiteAmbasCategorias()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            CategoriaProductoBLL sut = fixture.CreateSut();
            sut.Crear("Lácteos");

            CategoriaProducto creada = sut.Crear("Lacteos");

            Assert.AreEqual("Lacteos", creada.Nombre);
            Assert.AreEqual(2, fixture.CategoriaProductoRepo.Categorias.Count);
        }

        [TestMethod]
        public void Actualizar_Inexistente_ThrowsExcepcionNegocio()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            CategoriaProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(99, "Nuevo"));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Viejo" });
            CategoriaProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(1, "  Nueva Categoria  ");

            Assert.IsTrue(resultado);
            Assert.AreEqual("Nueva Categoria", fixture.Unidad.CategoriaProductoRepo.Categorias[1].Nombre);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.AreEqual("FormProductCategoryManagement.Value.Activity.Update", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreEqual("FormProductCategoryManagement.Value.Info.Update", fixture.Bitacora.Calls[0].Informacion);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_NombreDeOtraCategoria_ThrowsExcepcionNegocioYConservaNombre()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lácteos" });
            fixture.CategoriaProductoRepo.Categorias.Add(2, new CategoriaProducto { Id = 2, Nombre = "Bebidas" });
            CategoriaProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(2, "  LÁCTEOS  "));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoDuplicateName", ex.Message);
            Assert.AreEqual("Bebidas", fixture.CategoriaProductoRepo.Categorias[2].Nombre);
            Assert.AreEqual(0, fixture.Bitacora.Calls.Count);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_ConservandoNombrePropio_PermiteActualizacionYAudita()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lácteos" });
            CategoriaProductoBLL sut = fixture.CreateSut();

            Assert.IsTrue(sut.Actualizar(1, "  LÁCTEOS  "));

            Assert.AreEqual("LÁCTEOS", fixture.CategoriaProductoRepo.Categorias[1].Nombre);
            Assert.AreEqual("FormProductCategoryManagement.Value.Activity.Update", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreEqual("FormProductCategoryManagement.Value.Info.Update", fixture.Bitacora.Calls[0].Informacion);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Inexistente_ThrowsExcepcionNegocio()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            CategoriaProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(99));
        }

        [TestMethod]
        public void Eliminar_ConProductosAsociados_ThrowsExcepcionNegocioYNoElimina()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            fixture.Unidad.ProductoRepo.Productos.Add(5, new Producto { Id = 5, Nombre = "Leche", CategoriaProducto = new CategoriaProducto { Id = 1 } });
            CategoriaProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.CategoriaProductoRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaDelBorradoConViolacionForanea_InformaDeleteFailed()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            fixture.Unidad.CategoriaProductoRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForanea();
            CategoriaProductoBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoDeleteFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaTecnicaConViolacionForaneaEnvuelta_NoSeConvierteEnUso()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            fixture.Unidad.CategoriaProductoRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForaneaEnvuelta();
            CategoriaProductoBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.Technical.SqlFailure", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            CategoriaProductoBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(1);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.CategoriaProductoRepo.Categorias.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.AreEqual("FormProductCategoryManagement.Value.Activity.Deletion", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreEqual("FormProductCategoryManagement.Value.Info.Deletion", fixture.Bitacora.Calls[0].Informacion);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerTodas_DelegaAlRepositorio()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            fixture.CategoriaProductoRepo.Categorias.Add(2, new CategoriaProducto { Id = 2, Nombre = "Bebidas" });
            CategoriaProductoBLL sut = fixture.CreateSut();

            List<CategoriaProducto> todas = sut.ObtenerTodas();

            Assert.AreEqual(2, todas.Count);
        }

        [TestMethod]
        public void Eliminar_IncluidaPorUnaPromocion_ThrowsExcepcionNegocioYNoElimina()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            fixture.Unidad.PromocionCategoriaProductoRepo.IncluidasPorPromocion[7] = new System.Collections.Generic.List<int> { 1 };
            CategoriaProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.CategoriaProductoRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ExcluidaPorUnaPromocion_ThrowsExcepcionNegocioYNoElimina()
        {
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            fixture.Unidad.PromocionCategoriaProductoRepo.ExcluidasPorPromocion[7] = new System.Collections.Generic.List<int> { 1 };
            CategoriaProductoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.MerchantManagement.CategoriaProductoInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.CategoriaProductoRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        private sealed class CategoriaProductoFixture
        {
            public readonly FakeCategoriaProductoRepositorio CategoriaProductoRepo = new FakeCategoriaProductoRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public CategoriaProductoFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { CategoriaProductoRepo = CategoriaProductoRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public CategoriaProductoBLL CreateSut()
            {
                return new CategoriaProductoBLL(CategoriaProductoRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
