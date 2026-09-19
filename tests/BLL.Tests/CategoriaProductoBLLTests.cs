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
            Assert.IsTrue(fixture.Unidad.SaveCalled);
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
        public void Eliminar_ConProductosAsociados_ThrowsExcepcionNegocio()
        {
            // No hay validacion proactiva (no se expone ObtenerPorCategoriaProducto): se simula
            // la violacion de FK que la base real levantaria (FK_Producto_CategoriaProducto).
            CategoriaProductoFixture fixture = new CategoriaProductoFixture();
            fixture.Unidad.CategoriaProductoRepo.Categorias.Add(1, new CategoriaProducto { Id = 1, Nombre = "Lacteos" });
            fixture.Unidad.CategoriaProductoRepo.ThrowForeignKeyOnEliminar = true;
            CategoriaProductoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));
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
