using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BE;
using BLL.Tests.TestDoubles;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class IntegridadBLLTests
    {
        [TestMethod]
        public void CalcularDVH_DelegatesToService()
        {
            IntegridadFixture fixture = new IntegridadFixture();
            Usuario usuario = new Usuario { Id = 1, Username = "admin" };
            IntegridadBLL sut = fixture.CreateSut();

            string result = sut.CalcularDVH(usuario);

            Assert.AreEqual("DVH:admin::True", result);
            Assert.AreSame(usuario, fixture.IntegridadService.LastEntity);
        }

        [TestMethod]
        public void RecalcularDVVInterno_NullUnidad_ThrowsArgumentNullException()
        {
            IntegridadFixture fixture = new IntegridadFixture();
            IntegridadBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ArgumentNullException>(() => sut.RecalcularDVVInterno("Usuarios", new[] { "dvh" }, null));
        }

        [TestMethod]
        public void RecalcularDVVInterno_ValidData_UsesUnidadIntegridad()
        {
            IntegridadFixture fixture = new IntegridadFixture();
            fixture.IntegridadService.NextDvv = "new-dvv";
            IntegridadBLL sut = fixture.CreateSut();

            sut.RecalcularDVVInterno("Usuarios", new[] { "a", "b" }, fixture.Unidad);

            Assert.AreEqual("Usuarios", fixture.Unidad.IntegridadRepo.LastTablaGuardada);
            Assert.AreEqual("new-dvv", fixture.Unidad.IntegridadRepo.LastValorGuardado);
            CollectionAssert.AreEqual(new List<string> { "a", "b" }, fixture.IntegridadService.LastDvhList.ToList());
        }

        [TestMethod]
        public void ActualizarDVVInterno_ValidData_ReadsCurrentDvvAndSavesNewDvv()
        {
            IntegridadFixture fixture = new IntegridadFixture();
            fixture.Unidad.IntegridadRepo.DvvPorTabla["Usuarios"] = "current";
            fixture.IntegridadService.NextIncrementalDvv = "updated";
            IntegridadBLL sut = fixture.CreateSut();

            sut.ActualizarDVVInterno("Usuarios", "old", "new", fixture.Unidad);

            Assert.AreEqual("current", fixture.IntegridadService.LastIncrementalArgs.Item1);
            Assert.AreEqual("old", fixture.IntegridadService.LastIncrementalArgs.Item2);
            Assert.AreEqual("new", fixture.IntegridadService.LastIncrementalArgs.Item3);
            Assert.AreEqual("updated", fixture.Unidad.IntegridadRepo.LastValorGuardado);
        }

        [TestMethod]
        public void ValidarIntegridadGlobal_NoDvvAndNoDvh_InitializesSignaturesAndSaves()
        {
            IntegridadFixture fixture = new IntegridadFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true };
            IntegridadBLL sut = fixture.CreateSut();

            sut.ValidarIntegridadGlobal();

            Assert.AreEqual("DVH:admin:hash:True", fixture.Unidad.UsuarioRepo.Usuarios[1].DVH);
            Assert.AreEqual("DVV", fixture.Unidad.IntegridadRepo.LastValorGuardado);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ValidarIntegridadGlobal_HorizontalCorruption_ThrowsExcepcionIntegridad()
        {
            IntegridadFixture fixture = new IntegridadFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, DVH = "wrong" };
            fixture.Unidad.IntegridadRepo.DvvPorTabla["Usuarios"] = "any";
            IntegridadBLL sut = fixture.CreateSut();

            ExcepcionIntegridad ex = Assert.ThrowsExactly<ExcepcionIntegridad>(() => sut.ValidarIntegridadGlobal());

            Assert.AreEqual(TipoFalloIntegridad.Horizontal, ex.Tipo);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void ValidarIntegridadGlobal_VerticalCorruption_ThrowsExcepcionIntegridad()
        {
            IntegridadFixture fixture = new IntegridadFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, DVH = "DVH:admin:hash:True" };
            fixture.Unidad.IntegridadRepo.DvvPorTabla["Usuarios"] = "wrong";
            fixture.IntegridadService.NextDvv = "expected";
            IntegridadBLL sut = fixture.CreateSut();

            ExcepcionIntegridad ex = Assert.ThrowsExactly<ExcepcionIntegridad>(() => sut.ValidarIntegridadGlobal());

            Assert.AreEqual(TipoFalloIntegridad.Vertical, ex.Tipo);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        private sealed class IntegridadFixture
        {
            public readonly FakeIntegridadService IntegridadService = new FakeIntegridadService();
            public readonly FakeIntegridadRepositorio IntegridadRepo = new FakeIntegridadRepositorio();
            public readonly FakeUsuarioRepositorio UsuarioRepo = new FakeUsuarioRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public IntegridadFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public IntegridadBLL CreateSut()
            {
                return new IntegridadBLL(IntegridadService, IntegridadRepo, UsuarioRepo, Bitacora, UnidadFactory);
            }
        }
    }
}
