using System;
using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class TasaConversionBLLTests
    {
        [TestMethod]
        public void ObtenerTodas_DelegaAlRepositorio()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.TasaConversionRepo.Tasas.Add(1, new TasaConversion
            {
                Id = 1,
                MonedaBase = new Moneda { Id = 1, Codigo = "ARS" },
                MonedaConversion = new Moneda { Id = 2, Codigo = "USD" },
                CalculoConversion = 850m,
                FechaActualizacion = DateTime.Now
            });
            TasaConversionBLL sut = fixture.CreateSut();

            List<TasaConversion> todas = sut.ObtenerTodas();

            Assert.AreEqual(1, todas.Count);
        }

        [TestMethod]
        public void Crear_MonedasIguales_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, 1, 850m));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_CalculoInvalido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, 2, 0m));
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, 2, -5m));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_MonedaBaseInexistente_ThrowsExcepcionNegocio()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.MonedaRepo.Monedas.Add(2, new Moneda { Id = 2, Codigo = "USD" });
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, 2, 850m));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_MonedaConversionInexistente_ThrowsExcepcionNegocio()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS" });
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, 2, 850m));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_ParDuplicado_ThrowsExcepcionNegocio()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS" });
            fixture.MonedaRepo.Monedas.Add(2, new Moneda { Id = 2, Codigo = "USD" });
            fixture.Unidad.TasaConversionRepo.Tasas.Add(1, new TasaConversion
            {
                Id = 1,
                MonedaBase = new Moneda { Id = 1 },
                MonedaConversion = new Moneda { Id = 2 },
                CalculoConversion = 800m,
                FechaActualizacion = DateTime.Now
            });
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, 2, 850m));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_Valido_InsertaAuditaYGuarda()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS" });
            fixture.MonedaRepo.Monedas.Add(2, new Moneda { Id = 2, Codigo = "USD" });
            TasaConversionBLL sut = fixture.CreateSut();

            TasaConversion creada = sut.Crear(1, 2, 850.5m);

            TasaConversion insertada = fixture.Unidad.TasaConversionRepo.Tasas[creada.Id];
            Assert.AreEqual(1, insertada.MonedaBase.Id);
            Assert.AreEqual(2, insertada.MonedaConversion.Id);
            Assert.AreEqual(850.5m, insertada.CalculoConversion);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_Inexistente_ThrowsExcepcionNegocio()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(99, 1, 2, 850m));
        }

        [TestMethod]
        public void Actualizar_ManteniendoElMismoPar_NoFallaPorDuplicado()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS" });
            fixture.MonedaRepo.Monedas.Add(2, new Moneda { Id = 2, Codigo = "USD" });
            fixture.Unidad.TasaConversionRepo.Tasas.Add(1, new TasaConversion
            {
                Id = 1,
                MonedaBase = new Moneda { Id = 1 },
                MonedaConversion = new Moneda { Id = 2 },
                CalculoConversion = 800m,
                FechaActualizacion = new DateTime(2026, 1, 1)
            });
            TasaConversionBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(1, 1, 2, 900m);

            Assert.IsTrue(resultado);
            Assert.AreEqual(900m, fixture.Unidad.TasaConversionRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_HaciaParYaUsadoPorOtraTasa_ThrowsExcepcionNegocio()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.MonedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS" });
            fixture.MonedaRepo.Monedas.Add(2, new Moneda { Id = 2, Codigo = "USD" });
            fixture.Unidad.TasaConversionRepo.Tasas.Add(1, new TasaConversion { Id = 1, MonedaBase = new Moneda { Id = 1 }, MonedaConversion = new Moneda { Id = 2 }, CalculoConversion = 800m, FechaActualizacion = DateTime.Now });
            fixture.Unidad.TasaConversionRepo.Tasas.Add(2, new TasaConversion { Id = 2, MonedaBase = new Moneda { Id = 2 }, MonedaConversion = new Moneda { Id = 1 }, CalculoConversion = 0.001m, FechaActualizacion = DateTime.Now });
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(2, 1, 2, 850m));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Inexistente_ThrowsExcepcionNegocio()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            TasaConversionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(99));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            TasaConversionFixture fixture = new TasaConversionFixture();
            fixture.Unidad.TasaConversionRepo.Tasas.Add(1, new TasaConversion { Id = 1, MonedaBase = new Moneda { Id = 1 }, MonedaConversion = new Moneda { Id = 2 }, CalculoConversion = 800m, FechaActualizacion = DateTime.Now });
            TasaConversionBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(1);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.TasaConversionRepo.Tasas.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        private sealed class TasaConversionFixture
        {
            public readonly FakeTasaConversionRepositorio TasaConversionRepo = new FakeTasaConversionRepositorio();
            public readonly FakeMonedaRepositorio MonedaRepo = new FakeMonedaRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public TasaConversionFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { TasaConversionRepo = TasaConversionRepo, MonedaRepo = MonedaRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public TasaConversionBLL CreateSut()
            {
                return new TasaConversionBLL(TasaConversionRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
