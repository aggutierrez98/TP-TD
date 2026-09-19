using System;
using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class PromocionBLLTests
    {
        [TestMethod]
        public void ObtenerPorComercio_SinPromocionesAsociadas_DevuelveListaVacia()
        {
            PromocionFixture fixture = new PromocionFixture();
            PromocionBLL sut = fixture.CreateSut();

            List<Promocion> resultado = sut.ObtenerPorComercio(1);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void ObtenerPorComercio_ConPromocionesAsociadas_DevuelveSoloLasDelComercioOrdenadasPorVigencia()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.PromocionRepo.Promociones.Add(1, new Promocion { Id = 1, Descripcion = "2x1", Descuento = 50m, VigenciaDesde = new DateTime(2026, 1, 1), VigenciaHasta = new DateTime(2026, 1, 31) });
            fixture.PromocionRepo.Promociones.Add(2, new Promocion { Id = 2, Descripcion = "20% off", Descuento = 20m, VigenciaDesde = new DateTime(2026, 3, 1), VigenciaHasta = new DateTime(2026, 3, 31) });
            fixture.PromocionRepo.Promociones.Add(3, new Promocion { Id = 3, Descripcion = "Otro comercio", Descuento = 10m, VigenciaDesde = new DateTime(2026, 2, 1), VigenciaHasta = new DateTime(2026, 2, 28) });
            fixture.PromocionComercioRepo.PromocionIdsPorComercio[1] = new List<int> { 1, 2 };
            PromocionBLL sut = fixture.CreateSut();

            List<Promocion> resultado = sut.ObtenerPorComercio(1);

            Assert.AreEqual(2, resultado.Count);
            Assert.AreEqual(2, resultado[0].Id);
            Assert.AreEqual(1, resultado[1].Id);
        }

        [TestMethod]
        public void ObtenerPorComercio_IdDePromocionSinRegistro_SeIgnoraDefensivamente()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.PromocionComercioRepo.PromocionIdsPorComercio[1] = new List<int> { 99 };
            PromocionBLL sut = fixture.CreateSut();

            List<Promocion> resultado = sut.ObtenerPorComercio(1);

            Assert.AreEqual(0, resultado.Count);
        }

        private sealed class PromocionFixture
        {
            public readonly FakePromocionRepositorio PromocionRepo = new FakePromocionRepositorio();
            public readonly FakePromocionComercioRepositorio PromocionComercioRepo = new FakePromocionComercioRepositorio();

            public PromocionBLL CreateSut()
            {
                return new PromocionBLL(PromocionRepo, PromocionComercioRepo);
            }
        }
    }
}
