using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class DistanciaGeograficaTests
    {
        private const double RadioTerrestreMetros = 6371008.8;

        [TestMethod]
        public void CalcularMetros_MismoPunto_DevuelveCero()
        {
            double distancia = DistanciaGeografica.CalcularMetros(-34.603722m, -58.381592m, -34.603722m, -58.381592m);

            Assert.AreEqual(0d, distancia, 0.0001);
        }

        [TestMethod]
        public void CalcularMetros_ObeliscoAPlazaDeMayo_DevuelveAlrededorDeUnKilometro()
        {
            // Obelisco (-34.603722, -58.381592) y centro de Plaza de Mayo (-34.608333, -58.372222).
            double distancia = DistanciaGeografica.CalcularMetros(-34.603722m, -58.381592m, -34.608333m, -58.372222m);

            Assert.AreEqual(999.1, distancia, 5);
        }

        [TestMethod]
        public void CalcularMetros_UnGradoDeLatitud_DevuelveElArcoDeUnGrado()
        {
            double distancia = DistanciaGeografica.CalcularMetros(-34m, -58m, -35m, -58m);

            Assert.AreEqual(RadioTerrestreMetros * Math.PI / 180, distancia, 0.01);
        }

        [TestMethod]
        public void CalcularMetros_Antipodas_DevuelveMediaCircunferencia()
        {
            double distancia = DistanciaGeografica.CalcularMetros(10m, 20m, -10m, -160m);

            Assert.AreEqual(Math.PI * RadioTerrestreMetros, distancia, 1);
        }

        [TestMethod]
        public void CalcularMetros_EsSimetrica()
        {
            double ida = DistanciaGeografica.CalcularMetros(-34.603722m, -58.381592m, -31.420083m, -64.188776m);
            double vuelta = DistanciaGeografica.CalcularMetros(-31.420083m, -64.188776m, -34.603722m, -58.381592m);

            Assert.AreEqual(ida, vuelta, 0.0001);
        }
    }
}
