using System.Linq;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class CategoriaBLLTests
    {
        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("   "));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_NombreDuplicadoEnScopePropio_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "comida", UsuarioId = 1 });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Comida"));
        }

        [TestMethod]
        public void Crear_Valido_InsertaComoPropiaDelUsuarioActualAuditaYGuarda()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 7);
            CategoriaBLL sut = fixture.CreateSut();

            int id = sut.Crear("  Ocio  ");

            Categoria insertada = fixture.Unidad.CategoriaRepo.Categorias[id];
            Assert.AreEqual("Ocio", insertada.Nombre);
            Assert.AreEqual(7, insertada.UsuarioId);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(99, "Nuevo"));
        }

        [TestMethod]
        public void Actualizar_CategoriaGlobal_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Alimentacion", UsuarioId = null });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(1, "Nuevo"));
        }

        [TestMethod]
        public void Actualizar_CategoriaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Ajena", UsuarioId = 99 });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(2, "Nuevo"));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 5);
            fixture.Unidad.CategoriaRepo.Categorias.Add(3, new Categoria { Id = 3, Nombre = "Viejo", UsuarioId = 5 });
            CategoriaBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(3, "  Nuevo Nombre  ");

            Assert.IsTrue(resultado);
            Assert.AreEqual("Nuevo Nombre", fixture.Unidad.CategoriaRepo.Categorias[3].Nombre);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_CategoriaGlobal_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Alimentacion", UsuarioId = null });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));
        }

        [TestMethod]
        public void Eliminar_CategoriaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Ajena", UsuarioId = 99 });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(2));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 5);
            fixture.Unidad.CategoriaRepo.Categorias.Add(3, new Categoria { Id = 3, Nombre = "Propia", UsuarioId = 5 });
            CategoriaBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(3);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.CategoriaRepo.Categorias.ContainsKey(3));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerVisiblesParaUsuario_DelegaAlRepositorio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", UsuarioId = null });
            fixture.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Propia", UsuarioId = 5 });
            fixture.CategoriaRepo.Categorias.Add(3, new Categoria { Id = 3, Nombre = "Ajena", UsuarioId = 99 });
            CategoriaBLL sut = fixture.CreateSut();

            System.Collections.Generic.List<Categoria> visibles = sut.ObtenerVisiblesParaUsuario(5);

            Assert.AreEqual(2, visibles.Count);
        }

        private sealed class CategoriaFixture
        {
            public readonly FakeCategoriaRepositorio CategoriaRepo = new FakeCategoriaRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public CategoriaFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo { CategoriaRepo = CategoriaRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = usuarioActualId, Username = "test" });
            }

            public CategoriaBLL CreateSut()
            {
                return new CategoriaBLL(CategoriaRepo, UnidadFactory, Sesion, Bitacora);
            }
        }
    }
}
