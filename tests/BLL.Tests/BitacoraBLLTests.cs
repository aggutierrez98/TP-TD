using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BE;
using BLL.Tests.TestDoubles;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class BitacoraBLLTests
    {
        [TestMethod]
        public void Buscar_NullFilter_UsesDefaultFilter()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            BitacoraBLL sut = fixture.CreateSut();

            sut.Buscar(null);

            Assert.IsNotNull(fixture.BitacoraRepo.LastFiltro);
        }

        [TestMethod]
        public void RegistrarOperacion_CreatesInformacionEvent()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            BitacoraBLL sut = fixture.CreateSut();

            sut.RegistrarOperacion("actividad", "info", "test");

            EventoBitacora evento = fixture.Unidad.BitacoraRepo.Eventos.Single();
            Assert.AreEqual("Informacion", evento.Nivel);
            Assert.IsFalse(evento.EsTecnica);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void RegistrarFalloNegocio_CreatesAdvertenciaEvent()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            BitacoraBLL sut = fixture.CreateSut();

            sut.RegistrarFalloNegocio("actividad", "info", "test");

            EventoBitacora evento = fixture.Unidad.BitacoraRepo.Eventos.Single();
            Assert.AreEqual("Advertencia", evento.Nivel);
            Assert.IsFalse(evento.EsTecnica);
        }

        [TestMethod]
        public void RegistrarExcepcionTecnica_CreatesErrorTechnicalEvent()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            BitacoraBLL sut = fixture.CreateSut();

            sut.RegistrarExcepcionTecnica("actividad", new InvalidOperationException("boom"), "test");

            EventoBitacora evento = fixture.Unidad.BitacoraRepo.Eventos.Single();
            Assert.AreEqual("Error", evento.Nivel);
            Assert.IsTrue(evento.EsTecnica);
            StringAssert.Contains(evento.Informacion, "InvalidOperationException");
        }

        [TestMethod]
        public void RegistrarOperacion_UsesSessionSnapshot()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            fixture.Sesion.IniciarSesion(new Usuario { Id = 4, Username = "admin" });
            BitacoraBLL sut = fixture.CreateSut();

            sut.RegistrarOperacion("actividad", "info", "test");

            EventoBitacora evento = fixture.Unidad.BitacoraRepo.Eventos.Single();
            Assert.AreEqual(4, evento.UsuarioId);
            Assert.AreEqual("admin", evento.UsernameSnapshot);
        }

        [TestMethod]
        public void RegistrarOperacion_UsesUnidadBitacoraAndSave()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            BitacoraBLL sut = fixture.CreateSut();

            sut.RegistrarOperacion("actividad", "info", "test");

            Assert.AreEqual(1, fixture.Unidad.BitacoraRepo.Eventos.Count);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void RegistrarOperacionInterno_NullUnidad_ThrowsArgumentNullException()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            BitacoraBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ArgumentNullException>(() => sut.RegistrarOperacionInterno("actividad", "info", "test", null));
        }

        [TestMethod]
        public void RegistrarOperacionInterno_ValidUnidad_DoesNotCreateOwnUow()
        {
            BitacoraFixture fixture = new BitacoraFixture();
            BitacoraBLL sut = fixture.CreateSut();

            sut.RegistrarOperacionInterno("actividad", "info", "test", fixture.Unidad);

            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual(1, fixture.Unidad.BitacoraRepo.Eventos.Count);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        private sealed class BitacoraFixture
        {
            public readonly FakeBitacoraRepositorio BitacoraRepo = new FakeBitacoraRepositorio();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public BitacoraFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public BitacoraBLL CreateSut()
            {
                return new BitacoraBLL(BitacoraRepo, UnidadFactory, Sesion);
            }
        }
    }
}
