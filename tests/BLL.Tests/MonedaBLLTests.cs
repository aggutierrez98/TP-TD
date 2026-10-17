using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class MonedaBLLTests
    {
        [TestMethod]
        public void ObtenerTodas_DelegaAlRepositorio()
        {
            MonedaFixture fixture = new MonedaFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARG", Nombre = "Peso Argentino" });
            fixture.MonedaRepo.Monedas.Add(2, new Moneda { Id = 2, Codigo = "USD", Nombre = "Dólar Estadounidense" });
            MonedaBLL sut = fixture.CreateSut();

            List<Moneda> todas = sut.ObtenerTodas();

            Assert.AreEqual(2, todas.Count);
        }

        [TestMethod]
        public void ObtenerTodas_SinMonedas_DevuelveListaVacia()
        {
            MonedaFixture fixture = new MonedaFixture();
            MonedaBLL sut = fixture.CreateSut();

            List<Moneda> todas = sut.ObtenerTodas();

            Assert.AreEqual(0, todas.Count);
        }

        [TestMethod]
        public void Crear_CodigoVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            MonedaFixture fixture = new MonedaFixture();
            MonedaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(" ", "Peso Argentino"));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            MonedaFixture fixture = new MonedaFixture();
            MonedaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("ARG", null));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_Valido_InsertaNormalizadaAuditaYGuarda()
        {
            MonedaFixture fixture = new MonedaFixture();
            MonedaBLL sut = fixture.CreateSut();

            Moneda creada = sut.Crear(" ARG ", " Peso Argentino ");

            Assert.AreEqual("ARG", creada.Codigo);
            Assert.AreEqual("Peso Argentino", creada.Nombre);
            Assert.AreSame(creada, fixture.Unidad.MonedaRepo.LastInserted);
            Assert.AreEqual(1, fixture.Unidad.SaveCallCount);
            Assert.AreEqual(1, fixture.Bitacora.Calls.Count);
            Assert.AreEqual("FormCurrencyManagement.Value.Activity.MonedaCreation", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreSame(fixture.Unidad, fixture.Bitacora.Calls[0].Unidad);
        }

        [TestMethod]
        public void Actualizar_Inexistente_ThrowsExcepcionNegocio()
        {
            MonedaFixture fixture = new MonedaFixture();
            MonedaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(404, "ARG", "Peso Argentino"));
            Assert.AreEqual(0, fixture.Unidad.SaveCallCount);
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaNormalizadaAuditaYGuarda()
        {
            MonedaFixture fixture = new MonedaFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "OLD", Nombre = "Anterior" });
            MonedaBLL sut = fixture.CreateSut();

            bool actualizada = sut.Actualizar(1, " ARG ", " Peso Argentino ");

            Assert.IsTrue(actualizada);
            Assert.AreEqual("ARG", fixture.Unidad.MonedaRepo.LastUpdated.Codigo);
            Assert.AreEqual("Peso Argentino", fixture.Unidad.MonedaRepo.LastUpdated.Nombre);
            Assert.AreEqual(1, fixture.Unidad.SaveCallCount);
            Assert.AreEqual("FormCurrencyManagement.Value.Activity.MonedaUpdate", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreSame(fixture.Unidad, fixture.Bitacora.Calls[0].Unidad);
        }

        [TestMethod]
        public void Eliminar_Inexistente_ThrowsExcepcionNegocio()
        {
            MonedaFixture fixture = new MonedaFixture();
            MonedaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(404));
            Assert.AreEqual(0, fixture.Unidad.SaveCallCount);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            MonedaFixture fixture = new MonedaFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARG", Nombre = "Peso Argentino" });
            MonedaBLL sut = fixture.CreateSut();

            bool eliminada = sut.Eliminar(1);

            Assert.IsTrue(eliminada);
            Assert.AreEqual(1, fixture.Unidad.MonedaRepo.LastDeletedId);
            Assert.AreEqual(1, fixture.Unidad.SaveCallCount);
            Assert.AreEqual("FormCurrencyManagement.Value.Activity.MonedaDeletion", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreSame(fixture.Unidad, fixture.Bitacora.Calls[0].Unidad);
        }
    }

    internal sealed class MonedaFixture
    {
        public MonedaFixture()
        {
            MonedaRepo = new FakeMonedaRepositorio();
            Unidad = new FakeUnidadDeTrabajo { MonedaRepo = MonedaRepo };
            UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            Bitacora = new SpyBitacoraBLL();
        }

        public FakeMonedaRepositorio MonedaRepo { get; private set; }

        public FakeUnidadDeTrabajo Unidad { get; private set; }

        public FakeUnidadDeTrabajoFactory UnidadFactory { get; private set; }

        public SpyBitacoraBLL Bitacora { get; private set; }

        public MonedaBLL CreateSut()
        {
            return new MonedaBLL(MonedaRepo, UnidadFactory, Bitacora);
        }
    }
}
