using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using SERVICIOS;
using BE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class RiesgoInversionesTests
    {
        [TestMethod]
        [DataRow(1000, NivelRiesgoInversor.Bajo, null)]
        [DataRow(1000, NivelRiesgoInversor.Moderado, null)]
        [DataRow(1000, NivelRiesgoInversor.Alto, null)]
        [DataRow(950, NivelRiesgoInversor.Bajo, "Bajo")]
        [DataRow(950, NivelRiesgoInversor.Moderado, "Bajo")]
        [DataRow(950, NivelRiesgoInversor.Alto, "Bajo")]
        [DataRow(850, NivelRiesgoInversor.Bajo, "Bajo")]
        [DataRow(850, NivelRiesgoInversor.Moderado, "Moderado")]
        [DataRow(850, NivelRiesgoInversor.Alto, "Moderado")]
        [DataRow(500, NivelRiesgoInversor.Bajo, "Bajo")]
        [DataRow(500, NivelRiesgoInversor.Moderado, "Moderado")]
        [DataRow(500, NivelRiesgoInversor.Alto, "Alto")]
        public async Task CalcularAsync_CruceDePerfilYSaldo_NuncaElevaRiesgo(int gasto, NivelRiesgoInversor perfil, string esperado)
        {
            var f = new InversionFixture(gasto);
            var r = await f.Crear().CalcularAsync(new Usuario { Id = 7 }, 20, CancellationToken.None, PerfilAsistenteTests.Perfil(perfil));
            Assert.AreEqual(esperado, r.RiesgoEfectivo?.ToString());
            Assert.AreEqual(perfil, r.Perfil);
            if (esperado == null) Assert.IsEmpty(r.Instrumentos);
            else
            {
                Assert.Contains("mercadoDinero", f.Mercado.Llamadas);
                Assert.Contains("cuentas", f.Mercado.Llamadas);
                Assert.AreEqual(esperado != "Bajo", f.Mercado.Llamadas.Contains("rentaFija"));
                Assert.AreEqual(esperado == "Alto", f.Mercado.Llamadas.Contains("acciones"));
                Assert.AreEqual(esperado == "Alto", f.Mercado.Llamadas.Contains("BTC"));
                Assert.AreEqual(gasto != 950, f.Mercado.Llamadas.Contains("plazos"));
            }
        }

        [TestMethod]
        [DataRow("A", "C", 0, 0, true)]
        [DataRow("A", "C", 0, 1, false)]
        [DataRow("B", "C", 3, 0, true)]
        [DataRow("B", "C", 4, 0, false)]
        [DataRow("C", "C", 12, 0, true)]
        [DataRow("C", "C", 13, 0, false)]
        [DataRow("D", "C", 36, 0, true)]
        [DataRow("D", "C", 37, 0, false)]
        [DataRow("D", "A", 0, 0, true)]
        [DataRow("D", "A", 0, 1, false)]
        [DataRow("D", "B", 0, 1, true)]
        [DataRow("D", "B", 0, 2, false)]
        [DataRow("D", "B", 1, 0, false)]
        public async Task CalcularAsync_CompatibilidadVerificada_AplicaAmbosTopesSinDistribuir(string horizonte, string liquidez, int meses, int dias, bool permitido)
        {
            var f = new InversionFixture(500);
            var p = PerfilAsistenteTests.Perfil(NivelRiesgoInversor.Bajo);
            p.Respuestas.Single(x => x.Pregunta.Id == 2).OpcionId = horizonte;
            p.Respuestas.Single(x => x.Pregunta.Id == 3).OpcionId = liquidez;
            f.Mercado.Fondos = new System.Collections.Generic.List<DatoMercado>
            {
                new DatoMercado { Nombre = "Fondo verificable", Tipo = "FCI mercadoDinero", Vcp = 100, Fecha = f.Hoy,
                    MesesInmovilizacion = meses, DiasHabilesDisponibilidad = dias, RequierePreaviso = false, RequierePermanencia = false,
                    FuenteCondiciones = "Fixture verificable v1" }
            };
            var r = await f.Crear().CalcularAsync(new Usuario { Id = 7 }, 20, CancellationToken.None, p);
            Assert.AreEqual(permitido, r.Instrumentos.Any(x => x.Dato.Nombre == "Fondo verificable"));
            if (horizonte == "A" || liquidez != "C") Assert.DoesNotContain("plazos", f.Mercado.Llamadas);
        }

        [TestMethod]
        [DataRow("metadatos")]
        [DataRow("preaviso")]
        [DataRow("permanencia")]
        [DataRow("procedencia")]
        public async Task CalcularAsync_LiquidezNoComprobable_ExcluyeYExplica(string caso)
        {
            var f = new InversionFixture(500);
            var p = PerfilAsistenteTests.Perfil(NivelRiesgoInversor.Bajo);
            p.Respuestas.Single(x => x.Pregunta.Id == 3).OpcionId = "A";
            var d = new DatoMercado { Nombre = "Fondo dudoso", Tipo = "FCI mercadoDinero", Vcp = 100, Fecha = f.Hoy,
                MesesInmovilizacion = 0, DiasHabilesDisponibilidad = 0, RequierePreaviso = false, RequierePermanencia = false, FuenteCondiciones = "Fixture v1" };
            if (caso == "metadatos") d.DiasHabilesDisponibilidad = null;
            if (caso == "preaviso") d.RequierePreaviso = true;
            if (caso == "permanencia") d.RequierePermanencia = true;
            if (caso == "procedencia") d.FuenteCondiciones = null;
            f.Mercado.Fondos = new System.Collections.Generic.List<DatoMercado> { d };
            var r = await f.Crear().CalcularAsync(new Usuario { Id = 7 }, 20, CancellationToken.None, p);
            Assert.IsFalse(r.Instrumentos.Any(x => x.Dato.Nombre == "Fondo dudoso"));
            Assert.Contains("Fondo dudoso", r.CandidatosSinCompatibilidad);
        }

        [TestMethod]
        public async Task CalcularAsync_FuenteCaida_NoConsultaFamiliasDeMayorRiesgoYContextoNoExponeRespuestas()
        {
            var f = new InversionFixture(500);
            f.Mercado.FallaTodo = true;
            var r = await f.Crear().CalcularAsync(new Usuario { Id = 7 }, 20, CancellationToken.None, PerfilAsistenteTests.Perfil(NivelRiesgoInversor.Bajo));
            Assert.IsEmpty(r.Instrumentos);
            Assert.DoesNotContain("acciones", f.Mercado.Llamadas);
            Assert.DoesNotContain("BTC", f.Mercado.Llamadas);
            string json = r.CrearContexto().DatosJson;
            Assert.Contains("\"Perfil\":\"Bajo\"", json);
            Assert.Contains("FechaPerfilUtc", json);
            Assert.Contains("VersionPerfil", json);
            Assert.DoesNotContain("Respuestas", json);
            Assert.DoesNotContain("Capacidad", json);
            Assert.AreEqual(ResultadoValidacionRespuesta.ReferenciaNoPermitida,
                new ValidadorRespuestaAsistente().Validar("Compra acciones y BTC", r.UniversoReferencias, r.CrearContexto()));
        }

        [TestMethod]
        public async Task CalcularAsync_ProveedorRealSinMetadatos_NoInventaLiquidezYExplicaLimitacion()
        {
            var f = new InversionFixture(500);
            f.Mercado.Fondos = new System.Collections.Generic.List<DatoMercado>
            {
                new DatoMercado { Nombre = "Sin condiciones", Tipo = "FCI mercadoDinero", Fecha = f.Hoy, Vcp = 100 }
            };
            var r = await f.Crear().CalcularAsync(new Usuario { Id = 7 }, 20, CancellationToken.None, PerfilAsistenteTests.Perfil(NivelRiesgoInversor.Bajo));
            Assert.Contains("Sin condiciones", r.CandidatosSinCompatibilidad);
            Assert.IsFalse(r.CrearContexto().ReferenciasPermitidas.Any(x => x.Nombre == "Sin condiciones"));
        }

        [TestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        [DataRow(2, false)]
        public async Task CalcularAsync_RescateEnDiasSinTopeDeDias_RequiereFechaMaximaVerificada(int caso, bool esperado)
        {
            var f = new InversionFixture(500);
            f.Mercado.Fondos = new System.Collections.Generic.List<DatoMercado>
            {
                new DatoMercado { Nombre = "Rescate con calendario", Tipo = "FCI mercadoDinero", Fecha = f.Hoy, Vcp = 100,
                    MesesInmovilizacion = 0, DiasHabilesDisponibilidad = 2, RequierePreaviso = false, RequierePermanencia = false,
                    FuenteCondiciones = "Fixture calendario v1", FechaDisponibilidadMaxima = caso == 0 ? (DateTime?)null : caso == 1 ? f.Hoy.AddDays(4) : f.Hoy.AddMonths(37) }
            };
            var r = await f.Crear().CalcularAsync(new Usuario { Id = 7 }, 20, CancellationToken.None, PerfilAsistenteTests.Perfil(NivelRiesgoInversor.Bajo));
            Assert.AreEqual(esperado, r.Instrumentos.Any(x => x.Dato.Nombre == "Rescate con calendario"));
        }
    }
}
