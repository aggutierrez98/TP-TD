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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new Categoria { Nombre = "   " }));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_NombreDuplicadoEnScopePropio_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "comida", Usuario = new Usuario { Id = 1 } });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new Categoria { Nombre = "Comida" }));
        }

        [TestMethod]
        public void Crear_Valido_InsertaComoPropiaDelUsuarioActualAuditaYGuarda()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 7);
            CategoriaBLL sut = fixture.CreateSut();

            Categoria creada = sut.Crear(new Categoria { Nombre = "  Ocio  " });

            Categoria insertada = fixture.Unidad.CategoriaRepo.Categorias[creada.Id];
            Assert.AreEqual("Ocio", insertada.Nombre);
            Assert.AreEqual(7, insertada.Usuario.Id);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new Categoria { Id = 99, Nombre = "Nuevo" }));
        }

        [TestMethod]
        public void Actualizar_CategoriaGlobal_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Alimentacion", Usuario = null });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new Categoria { Id = 1, Nombre = "Nuevo" }));
        }

        [TestMethod]
        public void Actualizar_CategoriaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new Categoria { Id = 2, Nombre = "Nuevo" }));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 5);
            fixture.Unidad.CategoriaRepo.Categorias.Add(3, new Categoria { Id = 3, Nombre = "Viejo", Usuario = new Usuario { Id = 5 } });
            CategoriaBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(new Categoria { Id = 3, Nombre = "  Nuevo Nombre  " });

            Assert.IsTrue(resultado);
            Assert.AreEqual("Nuevo Nombre", fixture.Unidad.CategoriaRepo.Categorias[3].Nombre);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_CategoriaGlobal_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Alimentacion", Usuario = null });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Categoria { Id = 1 }));
        }

        [TestMethod]
        public void Eliminar_CategoriaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            CategoriaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Categoria { Id = 2 }));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 5);
            fixture.Unidad.CategoriaRepo.Categorias.Add(3, new Categoria { Id = 3, Nombre = "Propia", Usuario = new Usuario { Id = 5 } });
            CategoriaBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(new Categoria { Id = 3 });

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.CategoriaRepo.Categorias.ContainsKey(3));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerVisiblesParaUsuarioActual_DelegaAlRepositorioConElUsuarioDeSesion()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 5);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Propia", Usuario = new Usuario { Id = 5 } });
            fixture.CategoriaRepo.Categorias.Add(3, new Categoria { Id = 3, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            CategoriaBLL sut = fixture.CreateSut();

            System.Collections.Generic.List<Categoria> visibles = sut.ObtenerVisiblesParaUsuarioActual();

            Assert.AreEqual(2, visibles.Count);
        }

        [TestMethod]
        public void Crear_Valido_DevuelveLaMismaInstanciaConIdYUsuarioDeSesion()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 7);
            CategoriaBLL sut = fixture.CreateSut();
            Categoria categoria = new Categoria { Nombre = "Ocio" };

            Categoria creada = sut.Crear(categoria);

            Assert.AreSame(categoria, creada);
            Assert.IsTrue(creada.Id > 0);
            Assert.AreSame(fixture.UsuarioActual, creada.Usuario);
        }

        [TestMethod]
        public void Crear_CategoriaNula_ThrowsExcepcionNegocioNameRequired()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            CategoriaBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(null));
            Assert.AreEqual("Error.Category.NameRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_UsuarioAdulteradoEnLaEntidadRecibida_ValidaPropiedadContraLaPersistida()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            CategoriaBLL sut = fixture.CreateSut();
            Categoria adulterada = new Categoria { Id = 2, Nombre = "Nuevo", Usuario = fixture.UsuarioActual };

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(adulterada));

            Assert.AreEqual("Error.Category.NotOwned", ex.Message);
            Assert.AreEqual("Ajena", fixture.Unidad.CategoriaRepo.Categorias[2].Nombre);
            Assert.IsNull(fixture.Unidad.CategoriaRepo.LastUpdated);
        }

        [TestMethod]
        public void Actualizar_Valido_CopiaSoloElNombreSobreLaPersistida()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 5);
            Categoria persistida = new Categoria { Id = 3, Nombre = "Viejo", Usuario = new Usuario { Id = 5 } };
            fixture.Unidad.CategoriaRepo.Categorias.Add(3, persistida);
            CategoriaBLL sut = fixture.CreateSut();
            Categoria recibida = new Categoria { Id = 3, Nombre = "Nuevo", Usuario = new Usuario { Id = 99 } };

            sut.Actualizar(recibida);

            Assert.AreSame(persistida, fixture.Unidad.CategoriaRepo.LastUpdated);
            Assert.AreEqual("Nuevo", persistida.Nombre);
            Assert.AreEqual(5, persistida.Usuario.Id);
        }

        [TestMethod]
        public void Eliminar_UsuarioAdulteradoEnLaEntidadRecibida_ValidaPropiedadContraLaPersistida()
        {
            CategoriaFixture fixture = new CategoriaFixture(usuarioActualId: 1);
            fixture.Unidad.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            CategoriaBLL sut = fixture.CreateSut();
            Categoria adulterada = new Categoria { Id = 2, Nombre = "Ajena", Usuario = fixture.UsuarioActual };

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(adulterada));

            Assert.AreEqual("Error.Category.NotOwned", ex.Message);
            Assert.IsTrue(fixture.Unidad.CategoriaRepo.Categorias.ContainsKey(2));
            Assert.IsNull(fixture.Unidad.CategoriaRepo.LastDeleted);
        }

        private sealed class CategoriaFixture
        {
            public readonly FakeCategoriaRepositorio CategoriaRepo = new FakeCategoriaRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;
            public readonly Usuario UsuarioActual;

            public CategoriaFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo { CategoriaRepo = CategoriaRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                UsuarioActual = new Usuario { Id = usuarioActualId, Username = "test" };
                Sesion.IniciarSesion(UsuarioActual);
            }

            public CategoriaBLL CreateSut()
            {
                return new CategoriaBLL(CategoriaRepo, UnidadFactory, Sesion, Bitacora);
            }
        }
    }
}
