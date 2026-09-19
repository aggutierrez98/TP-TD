using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class MonedaBLLTests
    {
        [TestMethod]
        public void ObtenerTodas_DelegaAlRepositorio()
        {
            FakeMonedaRepositorio monedaRepo = new FakeMonedaRepositorio();
            monedaRepo.Monedas.Add(1, new Moneda { Id = 1, Codigo = "ARS", Nombre = "Peso Argentino" });
            monedaRepo.Monedas.Add(2, new Moneda { Id = 2, Codigo = "USD", Nombre = "Dolar Estadounidense" });
            MonedaBLL sut = new MonedaBLL(monedaRepo);

            List<Moneda> todas = sut.ObtenerTodas();

            Assert.AreEqual(2, todas.Count);
        }

        [TestMethod]
        public void ObtenerTodas_SinMonedas_DevuelveListaVacia()
        {
            FakeMonedaRepositorio monedaRepo = new FakeMonedaRepositorio();
            MonedaBLL sut = new MonedaBLL(monedaRepo);

            List<Moneda> todas = sut.ObtenerTodas();

            Assert.AreEqual(0, todas.Count);
        }
    }
}
